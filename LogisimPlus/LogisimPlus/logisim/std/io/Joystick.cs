﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using logisim.data;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using InstanceData = logisim.instance.InstanceData;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class Joystick : InstanceFactory
	{
		internal static readonly Attribute<BitWidth> ATTR_WIDTH = Attributes.forBitWidth("bits", Strings.getter("ioBitWidthAttr"), 2, 5);

		public Joystick() : base("Joystick", Strings.getter("joystickComponent"))
		{
			setAttributes(new Attribute[] {ATTR_WIDTH, Io.ATTR_COLOR}, new object[] {BitWidth.create(4), Color.RED});
			KeyConfigurator = new BitWidthConfigurator(ATTR_WIDTH, 2, 5);
			OffsetBounds = Bounds.create(-30, -10, 30, 30);
			IconName = "joystick.gif";
			setPorts(new Port[]
			{
				new Port(0, 0, Port.OUTPUT, ATTR_WIDTH),
				new Port(0, 10, Port.OUTPUT, ATTR_WIDTH)
			});
			InstancePoker = typeof(Poker);
		}

		public override void propagate(InstanceState state)
		{
			BitWidth bits = state.getAttributeValue(ATTR_WIDTH);
			int dx;
			int dy;
			State s = (State) state.Data;
			if (s == null)
			{
				dx = 0;
				dy = 0;
			}
			else
			{
				dx = s.xPos;
				dy = s.yPos;
			}

			int steps = (1 << bits.Width) - 1;
			dx = (dx + 14) * steps / 29 + 1;
			dy = (dy + 14) * steps / 29 + 1;
			if (bits.Width > 4)
			{
				if (dx >= steps / 2)
				{
					dx++;
				}
				if (dy >= steps / 2)
				{
					dy++;
				}
			}
			state.setPort(0, Value.createKnown(bits, dx), 1);
			state.setPort(1, Value.createKnown(bits, dy), 1);
		}

		public override void paintGhost(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			GraphicsUtil.switchToWidth(g, 2);
			g.drawRoundRect(-30, -10, 30, 30, 8, 8);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;

			Graphics g = painter.Graphics;
			g.drawRoundRect(x - 30, y - 10, 30, 30, 8, 8);
			g.drawRoundRect(x - 28, y - 8, 26, 26, 4, 4);
			drawBall(g, x - 15, y + 5, painter.getAttributeValue(Io.ATTR_COLOR), painter.shouldDrawColor());
			painter.drawPorts();
		}

		private static void drawBall(Graphics g, int x, int y, Color c, bool inColor)
		{
			if (inColor)
			{
				g.setColor(c == null ? Color.RED : c);
			}
			else
			{
				int hue = c == null ? 128 : (c.getRed() + c.getGreen() + c.getBlue()) / 3;
				g.setColor(new Color(hue, hue, hue));
			}
			GraphicsUtil.switchToWidth(g, 1);
			g.fillOval(x - 4, y - 4, 8, 8);
			g.setColor(Color.BLACK);
			g.drawOval(x - 4, y - 4, 8, 8);
		}

		private class State : InstanceData, ICloneable
		{
			internal int xPos;
			internal int yPos;

			public State(int x, int y)
			{
				xPos = x;
				yPos = y;
			}

			public virtual object clone()
			{
				try
				{
					return base.clone();
				}
				catch (CloneNotSupportedException)
				{
					return null;
				}
			}
		}

		public class Poker : InstancePoker
		{
			public override void mousePressed(InstanceState state, MouseEvent e)
			{
				mouseDragged(state, e);
			}

			public override void mouseReleased(InstanceState state, MouseEvent e)
			{
				updateState(state, 0, 0);
			}

			public override void mouseDragged(InstanceState state, MouseEvent e)
			{
				Location loc = state.Instance.Location;
				int cx = loc.X - 15;
				int cy = loc.Y + 5;
				updateState(state, e.getX() - cx, e.getY() - cy);
			}

			internal virtual void updateState(InstanceState state, int dx, int dy)
			{
				State s = (State) state.Data;
				if (dx < -14)
				{
					dx = -14;
				}
				if (dy < -14)
				{
					dy = -14;
				}
				if (dx > 14)
				{
					dx = 14;
				}
				if (dy > 14)
				{
					dy = 14;
				}
				if (s == null)
				{
					s = new State(dx, dy);
					state.Data = s;
				}
				else
				{
					s.xPos = dx;
					s.yPos = dy;
				}
				state.Instance.fireInvalidated();
			}

			public override void paint(InstancePainter painter)
			{
				State state = (State) painter.Data;
				if (state == null)
				{
					state = new State(0, 0);
					painter.Data = state;
				}
				Location loc = painter.Location;
				int x = loc.X;
				int y = loc.Y;
				Graphics g = painter.Graphics;
				g.setColor(Color.WHITE);
				g.fillRect(x - 20, y, 10, 10);
				GraphicsUtil.switchToWidth(g, 3);
				g.setColor(Color.BLACK);
				int dx = state.xPos;
				int dy = state.yPos;
				int x0 = x - 15 + (dx > 5 ? 1 : dx < -5 ? -1 : 0);
				int y0 = y + 5 + (dy > 5 ? 1 : dy < 0 ? -1 : 0);
				int x1 = x - 15 + dx;
				int y1 = y + 5 + dy;
				g.drawLine(x0, y0, x1, y1);
				Color ballColor = painter.getAttributeValue(Io.ATTR_COLOR);
				Joystick.drawBall(g, x1, y1, ballColor, true);
			}
		}
	}

}
