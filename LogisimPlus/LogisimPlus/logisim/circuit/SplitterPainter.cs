// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class SplitterPainter
	{
		private static readonly int SPINE_WIDTH = Wire.WIDTH + 2;
		private static readonly int SPINE_DOT = Wire.WIDTH + 4;

		internal static void drawLines(ComponentDrawContext context, SplitterAttributes attrs, Location origin)
		{
			bool showState = context.ShowState;
			CircuitState state = showState ? context.CircuitState : null;
			if (state == null)
			{
				showState = false;
			}

			SplitterParameters parms = attrs.Parameters;
			int x0 = origin.X;
			int y0 = origin.Y;
			int x = x0 + parms.End0X;
			int y = y0 + parms.End0Y;
			int dx = parms.EndToEndDeltaX;
			int dy = parms.EndToEndDeltaY;
			int dxEndSpine = parms.EndToSpineDeltaX;
			int dyEndSpine = parms.EndToSpineDeltaY;

			Graphics g = context.Graphics;
			Color oldColor = g.getColor();
			GraphicsUtil.switchToWidth(g, Wire.WIDTH);
			for (int i = 0, n = attrs.fanout; i < n; i++)
			{
				if (showState)
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") logisim.data.Value val = state.getValue(new logisim.data.Location(x, y));
					Value val = state.getValue(new Location(x, y));
					g.setColor(val.Color);
				}
				g.drawLine(x, y, x + dxEndSpine, y + dyEndSpine);
				x += dx;
				y += dy;
			}
			GraphicsUtil.switchToWidth(g, SPINE_WIDTH);
			g.setColor(oldColor);
			int spine0x = x0 + parms.Spine0X;
			int spine0y = y0 + parms.Spine0Y;
			int spine1x = x0 + parms.Spine1X;
			int spine1y = y0 + parms.Spine1Y;
			if (spine0x == spine1x && spine0y == spine1y)
			{ // centered
				int fanout = attrs.fanout;
				spine0x = x0 + parms.End0X + parms.EndToSpineDeltaX;
				spine0y = y0 + parms.End0Y + parms.EndToSpineDeltaY;
				spine1x = spine0x + (fanout - 1) * parms.EndToEndDeltaX;
				spine1y = spine0y + (fanout - 1) * parms.EndToEndDeltaY;
				if (parms.EndToEndDeltaX == 0)
				{ // vertical spine
					if (spine0y < spine1y)
					{
						spine0y++;
						spine1y--;
					}
					else
					{
						spine0y--;
						spine1y++;
					}
					g.drawLine(x0 + parms.Spine1X / 4, y0, spine0x, y0);
				}
				else
				{
					if (spine0x < spine1x)
					{
						spine0x++;
						spine1x--;
					}
					else
					{
						spine0x--;
						spine1x++;
					}
					g.drawLine(x0, y0 + parms.Spine1Y / 4, x0, spine0y);
				}
				if (fanout <= 1)
				{ // spine is empty
					int diam = SPINE_DOT;
					g.fillOval(spine0x - diam / 2, spine0y - diam / 2, diam, diam);
				}
				else
				{
					g.drawLine(spine0x, spine0y, spine1x, spine1y);
				}
			}
			else
			{
				int[] xSpine = new int[] {spine0x, spine1x, x0 + parms.Spine1X / 4};
				int[] ySpine = new int[] {spine0y, spine1y, y0 + parms.Spine1Y / 4};
				g.drawPolyline(xSpine, ySpine, 3);
			}
		}

		internal static void drawLabels(ComponentDrawContext context, SplitterAttributes attrs, Location origin)
		{
			// compute labels
			string[] ends = new string[attrs.fanout + 1];
			int curEnd = -1;
			int cur0 = 0;
			for (int i = 0, n = attrs.bit_end.Length; i <= n; i++)
			{
				int bit = i == n ? -1 : attrs.bit_end[i];
				if (bit != curEnd)
				{
					int cur1 = i - 1;
					string toAdd;
					if (curEnd <= 0)
					{
						toAdd = null;
					}
					else if (cur0 == cur1)
					{
						toAdd = "" + cur0;
					}
					else
					{
						toAdd = cur0 + "-" + cur1;
					}
					if (!string.ReferenceEquals(toAdd, null))
					{
						string old = ends[curEnd];
						if (string.ReferenceEquals(old, null))
						{
							ends[curEnd] = toAdd;
						}
						else
						{
							ends[curEnd] = old + "," + toAdd;
						}
					}
					curEnd = bit;
					cur0 = i;
				}
			}

			Graphics g = context.Graphics.create();
			Font font = g.getFont();
			g.setFont(font.deriveFont(7.0f));

			SplitterParameters parms = attrs.Parameters;
			int x = origin.X + parms.End0X + parms.EndToSpineDeltaX;
			int y = origin.Y + parms.End0Y + parms.EndToSpineDeltaY;
			int dx = parms.EndToEndDeltaX;
			int dy = parms.EndToEndDeltaY;
			if (parms.TextAngle != 0)
			{
				((Graphics2D) g).rotate(Math.PI / 2.0);
				int t;
				t = -x;
				x = y;
				y = t;
				t = -dx;
				dx = dy;
				dy = t;
			}
			int halign = parms.TextHorzAlign;
			int valign = parms.TextVertAlign;
			x += (halign == GraphicsUtil.H_RIGHT ? -1 : 1) * (SPINE_WIDTH / 2 + 1);
			y += valign == GraphicsUtil.V_TOP ? 0 : -3;
			for (int i = 0, n = attrs.fanout; i < n; i++)
			{
				string text = ends[i + 1];
				if (!string.ReferenceEquals(text, null))
				{
					GraphicsUtil.drawText(g, text, x, y, halign, valign);
				}
				x += dx;
				y += dy;
			}

			g.dispose();
		}

		internal static void drawLegacy(ComponentDrawContext context, SplitterAttributes attrs, Location origin)
		{
			Graphics g = context.Graphics;
			CircuitState state = context.CircuitState;
			Direction facing = attrs.facing;
			int fanout = attrs.fanout;
			SplitterParameters parms = attrs.Parameters;

			g.setColor(Color.BLACK);
			int x0 = origin.X;
			int y0 = origin.Y;
			int x1 = x0 + parms.End0X;
			int y1 = y0 + parms.End0Y;
			int dx = parms.EndToEndDeltaX;
			int dy = parms.EndToEndDeltaY;
			if (facing == Direction.North || facing == Direction.South)
			{
				int ySpine = (y0 + y1) / 2;
				GraphicsUtil.switchToWidth(g, Wire.WIDTH);
				g.drawLine(x0, y0, x0, ySpine);
				int xi = x1;
				int yi = y1;
				for (int i = 1; i <= fanout; i++)
				{
					if (context.ShowState)
					{
						g.setColor(state.getValue(new Location(xi, yi)).Color);
					}
					int xSpine = xi + (xi == x0 ? 0 : (xi < x0 ? 10 : -10));
					g.drawLine(xi, yi, xSpine, ySpine);
					xi += dx;
					yi += dy;
				}
				if (fanout > 3)
				{
					GraphicsUtil.switchToWidth(g, SPINE_WIDTH);
					g.setColor(Color.BLACK);
					g.drawLine(x1 + dx, ySpine, x1 + (fanout - 2) * dx, ySpine);
				}
				else
				{
					g.setColor(Color.BLACK);
					g.fillOval(x0 - SPINE_DOT / 2, ySpine - SPINE_DOT / 2, SPINE_DOT, SPINE_DOT);
				}
			}
			else
			{
				int xSpine = (x0 + x1) / 2;
				GraphicsUtil.switchToWidth(g, Wire.WIDTH);
				g.drawLine(x0, y0, xSpine, y0);
				int xi = x1;
				int yi = y1;
				for (int i = 1; i <= fanout; i++)
				{
					if (context.ShowState)
					{
						g.setColor(state.getValue(new Location(xi, yi)).Color);
					}
					int ySpine = yi + (yi == y0 ? 0 : (yi < y0 ? 10 : -10));
					g.drawLine(xi, yi, xSpine, ySpine);
					xi += dx;
					yi += dy;
				}
				if (fanout >= 3)
				{
					GraphicsUtil.switchToWidth(g, SPINE_WIDTH);
					g.setColor(Color.BLACK);
					g.drawLine(xSpine, y1 + dy, xSpine, y1 + (fanout - 2) * dy);
				}
				else
				{
					g.setColor(Color.BLACK);
					g.fillOval(xSpine - SPINE_DOT / 2, y0 - SPINE_DOT / 2, SPINE_DOT, SPINE_DOT);
				}
			}
			GraphicsUtil.switchToWidth(g, 1);
		}
	}
}
