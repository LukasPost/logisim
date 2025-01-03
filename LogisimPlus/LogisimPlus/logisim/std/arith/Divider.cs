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
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	public class Divider : InstanceFactory
	{
		internal const int PER_DELAY = 1;

		private const int IN0 = 0;
		private const int IN1 = 1;
		private const int OUT = 2;
		private const int UPPER = 3;
		private const int REM = 4;

		public Divider() : base("Divider", Strings.getter("dividerComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.Width}, new object[] {BitWidth.create(8)});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
			OffsetBounds = Bounds.create(-40, -20, 40, 40);
			IconName = "divider.gif";

			Port[] ps = new Port[5];
			ps[IN0] = new Port(-40, -10, Port.INPUT, StdAttr.Width);
			ps[IN1] = new Port(-40, 10, Port.INPUT, StdAttr.Width);
			ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.Width);
			ps[UPPER] = new Port(-20, -20, Port.INPUT, StdAttr.Width);
			ps[REM] = new Port(-20, 20, Port.OUTPUT, StdAttr.Width);
			ps[IN0].setToolTip(Strings.getter("dividerDividendLowerTip"));
			ps[IN1].setToolTip(Strings.getter("dividerDivisorTip"));
			ps[OUT].setToolTip(Strings.getter("dividerOutputTip"));
			ps[UPPER].setToolTip(Strings.getter("dividerDividendUpperTip"));
			ps[REM].setToolTip(Strings.getter("dividerRemainderTip"));
			setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			// get attributes
			BitWidth dataWidth = state.getAttributeValue(StdAttr.Width);

			// compute outputs
			Value a = state.getPort(IN0);
			Value b = state.getPort(IN1);
			Value upper = state.getPort(UPPER);
			Value[] outs = Divider.computeResult(dataWidth, a, b, upper);

			// propagate them
			int delay = dataWidth.Width * (dataWidth.Width + 2) * PER_DELAY;
			state.setPort(OUT, outs[0], delay);
			state.setPort(REM, outs[1], delay);
		}

		public override void paintInstance(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			painter.drawBounds();

			g.setColor(Color.Gray);
			painter.drawPort(IN0);
			painter.drawPort(IN1);
			painter.drawPort(OUT);
			painter.drawPort(UPPER, Strings.get("dividerUpperInput"), Direction.North);
			painter.drawPort(REM, Strings.get("dividerRemainderOutput"), Direction.South);

			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			JGraphicsUtil.switchToWidth(g, 2);
			g.setColor(Color.Black);
			g.fillOval(x - 12, y - 7, 4, 4);
			g.drawLine(x - 15, y, x - 5, y);
			g.fillOval(x - 12, y + 3, 4, 4);
			JGraphicsUtil.switchToWidth(g, 1);
		}

		internal static Value[] computeResult(BitWidth width, Value a, Value b, Value upper)
		{
			int w = width.Width;
			if (upper == Value.NIL || upper.Unknown)
			{
				upper = Value.createKnown(width, 0);
			}
			if (a.FullyDefined && b.FullyDefined && upper.FullyDefined)
			{
				long num = ((long) upper.toIntValue() << w) | ((long) a.toIntValue() & 0xFFFFFFFFL);
				long den = (long) b.toIntValue() & 0xFFFFFFFFL;
				if (den == 0)
				{
					den = 1;
				}
				long result = num / den;
				long rem = num % den;
				if (rem < 0)
				{
					if (den >= 0)
					{
						rem += den;
						result--;
					}
					else
					{
						rem -= den;
						result++;
					}
				}
				return new Value[] {Value.createKnown(width, (int) result), Value.createKnown(width, (int) rem)};
			}
			else if (a.ErrorValue || b.ErrorValue || upper.ErrorValue)
			{
				return new Value[] {Value.createError(width), Value.createError(width)};
			}
			else
			{
				return new Value[] {Value.createUnknown(width), Value.createUnknown(width)};
			}
		}
	}

}
