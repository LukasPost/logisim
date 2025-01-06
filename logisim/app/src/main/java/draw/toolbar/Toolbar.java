/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.toolbar;

import java.awt.BorderLayout;

import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.JPanel;

public class Toolbar extends JPanel {
	public static final Object VERTICAL = new Object();
	public static final Object HORIZONTAL = new Object();

	private class MyListener implements ToolbarModelListener {
		public void toolbarAppearanceChanged(ToolbarModelEvent event) {
			repaint();
		}

		public void toolbarContentsChanged(ToolbarModelEvent event) {
			computeContents();
		}
	}

	private ToolbarModel model;
	private JPanel subpanel;
	private Object orientation;
	private MyListener myListener;
	private ToolbarButton curPressed;

	public Toolbar(ToolbarModel model) {
		super(new BorderLayout());
		subpanel = new JPanel();
		this.model = model;
		orientation = HORIZONTAL;
		myListener = new MyListener();
		curPressed = null;

		add(new JPanel(), BorderLayout.CENTER);
		setOrientation(HORIZONTAL);

		computeContents();
		if (model != null)
			model.addToolbarModelListener(myListener);
	}

	public ToolbarModel getToolbarModel() {
		return model;
	}

	public void setToolbarModel(ToolbarModel value) {
		ToolbarModel oldValue = model;
		if (value != oldValue) {
			if (oldValue != null)
				oldValue.removeToolbarModelListener(myListener);
			if (value != null)
				value.addToolbarModelListener(myListener);
			model = value;
			computeContents();
		}
	}

	public void setOrientation(Object value) {
		int axis;
		String position;
		if (value == HORIZONTAL) {
			axis = BoxLayout.X_AXIS;
			position = BorderLayout.LINE_START;
		} else if (value == VERTICAL) {
			axis = BoxLayout.Y_AXIS;
			position = BorderLayout.NORTH;
		} else throw new IllegalArgumentException();
		remove(subpanel);
		subpanel.setLayout(new BoxLayout(subpanel, axis));
		add(subpanel, position);
		orientation = value;
	}

	private void computeContents() {
		subpanel.removeAll();
		ToolbarModel m = model;
		if (m != null) {
			for (ToolbarItem item : m.getItems()) subpanel.add(new ToolbarButton(this, item));
			subpanel.add(Box.createGlue());
		}
		revalidate();
	}

	ToolbarButton getPressed() {
		return curPressed;
	}

	void setPressed(ToolbarButton value) {
		ToolbarButton oldValue = curPressed;
		if (oldValue != value) {
			curPressed = value;
			if (oldValue != null)
				oldValue.repaint();
			if (value != null)
				value.repaint();
		}
	}

	Object getOrientation() {
		return orientation;
	}
}
