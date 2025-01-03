// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.shapes
{

	using Element = org.w3c.dom.Element;

	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Location = logisim.data.Location;
	using logisim.util;

	public class SvgReader
	{
		private SvgReader()
		{
		}

		private static readonly Pattern PATH_REGEX = Pattern.compile("[a-zA-Z]|[-0-9.]+");

		public static AbstractCanvasObject createShape(Element elt)
		{
			string name = elt.getTagName();
			AbstractCanvasObject ret;
			if (name.Equals("ellipse"))
			{
				ret = createOval(elt);
			}
			else if (name.Equals("line"))
			{
				ret = createLine(elt);
			}
			else if (name.Equals("path"))
			{
				ret = createPath(elt);
			}
			else if (name.Equals("polyline"))
			{
				ret = createPolyline(elt);
			}
			else if (name.Equals("polygon"))
			{
				ret = createPolygon(elt);
			}
			else if (name.Equals("rect"))
			{
				ret = createRectangle(elt);
			}
			else if (name.Equals("text"))
			{
				ret = createText(elt);
			}
			else
			{
				return null;
			}
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<logisim.data.Attribute<?>> attrs = ret.getAttributes();
			List<Attribute> attrs = ret.Attributes;
			if (attrs.Contains(DrawAttr.PAINT_TYPE))
			{
				string stroke = elt.getAttribute("stroke");
				string fill = elt.getAttribute("fill");
				if (stroke.Equals("") || stroke.Equals("none"))
				{
					ret.setValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_FILL);
				}
				else if (fill.Equals("none"))
				{
					ret.setValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_STROKE);
				}
				else
				{
					ret.setValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_STROKE_FILL);
				}
			}
			attrs = ret.Attributes; // since changing paintType could change it
			if (attrs.Contains(DrawAttr.STROKE_WIDTH) && elt.hasAttribute("stroke-width"))
			{
				int? width = Convert.ToInt32(elt.getAttribute("stroke-width"));
				ret.setValue(DrawAttr.STROKE_WIDTH, width);
			}
			if (attrs.Contains(DrawAttr.STROKE_COLOR))
			{
				string color = elt.getAttribute("stroke");
				string opacity = elt.getAttribute("stroke-opacity");
				if (!color.Equals("none"))
				{
					ret.setValue(DrawAttr.STROKE_COLOR, getColor(color, opacity));
				}
			}
			if (attrs.Contains(DrawAttr.FILL_COLOR))
			{
				string color = elt.getAttribute("fill");
				if (color.Equals(""))
				{
					color = "#000000";
				}
				string opacity = elt.getAttribute("fill-opacity");
				if (!color.Equals("none"))
				{
					ret.setValue(DrawAttr.FILL_COLOR, getColor(color, opacity));
				}
			}
			return ret;
		}

		private static AbstractCanvasObject createRectangle(Element elt)
		{
			int x = int.Parse(elt.getAttribute("x"));
			int y = int.Parse(elt.getAttribute("y"));
			int w = int.Parse(elt.getAttribute("width"));
			int h = int.Parse(elt.getAttribute("height"));
			if (elt.hasAttribute("rx"))
			{
				AbstractCanvasObject ret = new RoundRectangle(x, y, w, h);
				int rx = int.Parse(elt.getAttribute("rx"));
				ret.setValue(DrawAttr.CORNER_RADIUS, Convert.ToInt32(rx));
				return ret;
			}
			else
			{
				return new Rectangle(x, y, w, h);
			}
		}

		private static AbstractCanvasObject createOval(Element elt)
		{
			double cx = double.Parse(elt.getAttribute("cx"));
			double cy = double.Parse(elt.getAttribute("cy"));
			double rx = double.Parse(elt.getAttribute("rx"));
			double ry = double.Parse(elt.getAttribute("ry"));
			int x = (int) (long)Math.Round(cx - rx, MidpointRounding.AwayFromZero);
			int y = (int) (long)Math.Round(cy - ry, MidpointRounding.AwayFromZero);
			int w = (int) (long)Math.Round(rx * 2, MidpointRounding.AwayFromZero);
			int h = (int) (long)Math.Round(ry * 2, MidpointRounding.AwayFromZero);
			return new Oval(x, y, w, h);
		}

		private static AbstractCanvasObject createLine(Element elt)
		{
			int x0 = int.Parse(elt.getAttribute("x1"));
			int y0 = int.Parse(elt.getAttribute("y1"));
			int x1 = int.Parse(elt.getAttribute("x2"));
			int y1 = int.Parse(elt.getAttribute("y2"));
			return new Line(x0, y0, x1, y1);
		}

		private static AbstractCanvasObject createPolygon(Element elt)
		{
			return new Poly(true, parsePoints(elt.getAttribute("points")));
		}

		private static AbstractCanvasObject createPolyline(Element elt)
		{
			return new Poly(false, parsePoints(elt.getAttribute("points")));
		}

		private static AbstractCanvasObject createText(Element elt)
		{
			int x = int.Parse(elt.getAttribute("x"));
			int y = int.Parse(elt.getAttribute("y"));
			string text = elt.getTextContent();
			Text ret = new Text(x, y, text);

			string fontFamily = elt.getAttribute("font-family");
			string fontStyle = elt.getAttribute("font-style");
			string fontWeight = elt.getAttribute("font-weight");
			string fontSize = elt.getAttribute("font-size");
			int styleFlags = 0;
			if (fontStyle.Equals("italic"))
			{
				styleFlags |= Font.ITALIC;
			}
			if (fontWeight.Equals("bold"))
			{
				styleFlags |= Font.BOLD;
			}
			int size = int.Parse(fontSize);
			ret.setValue(DrawAttr.FONT, new Font(fontFamily, styleFlags, size));

			string alignStr = elt.getAttribute("text-anchor");
			AttributeOption halign;
			if (alignStr.Equals("start"))
			{
				halign = DrawAttr.ALIGN_LEFT;
			}
			else if (alignStr.Equals("end"))
			{
				halign = DrawAttr.ALIGN_RIGHT;
			}
			else
			{
				halign = DrawAttr.ALIGN_CENTER;
			}
			ret.setValue(DrawAttr.ALIGNMENT, halign);

			// fill color is handled after we return
			return ret;
		}

		private static List<Location> parsePoints(string points)
		{
			Pattern patt = Pattern.compile("[ ,\n\r\t]+");
			string[] toks = patt.split(points);
			Location[] ret = new Location[toks.Length / 2];
			for (int i = 0; i < ret.Length; i++)
			{
				int x = int.Parse(toks[2 * i]);
				int y = int.Parse(toks[2 * i + 1]);
				ret[i] = new Location(x, y);
			}
			return UnmodifiableList.create(ret);
		}

		private static AbstractCanvasObject createPath(Element elt)
		{
			Matcher patt = PATH_REGEX.matcher(elt.getAttribute("d"));
			List<string> tokens = new List<string>();
			int type = -1; // -1 error, 0 start, 1 curve, 2 polyline
			while (patt.find())
			{
				string token = patt.group();
				tokens.Add(token);
				if (char.IsLetter(token[0]))
				{
					switch (token[0])
					{
					case 'M':
						if (type == -1)
						{
							type = 0;
						}
						else
						{
							type = -1;
						}
						break;
					case 'Q':
					case 'q':
						if (type == 0)
						{
							type = 1;
						}
						else
						{
							type = -1;
						}
						break;
					/*
					 * not supported case 'L': case 'l': case 'H': case 'h': case 'V': case 'v': if (type == 0 || type == 2)
					 * type = 2; else type = -1; break;
					 */
					default:
						type = -1;
					break;
					}
					if (type == -1)
					{
						throw new System.FormatException("Unrecognized path command '" + token[0] + "'");
					}
				}
			}

			if (type == 1)
			{
				if (tokens.Count == 8 && tokens[0].Equals("M") && tokens[3].ToUpper().Equals("Q"))
				{
					int x0 = int.Parse(tokens[1]);
					int y0 = int.Parse(tokens[2]);
					int x1 = int.Parse(tokens[4]);
					int y1 = int.Parse(tokens[5]);
					int x2 = int.Parse(tokens[6]);
					int y2 = int.Parse(tokens[7]);
					if (tokens[3].Equals("q"))
					{
						x1 += x0;
						y1 += y0;
						x2 += x0;
						y2 += y0;
					}
					Location e0 = new Location(x0, y0);
					Location e1 = new Location(x2, y2);
					Location ct = new Location(x1, y1);
					return new Curve(e0, e1, ct);
				}
				else
				{
					throw new System.FormatException("Unexpected format for curve");
				}
			}
			else
			{
				throw new System.FormatException("Unrecognized path");
			}
		}

		private static Color getColor(string hue, string opacity)
		{
			int r;
			int g;
			int b;
			if (string.ReferenceEquals(hue, null) || hue.Equals(""))
			{
				r = 0;
				g = 0;
				b = 0;
			}
			else
			{
				r = Convert.ToInt32(hue.Substring(1, 2), 16);
				g = Convert.ToInt32(hue.Substring(3, 2), 16);
				b = Convert.ToInt32(hue.Substring(5, 2), 16);
			}
			int a;
			if (string.ReferenceEquals(opacity, null) || opacity.Equals(""))
			{
				a = 255;
			}
			else
			{
				a = (int) (long)Math.Round(double.Parse(opacity) * 255, MidpointRounding.AwayFromZero);
			}
			return Color.FromArgb(255, r, g, b, a);
		}
	}

}
