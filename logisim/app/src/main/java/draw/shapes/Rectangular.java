/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import java.awt.Graphics;
import java.util.List;

import draw.model.CanvasObject;
import draw.model.Handle;
import draw.model.HandleGesture;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.UnmodifiableList;

abstract class Rectangular extends FillableCanvasObject {
	private Bounds bounds; // excluding the stroke's width

	public Rectangular(int x, int y, int w, int h) {
		bounds = Bounds.create(x, y, w, h);
	}

	@Override
	public boolean matches(CanvasObject other) {
		return other instanceof Rectangular that && bounds.equals(that.bounds) && super.matches(that);
	}

	@Override
	public int matchesHashCode() {
		return bounds.hashCode() * 31 + super.matchesHashCode();
	}

	public int getX() {
		return bounds.getX();
	}

	public int getY() {
		return bounds.getY();
	}

	public int getWidth() {
		return bounds.getWidth();
	}

	public int getHeight() {
		return bounds.getHeight();
	}

	@Override
	public Bounds getBounds() {
		int wid = getStrokeWidth();
		return wid < 2 || getPaintType() == DrawAttr.PAINT_FILL ? bounds : bounds.expand(wid / 2);
	}

	@Override
	public void translate(Location distance) {
		bounds = bounds.translate(distance);
	}

	@Override
	public List<Handle> getHandles(HandleGesture gesture) {
		return UnmodifiableList.create(getHandleArray(gesture));
	}

	private Handle[] getHandleArray(HandleGesture gesture) {
		Bounds bds = bounds;
		int x0 = bds.getX();
		int y0 = bds.getY();
		int x1 = x0 + bds.getWidth();
		int y1 = y0 + bds.getHeight();
		if (gesture == null)
			return new Handle[]{
					new Handle(this, x0, y0),
					new Handle(this, x1, y0),
					new Handle(this, x1, y1),
					new Handle(this, x0, y1)
			};

		int hx = gesture.getHandle().getX();
		int hy = gesture.getHandle().getY();
		int dx = gesture.getDeltaX();
		int dy = gesture.getDeltaY();
		int newX0 = x0 == hx ? x0 + dx : x0;
		int newY0 = y0 == hy ? y0 + dy : y0;
		int newX1 = x1 == hx ? x1 + dx : x1;
		int newY1 = y1 == hy ? y1 + dy : y1;
		if (gesture.isShiftDown()) if (gesture.isAltDown()) {
			if (x0 == hx)
				newX1 -= dx;
			if (x1 == hx)
				newX0 -= dx;
			if (y0 == hy)
				newY1 -= dy;
			if (y1 == hy)
				newY0 -= dy;

			int w = Math.abs(newX1 - newX0);
			int h = Math.abs(newY1 - newY0);
			if (w > h) { // reduce width to h
				int dw = (w - h) / 2;
				newX0 -= newX0 > newX1 ? dw : -dw;
				newX1 -= newX1 > newX0 ? dw : -dw;
			}
			else {
				int dh = (h - w) / 2;
				newY0 -= newY0 > newY1 ? dh : -dh;
				newY1 -= newY1 > newY0 ? dh : -dh;
			}
		}
		else {
			int w = Math.abs(newX1 - newX0);
			int h = Math.abs(newY1 - newY0);
			if (w > h) { // reduce width to h
				if (x0 == hx)
					newX0 = newX1 + (newX0 > newX1 ? h : -h);
				if (x1 == hx)
					newX1 = newX0 + (newX1 > newX0 ? h : -h);
			}
			else { // reduce height to w
				if (y0 == hy)
					newY0 = newY1 + (newY0 > newY1 ? w : -w);
				if (y1 == hy)
					newY1 = newY0 + (newY1 > newY0 ? w : -w);
			}
		}
		else if (gesture.isAltDown()) {
			if (x0 == hx)
				newX1 -= dx;
			if (x1 == hx)
				newX0 -= dx;
			if (y0 == hy)
				newY1 -= dy;
			if (y1 == hy)
				newY0 -= dy;
		}
		return new Handle[]{
				new Handle(this, newX0, newY0),
				new Handle(this, newX1, newY0),
				new Handle(this, newX1, newY1),
				new Handle(this, newX0, newY1)
		};
	}

	@Override
	public boolean canMoveHandle(Handle handle) {
		return true;
	}

	@Override
	public Handle moveHandle(HandleGesture gesture) {
		Handle[] oldHandles = getHandleArray(null);
		Handle[] newHandles = getHandleArray(gesture);
		Handle moved = gesture == null ? null : gesture.getHandle();
		Handle result = null;
		int x0 = Integer.MAX_VALUE;
		int x1 = Integer.MIN_VALUE;
		int y0 = Integer.MAX_VALUE;
		int y1 = Integer.MIN_VALUE;
		int i = -1;
		for (Handle h : newHandles) {
			i++;
			if (oldHandles[i].equals(moved))
				result = h;
			int hx = h.getX();
			int hy = h.getY();
			if (hx < x0)
				x0 = hx;
			if (hx > x1)
				x1 = hx;
			if (hy < y0)
				y0 = hy;
			if (hy > y1)
				y1 = hy;
		}
		bounds = Bounds.create(x0, y0, x1 - x0, y1 - y0);
		return result;
	}

	@Override
	public void paint(Graphics g, HandleGesture gesture) {
		if (gesture == null) {
			Bounds bds = bounds;
			draw(g, bds.getX(), bds.getY(), bds.getWidth(), bds.getHeight());
			return;
		}

		Handle[] handles = getHandleArray(gesture);
		Handle p0 = handles[0];
		Handle p1 = handles[2];
		int x0 = p0.getX();
		int y0 = p0.getY();
		int x1 = p1.getX();
		int y1 = p1.getY();
		if (x1 < x0) {
			int t = x0;
			x0 = x1;
			x1 = t;
		}
		if (y1 < y0) {
			int t = y0;
			y0 = y1;
			y1 = t;
		}

		draw(g, x0, y0, x1 - x0, y1 - y0);
	}

	@Override
	public boolean contains(Location loc, boolean assumeFilled) {
		Object type = getPaintType();
		if (assumeFilled && type == DrawAttr.PAINT_STROKE)
			type = DrawAttr.PAINT_STROKE_FILL;
		Bounds b = bounds;
		int x = b.getX();
		int y = b.getY();
		int w = b.getWidth();
		int h = b.getHeight();
		int qx = loc.x();
		int qy = loc.y();
		if (type == DrawAttr.PAINT_FILL)
			return isInRect(qx, qy, x, y, w, h) && contains(x, y, w, h, loc);
		if (type == DrawAttr.PAINT_STROKE) {
			int stroke = getStrokeWidth();
			int tol2 = Math.max(2 * Line.ON_LINE_THRESH, stroke);
			int tol = tol2 / 2;
			return isInRect(qx, qy, x - tol, y - tol, w + tol2, h + tol2)
					&& contains(x - tol, y - tol, w + tol2, h + tol2, loc)
					&& !contains(x + tol, y + tol, w - tol2, h - tol2, loc);
		}
		if (type == DrawAttr.PAINT_STROKE_FILL) {
			int tol2 = getStrokeWidth();
			int tol = tol2 / 2;
			return isInRect(qx, qy, x - tol, y - tol, w + tol2, h + tol2)
					&& contains(x - tol, y - tol, w + tol2, h + tol2, loc);
		}
		return false;
	}

	boolean isInRect(int qx, int qy, int x0, int y0, int w, int h) {
		return qx >= x0 && qx < x0 + w && qy >= y0 && qy < y0 + h;
	}

	protected abstract boolean contains(int x, int y, int w, int h, Location q);

	protected abstract void draw(Graphics g, int x, int y, int w, int h);
}
