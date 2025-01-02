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

	using CanvasObject = draw.model.CanvasObject;
	using Handle = draw.model.Handle;
	using HandleGesture = draw.model.HandleGesture;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using logisim.util;

	internal abstract class Rectangular : FillableCanvasObject
	{
		private Bounds bounds; // excluding the stroke's width

		public Rectangular(int x, int y, int w, int h)
		{
			bounds = Bounds.create(x, y, w, h);
		}

		public override bool matches(CanvasObject other)
		{
			if (other is Rectangular)
			{
				Rectangular that = (Rectangular) other;
				return this.bounds.Equals(that.bounds) && base.matches(that);
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			return bounds.GetHashCode() * 31 + base.matchesHashCode();
		}

		public virtual int X
		{
			get
			{
				return bounds.X;
			}
		}

		public virtual int Y
		{
			get
			{
				return bounds.Y;
			}
		}

		public virtual int Width
		{
			get
			{
				return bounds.Width;
			}
		}

		public virtual int Height
		{
			get
			{
				return bounds.Height;
			}
		}

		public override Bounds Bounds
		{
			get
			{
				int wid = StrokeWidth;
				object type = PaintType;
				if (wid < 2 || type == DrawAttr.PAINT_FILL)
				{
					return bounds;
				}
				else
				{
					return bounds.expand(wid / 2);
				}
			}
		}

		public override void translate(int dx, int dy)
		{
			bounds = bounds.translate(dx, dy);
		}

		public override IList<Handle> getHandles(HandleGesture gesture)
		{
			return UnmodifiableList.create(getHandleArray(gesture));
		}

		private Handle[] getHandleArray(HandleGesture gesture)
		{
			Bounds bds = bounds;
			int x0 = bds.X;
			int y0 = bds.Y;
			int x1 = x0 + bds.Width;
			int y1 = y0 + bds.Height;
			if (gesture == null)
			{
				return new Handle[]
				{
					new Handle(this, x0, y0),
					new Handle(this, x1, y0),
					new Handle(this, x1, y1),
					new Handle(this, x0, y1)
				};
			}
			else
			{
				int hx = gesture.Handle.X;
				int hy = gesture.Handle.Y;
				int dx = gesture.DeltaX;
				int dy = gesture.DeltaY;
				int newX0 = x0 == hx ? x0 + dx : x0;
				int newY0 = y0 == hy ? y0 + dy : y0;
				int newX1 = x1 == hx ? x1 + dx : x1;
				int newY1 = y1 == hy ? y1 + dy : y1;
				if (gesture.ShiftDown)
				{
					if (gesture.AltDown)
					{
						if (x0 == hx)
						{
							newX1 -= dx;
						}
						if (x1 == hx)
						{
							newX0 -= dx;
						}
						if (y0 == hy)
						{
							newY1 -= dy;
						}
						if (y1 == hy)
						{
							newY0 -= dy;
						}

						int w = Math.Abs(newX1 - newX0);
						int h = Math.Abs(newY1 - newY0);
						if (w > h)
						{ // reduce width to h
							int dw = (w - h) / 2;
							newX0 -= (newX0 > newX1 ? 1 : -1) * dw;
							newX1 -= (newX1 > newX0 ? 1 : -1) * dw;
						}
						else
						{
							int dh = (h - w) / 2;
							newY0 -= (newY0 > newY1 ? 1 : -1) * dh;
							newY1 -= (newY1 > newY0 ? 1 : -1) * dh;
						}
					}
					else
					{
						int w = Math.Abs(newX1 - newX0);
						int h = Math.Abs(newY1 - newY0);
						if (w > h)
						{ // reduce width to h
							if (x0 == hx)
							{
								newX0 = newX1 + (newX0 > newX1 ? 1 : -1) * h;
							}
							if (x1 == hx)
							{
								newX1 = newX0 + (newX1 > newX0 ? 1 : -1) * h;
							}
						}
						else
						{ // reduce height to w
							if (y0 == hy)
							{
								newY0 = newY1 + (newY0 > newY1 ? 1 : -1) * w;
							}
							if (y1 == hy)
							{
								newY1 = newY0 + (newY1 > newY0 ? 1 : -1) * w;
							}
						}
					}
				}
				else
				{
					if (gesture.AltDown)
					{
						if (x0 == hx)
						{
							newX1 -= dx;
						}
						if (x1 == hx)
						{
							newX0 -= dx;
						}
						if (y0 == hy)
						{
							newY1 -= dy;
						}
						if (y1 == hy)
						{
							newY0 -= dy;
						}
					}
					else
					{
						; // already handled
					}
				}
				return new Handle[]
				{
					new Handle(this, newX0, newY0),
					new Handle(this, newX1, newY0),
					new Handle(this, newX1, newY1),
					new Handle(this, newX0, newY1)
				};
			}
		}

		public override bool canMoveHandle(Handle handle)
		{
			return true;
		}

		public override Handle moveHandle(HandleGesture gesture)
		{
			Handle[] oldHandles = getHandleArray(null);
			Handle[] newHandles = getHandleArray(gesture);
			Handle moved = gesture == null ? null : gesture.Handle;
			Handle result = null;
			int x0 = int.MaxValue;
			int x1 = int.MinValue;
			int y0 = int.MaxValue;
			int y1 = int.MinValue;
			int i = -1;
			foreach (Handle h in newHandles)
			{
				i++;
				if (oldHandles[i].Equals(moved))
				{
					result = h;
				}
				int hx = h.X;
				int hy = h.Y;
				if (hx < x0)
				{
					x0 = hx;
				}
				if (hx > x1)
				{
					x1 = hx;
				}
				if (hy < y0)
				{
					y0 = hy;
				}
				if (hy > y1)
				{
					y1 = hy;
				}
			}
			bounds = Bounds.create(x0, y0, x1 - x0, y1 - y0);
			return result;
		}

		public override void paint(Graphics g, HandleGesture gesture)
		{
			if (gesture == null)
			{
				Bounds bds = bounds;
				draw(g, bds.X, bds.Y, bds.Width, bds.Height);
			}
			else
			{
				Handle[] handles = getHandleArray(gesture);
				Handle p0 = handles[0];
				Handle p1 = handles[2];
				int x0 = p0.X;
				int y0 = p0.Y;
				int x1 = p1.X;
				int y1 = p1.Y;
				if (x1 < x0)
				{
					int t = x0;
					x0 = x1;
					x1 = t;
				}
				if (y1 < y0)
				{
					int t = y0;
					y0 = y1;
					y1 = t;
				}

				draw(g, x0, y0, x1 - x0, y1 - y0);
			}
		}

		public override bool contains(Location loc, bool assumeFilled)
		{
			object type = PaintType;
			if (assumeFilled && type == DrawAttr.PAINT_STROKE)
			{
				type = DrawAttr.PAINT_STROKE_FILL;
			}
			Bounds b = bounds;
			int x = b.X;
			int y = b.Y;
			int w = b.Width;
			int h = b.Height;
			int qx = loc.X;
			int qy = loc.Y;
			if (type == DrawAttr.PAINT_FILL)
			{
				return isInRect(qx, qy, x, y, w, h) && contains(x, y, w, h, loc);
			}
			else if (type == DrawAttr.PAINT_STROKE)
			{
				int stroke = StrokeWidth;
				int tol2 = Math.Max(2 * Line.ON_LINE_THRESH, stroke);
				int tol = tol2 / 2;
				return isInRect(qx, qy, x - tol, y - tol, w + tol2, h + tol2) && contains(x - tol, y - tol, w + tol2, h + tol2, loc) && !contains(x + tol, y + tol, w - tol2, h - tol2, loc);
			}
			else if (type == DrawAttr.PAINT_STROKE_FILL)
			{
				int stroke = StrokeWidth;
				int tol2 = stroke;
				int tol = tol2 / 2;
				return isInRect(qx, qy, x - tol, y - tol, w + tol2, h + tol2) && contains(x - tol, y - tol, w + tol2, h + tol2, loc);
			}
			else
			{
				return false;
			}
		}

		internal virtual bool isInRect(int qx, int qy, int x0, int y0, int w, int h)
		{
			return qx >= x0 && qx < x0 + w && qy >= y0 && qy < y0 + h;
		}

		protected internal abstract bool contains(int x, int y, int w, int h, Location q);

		protected internal abstract void draw(Graphics g, int x, int y, int w, int h);
	}

}
