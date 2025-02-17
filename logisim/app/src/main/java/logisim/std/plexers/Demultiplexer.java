/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.plexers;

import java.awt.Color;
import java.awt.Graphics;

import logisim.LogisimVersion;
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
import logisim.tools.key.JoinedConfigurator;
import logisim.util.GraphicsUtil;

public class Demultiplexer extends InstanceFactory {
	public Demultiplexer() {
		super("Demultiplexer", Strings.getter("demultiplexerComponent"));
		setAttributes(
				new Attribute[] { StdAttr.FACING, Plexers.ATTR_SELECT_LOC, Plexers.ATTR_SELECT, StdAttr.WIDTH,
						Plexers.ATTR_TRISTATE, Plexers.ATTR_DISABLED, Plexers.ATTR_ENABLE },
				new Object[] { Direction.East, Plexers.SELECT_BOTTOM_LEFT, Plexers.DEFAULT_SELECT, BitWidth.ONE,
						Plexers.DEFAULT_TRISTATE, Plexers.DISABLED_FLOATING, Boolean.TRUE });
		setKeyConfigurator(JoinedConfigurator.create(new BitWidthConfigurator(Plexers.ATTR_SELECT, 1, 5, 0),
				new BitWidthConfigurator(StdAttr.WIDTH)));
		setFacingAttribute(StdAttr.FACING);
		setIconName("demultiplexer.gif");
	}

	@Override
	public Object getDefaultAttributeValue(Attribute<?> attr, LogisimVersion ver) {
		if (attr == Plexers.ATTR_ENABLE) {
			int newer = ver.compareTo(LogisimVersion.get(2, 6, 3, 220));
			return newer >= 0;
		} else return super.getDefaultAttributeValue(attr, ver);
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		BitWidth select = attrs.getValue(Plexers.ATTR_SELECT);
		int outputs = 1 << select.getWidth();
		Bounds bds;
		if (outputs == 2) bds = Bounds.create(0, -20, 30, 40);
		else bds = Bounds.create(0, -(outputs / 2) * 10 - 10, 40, outputs * 10 + 20);
		return bds.rotate(Direction.East, facing, 0, 0);
	}

	@Override
	public boolean contains(Location loc, AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING).reverse();
		return Plexers.contains(loc, getOffsetBounds(attrs), facing);
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		instance.addAttributeListener();
		updatePorts(instance);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.FACING || attr == Plexers.ATTR_SELECT_LOC || attr == Plexers.ATTR_SELECT) {
			instance.recomputeBounds();
			updatePorts(instance);
		} else if (attr == StdAttr.WIDTH || attr == Plexers.ATTR_ENABLE) updatePorts(instance);
		else if (attr == Plexers.ATTR_TRISTATE || attr == Plexers.ATTR_DISABLED) instance.fireInvalidated();
	}

	private void updatePorts(Instance instance) {
		Direction facing = instance.getAttributeValue(StdAttr.FACING);
		Object selectLoc = instance.getAttributeValue(Plexers.ATTR_SELECT_LOC);
		BitWidth data = instance.getAttributeValue(StdAttr.WIDTH);
		BitWidth select = instance.getAttributeValue(Plexers.ATTR_SELECT);
		boolean enable = instance.getAttributeValue(Plexers.ATTR_ENABLE);
		int outputs = 1 << select.getWidth();
		Port[] ps = new Port[outputs + (enable ? 3 : 2)];
		Location sel;
		int selMult = selectLoc == Plexers.SELECT_BOTTOM_LEFT ? 1 : -1;
		if (outputs == 2) {
			Location end0;
			Location end1;
			if (facing == Direction.West) {
				end0 = new Location(-30, -10);
				end1 = new Location(-30, 10);
				sel = new Location(-20, selMult * 20);
			} else if (facing == Direction.North) {
				end0 = new Location(-10, -30);
				end1 = new Location(10, -30);
				sel = new Location(selMult * -20, -20);
			} else if (facing == Direction.South) {
				end0 = new Location(-10, 30);
				end1 = new Location(10, 30);
				sel = new Location(selMult * -20, 20);
			} else {
				end0 = new Location(30, -10);
				end1 = new Location(30, 10);
				sel = new Location(20, selMult * 20);
			}
			ps[0] = new Port(end0.x(), end0.y(), Port.OUTPUT, data.getWidth());
			ps[1] = new Port(end1.x(), end1.y(), Port.OUTPUT, data.getWidth());
		} else {
			int dx = -(outputs / 2) * 10;
			int ddx = 10;
			int dy = dx;
			int ddy = 10;
			if (facing == Direction.West) {
				dx = -40;
				ddx = 0;
				sel = new Location(-20, selMult * (dy + 10 * outputs));
			} else if (facing == Direction.North) {
				dy = -40;
				ddy = 0;
				sel = new Location(selMult * dx, -20);
			} else if (facing == Direction.South) {
				dy = 40;
				ddy = 0;
				sel = new Location(selMult * dx, 20);
			} else {
				dx = 40;
				ddx = 0;
				sel = new Location(20, selMult * (dy + 10 * outputs));
			}
			for (int i = 0; i < outputs; i++) {
				ps[i] = new Port(dx, dy, Port.OUTPUT, data.getWidth());
				dx += ddx;
				dy += ddy;
			}
		}
		Location en = sel.translate(facing, -10);
		ps[outputs] = new Port(sel.x(), sel.y(), Port.INPUT, select.getWidth());
		if (enable) ps[outputs + 1] = new Port(en.x(), en.y(), Port.INPUT, BitWidth.ONE);
		ps[ps.length - 1] = new Port(0, 0, Port.INPUT, data.getWidth());

		for (int i = 0; i < outputs; i++) ps[i].setToolTip(Strings.getter("demultiplexerOutTip", "" + i));
		ps[outputs].setToolTip(Strings.getter("demultiplexerSelectTip"));
		if (enable) ps[outputs + 1].setToolTip(Strings.getter("demultiplexerEnableTip"));
		ps[ps.length - 1].setToolTip(Strings.getter("demultiplexerInTip"));

		instance.setPorts(ps);
	}

	@Override
	public void propagate(InstanceState state) {
		// get attributes
		BitWidth data = state.getAttributeValue(StdAttr.WIDTH);
		BitWidth select = state.getAttributeValue(Plexers.ATTR_SELECT);
		Boolean threeState = state.getAttributeValue(Plexers.ATTR_TRISTATE);
		boolean enable = state.getAttributeValue(Plexers.ATTR_ENABLE);
		int outputs = 1 << select.getWidth();
		WireValue en = enable ? state.getPort(outputs + 1) : WireValues.TRUE;

		// determine output values
		WireValue others; // the default output
		if (threeState) others = WireValue.Companion.createUnknown(data);
		else others = WireValue.Companion.createKnown(data, 0);
		int outIndex = -1; // the special output
		WireValue out = null;
		if (en == WireValues.FALSE) {
			Object opt = state.getAttributeValue(Plexers.ATTR_DISABLED);
			WireValue base = opt == Plexers.DISABLED_ZERO ? WireValues.FALSE : WireValues.UNKNOWN;
			others = WireValue.Companion.repeat(base, data.getWidth());
		} else if (en == WireValues.ERROR && state.isPortConnected(outputs + 1)) others = WireValue.Companion.createError(data);
		else {
			WireValue sel = state.getPort(outputs);
			if (sel.isFullyDefined()) {
				outIndex = sel.toIntValue();
				out = state.getPort(outputs + (enable ? 2 : 1));
			} else if (sel.isErrorValue()) others = WireValue.Companion.createError(data);
			else others = WireValue.Companion.createUnknown(data);
		}

		// now propagate them
		for (int i = 0; i < outputs; i++) state.setPort(i, i == outIndex ? out : others, Plexers.DELAY);
	}

	@Override
	public void paintGhost(InstancePainter painter) {
		Direction facing = painter.getAttributeValue(StdAttr.FACING);
		BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
		Plexers.drawTrapezoid(painter.getGraphics(), painter.getBounds(), facing.reverse(),
				select.getWidth() == 1 ? 10 : 20);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		Bounds bds = painter.getBounds();
		Direction facing = painter.getAttributeValue(StdAttr.FACING);
		BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
		boolean enable = painter.getAttributeValue(Plexers.ATTR_ENABLE);
		int outputs = 1 << select.getWidth();

		// draw select and enable inputs
		GraphicsUtil.switchToWidth(g, 3);
		boolean vertical = facing == Direction.North || facing == Direction.South;
		Object selectLoc = painter.getAttributeValue(Plexers.ATTR_SELECT_LOC);
		int selMult = selectLoc == Plexers.SELECT_BOTTOM_LEFT ? 1 : -1;
		int dx = vertical ? selMult : 0;
		int dy = vertical ? 0 : -selMult;
		if (outputs == 2) { // draw select wire
			Location sel = painter.getInstance().getPortLocation(outputs);
			if (painter.getShowState()) g.setColor(painter.getPort(outputs).getColor());
			g.drawLine(sel.x(), sel.y(), sel.x() + 2 * dx, sel.y() + 2 * dy);
		}
		if (enable) {
			Location en = painter.getInstance().getPortLocation(outputs + 1);
			if (painter.getShowState()) g.setColor(painter.getPort(outputs + 1).getColor());
			int len = outputs == 2 ? 6 : 4;
			g.drawLine(en.x(), en.y(), en.x() + len * dx, en.y() + len * dy);
		}
		GraphicsUtil.switchToWidth(g, 1);

		// draw a circle indicating where the select input is located
		Multiplexer.drawSelectCircle(g, bds, painter.getInstance().getPortLocation(outputs));

		// draw "0" next to first input
		int x0;
		int y0;
		int halign;
		if (facing == Direction.West) {
			x0 = 3;
			y0 = 15;
			halign = GraphicsUtil.H_LEFT;
		} else if (facing == Direction.North) {
			x0 = 10;
			y0 = 15;
			halign = GraphicsUtil.H_CENTER;
		} else if (facing == Direction.South) {
			x0 = 10;
			y0 = bds.getHeight() - 3;
			halign = GraphicsUtil.H_CENTER;
		} else {
			x0 = bds.getWidth() - 3;
			y0 = 15;
			halign = GraphicsUtil.H_RIGHT;
		}
		g.setColor(Color.GRAY);
		GraphicsUtil.drawText(g, "0", bds.getX() + x0, bds.getY() + y0, halign, GraphicsUtil.V_BASELINE);

		// draw trapezoid, "DMX" label, and ports
		g.setColor(Color.BLACK);
		Plexers.drawTrapezoid(g, bds, facing.reverse(), select.getWidth() == 1 ? 10 : 20);
		GraphicsUtil.drawCenteredText(g, "DMX", bds.getX() + bds.getWidth() / 2, bds.getY() + bds.getHeight() / 2);
		painter.drawPorts();
	}
}
