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
	using AttributeSet = logisim.data.AttributeSet;
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
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
    using LogisimPlus.Java;

    public class TransmissionGate : InstanceFactory
	{
		internal const int OUTPUT = 0;
		internal const int INPUT = 1;
		internal const int GATE0 = 2;
		internal const int GATE1 = 3;

		public TransmissionGate() : base("Transmission Gate", Strings.getter("transmissionGateComponent"))
		{
			IconName = "transmis.gif";
			setAttributes(new Attribute[] {StdAttr.FACING, Wiring.ATTR_GATE, StdAttr.Width}, new object[] {Direction.East, Wiring.GATE_TOP_LEFT, BitWidth.ONE});
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			updatePorts(instance);
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.FACING || attr == Wiring.ATTR_GATE)
			{
				instance.recomputeBounds();
				updatePorts(instance);
			}
			else if (attr == StdAttr.Width)
			{
				instance.fireInvalidated();
			}
		}

		private void updatePorts(Instance instance)
		{
			int dx = 0;
			int dy = 0;
			Direction facing = (Direction)instance.getAttributeValue(StdAttr.FACING);
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

			Port[] ports = new Port[4];
			ports[OUTPUT] = new Port(0, 0, Port.OUTPUT, StdAttr.Width);
			ports[INPUT] = new Port(40 * dx, 40 * dy, Port.INPUT, StdAttr.Width);
			if (flip)
			{
				ports[GATE1] = new Port(20 * (dx - dy), 20 * (dx + dy), Port.INPUT, 1);
				ports[GATE0] = new Port(20 * (dx + dy), 20 * (-dx + dy), Port.INPUT, 1);
			}
			else
			{
				ports[GATE0] = new Port(20 * (dx - dy), 20 * (dx + dy), Port.INPUT, 1);
				ports[GATE1] = new Port(20 * (dx + dy), 20 * (-dx + dy), Port.INPUT, 1);
			}
			instance.Ports = ports;
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			return Bounds.create(0, -20, 40, 40).rotate(Direction.West, facing, 0, 0);
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
			BitWidth width = state.getAttributeValue(StdAttr.Width);
			Value input = state.getPort(INPUT);
			Value gate0 = state.getPort(GATE0);
			Value gate1 = state.getPort(GATE1);

			if (gate0.FullyDefined && gate1.FullyDefined && gate0 != gate1)
			{
				if (gate0 == Value.TRUE)
				{
					return Value.createUnknown(width);
				}
				else
				{
					return input;
				}
			}
			else
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
			Bounds bds = painter.Bounds;
			object powerLoc = painter.getAttributeValue(Wiring.ATTR_GATE);
			Direction facing = (Direction)painter.getAttributeValue(StdAttr.FACING);
			bool flip = (facing == Direction.South || facing == Direction.West) == (powerLoc == Wiring.GATE_TOP_LEFT);

			int degrees = Direction.West.toDegrees() - facing.toDegrees();
			if (flip)
			{
				degrees += 180;
			}
			double radians = Math.toRadians((degrees + 360) % 360);

			JGraphics g = painter.Graphics;
			g.rotate(radians, bds.X + 20, bds.Y + 20);
			g.translate(bds.X, bds.Y);
			JGraphicsUtil.switchToWidth(g, Wire.WIDTH);

			Color gate0 = g.getColor();
			Color gate1 = gate0;
			Color input = gate0;
			Color output = gate0;
			Color platform = gate0;
			if (!isGhost && painter.ShowState)
			{
				gate0 = painter.getPort(GATE0).Color;
				gate1 = painter.getPort(GATE0).Color;
				input = painter.getPort(INPUT).Color;
				output = painter.getPort(OUTPUT).Color;
				platform = computeOutput(painter).Color;
			}

			g.setColor(flip ? input : output);
			g.drawLine(0, 20, 11, 20);
			g.drawLine(11, 13, 11, 27);

			g.setColor(flip ? output : input);
			g.drawLine(29, 20, 40, 20);
			g.drawLine(29, 13, 29, 27);

			g.setColor(gate0);
			g.drawLine(20, 35, 20, 40);
			JGraphicsUtil.switchToWidth(g, 1);
			g.drawOval(18, 30, 4, 4);
			g.drawLine(10, 30, 30, 30);
			JGraphicsUtil.switchToWidth(g, Wire.WIDTH);

			g.setColor(gate1);
			g.drawLine(20, 9, 20, 0);
			JGraphicsUtil.switchToWidth(g, 1);
			g.drawLine(10, 10, 30, 10);

			g.setColor(platform);
			g.drawLine(9, 12, 31, 12);
			g.drawLine(9, 28, 31, 28);
			if (flip)
			{ // arrow
				g.drawLine(18, 17, 21, 20);
				g.drawLine(18, 23, 21, 20);
			}
			else
			{
				g.drawLine(22, 17, 19, 20);
				g.drawLine(22, 23, 19, 20);
			}

			g.dispose();
		}
	}

}
