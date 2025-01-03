// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.shapes
{

	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;

	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using Handle = draw.model.Handle;
	using Location = logisim.data.Location;

	internal class SvgCreator
	{
		private SvgCreator()
		{
		}

		public static Element createRectangle(Document doc, Rectangle rect)
		{
			return createRectangular(doc, rect);
		}

		public static Element createRoundRectangle(Document doc, RoundRectangle rrect)
		{
			Element elt = createRectangular(doc, rrect);
			int r = (int)rrect.getValue(DrawAttr.CORNER_RADIUS);
			elt.setAttribute("rx", "" + r);
			elt.setAttribute("ry", "" + r);
			return elt;
		}

		private static Element createRectangular(Document doc, Rectangular rect)
		{
			Element elt = doc.createElement("rect");
			elt.setAttribute("x", "" + rect.X);
			elt.setAttribute("y", "" + rect.Y);
			elt.setAttribute("width", "" + rect.Width);
			elt.setAttribute("height", "" + rect.Height);
			populateFill(elt, rect);
			return elt;
		}

		public static Element createOval(Document doc, Oval oval)
		{
			double x = oval.X;
			double y = oval.Y;
			double width = oval.Width;
			double height = oval.Height;
			Element elt = doc.createElement("ellipse");
			elt.setAttribute("cx", "" + (x + width / 2));
			elt.setAttribute("cy", "" + (y + height / 2));
			elt.setAttribute("rx", "" + (width / 2));
			elt.setAttribute("ry", "" + (height / 2));
			populateFill(elt, oval);
			return elt;
		}

		public static Element createLine(Document doc, Line line)
		{
			Element elt = doc.createElement("line");
			Location v1 = line.End0;
			Location v2 = line.End1;
			elt.setAttribute("x1", "" + v1.X);
			elt.setAttribute("y1", "" + v1.Y);
			elt.setAttribute("x2", "" + v2.X);
			elt.setAttribute("y2", "" + v2.Y);
			populateStroke(elt, line);
			return elt;
		}

		public static Element createCurve(Document doc, Curve curve)
		{
			Element elt = doc.createElement("path");
			Location e0 = curve.End0;
			Location e1 = curve.End1;
			Location ct = curve.Control;
			elt.setAttribute("d", "M" + e0.X + "," + e0.Y + " Q" + ct.X + "," + ct.Y + " " + e1.X + "," + e1.Y);
			populateFill(elt, curve);
			return elt;
		}

		public static Element createPoly(Document doc, Poly poly)
		{
			Element elt;
			if (poly.Closed)
			{
				elt = doc.createElement("polygon");
			}
			else
			{
				elt = doc.createElement("polyline");
			}

			StringBuilder points = new StringBuilder();
			bool first = true;
			foreach (Handle h in poly.getHandles(null))
			{
				if (!first)
				{
					points.Append(" ");
				}
				points.Append(h.X + "," + h.Y);
				first = false;
			}
			elt.setAttribute("points", points.ToString());

			populateFill(elt, poly);
			return elt;
		}

		public static Element createText(Document doc, Text text)
		{
			Element elt = doc.createElement("text");
			Location loc = text.Location;
			Font font = text.getValue(DrawAttr.FONT);
			Color fill = text.getValue(DrawAttr.FILL_COLOR);
			object halign = text.getValue(DrawAttr.ALIGNMENT);
			elt.setAttribute("x", "" + loc.X);
			elt.setAttribute("y", "" + loc.Y);
			if (!colorMatches(fill, Color.Black))
			{
				elt.setAttribute("fill", getColorString(fill));
			}
			if (showOpacity(fill))
			{
				elt.setAttribute("fill-opacity", getOpacityString(fill));
			}
			elt.setAttribute("font-family", font.getFamily());
			elt.setAttribute("font-size", "" + font.getSize());
			int style = font.getStyle();
			if ((style & Font.ITALIC) != 0)
			{
				elt.setAttribute("font-style", "italic");
			}
			if ((style & Font.BOLD) != 0)
			{
				elt.setAttribute("font-weight", "bold");
			}
			if (halign == DrawAttr.ALIGN_LEFT)
			{
				elt.setAttribute("text-anchor", "start");
			}
			else if (halign == DrawAttr.ALIGN_RIGHT)
			{
				elt.setAttribute("text-anchor", "end");
			}
			else
			{
				elt.setAttribute("text-anchor", "middle");
			}
			elt.appendChild(doc.createTextNode(text.Text));
			return elt;
		}

		private static void populateFill(Element elt, AbstractCanvasObject shape)
		{
			object type = shape.getValue(DrawAttr.PAINT_TYPE);
			if (type == DrawAttr.PAINT_FILL)
			{
				elt.setAttribute("stroke", "none");
			}
			else
			{
				populateStroke(elt, shape);
			}
			if (type == DrawAttr.PAINT_STROKE)
			{
				elt.setAttribute("fill", "none");
			}
			else
			{
				Color fill = shape.getValue(DrawAttr.FILL_COLOR);
				if (colorMatches(fill, Color.Black))
				{
					elt.removeAttribute("fill");
				}
				else
				{
					elt.setAttribute("fill", getColorString(fill));
				}
				if (showOpacity(fill))
				{
					elt.setAttribute("fill-opacity", getOpacityString(fill));
				}
			}
		}

		private static void populateStroke(Element elt, AbstractCanvasObject shape)
		{
			int? width = shape.getValue(DrawAttr.STROKE_WIDTH);
			if (width != null && width.Value != 1)
			{
				elt.setAttribute("stroke-width", width.ToString());
			}
			Color stroke = shape.getValue(DrawAttr.STROKE_COLOR);
			elt.setAttribute("stroke", getColorString(stroke));
			if (showOpacity(stroke))
			{
				elt.setAttribute("stroke-opacity", getOpacityString(stroke));
			}
			elt.setAttribute("fill", "none");
		}

		private static bool colorMatches(Color a, Color b)
		{
			return a.R == b.R && a.G == b.G && a.B == b.B;
		}

		private static string getColorString(Color color)
		{
			return string.Format("#{0:x2}{1:x2}{2:x2}", Convert.ToInt32(color.R), Convert.ToInt32(color.G), Convert.ToInt32(color.B));
		}

		private static bool showOpacity(Color color)
		{
			return color.A != 255;
		}

		private static string getOpacityString(Color color)
		{
			return string.Format("{0,5:F3}", Convert.ToDouble(color.A / 255.0));
		}
	}

}
