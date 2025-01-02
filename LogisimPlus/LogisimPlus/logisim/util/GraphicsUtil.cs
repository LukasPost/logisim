// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class GraphicsUtil
	{
		public const int H_LEFT = -1;
		public const int H_CENTER = 0;
		public const int H_RIGHT = 1;
		public const int V_TOP = -1;
		public const int V_CENTER = 0;
		public const int V_BASELINE = 1;
		public const int V_BOTTOM = 2;
		public const int V_CENTER_OVERALL = 3;

		public static void switchToWidth(JGraphics g, int width)
		{
			g.setStroke(width);
		}

		public static void drawCenteredArc(Graphics g, int x, int y, int r, int start, int dist)
		{
			g.drawArc(x - r, y - r, 2 * r, 2 * r, start, dist);
		}

		public static Rectangle getTextBounds(Graphics g, Font font, string text, int x, int y, int halign, int valign)
		{
			if (g == null)
			{
				return new Rectangle(x, y, 0, 0);
			}
			Font oldfont = g.getFont();
			if (font != null)
			{
				g.setFont(font);
			}
			Rectangle ret = getTextBounds(g, text, x, y, halign, valign);
			if (font != null)
			{
				g.setFont(oldfont);
			}
			return ret;
		}

		public static Rectangle getTextBounds(Graphics g, string text, int x, int y, int halign, int valign)
		{
			if (g == null)
			{
				return new Rectangle(x, y, 0, 0);
			}
			FontMetrics mets = g.getFontMetrics();
			int width = mets.stringWidth(text);
			int ascent = mets.getAscent();
			int descent = mets.getDescent();
			int height = ascent + descent;

			Rectangle ret = new Rectangle(x, y, width, height);
			switch (halign)
			{
			case H_CENTER:
				ret.translate(-(width / 2), 0);
				break;
			case H_RIGHT:
				ret.translate(-width, 0);
				break;
			default:
				;
			break;
			}
			switch (valign)
			{
			case V_TOP:
				break;
			case V_CENTER:
				ret.translate(0, -(ascent / 2));
				break;
			case V_CENTER_OVERALL:
				ret.translate(0, -(height / 2));
				break;
			case V_BASELINE:
				ret.translate(0, -ascent);
				break;
			case V_BOTTOM:
				ret.translate(0, -height);
				break;
			default:
				;
			break;
			}
			return ret;
		}

		public static void drawText(Graphics g, Font font, string text, int x, int y, int halign, int valign)
		{
			Font oldfont = g.getFont();
			if (font != null)
			{
				g.setFont(font);
			}
			drawText(g, text, x, y, halign, valign);
			if (font != null)
			{
				g.setFont(oldfont);
			}
		}

		public static void drawText(Graphics g, string text, int x, int y, int halign, int valign)
		{
			if (text.Length == 0)
			{
				return;
			}
			Rectangle bd = getTextBounds(g, text, x, y, halign, valign);
			g.drawString(text, bd.x, bd.y + g.getFontMetrics().getAscent());
		}

		public static void drawCenteredText(Graphics g, string text, int x, int y)
		{
			drawText(g, text, x, y, H_CENTER, V_CENTER);
		}

		public static void drawArrow(Graphics g, int x0, int y0, int x1, int y1, int headLength, int headAngle)
		{
			double offs = headAngle * Math.PI / 180.0;
			double angle = Math.Atan2(y0 - y1, x0 - x1);
			int[] xs = new int[] {x1 + (int)(headLength * Math.Cos(angle + offs)), x1, x1 + (int)(headLength * Math.Cos(angle - offs))};
			int[] ys = new int[] {y1 + (int)(headLength * Math.Sin(angle + offs)), y1, y1 + (int)(headLength * Math.Sin(angle - offs))};
			g.drawLine(x0, y0, x1, y1);
			g.drawPolyline(xs, ys, 3);
		}
	}

}
