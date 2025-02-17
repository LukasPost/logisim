/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.Color;
import java.awt.Graphics2D;

import javax.swing.Icon;

import logisim.circuit.Wire;
import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.AttributeSet;
import logisim.data.Attributes;
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
import logisim.util.Icons;

public class Transistor extends InstanceFactory {
	static final AttributeOption TYPE_P = new AttributeOption("p", Strings.getter("transistorTypeP"));
	static final AttributeOption TYPE_N = new AttributeOption("n", Strings.getter("transistorTypeN"));
	static final Attribute<AttributeOption> ATTR_TYPE = Attributes.forOption("type",
			Strings.getter("transistorTypeAttr"), new AttributeOption[] { TYPE_P, TYPE_N });

	static final int OUTPUT = 0;
	static final int INPUT = 1;
	static final int GATE = 2;

	private static final Icon ICON_N = Icons.getIcon("trans1.gif");
	private static final Icon ICON_P = Icons.getIcon("trans0.gif");

	public Transistor() {
		super("Transistor", Strings.getter("transistorComponent"));
		setAttributes(new Attribute[] { ATTR_TYPE, StdAttr.FACING, Wiring.ATTR_GATE, StdAttr.WIDTH },
				new Object[] { TYPE_P, Direction.East, Wiring.GATE_TOP_LEFT, BitWidth.ONE });
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
		} else if (attr == StdAttr.WIDTH) updatePorts(instance);
		else if (attr == ATTR_TYPE) instance.fireInvalidated();
	}

	private void updatePorts(Instance instance) {
		Direction facing = instance.getAttributeValue(StdAttr.FACING);
		int dx = 0;
		int dy = 0;
		if (facing == Direction.North) dy = 1;
		else if (facing == Direction.East) dx = -1;
		else if (facing == Direction.South) dy = -1;
		else if (facing == Direction.West) dx = 1;

		Object powerLoc = instance.getAttributeValue(Wiring.ATTR_GATE);
		boolean flip = (facing == Direction.South || facing == Direction.West) == (powerLoc == Wiring.GATE_TOP_LEFT);

		Port[] ports = new Port[3];
		ports[OUTPUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
		ports[INPUT] = new Port(40 * dx, 40 * dy, Port.INPUT, StdAttr.WIDTH);
		if (flip) ports[GATE] = new Port(20 * (dx + dy), 20 * (-dx + dy), Port.INPUT, 1);
		else ports[GATE] = new Port(20 * (dx - dy), 20 * (dx + dy), Port.INPUT, 1);
		instance.setPorts(ports);
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		Object gateLoc = attrs.getValue(Wiring.ATTR_GATE);
		int delta = gateLoc == Wiring.GATE_TOP_LEFT ? -20 : 0;
		if (facing == Direction.North) return Bounds.create(delta, 0, 20, 40);
		else if (facing == Direction.South) return Bounds.create(delta, -40, 20, 40);
		else // facing == Direction.EAST
			if (facing == Direction.West) return Bounds.create(0, delta, 40, 20);
			else return Bounds.create(-40, delta, 40, 20);
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
		WireValue gate = state.getPort(GATE);
		WireValue input = state.getPort(INPUT);
		WireValue desired = state.getAttributeValue(ATTR_TYPE) == TYPE_P ? WireValues.FALSE : WireValues.TRUE;

		if (!gate.isFullyDefined()) if (input.isFullyDefined()) return WireValue.Companion.createError(width);
		else {
			WireValue[] v = input.getAll();
			for (int i = 0; i < v.length; i++) if (v[i] != WireValues.UNKNOWN) v[i] = WireValues.ERROR;
			return WireValue.Companion.create(v);
		}
		else if (gate != desired) return WireValue.Companion.createUnknown(width);
		else return input;
	}

	@Override
	public void paintIcon(InstancePainter painter) {
		Object type = painter.getAttributeValue(ATTR_TYPE);
		Icon icon = type == TYPE_N ? ICON_N : ICON_P;
		icon.paintIcon(painter.getDestination(), painter.getGraphics(), 2, 2);
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
		Object type = painter.getAttributeValue(ATTR_TYPE);
		Object powerLoc = painter.getAttributeValue(Wiring.ATTR_GATE);
		Direction from = painter.getAttributeValue(StdAttr.FACING);
		Direction facing = painter.getAttributeValue(StdAttr.FACING);
		boolean flip = (facing == Direction.South || facing == Direction.West) == (powerLoc == Wiring.GATE_TOP_LEFT);

		int degrees = Direction.East.toDegrees() - from.toDegrees();
		double radians = Math.toRadians((degrees + 360) % 360);
		int m = flip ? 1 : -1;

		Graphics2D g = (Graphics2D) painter.getGraphics();
		Location loc = painter.getLocation();
		g.translate(loc.x(), loc.y());
		g.rotate(radians);

		Color gate;
		Color input;
		Color output;
		Color platform;
		if (!isGhost && painter.getShowState()) {
			gate = painter.getPort(GATE).getColor();
			input = painter.getPort(INPUT).getColor();
			output = painter.getPort(OUTPUT).getColor();
			WireValue out = computeOutput(painter);
			platform = out.isUnknown() ? WireValues.UNKNOWN.getColor() : out.getColor();
		} else {
			Color base = g.getColor();
			gate = base;
			input = base;
			output = base;
			platform = base;
		}

		// input and output lines
		GraphicsUtil.switchToWidth(g, Wire.WIDTH);
		g.setColor(output);
		g.drawLine(0, 0, -11, 0);
		g.drawLine(-11, m * 7, -11, 0);

		g.setColor(input);
		g.drawLine(-40, 0, -29, 0);
		g.drawLine(-29, m * 7, -29, 0);

		// gate line
		g.setColor(gate);
		if (type == TYPE_P) {
			g.drawLine(-20, m * 20, -20, m * 15);
			GraphicsUtil.switchToWidth(g, 1);
			g.drawOval(-22, m * 12 - 2, 4, 4);
		} else {
			g.drawLine(-20, m * 20, -20, m * 11);
			GraphicsUtil.switchToWidth(g, 1);
		}

		// draw platforms
		g.drawLine(-10, m * 10, -30, m * 10); // gate platform
		g.setColor(platform);
		g.drawLine(-9, m * 8, -31, m * 8); // input/output platform

		// arrow (same color as platform)
		g.drawLine(-21, m * 6, -18, m * 3);
		g.drawLine(-21, 0, -18, m * 3);

		g.rotate(-radians);
		g.translate(-loc.x(), -loc.y());
	}
}
