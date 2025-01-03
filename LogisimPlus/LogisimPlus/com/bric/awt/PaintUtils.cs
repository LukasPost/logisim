// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/*
* @(#)PaintUtils.java  1.0  2008-03-01
*
* Copyright (c) 2008 Jeremy Wood
* E-mail: mickleness@gmail.com
* All rights reserved.
*
* The copyright of this software is owned by Jeremy Wood.
* You may not use, copy or modify this software, except in
* accordance with the license agreement you entered into with
* Jeremy Wood. For details see accompanying license terms.
*/

using LogisimPlus.Java;

namespace com.bric.awt
{

	/// <summary>
	/// Some static methods for some common painting functions.
	/// 
	/// @version 1.0
	/// @author Jeremy Wood
	/// 
	/// </summary>
	public class PaintUtils
	{

		/// <summary>
		/// Four shades of white, each with increasing opacity. </summary>
		public static readonly Color[] whites = new Color[]
		{
			Color.FromArgb(50, 255, 255, 255),
            Color.FromArgb(100, 255, 255, 255),
			Color.FromArgb(150, 255, 255, 255)
		};

		/// <summary>
		/// Four shades of black, each with increasing opacity. </summary>
		public static readonly Color[] blacks = new Color[]
		{
			Color.FromArgb(50, 0, 0, 0),
			Color.FromArgb(100, 0, 0, 0),
			Color.FromArgb(150, 0, 0, 0)
		};

		/// <returns> the color used to indicate when a component has focus. By default this uses the color (64,113,167), but
		///         you can override this by calling: <BR>
		///         <code>UIManager.put("focusRing",customColor);</code> </returns>
		public static Color FocusRingColor
		{
			get
			{
				object obj = UIManager.getColor("focusRing");
				if (obj is Color)
				{
					return (Color) obj;
				}
				return Color.FromArgb(255, 64, 113, 167);
			}
		}

		/// <summary>
		/// Paints 3 different strokes around a shape to indicate focus. The widest stroke is the most transparent, so this
		/// achieves a nice "glow" effect.
		/// <P>
		/// The catch is that you have to render this underneath the shape, and the shape should be filled completely.
		/// </summary>
		/// <param name="g">             the JGraphics to paint to </param>
		/// <param name="shape">         the shape to outline </param>
		/// <param name="biggestStroke"> the widest stroke to use. </param>
		public static void paintFocus(JGraphics g, Shape shape, int biggestStroke)
		{
			Color focusColor = FocusRingColor;
			Color[] focusArray = new Color[]
			{
				Color.FromArgb(255, focuscolor.R, focuscolor.G, focuscolor.B, 255),
				Color.FromArgb(255, focuscolor.R, focuscolor.G, focuscolor.B, 170),
				Color.FromArgb(255, focuscolor.R, focuscolor.G, focuscolor.B, 110)
			};
			g.setStroke(new BasicStroke(biggestStroke));
			g.setColor(focusArray[2]);
			g.draw(shape);
			g.setStroke(new BasicStroke(biggestStroke - 1));
			g.setColor(focusArray[1]);
			g.draw(shape);
			g.setStroke(new BasicStroke(biggestStroke - 2));
			g.setColor(focusArray[0]);
			g.draw(shape);
			g.setStroke(new BasicStroke(1));
		}

		/// <summary>
		/// Uses translucent shades of white and black to draw highlights and shadows around a rectangle, and then frames the
		/// rectangle with a shade of gray (120).
		/// <P>
		/// This should be called to add a finishing touch on top of existing JGraphics.
		/// </summary>
		/// <param name="g"> the JGraphics to paint to. </param>
		/// <param name="r"> the rectangle to paint. </param>
		public static void drawBevel(JGraphics g, Rectangle r)
		{
			drawColors(blacks, g, r.X, r.Y + r.Height, r.X + r.Width, r.Y + r.Height, SwingConstants.SOUTH);
			drawColors(blacks, g, r.X + r.Width, r.Y, r.X + r.Width, r.Y + r.Height, SwingConstants.EAST);

			drawColors(whites, g, r.X, r.Y, r.X + r.Width, r.Y, SwingConstants.NORTH);
			drawColors(whites, g, r.X, r.Y, r.X, r.Y + r.Height, SwingConstants.WEST);

			g.setColor(Color.FromArgb(255, 120, 120, 120));
			g.drawRect(r.X, r.Y, r.Width, r.Height);
		}

		private static void drawColors(Color[] colors, JGraphics g, int x1, int y1, int x2, int y2, int direction)
		{
			for (int a = 0; a < colors.Length; a++)
			{
				g.setColor(colors[colors.Length - a - 1]);
				if (direction == SwingConstants.SOUTH)
				{
					g.drawLine(x1, y1 - a, x2, y2 - a);
				}
				else if (direction == SwingConstants.NORTH)
				{
					g.drawLine(x1, y1 + a, x2, y2 + a);
				}
				else if (direction == SwingConstants.EAST)
				{
					g.drawLine(x1 - a, y1, x2 - a, y2);
				}
				else if (direction == SwingConstants.WEST)
				{
					g.drawLine(x1 + a, y1, x2 + a, y2);
				}
			}
		}
	}

}
