/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.gui;

import java.awt.event.ItemListener;

import javax.swing.AbstractListModel;
import javax.swing.ComboBoxModel;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;

import logisim.analyze.model.AnalyzerModel;
import logisim.analyze.model.VariableList;
import logisim.analyze.model.VariableListEvent;
import logisim.analyze.model.VariableListListener;

class OutputSelector {
	private class Model extends AbstractListModel<String> implements ComboBoxModel<String>, VariableListListener {
		private String selected;

		public void setSelectedItem(Object value) {
			selected = (String)value;
		}

		public Object getSelectedItem() {
			return selected;
		}

		public int getSize() {
			return source.size();
		}

		public String getElementAt(int index) {
			return source.get(index);
		}

		public void listChanged(VariableListEvent event) {
			int index;
			String variable;
			Object selection;
			switch (event.getType()) {
			case VariableListEvent.ALL_REPLACED:
				computePrototypeValue();
				fireContentsChanged(this, 0, getSize());
				if (source.isEmpty()) {
					select.setSelectedItem(null);
				} else {
					select.setSelectedItem(source.get(0));
				}
				break;
			case VariableListEvent.ADD:
				variable = event.getVariable();
				if (prototypeValue == null || variable.length() > prototypeValue.length()) {
					computePrototypeValue();
				}

				index = source.indexOf(variable);
				fireIntervalAdded(this, index, index);
				if (select.getSelectedItem() == null) {
					select.setSelectedItem(variable);
				}
				break;
			case VariableListEvent.REMOVE:
				variable = event.getVariable();
				if (variable.equals(prototypeValue))
					computePrototypeValue();
				index = ((Integer) event.getData()).intValue();
				fireIntervalRemoved(this, index, index);
				selection = select.getSelectedItem();
				if (selection != null && selection.equals(variable)) {
					selection = source.isEmpty() ? null : source.get(0);
					select.setSelectedItem(selection);
				}
				break;
			case VariableListEvent.MOVE:
				fireContentsChanged(this, 0, getSize());
				break;
			case VariableListEvent.REPLACE:
				variable = event.getVariable();
				if (variable.equals(prototypeValue))
					computePrototypeValue();
				index = ((Integer) event.getData()).intValue();
				fireContentsChanged(this, index, index);
				selection = select.getSelectedItem();
				if (selection != null && selection.equals(variable)) {
					select.setSelectedItem(event.getSource().get(index));
				}
				break;
			}
		}
	}

	private VariableList source;
	private JLabel label = new JLabel();
	private JComboBox<String> select = new JComboBox<>();
	private String prototypeValue = null;

	public OutputSelector(AnalyzerModel model) {
		this.source = model.getOutputs();

		Model listModel = new Model();
		select.setModel(listModel);
		source.addVariableListListener(listModel);
	}

	public JPanel createPanel() {
		JPanel ret = new JPanel();
		ret.add(label);
		ret.add(select);
		return ret;
	}

	public JLabel getLabel() {
		return label;
	}

	public JComboBox<String> getComboBox() {
		return select;
	}

	void localeChanged() {
		label.setText(Strings.get("outputSelectLabel"));
	}

	public void addItemListener(ItemListener l) {
		select.addItemListener(l);
	}

	public void removeItemListener(ItemListener l) {
		select.removeItemListener(l);
	}

	public String getSelectedOutput() {
		String value = (String) select.getSelectedItem();
		if (value != null && !source.contains(value)) {
			if (source.isEmpty()) {
				value = null;
			} else {
				value = source.get(0);
			}
			select.setSelectedItem(value);
		}
		return value;
	}

	private void computePrototypeValue() {
		String newValue;
		if (source.isEmpty()) {
			newValue = "xx";
		} else {
			newValue = "xx";
			for (int i = 0, n = source.size(); i < n; i++) {
				String candidate = source.get(i);
				if (candidate.length() > newValue.length())
					newValue = candidate;
			}
		}
		if (prototypeValue == null || newValue.length() != prototypeValue.length()) {
			prototypeValue = newValue;
			select.setPrototypeDisplayValue(prototypeValue + "xx");
			select.revalidate();
		}
	}
}
