/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.arith;

import logisim.data.Attribute;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.tools.key.BitWidthConfigurator;

public class Negator extends InstanceFactory {
	private static final int IN = 0;
	private static final int OUT = 1;

	public Negator() {
		super("Negator", Strings.getter("negatorComponent"));
		setAttributes(new Attribute[] { StdAttr.WIDTH }, new Object[] { BitWidth.create(8) });
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setOffsetBounds(Bounds.create(-40, -20, 40, 40));
		setIconName("negator.gif");

		Port[] ps = new Port[2];
		ps[IN] = new Port(-40, 0, Port.INPUT, StdAttr.WIDTH);
		ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
		ps[IN].setToolTip(Strings.getter("negatorInputTip"));
		ps[OUT].setToolTip(Strings.getter("negatorOutputTip"));
		setPorts(ps);
	}

	@Override
	public void propagate(InstanceState state) {
		// get attributes
		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);

		// compute outputs
		WireValue in = state.getPort(IN);
		WireValue out;
		if (in.isFullyDefined()) out = WireValue.Companion.createKnown(in.getBitWidth(), -in.toIntValue());
		else {
			WireValue[] bits = in.getAll();
			WireValue fill = WireValues.FALSE;
			int pos = 0;
			while (pos < bits.length) {
				if (bits[pos] == WireValues.FALSE) bits[pos] = fill;
				else if (bits[pos] == WireValues.TRUE) {
					if (fill != WireValues.FALSE)
						bits[pos] = fill;
					pos++;
					break;
				} else if (bits[pos] == WireValues.ERROR) fill = WireValues.ERROR;
				else if (fill == WireValues.FALSE)
					fill = bits[pos];
				else
					bits[pos] = fill;
				pos++;
			}
			while (pos < bits.length) {
				if (bits[pos] == WireValues.TRUE) bits[pos] = WireValues.FALSE;
				else if (bits[pos] == WireValues.FALSE) bits[pos] = WireValues.TRUE;
				pos++;
			}
			out = WireValue.Companion.create(bits);
		}

		// propagate them
		int delay = (dataWidth.getWidth() + 2) * Adder.PER_DELAY;
		state.setPort(OUT, out, delay);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		painter.drawBounds();
		painter.drawPort(IN);
		painter.drawPort(OUT, "-x", Direction.West);
	}
}
