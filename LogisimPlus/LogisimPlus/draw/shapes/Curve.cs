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
	using Handle = draw.model.Handle;
	using HandleGesture = draw.model.HandleGesture;
	using logisim.data;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using logisim.util;

	public class Curve : FillableCanvasObject
	{
		private Location p0;
		private Location p1;
		private Location p2;
		private Bounds bounds;

		public Curve(Location end0, Location end1, Location ctrl)
		{
			this.p0 = end0;
			this.p1 = ctrl;
			this.p2 = end1;
			bounds = CurveUtil.getBounds(toArray(p0), toArray(p1), toArray(p2));
		}

		public override bool matches(CanvasObject other)
		{
			if (other is Curve)
			{
				Curve that = (Curve) other;
				return this.p0.Equals(that.p0) && this.p1.Equals(that.p1) && this.p2.Equals(that.p2) && base.matches(that);
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			int ret = p0.GetHashCode();
			ret = ret * 31 * 31 + p1.GetHashCode();
			ret = ret * 31 * 31 + p2.GetHashCode();
			ret = ret * 31 + base.matchesHashCode();
			return ret;
		}

		public override Element toSvgElement(Document doc)
		{
			return SvgCreator.createCurve(doc, this);
		}

		public virtual Location End0
		{
			get
			{
				return p0;
			}
		}

		public virtual Location End1
		{
			get
			{
				return p2;
			}
		}

		public virtual Location Control
		{
			get
			{
				return p1;
			}
		}

		public virtual QuadCurve2D Curve2D
		{
			get
			{
				return new QuadCurve2D.Double(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y);
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("shapeCurve");
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return DrawAttr.getFillAttributes(PaintType);
			}
		}

		public override Bounds Bounds
		{
			get
			{
				return bounds;
			}
		}

		public override bool contains(Location loc, bool assumeFilled)
		{
			object type = PaintType;
			if (assumeFilled && type == DrawAttr.PAINT_STROKE)
			{
				type = DrawAttr.PAINT_STROKE_FILL;
			}
			if (type != DrawAttr.PAINT_FILL)
			{
				int stroke = StrokeWidth;
				double[] q = toArray(loc);
				double[] p0 = toArray(this.p0);
				double[] p1 = toArray(this.p1);
				double[] p2 = toArray(this.p2);
				double[] p = CurveUtil.findNearestPoint(q, p0, p1, p2);
				if (p == null)
				{
					return false;
				}

				int thr;
				if (type == DrawAttr.PAINT_STROKE)
				{
					thr = Math.Max(Line.ON_LINE_THRESH, stroke / 2);
				}
				else
				{
					thr = stroke / 2;
				}
				if (LineUtil.distanceSquared(p[0], p[1], q[0], q[1]) < thr * thr)
				{
					return true;
				}
			}
			if (type != DrawAttr.PAINT_STROKE)
			{
				QuadCurve2D curve = getCurve(null);
				if (curve.contains(loc.X, loc.Y))
				{
					return true;
				}
			}
			return false;
		}

		public override void translate(int dx, int dy)
		{
			p0 = p0.translate(dx, dy);
			p1 = p1.translate(dx, dy);
			p2 = p2.translate(dx, dy);
			bounds = bounds.translate(dx, dy);
		}

		public virtual IList<Handle> Handles
		{
			get
			{
				return UnmodifiableList.create(getHandleArray(null));
			}
		}

		public override IList<Handle> getHandles(HandleGesture gesture)
		{
			return UnmodifiableList.create(getHandleArray(gesture));
		}

		private Handle[] getHandleArray(HandleGesture gesture)
		{
			if (gesture == null)
			{
				return new Handle[]
				{
					new Handle(this, p0),
					new Handle(this, p1),
					new Handle(this, p2)
				};
			}
			else
			{
				Handle g = gesture.Handle;
				int gx = g.X + gesture.DeltaX;
				int gy = g.Y + gesture.DeltaY;
				Handle[] ret = new Handle[]
				{
					new Handle(this, p0),
					new Handle(this, p1),
					new Handle(this, p2)
				};
				if (g.isAt(p0))
				{
					if (gesture.ShiftDown)
					{
						Location p = LineUtil.snapTo8Cardinals(p2, gx, gy);
						ret[0] = new Handle(this, p);
					}
					else
					{
						ret[0] = new Handle(this, gx, gy);
					}
				}
				else if (g.isAt(p2))
				{
					if (gesture.ShiftDown)
					{
						Location p = LineUtil.snapTo8Cardinals(p0, gx, gy);
						ret[2] = new Handle(this, p);
					}
					else
					{
						ret[2] = new Handle(this, gx, gy);
					}
				}
				else if (g.isAt(p1))
				{
					if (gesture.ShiftDown)
					{
						double x0 = p0.X;
						double y0 = p0.Y;
						double x1 = p2.X;
						double y1 = p2.Y;
						double midx = (x0 + x1) / 2;
						double midy = (y0 + y1) / 2;
						double dx = x1 - x0;
						double dy = y1 - y0;
						double[] p = LineUtil.nearestPointInfinite(gx, gy, midx, midy, midx - dy, midy + dx);
						gx = (int) (long)Math.Round(p[0], MidpointRounding.AwayFromZero);
						gy = (int) (long)Math.Round(p[1], MidpointRounding.AwayFromZero);
					}
					if (gesture.AltDown)
					{
						double[] e0 = new double[] {p0.X, p0.Y};
						double[] e1 = new double[] {p2.X, p2.Y};
						double[] mid = new double[] {gx, gy};
						double[] ct = CurveUtil.interpolate(e0, e1, mid);
						gx = (int) (long)Math.Round(ct[0], MidpointRounding.AwayFromZero);
						gy = (int) (long)Math.Round(ct[1], MidpointRounding.AwayFromZero);
					}
					ret[1] = new Handle(this, gx, gy);
				}
				return ret;
			}
		}

		public override bool canMoveHandle(Handle handle)
		{
			return true;
		}

		public override Handle moveHandle(HandleGesture gesture)
		{
			Handle[] hs = getHandleArray(gesture);
			Handle ret = null;
			if (!hs[0].Equals(p0))
			{
				p0 = hs[0].Location;
				ret = hs[0];
			}
			if (!hs[1].Equals(p1))
			{
				p1 = hs[1].Location;
				ret = hs[1];
			}
			if (!hs[2].Equals(p2))
			{
				p2 = hs[2].Location;
				ret = hs[2];
			}
			bounds = CurveUtil.getBounds(toArray(p0), toArray(p1), toArray(p2));
			return ret;
		}

		public override void paint(Graphics g, HandleGesture gesture)
		{
			QuadCurve2D curve = getCurve(gesture);
			if (setForFill(g))
			{
				((Graphics2D) g).fill(curve);
			}
			if (setForStroke(g))
			{
				((Graphics2D) g).draw(curve);
			}
		}

		private QuadCurve2D getCurve(HandleGesture gesture)
		{
			Handle[] p = getHandleArray(gesture);
			return new QuadCurve2D.Double(p[0].X, p[0].Y, p[1].X, p[1].Y, p[2].X, p[2].Y);
		}

		private static double[] toArray(Location loc)
		{
			return new double[] {loc.X, loc.Y};
		}
	}

}
