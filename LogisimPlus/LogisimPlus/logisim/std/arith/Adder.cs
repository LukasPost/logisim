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
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class Adder : InstanceFactory
	{
		internal const int PER_DELAY = 1;

		private const int IN0 = 0;
		private const int IN1 = 1;
		private const int OUT = 2;
		private const int C_IN = 3;
		private const int C_OUT = 4;

		public Adder() : base("Adder", Strings.getter("adderComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.WIDTH}, new object[] {BitWidth.create(8)});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);
			OffsetBounds = Bounds.create(-40, -20, 40, 40);
			IconName = "adder.gif";

			Port[] ps = new Port[5];
			ps[IN0] = new Port(-40, -10, Port.INPUT, StdAttr.WIDTH);
			ps[IN1] = new Port(-40, 10, Port.INPUT, StdAttr.WIDTH);
			ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			ps[C_IN] = new Port(-20, -20, Port.INPUT, 1);
			ps[C_OUT] = new Port(-20, 20, Port.INPUT, 1);
			ps[IN0].setToolTip(Strings.getter("adderInputTip"));
			ps[IN1].setToolTip(Strings.getter("adderInputTip"));
			ps[OUT].setToolTip(Strings.getter("adderOutputTip"));
			ps[C_IN].setToolTip(Strings.getter("adderCarryInTip"));
			ps[C_OUT].setToolTip(Strings.getter("adderCarryOutTip"));
			setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			// get attributes
			BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);

			// compute outputs
			Value a = state.getPort(IN0);
			Value b = state.getPort(IN1);
			Value c_in = state.getPort(C_IN);
			Value[] outs = Adder.computeSum(dataWidth, a, b, c_in);

			// propagate them
			int delay = (dataWidth.Width + 2) * PER_DELAY;
			state.setPort(OUT, outs[0], delay);
			state.setPort(C_OUT, outs[1], delay);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			painter.drawBounds();

			g.setColor(Color.GRAY);
			painter.drawPort(IN0);
			painter.drawPort(IN1);
			painter.drawPort(OUT);
			painter.drawPort(C_IN, "c in", Direction.North);
			painter.drawPort(C_OUT, "c out", Direction.South);

			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			GraphicsUtil.switchToWidth(g, 2);
			g.setColor(Color.BLACK);
			g.drawLine(x - 15, y, x - 5, y);
			g.drawLine(x - 10, y - 5, x - 10, y + 5);
			GraphicsUtil.switchToWidth(g, 1);
		}

		internal static Value[] computeSum(BitWidth width, Value a, Value b, Value c_in)
		{
			int w = width.Width;
			if (c_in == Value.UNKNOWN || c_in == Value.NIL)
			{
				c_in = Value.FALSE;
			}
			if (a.FullyDefined && b.FullyDefined && c_in.FullyDefined)
			{
				if (w >= 32)
				{
					long mask = (1L << w) - 1;
					long ax = (long) a.toIntValue() & mask;
					long bx = (long) b.toIntValue() & mask;
					long cx = (long) c_in.toIntValue() & mask;
					long sum = ax + bx + cx;
					return new Value[] {Value.createKnown(width, (int) sum), ((sum >> w) & 1) == 0 ? Value.FALSE : Value.TRUE};
				}
				else
				{
					int sum = a.toIntValue() + b.toIntValue() + c_in.toIntValue();
					return new Value[] {Value.createKnown(width, sum), ((sum >> w) & 1) == 0 ? Value.FALSE : Value.TRUE};
				}
			}
			else
			{
				Value[] bits = new Value[w];
				Value carry = c_in;
				for (int i = 0; i < w; i++)
				{
					if (carry == Value.ERROR)
					{
						bits[i] = Value.ERROR;
					}
					else if (carry == Value.UNKNOWN)
					{
						bits[i] = Value.UNKNOWN;
					}
					else
					{
						Value ab = a.get(i);
						Value bb = b.get(i);
						if (ab == Value.ERROR || bb == Value.ERROR)
						{
							bits[i] = Value.ERROR;
							carry = Value.ERROR;
						}
						else if (ab == Value.UNKNOWN || bb == Value.UNKNOWN)
						{
							bits[i] = Value.UNKNOWN;
							carry = Value.UNKNOWN;
						}
						else
						{
							int sum = (ab == Value.TRUE ? 1 : 0) + (bb == Value.TRUE ? 1 : 0) + (carry == Value.TRUE ? 1 : 0);
							bits[i] = (sum & 1) == 1 ? Value.TRUE : Value.FALSE;
							carry = (sum >= 2) ? Value.TRUE : Value.FALSE;
						}
					}
				}
				return new Value[] {Value.create(bits), carry};
			}
		}
	}

}
