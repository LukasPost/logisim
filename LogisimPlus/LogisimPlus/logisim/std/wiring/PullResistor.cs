// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
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
	using AppPreferences = logisim.prefs.AppPreferences;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using Icons = logisim.util.Icons;

	public class PullResistor : InstanceFactory
	{
		public static readonly Attribute<AttributeOption> ATTR_PULL_TYPE = Attributes.forOption("pull", Strings.getter("pullTypeAttr"), new AttributeOption[]
		{
			new AttributeOption(Value.FALSE, "0", Strings.getter("pullZeroType")),
			new AttributeOption(Value.TRUE, "1", Strings.getter("pullOneType")),
			new AttributeOption(Value.ERROR, "X", Strings.getter("pullErrorType"))
		});

		public static readonly PullResistor FACTORY = new PullResistor();

		private static readonly Icon ICON_SHAPED = Icons.getIcon("pullshap.gif");
		private static readonly Icon ICON_RECTANGULAR = Icons.getIcon("pullrect.gif");

		public PullResistor() : base("Pull Resistor", Strings.getter("pullComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, ATTR_PULL_TYPE}, new object[] {Direction.South, ATTR_PULL_TYPE.parse("0")});
			FacingAttribute = StdAttr.FACING;
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			if (facing == Direction.East)
			{
				return Bounds.create(-42, -6, 42, 12);
			}
			else if (facing == Direction.West)
			{
				return Bounds.create(0, -6, 42, 12);
			}
			else if (facing == Direction.North)
			{
				return Bounds.create(-6, 0, 12, 42);
			}
			else
			{
				return Bounds.create(-6, -42, 12, 42);
			}
		}

		//
		// graphics methods
		//
		public override void paintIcon(InstancePainter painter)
		{
			Icon icon;
			if (painter.GateShape == AppPreferences.SHAPE_SHAPED)
			{
				icon = ICON_SHAPED;
			}
			else
			{
				icon = ICON_RECTANGULAR;
			}
			icon.paintIcon(painter.Destination, painter.Graphics, 2, 2);
		}

		public override void paintGhost(InstancePainter painter)
		{
			Value pull = getPullValue(painter.AttributeSet);
			paintBase(painter, pull, null, null);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			Graphics g = painter.Graphics;
			g.translate(x, y);
			Value pull = getPullValue(painter.AttributeSet);
			Value actual = painter.getPort(0);
			paintBase(painter, pull, pull.Color, actual.Color);
			g.translate(-x, -y);
			painter.drawPorts();
		}

		private void paintBase(InstancePainter painter, Value pullValue, Color inColor, Color outColor)
		{
			bool color = painter.shouldDrawColor();
			Direction facing = painter.getAttributeValue(StdAttr.FACING);
			Graphics g = painter.Graphics;
			Color baseColor = g.getColor();
			GraphicsUtil.switchToWidth(g, 3);
			if (color && inColor != null)
			{
				g.setColor(inColor);
			}
			if (facing == Direction.East)
			{
				GraphicsUtil.drawText(g, pullValue.toDisplayString(), -32, 0, GraphicsUtil.H_RIGHT, GraphicsUtil.V_CENTER);
			}
			else if (facing == Direction.West)
			{
				GraphicsUtil.drawText(g, pullValue.toDisplayString(), 32, 0, GraphicsUtil.H_LEFT, GraphicsUtil.V_CENTER);
			}
			else if (facing == Direction.North)
			{
				GraphicsUtil.drawText(g, pullValue.toDisplayString(), 0, 32, GraphicsUtil.H_CENTER, GraphicsUtil.V_TOP);
			}
			else
			{
				GraphicsUtil.drawText(g, pullValue.toDisplayString(), 0, -32, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
			}

			double rotate = 0.0;
			if (g is Graphics2D)
			{
				rotate = Direction.South.toRadians() - facing.toRadians();
				if (rotate != 0.0)
				{
					((Graphics2D) g).rotate(rotate);
				}
			}
			g.drawLine(0, -30, 0, -26);
			g.drawLine(-6, -30, 6, -30);
			if (color && outColor != null)
			{
				g.setColor(outColor);
			}
			g.drawLine(0, -4, 0, 0);
			g.setColor(baseColor);
			GraphicsUtil.switchToWidth(g, 2);
			if (painter.GateShape == AppPreferences.SHAPE_SHAPED)
			{
				int[] xp = new int[] {0, -5, 5, -5, 5, -5, 0};
				int[] yp = new int[] {-25, -23, -19, -15, -11, -7, -5};
				g.drawPolyline(xp, yp, xp.Length);
			}
			else
			{
				g.drawRect(-5, -25, 10, 20);
			}
			if (rotate != 0.0)
			{
				((Graphics2D) g).rotate(-rotate);
			}
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			instance.setPorts(new Port[] {new Port(0, 0, Port.INOUT, BitWidth.UNKNOWN)});
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
			}
			else if (attr == ATTR_PULL_TYPE)
			{
				instance.fireInvalidated();
			}
		}

		public override void propagate(InstanceState state)
		{
			; // nothing to do - handled by CircuitWires
		}

		public static Value getPullValue(Instance instance)
		{
			return getPullValue(instance.AttributeSet);
		}

		private static Value getPullValue(AttributeSet attrs)
		{
			AttributeOption opt = attrs.getValue(ATTR_PULL_TYPE);
			return (Value) opt.Value;
		}
	}
}
