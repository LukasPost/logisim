/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.arith;

import java.awt.Color;
import java.awt.Graphics;
import java.util.Arrays;

import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.Attributes;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.tools.key.BitWidthConfigurator;

public class Shifter extends InstanceFactory {
	static final AttributeOption SHIFT_LOGICAL_LEFT = new AttributeOption("ll", Strings.getter("shiftLogicalLeft"));
	static final AttributeOption SHIFT_LOGICAL_RIGHT = new AttributeOption("lr", Strings.getter("shiftLogicalRight"));
	static final AttributeOption SHIFT_ARITHMETIC_RIGHT = new AttributeOption("ar",
			Strings.getter("shiftArithmeticRight"));
	static final AttributeOption SHIFT_ROLL_LEFT = new AttributeOption("rl", Strings.getter("shiftRollLeft"));
	static final AttributeOption SHIFT_ROLL_RIGHT = new AttributeOption("rr", Strings.getter("shiftRollRight"));
	static final Attribute<AttributeOption> ATTR_SHIFT = Attributes.forOption("shift",
			Strings.getter("shifterShiftAttr"), new AttributeOption[] { SHIFT_LOGICAL_LEFT, SHIFT_LOGICAL_RIGHT,
					SHIFT_ARITHMETIC_RIGHT, SHIFT_ROLL_LEFT, SHIFT_ROLL_RIGHT });

	private static final int IN0 = 0;
	private static final int IN1 = 1;
	private static final int OUT = 2;

	public Shifter() {
		super("Shifter", Strings.getter("shifterComponent"));
		setAttributes(new Attribute[] { StdAttr.WIDTH, ATTR_SHIFT },
				new Object[] { BitWidth.create(8), SHIFT_LOGICAL_LEFT });
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setOffsetBounds(Bounds.create(-40, -20, 40, 40));
		setIconName("shifter.gif");
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		configurePorts(instance);
		instance.addAttributeListener();
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.WIDTH) configurePorts(instance);
	}

	private void configurePorts(Instance instance) {
		BitWidth dataWid = instance.getAttributeValue(StdAttr.WIDTH);
		int data = dataWid == null ? 32 : dataWid.getWidth();
		int shift = 1;
		while ((1 << shift) < data)
			shift++;

		Port[] ps = new Port[3];
		ps[IN0] = new Port(-40, -10, Port.INPUT, data);
		ps[IN1] = new Port(-40, 10, Port.INPUT, shift);
		ps[OUT] = new Port(0, 0, Port.OUTPUT, data);
		ps[IN0].setToolTip(Strings.getter("shifterInputTip"));
		ps[IN1].setToolTip(Strings.getter("shifterDistanceTip"));
		ps[OUT].setToolTip(Strings.getter("shifterOutputTip"));
		instance.setPorts(ps);
	}

	@Override
	public void propagate(InstanceState state) {
		// compute output
		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
		int bits = dataWidth.getWidth();
		WireValue vx = state.getPort(IN0);
		WireValue vd = state.getPort(IN1);
		WireValue vy; // y will by x shifted by d
		if (vd.isFullyDefined() && vx.getWidth() == bits) {
			int d = vd.toIntValue();
			Object shift = state.getAttributeValue(ATTR_SHIFT);
			if (d == 0) vy = vx;
			else if (vx.isFullyDefined()) {
				int x = vx.toIntValue();
				int y;
				if (shift == SHIFT_LOGICAL_RIGHT) y = x >>> d;
				else if (shift == SHIFT_ARITHMETIC_RIGHT) {
					if (d >= bits)
						d = bits - 1;
					y = x >> d | ((x << (32 - bits)) >> (32 - bits + d));
				} else if (shift == SHIFT_ROLL_RIGHT) {
					if (d >= bits)
						d -= bits;
					y = (x >>> d) | (x << (bits - d));
				} else // SHIFT_LOGICAL_LEFT
					if (shift == SHIFT_ROLL_LEFT) {
					if (d >= bits)
						d -= bits;
					y = (x << d) | (x >>> (bits - d));
				} else y = x << d;
				vy = WireValue.Companion.createKnown(dataWidth, y);
			} else {
				WireValue[] x = vx.getAll();
				WireValue[] y = new WireValue[bits];
				if (shift == SHIFT_LOGICAL_RIGHT) {
					if (d >= bits)
						d = bits;
					System.arraycopy(x, d, y, 0, bits - d);
					Arrays.fill(y, bits - d, bits, WireValues.FALSE);
				} else if (shift == SHIFT_ARITHMETIC_RIGHT) {
					if (d >= bits)
						d = bits;
					System.arraycopy(x, d, y, 0, x.length - d);
					Arrays.fill(y, bits - d, y.length, x[bits - 1]);
				} else if (shift == SHIFT_ROLL_RIGHT) {
					if (d >= bits)
						d -= bits;
					System.arraycopy(x, d, y, 0, bits - d);
					System.arraycopy(x, 0, y, bits - d, d);
				} else if (shift == SHIFT_ROLL_LEFT) {
					if (d >= bits)
						d -= bits;
					System.arraycopy(x, x.length - d, y, 0, d);
					System.arraycopy(x, 0, y, d, bits - d);
				} else { // SHIFT_LOGICAL_LEFT
					if (d >= bits)
						d = bits;
					Arrays.fill(y, 0, d, WireValues.FALSE);
					System.arraycopy(x, 0, y, d, bits - d);
				}
				vy = WireValue.Companion.create(y);
			}
		} else vy = WireValue.Companion.createError(dataWidth);

		// propagate them
		int delay = dataWidth.getWidth() * (3 * Adder.PER_DELAY);
		state.setPort(OUT, vy, delay);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		painter.drawBounds();

		painter.drawPorts();

		Location loc = painter.getLocation();
		int x = loc.x() - 15;
		int y = loc.y();
		Object shift = painter.getAttributeValue(ATTR_SHIFT);
		g.setColor(Color.BLACK);
		if (shift == SHIFT_LOGICAL_RIGHT) {
			g.fillRect(x, y - 1, 8, 3);
			drawArrow(g, x + 10, y, -4);
		} else if (shift == SHIFT_ARITHMETIC_RIGHT) {
			g.fillRect(x, y - 1, 2, 3);
			g.fillRect(x + 3, y - 1, 5, 3);
			drawArrow(g, x + 10, y, -4);
		} else if (shift == SHIFT_ROLL_RIGHT) {
			g.fillRect(x, y - 1, 5, 3);
			g.fillRect(x + 8, y - 7, 2, 8);
			g.fillRect(x, y - 7, 2, 8);
			g.fillRect(x, y - 7, 10, 2);
			drawArrow(g, x + 8, y, -4);
		} else if (shift == SHIFT_ROLL_LEFT) {
			g.fillRect(x + 6, y - 1, 4, 3);
			g.fillRect(x + 8, y - 7, 2, 8);
			g.fillRect(x, y - 7, 2, 8);
			g.fillRect(x, y - 7, 10, 2);
			drawArrow(g, x + 3, y, 4);
		} else { // SHIFT_LOGICAL_LEFT
			g.fillRect(x + 2, y - 1, 8, 3);
			drawArrow(g, x, y, 4);
		}
	}

	private void drawArrow(Graphics g, int x, int y, int d) {
		int[] px = { x + d, x, x + d };
		int[] py = { y + d, y, y - d };
		g.fillPolygon(px, py, 3);
	}
}
