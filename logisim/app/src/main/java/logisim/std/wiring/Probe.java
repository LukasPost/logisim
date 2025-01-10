/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.Color;
import java.awt.Graphics;
import java.util.Objects;

import logisim.circuit.RadixOption;
import logisim.comp.TextField;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceData;
import logisim.instance.InstanceFactory;
import logisim.instance.InstanceLogger;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.util.GraphicsUtil;

public class Probe extends InstanceFactory {
	public static final Probe FACTORY = new Probe();

	private static class StateData implements InstanceData, Cloneable {
		WireValue curValue = WireValues.NIL;

		@Override
		public Object clone() {
			try {
				return super.clone();
			}
			catch (CloneNotSupportedException e) {
				return null;
			}
		}
	}

	public static class ProbeLogger extends InstanceLogger {
		public ProbeLogger() {
		}

		@Override
		public String getLogName(InstanceState state, Object option) {
			String ret = state.getAttributeValue(StdAttr.LABEL);
			return ret != null && !ret.isEmpty() ? ret : null;
		}

		@Override
		public WireValue getLogValue(InstanceState state, Object option) {
			return getValue(state);
		}
	}

	public Probe() {
		super("Probe", Strings.getter("probeComponent"));
		setIconName("probe.gif");
		setFacingAttribute(StdAttr.FACING);
		setInstanceLogger(ProbeLogger.class);
	}

	@Override
	public AttributeSet createAttributeSet() {
		return new ProbeAttributes();
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrsBase) {
		ProbeAttributes attrs = (ProbeAttributes) attrsBase;
		return getOffsetBounds(attrs.facing, attrs.width, attrs.radix);
	}

	//
	// graphics methods
	//
	@Override
	public void paintGhost(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		Bounds bds = painter.getOffsetBounds();
		g.drawOval(bds.getX() + 1, bds.getY() + 1, bds.getWidth() - 1, bds.getHeight() - 1);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		WireValue value = getValue(painter);

		Graphics g = painter.getGraphics();
		Bounds bds = painter.getBounds(); // intentionally with no graphics object - we don't want label included
		int x = bds.getX();
		int y = bds.getY();
		g.setColor(Color.WHITE);
		g.fillRect(x + 5, y + 5, bds.getWidth() - 10, bds.getHeight() - 10);
		g.setColor(Color.GRAY);
		if (value.getWidth() <= 1) g.drawOval(x + 1, y + 1, bds.getWidth() - 2, bds.getHeight() - 2);
		else g.drawRoundRect(x + 1, y + 1, bds.getWidth() - 2, bds.getHeight() - 2, 6, 6);

		g.setColor(Color.BLACK);
		painter.drawLabel();

		if (!painter.getShowState()) {
			if (value.getWidth() > 0)
				GraphicsUtil.drawCenteredText(g, "x" + value.getWidth(), bds.getX() + bds.getWidth() / 2,
						bds.getY() + bds.getHeight() / 2);
		} else paintValue(painter, value);

		painter.drawPorts();
	}

	static void paintValue(InstancePainter painter, WireValue value) {
		Graphics g = painter.getGraphics();
		Bounds bds = painter.getBounds(); // intentionally with no graphics object - we don't want label included

		RadixOption radix = painter.getAttributeValue(RadixOption.ATTRIBUTE);
		if (radix == null || radix == RadixOption.RADIX_2) {
			int x = bds.getX();
			int y = bds.getY();
			int wid = value.getWidth();
			if (wid == 0) {
				x += bds.getWidth() / 2;
				y += bds.getHeight() / 2;
				GraphicsUtil.switchToWidth(g, 2);
				g.drawLine(x - 4, y, x + 4, y);
				return;
			}
			int x0 = bds.getX() + bds.getWidth() - 5;
			int compWidth = wid * 10;
			if (compWidth < bds.getWidth() - 3) x0 = bds.getX() + (bds.getWidth() + compWidth) / 2 - 5;
			int cx = x0;
			int cy = bds.getY() + bds.getHeight() - 12;
			int cur = 0;
			for (int k = 0; k < wid; k++) {
				GraphicsUtil.drawCenteredText(g, value.get(k).toBinString(), cx, cy);
				++cur;
				if (cur == 8) {
					cur = 0;
					cx = x0;
					cy -= 20;
				} else cx -= 10;
			}
		} else {
			String text = radix.toString(value);
			GraphicsUtil.drawCenteredText(g, text, bds.getX() + bds.getWidth() / 2, bds.getY() + bds.getHeight() / 2);
		}
	}

	//
	// methods for instances
	//
	@Override
	protected void configureNewInstance(Instance instance) {
		instance.setPorts(new Port[] { new Port(0, 0, Port.INPUT, BitWidth.UNKNOWN) });
		instance.addAttributeListener();
		configureLabel(instance);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == Pin.ATTR_LABEL_LOC) configureLabel(instance);
		else if (attr == StdAttr.FACING || attr == RadixOption.ATTRIBUTE) {
			instance.recomputeBounds();
			configureLabel(instance);
		}
	}

	@Override
	public void propagate(InstanceState state) {
		StateData oldData = (StateData) state.getData();
		WireValue oldValue = oldData == null ? WireValues.NIL : oldData.curValue;
		WireValue newValue = state.getPort(0);
		boolean same = Objects.equals(oldValue, newValue);
		if (!same) {
			if (oldData == null) {
				oldData = new StateData();
				oldData.curValue = newValue;
				state.setData(oldData);
			} else oldData.curValue = newValue;
			int oldWidth = oldValue == null ? 1 : oldValue.getWidth();
			@SuppressWarnings("null")
			int newWidth = newValue.getWidth();
			if (oldWidth != newWidth) {
				ProbeAttributes attrs = (ProbeAttributes) state.getAttributeSet();
				attrs.width = newValue.getBitWidth();
				state.getInstance().recomputeBounds();
				configureLabel(state.getInstance());
			}
			state.fireInvalidated();
		}
	}

	private static WireValue getValue(InstanceState state) {
		StateData data = (StateData) state.getData();
		return data == null ? WireValues.NIL : data.curValue;
	}

	void configureLabel(Instance instance) {
		ProbeAttributes attrs = (ProbeAttributes) instance.getAttributeSet();
		Probe.configureLabel(instance, attrs.labelloc, attrs.facing);
	}

	//
	// static methods
	//
	static Bounds getOffsetBounds(Direction dir, BitWidth width, RadixOption radix) {
		Bounds ret = null;
		int len = radix == null || radix == RadixOption.RADIX_2 ? width.getWidth() : radix.getMaxLength(width);
		if (dir == Direction.East) ret = switch (len) {
			case 0, 1, 2 -> Bounds.create(-20, -10, 20, 20);
			case 3 -> Bounds.create(-30, -10, 30, 20);
			case 4 -> Bounds.create(-40, -10, 40, 20);
			case 5 -> Bounds.create(-50, -10, 50, 20);
			case 6 -> Bounds.create(-60, -10, 60, 20);
			case 7 -> Bounds.create(-70, -10, 70, 20);
			case 8 -> Bounds.create(-80, -10, 80, 20);
			case 9, 10, 11, 12, 13, 14, 15, 16 -> Bounds.create(-80, -20, 80, 40);
			case 17, 18, 19, 20, 21, 22, 23, 24 -> Bounds.create(-80, -30, 80, 60);
			case 25, 26, 27, 28, 29, 30, 31, 32 -> Bounds.create(-80, -40, 80, 80);
			default -> ret;
		};
		else if (dir == Direction.West) ret = switch (len) {
			case 0, 1, 2 -> Bounds.create(0, -10, 20, 20);
			case 3 -> Bounds.create(0, -10, 30, 20);
			case 4 -> Bounds.create(0, -10, 40, 20);
			case 5 -> Bounds.create(0, -10, 50, 20);
			case 6 -> Bounds.create(0, -10, 60, 20);
			case 7 -> Bounds.create(0, -10, 70, 20);
			case 8 -> Bounds.create(0, -10, 80, 20);
			case 9, 10, 11, 12, 13, 14, 15, 16 -> Bounds.create(0, -20, 80, 40);
			case 17, 18, 19, 20, 21, 22, 23, 24 -> Bounds.create(0, -30, 80, 60);
			case 25, 26, 27, 28, 29, 30, 31, 32 -> Bounds.create(0, -40, 80, 80);
			default -> null;
		};
		else if (dir == Direction.South) ret = switch (len) {
			case 0, 1, 2 -> Bounds.create(-10, -20, 20, 20);
			case 3 -> Bounds.create(-15, -20, 30, 20);
			case 4 -> Bounds.create(-20, -20, 40, 20);
			case 5 -> Bounds.create(-25, -20, 50, 20);
			case 6 -> Bounds.create(-30, -20, 60, 20);
			case 7 -> Bounds.create(-35, -20, 70, 20);
			case 8 -> Bounds.create(-40, -20, 80, 20);
			case 9, 10, 11, 12, 13, 14, 15, 16 -> Bounds.create(-40, -40, 80, 40);
			case 17, 18, 19, 20, 21, 22, 23, 24 -> Bounds.create(-40, -60, 80, 60);
			case 25, 26, 27, 28, 29, 30, 31, 32 -> Bounds.create(-40, -80, 80, 80);
			default -> null;
		};
		else if (dir == Direction.North) ret = switch (len) {
			case 0, 1, 2 -> Bounds.create(-10, 0, 20, 20);
			case 3 -> Bounds.create(-15, 0, 30, 20);
			case 4 -> Bounds.create(-20, 0, 40, 20);
			case 5 -> Bounds.create(-25, 0, 50, 20);
			case 6 -> Bounds.create(-30, 0, 60, 20);
			case 7 -> Bounds.create(-35, 0, 70, 20);
			case 8 -> Bounds.create(-40, 0, 80, 20);
			case 9, 10, 11, 12, 13, 14, 15, 16 -> Bounds.create(-40, 0, 80, 40);
			case 17, 18, 19, 20, 21, 22, 23, 24 -> Bounds.create(-40, 0, 80, 60);
			case 25, 26, 27, 28, 29, 30, 31, 32 -> Bounds.create(-40, 0, 80, 80);
			default -> null;
		};
		if (ret == null) return Bounds.create(0, -10, 20, 20); // should never happen
		return ret;
	}

	static void configureLabel(Instance instance, Direction labelLoc, Direction facing) {
		Bounds bds = instance.getBounds();
		int x;
		int y;
		int halign;
		int valign;
		if (labelLoc == Direction.North) {
			halign = TextField.H_CENTER;
			valign = TextField.V_BOTTOM;
			x = bds.getX() + bds.getWidth() / 2;
			y = bds.getY() - 2;
			if (facing == labelLoc) {
				halign = TextField.H_LEFT;
				x += 2;
			}
		} else if (labelLoc == Direction.South) {
			halign = TextField.H_CENTER;
			valign = TextField.V_TOP;
			x = bds.getX() + bds.getWidth() / 2;
			y = bds.getY() + bds.getHeight() + 2;
			if (facing == labelLoc) {
				halign = TextField.H_LEFT;
				x += 2;
			}
		} else if (labelLoc == Direction.East) {
			halign = TextField.H_LEFT;
			valign = TextField.V_CENTER;
			x = bds.getX() + bds.getWidth() + 2;
			y = bds.getY() + bds.getHeight() / 2;
			if (facing == labelLoc) {
				valign = TextField.V_BOTTOM;
				y -= 2;
			}
		} else { // WEST
			halign = TextField.H_RIGHT;
			valign = TextField.V_CENTER;
			x = bds.getX() - 2;
			y = bds.getY() + bds.getHeight() / 2;
			if (facing == labelLoc) {
				valign = TextField.V_BOTTOM;
				y -= 2;
			}
		}

		instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, x, y, halign, valign);
	}
}