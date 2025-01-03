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
    using LogisimPlus.Java;

    public class Rectangle : Rectangular
	{
		public Rectangle(int x, int y, int w, int h) : base(x, y, w, h)
		{
		}

		public override bool matches(CanvasObject other)
		{
			if (other is Rectangle)
			{
				return base.matches(other);
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			return base.matchesHashCode();
		}

		public override string ToString()
		{
			return "Rectangle:" + Bounds;
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("shapeRect");
			}
		}

		public override Element toSvgElement(Document doc)
		{
			return SvgCreator.createRectangle(doc, this);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return DrawAttr.getFillAttributes(PaintType);
			}
		}

		protected internal override bool contains(int x, int y, int w, int h, Location q)
		{
			return isInRect(q.X, q.Y, x, y, w, h);
		}

		protected internal override Location getRandomPoint(Bounds bds, Random rand)
		{
			if (PaintType == DrawAttr.PAINT_STROKE)
			{
				int w = Width;
				int h = Height;
				int u = rand.Next(2 * w + 2 * h);
				int x = X;
				int y = Y;
				if (u < w)
				{
					x += u;
				}
				else if (u < 2 * w)
				{
					x += (u - w);
					y += h;
				}
				else if (u < 2 * w + h)
				{
					y += (u - 2 * w);
				}
				else
				{
					x += w;
					y += (u - 2 * w - h);
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

		public override void draw(JGraphics g, int x, int y, int w, int h)
		{
			if (setForFill(g))
			{
				g.fillRect(x, y, w, h);
			}
			if (setForStroke(g))
			{
				g.drawRect(x, y, w, h);
			}
		}
	}

}
