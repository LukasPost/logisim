// ====================================================================================================
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
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class Demultiplexer : InstanceFactory
	{
		public Demultiplexer() : base("Demultiplexer", Strings.getter("demultiplexerComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, Plexers.ATTR_SELECT_LOC, Plexers.ATTR_SELECT, StdAttr.WIDTH, Plexers.ATTR_TRISTATE, Plexers.ATTR_DISABLED, Plexers.ATTR_ENABLE}, new object[] {Direction.East, Plexers.SELECT_BOTTOM_LEFT, Plexers.DEFAULT_SELECT, BitWidth.ONE, Plexers.DEFAULT_TRISTATE, Plexers.DISABLED_FLOATING, true});
			KeyConfigurator = JoinedConfigurator.create(new BitWidthConfigurator(Plexers.ATTR_SELECT, 1, 5, 0), new BitWidthConfigurator(StdAttr.WIDTH));
			FacingAttribute = StdAttr.FACING;
			IconName = "demultiplexer.gif";
		}

		public virtual object getDefaultAttributeValue<T1>(Attribute<T1> attr, LogisimVersion ver)
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
			BitWidth select = attrs.getValue(Plexers.ATTR_SELECT);
			int outputs = 1 << select.Width;
			Bounds bds;
			if (outputs == 2)
			{
				bds = Bounds.create(0, -20, 30, 40);
			}
			else
			{
				bds = Bounds.create(0, -(outputs / 2) * 10 - 10, 40, outputs * 10 + 20);
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

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == StdAttr.FACING || attr == Plexers.ATTR_SELECT_LOC || attr == Plexers.ATTR_SELECT)
			{
				instance.recomputeBounds();
				updatePorts(instance);
			}
			else if (attr == StdAttr.WIDTH || attr == Plexers.ATTR_ENABLE)
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
			BitWidth data = instance.getAttributeValue(StdAttr.WIDTH);
			BitWidth select = instance.getAttributeValue(Plexers.ATTR_SELECT);
			bool enable = (bool)instance.getAttributeValue(Plexers.ATTR_ENABLE);
			int outputs = 1 << select.Width;
			Port[] ps = new Port[outputs + (enable ? 3 : 2)];
			Location sel;
			int selMult = selectLoc == Plexers.SELECT_BOTTOM_LEFT ? 1 : -1;
			if (outputs == 2)
			{
				Location end0;
				Location end1;
				if (facing == Direction.West)
				{
					end0 = new Location(-30, -10);
					end1 = new Location(-30, 10);
					sel = new Location(-20, selMult * 20);
				}
				else if (facing == Direction.North)
				{
					end0 = new Location(-10, -30);
					end1 = new Location(10, -30);
					sel = new Location(selMult * -20, -20);
				}
				else if (facing == Direction.South)
				{
					end0 = new Location(-10, 30);
					end1 = new Location(10, 30);
					sel = new Location(selMult * -20, 20);
				}
				else
				{
					end0 = new Location(30, -10);
					end1 = new Location(30, 10);
					sel = new Location(20, selMult * 20);
				}
				ps[0] = new Port(end0.X, end0.Y, Port.OUTPUT, data.Width);
				ps[1] = new Port(end1.X, end1.Y, Port.OUTPUT, data.Width);
			}
			else
			{
				int dx = -(outputs / 2) * 10;
				int ddx = 10;
				int dy = dx;
				int ddy = 10;
				if (facing == Direction.West)
				{
					dx = -40;
					ddx = 0;
					sel = new Location(-20, selMult * (dy + 10 * outputs));
				}
				else if (facing == Direction.North)
				{
					dy = -40;
					ddy = 0;
					sel = new Location(selMult * dx, -20);
				}
				else if (facing == Direction.South)
				{
					dy = 40;
					ddy = 0;
					sel = new Location(selMult * dx, 20);
				}
				else
				{
					dx = 40;
					ddx = 0;
					sel = new Location(20, selMult * (dy + 10 * outputs));
				}
				for (int i = 0; i < outputs; i++)
				{
					ps[i] = new Port(dx, dy, Port.OUTPUT, data.Width);
					dx += ddx;
					dy += ddy;
				}
			}
			Location en = sel.translate(facing, -10);
			ps[outputs] = new Port(sel.X, sel.Y, Port.INPUT, select.Width);
			if (enable)
			{
				ps[outputs + 1] = new Port(en.X, en.Y, Port.INPUT, BitWidth.ONE);
			}
			ps[ps.Length - 1] = new Port(0, 0, Port.INPUT, data.Width);

			for (int i = 0; i < outputs; i++)
			{
				ps[i].setToolTip(Strings.getter("demultiplexerOutTip", "" + i));
			}
			ps[outputs].setToolTip(Strings.getter("demultiplexerSelectTip"));
			if (enable)
			{
				ps[outputs + 1].setToolTip(Strings.getter("demultiplexerEnableTip"));
			}
			ps[ps.Length - 1].setToolTip(Strings.getter("demultiplexerInTip"));

			instance.setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			// get attributes
			BitWidth data = state.getAttributeValue(StdAttr.WIDTH);
			BitWidth select = state.getAttributeValue(Plexers.ATTR_SELECT);
			bool? threeState = state.getAttributeValue(Plexers.ATTR_TRISTATE);
			bool enable = (bool)state.getAttributeValue(Plexers.ATTR_ENABLE);
			int outputs = 1 << select.Width;
			Value en = enable ? state.getPort(outputs + 1) : Value.TRUE;

			// determine output values
			Value others; // the default output
			if (threeState.Value)
			{
				others = Value.createUnknown(data);
			}
			else
			{
				others = Value.createKnown(data, 0);
			}
			int outIndex = -1; // the special output
			Value @out = null;
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
					@out = state.getPort(outputs + (enable ? 2 : 1));
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
			Graphics g = painter.Graphics;
			Bounds bds = painter.Bounds;
			Direction facing = painter.getAttributeValue(StdAttr.FACING);
			BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
			bool enable = (bool)painter.getAttributeValue(Plexers.ATTR_ENABLE);
			int outputs = 1 << select.Width;

			// draw select and enable inputs
			GraphicsUtil.switchToWidth(g, 3);
			bool vertical = facing == Direction.North || facing == Direction.South;
			object selectLoc = painter.getAttributeValue(Plexers.ATTR_SELECT_LOC);
			int selMult = selectLoc == Plexers.SELECT_BOTTOM_LEFT ? 1 : -1;
			int dx = vertical ? selMult : 0;
			int dy = vertical ? 0 : -selMult;
			if (outputs == 2)
			{ // draw select wire
				Location sel = painter.getInstance().getPortLocation(outputs);
				if (painter.ShowState)
				{
					g.setColor(painter.getPort(outputs).Color);
				}
				g.drawLine(sel.X, sel.Y, sel.X + 2 * dx, sel.Y + 2 * dy);
			}
			if (enable)
			{
				Location en = painter.getInstance().getPortLocation(outputs + 1);
				if (painter.ShowState)
				{
					g.setColor(painter.getPort(outputs + 1).Color);
				}
				int len = outputs == 2 ? 6 : 4;
				g.drawLine(en.X, en.Y, en.X + len * dx, en.Y + len * dy);
			}
			GraphicsUtil.switchToWidth(g, 1);

			// draw a circle indicating where the select input is located
			Multiplexer.drawSelectCircle(g, bds, painter.getInstance().getPortLocation(outputs));

			// draw "0" next to first input
			int x0;
			int y0;
			int halign;
			if (facing == Direction.West)
			{
				x0 = 3;
				y0 = 15;
				halign = GraphicsUtil.H_LEFT;
			}
			else if (facing == Direction.North)
			{
				x0 = 10;
				y0 = 15;
				halign = GraphicsUtil.H_CENTER;
			}
			else if (facing == Direction.South)
			{
				x0 = 10;
				y0 = bds.Height - 3;
				halign = GraphicsUtil.H_CENTER;
			}
			else
			{
				x0 = bds.Width - 3;
				y0 = 15;
				halign = GraphicsUtil.H_RIGHT;
			}
			g.setColor(Color.GRAY);
			GraphicsUtil.drawText(g, "0", bds.X + x0, bds.Y + y0, halign, GraphicsUtil.V_BASELINE);

			// draw trapezoid, "DMX" label, and ports
			g.setColor(Color.BLACK);
			Plexers.drawTrapezoid(g, bds, facing.reverse(), select.Width == 1 ? 10 : 20);
			GraphicsUtil.drawCenteredText(g, "DMX", bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			painter.drawPorts();
		}
	}

}
