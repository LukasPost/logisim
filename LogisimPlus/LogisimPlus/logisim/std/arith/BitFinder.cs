// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.arith
{

	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	public class BitFinder : InstanceFactory
	{
		internal static readonly AttributeOption LOW_ONE = new AttributeOption("low1", Strings.getter("bitFinderLowOption", "1"));
		internal static readonly AttributeOption HIGH_ONE = new AttributeOption("high1", Strings.getter("bitFinderHighOption", "1"));
		internal static readonly AttributeOption LOW_ZERO = new AttributeOption("low0", Strings.getter("bitFinderLowOption", "0"));
		internal static readonly AttributeOption HIGH_ZERO = new AttributeOption("high0", Strings.getter("bitFinderHighOption", "0"));
		internal static readonly Attribute TYPE = Attributes.forOption("type", Strings.getter("bitFinderTypeAttr"), new AttributeOption[] {LOW_ONE, HIGH_ONE, LOW_ZERO, HIGH_ZERO});

		public BitFinder() : base("BitFinder", Strings.getter("bitFinderComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.Width, TYPE}, new object[] {BitWidth.create(8), LOW_ONE});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
			IconName = "bitfindr.gif";
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			return Bounds.create(-40, -20, 40, 40);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			configurePorts(instance);
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.Width)
			{
				configurePorts(instance);
			}
			else if (attr == TYPE)
			{
				instance.fireInvalidated();
			}
		}

		private void configurePorts(Instance instance)
		{
			BitWidth inWidth = instance.getAttributeValue(StdAttr.Width);
			int outWidth = computeOutputBits(inWidth.Width - 1);

			Port[] ps = new Port[3];
			ps[0] = new Port(-20, 20, Port.OUTPUT, BitWidth.ONE);
			ps[1] = new Port(0, 0, Port.OUTPUT, BitWidth.create(outWidth));
			ps[2] = new Port(-40, 0, Port.INPUT, inWidth);

			object type = instance.getAttributeValue(TYPE);
			if (type == HIGH_ZERO)
			{
				ps[0].setToolTip(Strings.getter("bitFinderPresentTip", "0"));
				ps[1].setToolTip(Strings.getter("bitFinderIndexHighTip", "0"));
			}
			else if (type == LOW_ZERO)
			{
				ps[0].setToolTip(Strings.getter("bitFinderPresentTip", "0"));
				ps[1].setToolTip(Strings.getter("bitFinderIndexLowTip", "0"));
			}
			else if (type == HIGH_ONE)
			{
				ps[0].setToolTip(Strings.getter("bitFinderPresentTip", "1"));
				ps[1].setToolTip(Strings.getter("bitFinderIndexHighTip", "1"));
			}
			else
			{
				ps[0].setToolTip(Strings.getter("bitFinderPresentTip", "1"));
				ps[1].setToolTip(Strings.getter("bitFinderIndexLowTip", "1"));
			}
			ps[2].setToolTip(Strings.getter("bitFinderInputTip"));
			instance.setPorts(ps);
		}

		private int computeOutputBits(int maxBits)
		{
			int outWidth = 1;
			while ((1 << outWidth) <= maxBits)
			{
				outWidth++;
			}
			return outWidth;
		}

		public override void propagate(InstanceState state)
		{
			int width = state.getAttributeValue(StdAttr.Width).getWidth();
			int outWidth = computeOutputBits(width - 1);
			object type = state.getAttributeValue(TYPE);

			Value[] bits = state.getPort(2).All;
			Value want;
			int i;
			if (type == HIGH_ZERO)
			{
				want = Value.FALSE;
				for (i = bits.Length - 1; i >= 0 && bits[i] == Value.TRUE; i--)
				{
				}
			}
			else if (type == LOW_ZERO)
			{
				want = Value.FALSE;
				for (i = 0; i < bits.Length && bits[i] == Value.TRUE; i++)
				{
				}
			}
			else if (type == HIGH_ONE)
			{
				want = Value.TRUE;
				for (i = bits.Length - 1; i >= 0 && bits[i] == Value.FALSE; i--)
				{
				}
			}
			else
			{
				want = Value.TRUE;
				for (i = 0; i < bits.Length && bits[i] == Value.FALSE; i++)
				{
				}
			}

			Value present;
			Value index;
			if (i < 0 || i >= bits.Length)
			{
				present = Value.FALSE;
				index = Value.createKnown(BitWidth.create(outWidth), 0);
			}
			else if (bits[i] == want)
			{
				present = Value.TRUE;
				index = Value.createKnown(BitWidth.create(outWidth), i);
			}
			else
			{
				present = Value.ERROR;
				index = Value.createError(BitWidth.create(outWidth));
			}

			int delay = outWidth * Adder.PER_DELAY;
			state.setPort(0, present, delay);
			state.setPort(1, index, delay);
		}

		public override void paintInstance(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			painter.drawBounds();
			painter.drawPorts();

			string top = Strings.get("bitFinderFindLabel");
			string mid;
			string bot;
			object type = painter.getAttributeValue(TYPE);
			if (type == HIGH_ZERO)
			{
				mid = Strings.get("bitFinderHighLabel");
				bot = "0";
			}
			else if (type == LOW_ZERO)
			{
				mid = Strings.get("bitFinderLowLabel");
				bot = "0";
			}
			else if (type == HIGH_ONE)
			{
				mid = Strings.get("bitFinderHighLabel");
				bot = "1";
			}
			else
			{
				mid = Strings.get("bitFinderLowLabel");
				bot = "1";
			}

			Bounds bds = painter.Bounds;
			int x = bds.X + bds.Width / 2;
			int y0 = bds.Y;
			JGraphicsUtil.drawCenteredText(g, top, x, y0 + 8);
			JGraphicsUtil.drawCenteredText(g, mid, x, y0 + 20);
			JGraphicsUtil.drawCenteredText(g, bot, x, y0 + 32);
		}
	}

}
