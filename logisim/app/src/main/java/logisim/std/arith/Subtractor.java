/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.arith;

import java.awt.Color;
import java.awt.Graphics;

import logisim.data.Attribute;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.tools.key.BitWidthConfigurator;
import logisim.util.GraphicsUtil;

public class Subtractor extends InstanceFactory {
	private static final int IN0 = 0;
	private static final int IN1 = 1;
	private static final int OUT = 2;
	private static final int B_IN = 3;
	private static final int B_OUT = 4;

	public Subtractor() {
		super("Subtractor", Strings.getter("subtractorComponent"));
		setAttributes(new Attribute[] { StdAttr.WIDTH }, new Object[] { BitWidth.create(8) });
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setOffsetBounds(Bounds.create(-40, -20, 40, 40));
		setIconName("subtractor.gif");

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

	@Override
	public void propagate(InstanceState state) {
		// get attributes
		BitWidth data = state.getAttributeValue(StdAttr.WIDTH);

		// compute outputs
		WireValue a = state.getPort(IN0);
		WireValue b = state.getPort(IN1);
		WireValue b_in = state.getPort(B_IN);
		if (b_in == WireValues.UNKNOWN || b_in == WireValues.NIL)
			b_in = WireValues.FALSE;
		WireValue[] outs = Adder.computeSum(data, a, b.not(), b_in.not());

		// propagate them
		int delay = (data.getWidth() + 4) * Adder.PER_DELAY;
		state.setPort(OUT, outs[0], delay);
		state.setPort(B_OUT, outs[1].not(), delay);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		painter.drawBounds();

		g.setColor(Color.GRAY);
		painter.drawPort(IN0);
		painter.drawPort(IN1);
		painter.drawPort(OUT);
		painter.drawPort(B_IN, "b in", Direction.North);
		painter.drawPort(B_OUT, "b out", Direction.South);

		Location loc = painter.getLocation();
		int x = loc.x();
		int y = loc.y();
		GraphicsUtil.switchToWidth(g, 2);
		g.setColor(Color.BLACK);
		g.drawLine(x - 15, y, x - 5, y);
		GraphicsUtil.switchToWidth(g, 1);
	}
}
