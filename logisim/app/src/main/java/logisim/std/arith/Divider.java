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

public class Divider extends InstanceFactory {
	static final int PER_DELAY = 1;

	private static final int IN0 = 0;
	private static final int IN1 = 1;
	private static final int OUT = 2;
	private static final int UPPER = 3;
	private static final int REM = 4;

	public Divider() {
		super("Divider", Strings.getter("dividerComponent"));
		setAttributes(new Attribute[] { StdAttr.WIDTH }, new Object[] { BitWidth.create(8) });
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setOffsetBounds(Bounds.create(-40, -20, 40, 40));
		setIconName("divider.gif");

		Port[] ps = new Port[5];
		ps[IN0] = new Port(-40, -10, Port.INPUT, StdAttr.WIDTH);
		ps[IN1] = new Port(-40, 10, Port.INPUT, StdAttr.WIDTH);
		ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
		ps[UPPER] = new Port(-20, -20, Port.INPUT, StdAttr.WIDTH);
		ps[REM] = new Port(-20, 20, Port.OUTPUT, StdAttr.WIDTH);
		ps[IN0].setToolTip(Strings.getter("dividerDividendLowerTip"));
		ps[IN1].setToolTip(Strings.getter("dividerDivisorTip"));
		ps[OUT].setToolTip(Strings.getter("dividerOutputTip"));
		ps[UPPER].setToolTip(Strings.getter("dividerDividendUpperTip"));
		ps[REM].setToolTip(Strings.getter("dividerRemainderTip"));
		setPorts(ps);
	}

	@Override
	public void propagate(InstanceState state) {
		// get attributes
		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);

		// compute outputs
		WireValue a = state.getPort(IN0);
		WireValue b = state.getPort(IN1);
		WireValue upper = state.getPort(UPPER);
		WireValue[] outs = Divider.computeResult(dataWidth, a, b, upper);

		// propagate them
		int delay = dataWidth.getWidth() * (dataWidth.getWidth() + 2) * PER_DELAY;
		state.setPort(OUT, outs[0], delay);
		state.setPort(REM, outs[1], delay);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		painter.drawBounds();

		g.setColor(Color.GRAY);
		painter.drawPort(IN0);
		painter.drawPort(IN1);
		painter.drawPort(OUT);
		painter.drawPort(UPPER, Strings.get("dividerUpperInput"), Direction.North);
		painter.drawPort(REM, Strings.get("dividerRemainderOutput"), Direction.South);

		Location loc = painter.getLocation();
		int x = loc.x();
		int y = loc.y();
		GraphicsUtil.switchToWidth(g, 2);
		g.setColor(Color.BLACK);
		g.fillOval(x - 12, y - 7, 4, 4);
		g.drawLine(x - 15, y, x - 5, y);
		g.fillOval(x - 12, y + 3, 4, 4);
		GraphicsUtil.switchToWidth(g, 1);
	}

	static WireValue[] computeResult(BitWidth width, WireValue a, WireValue b, WireValue upper) {
		int w = width.getWidth();
		if (upper == WireValues.NIL || upper.isUnknown())
			upper = WireValue.Companion.createKnown(width, 0);
		if (a.isFullyDefined() && b.isFullyDefined() && upper.isFullyDefined()) {
			long num = ((long) upper.toIntValue() << w) | ((long) a.toIntValue() & 0xFFFFFFFFL);
			long den = (long) b.toIntValue() & 0xFFFFFFFFL;
			if (den == 0)
				den = 1;
			long result = num / den;
			long rem = num % den;
			if (rem < 0) {
				rem += den;
				result--;
			}
			return new WireValue[] { WireValue.Companion.createKnown(width, (int) result), WireValue.Companion.createKnown(width, (int) rem) };
		} else if (a.isErrorValue() || b.isErrorValue() || upper.isErrorValue())
			return new WireValue[]{WireValue.Companion.createError(width), WireValue.Companion.createError(width)};
		else return new WireValue[]{WireValue.Companion.createUnknown(width), WireValue.Companion.createUnknown(width)};
	}
}
