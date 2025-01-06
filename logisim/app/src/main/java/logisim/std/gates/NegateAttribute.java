/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import logisim.data.Attribute;
import logisim.data.Attributes;
import logisim.data.Direction;
import logisim.util.StringUtil;

import java.awt.Component;

class NegateAttribute extends Attribute<Boolean> {
	private static Attribute<Boolean> BOOLEAN_ATTR = Attributes.forBoolean("negateDummy");

	int index;
	private Direction side;

	public NegateAttribute(int index, Direction side) {
		super("negate" + index, null);
		this.index = index;
		this.side = side;
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof NegateAttribute o && index == o.index && side == o.side;
	}

	@Override
	public int hashCode() {
		return index * 31 + (side == null ? 0 : side.hashCode());
	}

	@Override
	public String getDisplayName() {
		String ret = StringUtil.format(Strings.get("gateNegateAttr"), "" + (index + 1));
		if (side != null) ret += " (" + side.toVerticalDisplayString() + ")";
		return ret;
	}

	@Override
	public String toDisplayString(Boolean value) {
		return BOOLEAN_ATTR.toDisplayString(value);
	}

	@Override
	public Boolean parse(String value) {
		return BOOLEAN_ATTR.parse(value);
	}

	@Override
	public Component getCellEditor(Boolean value) {
		return BOOLEAN_ATTR.getCellEditor(null, value);
	}

}
