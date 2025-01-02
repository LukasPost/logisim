// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using ComponentFactory = logisim.comp.ComponentFactory;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Options = logisim.file.Options;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using WireRepair = logisim.tools.WireRepair;
	using WireRepairData = logisim.tools.WireRepairData;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using Icons = logisim.util.Icons;

	internal class ControlledBuffer : InstanceFactory
	{
		private static readonly AttributeOption RIGHT_HANDED = new AttributeOption("right", Strings.getter("controlledRightHanded"));
		private static readonly AttributeOption LEFT_HANDED = new AttributeOption("left", Strings.getter("controlledLeftHanded"));
		private static readonly Attribute<AttributeOption> ATTR_CONTROL = Attributes.forOption("control", Strings.getter("controlledControlOption"), new AttributeOption[] {RIGHT_HANDED, LEFT_HANDED});

		public static ComponentFactory FACTORY_BUFFER = new ControlledBuffer(false);
		public static ComponentFactory FACTORY_INVERTER = new ControlledBuffer(true);

		private static readonly Icon ICON_BUFFER = Icons.getIcon("controlledBuffer.gif");
		private static readonly Icon ICON_INVERTER = Icons.getIcon("controlledInverter.gif");

		private bool isInverter;

		private ControlledBuffer(bool isInverter) : base(isInverter ? "Controlled Inverter" : "Controlled Buffer", isInverter ? Strings.getter("controlledInverterComponent") : Strings.getter("controlledBufferComponent"))
		{
			this.isInverter = isInverter;
			if (isInverter)
			{
				setAttributes(new Attribute[] {StdAttr.FACING, StdAttr.WIDTH, NotGate.ATTR_SIZE, ATTR_CONTROL, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {Direction.East, BitWidth.ONE, NotGate.SIZE_WIDE, RIGHT_HANDED, "", StdAttr.DEFAULT_LABEL_FONT});
			}
			else
			{
				setAttributes(new Attribute[] {StdAttr.FACING, StdAttr.WIDTH, ATTR_CONTROL, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {Direction.East, BitWidth.ONE, RIGHT_HANDED, "", StdAttr.DEFAULT_LABEL_FONT});
			}
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			int w = 20;
			if (isInverter && !NotGate.SIZE_NARROW.Equals(attrs.getValue(NotGate.ATTR_SIZE)))
			{
				w = 30;
			}
			Direction facing = attrs.getValue(StdAttr.FACING);
			if (facing == Direction.North)
			{
				return Bounds.create(-10, 0, 20, w);
			}
			if (facing == Direction.South)
			{
				return Bounds.create(-10, -w, 20, w);
			}
			if (facing == Direction.West)
			{
				return Bounds.create(0, -10, w, 20);
			}
			return Bounds.create(-w, -10, w, 20);
		}

		//
		// graphics methods
		//
		public override void paintGhost(InstancePainter painter)
		{
			paintShape(painter);
		}

		public override void paintIcon(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			Icon icon = isInverter ? ICON_INVERTER : ICON_BUFFER;
			if (icon != null)
			{
				icon.paintIcon(painter.Destination, g, 2, 2);
			}
			else
			{
				int x = isInverter ? 0 : 2;
				g.setColor(Color.BLACK);
				int[] xp = new int[] {x + 15, x + 1, x + 1, x + 15};
				int[] yp = new int[] {10, 3, 17, 10};
				g.drawPolyline(xp, yp, 4);
				if (isInverter)
				{
					g.drawOval(x + 13, 8, 4, 4);
				}
				g.setColor(Value.FALSE_COLOR);
				g.drawLine(x + 8, 14, x + 8, 18);
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			Direction face = painter.getAttributeValue(StdAttr.FACING);

			Graphics g = painter.Graphics;

			// draw control wire
			GraphicsUtil.switchToWidth(g, 3);
			Location pt0 = painter.getInstance().getPortLocation(2);
			Location pt1;
			if (painter.getAttributeValue(ATTR_CONTROL) == LEFT_HANDED)
			{
				pt1 = pt0.translate(face, 0, 6);
			}
			else
			{
				pt1 = pt0.translate(face, 0, -6);
			}
			if (painter.ShowState)
			{
				g.setColor(painter.getPort(2).Color);
			}
			g.drawLine(pt0.X, pt0.Y, pt1.X, pt1.Y);

			// draw triangle
			g.setColor(Color.BLACK);
			paintShape(painter);

			// draw input and output pins
			if (!painter.PrintView)
			{
				painter.drawPort(0);
				painter.drawPort(1);
			}
			painter.drawLabel();
		}

		private void paintShape(InstancePainter painter)
		{
			Direction facing = painter.getAttributeValue(StdAttr.FACING);
			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			double rotate = 0.0;
			Graphics g = painter.Graphics;
			g.translate(x, y);
			if (facing != Direction.East && g is Graphics2D)
			{
				rotate = -facing.toRadians();
				((Graphics2D) g).rotate(rotate);
			}

			if (isInverter)
			{
				PainterShaped.paintNot(painter);
			}
			else
			{
				GraphicsUtil.switchToWidth(g, 2);
				int d = isInverter ? 10 : 0;
				int[] xp = new int[] {-d, -19 - d, -19 - d, -d};
				int[] yp = new int[] {0, -7, 7, 0};
				g.drawPolyline(xp, yp, 4);
				// if (isInverter) g.drawOval(-9, -4, 9, 9);
			}

			if (rotate != 0.0)
			{
				((Graphics2D) g).rotate(-rotate);
			}
			g.translate(-x, -y);
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			configurePorts(instance);
			NotGate.configureLabel(instance, false, instance.getPortLocation(2));
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == StdAttr.FACING || attr == NotGate.ATTR_SIZE)
			{
				instance.recomputeBounds();
				configurePorts(instance);
				NotGate.configureLabel(instance, false, instance.getPortLocation(2));
			}
			else if (attr == ATTR_CONTROL)
			{
				configurePorts(instance);
				NotGate.configureLabel(instance, false, instance.getPortLocation(2));
			}
		}

		private void configurePorts(Instance instance)
		{
			Direction facing = instance.getAttributeValue(StdAttr.FACING);
			Bounds bds = getOffsetBounds(instance.AttributeSet);
			int d = Math.Max(bds.Width, bds.Height) - 20;
			Location loc0 = new Location(0, 0);
			Location loc1 = loc0.translate(facing.reverse(), 20 + d);
			Location loc2;
			if (instance.getAttributeValue(ATTR_CONTROL) == LEFT_HANDED)
			{
				loc2 = loc0.translate(facing.reverse(), 10 + d, 10);
			}
			else
			{
				loc2 = loc0.translate(facing.reverse(), 10 + d, -10);
			}

			Port[] ports = new Port[3];
			ports[0] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			ports[1] = new Port(loc1.X, loc1.Y, Port.INPUT, StdAttr.WIDTH);
			ports[2] = new Port(loc2.X, loc2.Y, Port.INPUT, 1);
			instance.setPorts(ports);
		}

		public override void propagate(InstanceState state)
		{
			Value control = state.getPort(2);
			BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
			if (control == Value.TRUE)
			{
				Value @in = state.getPort(1);
				state.setPort(0, isInverter ? @in.not() : @in, GateAttributes.DELAY);
			}
			else if (control == Value.ERROR || control == Value.UNKNOWN)
			{
				state.setPort(0, Value.createError(width), GateAttributes.DELAY);
			}
			else
			{
				Value @out;
				if (control == Value.UNKNOWN || control == Value.NIL)
				{
					AttributeSet opts = state.Project.Options.AttributeSet;
					if (opts.getValue(Options.ATTR_GATE_UNDEFINED).Equals(Options.GATE_UNDEFINED_ERROR))
					{
						@out = Value.createError(width);
					}
					else
					{
						@out = Value.createUnknown(width);
					}
				}
				else
				{
					@out = Value.createUnknown(width);
				}
				state.setPort(0, @out, GateAttributes.DELAY);
			}
		}

		public override object getInstanceFeature(in Instance instance, object key)
		{
			if (key == typeof(WireRepair))
			{
				return new WireRepairAnonymousInnerClass(this, instance);
			}
			return base.getInstanceFeature(instance, key);
		}

		private class WireRepairAnonymousInnerClass : WireRepair
		{
			private readonly ControlledBuffer outerInstance;

			private Instance instance;

			public WireRepairAnonymousInnerClass(ControlledBuffer outerInstance, Instance instance)
			{
				this.outerInstance = outerInstance;
				this.instance = instance;
			}

			public bool shouldRepairWire(WireRepairData data)
			{
				Location port2 = instance.getPortLocation(2);
				return data.Point.Equals(port2);
			}
		}
	}

}
