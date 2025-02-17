/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;

import javax.swing.AbstractListModel;
import javax.swing.DefaultListCellRenderer;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.ListSelectionModel;

import logisim.comp.Component;
import logisim.data.WireValue.WireValue;

class SelectionList extends JList<SelectionItem> {
	private class Model extends AbstractListModel<SelectionItem> implements ModelListener {
		public int getSize() {
			return selection == null ? 0 : selection.size();
		}

		public SelectionItem getElementAt(int index) {
			return selection.get(index);
		}

		public void selectionChanged(ModelEvent event) {
			fireContentsChanged(this, 0, getSize());
		}

		public void entryAdded(ModelEvent event, WireValue[] values) {
		}

		public void filePropertyChanged(ModelEvent event) {
		}
	}

	private static class MyCellRenderer extends DefaultListCellRenderer {
		@Override
		public java.awt.Component getListCellRendererComponent(JList<?> list, Object value, int index, boolean isSelected, boolean hasFocus) {
			java.awt.Component ret = super.getListCellRendererComponent(list, value, index, isSelected, hasFocus);
			if (ret instanceof JLabel label && value instanceof SelectionItem item) {
				Component comp = item.getComponent();
				label.setIcon(new ComponentIcon(comp));
				label.setText(item + " - " + item.getRadix());
			}
			return ret;
		}
	}

	private Selection selection;

	public SelectionList() {
		selection = null;
		setModel(new Model());
		setCellRenderer(new MyCellRenderer());
		setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
	}

	public void setSelection(Selection value) {
		if (selection != value) {
			Model model = (Model) getModel();
			if (selection != null)
				selection.removeModelListener(model);
			selection = value;
			if (selection != null)
				selection.addModelListener(model);
			model.selectionChanged(null);
		}
	}

	public void localeChanged() {
		repaint();
	}
}
