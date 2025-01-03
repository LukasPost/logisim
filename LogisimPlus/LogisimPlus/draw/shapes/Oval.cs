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

    public class Oval : Rectangular
	{
		public Oval(int x, int y, int w, int h) : base(x, y, w, h)
		{
		}

		public override bool matches(CanvasObject other)
		{
			if (other is Oval)
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

		public override Element toSvgElement(Document doc)
		{
			return SvgCreator.createOval(doc, this);
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("shapeOval");
			}
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
			int qx = q.X;
			int qy = q.Y;
			double dx = qx - (x + 0.5 * w);
			double dy = qy - (y + 0.5 * h);
			double sum = (dx * dx) / (w * w) + (dy * dy) / (h * h);
			return sum <= 0.25;
		}

		protected internal override Location getRandomPoint(Bounds bds, Random rand)
		{
			if (PaintType == DrawAttr.PAINT_STROKE)
			{
				double rx = Width / 2.0;
				double ry = Height / 2.0;
				double u = 2 * Math.PI * rand.NextDouble();
				int x = (int) (long)Math.Round(X + rx + rx * Math.Cos(u), MidpointRounding.AwayFromZero);
				int y = (int) (long)Math.Round(Y + ry + ry * Math.Sin(u), MidpointRounding.AwayFromZero);
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
				g.fillOval(x, y, w, h);
			}
			if (setForStroke(g))
			{
				g.drawOval(x, y, w, h);
			}
		}
	}

}
