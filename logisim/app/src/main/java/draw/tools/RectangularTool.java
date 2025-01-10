/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import java.awt.Color;
import java.awt.Cursor;
import java.awt.Graphics;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;

import draw.actions.ModelAddAction;
import draw.canvas.Canvas;
import draw.model.CanvasObject;
import logisim.data.Bounds;
import logisim.data.Location;

abstract class RectangularTool extends AbstractTool {
	private boolean active;
	private Location dragStart;
	private int lastMouseX;
	private int lastMouseY;
	private Bounds currentBounds;

	public RectangularTool() {
		active = false;
		currentBounds = Bounds.EMPTY_BOUNDS;
	}

	public abstract CanvasObject createShape(int x, int y, int w, int h);

	public abstract void drawShape(Graphics g, int x, int y, int w, int h);

	public abstract void fillShape(Graphics g, int x, int y, int w, int h);

	@Override
	public Cursor getCursor(Canvas canvas) {
		return Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
	}

	@Override
	public void toolDeselected(Canvas canvas) {
		Bounds bds = currentBounds;
		active = false;
		repaintArea(canvas, bds);
	}

	@Override
	public void mousePressed(Canvas canvas, MouseEvent e) {
		Location loc = new Location(e.getX(), e.getY());
		dragStart = loc;
		lastMouseX = loc.x();
		lastMouseY = loc.y();
		active = canvas.getModel() != null;
		repaintArea(canvas, Bounds.create(loc));
	}

	@Override
	public void mouseDragged(Canvas canvas, MouseEvent e) {
		updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
	}

	@Override
	public void mouseReleased(Canvas canvas, MouseEvent e) {
		if (!active)
			return;

		Bounds oldBounds = currentBounds;
		Bounds bds = computeBounds(canvas, e.getX(), e.getY(), e.getModifiersEx());
		currentBounds = Bounds.EMPTY_BOUNDS;
		active = false;
		CanvasObject add = null;
		if (bds.getWidth() != 0 && bds.getHeight() != 0) {
			add = createShape(bds.getX(), bds.getY(), bds.getWidth(), bds.getHeight());
			canvas.doAction(new ModelAddAction(canvas.getModel(), add));
			repaintArea(canvas, oldBounds.add(bds));
		}
		canvas.toolGestureComplete(this, add);
	}

	@Override
	public void keyPressed(Canvas canvas, KeyEvent e) {
		int code = e.getKeyCode();
		if (active && (code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_ALT || code == KeyEvent.VK_CONTROL))
			updateMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
	}

	@Override
	public void keyReleased(Canvas canvas, KeyEvent e) {
		keyPressed(canvas, e);
	}

	private void updateMouse(Canvas canvas, int mx, int my, int mods) {
		Bounds oldBounds = currentBounds;
		Bounds bds = computeBounds(canvas, mx, my, mods);
		if (!bds.equals(oldBounds)) {
			currentBounds = bds;
			repaintArea(canvas, oldBounds.add(bds));
		}
	}

	private Bounds computeBounds(Canvas canvas, int mx, int my, int mods) {
		lastMouseX = mx;
		lastMouseY = my;
		Location mLoc = new Location(mx, my);
		if (!active)
			return Bounds.EMPTY_BOUNDS;

		Location start = dragStart;
		if (mLoc.equals(start))
			return Bounds.EMPTY_BOUNDS;

		boolean ctrlDown = (mods & MouseEvent.CTRL_DOWN_MASK) != 0;
		if (ctrlDown) {
			start = canvas.snapXY(start);
			mLoc = canvas.snapXY(mLoc);
		}

		boolean altDown = (mods & MouseEvent.ALT_DOWN_MASK) != 0;
		boolean shiftDown = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0;
		if (altDown) {
			if (shiftDown) {
				Location diff = start.sub(mLoc).abs();
				int r = Math.min(diff.x(), diff.y());
				mLoc = mLoc.add(r, r);
				start = start.sub(r, r);
			}
			else
				start = start.add(mLoc.sub(start));
		}
		else if (shiftDown) {
			Location diff = start.sub(mLoc).abs();
			int r = Math.min(diff.x(), diff.y());
			int rx = mLoc.x() < start.x() ? -r : r;
			int ry = mLoc.y() < start.y() ? -r : r;
			mLoc = start.add(rx, ry);
		}


		Location loc = new Location(Math.max(start.x(), mLoc.x()), Math.max(start.y(), mLoc.y()));
		Location size = mLoc.sub(start).abs();
		return Bounds.create(loc, size.x(), size.y());
	}

	private void repaintArea(Canvas canvas, Bounds bds) {
		canvas.repaint();
		/*
		 * The below doesn't work because Java doesn't deal correctly with stroke widths that go outside the clip area
		 * canvas.repaintCanvasCoords(bds.getX() - 10, bds.getY() - 10, bds.getWidth() + 20, bds.getHeight() + 20);
		 */
	}

	@Override
	public void draw(Canvas canvas, Graphics g) {
		Bounds bds = currentBounds;
		if (active && bds != null && bds != Bounds.EMPTY_BOUNDS) {
			g.setColor(Color.GRAY);
			drawShape(g, bds.getX(), bds.getY(), bds.getWidth(), bds.getHeight());
		}
	}

}
