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

	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;

	using CanvasObject = draw.model.CanvasObject;
	using logisim.data;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;

	public class RoundRectangle : Rectangular
	{
		private int radius;

		public RoundRectangle(int x, int y, int w, int h) : base(x, y, w, h)
		{
			this.radius = 10;
		}

		public override bool matches(CanvasObject other)
		{
			if (other is RoundRectangle)
			{
				RoundRectangle that = (RoundRectangle) other;
				return base.matches(other) && this.radius == that.radius;
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			return base.matchesHashCode() * 31 + radius;
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("shapeRoundRect");
			}
		}

		public override Element toSvgElement(Document doc)
		{
			return SvgCreator.createRoundRectangle(doc, this);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return DrawAttr.getRoundRectAttributes(PaintType);
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public virtual V getValue<V>(Attribute<V> attr)
		{
			if (attr == DrawAttr.CORNER_RADIUS)
			{
				return (V) Convert.ToInt32(radius);
			}
			else
			{
				return base.getValue(attr);
			}
		}

		public override void updateValue<T1>(Attribute<T1> attr, object value)
		{
			if (attr == DrawAttr.CORNER_RADIUS)
			{
				radius = ((int?) value).Value;
			}
			else
			{
				base.updateValue(attr, value);
			}
		}

		protected internal override bool contains(int x, int y, int w, int h, Location q)
		{
			int qx = q.X;
			int qy = q.Y;
			int rx = radius;
			int ry = radius;
			if (2 * rx > w)
			{
				rx = w / 2;
			}
			if (2 * ry > h)
			{
				ry = h / 2;
			}
			if (!isInRect(qx, qy, x, y, w, h))
			{
				return false;
			}
			else if (qx < x + rx)
			{
				if (qy < y + ry)
				{
					return inCircle(qx, qy, x + rx, y + ry, rx, ry);
				}
				else if (qy < y + h - ry)
				{
					return true;
				}
				else
				{
					return inCircle(qx, qy, x + rx, y + h - ry, rx, ry);
				}
			}
			else if (qx < x + w - rx)
			{
				return true;
			}
			else
			{
				if (qy < y + ry)
				{
					return inCircle(qx, qy, x + w - rx, y + ry, rx, ry);
				}
				else if (qy < y + h - ry)
				{
					return true;
				}
				else
				{
					return inCircle(qx, qy, x + w - rx, y + h - ry, rx, ry);
				}
			}
		}

		protected internal override Location getRandomPoint(Bounds bds, Random rand)
		{
			if (PaintType == DrawAttr.PAINT_STROKE)
			{
				int w = Width;
				int h = Height;
				int r = radius;
				int horz = Math.Max(0, w - 2 * r); // length of horizontal segment
				int vert = Math.Max(0, h - 2 * r);
				double len = 2 * horz + 2 * vert + 2 * Math.PI * r;
				double u = len * rand.NextDouble();
				int x = X;
				int y = Y;
				if (u < horz)
				{
					x += r + (int) u;
				}
				else if (u < 2 * horz)
				{
					x += r + (int)(u - horz);
					y += h;
				}
				else if (u < 2 * horz + vert)
				{
					y += r + (int)(u - 2 * horz);
				}
				else if (u < 2 * horz + 2 * vert)
				{
					x += w;
					y += (int)(u - 2 * w - h);
				}
				else
				{
					int rx = radius;
					int ry = radius;
					if (2 * rx > w)
					{
						rx = w / 2;
					}
					if (2 * ry > h)
					{
						ry = h / 2;
					}
					u = 2 * Math.PI * rand.NextDouble();
					int dx = (int) (long)Math.Round(rx * Math.Cos(u), MidpointRounding.AwayFromZero);
					int dy = (int) (long)Math.Round(ry * Math.Sin(u), MidpointRounding.AwayFromZero);
					if (dx < 0)
					{
						x += r + dx;
					}
					else
					{
						x += r + horz + dx;
					}
					if (dy < 0)
					{
						y += r + dy;
					}
					else
					{
						y += r + vert + dy;
					}
				}

				int d = StrokeWidth;
				if (d > 1)
				{
					x += rand.Next(d) - d / 2;
					y += rand.Next(d) - d / 2;
				}
				return new Location(x, y);
			}
			else
			{
				return base.getRandomPoint(bds, rand);
			}
		}

		private static bool inCircle(int qx, int qy, int cx, int cy, int rx, int ry)
		{
			double dx = qx - cx;
			double dy = qy - cy;
			double sum = (dx * dx) / (4 * rx * rx) + (dy * dy) / (4 * ry * ry);
			return sum <= 0.25;
		}

		public override void draw(Graphics g, int x, int y, int w, int h)
		{
			int diam = 2 * radius;
			if (setForFill(g))
			{
				g.fillRoundRect(x, y, w, h, diam, diam);
			}
			if (setForStroke(g))
			{
				g.drawRoundRect(x, y, w, h, diam, diam);
			}
		}
	}

}
