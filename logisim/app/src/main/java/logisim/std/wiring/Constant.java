/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.Color;
import java.awt.Graphics;
import java.util.Arrays;
import java.util.List;
import java.util.Map;

import logisim.analyze.model.Expression;
import logisim.analyze.model.Expressions;
import logisim.circuit.ExpressionComputer;
import logisim.data.AbstractAttributeSet;
import logisim.data.Attribute;
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
import logisim.tools.key.JoinedConfigurator;
import logisim.util.GraphicsUtil;

public class Constant extends InstanceFactory {
	public static final Attribute<Integer> ATTR_VALUE = Attributes.forHexInteger("value",
			Strings.getter("constantValueAttr"));

	public static InstanceFactory FACTORY = new Constant();

	private static final Color BACKGROUND_COLOR = new Color(230, 230, 230);

	private static final List<Attribute<?>> ATTRIBUTES = Arrays
			.asList(new Attribute<?>[] { StdAttr.FACING, StdAttr.WIDTH, ATTR_VALUE });

	private static class ConstantAttributes extends AbstractAttributeSet {
		private Direction facing = Direction.East;
		private BitWidth width = BitWidth.ONE;
		private WireValue value = WireValues.TRUE;

		@Override
		protected void copyInto(AbstractAttributeSet destObj) {
			ConstantAttributes dest = (ConstantAttributes) destObj;
			dest.facing = facing;
			dest.width = width;
			dest.value = value;
		}

		@Override
		public List<Attribute<?>> getAttributes() {
			return ATTRIBUTES;
		}

		@Override
		@SuppressWarnings("unchecked")
		public <V> V getValue(Attribute<V> attr) {
			if (attr == StdAttr.FACING)
				return (V) facing;
			if (attr == StdAttr.WIDTH)
				return (V) width;
			if (attr == ATTR_VALUE)
				return (V) Integer.valueOf(value.toIntValue());
			return null;
		}

		@Override
		public <V> void setValue(Attribute<V> attr, V value) {
			if (attr == StdAttr.FACING) facing = (Direction) value;
			else if (attr == StdAttr.WIDTH) {
				width = (BitWidth) value;
				this.value = this.value.extendWidth(width.getWidth(), this.value.get(this.value.getWidth() - 1));
			} else if (attr == ATTR_VALUE) {
				int val = (Integer) value;
				this.value = WireValue.Companion.createKnown(width, val);
			} else throw new IllegalArgumentException("unknown attribute " + attr);
			fireAttributeValueChanged(attr, value);
		}
	}

	private static class ConstantExpression implements ExpressionComputer {
		private Instance instance;

		public ConstantExpression(Instance instance) {
			this.instance = instance;
		}

		public void computeExpression(Map<Location, Expression> expressionMap) {
			AttributeSet attrs = instance.getAttributeSet();
			int intValue = attrs.getValue(ATTR_VALUE);

			expressionMap.put(instance.getLocation(), Expressions.constant(intValue));
		}
	}

	public Constant() {
		super("Constant", Strings.getter("constantComponent"));
		setFacingAttribute(StdAttr.FACING);
		setKeyConfigurator(
				JoinedConfigurator.create(new ConstantConfigurator(), new BitWidthConfigurator(StdAttr.WIDTH)));
	}

	@Override
	public AttributeSet createAttributeSet() {
		return new ConstantAttributes();
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		instance.addAttributeListener();
		updatePorts(instance);
	}

	private void updatePorts(Instance instance) {
		Port[] ps = { new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH) };
		instance.setPorts(ps);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.WIDTH) {
			instance.recomputeBounds();
			updatePorts(instance);
		} else if (attr == StdAttr.FACING) instance.recomputeBounds();
		else if (attr == ATTR_VALUE) instance.fireInvalidated();
	}

	@Override
	protected Object getInstanceFeature(Instance instance, Object key) {
		if (key == ExpressionComputer.class)
			return new ConstantExpression(instance);
		return super.getInstanceFeature(instance, key);
	}

	@Override
	public void propagate(InstanceState state) {
		BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
		int value = state.getAttributeValue(ATTR_VALUE);
		state.setPort(0, WireValue.Companion.createKnown(width, value), 1);
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		BitWidth width = attrs.getValue(StdAttr.WIDTH);
		int chars = (width.getWidth() + 3) / 4;

		Bounds ret = null;
		if (facing == Direction.East) ret = switch (chars) {
			case 1, 2 -> Bounds.create(-16, -8, 16, 16);
			case 3 -> Bounds.create(-26, -8, 26, 16);
			case 4 -> Bounds.create(-36, -8, 36, 16);
			case 5 -> Bounds.create(-46, -8, 46, 16);
			case 6 -> Bounds.create(-56, -8, 56, 16);
			case 7 -> Bounds.create(-66, -8, 66, 16);
			case 8 -> Bounds.create(-76, -8, 76, 16);
			default -> null;
		};
		else if (facing == Direction.West) ret = switch (chars) {
			case 1, 2 -> Bounds.create(0, -8, 16, 16);
			case 3 -> Bounds.create(0, -8, 26, 16);
			case 4 -> Bounds.create(0, -8, 36, 16);
			case 5 -> Bounds.create(0, -8, 46, 16);
			case 6 -> Bounds.create(0, -8, 56, 16);
			case 7 -> Bounds.create(0, -8, 66, 16);
			case 8 -> Bounds.create(0, -8, 76, 16);
			default -> null;
		};
		else if (facing == Direction.South) ret = switch (chars) {
			case 1, 2 -> Bounds.create(-8, -16, 16, 16);
			case 3 -> Bounds.create(-13, -16, 26, 16);
			case 4 -> Bounds.create(-18, -16, 36, 16);
			case 5 -> Bounds.create(-23, -16, 46, 16);
			case 6 -> Bounds.create(-28, -16, 56, 16);
			case 7 -> Bounds.create(-33, -16, 66, 16);
			case 8 -> Bounds.create(-38, -16, 76, 16);
			default -> null;
		};
		else if (facing == Direction.North) ret = switch (chars) {
			case 1, 2 -> Bounds.create(-8, 0, 16, 16);
			case 3 -> Bounds.create(-13, 0, 26, 16);
			case 4 -> Bounds.create(-18, 0, 36, 16);
			case 5 -> Bounds.create(-23, 0, 46, 16);
			case 6 -> Bounds.create(-28, 0, 56, 16);
			case 7 -> Bounds.create(-33, 0, 66, 16);
			case 8 -> Bounds.create(-38, 0, 76, 16);
			default -> null;
		};
		if (ret == null) throw new IllegalArgumentException("unrecognized arguments " + facing + " " + width);
		return ret;
	}

	//
	// painting methods
	//
	@Override
	public void paintIcon(InstancePainter painter) {
		int w = painter.getAttributeValue(StdAttr.WIDTH).getWidth();
		int pinx = 16;
		int piny = 9;
		Direction dir = painter.getAttributeValue(StdAttr.FACING);
		if (dir == Direction.East) {
		} // keep defaults
		else if (dir == Direction.West) pinx = 4;
		else if (dir == Direction.North) {
			pinx = 9;
			piny = 4;
		} else if (dir == Direction.South) {
			pinx = 9;
			piny = 16;
		}

		Graphics g = painter.getGraphics();
		if (w == 1) {
			int v = painter.getAttributeValue(ATTR_VALUE);
			WireValue val = v == 1 ? WireValues.TRUE : WireValues.FALSE;
			g.setColor(val.getColor());
			GraphicsUtil.drawCenteredText(g, "" + v, 10, 9);
		} else {
			g.setFont(g.getFont().deriveFont(9.0f));
			GraphicsUtil.drawCenteredText(g, "x" + w, 10, 9);
		}
		g.fillOval(pinx, piny, 3, 3);
	}

	@Override
	public void paintGhost(InstancePainter painter) {
		int v = painter.getAttributeValue(ATTR_VALUE);
		String vStr = Integer.toHexString(v);
		Bounds bds = getOffsetBounds(painter.getAttributeSet());

		Graphics g = painter.getGraphics();
		GraphicsUtil.switchToWidth(g, 2);
		g.fillOval(-2, -2, 5, 5);
		GraphicsUtil.drawCenteredText(g, vStr, bds.getX() + bds.getWidth() / 2, bds.getY() + bds.getHeight() / 2);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Bounds bds = painter.getOffsetBounds();
		BitWidth width = painter.getAttributeValue(StdAttr.WIDTH);
		int intValue = painter.getAttributeValue(ATTR_VALUE);
		WireValue v = WireValue.Companion.createKnown(width, intValue);
		Location loc = painter.getLocation();
		int x = loc.x();
		int y = loc.y();

		Graphics g = painter.getGraphics();
		if (painter.shouldDrawColor()) {
			g.setColor(BACKGROUND_COLOR);
			g.fillRect(x + bds.getX(), y + bds.getY(), bds.getWidth(), bds.getHeight());
		}
		if (v.getWidth() == 1) {
			if (painter.shouldDrawColor())
				g.setColor(v.getColor());
			GraphicsUtil.drawCenteredText(g, v.toString(), x + bds.getX() + bds.getWidth() / 2,
					y + bds.getY() + bds.getHeight() / 2 - 2);
		} else {
			g.setColor(Color.BLACK);
			GraphicsUtil.drawCenteredText(g, v.toHexString(), x + bds.getX() + bds.getWidth() / 2,
					y + bds.getY() + bds.getHeight() / 2 - 2);
		}
		painter.drawPorts();
	}

	// TODO: Allow editing of value via text tool/attribute table
}
