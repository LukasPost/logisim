/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.prefs;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;

import logisim.prefs.PrefMonitor;
import logisim.util.StringGetter;

class PrefOptionList implements ActionListener, PropertyChangeListener {
	private PrefMonitor<String> pref;
	private StringGetter labelStr;

	private JLabel label;
	private JComboBox<PrefOption> combo;

	public PrefOptionList(PrefMonitor<String> pref, StringGetter labelStr, PrefOption[] options) {
		this.pref = pref;
		this.labelStr = labelStr;

		label = new JLabel(labelStr.get() + " ");
		combo = new JComboBox<>();
		for (PrefOption opt : options) combo.addItem(opt);

		combo.addActionListener(this);
		pref.addPropertyChangeListener(this);
		selectOption(pref.get());
	}

	JPanel createJPanel() {
		JPanel ret = new JPanel();
		ret.add(label);
		ret.add(combo);
		return ret;
	}

	JLabel getJLabel() {
		return label;
	}

	JComboBox<PrefOption> getJComboBox() {
		return combo;
	}

	void localeChanged() {
		label.setText(labelStr.get() + " ");
	}

	public void actionPerformed(ActionEvent e) {
		PrefOption x = (PrefOption) combo.getSelectedItem();
		pref.set((String) x.getValue());
	}

	public void propertyChange(PropertyChangeEvent event) {
		if (pref.isSource(event)) selectOption(pref.get());
	}

	private void selectOption(Object value) {
		PrefOption.setSelected(combo, value);
	}
}
