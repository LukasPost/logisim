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

public class Adder extends InstanceFactory {
	static final int PER_DELAY = 1;

	private static final int IN0 = 0;
	private static final int IN1 = 1;
	private static final int OUT = 2;
	private static final int C_IN = 3;
	private static final int C_OUT = 4;

	public Adder() {
		super("Adder", Strings.getter("adderComponent"));
		setAttributes(new Attribute[] { StdAttr.WIDTH }, new Object[] { BitWidth.create(8) });
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setOffsetBounds(Bounds.create(-40, -20, 40, 40));
		setIconName("adder.gif");

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

	@Override
	public void propagate(InstanceState state) {
		// get attributes
		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);

		// compute outputs
		WireValue a = state.getPort(IN0);
		WireValue b = state.getPort(IN1);
		WireValue c_in = state.getPort(C_IN);
		WireValue[] outs = Adder.computeSum(dataWidth, a, b, c_in);

		// propagate them
		int delay = (dataWidth.getWidth() + 2) * PER_DELAY;
		state.setPort(OUT, outs[0], delay);
		state.setPort(C_OUT, outs[1], delay);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		painter.drawBounds();

		g.setColor(Color.GRAY);
		painter.drawPort(IN0);
		painter.drawPort(IN1);
		painter.drawPort(OUT);
		painter.drawPort(C_IN, "c in", Direction.North);
		painter.drawPort(C_OUT, "c out", Direction.South);

		Location loc = painter.getLocation();
		int x = loc.x();
		int y = loc.y();
		GraphicsUtil.switchToWidth(g, 2);
		g.setColor(Color.BLACK);
		g.drawLine(x - 15, y, x - 5, y);
		g.drawLine(x - 10, y - 5, x - 10, y + 5);
		GraphicsUtil.switchToWidth(g, 1);
	}

	static WireValue[] computeSum(BitWidth width, WireValue a, WireValue b, WireValue c_in) {
		int w = width.getWidth();
		if (c_in == WireValues.UNKNOWN || c_in == WireValues.NIL) c_in = WireValues.FALSE;
		if (a.isFullyDefined() && b.isFullyDefined() && c_in.isFullyDefined()) if (w >= 32) {
			long mask = (1L << w) - 1;
			long ax = (long) a.toIntValue() & mask;
			long bx = (long) b.toIntValue() & mask;
			long cx = (long) c_in.toIntValue() & mask;
			long sum = ax + bx + cx;
			return new WireValue[]{WireValue.Companion.createKnown(width, (int) sum),
					((sum >> w) & 1) == 0 ? WireValues.FALSE : WireValues.TRUE};
		}
		else {
			int sum = a.toIntValue() + b.toIntValue() + c_in.toIntValue();
			return new WireValue[]{WireValue.Companion.createKnown(width, sum), ((sum >> w) & 1) == 0 ? WireValues.FALSE : WireValues.TRUE};
		}
		else {
			WireValue[] bits = new WireValue[w];
			WireValue carry = c_in;
			for (int i = 0; i < w; i++)
				if (carry == WireValues.ERROR) bits[i] = WireValues.ERROR;
				else if (carry == WireValues.UNKNOWN) bits[i] = WireValues.UNKNOWN;
				else {
					WireValue ab = a.get(i);
					WireValue bb = b.get(i);
					if (ab == WireValues.ERROR || bb == WireValues.ERROR) {
						bits[i] = WireValues.ERROR;
						carry = WireValues.ERROR;
					}
					else if (ab == WireValues.UNKNOWN || bb == WireValues.UNKNOWN) {
						bits[i] = WireValues.UNKNOWN;
						carry = WireValues.UNKNOWN;
					}
					else {
						int sum = (ab == WireValues.TRUE ? 1 : 0) + (bb == WireValues.TRUE ? 1 : 0)
								+ (carry == WireValues.TRUE ? 1 : 0);
						bits[i] = (sum & 1) == 1 ? WireValues.TRUE : WireValues.FALSE;
						carry = (sum >= 2) ? WireValues.TRUE : WireValues.FALSE;
					}
				}
			return new WireValue[] { WireValue.Companion.create(bits), carry };
		}
	}
}
