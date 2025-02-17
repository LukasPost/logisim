﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.plexers
{

	using LogisimVersion = logisim.LogisimVersion;
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

	public class Decoder : InstanceFactory
	{
		public Decoder() : base("Decoder", Strings.getter("decoderComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, Plexers.ATTR_SELECT_LOC, Plexers.ATTR_SELECT, Plexers.ATTR_TRISTATE, Plexers.ATTR_DISABLED, Plexers.ATTR_ENABLE}, new object[] {Direction.East, Plexers.SELECT_BOTTOM_LEFT, Plexers.DEFAULT_SELECT, Plexers.DEFAULT_TRISTATE, Plexers.DISABLED_FLOATING, true});
			KeyConfigurator = new BitWidthConfigurator(Plexers.ATTR_SELECT, 1, 5, 0);
			IconName = "decoder.gif";
			FacingAttribute = StdAttr.FACING;
		}

		public virtual object getDefaultAttributeValue<T1>(Attribute attr, LogisimVersion ver)
		{
			if (attr == Plexers.ATTR_ENABLE)
			{
				int newer = ver.compareTo(LogisimVersion.get(2, 6, 3, 220));
				return Convert.ToBoolean(newer >= 0);
			}
			else
			{
				return base.getDefaultAttributeValue(attr, ver);
			}
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			object selectLoc = attrs.getValue(Plexers.ATTR_SELECT_LOC);
			BitWidth select = attrs.getValue(Plexers.ATTR_SELECT);
			int outputs = 1 << select.Width;
			Bounds bds;
			bool reversed = facing == Direction.West || facing == Direction.North;
			if (selectLoc == Plexers.SELECT_TOP_RIGHT)
			{
				reversed = !reversed;
			}
			if (outputs == 2)
			{
				int y = reversed ? 0 : -40;
				bds = Bounds.create(-20, y, 30, 40);
			}
			else
			{
				int x = -20;
				int y = reversed ? -10 : -(outputs * 10 + 10);
				bds = Bounds.create(x, y, 40, outputs * 10 + 20);
			}
			return bds.rotate(Direction.East, facing, 0, 0);
		}

		public override bool contains(Location loc, AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING).reverse();
			return Plexers.contains(loc, getOffsetBounds(attrs), facing);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			updatePorts(instance);
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.FACING || attr == Plexers.ATTR_SELECT_LOC || attr == Plexers.ATTR_SELECT)
			{
				instance.recomputeBounds();
				updatePorts(instance);
			}
			else if (attr == Plexers.ATTR_SELECT || attr == Plexers.ATTR_ENABLE)
			{
				updatePorts(instance);
			}
			else if (attr == Plexers.ATTR_TRISTATE || attr == Plexers.ATTR_DISABLED)
			{
				instance.fireInvalidated();
			}
		}

		private void updatePorts(Instance instance)
		{
			Direction facing = instance.getAttributeValue(StdAttr.FACING);
			object selectLoc = instance.getAttributeValue(Plexers.ATTR_SELECT_LOC);
			BitWidth select = instance.getAttributeValue(Plexers.ATTR_SELECT);
			bool enable = (bool)instance.getAttributeValue(Plexers.ATTR_ENABLE);
			int outputs = 1 << select.Width;
			Port[] ps = new Port[outputs + (enable ? 2 : 1)];
			if (outputs == 2)
			{
				Location end0;
				Location end1;
				if (facing == Direction.North || facing == Direction.South)
				{
					int y = facing == Direction.North ? -10 : 10;
					if (selectLoc == Plexers.SELECT_TOP_RIGHT)
					{
						end0 = new Location(-30, y);
						end1 = new Location(-10, y);
					}
					else
					{
						end0 = new Location(10, y);
						end1 = new Location(30, y);
					}
				}
				else
				{
					int x = facing == Direction.West ? -10 : 10;
					if (selectLoc == Plexers.SELECT_TOP_RIGHT)
					{
						end0 = new Location(x, 10);
						end1 = new Location(x, 30);
					}
					else
					{
						end0 = new Location(x, -30);
						end1 = new Location(x, -10);
					}
				}
				ps[0] = new Port(end0.X, end0.Y, Port.OUTPUT, 1);
				ps[1] = new Port(end1.X, end1.Y, Port.OUTPUT, 1);
			}
			else
			{
				int dx;
				int ddx;
				int dy;
				int ddy;
				if (facing == Direction.North || facing == Direction.South)
				{
					dy = facing == Direction.North ? -20 : 20;
					ddy = 0;
					dx = selectLoc == Plexers.SELECT_TOP_RIGHT ? -10 * outputs : 0;
					ddx = 10;
				}
				else
				{
					dx = facing == Direction.West ? -20 : 20;
					ddx = 0;
					dy = selectLoc == Plexers.SELECT_TOP_RIGHT ? 0 : -10 * outputs;
					ddy = 10;
				}
				for (int i = 0; i < outputs; i++)
				{
					ps[i] = new Port(dx, dy, Port.OUTPUT, 1);
					dx += ddx;
					dy += ddy;
				}
			}
			Location en = (new Location(0, 0)).translate(facing, -10);
			ps[outputs] = new Port(0, 0, Port.INPUT, select.Width);
			if (enable)
			{
				ps[outputs + 1] = new Port(en.X, en.Y, Port.INPUT, BitWidth.ONE);
			}
			for (int i = 0; i < outputs; i++)
			{
				ps[i].ToolTip = Strings.getter("decoderOutTip", "" + i);
			}
			ps[outputs].ToolTip = Strings.getter("decoderSelectTip");
			if (enable)
			{
				ps[outputs + 1].ToolTip = Strings.getter("decoderEnableTip");
			}
			instance.Ports = ps;
		}

		public override void propagate(InstanceState state)
		{
			// get attributes
			BitWidth data = BitWidth.ONE;
			BitWidth select = state.getAttributeValue(Plexers.ATTR_SELECT);
			bool? threeState = state.getAttributeValue(Plexers.ATTR_TRISTATE);
			bool enable = (bool)state.getAttributeValue(Plexers.ATTR_ENABLE);
			int outputs = 1 << select.Width;

			// determine default output values
			Value others; // the default output
			if (threeState.Value)
			{
				others = Value.UNKNOWN;
			}
			else
			{
				others = Value.FALSE;
			}

			// determine selected output value
			int outIndex = -1; // the special output
			Value @out = null;
			Value en = enable ? state.getPort(outputs + 1) : Value.TRUE;
			if (en == Value.FALSE)
			{
				object opt = state.getAttributeValue(Plexers.ATTR_DISABLED);
				Value @base = opt == Plexers.DISABLED_ZERO ? Value.FALSE : Value.UNKNOWN;
				others = Value.repeat(@base, data.Width);
			}
			else if (en == Value.ERROR && state.isPortConnected(outputs + 1))
			{
				others = Value.createError(data);
			}
			else
			{
				Value sel = state.getPort(outputs);
				if (sel.FullyDefined)
				{
					outIndex = sel.toIntValue();
					@out = Value.TRUE;
				}
				else if (sel.ErrorValue)
				{
					others = Value.createError(data);
				}
				else
				{
					others = Value.createUnknown(data);
				}
			}

			// now propagate them
			for (int i = 0; i < outputs; i++)
			{
				state.setPort(i, i == outIndex ? @out : others, Plexers.DELAY);
			}
		}

		public override void paintGhost(InstancePainter painter)
		{
			Direction facing = painter.getAttributeValue(StdAttr.FACING);
			BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
			Plexers.drawTrapezoid(painter.Graphics, painter.Bounds, facing.reverse(), select.Width == 1 ? 10 : 20);
		}

		public override void paintInstance(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			Bounds bds = painter.Bounds;
			Direction facing = painter.getAttributeValue(StdAttr.FACING);
			object selectLoc = painter.getAttributeValue(Plexers.ATTR_SELECT_LOC);
			BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
			bool enable = (bool)painter.getAttributeValue(Plexers.ATTR_ENABLE);
			int selMult = selectLoc == Plexers.SELECT_TOP_RIGHT ? -1 : 1;
			int outputs = 1 << select.Width;

			// draw stubs for select and enable ports
			JGraphicsUtil.switchToWidth(g, 3);
			bool vertical = facing == Direction.North || facing == Direction.South;
			int dx = vertical ? selMult : 0;
			int dy = vertical ? 0 : -selMult;
			if (outputs == 2)
			{ // draw select wire
				if (painter.ShowState)
				{
					g.setColor(painter.getPort(outputs).Color);
				}
				Location pt = painter.getInstance().getPortLocation(outputs);
				g.drawLine(pt.X, pt.Y, pt.X + 2 * dx, pt.Y + 2 * dy);
			}
			if (enable)
			{
				Location en = painter.getInstance().getPortLocation(outputs + 1);
				int len = outputs == 2 ? 6 : 4;
				if (painter.ShowState)
				{
					g.setColor(painter.getPort(outputs + 1).Color);
				}
				g.drawLine(en.X, en.Y, en.X + len * dx, en.Y + len * dy);
			}
			JGraphicsUtil.switchToWidth(g, 1);

			// draw a circle indicating where the select input is located
			Multiplexer.drawSelectCircle(g, bds, painter.getInstance().getPortLocation(outputs));

			// draw "0"
			int x0;
			int y0;
			int halign;
			if (facing == Direction.West)
			{
				x0 = 3;
				y0 = 15;
				halign = JGraphicsUtil.H_LEFT;
			}
			else if (facing == Direction.North)
			{
				x0 = 10;
				y0 = 15;
				halign = JGraphicsUtil.H_CENTER;
			}
			else if (facing == Direction.South)
			{
				x0 = 10;
				y0 = bds.Height - 3;
				halign = JGraphicsUtil.H_CENTER;
			}
			else
			{
				x0 = bds.Width - 3;
				y0 = 15;
				halign = JGraphicsUtil.H_RIGHT;
			}
			g.setColor(Color.Gray);
			JGraphicsUtil.drawText(g, "0", bds.X + x0, bds.Y + y0, halign, JGraphicsUtil.V_BASELINE);

			// draw trapezoid, "Decd", and ports
			g.setColor(Color.Black);
			Plexers.drawTrapezoid(g, bds, facing.reverse(), outputs == 2 ? 10 : 20);
			JGraphicsUtil.drawCenteredText(g, "Decd", bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			painter.drawPorts();
		}
	}

}
