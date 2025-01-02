/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.prefs;

import javax.swing.JComboBox;

class PrefOption {
	private Object value;
	private String displayString;

	PrefOption(String value, String displayString) {
		this.value = value;
		this.displayString = displayString;
	}

	@Override
	public String toString() {
		return displayString;
	}

	public Object getValue() {
		return value;
	}

	static void setSelected(JComboBox<PrefOption> combo, Object value) {
		for (int i = combo.getItemCount() - 1; i >= 0; i--) {
			PrefOption opt = combo.getItemAt(i);
			if (opt.getValue().equals(value)) {
				combo.setSelectedItem(opt);
				return;
			}
		}
		combo.setSelectedItem(combo.getItemAt(0));
	}

}
