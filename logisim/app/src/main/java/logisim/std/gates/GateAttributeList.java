/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.util.AbstractList;

import logisim.data.Attribute;
import logisim.data.Direction;
import logisim.instance.StdAttr;

class GateAttributeList extends AbstractList<Attribute<?>> {
	private static final Attribute<?>[] BASE_ATTRIBUTES = { StdAttr.FACING, StdAttr.WIDTH, GateAttributes.ATTR_SIZE,
			GateAttributes.ATTR_INPUTS, GateAttributes.ATTR_OUTPUT, StdAttr.LABEL, StdAttr.LABEL_FONT, };

	private GateAttributes attrs;

	public GateAttributeList(GateAttributes attrs) {
		this.attrs = attrs;
	}

	@Override
	public Attribute<?> get(int index) {
		int len = BASE_ATTRIBUTES.length;
		if (index < len) return BASE_ATTRIBUTES[index];
		index -= len;
		if (attrs.xorBehave != null) {
			index--;
			if (index < 0)
				return GateAttributes.ATTR_XOR;
		}
		Direction facing = attrs.facing;
		int inputs = attrs.inputs;
		if (index == 0) if (facing == Direction.East || facing == Direction.West)
			return new NegateAttribute(index, Direction.North);
		else return new NegateAttribute(index, Direction.West);
		else if (index == inputs - 1) if (facing == Direction.East || facing == Direction.West)
			return new NegateAttribute(index, Direction.South);
		else return new NegateAttribute(index, Direction.East);
		else if (index < inputs) return new NegateAttribute(index, null);
		return null;
	}

	@Override
	public int size() {
		int ret = BASE_ATTRIBUTES.length;
		if (attrs.xorBehave != null)
			ret++;
		ret += attrs.inputs;
		return ret;
	}
}
