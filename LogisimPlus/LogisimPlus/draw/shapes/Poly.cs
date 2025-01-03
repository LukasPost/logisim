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

	public class Poly : FillableCanvasObject
	{
		private bool closed;
		// "handles" should be immutable - create a new array and change using
		// setHandles rather than changing contents
		private Handle[] handles;
		private GeneralPath path;
		private double[] lens;
		private Bounds bounds;

		public Poly(bool closed, List<Location> locations)
		{
			Handle[] hs = new Handle[locations.Count];
			int i = -1;
			foreach (Location loc in locations)
			{
				i++;
				hs[i] = new Handle(this, loc.X, loc.Y);
			}

			this.closed = closed;
			handles = hs;
			recomputeBounds();
		}

		public override bool matches(CanvasObject other)
		{
			if (other is Poly)
			{
				Poly that = (Poly) other;
				Handle[] a = this.handles;
				Handle[] b = that.handles;
				if (this.closed != that.closed || a.Length != b.Length)
				{
					return false;
				}
				else
				{
					for (int i = 0, n = a.Length; i < n; i++)
					{
						if (!a[i].Equals(b[i]))
						{
							return false;
						}
					}
					return base.matches(that);
				}
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			int ret = base.matchesHashCode();
			ret = ret * 3 + (closed ? 1 : 0);
			Handle[] hs = handles;
			for (int i = 0, n = hs.Length; i < n; i++)
			{
				ret = ret * 31 + hs[i].GetHashCode();
			}
			return ret;
		}

		public override string DisplayName
		{
			get
			{
				if (closed)
				{
					return Strings.get("shapePolygon");
				}
				else
				{
					return Strings.get("shapePolyline");
				}
			}
		}

		public override Element toSvgElement(Document doc)
		{
			return SvgCreator.createPoly(doc, this);
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

		public override sealed bool contains(Location loc, bool assumeFilled)
		{
			object type = PaintType;
			if (assumeFilled && type == DrawAttr.PAINT_STROKE)
			{
				type = DrawAttr.PAINT_STROKE_FILL;
			}
			if (type == DrawAttr.PAINT_STROKE)
			{
				int thresh = Math.Max(Line.ON_LINE_THRESH, StrokeWidth / 2);
				PolyUtil.ClosestResult result = PolyUtil.getClosestPoint(loc, closed, handles);
				return result.DistanceSq < thresh * thresh;
			}
			else if (type == DrawAttr.PAINT_FILL)
			{
				GeneralPath path = Path;
				return path.contains(loc.X, loc.Y);
			}
			else
			{ // fill and stroke
				GeneralPath path = Path;
				if (path.contains(loc.X, loc.Y))
				{
					return true;
				}
				int width = StrokeWidth;
				PolyUtil.ClosestResult result = PolyUtil.getClosestPoint(loc, closed, handles);
				return result.DistanceSq < (width * width) / 4;
			}
		}

		public override sealed Location getRandomPoint(Bounds bds, Random rand)
		{
			if (PaintType == DrawAttr.PAINT_STROKE)
			{
				Location ret = getRandomBoundaryPoint(bds, rand);
				int w = StrokeWidth;
				if (w > 1)
				{
					int dx = rand.Next(w) - w / 2;
					int dy = rand.Next(w) - w / 2;
					ret = ret.translate(dx, dy);
				}
				return ret;
			}
			else
			{
				return base.getRandomPoint(bds, rand);
			}
		}

		private Location getRandomBoundaryPoint(Bounds bds, Random rand)
		{
			Handle[] hs = handles;
			double[] ls = lens;
			if (ls == null)
			{
				ls = new double[hs.Length + (closed ? 1 : 0)];
				double total = 0.0;
				for (int i = 0; i < ls.Length; i++)
				{
					int j = (i + 1) % hs.Length;
					total += LineUtil.distance(hs[i].X, hs[i].Y, hs[j].X, hs[j].Y);
					ls[i] = total;
				}
				lens = ls;
			}
			double pos = ls[ls.Length - 1] * rand.NextDouble();
			for (int i = 0; true; i++)
			{
				if (pos < ls[i])
				{
					Handle p = hs[i];
					Handle q = hs[(i + 1) % hs.Length];
					double u = MathHelper.NextDouble;
					int x = (int) (long)Math.Round(p.X + u * (q.X - p.X), MidpointRounding.AwayFromZero);
					int y = (int) (long)Math.Round(p.Y + u * (q.Y - p.Y), MidpointRounding.AwayFromZero);
					return new Location(x, y);
				}
			}
		}

		public override Bounds Bounds
		{
			get
			{
				return bounds;
			}
		}

		public override void translate(int dx, int dy)
		{
			Handle[] hs = handles;
			Handle[] @is = new Handle[hs.Length];
			for (int i = 0; i < hs.Length; i++)
			{
				@is[i] = new Handle(this, hs[i].X + dx, hs[i].Y + dy);
			}
			Handles = @is;
		}

		public virtual bool Closed
		{
			get
			{
				return closed;
			}
		}

		public override List<Handle> getHandles(HandleGesture gesture)
		{
			Handle[] hs = handles;
			if (gesture == null)
			{
				return UnmodifiableList.create(hs);
			}
			else
			{
				Handle g = gesture.Handle;
				Handle[] ret = new Handle[hs.Length];
				for (int i = 0, n = hs.Length; i < n; i++)
				{
					Handle h = hs[i];
					if (h.Equals(g))
					{
						int x = h.X + gesture.DeltaX;
						int y = h.Y + gesture.DeltaY;
						Location r;
						if (gesture.ShiftDown)
						{
							Location prev = hs[(i + n - 1) % n].Location;
							Location next = hs[(i + 1) % n].Location;
							if (!closed)
							{
								if (i == 0)
								{
									prev = null;
								}
								if (i == n - 1)
								{
									next = null;
								}
							}
							if (prev == null)
							{
								r = LineUtil.snapTo8Cardinals(next, x, y);
							}
							else if (next == null)
							{
								r = LineUtil.snapTo8Cardinals(prev, x, y);
							}
							else
							{
								Location to = new Location(x, y);
								Location a = LineUtil.snapTo8Cardinals(prev, x, y);
								Location b = LineUtil.snapTo8Cardinals(next, x, y);
								int ad = a.manhattanDistanceTo(to);
								int bd = b.manhattanDistanceTo(to);
								r = ad < bd ? a : b;
							}
						}
						else
						{
							r = new Location(x, y);
						}
						ret[i] = new Handle(this, r);
					}
					else
					{
						ret[i] = h;
					}
				}
				return UnmodifiableList.create(ret);
			}
		}

		public override bool canMoveHandle(Handle handle)
		{
			return true;
		}

		public override Handle moveHandle(HandleGesture gesture)
		{
			List<Handle> hs = getHandles(gesture);
			Handle[] @is = new Handle[hs.Count];
			Handle ret = null;
			int i = -1;
			foreach (Handle h in hs)
			{
				i++;
				@is[i] = h;
			}
			Handles = @is;
			return ret;
		}

		public override Handle canInsertHandle(Location loc)
		{
			PolyUtil.ClosestResult result = PolyUtil.getClosestPoint(loc, closed, handles);
			int thresh = Math.Max(Line.ON_LINE_THRESH, StrokeWidth / 2);
			if (result.DistanceSq < thresh * thresh)
			{
				Location resLoc = result.Location;
				if (result.PreviousHandle.isAt(resLoc) || result.NextHandle.isAt(resLoc))
				{
					return null;
				}
				else
				{
					return new Handle(this, result.Location);
				}
			}
			else
			{
				return null;
			}
		}

		public override Handle canDeleteHandle(Location loc)
		{
			int minHandles = closed ? 3 : 2;
			Handle[] hs = handles;
			if (hs.Length <= minHandles)
			{
				return null;
			}
			else
			{
				int qx = loc.X;
				int qy = loc.Y;
				int w = Math.Max(Line.ON_LINE_THRESH, StrokeWidth / 2);
				foreach (Handle h in hs)
				{
					int hx = h.X;
					int hy = h.Y;
					if (LineUtil.distance(qx, qy, hx, hy) < w * w)
					{
						return h;
					}
				}
				return null;
			}
		}

		public override void insertHandle(Handle desired, Handle previous)
		{
			Location loc = desired.Location;
			Handle[] hs = handles;
			Handle prev;
			if (previous == null)
			{
				PolyUtil.ClosestResult result = PolyUtil.getClosestPoint(loc, closed, hs);
				prev = result.PreviousHandle;
			}
			else
			{
				prev = previous;
			}
			Handle[] @is = new Handle[hs.Length + 1];
			bool inserted = false;
			for (int i = 0; i < hs.Length; i++)
			{
				if (inserted)
				{
					@is[i + 1] = hs[i];
				}
				else if (hs[i].Equals(prev))
				{
					inserted = true;
					@is[i] = hs[i];
					@is[i + 1] = desired;
				}
				else
				{
					@is[i] = hs[i];
				}
			}
			if (!inserted)
			{
				throw new System.ArgumentException("no such handle");
			}
			Handles = @is;
		}

		public override Handle deleteHandle(Handle handle)
		{
			Handle[] hs = handles;
			int n = hs.Length;
			Handle[] @is = new Handle[n - 1];
			Handle previous = null;
			bool deleted = false;
			for (int i = 0; i < n; i++)
			{
				if (deleted)
				{
					@is[i - 1] = hs[i];
				}
				else if (hs[i].Equals(handle))
				{
					if (previous == null)
					{
						previous = hs[n - 1];
					}
					deleted = true;
				}
				else
				{
					previous = hs[i];
					@is[i] = hs[i];
				}
			}
			Handles = @is;
			return previous;
		}

		public override void paint(JGraphics g, HandleGesture gesture)
		{
			List<Handle> hs = getHandles(gesture);
			int[] xs = new int[hs.Count];
			int[] ys = new int[hs.Count];
			int i = -1;
			foreach (Handle h in hs)
			{
				i++;
				xs[i] = h.X;
				ys[i] = h.Y;
			}

			if (setForFill(g))
			{
				g.fillPolygon(xs, ys, xs.Length);
			}
			if (setForStroke(g))
			{
				if (closed)
				{
					g.drawPolygon(xs, ys, xs.Length);
				}
				else
				{
					g.drawPolyline(xs, ys, xs.Length);
				}
			}
		}

		private Handle[] Handles
		{
			set
			{
				handles = value;
				lens = null;
				path = null;
				recomputeBounds();
			}
		}

		private void recomputeBounds()
		{
			Handle[] hs = handles;
			int x0 = hs[0].X;
			int y0 = hs[0].Y;
			int x1 = x0;
			int y1 = y0;
			for (int i = 1; i < hs.Length; i++)
			{
				int x = hs[i].X;
				int y = hs[i].Y;
				if (x < x0)
				{
					x0 = x;
				}
				if (x > x1)
				{
					x1 = x;
				}
				if (y < y0)
				{
					y0 = y;
				}
				if (y > y1)
				{
					y1 = y;
				}
			}
			Bounds bds = Bounds.create(x0, y0, x1 - x0 + 1, y1 - y0 + 1);
			int stroke = StrokeWidth;
			bounds = stroke < 2 ? bds : bds.expand(stroke / 2);
		}

		private GeneralPath Path
		{
			get
			{
				GeneralPath p = path;
				if (p == null)
				{
					p = new GeneralPath();
					Handle[] hs = handles;
					if (hs.Length > 0)
					{
						bool first = true;
						foreach (Handle h in hs)
						{
							if (first)
							{
								p.moveTo(h.X, h.Y);
								first = false;
							}
							else
							{
								p.lineTo(h.X, h.Y);
							}
						}
					}
					path = p;
				}
				return p;
			}
		}
	}

}
