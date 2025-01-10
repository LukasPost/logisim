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

public class Multiplier extends InstanceFactory {
	static final int PER_DELAY = 1;

	private static final int IN0 = 0;
	private static final int IN1 = 1;
	private static final int OUT = 2;
	private static final int C_IN = 3;
	private static final int C_OUT = 4;

	public Multiplier() {
		super("Multiplier", Strings.getter("multiplierComponent"));
		setAttributes(new Attribute[] { StdAttr.WIDTH }, new Object[] { BitWidth.create(8) });
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setOffsetBounds(Bounds.create(-40, -20, 40, 40));
		setIconName("multiplier.gif");

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

	@Override
	public void propagate(InstanceState state) {
		// get attributes
		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);

		// compute outputs
		WireValue a = state.getPort(IN0);
		WireValue b = state.getPort(IN1);
		WireValue c_in = state.getPort(C_IN);
		WireValue[] outs = Multiplier.computeProduct(dataWidth, a, b, c_in);

		// propagate them
		int delay = dataWidth.getWidth() * (dataWidth.getWidth() + 2) * PER_DELAY;
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
		g.drawLine(x - 15, y - 5, x - 5, y + 5);
		g.drawLine(x - 15, y + 5, x - 5, y - 5);
		GraphicsUtil.switchToWidth(g, 1);
	}

	static WireValue[] computeProduct(BitWidth width, WireValue a, WireValue b, WireValue c_in) {
		int w = width.getWidth();
		if (c_in == WireValues.NIL || c_in.isUnknown())
			c_in = WireValue.Companion.createKnown(width, 0);
		if (a.isFullyDefined() && b.isFullyDefined() && c_in.isFullyDefined()) {
			long sum = (long) a.toIntValue() * (long) b.toIntValue() + (long) c_in.toIntValue();
			return new WireValue[] { WireValue.Companion.createKnown(width, (int) sum), WireValue.Companion.createKnown(width, (int) (sum >> w)) };
		} else {
			WireValue[] avals = a.getAll();
			int aOk = findUnknown(avals);
			int aErr = findError(avals);
			int ax = getKnown(avals);
			WireValue[] bvals = b.getAll();
			int bOk = findUnknown(bvals);
			int bErr = findError(bvals);
			int bx = getKnown(bvals);
			WireValue[] cvals = c_in.getAll();
			int cOk = findUnknown(cvals);
			int cErr = findError(cvals);
			int cx = getKnown(cvals);

			int known = Math.min(Math.min(aOk, bOk), cOk);
			int error = Math.min(Math.min(aErr, bErr), cErr);
			int ret = ax * bx + cx;

			WireValue[] bits = new WireValue[w];
			for (int i = 0; i < w; i++)
				if (i < known) bits[i] = ((ret & (1 << i)) != 0 ? WireValues.TRUE : WireValues.FALSE);
				else if (i < error) bits[i] = WireValues.UNKNOWN;
				else bits[i] = WireValues.ERROR;
			return new WireValue[] { WireValue.Companion.create(bits),
					error < w ? WireValue.Companion.createError(width) : WireValue.Companion.createUnknown(width) };
		}
	}

	private static int findUnknown(WireValue[] vals) {
		for (int i = 0; i < vals.length; i++)
			if (!vals[i].isFullyDefined())
				return i;
		return vals.length;
	}

	private static int findError(WireValue[] vals) {
		for (int i = 0; i < vals.length; i++)
			if (vals[i].isErrorValue())
				return i;
		return vals.length;
	}

	private static int getKnown(WireValue[] vals) {
		int ret = 0;
		for (int i = 0; i < vals.length; i++) {
			int val = vals[i].toIntValue();
			if (val < 0)
				return ret;
			ret |= val << i;
		}
		return ret;
	}
}
