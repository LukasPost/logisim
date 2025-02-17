/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.Color;
import java.awt.Graphics2D;

import logisim.circuit.Wire;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
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
import logisim.util.GraphicsUtil;

public class TransmissionGate extends InstanceFactory {
	static final int OUTPUT = 0;
	static final int INPUT = 1;
	static final int GATE0 = 2;
	static final int GATE1 = 3;

	public TransmissionGate() {
		super("Transmission Gate", Strings.getter("transmissionGateComponent"));
		setIconName("transmis.gif");
		setAttributes(new Attribute[] { StdAttr.FACING, Wiring.ATTR_GATE, StdAttr.WIDTH },
				new Object[] { Direction.East, Wiring.GATE_TOP_LEFT, BitWidth.ONE });
		setFacingAttribute(StdAttr.FACING);
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		instance.addAttributeListener();
		updatePorts(instance);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.FACING || attr == Wiring.ATTR_GATE) {
			instance.recomputeBounds();
			updatePorts(instance);
		} else if (attr == StdAttr.WIDTH) instance.fireInvalidated();
	}

	private void updatePorts(Instance instance) {
		int dx = 0;
		int dy = 0;
		Direction facing = instance.getAttributeValue(StdAttr.FACING);
		if (facing == Direction.North) dy = 1;
		else if (facing == Direction.East) dx = -1;
		else if (facing == Direction.South) dy = -1;
		else if (facing == Direction.West) dx = 1;

		Object powerLoc = instance.getAttributeValue(Wiring.ATTR_GATE);
		boolean flip = (facing == Direction.South || facing == Direction.West) == (powerLoc == Wiring.GATE_TOP_LEFT);

		Port[] ports = new Port[4];
		ports[OUTPUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
		ports[INPUT] = new Port(40 * dx, 40 * dy, Port.INPUT, StdAttr.WIDTH);
		if (flip) {
			ports[GATE1] = new Port(20 * (dx - dy), 20 * (dx + dy), Port.INPUT, 1);
			ports[GATE0] = new Port(20 * (dx + dy), 20 * (-dx + dy), Port.INPUT, 1);
		} else {
			ports[GATE0] = new Port(20 * (dx - dy), 20 * (dx + dy), Port.INPUT, 1);
			ports[GATE1] = new Port(20 * (dx + dy), 20 * (-dx + dy), Port.INPUT, 1);
		}
		instance.setPorts(ports);
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		return Bounds.create(0, -20, 40, 40).rotate(Direction.West, facing, 0, 0);
	}

	@Override
	public boolean contains(Location loc, AttributeSet attrs) {
		if (super.contains(loc, attrs)) {
			Direction facing = attrs.getValue(StdAttr.FACING);
			Location center = new Location(0, 0).translate(facing, -20);
			return center.manhattanDistanceTo(loc) < 24;
		} else return false;
	}

	@Override
	public void propagate(InstanceState state) {
		state.setPort(OUTPUT, computeOutput(state), 1);
	}

	private WireValue computeOutput(InstanceState state) {
		BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
		WireValue input = state.getPort(INPUT);
		WireValue gate0 = state.getPort(GATE0);
		WireValue gate1 = state.getPort(GATE1);

		if (gate0.isFullyDefined() && gate1.isFullyDefined() && gate0 != gate1)
			if (gate0 == WireValues.TRUE) return WireValue.Companion.createUnknown(width);
			else return input;
		else if (input.isFullyDefined()) return WireValue.Companion.createError(width);
		else {
			WireValue[] v = input.getAll();
			for (int i = 0; i < v.length; i++) if (v[i] != WireValues.UNKNOWN) v[i] = WireValues.ERROR;
			return WireValue.Companion.create(v);
		}
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		drawInstance(painter, false);
		painter.drawPorts();
	}

	@Override
	public void paintGhost(InstancePainter painter) {
		drawInstance(painter, true);
	}

	private void drawInstance(InstancePainter painter, boolean isGhost) {
		Bounds bds = painter.getBounds();
		Object powerLoc = painter.getAttributeValue(Wiring.ATTR_GATE);
		Direction facing = painter.getAttributeValue(StdAttr.FACING);
		boolean flip = (facing == Direction.South || facing == Direction.West) == (powerLoc == Wiring.GATE_TOP_LEFT);

		int degrees = Direction.West.toDegrees() - facing.toDegrees();
		if (flip)
			degrees += 180;
		double radians = Math.toRadians((degrees + 360) % 360);

		Graphics2D g = (Graphics2D) painter.getGraphics().create();
		g.rotate(radians, bds.getX() + 20, bds.getY() + 20);
		g.translate(bds.getX(), bds.getY());
		GraphicsUtil.switchToWidth(g, Wire.WIDTH);

		Color gate0 = g.getColor();
		Color gate1 = gate0;
		Color input = gate0;
		Color output = gate0;
		Color platform = gate0;
		if (!isGhost && painter.getShowState()) {
			gate0 = painter.getPort(GATE0).getColor();
			gate1 = painter.getPort(GATE0).getColor();
			input = painter.getPort(INPUT).getColor();
			output = painter.getPort(OUTPUT).getColor();
			platform = computeOutput(painter).getColor();
		}

		g.setColor(flip ? input : output);
		g.drawLine(0, 20, 11, 20);
		g.drawLine(11, 13, 11, 27);

		g.setColor(flip ? output : input);
		g.drawLine(29, 20, 40, 20);
		g.drawLine(29, 13, 29, 27);

		g.setColor(gate0);
		g.drawLine(20, 35, 20, 40);
		GraphicsUtil.switchToWidth(g, 1);
		g.drawOval(18, 30, 4, 4);
		g.drawLine(10, 30, 30, 30);
		GraphicsUtil.switchToWidth(g, Wire.WIDTH);

		g.setColor(gate1);
		g.drawLine(20, 9, 20, 0);
		GraphicsUtil.switchToWidth(g, 1);
		g.drawLine(10, 10, 30, 10);

		g.setColor(platform);
		g.drawLine(9, 12, 31, 12);
		g.drawLine(9, 28, 31, 28);
		if (flip) { // arrow
			g.drawLine(18, 17, 21, 20);
			g.drawLine(18, 23, 21, 20);
		} else {
			g.drawLine(22, 17, 19, 20);
			g.drawLine(22, 23, 19, 20);
		}

		g.dispose();
	}
}
