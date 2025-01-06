/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;

import logisim.analyze.model.Expression;
import logisim.circuit.ExpressionComputer;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.data.Value;
import logisim.file.Options;
import logisim.instance.Instance;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.tools.key.BitWidthConfigurator;
import logisim.util.GraphicsUtil;
import logisim.util.Icons;

class Buffer extends InstanceFactory {
	public static InstanceFactory FACTORY = new Buffer();

	private Buffer() {
		super("Buffer", Strings.getter("bufferComponent"));
		setAttributes(
				new Attribute[] { StdAttr.FACING, StdAttr.WIDTH, GateAttributes.ATTR_OUTPUT, StdAttr.LABEL,
						StdAttr.LABEL_FONT },
				new Object[] { Direction.East, BitWidth.ONE, GateAttributes.OUTPUT_01, "",
						StdAttr.DEFAULT_LABEL_FONT });
		setIcon(Icons.getIcon("bufferGate.gif"));
		setFacingAttribute(StdAttr.FACING);
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setPorts(new Port[] { new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH),
				new Port(0, -20, Port.INPUT, StdAttr.WIDTH), });
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		if (facing == Direction.South)
			return Bounds.create(-9, -20, 18, 20);
		if (facing == Direction.North)
			return Bounds.create(-9, 0, 18, 20);
		if (facing == Direction.West)
			return Bounds.create(0, -9, 20, 18);
		return Bounds.create(-20, -9, 20, 18);
	}

	@Override
	public void propagate(InstanceState state) {
		Value in = state.getPort(1);
		in = Buffer.repair(state, in);
		state.setPort(0, in, GateAttributes.DELAY);
	}

	//
	// methods for instances
	//
	@Override
	protected void configureNewInstance(Instance instance) {
		configurePorts(instance);
		instance.addAttributeListener();
		NotGate.configureLabel(instance, false, null);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.FACING) {
			instance.recomputeBounds();
			configurePorts(instance);
			NotGate.configureLabel(instance, false, null);
		}
	}

	private void configurePorts(Instance instance) {
		Direction facing = instance.getAttributeValue(StdAttr.FACING);

		Port[] ports = new Port[2];
		ports[0] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
		Location out = new Location(0, 0).translate(facing, -20);
		ports[1] = new Port(out.x(), out.y(), Port.INPUT, StdAttr.WIDTH);
		instance.setPorts(ports);
	}

	@Override
	public Object getInstanceFeature(final Instance instance, Object key) {
		if (key == ExpressionComputer.class) return (ExpressionComputer) expressionMap -> {
			Expression e = expressionMap.get(instance.getPortLocation(1));
			if (e != null) expressionMap.put(instance.getPortLocation(0), e);
		};
		return super.getInstanceFeature(instance, key);
	}

	//
	// painting methods
	//
	@Override
	public void paintGhost(InstancePainter painter) {
		paintBase(painter);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		g.setColor(Color.BLACK);
		paintBase(painter);
		painter.drawPorts();
		painter.drawLabel();
	}

	private void paintBase(InstancePainter painter) {
		Direction facing = painter.getAttributeValue(StdAttr.FACING);
		Location loc = painter.getLocation();
		int x = loc.x();
		int y = loc.y();
		Graphics g = painter.getGraphics();
		g.translate(x, y);
		double rotate = 0.0;
		if (facing != Direction.East && g instanceof Graphics2D) {
			rotate = -facing.toRadians();
			((Graphics2D) g).rotate(rotate);
		}

		GraphicsUtil.switchToWidth(g, 2);
		int[] xp = new int[4];
		int[] yp = new int[4];
		xp[0] = 0;
		yp[0] = 0;
		xp[1] = -19;
		yp[1] = -7;
		xp[2] = -19;
		yp[2] = 7;
		xp[3] = 0;
		yp[3] = 0;
		g.drawPolyline(xp, yp, 4);

		if (rotate != 0.0) ((Graphics2D) g).rotate(-rotate);
		g.translate(-x, -y);
	}

	//
	// static methods - shared with other classes
	//
	static Value repair(InstanceState state, Value v) {
		AttributeSet opts = state.getProject().getOptions().getAttributeSet();
		Object onUndefined = opts.getValue(Options.ATTR_GATE_UNDEFINED);
		boolean errorIfUndefined = onUndefined.equals(Options.GATE_UNDEFINED_ERROR);
		Value repaired;
		if (errorIfUndefined) {
			int vw = v.getWidth();
			BitWidth w = state.getAttributeValue(StdAttr.WIDTH);
			int ww = w.getWidth();
			if (vw == ww && v.isFullyDefined())
				return v;
			Value[] vs = new Value[w.getWidth()];
			for (int i = 0; i < vs.length; i++) {
				Value ini = i < vw ? v.get(i) : Value.ERROR;
				vs[i] = ini.isFullyDefined() ? ini : Value.ERROR;
			}
			repaired = Value.create(vs);
		} else repaired = v;

		Object outType = state.getAttributeValue(GateAttributes.ATTR_OUTPUT);
		return AbstractGate.pullOutput(repaired, outType);
	}
}
