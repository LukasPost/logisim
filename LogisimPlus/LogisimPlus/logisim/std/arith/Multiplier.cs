// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

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

	public class Multiplier : InstanceFactory
	{
		internal const int PER_DELAY = 1;

		private const int IN0 = 0;
		private const int IN1 = 1;
		private const int OUT = 2;
		private const int C_IN = 3;
		private const int C_OUT = 4;

		public Multiplier() : base("Multiplier", Strings.getter("multiplierComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.WIDTH}, new object[] {BitWidth.create(8)});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);
			OffsetBounds = Bounds.create(-40, -20, 40, 40);
			IconName = "multiplier.gif";

			Port[] ps = new Port[5];
			ps[IN0] = new Port(-40, -10, Port.INPUT, StdAttr.WIDTH);
			ps[IN1] = new Port(-40, 10, Port.INPUT, StdAttr.WIDTH);
			ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			ps[C_IN] = new Port(-20, -20, Port.INPUT, StdAttr.WIDTH);
			ps[C_OUT] = new Port(-20, 20, Port.OUTPUT, StdAttr.WIDTH);
			ps[IN0].setToolTip(Strings.getter("multiplierInputTip"));
			ps[IN1].setToolTip(Strings.getter("multiplierInputTip"));
			ps[OUT].setToolTip(Strings.getter("multiplierOutputTip"));
			ps[C_IN].setToolTip(Strings.getter("multiplierCarryInTip"));
			ps[C_OUT].setToolTip(Strings.getter("multiplierCarryOutTip"));
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
			Value[] outs = Multiplier.computeProduct(dataWidth, a, b, c_in);

			// propagate them
			int delay = dataWidth.Width * (dataWidth.Width + 2) * PER_DELAY;
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
			g.drawLine(x - 15, y - 5, x - 5, y + 5);
			g.drawLine(x - 15, y + 5, x - 5, y - 5);
			GraphicsUtil.switchToWidth(g, 1);
		}

		internal static Value[] computeProduct(BitWidth width, Value a, Value b, Value c_in)
		{
			int w = width.Width;
			if (c_in == Value.NIL || c_in.Unknown)
			{
				c_in = Value.createKnown(width, 0);
			}
			if (a.FullyDefined && b.FullyDefined && c_in.FullyDefined)
			{
				long sum = (long) a.toIntValue() * (long) b.toIntValue() + (long) c_in.toIntValue();
				return new Value[] {Value.createKnown(width, (int) sum), Value.createKnown(width, (int)(sum >> w))};
			}
			else
			{
				Value[] avals = a.All;
				int aOk = findUnknown(avals);
				int aErr = findError(avals);
				int ax = getKnown(avals);
				Value[] bvals = b.All;
				int bOk = findUnknown(bvals);
				int bErr = findError(bvals);
				int bx = getKnown(bvals);
				Value[] cvals = c_in.All;
				int cOk = findUnknown(cvals);
				int cErr = findError(cvals);
				int cx = getKnown(cvals);

				int known = Math.Min(Math.Min(aOk, bOk), cOk);
				int error = Math.Min(Math.Min(aErr, bErr), cErr);
				int ret = ax * bx + cx;

				Value[] bits = new Value[w];
				for (int i = 0; i < w; i++)
				{
					if (i < known)
					{
						bits[i] = ((ret & (1 << i)) != 0 ? Value.TRUE : Value.FALSE);
					}
					else if (i < error)
					{
						bits[i] = Value.UNKNOWN;
					}
					else
					{
						bits[i] = Value.ERROR;
					}
				}
				return new Value[] {Value.create(bits), error < w ? Value.createError(width) : Value.createUnknown(width)};
			}
		}

		private static int findUnknown(Value[] vals)
		{
			for (int i = 0; i < vals.Length; i++)
			{
				if (!vals[i].FullyDefined)
				{
					return i;
				}
			}
			return vals.Length;
		}

		private static int findError(Value[] vals)
		{
			for (int i = 0; i < vals.Length; i++)
			{
				if (vals[i].ErrorValue)
				{
					return i;
				}
			}
			return vals.Length;
		}

		private static int getKnown(Value[] vals)
		{
			int ret = 0;
			for (int i = 0; i < vals.Length; i++)
			{
				int val = vals[i].toIntValue();
				if (val < 0)
				{
					return ret;
				}
				ret |= val << i;
			}
			return ret;
		}
	}

}
