/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

import java.awt.Frame;

import javax.swing.JFrame;
import javax.swing.JRadioButtonMenuItem;

class WindowMenuItem extends JRadioButtonMenuItem {
	private WindowMenuItemManager manager;

	WindowMenuItem(WindowMenuItemManager manager) {
		this.manager = manager;
		setText(manager.getText());
		setSelected(WindowMenuManager.getCurrentManager() == manager);
	}

	public JFrame getJFrame() {
		return manager.getJFrame(true);
	}

	public void actionPerformed() {
		JFrame frame = getJFrame();
		frame.setExtendedState(Frame.NORMAL);
		frame.setVisible(true);
		frame.toFront();
	}
}
