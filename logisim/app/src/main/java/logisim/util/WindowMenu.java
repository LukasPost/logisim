/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

import java.awt.Dimension;
import java.awt.Frame;
import java.awt.Point;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.util.ArrayList;

import javax.swing.ButtonGroup;
import javax.swing.JFrame;
import javax.swing.JMenu;
import javax.swing.JMenuItem;
import javax.swing.JRadioButtonMenuItem;
import javax.swing.KeyStroke;

public class WindowMenu extends JMenu {
	private class MyListener implements LocaleListener, ActionListener {
		public void localeChanged() {
			setText(Strings.get("windowMenu"));
			minimize.setText(Strings.get("windowMinimizeItem"));
			close.setText(Strings.get("windowCloseItem"));
			zoom.setText(MacCompatibility.isQuitAutomaticallyPresent() ? Strings.get("windowZoomItemMac")
					: Strings.get("windowZoomItem"));
		}

		public void actionPerformed(ActionEvent e) {
			Object src = e.getSource();
			if (src == minimize) doMinimize();
			else if (src == zoom) doZoom();
			else if (src == close) doClose();
			else if (src instanceof WindowMenuItem choice) if (choice.isSelected()) {
				WindowMenuItem item = findOwnerItem();
				if (item != null) item.setSelected(true);
				choice.actionPerformed();
			}
		}

		private WindowMenuItem findOwnerItem() {
			for (WindowMenuItem i : persistentItems)
				if (i.getJFrame() == owner)
					return i;
			for (WindowMenuItem i : transientItems)
				if (i.getJFrame() == owner)
					return i;
			return null;
		}
	}

	private JFrame owner;
	private MyListener myListener = new MyListener();
	private JMenuItem minimize = new JMenuItem();
	private JMenuItem zoom = new JMenuItem();
	private JMenuItem close = new JMenuItem();
	private JRadioButtonMenuItem nullItem = new JRadioButtonMenuItem();
	private ArrayList<WindowMenuItem> persistentItems = new ArrayList<>();
	private ArrayList<WindowMenuItem> transientItems = new ArrayList<>();

	public WindowMenu(JFrame owner) {
		this.owner = owner;
		WindowMenuManager.addMenu(this);

		int menuMask = getToolkit().getMenuShortcutKeyMaskEx();
		minimize.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_M, menuMask));
		close.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_W, menuMask));

		if (owner == null) {
			minimize.setEnabled(false);
			zoom.setEnabled(false);
			close.setEnabled(false);
		} else {
			minimize.addActionListener(myListener);
			zoom.addActionListener(myListener);
			close.addActionListener(myListener);
		}

		computeEnabled();
		computeContents();

		LocaleManager.addLocaleListener(myListener);
		myListener.localeChanged();
	}

	void addMenuItem(WindowMenuItem item, boolean persistent) {
		if (persistent)
			persistentItems.add(item);
		else
			transientItems.add(item);
		item.addActionListener(myListener);
		computeContents();
	}

	void removeMenuItem(JRadioButtonMenuItem item) {
		if (transientItems.remove((WindowMenuItem) item)) item.removeActionListener(myListener);
		computeContents();
	}

	void computeEnabled() {
		WindowMenuItemManager currentManager = WindowMenuManager.getCurrentManager();
		minimize.setEnabled(currentManager != null);
		zoom.setEnabled(currentManager != null);
		close.setEnabled(currentManager != null);
	}

	void setNullItemSelected(boolean value) {
		nullItem.setSelected(value);
	}

	private void computeContents() {
		ButtonGroup bgroup = new ButtonGroup();
		bgroup.add(nullItem);

		removeAll();
		add(minimize);
		add(zoom);
		add(close);

		if (!persistentItems.isEmpty()) {
			addSeparator();
			for (JRadioButtonMenuItem item : persistentItems) {
				bgroup.add(item);
				add(item);
			}
		}

		if (!transientItems.isEmpty()) {
			addSeparator();
			for (JRadioButtonMenuItem item : transientItems) {
				bgroup.add(item);
				add(item);
			}
		}

		WindowMenuItemManager currentManager = WindowMenuManager.getCurrentManager();
		if (currentManager != null) {
			JRadioButtonMenuItem item = currentManager.getMenuItem(this);
			if (item != null) item.setSelected(true);
		}
	}

	void doMinimize() {
		if (owner != null) owner.setExtendedState(Frame.ICONIFIED);
	}

	void doClose() {
		if (owner instanceof WindowClosable) ((WindowClosable) owner).requestClose();
		else if (owner != null) {
			int action = owner.getDefaultCloseOperation();
			if (action == JFrame.EXIT_ON_CLOSE) System.exit(0);
			else if (action == JFrame.HIDE_ON_CLOSE) owner.setVisible(false);
			else if (action == JFrame.DISPOSE_ON_CLOSE) owner.dispose();
		}
	}

	void doZoom() {
		if (owner == null)
			return;

		owner.pack();
		Dimension screenSize = owner.getToolkit().getScreenSize();
		Dimension windowSize = owner.getPreferredSize();
		Point windowLoc = owner.getLocation();

		boolean locChanged = false;
		boolean sizeChanged = false;
		if (windowLoc.x + windowSize.width > screenSize.width) {
			windowLoc.x = Math.max(0, screenSize.width - windowSize.width);
			locChanged = true;
			if (windowLoc.x + windowSize.width > screenSize.width) {
				windowSize.width = screenSize.width - windowLoc.x;
				sizeChanged = true;
			}
		}
		if (windowLoc.y + windowSize.height > screenSize.height) {
			windowLoc.y = Math.max(0, screenSize.height - windowSize.height);
			locChanged = true;
			if (windowLoc.y + windowSize.height > screenSize.height) {
				windowSize.height = screenSize.height - windowLoc.y;
				sizeChanged = true;
			}
		}

		if (locChanged)
			owner.setLocation(windowLoc);
		if (sizeChanged)
			owner.setSize(windowSize);
	}
}
