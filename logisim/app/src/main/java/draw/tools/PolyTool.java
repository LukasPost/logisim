/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import java.awt.Color;
import java.awt.Cursor;
import java.awt.Graphics;
import java.awt.event.InputEvent;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import java.util.ArrayList;
import java.util.List;

import javax.swing.Icon;

import draw.actions.ModelAddAction;
import draw.canvas.Canvas;
import draw.model.CanvasObject;
import draw.shapes.DrawAttr;
import draw.shapes.LineUtil;
import draw.shapes.Poly;
import logisim.data.Attribute;
import logisim.data.Location;
import logisim.util.Icons;

public class PolyTool extends AbstractTool {
	// how close we need to be to the start point to count as "closing the loop"
	private static final int CLOSE_TOLERANCE = 2;

	private boolean closed; // whether we are drawing polygons or polylines
	private DrawingAttributeSet attrs;
	private boolean active;
	private ArrayList<Location> locations;
	private boolean mouseDown;
	private int lastMouseX;
	private int lastMouseY;

	public PolyTool(boolean closed, DrawingAttributeSet attrs) {
		this.closed = closed;
		this.attrs = attrs;
		active = false;
		locations = new ArrayList<>();
	}

	@Override
	public Icon getIcon() {
		return Icons.getIcon(closed ? "drawpoly.gif" : "drawplin.gif");
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.getFillAttributes(attrs.getValue(DrawAttr.PAINT_TYPE));
	}

	@Override
	public Cursor getCursor(Canvas canvas) {
		return Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
	}

	@Override
	public void toolDeselected(Canvas canvas) {
		CanvasObject add = commit(canvas);
		canvas.toolGestureComplete(this, add);
		repaintArea(canvas);
	}

	@Override
	public void mousePressed(Canvas canvas, MouseEvent e) {
		lastMouseX = e.getX();
		lastMouseY = e.getY();
		int mods = e.getModifiersEx();
		if ((mods & InputEvent.CTRL_DOWN_MASK) != 0) {
			lastMouseX = canvas.snapX(lastMouseX);
			lastMouseY = canvas.snapY(lastMouseY);
		}

		if (active && e.getClickCount() > 1) {
			CanvasObject add = commit(canvas);
			canvas.toolGestureComplete(this, add);
			return;
		}

		Location loc = new Location(lastMouseX, lastMouseY);
		ArrayList<Location> locs = locations;
		if (!active) {
			locs.clear();
			locs.add(loc);
		}
		locs.add(loc);

		mouseDown = true;
		active = canvas.getModel() != null;
		repaintArea(canvas);
	}

	@Override
	public void mouseDragged(Canvas canvas, MouseEvent e) {
		updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
	}

	@Override
	public void mouseReleased(Canvas canvas, MouseEvent e) {
		if (!active)
			return;
		updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
		mouseDown = false;
		int size = locations.size();
		if (size < 3)
			return;

		if (locations.getFirst().manhattanDistanceTo(locations.getLast()) <= CLOSE_TOLERANCE) {
			locations.remove(size - 1);
			CanvasObject add = commit(canvas);
			canvas.toolGestureComplete(this, add);
		}
	}

	@Override
	public void keyPressed(Canvas canvas, KeyEvent e) {
		int code = e.getKeyCode();
		if (active && mouseDown && (code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_CONTROL))
			updateMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
	}

	@Override
	public void keyReleased(Canvas canvas, KeyEvent e) {
		keyPressed(canvas, e);
	}

	@Override
	public void keyTyped(Canvas canvas, KeyEvent e) {
		if (!active)
			return;

		char ch = e.getKeyChar();
		if (ch == '\u001b') { // escape key
			active = false;
			locations.clear();
			repaintArea(canvas);
			canvas.toolGestureComplete(this, null);
		} else if (ch == '\n') { // enter key
			CanvasObject add = commit(canvas);
			canvas.toolGestureComplete(this, add);
		}
	}

	private CanvasObject commit(Canvas canvas) {
		if (!active)
			return null;
		CanvasObject add = null;
		active = false;
		ArrayList<Location> locs = locations;
		for (int i = locs.size() - 2; i >= 0; i--)
			if (locs.get(i).equals(locs.get(i + 1)))
				locs.remove(i);
		if (locs.size() > 1) {
			add = new Poly(closed, locs);
			canvas.doAction(new ModelAddAction(canvas.getModel(), add));
			repaintArea(canvas);
		}
		locs.clear();
		return add;
	}

	private void updateMouse(Canvas canvas, int mx, int my, int mods) {
		lastMouseX = mx;
		lastMouseY = my;
		if (!active)
			return;

		int index = locations.size() - 1;
		Location last = locations.getLast();
		Location newLast = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0 && index > 0
				? LineUtil.snapTo8Cardinals(last, mx, my) : new Location(mx, my);
		if ((mods & MouseEvent.CTRL_DOWN_MASK) != 0)
			newLast=canvas.snapXY(newLast);

		if (!newLast.equals(last)) {
			locations.set(index, newLast);
			repaintArea(canvas);
		}
	}

	private void repaintArea(Canvas canvas) {
		canvas.repaint();
	}

	@Override
	public void draw(Canvas canvas, Graphics g) {
		if (!active)
			return;

		g.setColor(Color.GRAY);
		int size = locations.size();
		int[] xs = new int[size];
		int[] ys = new int[size];
		for (int i = 0; i < size; i++) {
			Location loc = locations.get(i);
			xs[i] = loc.x();
			ys[i] = loc.y();
		}
		g.drawPolyline(xs, ys, size);
		int lastX = xs[xs.length - 1];
		int lastY = ys[ys.length - 1];
		g.fillOval(lastX - 2, lastY - 2, 4, 4);
	}
}
