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

public class Multiplexer extends InstanceFactory {
	public Multiplexer() {
		super("Multiplexer", Strings.getter("multiplexerComponent"));
		setAttributes(
				new Attribute[] { StdAttr.FACING, Plexers.ATTR_SELECT_LOC, Plexers.ATTR_SELECT, StdAttr.WIDTH,
						Plexers.ATTR_DISABLED, Plexers.ATTR_ENABLE },
				new Object[] { Direction.East, Plexers.SELECT_BOTTOM_LEFT, Plexers.DEFAULT_SELECT, BitWidth.ONE,
						Plexers.DISABLED_FLOATING, Boolean.TRUE });
		setKeyConfigurator(JoinedConfigurator.create(new BitWidthConfigurator(Plexers.ATTR_SELECT, 1, 5, 0),
				new BitWidthConfigurator(StdAttr.WIDTH)));
		setIconName("multiplexer.gif");
		setFacingAttribute(StdAttr.FACING);
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
		Direction dir = attrs.getValue(StdAttr.FACING);
		BitWidth select = attrs.getValue(Plexers.ATTR_SELECT);
		int inputs = 1 << select.getWidth();
		if (inputs == 2) return Bounds.create(-30, -20, 30, 40).rotate(Direction.East, dir, 0, 0);
		else {
			int offs = -(inputs / 2) * 10 - 10;
			int length = inputs * 10 + 20;
			return Bounds.create(-40, offs, 40, length).rotate(Direction.East, dir, 0, 0);
		}
	}

	@Override
	public boolean contains(Location loc, AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
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
		else if (attr == Plexers.ATTR_DISABLED) instance.fireInvalidated();
	}

	private void updatePorts(Instance instance) {
		Direction dir = instance.getAttributeValue(StdAttr.FACING);
		Object selectLoc = instance.getAttributeValue(Plexers.ATTR_SELECT_LOC);
		BitWidth data = instance.getAttributeValue(StdAttr.WIDTH);
		BitWidth select = instance.getAttributeValue(Plexers.ATTR_SELECT);
		boolean enable = instance.getAttributeValue(Plexers.ATTR_ENABLE);

		int selMult = selectLoc == Plexers.SELECT_BOTTOM_LEFT ? 1 : -1;
		int inputs = 1 << select.getWidth();
		Port[] ps = new Port[inputs + (enable ? 3 : 2)];
		Location sel;
		if (inputs == 2) {
			Location end0;
			Location end1;
			if (dir == Direction.West) {
				end0 = new Location(30, -10);
				end1 = new Location(30, 10);
				sel = new Location(20, selMult * 20);
			} else if (dir == Direction.North) {
				end0 = new Location(-10, 30);
				end1 = new Location(10, 30);
				sel = new Location(selMult * -20, 20);
			} else if (dir == Direction.South) {
				end0 = new Location(-10, -30);
				end1 = new Location(10, -30);
				sel = new Location(selMult * -20, -20);
			} else {
				end0 = new Location(-30, -10);
				end1 = new Location(-30, 10);
				sel = new Location(-20, selMult * 20);
			}
			ps[0] = new Port(end0.x(), end0.y(), Port.INPUT, data.getWidth());
			ps[1] = new Port(end1.x(), end1.y(), Port.INPUT, data.getWidth());
		} else {
			int dx = -(inputs / 2) * 10;
			int ddx = 10;
			int dy = -(inputs / 2) * 10;
			int ddy = 10;
			if (dir == Direction.West) {
				dx = 40;
				ddx = 0;
				sel = new Location(20, selMult * (dy + 10 * inputs));
			} else if (dir == Direction.North) {
				dy = 40;
				ddy = 0;
				sel = new Location(selMult * dx, 20);
			} else if (dir == Direction.South) {
				dy = -40;
				ddy = 0;
				sel = new Location(selMult * dx, -20);
			} else {
				dx = -40;
				ddx = 0;
				sel = new Location(-20, selMult * (dy + 10 * inputs));
			}
			for (int i = 0; i < inputs; i++) {
				ps[i] = new Port(dx, dy, Port.INPUT, data.getWidth());
				dx += ddx;
				dy += ddy;
			}
		}
		Location en = sel.translate(dir, 10);
		ps[inputs] = new Port(sel.x(), sel.y(), Port.INPUT, select.getWidth());
		if (enable) ps[inputs + 1] = new Port(en.x(), en.y(), Port.INPUT, BitWidth.ONE);
		ps[ps.length - 1] = new Port(0, 0, Port.OUTPUT, data.getWidth());

		for (int i = 0; i < inputs; i++) ps[i].setToolTip(Strings.getter("multiplexerInTip", "" + i));
		ps[inputs].setToolTip(Strings.getter("multiplexerSelectTip"));
		if (enable) ps[inputs + 1].setToolTip(Strings.getter("multiplexerEnableTip"));
		ps[ps.length - 1].setToolTip(Strings.getter("multiplexerOutTip"));

		instance.setPorts(ps);
	}

	@Override
	public void propagate(InstanceState state) {
		BitWidth data = state.getAttributeValue(StdAttr.WIDTH);
		BitWidth select = state.getAttributeValue(Plexers.ATTR_SELECT);
		boolean enable = state.getAttributeValue(Plexers.ATTR_ENABLE);
		int inputs = 1 << select.getWidth();
		WireValue en = enable ? state.getPort(inputs + 1) : WireValues.TRUE;
		WireValue out;
		if (en == WireValues.FALSE) {
			Object opt = state.getAttributeValue(Plexers.ATTR_DISABLED);
			WireValue base = opt == Plexers.DISABLED_ZERO ? WireValues.FALSE : WireValues.UNKNOWN;
			out = WireValue.Companion.repeat(base, data.getWidth());
		} else if (en == WireValues.ERROR && state.isPortConnected(inputs + 1)) out = WireValue.Companion.createError(data);
		else {
			WireValue sel = state.getPort(inputs);
			if (sel.isFullyDefined()) out = state.getPort(sel.toIntValue());
			else if (sel.isErrorValue()) out = WireValue.Companion.createError(data);
			else out = WireValue.Companion.createUnknown(data);
		}
		state.setPort(inputs + (enable ? 2 : 1), out, Plexers.DELAY);
	}

	@Override
	public void paintGhost(InstancePainter painter) {
		Direction facing = painter.getAttributeValue(StdAttr.FACING);
		BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
		Plexers.drawTrapezoid(painter.getGraphics(), painter.getBounds(), facing, select.getWidth() == 1 ? 10 : 20);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		Bounds bds = painter.getBounds();
		Direction facing = painter.getAttributeValue(StdAttr.FACING);
		BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
		boolean enable = painter.getAttributeValue(Plexers.ATTR_ENABLE);
		int inputs = 1 << select.getWidth();

		// draw stubs for select/enable inputs that aren't on instance boundary
		GraphicsUtil.switchToWidth(g, 3);
		boolean vertical = facing != Direction.North && facing != Direction.South;
		Object selectLoc = painter.getAttributeValue(Plexers.ATTR_SELECT_LOC);
		int selMult = selectLoc == Plexers.SELECT_BOTTOM_LEFT ? 1 : -1;
		int dx = vertical ? 0 : -selMult;
		int dy = vertical ? selMult : 0;
		if (inputs == 2) { // draw select wire
			Location pt = painter.getInstance().getPortLocation(inputs);
			if (painter.getShowState()) g.setColor(painter.getPort(inputs).getColor());
			g.drawLine(pt.x() - 2 * dx, pt.y() - 2 * dy, pt.x(), pt.y());
		}
		if (enable) {
			Location en = painter.getInstance().getPortLocation(inputs + 1);
			if (painter.getShowState()) g.setColor(painter.getPort(inputs + 1).getColor());
			int len = inputs == 2 ? 6 : 4;
			g.drawLine(en.x() - len * dx, en.y() - len * dy, en.x(), en.y());
		}
		GraphicsUtil.switchToWidth(g, 1);

		// draw a circle indicating where the select input is located
		Multiplexer.drawSelectCircle(g, bds, painter.getInstance().getPortLocation(inputs));

		// draw a 0 indicating where the numbering starts for inputs
		int x0;
		int y0;
		int halign;
		if (facing == Direction.West) {
			x0 = bds.getX() + bds.getWidth() - 3;
			y0 = bds.getY() + 15;
			halign = GraphicsUtil.H_RIGHT;
		} else if (facing == Direction.North) {
			x0 = bds.getX() + 10;
			y0 = bds.getY() + bds.getHeight() - 2;
			halign = GraphicsUtil.H_CENTER;
		} else if (facing == Direction.South) {
			x0 = bds.getX() + 10;
			y0 = bds.getY() + 12;
			halign = GraphicsUtil.H_CENTER;
		} else {
			x0 = bds.getX() + 3;
			y0 = bds.getY() + 15;
			halign = GraphicsUtil.H_LEFT;
		}
		g.setColor(Color.GRAY);
		GraphicsUtil.drawText(g, "0", x0, y0, halign, GraphicsUtil.V_BASELINE);

		// draw the trapezoid, "MUX" string, the individual ports
		g.setColor(Color.BLACK);
		Plexers.drawTrapezoid(g, bds, facing, select.getWidth() == 1 ? 10 : 20);
		GraphicsUtil.drawCenteredText(g, "MUX", bds.getX() + bds.getWidth() / 2, bds.getY() + bds.getHeight() / 2);
		painter.drawPorts();
	}

	static void drawSelectCircle(Graphics g, Bounds bds, Location loc) {
		int locDelta = Math.max(bds.getHeight(), bds.getWidth()) <= 50 ? 8 : 6;
		Location circLoc;
		// at right
		// at left
		// vertically oriented
		// at bottom
		// at top
		if (bds.getHeight() >= bds.getWidth())
			if (loc.y() < bds.getY() + bds.getHeight() / 2) circLoc = loc.add(0, locDelta);
			else circLoc = loc.add(0, -locDelta);
		else if (loc.x() < bds.getX() + bds.getWidth() / 2) circLoc = loc.add(locDelta, 0);
		else circLoc = loc.add(-locDelta, 0);
		g.setColor(Color.LIGHT_GRAY);
		g.fillOval(circLoc.x() - 3, circLoc.y() - 3, 6, 6);
	}
}
