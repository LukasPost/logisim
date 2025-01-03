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

	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

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

	public class Ground : InstanceFactory
	{
		public Ground() : base("Ground", Strings.getter("groundComponent"))
		{
			IconName = "ground.gif";
			setAttributes(new Attribute[] {StdAttr.FACING, StdAttr.Width}, new object[] {Direction.South, BitWidth.ONE});
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
			setPorts(new Port[] {new Port(0, 0, Port.OUTPUT, StdAttr.Width)});
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
			}
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			return Bounds.create(0, -8, 14, 16).rotate(Direction.East, (Direction)attrs.getValue(StdAttr.FACING), 0, 0);
		}

		public override void propagate(InstanceState state)
		{
			BitWidth width = (BitWidth)state.getAttributeValue(StdAttr.Width);
			state.setPort(0, Value.repeat(Value.FALSE, width.Width), 1);
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
			JGraphics g = painter.Graphics;
			Location loc = painter.Location;
			g.translate(loc.X, loc.Y);

			Direction from = (Direction)painter.getAttributeValue(StdAttr.FACING);
			int degrees = Direction.East.toDegrees() - from.toDegrees();
			double radians = Math.toRadians((degrees + 360) % 360);
			g.rotate(radians);

			JGraphicsUtil.switchToWidth(g, Wire.WIDTH);
			if (!isGhost && painter.ShowState)
			{
				g.setColor(painter.getPort(0).Color);
			}
			g.drawLine(0, 0, 5, 0);

			JGraphicsUtil.switchToWidth(g, 1);
			if (!isGhost && painter.shouldDrawColor())
			{
				BitWidth width = (BitWidth)painter.getAttributeValue(StdAttr.Width);
				g.setColor(Value.repeat(Value.FALSE, width.Width).Color);
			}
			g.drawLine(6, -8, 6, 8);
			g.drawLine(9, -5, 9, 5);
			g.drawLine(12, -2, 12, 2);

			g.dispose();
		}
	}

}
