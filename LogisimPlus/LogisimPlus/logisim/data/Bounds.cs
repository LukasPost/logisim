// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{

	using Cache = logisim.util.Cache;

	/// <summary>
	/// Represents an immutable rectangular bounding box. This is analogous to java.awt's <code>Rectangle</code> class,
	/// except that objects of this type are immutable.
	/// </summary>
	public class Bounds
	{
		public static Bounds EMPTY_BOUNDS = new Bounds(0, 0, 0, 0);
		private static readonly Cache cache = new Cache();

		public static Bounds create(int x, int y, int wid, int ht)
		{
			int hashCode = 13 * (31 * (31 * x + y) + wid) + ht;
			object cached = cache.get(hashCode);
			if (cached != null)
			{
				Bounds bds = (Bounds) cached;
				if (bds.x == x && bds.y == y && bds.wid == wid && bds.ht == ht)
				{
					return bds;
				}
			}
			Bounds ret = new Bounds(x, y, wid, ht);
			cache.put(hashCode, ret);
			return ret;
		}

		public static Bounds create(Rectangle rect)
		{
			return create(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Bounds create(Location pt)
		{
			return create(pt.X, pt.Y, 1, 1);
		}

		private readonly int x;
		private readonly int y;
		private readonly int wid;
		private readonly int ht;

		private Bounds(int x, int y, int wid, int ht)
		{
			this.x = x;
			this.y = y;
			this.wid = wid;
			this.ht = ht;
			if (wid < 0)
			{
				x += wid / 2;
				wid = 0;
			}
			if (ht < 0)
			{
				y += ht / 2;
				ht = 0;
			}
		}

		public override bool Equals(object other_obj)
		{
			if (!(other_obj is Bounds))
			{
				return false;
			}
			Bounds other = (Bounds) other_obj;
			return x == other.X && y == other.Y && wid == other.wid && ht == other.ht;
		}

		public override int GetHashCode()
		{
			int ret = 31 * x + y;
			ret = 31 * ret + wid;
			ret = 31 * ret + ht;
			return ret;
		}

		public override string ToString()
		{
			return "(" + x + "," + y + "): " + wid + "x" + ht;
		}

		public virtual int X
		{
			get
			{
				return x;
			}
		}

		public virtual int Y
		{
			get
			{
				return y;
			}
		}

		public virtual int Width
		{
			get
			{
				return wid;
			}
		}

		public virtual int Height
		{
			get
			{
				return ht;
			}
		}

		public virtual Rectangle toRectangle()
		{
			return new Rectangle(x, y, wid, ht);
		}

		public virtual bool contains(Location p)
		{
			return contains(p.X, p.Y, 0);
		}

		public virtual bool contains(Location p, int allowedError)
		{
			return contains(p.X, p.Y, allowedError);
		}

		public virtual bool contains(int px, int py)
		{
			return contains(px, py, 0);
		}

		public virtual bool contains(int px, int py, int allowedError)
		{
			return px >= x - allowedError && px < x + wid + allowedError && py >= y - allowedError && py < y + ht + allowedError;
		}

		public virtual bool contains(int x, int y, int wid, int ht)
		{
			int oth_x = (wid <= 0 ? x : x + wid - 1);
			int oth_y = (ht <= 0 ? y : y + ht - 1);
			return contains(x, y) && contains(oth_x, oth_y);
		}

		public virtual bool contains(Bounds bd)
		{
			return contains(bd.x, bd.y, bd.wid, bd.ht);
		}

		public virtual bool borderContains(Location p, int fudge)
		{
			return borderContains(p.X, p.Y, fudge);
		}

		public virtual bool borderContains(int px, int py, int fudge)
		{
			int x1 = x + wid - 1;
			int y1 = y + ht - 1;
			if (Math.Abs(px - x) <= fudge || Math.Abs(px - x1) <= fudge)
			{
				// maybe on east or west border?
				return y - fudge >= py && py <= y1 + fudge;
			}
			if (Math.Abs(py - y) <= fudge || Math.Abs(py - y1) <= fudge)
			{
				// maybe on north or south border?
				return x - fudge >= px && px <= x1 + fudge;
			}
			return false;
		}

		public virtual Bounds add(Location p)
		{
			return add(p.X, p.Y);
		}

		public virtual Bounds add(int x, int y)
		{
			if (this == EMPTY_BOUNDS)
			{
				return Bounds.create(x, y, 1, 1);
			}
			if (contains(x, y))
			{
				return this;
			}

			int new_x = this.x;
			int new_wid = this.wid;
			int new_y = this.y;
			int new_ht = this.ht;
			if (x < this.x)
			{
				new_x = x;
				new_wid = (this.x + this.wid) - x;
			}
			else if (x >= this.x + this.wid)
			{
				new_x = this.x;
				new_wid = x - this.x + 1;
			}
			if (y < this.y)
			{
				new_y = y;
				new_ht = (this.y + this.ht) - y;
			}
			else if (y >= this.y + this.ht)
			{
				new_y = this.y;
				new_ht = y - this.y + 1;
			}
			return create(new_x, new_y, new_wid, new_ht);
		}

		public virtual Bounds add(int x, int y, int wid, int ht)
		{
			if (this == EMPTY_BOUNDS)
			{
				return Bounds.create(x, y, wid, ht);
			}
			int retX = Math.Min(x, this.x);
			int retY = Math.Min(y, this.y);
			int retWidth = Math.Max(x + wid, this.x + this.wid) - retX;
			int retHeight = Math.Max(y + ht, this.y + this.ht) - retY;
			if (retX == this.x && retY == this.y && retWidth == this.wid && retHeight == this.ht)
			{
				return this;
			}
			else
			{
				return Bounds.create(retX, retY, retWidth, retHeight);
			}
		}

		public virtual Bounds add(Bounds bd)
		{
			if (this == EMPTY_BOUNDS)
			{
				return bd;
			}
			if (bd == EMPTY_BOUNDS)
			{
				return this;
			}
			int retX = Math.Min(bd.x, this.x);
			int retY = Math.Min(bd.y, this.y);
			int retWidth = Math.Max(bd.x + bd.wid, this.x + this.wid) - retX;
			int retHeight = Math.Max(bd.y + bd.ht, this.y + this.ht) - retY;
			if (retX == this.x && retY == this.y && retWidth == this.wid && retHeight == this.ht)
			{
				return this;
			}
			else if (retX == bd.x && retY == bd.y && retWidth == bd.wid && retHeight == bd.ht)
			{
				return bd;
			}
			else
			{
				return Bounds.create(retX, retY, retWidth, retHeight);
			}
		}

		public virtual Bounds expand(int d)
		{ // d pixels in each direction
			if (this == EMPTY_BOUNDS)
			{
				return this;
			}
			if (d == 0)
			{
				return this;
			}
			return create(x - d, y - d, wid + 2 * d, ht + 2 * d);
		}

		public virtual Bounds translate(int dx, int dy)
		{
			if (this == EMPTY_BOUNDS)
			{
				return this;
			}
			if (dx == 0 && dy == 0)
			{
				return this;
			}
			return create(x + dx, y + dy, wid, ht);
		}

		// rotates this around (xc,yc) assuming that this is facing in the
		// from direction and the returned bounds should face in the to direction.
		public virtual Bounds rotate(Direction from, Direction to, int xc, int yc)
		{
			int degrees = to.toDegrees() - from.toDegrees();
			while (degrees >= 360)
			{
				degrees -= 360;
			}
			while (degrees < 0)
			{
				degrees += 360;
			}

			int dx = x - xc;
			int dy = y - yc;
			if (degrees == 90)
			{
				return create(xc + dy, yc - dx - wid, ht, wid);
			}
			else if (degrees == 180)
			{
				return create(xc - dx - wid, yc - dy - ht, wid, ht);
			}
			else if (degrees == 270)
			{
				return create(xc - dy - ht, yc + dx, ht, wid);
			}
			else
			{
				return this;
			}
		}

		public virtual Bounds intersect(Bounds other)
		{
			int x0 = this.x;
			int y0 = this.y;
			int x1 = x0 + this.wid;
			int y1 = y0 + this.ht;
			int x2 = other.X;
			int y2 = other.Y;
			int x3 = x2 + other.wid;
			int y3 = y2 + other.ht;
			if (x2 > x0)
			{
				x0 = x2;
			}
			if (y2 > y0)
			{
				y0 = y2;
			}
			if (x3 < x1)
			{
				x1 = x3;
			}
			if (y3 < y1)
			{
				y1 = y3;
			}
			if (x1 < x0 || y1 < y0)
			{
				return EMPTY_BOUNDS;
			}
			else
			{
				return create(x0, y0, x1 - x0, y1 - y0);
			}
		}
	}

}
