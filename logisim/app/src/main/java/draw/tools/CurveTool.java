/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import java.awt.Color;
import java.awt.Cursor;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.event.InputEvent;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import java.util.List;

import javax.swing.Icon;

import draw.actions.ModelAddAction;
import draw.canvas.Canvas;
import draw.model.CanvasModel;
import draw.shapes.Curve;
import draw.shapes.CurveUtil;
import draw.shapes.DrawAttr;
import draw.shapes.LineUtil;
import logisim.data.Attribute;
import logisim.data.Location;
import logisim.util.Icons;

public class CurveTool extends AbstractTool {
	private static final int BEFORE_CREATION = 0;
	private static final int ENDPOINT_DRAG = 1;
	private static final int CONTROL_DRAG = 2;

	private DrawingAttributeSet attrs;
	private int state;
	private Location end0;
	private Location end1;
	private Curve curCurve;
	private boolean mouseDown;
	private int lastMouseX;
	private int lastMouseY;

	public CurveTool(DrawingAttributeSet attrs) {
		this.attrs = attrs;
		state = BEFORE_CREATION;
		mouseDown = false;
	}

	@Override
	public Icon getIcon() {
		return Icons.getIcon("drawcurv.gif");
	}

	@Override
	public Cursor getCursor(Canvas canvas) {
		return Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
	}

	@Override
	public void toolDeselected(Canvas canvas) {
		state = BEFORE_CREATION;
		repaintArea(canvas);
	}

	@Override
	public void mousePressed(Canvas canvas, MouseEvent e) {
		int mx = e.getX();
		int my = e.getY();
		lastMouseX = mx;
		lastMouseY = my;
		mouseDown = true;
		int mods = e.getModifiersEx();
		if ((mods & InputEvent.CTRL_DOWN_MASK) != 0) {
			mx = canvas.snapX(mx);
			my = canvas.snapY(my);
		}

		switch (state) {
		case BEFORE_CREATION:
		case CONTROL_DRAG:
			end0 = new Location(mx, my);
			end1 = end0;
			state = ENDPOINT_DRAG;
			break;
		case ENDPOINT_DRAG:
			curCurve = new Curve(end0, end1, new Location(mx, my));
			state = CONTROL_DRAG;
			break;
		}
		repaintArea(canvas);
	}

	@Override
	public void mouseDragged(Canvas canvas, MouseEvent e) {
		updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
		repaintArea(canvas);
	}

	@Override
	public void mouseReleased(Canvas canvas, MouseEvent e) {
		Curve c = updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
		mouseDown = false;
		if (state == CONTROL_DRAG) {
			if (c != null) {
				attrs.applyTo(c);
				CanvasModel model = canvas.getModel();
				canvas.doAction(new ModelAddAction(model, c));
				canvas.toolGestureComplete(this, c);
			}
			state = BEFORE_CREATION;
		}
		repaintArea(canvas);
	}

	@Override
	public void keyPressed(Canvas canvas, KeyEvent e) {
		int code = e.getKeyCode();
		if (mouseDown && (code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_CONTROL || code == KeyEvent.VK_ALT)) {
			updateMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
			repaintArea(canvas);
		}
	}

	@Override
	public void keyReleased(Canvas canvas, KeyEvent e) {
		keyPressed(canvas, e);
	}

	@Override
	public void keyTyped(Canvas canvas, KeyEvent e) {
		char ch = e.getKeyChar();
		if (ch == '\u001b') { // escape key
			state = BEFORE_CREATION;
			repaintArea(canvas);
			canvas.toolGestureComplete(this, null);
		}
	}

	private Curve updateMouse(Canvas canvas, int mx, int my, int mods) {
		lastMouseX = mx;
		lastMouseY = my;

		boolean shiftDown = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0;
		boolean ctrlDown = (mods & MouseEvent.CTRL_DOWN_MASK) != 0;
		boolean altDown = (mods & MouseEvent.ALT_DOWN_MASK) != 0;
		Curve ret = null;
		return switch (state) {
			case ENDPOINT_DRAG -> {
				if (mouseDown) {
					if (shiftDown) {
						Location p = LineUtil.snapTo8Cardinals(end0, mx, my);
						mx = p.x();
						my = p.y();
					}
					if (ctrlDown) {
						mx = canvas.snapX(mx);
						my = canvas.snapY(my);
					}
					end1 = new Location(mx, my);
				}
				yield null;
			}
			case CONTROL_DRAG -> {
				if (mouseDown) {
					int cx = mx;
					int cy = my;
					if (ctrlDown) {
						cx = canvas.snapX(cx);
						cy = canvas.snapY(cy);
					}
					if (shiftDown) {
						double midx = (double) (end0.x() + end1.x()) / 2;
						double midy = (double) (end0.y() + end1.y()) / 2;
						double dx = end1.x() - end0.x();
						double dy = end1.y() - end0.y();
						double[] p = LineUtil.nearestPointInfinite(cx, cy, midx, midy, midx - dy, midy + dx);
						cx = (int) Math.round(p[0]);
						cy = (int) Math.round(p[1]);
					}
					if (altDown) {
						double[] e0 = Location.toArray(end0);
						double[] e1 = Location.toArray(end1);
						double[] mid = {cx, cy};
						double[] ct = CurveUtil.interpolate(e0, e1, mid);
						cx = (int) Math.round(ct[0]);
						cy = (int) Math.round(ct[1]);
					}
					ret = new Curve(end0, end1, new Location(cx, cy));
					curCurve = ret;
				}
				yield ret;
			}
			default -> null;
		};
	}

	private void repaintArea(Canvas canvas) {
		canvas.repaint();
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.getFillAttributes(attrs.getValue(DrawAttr.PAINT_TYPE));
	}

	@Override
	public void draw(Canvas canvas, Graphics g) {
		g.setColor(Color.GRAY);
		if (state == ENDPOINT_DRAG)
			g.drawLine(end0.x(), end0.y(), end1.x(), end1.y());
		else if (state == CONTROL_DRAG)
			((Graphics2D) g).draw(curCurve.getCurve2D());
	}
}
