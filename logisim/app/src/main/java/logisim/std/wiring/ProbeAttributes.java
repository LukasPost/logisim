/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.Font;
import java.util.Arrays;
import java.util.List;

import logisim.circuit.RadixOption;
import logisim.data.AbstractAttributeSet;
import logisim.data.Attribute;
import logisim.data.BitWidth;
import logisim.data.Direction;
import logisim.instance.StdAttr;

class ProbeAttributes extends AbstractAttributeSet {
	public static ProbeAttributes instance = new ProbeAttributes();

	private static final List<Attribute<?>> ATTRIBUTES = Arrays.asList(new Attribute<?>[] { StdAttr.FACING,
			RadixOption.ATTRIBUTE, StdAttr.LABEL, Pin.ATTR_LABEL_LOC, StdAttr.LABEL_FONT, });

	Direction facing = Direction.East;
	String label = "";
	Direction labelloc = Direction.West;
	Font labelfont = StdAttr.DEFAULT_LABEL_FONT;
	RadixOption radix = RadixOption.RADIX_2;
	BitWidth width = BitWidth.ONE;

	public ProbeAttributes() {
	}

	@Override
	protected void copyInto(AbstractAttributeSet destObj) {
		; // nothing to do
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return ATTRIBUTES;
	}

	@Override
	@SuppressWarnings("unchecked")
	public <E> E getValue(Attribute<E> attr) {
		if (attr == StdAttr.FACING)
			return (E) facing;
		if (attr == StdAttr.LABEL)
			return (E) label;
		if (attr == Pin.ATTR_LABEL_LOC)
			return (E) labelloc;
		if (attr == StdAttr.LABEL_FONT)
			return (E) labelfont;
		if (attr == RadixOption.ATTRIBUTE)
			return (E) radix;
		return null;
	}

	@Override
	public <V> void setValue(Attribute<V> attr, V value) {
		if (attr == StdAttr.FACING) {
			facing = (Direction) value;
		} else if (attr == StdAttr.LABEL) {
			label = (String) value;
		} else if (attr == Pin.ATTR_LABEL_LOC) {
			labelloc = (Direction) value;
		} else if (attr == StdAttr.LABEL_FONT) {
			labelfont = (Font) value;
		} else if (attr == RadixOption.ATTRIBUTE) {
			radix = (RadixOption) value;
		} else {
			throw new IllegalArgumentException("unknown attribute");
		}
		fireAttributeValueChanged(attr, value);
	}
}
