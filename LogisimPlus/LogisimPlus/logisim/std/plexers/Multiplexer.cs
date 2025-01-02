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

	public class Multiplexer : InstanceFactory
	{
		public Multiplexer() : base("Multiplexer", Strings.getter("multiplexerComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, Plexers.ATTR_SELECT_LOC, Plexers.ATTR_SELECT, StdAttr.WIDTH, Plexers.ATTR_DISABLED, Plexers.ATTR_ENABLE}, new object[] {Direction.East, Plexers.SELECT_BOTTOM_LEFT, Plexers.DEFAULT_SELECT, BitWidth.ONE, Plexers.DISABLED_FLOATING, true});
			KeyConfigurator = JoinedConfigurator.create(new BitWidthConfigurator(Plexers.ATTR_SELECT, 1, 5, 0), new BitWidthConfigurator(StdAttr.WIDTH));
			IconName = "multiplexer.gif";
			FacingAttribute = StdAttr.FACING;
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
			Direction dir = attrs.getValue(StdAttr.FACING);
			BitWidth select = attrs.getValue(Plexers.ATTR_SELECT);
			int inputs = 1 << select.Width;
			if (inputs == 2)
			{
				return Bounds.create(-30, -20, 30, 40).rotate(Direction.East, dir, 0, 0);
			}
			else
			{
				int offs = -(inputs / 2) * 10 - 10;
				int length = inputs * 10 + 20;
				return Bounds.create(-40, offs, 40, length).rotate(Direction.East, dir, 0, 0);
			}
		}

		public override bool contains(Location loc, AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
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
			else if (attr == Plexers.ATTR_DISABLED)
			{
				instance.fireInvalidated();
			}
		}

		private void updatePorts(Instance instance)
		{
			Direction dir = instance.getAttributeValue(StdAttr.FACING);
			object selectLoc = instance.getAttributeValue(Plexers.ATTR_SELECT_LOC);
			BitWidth data = instance.getAttributeValue(StdAttr.WIDTH);
			BitWidth select = instance.getAttributeValue(Plexers.ATTR_SELECT);
			bool enable = (bool)instance.getAttributeValue(Plexers.ATTR_ENABLE);

			int selMult = selectLoc == Plexers.SELECT_BOTTOM_LEFT ? 1 : -1;
			int inputs = 1 << select.Width;
			Port[] ps = new Port[inputs + (enable ? 3 : 2)];
			Location sel;
			if (inputs == 2)
			{
				Location end0;
				Location end1;
				if (dir == Direction.West)
				{
					end0 = new Location(30, -10);
					end1 = new Location(30, 10);
					sel = new Location(20, selMult * 20);
				}
				else if (dir == Direction.North)
				{
					end0 = new Location(-10, 30);
					end1 = new Location(10, 30);
					sel = new Location(selMult * -20, 20);
				}
				else if (dir == Direction.South)
				{
					end0 = new Location(-10, -30);
					end1 = new Location(10, -30);
					sel = new Location(selMult * -20, -20);
				}
				else
				{
					end0 = new Location(-30, -10);
					end1 = new Location(-30, 10);
					sel = new Location(-20, selMult * 20);
				}
				ps[0] = new Port(end0.X, end0.Y, Port.INPUT, data.Width);
				ps[1] = new Port(end1.X, end1.Y, Port.INPUT, data.Width);
			}
			else
			{
				int dx = -(inputs / 2) * 10;
				int ddx = 10;
				int dy = -(inputs / 2) * 10;
				int ddy = 10;
				if (dir == Direction.West)
				{
					dx = 40;
					ddx = 0;
					sel = new Location(20, selMult * (dy + 10 * inputs));
				}
				else if (dir == Direction.North)
				{
					dy = 40;
					ddy = 0;
					sel = new Location(selMult * dx, 20);
				}
				else if (dir == Direction.South)
				{
					dy = -40;
					ddy = 0;
					sel = new Location(selMult * dx, -20);
				}
				else
				{
					dx = -40;
					ddx = 0;
					sel = new Location(-20, selMult * (dy + 10 * inputs));
				}
				for (int i = 0; i < inputs; i++)
				{
					ps[i] = new Port(dx, dy, Port.INPUT, data.Width);
					dx += ddx;
					dy += ddy;
				}
			}
			Location en = sel.translate(dir, 10);
			ps[inputs] = new Port(sel.X, sel.Y, Port.INPUT, select.Width);
			if (enable)
			{
				ps[inputs + 1] = new Port(en.X, en.Y, Port.INPUT, BitWidth.ONE);
			}
			ps[ps.Length - 1] = new Port(0, 0, Port.OUTPUT, data.Width);

			for (int i = 0; i < inputs; i++)
			{
				ps[i].setToolTip(Strings.getter("multiplexerInTip", "" + i));
			}
			ps[inputs].setToolTip(Strings.getter("multiplexerSelectTip"));
			if (enable)
			{
				ps[inputs + 1].setToolTip(Strings.getter("multiplexerEnableTip"));
			}
			ps[ps.Length - 1].setToolTip(Strings.getter("multiplexerOutTip"));

			instance.setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			BitWidth data = state.getAttributeValue(StdAttr.WIDTH);
			BitWidth select = state.getAttributeValue(Plexers.ATTR_SELECT);
			bool enable = (bool)state.getAttributeValue(Plexers.ATTR_ENABLE);
			int inputs = 1 << select.Width;
			Value en = enable ? state.getPort(inputs + 1) : Value.TRUE;
			Value @out;
			if (en == Value.FALSE)
			{
				object opt = state.getAttributeValue(Plexers.ATTR_DISABLED);
				Value @base = opt == Plexers.DISABLED_ZERO ? Value.FALSE : Value.UNKNOWN;
				@out = Value.repeat(@base, data.Width);
			}
			else if (en == Value.ERROR && state.isPortConnected(inputs + 1))
			{
				@out = Value.createError(data);
			}
			else
			{
				Value sel = state.getPort(inputs);
				if (sel.FullyDefined)
				{
					@out = state.getPort(sel.toIntValue());
				}
				else if (sel.ErrorValue)
				{
					@out = Value.createError(data);
				}
				else
				{
					@out = Value.createUnknown(data);
				}
			}
			state.setPort(inputs + (enable ? 2 : 1), @out, Plexers.DELAY);
		}

		public override void paintGhost(InstancePainter painter)
		{
			Direction facing = painter.getAttributeValue(StdAttr.FACING);
			BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
			Plexers.drawTrapezoid(painter.Graphics, painter.Bounds, facing, select.Width == 1 ? 10 : 20);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			Bounds bds = painter.Bounds;
			Direction facing = painter.getAttributeValue(StdAttr.FACING);
			BitWidth select = painter.getAttributeValue(Plexers.ATTR_SELECT);
			bool enable = (bool)painter.getAttributeValue(Plexers.ATTR_ENABLE);
			int inputs = 1 << select.Width;

			// draw stubs for select/enable inputs that aren't on instance boundary
			GraphicsUtil.switchToWidth(g, 3);
			bool vertical = facing != Direction.North && facing != Direction.South;
			object selectLoc = painter.getAttributeValue(Plexers.ATTR_SELECT_LOC);
			int selMult = selectLoc == Plexers.SELECT_BOTTOM_LEFT ? 1 : -1;
			int dx = vertical ? 0 : -selMult;
			int dy = vertical ? selMult : 0;
			if (inputs == 2)
			{ // draw select wire
				Location pt = painter.getInstance().getPortLocation(inputs);
				if (painter.ShowState)
				{
					g.setColor(painter.getPort(inputs).Color);
				}
				g.drawLine(pt.X - 2 * dx, pt.Y - 2 * dy, pt.X, pt.Y);
			}
			if (enable)
			{
				Location en = painter.getInstance().getPortLocation(inputs + 1);
				if (painter.ShowState)
				{
					g.setColor(painter.getPort(inputs + 1).Color);
				}
				int len = inputs == 2 ? 6 : 4;
				g.drawLine(en.X - len * dx, en.Y - len * dy, en.X, en.Y);
			}
			GraphicsUtil.switchToWidth(g, 1);

			// draw a circle indicating where the select input is located
			Multiplexer.drawSelectCircle(g, bds, painter.getInstance().getPortLocation(inputs));

			// draw a 0 indicating where the numbering starts for inputs
			int x0;
			int y0;
			int halign;
			if (facing == Direction.West)
			{
				x0 = bds.X + bds.Width - 3;
				y0 = bds.Y + 15;
				halign = GraphicsUtil.H_RIGHT;
			}
			else if (facing == Direction.North)
			{
				x0 = bds.X + 10;
				y0 = bds.Y + bds.Height - 2;
				halign = GraphicsUtil.H_CENTER;
			}
			else if (facing == Direction.South)
			{
				x0 = bds.X + 10;
				y0 = bds.Y + 12;
				halign = GraphicsUtil.H_CENTER;
			}
			else
			{
				x0 = bds.X + 3;
				y0 = bds.Y + 15;
				halign = GraphicsUtil.H_LEFT;
			}
			g.setColor(Color.GRAY);
			GraphicsUtil.drawText(g, "0", x0, y0, halign, GraphicsUtil.V_BASELINE);

			// draw the trapezoid, "MUX" string, the individual ports
			g.setColor(Color.BLACK);
			Plexers.drawTrapezoid(g, bds, facing, select.Width == 1 ? 10 : 20);
			GraphicsUtil.drawCenteredText(g, "MUX", bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			painter.drawPorts();
		}

		internal static void drawSelectCircle(Graphics g, Bounds bds, Location loc)
		{
			int locDelta = Math.Max(bds.Height, bds.Width) <= 50 ? 8 : 6;
			Location circLoc;
			if (bds.Height >= bds.Width)
			{ // vertically oriented
				if (loc.Y < bds.Y + bds.Height / 2)
				{ // at top
					circLoc = loc.translate(0, locDelta);
				}
				else
				{ // at bottom
					circLoc = loc.translate(0, -locDelta);
				}
			}
			else
			{
				if (loc.X < bds.X + bds.Width / 2)
				{ // at left
					circLoc = loc.translate(locDelta, 0);
				}
				else
				{ // at right
					circLoc = loc.translate(-locDelta, 0);
				}
			}
			g.setColor(Color.LIGHT_GRAY);
			g.fillOval(circLoc.X - 3, circLoc.Y - 3, 6, 6);
		}
	}

}
