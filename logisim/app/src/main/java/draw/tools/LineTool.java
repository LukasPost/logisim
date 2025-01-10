/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import java.awt.Color;
import java.awt.Cursor;
import java.awt.Graphics;
import java.awt.event.InputEvent;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import java.util.List;

import javax.swing.Icon;

import draw.actions.ModelAddAction;
import draw.canvas.Canvas;
import draw.model.CanvasModel;
import draw.model.CanvasObject;
import draw.shapes.DrawAttr;
import draw.shapes.LineUtil;
import draw.shapes.Poly;
import logisim.data.Attribute;
import logisim.data.Location;
import logisim.util.Icons;
import logisim.util.UnmodifiableList;

public class LineTool extends AbstractTool {
	private DrawingAttributeSet attrs;
	private boolean active;
	private Location mouseStart;
	private Location mouseEnd;
	private int lastMouseX;
	private int lastMouseY;

	public LineTool(DrawingAttributeSet attrs) {
		this.attrs = attrs;
		active = false;
	}

	@Override
	public Icon getIcon() {
		return Icons.getIcon("drawline.gif");
	}

	@Override
	public Cursor getCursor(Canvas canvas) {
		return Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.ATTRS_STROKE;
	}

	@Override
	public void toolDeselected(Canvas canvas) {
		active = false;
		repaintArea(canvas);
	}

	@Override
	public void mousePressed(Canvas canvas, MouseEvent e) {
		Location loc = new Location(e.getX(), e.getY());
		int mods = e.getModifiersEx();
		if ((mods & InputEvent.CTRL_DOWN_MASK) != 0)
			loc = canvas.snapXY(loc);

		mouseStart = loc;
		mouseEnd = loc;
		lastMouseX = loc.x();
		lastMouseY = loc.y();
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
		Location start = mouseStart;
		Location end = mouseEnd;
		CanvasObject add = null;
		if (!start.equals(end)) {
			active = false;
			CanvasModel model = canvas.getModel();
			Location[] ends = { start, end };
			List<Location> locs = UnmodifiableList.create(ends);
			add = attrs.applyTo(new Poly(false, locs));
			add.setValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_STROKE);
			canvas.doAction(new ModelAddAction(model, add));
			repaintArea(canvas);
		}
		canvas.toolGestureComplete(this, add);
	}

	@Override
	public void keyPressed(Canvas canvas, KeyEvent e) {
		int code = e.getKeyCode();
		if (active && (code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_CONTROL))
			updateMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
	}

	@Override
	public void keyReleased(Canvas canvas, KeyEvent e) {
		keyPressed(canvas, e);
	}

	private void updateMouse(Canvas canvas, int mx, int my, int mods) {
		if (active) {
			Location newEnd = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0
					? LineUtil.snapTo8Cardinals(mouseStart, mx, my)
					: new Location(mx, my);

			if ((mods & InputEvent.CTRL_DOWN_MASK) != 0)
				newEnd = canvas.snapXY(newEnd);

			if (!newEnd.equals(mouseEnd)) {
				mouseEnd = newEnd;
				repaintArea(canvas);
			}
		}
		lastMouseX = mx;
		lastMouseY = my;
	}

	private void repaintArea(Canvas canvas) {
		canvas.repaint();
	}

	@Override
	public void draw(Canvas canvas, Graphics g) {
		if (!active)
			return;
		Location start = mouseStart;
		Location end = mouseEnd;
		g.setColor(Color.GRAY);
		g.drawLine(start.x(), start.y(), end.x(), end.y());
	}

	static Location snapTo4Cardinals(Location from, int mx, int my) {
		int px = from.x();
		int py = from.y();
		if (mx != px && my != py)
			if (Math.abs(my - py) < Math.abs(mx - px))
				return new Location(mx, py);
			else
				return new Location(px, my);
		return new Location(mx, my); // should never happen
	}
}
