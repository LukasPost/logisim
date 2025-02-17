/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.arith;

import java.awt.Graphics;

import logisim.data.Attribute;
import logisim.data.AttributeSet;
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
import logisim.tools.key.IntegerConfigurator;
import logisim.tools.key.JoinedConfigurator;
import logisim.util.GraphicsUtil;

public class BitAdder extends InstanceFactory {
	static final Attribute<Integer> NUM_INPUTS = Attributes.forIntegerRange("inputs", Strings.getter("gateInputsAttr"),
			1, 32);

	public BitAdder() {
		super("BitAdder", Strings.getter("bitAdderComponent"));
		setAttributes(new Attribute[] { StdAttr.WIDTH, NUM_INPUTS },
				new Object[] { BitWidth.create(8), 1});
		setKeyConfigurator(JoinedConfigurator.create(new IntegerConfigurator(NUM_INPUTS, 1, 32, 0),
				new BitWidthConfigurator(StdAttr.WIDTH)));
		setIconName("bitadder.gif");
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		int inputs = attrs.getValue(NUM_INPUTS);
		int h = Math.max(40, 10 * inputs);
		int y = inputs < 4 ? 20 : (((inputs - 1) / 2) * 10 + 5);
		return Bounds.create(-40, -y, 40, h);
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		configurePorts(instance);
		instance.addAttributeListener();
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.WIDTH) configurePorts(instance);
		else if (attr == NUM_INPUTS) {
			configurePorts(instance);
			instance.recomputeBounds();
		}
	}

	private void configurePorts(Instance instance) {
		BitWidth inWidth = instance.getAttributeValue(StdAttr.WIDTH);
		int inputs = instance.getAttributeValue(NUM_INPUTS);
		int outWidth = computeOutputBits(inWidth.getWidth(), inputs);

		int y;
		int dy = 10;
		switch (inputs) {
		case 1:
			y = 0;
			break;
		case 2:
			y = -10;
			dy = 20;
			break;
		case 3:
			y = -10;
			break;
		default:
			y = ((inputs - 1) / 2) * -10;
		}

		Port[] ps = new Port[inputs + 1];
		ps[0] = new Port(0, 0, Port.OUTPUT, BitWidth.create(outWidth));
		ps[0].setToolTip(Strings.getter("bitAdderOutputManyTip"));
		for (int i = 0; i < inputs; i++) {
			ps[i + 1] = new Port(-40, y + i * dy, Port.INPUT, inWidth);
			ps[i + 1].setToolTip(Strings.getter("bitAdderInputTip"));
		}
		instance.setPorts(ps);
	}

	private int computeOutputBits(int width, int inputs) {
		int maxBits = width * inputs;
		int outWidth = 1;
		while ((1 << outWidth) <= maxBits)
			outWidth++;
		return outWidth;
	}

	@Override
	public void propagate(InstanceState state) {
		int width = state.getAttributeValue(StdAttr.WIDTH).getWidth();
		int inputs = state.getAttributeValue(NUM_INPUTS);

		// compute the number of 1 bits
		int minCount = 0; // number that are definitely 1
		int maxCount = 0; // number that are definitely not 0 (incl X/Z)
		for (int i = 1; i <= inputs; i++) {
			WireValue v = state.getPort(i);
			WireValue[] bits = v.getAll();
			for (WireValue b : bits) {
				if (b == WireValues.TRUE)
					minCount++;
				if (b != WireValues.FALSE)
					maxCount++;
			}
		}

		// compute which output bits should be error bits
		int unknownMask = 0;
		for (int i = minCount + 1; i <= maxCount; i++) unknownMask |= (minCount ^ i);

		WireValue[] out = new WireValue[computeOutputBits(width, inputs)];
		for (int i = 0; i < out.length; i++)
			if (((unknownMask >> i) & 1) != 0) out[i] = WireValues.ERROR;
			else if (((minCount >> i) & 1) != 0) out[i] = WireValues.TRUE;
			else out[i] = WireValues.FALSE;

		int delay = out.length * Adder.PER_DELAY;
		state.setPort(0, WireValue.Companion.create(out), delay);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		painter.drawBounds();
		painter.drawPorts();

		GraphicsUtil.switchToWidth(g, 2);
		Location loc = painter.getLocation();
		int x = loc.x() - 10;
		int y = loc.y();
		g.drawLine(x - 2, y - 5, x - 2, y + 5);
		g.drawLine(x + 2, y - 5, x + 2, y + 5);
		g.drawLine(x - 5, y - 2, x + 5, y - 2);
		g.drawLine(x - 5, y + 2, x + 5, y + 2);
	}
}
