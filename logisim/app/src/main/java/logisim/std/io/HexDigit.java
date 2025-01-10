/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.io;

import java.awt.Color;

import logisim.data.Attribute;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.InstanceDataSingleton;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;

public class HexDigit extends InstanceFactory {
	public HexDigit() {
		super("Hex Digit Display", Strings.getter("hexDigitComponent"));
		setAttributes(new Attribute[] { Io.ATTR_ON_COLOR, Io.ATTR_OFF_COLOR, Io.ATTR_BACKGROUND },
				new Object[] { new Color(240, 0, 0), SevenSegment.DEFAULT_OFF, Io.DEFAULT_BACKGROUND });
		setPorts(new Port[] { new Port(0, 0, Port.INPUT, 4), new Port(10, 0, Port.INPUT, 1) });
		setOffsetBounds(Bounds.create(-15, -60, 40, 60));
		setIconName("hexdig.gif");
	}

	@Override
	public void propagate(InstanceState state) {
		int summary = 0;
		WireValue baseVal = state.getPort(0);
		if (baseVal == null)
			baseVal = WireValue.Companion.createUnknown(BitWidth.create(4));
		int segs = switch (baseVal.toIntValue()) {
			case 0 -> 0x1110111;
			case 1 -> 0x0000011;
			case 2 -> 0x0111110;
			case 3 -> 0x0011111;
			case 4 -> 0x1001011;
			case 5 -> 0x1011101;
			case 6 -> 0x1111101;
			case 7 -> 0x0010011;
			case 8 -> 0x1111111;
			case 9 -> 0x1011011;
			case 10 -> 0x1111011;
			case 11 -> 0x1101101;
			case 12 -> 0x1110100;
			case 13 -> 0x0101111;
			case 14 -> 0x1111100;
			case 15 -> 0x1111000;
			default -> 0x0001000; // a dash '-'
		}; // each nibble is one segment, in top-down, left-to-right
		// order: middle three nibbles are the three horizontal segments
		if ((segs & 0x1) != 0)
			summary |= 4; // vertical seg in bottom right
		if ((segs & 0x10) != 0)
			summary |= 2; // vertical seg in top right
		if ((segs & 0x100) != 0)
			summary |= 8; // horizontal seg at bottom
		if ((segs & 0x1000) != 0)
			summary |= 64; // horizontal seg at middle
		if ((segs & 0x10000) != 0)
			summary |= 1; // horizontal seg at top
		if ((segs & 0x100000) != 0)
			summary |= 16; // vertical seg at bottom left
		if ((segs & 0x1000000) != 0)
			summary |= 32; // vertical seg at top left
		if (state.getPort(1) == WireValues.TRUE)
			summary |= 128;

		Object value = summary;
		InstanceDataSingleton data = (InstanceDataSingleton) state.getData();
		if (data == null) state.setData(new InstanceDataSingleton(value));
		else data.setValue(value);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		SevenSegment.drawBase(painter);
	}
}
