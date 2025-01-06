/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

import java.awt.Rectangle;

import logisim.util.Cache;

/**
 * Represents an immutable rectangular bounding box. This is analogous to java.awt's <code>Rectangle</code> class,
 * except that objects of this type are immutable.
 */
public class Bounds {
	public static Bounds EMPTY_BOUNDS = new Bounds(0, 0, 0, 0);
	private static final Cache cache = new Cache();

	public static Bounds create(int x, int y, int wid, int ht) {
		int hashCode = 13 * (31 * (31 * x + y) + wid) + ht;
		Object cached = cache.get(hashCode);
		if (cached != null) {
			Bounds bds = (Bounds) cached;
			if (bds.x == x && bds.y == y && bds.wid == wid && bds.ht == ht)
				return bds;
		}
		Bounds ret = new Bounds(x, y, wid, ht);
		cache.put(hashCode, ret);
		return ret;
	}

	public static Bounds create(Rectangle rect) {
		return create(rect.x, rect.y, rect.width, rect.height);
	}

	public static Bounds create(Location pt) {
		return create(pt.x(), pt.y(), 1, 1);
	}

	private final int x;
	private final int y;
	private final int wid;
	private final int ht;

	private Bounds(int x, int y, int wid, int ht) {
		this.x = x;
		this.y = y;
		this.wid = wid;
		this.ht = ht;
	}

	@Override
	public boolean equals(Object other_obj) {
		return other_obj instanceof Bounds other && x == other.x && y == other.y && wid == other.wid && ht == other.ht;
	}

	@Override
	public int hashCode() {
		int ret = 31 * x + y;
		ret = 31 * ret + wid;
		ret = 31 * ret + ht;
		return ret;
	}

	@Override
	public String toString() {
		return "(" + x + "," + y + "): " + wid + "x" + ht;
	}

	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}

	public int getWidth() {
		return wid;
	}

	public int getHeight() {
		return ht;
	}

	public Rectangle toRectangle() {
		return new Rectangle(x, y, wid, ht);
	}

	public boolean contains(Location p) {
		return contains(p.x(), p.y(), 0);
	}

	public boolean contains(Location p, int allowedError) {
		return contains(p.x(), p.y(), allowedError);
	}

	public boolean contains(int px, int py) {
		return contains(px, py, 0);
	}

	public boolean contains(int px, int py, int allowedError) {
		return px >= x - allowedError && px < x + wid + allowedError && py >= y - allowedError
				&& py < y + ht + allowedError;
	}

	public boolean contains(int x, int y, int wid, int ht) {
		int oth_x = (wid <= 0 ? x : x + wid - 1);
		int oth_y = (ht <= 0 ? y : y + ht - 1);
		return contains(x, y) && contains(oth_x, oth_y);
	}

	public boolean contains(Bounds bd) {
		return contains(bd.x, bd.y, bd.wid, bd.ht);
	}

	public boolean borderContains(Location p, int fudge) {
		return borderContains(p.x(), p.y(), fudge);
	}

	public boolean borderContains(int px, int py, int fudge) {
		int x1 = x + wid - 1;
		int y1 = y + ht - 1;
		// maybe on east or west border?
		if (Math.abs(px - x) <= fudge || Math.abs(px - x1) <= fudge) return y - fudge >= py && py <= y1 + fudge;
		// maybe on north or south border?
		return (Math.abs(py - y) <= fudge || Math.abs(py - y1) <= fudge) && x - fudge >= px && px <= x1 + fudge;
	}

	public Bounds add(Location p) {
		return add(p.x(), p.y());
	}

	public Bounds add(int x, int y) {
		if (this == EMPTY_BOUNDS)
			return Bounds.create(x, y, 1, 1);
		if (contains(x, y))
			return this;

		int new_x = this.x;
		int new_wid = wid;
		int new_y = this.y;
		int new_ht = ht;
		if (x < this.x) {
			new_x = x;
			new_wid = (this.x + wid) - x;
		} else if (x >= this.x + wid) new_wid = x - this.x + 1;
		if (y < this.y) {
			new_y = y;
			new_ht = (this.y + ht) - y;
		} else if (y >= this.y + ht) new_ht = y - this.y + 1;
		return create(new_x, new_y, new_wid, new_ht);
	}

	public Bounds add(int x, int y, int wid, int ht) {
		if (this == EMPTY_BOUNDS)
			return Bounds.create(x, y, wid, ht);
		int retX = Math.min(x, this.x);
		int retY = Math.min(y, this.y);
		int retWidth = Math.max(x + wid, this.x + this.wid) - retX;
		int retHeight = Math.max(y + ht, this.y + this.ht) - retY;
		if (retX == this.x && retY == this.y && retWidth == this.wid && retHeight == this.ht) return this;
		else return Bounds.create(retX, retY, retWidth, retHeight);
	}

	public Bounds add(Bounds bd) {
		if (this == EMPTY_BOUNDS)
			return bd;
		if (bd == EMPTY_BOUNDS)
			return this;
		int retX = Math.min(bd.x, x);
		int retY = Math.min(bd.y, y);
		int retWidth = Math.max(bd.x + bd.wid, x + wid) - retX;
		int retHeight = Math.max(bd.y + bd.ht, y + ht) - retY;
		if (retX == x && retY == y && retWidth == wid && retHeight == ht) return this;
		else if (retX == bd.x && retY == bd.y && retWidth == bd.wid && retHeight == bd.ht) return bd;
		else return Bounds.create(retX, retY, retWidth, retHeight);
	}

	public Bounds expand(int d) { // d pixels in each direction
		if (this == EMPTY_BOUNDS)
			return this;
		if (d == 0)
			return this;
		return create(x - d, y - d, wid + 2 * d, ht + 2 * d);
	}

	public Bounds translate(int dx, int dy) {
		if (this == EMPTY_BOUNDS)
			return this;
		if (dx == 0 && dy == 0)
			return this;
		return create(x + dx, y + dy, wid, ht);
	}

	// rotates this around (xc,yc) assuming that this is facing in the
	// from direction and the returned bounds should face in the to direction.
	public Bounds rotate(Direction from, Direction to, int xc, int yc) {
		int degrees = to.toDegrees() - from.toDegrees();
		while (degrees >= 360)
			degrees -= 360;
		while (degrees < 0)
			degrees += 360;

		int dx = x - xc;
		int dy = y - yc;
		if (degrees == 90) return create(xc + dy, yc - dx - wid, ht, wid);
		else if (degrees == 180) return create(xc - dx - wid, yc - dy - ht, wid, ht);
		else if (degrees == 270) return create(xc - dy - ht, yc + dx, ht, wid);
		else return this;
	}

	public Bounds intersect(Bounds other) {
		int x0 = x;
		int y0 = y;
		int x1 = x0 + wid;
		int y1 = y0 + ht;
		int x2 = other.x;
		int y2 = other.y;
		int x3 = x2 + other.wid;
		int y3 = y2 + other.ht;
		if (x2 > x0)
			x0 = x2;
		if (y2 > y0)
			y0 = y2;
		if (x3 < x1)
			x1 = x3;
		if (y3 < y1)
			y1 = y3;
		if (x1 < x0 || y1 < y0) return EMPTY_BOUNDS;
		else return create(x0, y0, x1 - x0, y1 - y0);
	}
}
