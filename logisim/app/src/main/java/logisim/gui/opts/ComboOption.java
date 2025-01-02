/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.opts;

import javax.swing.JComboBox;

import logisim.data.AttributeOption;
import logisim.util.StringGetter;

class ComboOption {
	private AttributeOption value;
	private StringGetter getter;

	ComboOption(AttributeOption value) {
		this.value = value;
		this.getter = null;
	}

	@Override
	public String toString() {
		if (getter != null)
			return getter.get();
		return value.toDisplayString();
	}

	public AttributeOption getValue() {
		return value;
	}

	static void setSelected(JComboBox<ComboOption> combo, AttributeOption value) {
		for (int i = combo.getItemCount() - 1; i >= 0; i--) {
			ComboOption opt = combo.getItemAt(i);
			if (opt.getValue().equals(value)) {
				combo.setSelectedItem(opt);
				return;
			}
		}
		combo.setSelectedItem(combo.getItemAt(0));
	}

}
