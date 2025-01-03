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
	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using Handle = draw.model.Handle;
	using HandleGesture = draw.model.HandleGesture;
	using logisim.data;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using logisim.util;

	public class Line : AbstractCanvasObject
	{
		internal const int ON_LINE_THRESH = 2;

		private int x0;
		private int y0;
		private int x1;
		private int y1;
		private Bounds bounds;
		private int strokeWidth;
		private Color strokeColor;

		public Line(int x0, int y0, int x1, int y1)
		{
			this.x0 = x0;
			this.y0 = y0;
			this.x1 = x1;
			this.y1 = y1;
			bounds = Bounds.create(x0, y0, 0, 0).add(x1, y1);
			strokeWidth = 1;
			strokeColor = Color.Black;
		}

		public override bool matches(CanvasObject other)
		{
			if (other is Line)
			{
				Line that = (Line) other;
				return this.x0 == that.x0 && this.y0 == that.x1 && this.x1 == that.y0 && this.y1 == that.y1 && this.strokeWidth == that.strokeWidth && this.strokeColor.Equals(that.strokeColor);
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			int ret = x0 * 31 + y0;
			ret = ret * 31 * 31 + x1 * 31 + y1;
			ret = ret * 31 + strokeWidth;
			ret = ret * 31 + strokeColor.GetHashCode();
			return ret;
		}

		public override Element toSvgElement(Document doc)
		{
			return SvgCreator.createLine(doc, this);
		}

		public virtual Location End0
		{
			get
			{
				return new Location(x0, y0);
			}
		}

		public virtual Location End1
		{
			get
			{
				return new Location(x1, y1);
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("shapeLine");
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return DrawAttr.ATTRS_STROKE;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public override object getValue(Attribute attr)
		{
			if (attr == DrawAttr.STROKE_COLOR)
			{
				return strokeColor;
			}
			else if (attr == DrawAttr.STROKE_WIDTH)
			{
				return Convert.ToInt32(strokeWidth);
			}
			else
			{
				return null;
			}
		}

        protected internal override void updateValue(Attribute attr, object value)
		{
			if (attr == DrawAttr.STROKE_COLOR)
			{
				strokeColor = (Color) value;
			}
			else if (attr == DrawAttr.STROKE_WIDTH)
			{
				strokeWidth = ((int?) value).Value;
			}
		}

		public override Bounds Bounds
		{
			get
			{
				return bounds;
			}
		}

		public override Location getRandomPoint(Bounds bds, Random rand)
		{
			double u = rand.NextDouble();
			int x = (int) (long)Math.Round(x0 + u * (x1 - x0), MidpointRounding.AwayFromZero);
			int y = (int) (long)Math.Round(y0 + u * (y1 - y0), MidpointRounding.AwayFromZero);
			int w = strokeWidth;
			if (w > 1)
			{
				x += (rand.Next(w) - w / 2);
				y += (rand.Next(w) - w / 2);
			}
			return new Location(x, y);
		}

		public override bool contains(Location loc, bool assumeFilled)
		{
			int xq = loc.X;
			int yq = loc.Y;
			double d = LineUtil.ptDistSqSegment(x0, y0, x1, y1, xq, yq);
			int thresh = Math.Max(ON_LINE_THRESH, strokeWidth / 2);
			return d < thresh * thresh;
		}

		public override void translate(int dx, int dy)
		{
			x0 += dx;
			y0 += dy;
			x1 += dx;
			y1 += dy;
		}

		public virtual List<Handle> Handles
		{
			get
			{
				return getHandles(null);
			}
		}

		public override List<Handle> getHandles(HandleGesture gesture)
		{
			if (gesture == null)
			{
				return UnmodifiableList.create(new Handle[]
				{
					new Handle(this, x0, y0),
					new Handle(this, x1, y1)
				});
			}
			else
			{
				Handle h = gesture.Handle;
				int dx = gesture.DeltaX;
				int dy = gesture.DeltaY;
				Handle[] ret = new Handle[2];
				ret[0] = new Handle(this, h.isAt(x0, y0) ? new Location(x0 + dx, y0 + dy) : new Location(x0, y0));
				ret[1] = new Handle(this, h.isAt(x1, y1) ? new Location(x1 + dx, y1 + dy) : new Location(x1, y1));
				return UnmodifiableList.create(ret);
			}
		}

		public override bool canMoveHandle(Handle handle)
		{
			return true;
		}

		public override Handle moveHandle(HandleGesture gesture)
		{
			Handle h = gesture.Handle;
			int dx = gesture.DeltaX;
			int dy = gesture.DeltaY;
			Handle ret = null;
			if (h.isAt(x0, y0))
			{
				x0 += dx;
				y0 += dy;
				ret = new Handle(this, x0, y0);
			}
			if (h.isAt(x1, y1))
			{
				x1 += dx;
				y1 += dy;
				ret = new Handle(this, x1, y1);
			}
			bounds = Bounds.create(x0, y0, 0, 0).add(x1, y1);
			return ret;
		}

		public override void paint(JGraphics g, HandleGesture gesture)
		{
			if (setForStroke(g))
			{
				int x0 = this.x0;
				int y0 = this.y0;
				int x1 = this.x1;
				int y1 = this.y1;
				Handle h = gesture.Handle;
				if (h.isAt(x0, y0))
				{
					x0 += gesture.DeltaX;
					y0 += gesture.DeltaY;
				}
				if (h.isAt(x1, y1))
				{
					x1 += gesture.DeltaX;
					y1 += gesture.DeltaY;
				}
				g.drawLine(x0, y0, x1, y1);
			}
		}

	}

}
