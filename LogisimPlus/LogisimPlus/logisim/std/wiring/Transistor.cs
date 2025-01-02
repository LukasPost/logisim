// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

/// <summary>
/// Based on PUCTools (v0.9 beta) by CRC - PUC - Minas (pucmg.crc at gmail.com)
/// </summary>

namespace logisim.std.wiring
{

	using Wire = logisim.circuit.Wire;
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
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using Icons = logisim.util.Icons;

	public class Transistor : InstanceFactory
	{
		internal static readonly AttributeOption TYPE_P = new AttributeOption("p", Strings.getter("transistorTypeP"));
		internal static readonly AttributeOption TYPE_N = new AttributeOption("n", Strings.getter("transistorTypeN"));
		internal static readonly Attribute<AttributeOption> ATTR_TYPE = Attributes.forOption("type", Strings.getter("transistorTypeAttr"), new AttributeOption[] {TYPE_P, TYPE_N});

		internal const int OUTPUT = 0;
		internal const int INPUT = 1;
		internal const int GATE = 2;

		private static readonly Icon ICON_N = Icons.getIcon("trans1.gif");
		private static readonly Icon ICON_P = Icons.getIcon("trans0.gif");

		public Transistor() : base("Transistor", Strings.getter("transistorComponent"))
		{
			setAttributes(new Attribute[] {ATTR_TYPE, StdAttr.FACING, Wiring.ATTR_GATE, StdAttr.WIDTH}, new object[] {TYPE_P, Direction.East, Wiring.GATE_TOP_LEFT, BitWidth.ONE});
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			updatePorts(instance);
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == StdAttr.FACING || attr == Wiring.ATTR_GATE)
			{
				instance.recomputeBounds();
				updatePorts(instance);
			}
			else if (attr == StdAttr.WIDTH)
			{
				updatePorts(instance);
			}
			else if (attr == ATTR_TYPE)
			{
				instance.fireInvalidated();
			}
		}

		private void updatePorts(Instance instance)
		{
			Direction facing = instance.getAttributeValue(StdAttr.FACING);
			int dx = 0;
			int dy = 0;
			if (facing == Direction.North)
			{
				dy = 1;
			}
			else if (facing == Direction.East)
			{
				dx = -1;
			}
			else if (facing == Direction.South)
			{
				dy = -1;
			}
			else if (facing == Direction.West)
			{
				dx = 1;
			}

			object powerLoc = instance.getAttributeValue(Wiring.ATTR_GATE);
			bool flip = (facing == Direction.South || facing == Direction.West) == (powerLoc == Wiring.GATE_TOP_LEFT);

			Port[] ports = new Port[3];
			ports[OUTPUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			ports[INPUT] = new Port(40 * dx, 40 * dy, Port.INPUT, StdAttr.WIDTH);
			if (flip)
			{
				ports[GATE] = new Port(20 * (dx + dy), 20 * (-dx + dy), Port.INPUT, 1);
			}
			else
			{
				ports[GATE] = new Port(20 * (dx - dy), 20 * (dx + dy), Port.INPUT, 1);
			}
			instance.setPorts(ports);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			object gateLoc = attrs.getValue(Wiring.ATTR_GATE);
			int delta = gateLoc == Wiring.GATE_TOP_LEFT ? -20 : 0;
			if (facing == Direction.North)
			{
				return Bounds.create(delta, 0, 20, 40);
			}
			else if (facing == Direction.South)
			{
				return Bounds.create(delta, -40, 20, 40);
			}
			else if (facing == Direction.West)
			{
				return Bounds.create(0, delta, 40, 20);
			}
			else
			{ // facing == Direction.EAST
				return Bounds.create(-40, delta, 40, 20);
			}
		}

		public override bool contains(Location loc, AttributeSet attrs)
		{
			if (base.contains(loc, attrs))
			{
				Direction facing = attrs.getValue(StdAttr.FACING);
				Location center = (new Location(0, 0)).translate(facing, -20);
				return center.manhattanDistanceTo(loc) < 24;
			}
			else
			{
				return false;
			}
		}

		public override void propagate(InstanceState state)
		{
			state.setPort(OUTPUT, computeOutput(state), 1);
		}

		private Value computeOutput(InstanceState state)
		{
			BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
			Value gate = state.getPort(GATE);
			Value input = state.getPort(INPUT);
			Value desired = state.getAttributeValue(ATTR_TYPE) == TYPE_P ? Value.FALSE : Value.TRUE;

			if (!gate.FullyDefined)
			{
				if (input.FullyDefined)
				{
					return Value.createError(width);
				}
				else
				{
					Value[] v = input.All;
					for (int i = 0; i < v.Length; i++)
					{
						if (v[i] != Value.UNKNOWN)
						{
							v[i] = Value.ERROR;
						}
					}
					return Value.create(v);
				}
			}
			else if (gate != desired)
			{
				return Value.createUnknown(width);
			}
			else
			{
				return input;
			}
		}

		public override void paintIcon(InstancePainter painter)
		{
			object type = painter.getAttributeValue(ATTR_TYPE);
			Icon icon = type == TYPE_N ? ICON_N : ICON_P;
			icon.paintIcon(painter.Destination, painter.Graphics, 2, 2);
		}

		public override void paintInstance(InstancePainter painter)
		{
			drawInstance(painter, false);
			painter.drawPorts();
		}

		public override void paintGhost(InstancePainter painter)
		{
			drawInstance(painter, true);
		}

		private void drawInstance(InstancePainter painter, bool isGhost)
		{
			object type = painter.getAttributeValue(ATTR_TYPE);
			object powerLoc = painter.getAttributeValue(Wiring.ATTR_GATE);
			Direction from = painter.getAttributeValue(StdAttr.FACING);
			Direction facing = painter.getAttributeValue(StdAttr.FACING);
			bool flip = (facing == Direction.South || facing == Direction.West) == (powerLoc == Wiring.GATE_TOP_LEFT);

			int degrees = Direction.East.toDegrees() - from.toDegrees();
			double radians = Math.toRadians((degrees + 360) % 360);
			int m = flip ? 1 : -1;

			Graphics2D g = (Graphics2D) painter.Graphics;
			Location loc = painter.Location;
			g.translate(loc.X, loc.Y);
			g.rotate(radians);

			Color gate;
			Color input;
			Color output;
			Color platform;
			if (!isGhost && painter.ShowState)
			{
				gate = painter.getPort(GATE).Color;
				input = painter.getPort(INPUT).Color;
				output = painter.getPort(OUTPUT).Color;
				Value @out = computeOutput(painter);
				platform = @out.Unknown ? Value.UNKNOWN.Color : @out.Color;
			}
			else
			{
				Color @base = g.getColor();
				gate = @base;
				input = @base;
				output = @base;
				platform = @base;
			}

			// input and output lines
			GraphicsUtil.switchToWidth(g, Wire.WIDTH);
			g.setColor(output);
			g.drawLine(0, 0, -11, 0);
			g.drawLine(-11, m * 7, -11, 0);

			g.setColor(input);
			g.drawLine(-40, 0, -29, 0);
			g.drawLine(-29, m * 7, -29, 0);

			// gate line
			g.setColor(gate);
			if (type == TYPE_P)
			{
				g.drawLine(-20, m * 20, -20, m * 15);
				GraphicsUtil.switchToWidth(g, 1);
				g.drawOval(-22, m * 12 - 2, 4, 4);
			}
			else
			{
				g.drawLine(-20, m * 20, -20, m * 11);
				GraphicsUtil.switchToWidth(g, 1);
			}

			// draw platforms
			g.drawLine(-10, m * 10, -30, m * 10); // gate platform
			g.setColor(platform);
			g.drawLine(-9, m * 8, -31, m * 8); // input/output platform

			// arrow (same color as platform)
			g.drawLine(-21, m * 6, -18, m * 3);
			g.drawLine(-21, 0, -18, m * 3);

			g.rotate(-radians);
			g.translate(-loc.X, -loc.Y);
		}
	}

}
