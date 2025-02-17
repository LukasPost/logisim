/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.io;

import java.awt.Color;
import java.awt.Graphics;

import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceDataSingleton;
import logisim.instance.InstanceFactory;
import logisim.instance.InstanceLogger;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.util.GraphicsUtil;

public class Led extends InstanceFactory {
	public Led() {
		super("LED", Strings.getter("ledComponent"));
		setAttributes(
				new Attribute[] { StdAttr.FACING, Io.ATTR_ON_COLOR, Io.ATTR_OFF_COLOR, Io.ATTR_ACTIVE, StdAttr.LABEL,
						Io.ATTR_LABEL_LOC, StdAttr.LABEL_FONT, Io.ATTR_LABEL_COLOR },
				new Object[] { Direction.West, new Color(240, 0, 0), Color.DARK_GRAY, Boolean.TRUE, "", Io.LABEL_CENTER,
						StdAttr.DEFAULT_LABEL_FONT, Color.BLACK });
		setFacingAttribute(StdAttr.FACING);
		setIconName("led.gif");
		setPorts(new Port[] { new Port(0, 0, Port.INPUT, 1) });
		setInstanceLogger(Logger.class);
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		return Bounds.create(0, -10, 20, 20).rotate(Direction.West, facing, 0, 0);
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
		if (labelLoc == Direction.North) {
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
		WireValue val = state.getPort(0);
		InstanceDataSingleton data = (InstanceDataSingleton) state.getData();
		if (data == null) state.setData(new InstanceDataSingleton(val));
		else data.setValue(val);
	}

	@Override
	public void paintGhost(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		Bounds bds = painter.getBounds();
		GraphicsUtil.switchToWidth(g, 2);
		g.drawOval(bds.getX() + 1, bds.getY() + 1, bds.getWidth() - 2, bds.getHeight() - 2);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		InstanceDataSingleton data = (InstanceDataSingleton) painter.getData();
		WireValue val = data == null ? WireValues.FALSE : (WireValue) data.getValue();
		Bounds bds = painter.getBounds().expand(-1);

		Graphics g = painter.getGraphics();
		if (painter.getShowState()) {
			Color onColor = painter.getAttributeValue(Io.ATTR_ON_COLOR);
			Color offColor = painter.getAttributeValue(Io.ATTR_OFF_COLOR);
			Boolean activ = painter.getAttributeValue(Io.ATTR_ACTIVE);
			Object desired = activ ? WireValues.TRUE : WireValues.FALSE;
			g.setColor(val == desired ? onColor : offColor);
			g.fillOval(bds.getX(), bds.getY(), bds.getWidth(), bds.getHeight());
		}
		g.setColor(Color.BLACK);
		GraphicsUtil.switchToWidth(g, 2);
		g.drawOval(bds.getX(), bds.getY(), bds.getWidth(), bds.getHeight());
		GraphicsUtil.switchToWidth(g, 1);
		g.setColor(painter.getAttributeValue(Io.ATTR_LABEL_COLOR));
		painter.drawLabel();
		painter.drawPorts();
	}

	public static class Logger extends InstanceLogger {
		@Override
		public String getLogName(InstanceState state, Object option) {
			return state.getAttributeValue(StdAttr.LABEL);
		}

		@Override
		public WireValue getLogValue(InstanceState state, Object option) {
			InstanceDataSingleton data = (InstanceDataSingleton) state.getData();
			if (data == null)
				return WireValues.FALSE;
			return data.getValue() == WireValues.TRUE ? WireValues.TRUE : WireValues.FALSE;
		}
	}
}
