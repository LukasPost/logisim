/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.io;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.event.MouseEvent;

import logisim.circuit.Wire;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceDataSingleton;
import logisim.instance.InstanceFactory;
import logisim.instance.InstanceLogger;
import logisim.instance.InstancePainter;
import logisim.instance.InstancePoker;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.util.GraphicsUtil;

public class Button extends InstanceFactory {
	private static final int DEPTH = 3;

	public Button() {
		super("Button", Strings.getter("buttonComponent"));
		setAttributes(
				new Attribute[] { StdAttr.FACING, Io.ATTR_COLOR, StdAttr.LABEL, Io.ATTR_LABEL_LOC, StdAttr.LABEL_FONT,
						Io.ATTR_LABEL_COLOR },
				new Object[] { Direction.East, Color.WHITE, "", Io.LABEL_CENTER, StdAttr.DEFAULT_LABEL_FONT,
						Color.BLACK });
		setFacingAttribute(StdAttr.FACING);
		setIconName("button.gif");
		setPorts(new Port[] { new Port(0, 0, Port.OUTPUT, 1) });
		setInstancePoker(Poker.class);
		setInstanceLogger(Logger.class);
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		return Bounds.create(-20, -10, 20, 20).rotate(Direction.East, facing, 0, 0);
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		instance.addAttributeListener();
		computeTextField(instance);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.FACING) {
			instance.recomputeBounds();
			computeTextField(instance);
		} else if (attr == Io.ATTR_LABEL_LOC) computeTextField(instance);
	}

	private void computeTextField(Instance instance) {
		Direction facing = instance.getAttributeValue(StdAttr.FACING);
		Object labelLoc = instance.getAttributeValue(Io.ATTR_LABEL_LOC);

		Bounds bds = instance.getBounds();
		int x = bds.getX() + bds.getWidth() / 2;
		int y = bds.getY() + bds.getHeight() / 2;
		int halign = GraphicsUtil.H_CENTER;
		int valign = GraphicsUtil.V_CENTER;
		if (labelLoc == Io.LABEL_CENTER) {
			x = bds.getX() + (bds.getWidth() - DEPTH) / 2;
			y = bds.getY() + (bds.getHeight() - DEPTH) / 2;
		} else if (labelLoc == Direction.North) {
			y = bds.getY() - 2;
			valign = GraphicsUtil.V_BOTTOM;
		} else if (labelLoc == Direction.South) {
			y = bds.getY() + bds.getHeight() + 2;
			valign = GraphicsUtil.V_TOP;
		} else if (labelLoc == Direction.East) {
			x = bds.getX() + bds.getWidth() + 2;
			halign = GraphicsUtil.H_LEFT;
		} else if (labelLoc == Direction.West) {
			x = bds.getX() - 2;
			halign = GraphicsUtil.H_RIGHT;
		}
		if (labelLoc == facing) if (labelLoc == Direction.North || labelLoc == Direction.South) {
			x += 2;
			halign = GraphicsUtil.H_LEFT;
		}
		else {
			y -= 2;
			valign = GraphicsUtil.V_BOTTOM;
		}

		instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, x, y, halign, valign);
	}

	@Override
	public void propagate(InstanceState state) {
		InstanceDataSingleton data = (InstanceDataSingleton) state.getData();
		WireValue val = data == null ? WireValues.FALSE : (WireValue) data.getValue();
		state.setPort(0, val, 1);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Bounds bds = painter.getBounds();
		int x = bds.getX();
		int y = bds.getY();
		int w = bds.getWidth();
		int h = bds.getHeight();

		WireValue val;
		if (painter.getShowState()) {
			InstanceDataSingleton data = (InstanceDataSingleton) painter.getData();
			val = data == null ? WireValues.FALSE : (WireValue) data.getValue();
		} else val = WireValues.FALSE;

		Color color = painter.getAttributeValue(Io.ATTR_COLOR);
		if (!painter.shouldDrawColor()) {
			int hue = (color.getRed() + color.getGreen() + color.getBlue()) / 3;
			color = new Color(hue, hue, hue);
		}

		Graphics g = painter.getGraphics();
		int depress;
		if (val == WireValues.TRUE) {
			x += DEPTH;
			y += DEPTH;
			Object labelLoc = painter.getAttributeValue(Io.ATTR_LABEL_LOC);
			if (labelLoc == Io.LABEL_CENTER || labelLoc == Direction.North || labelLoc == Direction.West)
				depress = DEPTH;
			else depress = 0;

			Object facing = painter.getAttributeValue(StdAttr.FACING);
			if (facing == Direction.North || facing == Direction.West) {
				Location p = painter.getLocation();
				int px = p.x();
				int py = p.y();
				GraphicsUtil.switchToWidth(g, Wire.WIDTH);
				g.setColor(WireValues.Companion.getTRUE_COLOR());
				if (facing == Direction.North)
					g.drawLine(px, py, px, py + 10);
				else
					g.drawLine(px, py, px + 10, py);
				GraphicsUtil.switchToWidth(g, 1);
			}

			g.setColor(color);
			g.fillRect(x, y, w - DEPTH, h - DEPTH);
			g.setColor(Color.BLACK);
			g.drawRect(x, y, w - DEPTH, h - DEPTH);
		} else {
			depress = 0;
			int[] xp = { x, x + w - DEPTH, x + w, x + w, x + DEPTH, x };
			int[] yp = { y, y, y + DEPTH, y + h, y + h, y + h - DEPTH };
			g.setColor(color.darker());
			g.fillPolygon(xp, yp, xp.length);
			g.setColor(color);
			g.fillRect(x, y, w - DEPTH, h - DEPTH);
			g.setColor(Color.BLACK);
			g.drawRect(x, y, w - DEPTH, h - DEPTH);
			g.drawLine(x + w - DEPTH, y + h - DEPTH, x + w, y + h);
			g.drawPolygon(xp, yp, xp.length);
		}

		g.translate(depress, depress);
		g.setColor(painter.getAttributeValue(Io.ATTR_LABEL_COLOR));
		painter.drawLabel();
		g.translate(-depress, -depress);
		painter.drawPorts();
	}

	public static class Poker extends InstancePoker {
		@Override
		public void mousePressed(InstanceState state, MouseEvent e) {
			setValue(state, WireValues.TRUE);
		}

		@Override
		public void mouseReleased(InstanceState state, MouseEvent e) {
			setValue(state, WireValues.FALSE);
		}

		private void setValue(InstanceState state, WireValue val) {
			InstanceDataSingleton data = (InstanceDataSingleton) state.getData();
			if (data == null) state.setData(new InstanceDataSingleton(val));
			else data.setValue(val);
			state.getInstance().fireInvalidated();
		}
	}

	public static class Logger extends InstanceLogger {
		@Override
		public String getLogName(InstanceState state, Object option) {
			return state.getAttributeValue(StdAttr.LABEL);
		}

		@Override
		public WireValue getLogValue(InstanceState state, Object option) {
			InstanceDataSingleton data = (InstanceDataSingleton) state.getData();
			return data == null ? WireValues.FALSE : (WireValue) data.getValue();
		}
	}
}
