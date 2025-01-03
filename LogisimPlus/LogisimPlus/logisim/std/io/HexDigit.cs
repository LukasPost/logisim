// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using logisim.data;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Value = logisim.data.Value;
	using InstanceDataSingleton = logisim.instance.InstanceDataSingleton;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;

	public class HexDigit : InstanceFactory
	{
		public HexDigit() : base("Hex Digit Display", Strings.getter("hexDigitComponent"))
		{
			setAttributes(new Attribute[] {Io.ATTR_ON_COLOR, Io.ATTR_OFF_COLOR, Io.ATTR_BACKGROUND}, new object[] {Color.FromArgb(255, 240, 0, 0), SevenSegment.DEFAULT_OFF, Io.DEFAULT_BACKGROUND});
			setPorts(new Port[]
			{
				new Port(0, 0, Port.INPUT, 4),
				new Port(10, 0, Port.INPUT, 1)
			});
			OffsetBounds = Bounds.create(-15, -60, 40, 60);
			IconName = "hexdig.gif";
		}

		public override void propagate(InstanceState state)
		{
			int summary = 0;
			Value baseVal = state.getPort(0);
			if (baseVal == null)
			{
				baseVal = Value.createUnknown(BitWidth.create(4));
			}
			int segs; // each nibble is one segment, in top-down, left-to-right
			// order: middle three nibbles are the three horizontal segments
			switch (baseVal.toIntValue())
			{
			case 0:
				segs = 0x1110111;
				break;
			case 1:
				segs = 0x0000011;
				break;
			case 2:
				segs = 0x0111110;
				break;
			case 3:
				segs = 0x0011111;
				break;
			case 4:
				segs = 0x1001011;
				break;
			case 5:
				segs = 0x1011101;
				break;
			case 6:
				segs = 0x1111101;
				break;
			case 7:
				segs = 0x0010011;
				break;
			case 8:
				segs = 0x1111111;
				break;
			case 9:
				segs = 0x1011011;
				break;
			case 10:
				segs = 0x1111011;
				break;
			case 11:
				segs = 0x1101101;
				break;
			case 12:
				segs = 0x1110100;
				break;
			case 13:
				segs = 0x0101111;
				break;
			case 14:
				segs = 0x1111100;
				break;
			case 15:
				segs = 0x1111000;
				break;
			default:
				segs = 0x0001000;
				break; // a dash '-'
			}
			if ((segs & 0x1) != 0)
			{
				summary |= 4; // vertical seg in bottom right
			}
			if ((segs & 0x10) != 0)
			{
				summary |= 2; // vertical seg in top right
			}
			if ((segs & 0x100) != 0)
			{
				summary |= 8; // horizontal seg at bottom
			}
			if ((segs & 0x1000) != 0)
			{
				summary |= 64; // horizontal seg at middle
			}
			if ((segs & 0x10000) != 0)
			{
				summary |= 1; // horizontal seg at top
			}
			if ((segs & 0x100000) != 0)
			{
				summary |= 16; // vertical seg at bottom left
			}
			if ((segs & 0x1000000) != 0)
			{
				summary |= 32; // vertical seg at top left
			}
			if (state.getPort(1) == Value.TRUE)
			{
				summary |= 128;
			}

			object value = Convert.ToInt32(summary);
			InstanceDataSingleton data = (InstanceDataSingleton) state.Data;
			if (data == null)
			{
				state.Data = new InstanceDataSingleton(value);
			}
			else
			{
				data.Value = value;
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			SevenSegment.drawBase(painter);
		}
	}

}
