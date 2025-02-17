/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.opts;

import java.awt.Component;
import java.awt.Graphics;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

import javax.swing.AbstractListModel;
import javax.swing.DefaultListCellRenderer;
import javax.swing.Icon;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.ListSelectionModel;

import logisim.comp.ComponentDrawContext;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.file.ToolbarData;
import logisim.file.ToolbarData.ToolbarListener;
import logisim.prefs.AppPreferences;
import logisim.tools.Tool;

class ToolbarList extends JList {
	private static class ToolIcon implements Icon {
		private Tool tool;

		ToolIcon(Tool tool) {
			this.tool = tool;
		}

		public void paintIcon(Component comp, Graphics g, int x, int y) {
			Graphics gNew = g.create();
			tool.paintIcon(new ComponentDrawContext(comp, null, null, g, gNew), x + 2, y + 2);
			gNew.dispose();
		}

		public int getIconWidth() {
			return 20;
		}

		public int getIconHeight() {
			return 20;
		}
	}

	private static class ListRenderer extends DefaultListCellRenderer {
		@Override
		public Component getListCellRendererComponent(JList list, Object value, int index, boolean isSelected, boolean cellHasFocus) {
			Component ret;
			Icon icon;
			if (value instanceof Tool t) {
				ret = super.getListCellRendererComponent(list, t.getDisplayName(), index, isSelected, cellHasFocus);
				icon = new ToolIcon(t);
			} else if (value == null) {
				ret = super.getListCellRendererComponent(list, "---", index, isSelected, cellHasFocus);
				icon = null;
			} else {
				ret = super.getListCellRendererComponent(list, value.toString(), index, isSelected, cellHasFocus);
				icon = null;
			}
			if (ret instanceof JLabel) ((JLabel) ret).setIcon(icon);
			return ret;
		}
	}

	private class Model extends AbstractListModel
			implements ToolbarListener, AttributeListener, PropertyChangeListener {
		public int getSize() {
			return base.size();
		}

		public Object getElementAt(int index) {
			return base.get(index);
		}

		public void toolbarChanged() {
			fireContentsChanged(this, 0, getSize());
		}

		public void attributeListChanged(AttributeEvent e) {
		}

		public void attributeValueChanged(AttributeEvent e) {
			repaint();
		}

		public void propertyChange(PropertyChangeEvent event) {
			if (AppPreferences.GATE_SHAPE.isSource(event)) repaint();
		}
	}

	private ToolbarData base;
	private Model model;

	public ToolbarList(ToolbarData base) {
		this.base = base;
		model = new Model();

		setModel(model);
		setCellRenderer(new ListRenderer());
		setSelectionMode(ListSelectionModel.SINGLE_SELECTION);

		AppPreferences.GATE_SHAPE.addPropertyChangeListener(model);
		base.addToolbarListener(model);
		base.addToolAttributeListener(model);
	}

	public void localeChanged() {
		model.toolbarChanged();
	}
}
