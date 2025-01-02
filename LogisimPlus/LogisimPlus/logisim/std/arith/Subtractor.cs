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

	public class Subtractor : InstanceFactory
	{
		private const int IN0 = 0;
		private const int IN1 = 1;
		private const int OUT = 2;
		private const int B_IN = 3;
		private const int B_OUT = 4;

		public Subtractor() : base("Subtractor", Strings.getter("subtractorComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.WIDTH}, new object[] {BitWidth.create(8)});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);
			OffsetBounds = Bounds.create(-40, -20, 40, 40);
			IconName = "subtractor.gif";

			Port[] ps = new Port[5];
			ps[IN0] = new Port(-40, -10, Port.INPUT, StdAttr.WIDTH);
			ps[IN1] = new Port(-40, 10, Port.INPUT, StdAttr.WIDTH);
			ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			ps[B_IN] = new Port(-20, -20, Port.INPUT, 1);
			ps[B_OUT] = new Port(-20, 20, Port.OUTPUT, 1);
			ps[IN0].setToolTip(Strings.getter("subtractorMinuendTip"));
			ps[IN1].setToolTip(Strings.getter("subtractorSubtrahendTip"));
			ps[OUT].setToolTip(Strings.getter("subtractorOutputTip"));
			ps[B_IN].setToolTip(Strings.getter("subtractorBorrowInTip"));
			ps[B_OUT].setToolTip(Strings.getter("subtractorBorrowOutTip"));
			setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			// get attributes
			BitWidth data = state.getAttributeValue(StdAttr.WIDTH);

			// compute outputs
			Value a = state.getPort(IN0);
			Value b = state.getPort(IN1);
			Value b_in = state.getPort(B_IN);
			if (b_in == Value.UNKNOWN || b_in == Value.NIL)
			{
				b_in = Value.FALSE;
			}
			Value[] outs = Adder.computeSum(data, a, b.not(), b_in.not());

			// propagate them
			int delay = (data.Width + 4) * Adder.PER_DELAY;
			state.setPort(OUT, outs[0], delay);
			state.setPort(B_OUT, outs[1].not(), delay);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			painter.drawBounds();

			g.setColor(Color.GRAY);
			painter.drawPort(IN0);
			painter.drawPort(IN1);
			painter.drawPort(OUT);
			painter.drawPort(B_IN, "b in", Direction.North);
			painter.drawPort(B_OUT, "b out", Direction.South);

			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			GraphicsUtil.switchToWidth(g, 2);
			g.setColor(Color.BLACK);
			g.drawLine(x - 15, y, x - 5, y);
			GraphicsUtil.switchToWidth(g, 1);
		}
	}

}
