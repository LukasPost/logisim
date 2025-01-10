/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.arith;

import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.Attributes;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.tools.key.BitWidthConfigurator;

public class Comparator extends InstanceFactory {
	private static final AttributeOption SIGNED_OPTION = new AttributeOption("twosComplement", "twosComplement",
			Strings.getter("twosComplementOption"));
	private static final AttributeOption UNSIGNED_OPTION = new AttributeOption("unsigned", "unsigned",
			Strings.getter("unsignedOption"));
	private static final Attribute<AttributeOption> MODE_ATTRIBUTE = Attributes.forOption("mode",
			Strings.getter("comparatorType"), new AttributeOption[] { SIGNED_OPTION, UNSIGNED_OPTION });

	private static final int IN0 = 0;
	private static final int IN1 = 1;
	private static final int GT = 2;
	private static final int EQ = 3;
	private static final int LT = 4;

	public Comparator() {
		super("Comparator", Strings.getter("comparatorComponent"));
		setAttributes(new Attribute[] { StdAttr.WIDTH, MODE_ATTRIBUTE },
				new Object[] { BitWidth.create(8), SIGNED_OPTION });
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setOffsetBounds(Bounds.create(-40, -20, 40, 40));
		setIconName("comparator.gif");

		Port[] ps = new Port[5];
		ps[IN0] = new Port(-40, -10, Port.INPUT, StdAttr.WIDTH);
		ps[IN1] = new Port(-40, 10, Port.INPUT, StdAttr.WIDTH);
		ps[GT] = new Port(0, -10, Port.OUTPUT, 1);
		ps[EQ] = new Port(0, 0, Port.OUTPUT, 1);
		ps[LT] = new Port(0, 10, Port.OUTPUT, 1);
		ps[IN0].setToolTip(Strings.getter("comparatorInputATip"));
		ps[IN1].setToolTip(Strings.getter("comparatorInputBTip"));
		ps[GT].setToolTip(Strings.getter("comparatorGreaterTip"));
		ps[EQ].setToolTip(Strings.getter("comparatorEqualTip"));
		ps[LT].setToolTip(Strings.getter("comparatorLessTip"));
		setPorts(ps);
	}

	@Override
	public void propagate(InstanceState state) {
		// get attributes
		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);

		// compute outputs
		WireValue gt = WireValues.FALSE;
		WireValue eq = WireValues.TRUE;
		WireValue lt = WireValues.FALSE;

		WireValue a = state.getPort(IN0);
		WireValue b = state.getPort(IN1);
		WireValue[] ax = a.getAll();
		WireValue[] bx = b.getAll();
		int maxlen = Math.max(ax.length, bx.length);
		for (int pos = maxlen - 1; pos >= 0; pos--) {
			WireValue ab = pos < ax.length ? ax[pos] : WireValues.ERROR;
			WireValue bb = pos < bx.length ? bx[pos] : WireValues.ERROR;
			if (pos == ax.length - 1 && ab != bb) {
				Object mode = state.getAttributeValue(MODE_ATTRIBUTE);
				if (mode != UNSIGNED_OPTION) {
					WireValue t = ab;
					ab = bb;
					bb = t;
				}
			}

			if (ab == WireValues.ERROR || bb == WireValues.ERROR) {
				gt = WireValues.ERROR;
				eq = WireValues.ERROR;
				lt = WireValues.ERROR;
				break;
			} else if (ab == WireValues.UNKNOWN || bb == WireValues.UNKNOWN) {
				gt = WireValues.UNKNOWN;
				eq = WireValues.UNKNOWN;
				lt = WireValues.UNKNOWN;
				break;
			} else if (ab != bb) {
				eq = WireValues.FALSE;
				if (ab == WireValues.TRUE)
					gt = WireValues.TRUE;
				else
					lt = WireValues.TRUE;
				break;
			}
		}

		// propagate them
		int delay = (dataWidth.getWidth() + 2) * Adder.PER_DELAY;
		state.setPort(GT, gt, delay);
		state.setPort(EQ, eq, delay);
		state.setPort(LT, lt, delay);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		painter.drawBounds();
		painter.drawPort(IN0);
		painter.drawPort(IN1);
		painter.drawPort(GT, ">", Direction.West);
		painter.drawPort(EQ, "=", Direction.West);
		painter.drawPort(LT, "<", Direction.West);
	}

	//
	// methods for instances
	//
	@Override
	protected void configureNewInstance(Instance instance) {
		instance.addAttributeListener();
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		instance.fireInvalidated();
	}
}
