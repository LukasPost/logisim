/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.Graphics2D;

import logisim.data.WireValue.WireValue;
import logisim.tools.key.BitWidthConfigurator;
import logisim.util.GraphicsUtil;

import logisim.circuit.Wire;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;

public class Power extends InstanceFactory {
	public Power() {
		super("Power", Strings.getter("powerComponent"));
		setIconName("power.gif");
		setAttributes(new Attribute[] { StdAttr.FACING, StdAttr.WIDTH },
				new Object[] { Direction.North, BitWidth.ONE });
		setFacingAttribute(StdAttr.FACING);
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
		setPorts(new Port[] { new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH) });
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		instance.addAttributeListener();
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.FACING) instance.recomputeBounds();
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		return Bounds.create(0, -8, 15, 16).rotate(Direction.East, attrs.getValue(StdAttr.FACING), 0, 0);
	}

	@Override
	public void propagate(InstanceState state) {
		BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
		state.setPort(0, WireValue.Companion.repeat(WireValues.TRUE, width.getWidth()), 1);
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
		Graphics2D g = (Graphics2D) painter.getGraphics().create();
		Location loc = painter.getLocation();
		g.translate(loc.x(), loc.y());

		Direction from = painter.getAttributeValue(StdAttr.FACING);
		int degrees = Direction.East.toDegrees() - from.toDegrees();
		double radians = Math.toRadians((degrees + 360) % 360);
		g.rotate(radians);

		GraphicsUtil.switchToWidth(g, Wire.WIDTH);
		if (!isGhost && painter.getShowState()) g.setColor(painter.getPort(0).getColor());
		g.drawLine(0, 0, 5, 0);

		GraphicsUtil.switchToWidth(g, 1);
		if (!isGhost && painter.shouldDrawColor()) {
			BitWidth width = painter.getAttributeValue(StdAttr.WIDTH);
			g.setColor(WireValue.Companion.repeat(WireValues.TRUE, width.getWidth()).getColor());
		}
		g.drawPolygon(new int[] { 6, 14, 6 }, new int[] { -8, 0, 8 }, 3);

		g.dispose();
	}
}
