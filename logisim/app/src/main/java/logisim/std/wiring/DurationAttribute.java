/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import javax.swing.JTextField;

import logisim.data.Attribute;
import logisim.util.StringGetter;
import logisim.util.StringUtil;

public class DurationAttribute extends Attribute<Integer> {
	private int min;
	private int max;

	public DurationAttribute(String name, StringGetter disp, int min, int max) {
		super(name, disp);
		this.min = min;
		this.max = max;
	}

	@Override
	public Integer parse(String value) {
		try {
			Integer ret = Integer.valueOf(value);
			if (ret.intValue() < min) {
				throw new NumberFormatException(StringUtil.format(Strings.get("durationSmallMessage"), "" + min));
			} else if (ret.intValue() > max) {
				throw new NumberFormatException(StringUtil.format(Strings.get("durationLargeMessage"), "" + max));
			}
			return ret;
		}
		catch (NumberFormatException e) {
			throw new NumberFormatException(Strings.get("freqInvalidMessage"));
		}
	}

	@Override
	public String toDisplayString(Integer value) {
		if (value.equals(Integer.valueOf(1))) {
			return Strings.get("clockDurationOneValue");
		} else {
			return StringUtil.format(Strings.get("clockDurationValue"), value.toString());
		}
	}

	@Override
	public java.awt.Component getCellEditor(Integer value) {
		JTextField field = new JTextField();
		field.setText(value.toString());
		return field;
	}

}
