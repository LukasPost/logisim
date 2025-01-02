// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using Expression = logisim.analyze.model.Expression;
	using Expressions = logisim.analyze.model.Expressions;
	using ExpressionComputer = logisim.circuit.ExpressionComputer;
	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class Constant : InstanceFactory
	{
		public static readonly Attribute<int> ATTR_VALUE = Attributes.forHexInteger("value", Strings.getter("constantValueAttr"));

		public static InstanceFactory FACTORY = new Constant();

		private static readonly Color BACKGROUND_COLOR = new Color(230, 230, 230);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRIBUTES = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { logisim.instance.StdAttr.FACING, logisim.instance.StdAttr.WIDTH, ATTR_VALUE });
		private static readonly IList<Attribute<object>> ATTRIBUTES = new List<Attribute<object>> {StdAttr.FACING, StdAttr.WIDTH, ATTR_VALUE};

		private class ConstantAttributes : AbstractAttributeSet
		{
			internal Direction facing = Direction.East;
			internal BitWidth width = BitWidth.ONE;
			internal Value value = Value.TRUE;

			protected internal override void copyInto(AbstractAttributeSet destObj)
			{
				ConstantAttributes dest = (ConstantAttributes) destObj;
				dest.facing = this.facing;
				dest.width = this.width;
				dest.value = this.value;
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
			public override IList<Attribute<object>> Attributes
			{
				get
				{
					return ATTRIBUTES;
				}
			}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
			public override V getValue<V>(Attribute<V> attr)
			{
				if (attr == StdAttr.FACING)
				{
					return (V) facing;
				}
				if (attr == StdAttr.WIDTH)
				{
					return (V) width;
				}
				if (attr == ATTR_VALUE)
				{
					return (V) Convert.ToInt32(value.toIntValue());
				}
				return null;
			}

			public override void setValue<V>(Attribute<V> attr, V value)
			{
				if (attr == StdAttr.FACING)
				{
					facing = (Direction) value;
				}
				else if (attr == StdAttr.WIDTH)
				{
					width = (BitWidth) value;
					this.value = this.value.extendWidth(width.Width, this.value.get(this.value.Width - 1));
				}
				else if (attr == ATTR_VALUE)
				{
					int val = ((int?) value).Value;
					this.value = Value.createKnown(width, val);
				}
				else
				{
					throw new System.ArgumentException("unknown attribute " + attr);
				}
				fireAttributeValueChanged(attr, value);
			}
		}

		private class ConstantExpression : ExpressionComputer
		{
			internal Instance instance;

			public ConstantExpression(Instance instance)
			{
				this.instance = instance;
			}

			public virtual void computeExpression(IDictionary<Location, Expression> expressionMap)
			{
				AttributeSet attrs = instance.AttributeSet;
				int intValue = (int)attrs.getValue(ATTR_VALUE);

				expressionMap[instance.Location] = Expressions.constant(intValue);
			}
		}

		public Constant() : base("Constant", Strings.getter("constantComponent"))
		{
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = JoinedConfigurator.create(new ConstantConfigurator(), new BitWidthConfigurator(StdAttr.WIDTH));
		}

		public override AttributeSet createAttributeSet()
		{
			return new ConstantAttributes();
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			updatePorts(instance);
		}

		private void updatePorts(Instance instance)
		{
			Port[] ps = new Port[] {new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH)};
			instance.setPorts(ps);
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == StdAttr.WIDTH)
			{
				instance.recomputeBounds();
				updatePorts(instance);
			}
			else if (attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
			}
			else if (attr == ATTR_VALUE)
			{
				instance.fireInvalidated();
			}
		}

		protected internal override object getInstanceFeature(Instance instance, object key)
		{
			if (key == typeof(ExpressionComputer))
			{
				return new ConstantExpression(instance);
			}
			return base.getInstanceFeature(instance, key);
		}

		public override void propagate(InstanceState state)
		{
			BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
			int value = (int)state.getAttributeValue(ATTR_VALUE);
			state.setPort(0, Value.createKnown(width, value), 1);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			BitWidth width = attrs.getValue(StdAttr.WIDTH);
			int chars = (width.Width + 3) / 4;

			Bounds ret = null;
			if (facing == Direction.East)
			{
				switch (chars)
				{
				case 1:
					ret = Bounds.create(-16, -8, 16, 16);
					break;
				case 2:
					ret = Bounds.create(-16, -8, 16, 16);
					break;
				case 3:
					ret = Bounds.create(-26, -8, 26, 16);
					break;
				case 4:
					ret = Bounds.create(-36, -8, 36, 16);
					break;
				case 5:
					ret = Bounds.create(-46, -8, 46, 16);
					break;
				case 6:
					ret = Bounds.create(-56, -8, 56, 16);
					break;
				case 7:
					ret = Bounds.create(-66, -8, 66, 16);
					break;
				case 8:
					ret = Bounds.create(-76, -8, 76, 16);
					break;
				}
			}
			else if (facing == Direction.West)
			{
				switch (chars)
				{
				case 1:
					ret = Bounds.create(0, -8, 16, 16);
					break;
				case 2:
					ret = Bounds.create(0, -8, 16, 16);
					break;
				case 3:
					ret = Bounds.create(0, -8, 26, 16);
					break;
				case 4:
					ret = Bounds.create(0, -8, 36, 16);
					break;
				case 5:
					ret = Bounds.create(0, -8, 46, 16);
					break;
				case 6:
					ret = Bounds.create(0, -8, 56, 16);
					break;
				case 7:
					ret = Bounds.create(0, -8, 66, 16);
					break;
				case 8:
					ret = Bounds.create(0, -8, 76, 16);
					break;
				}
			}
			else if (facing == Direction.South)
			{
				switch (chars)
				{
				case 1:
					ret = Bounds.create(-8, -16, 16, 16);
					break;
				case 2:
					ret = Bounds.create(-8, -16, 16, 16);
					break;
				case 3:
					ret = Bounds.create(-13, -16, 26, 16);
					break;
				case 4:
					ret = Bounds.create(-18, -16, 36, 16);
					break;
				case 5:
					ret = Bounds.create(-23, -16, 46, 16);
					break;
				case 6:
					ret = Bounds.create(-28, -16, 56, 16);
					break;
				case 7:
					ret = Bounds.create(-33, -16, 66, 16);
					break;
				case 8:
					ret = Bounds.create(-38, -16, 76, 16);
					break;
				}
			}
			else if (facing == Direction.North)
			{
				switch (chars)
				{
				case 1:
					ret = Bounds.create(-8, 0, 16, 16);
					break;
				case 2:
					ret = Bounds.create(-8, 0, 16, 16);
					break;
				case 3:
					ret = Bounds.create(-13, 0, 26, 16);
					break;
				case 4:
					ret = Bounds.create(-18, 0, 36, 16);
					break;
				case 5:
					ret = Bounds.create(-23, 0, 46, 16);
					break;
				case 6:
					ret = Bounds.create(-28, 0, 56, 16);
					break;
				case 7:
					ret = Bounds.create(-33, 0, 66, 16);
					break;
				case 8:
					ret = Bounds.create(-38, 0, 76, 16);
					break;
				}
			}
			if (ret == null)
			{
				throw new System.ArgumentException("unrecognized arguments " + facing + " " + width);
			}
			return ret;
		}

		//
		// painting methods
		//
		public override void paintIcon(InstancePainter painter)
		{
			int w = painter.getAttributeValue(StdAttr.WIDTH).getWidth();
			int pinx = 16;
			int piny = 9;
			Direction dir = painter.getAttributeValue(StdAttr.FACING);
			if (dir == Direction.East)
			{
			} // keep defaults
			else if (dir == Direction.West)
			{
				pinx = 4;
			}
			else if (dir == Direction.North)
			{
				pinx = 9;
				piny = 4;
			}
			else if (dir == Direction.South)
			{
				pinx = 9;
				piny = 16;
			}

			Graphics g = painter.Graphics;
			if (w == 1)
			{
				int v = (int)painter.getAttributeValue(ATTR_VALUE);
				Value val = v == 1 ? Value.TRUE : Value.FALSE;
				g.setColor(val.Color);
				GraphicsUtil.drawCenteredText(g, "" + v, 10, 9);
			}
			else
			{
				g.setFont(g.getFont().deriveFont(9.0f));
				GraphicsUtil.drawCenteredText(g, "x" + w, 10, 9);
			}
			g.fillOval(pinx, piny, 3, 3);
		}

		public override void paintGhost(InstancePainter painter)
		{
			int v = (int)painter.getAttributeValue(ATTR_VALUE);
			string vStr = Convert.ToString(v, 16);
			Bounds bds = getOffsetBounds(painter.AttributeSet);

			Graphics g = painter.Graphics;
			GraphicsUtil.switchToWidth(g, 2);
			g.fillOval(-2, -2, 5, 5);
			GraphicsUtil.drawCenteredText(g, vStr, bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Bounds bds = painter.OffsetBounds;
			BitWidth width = painter.getAttributeValue(StdAttr.WIDTH);
			int intValue = (int)painter.getAttributeValue(ATTR_VALUE);
			Value v = Value.createKnown(width, intValue);
			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;

			Graphics g = painter.Graphics;
			if (painter.shouldDrawColor())
			{
				g.setColor(BACKGROUND_COLOR);
				g.fillRect(x + bds.X, y + bds.Y, bds.Width, bds.Height);
			}
			if (v.Width == 1)
			{
				if (painter.shouldDrawColor())
				{
					g.setColor(v.Color);
				}
				GraphicsUtil.drawCenteredText(g, v.ToString(), x + bds.X + bds.Width / 2, y + bds.Y + bds.Height / 2 - 2);
			}
			else
			{
				g.setColor(Color.BLACK);
				GraphicsUtil.drawCenteredText(g, v.toHexString(), x + bds.X + bds.Width / 2, y + bds.Y + bds.Height / 2 - 2);
			}
			painter.drawPorts();
		}

		// TODO: Allow editing of value via text tool/attribute table
	}

}
