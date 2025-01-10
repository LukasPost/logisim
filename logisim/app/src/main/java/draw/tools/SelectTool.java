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
import java.util.*;

import javax.swing.Icon;

import draw.actions.ModelMoveHandleAction;
import draw.actions.ModelRemoveAction;
import draw.actions.ModelTranslateAction;
import draw.canvas.Canvas;
import draw.canvas.Selection;
import draw.model.CanvasModel;
import draw.model.CanvasObject;
import draw.model.Handle;
import draw.model.HandleGesture;
import logisim.data.Attribute;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.GraphicsUtil;
import logisim.util.Icons;

public class SelectTool extends AbstractTool {
	private static final int IDLE = 0;
	private static final int MOVE_ALL = 1;
	private static final int RECT_SELECT = 2;
	private static final int RECT_TOGGLE = 3;
	private static final int MOVE_HANDLE = 4;

	private static final int DRAG_TOLERANCE = 2;
	private static final int HANDLE_SIZE = 8;

	private static final Color RECT_SELECT_BACKGROUND = new Color(0, 0, 0, 32);

	private int curAction;
	private List<CanvasObject> beforePressSelection;
	private Handle beforePressHandle;
	private Location dragStart;
	private Location dragEnd;
	private boolean dragEffective;
	private int lastMouseX;
	private int lastMouseY;
	private HandleGesture curGesture;

	public SelectTool() {
		curAction = IDLE;
		dragStart = new Location(0, 0);
		dragEnd = dragStart;
		dragEffective = false;
	}

	@Override
	public Icon getIcon() {
		return Icons.getIcon("select.gif");
	}

	@Override
	public Cursor getCursor(Canvas canvas) {
		return Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR);
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return Collections.emptyList();
	}

	@Override
	public void toolSelected(Canvas canvas) {
		curAction = IDLE;
		canvas.getSelection().clearSelected();
		repaintArea(canvas);
	}

	@Override
	public void toolDeselected(Canvas canvas) {
		curAction = IDLE;
		canvas.getSelection().clearSelected();
		repaintArea(canvas);
	}

	private int getHandleSize(Canvas canvas) {
		double zoom = canvas.getZoomFactor();
		return (int) Math.ceil(HANDLE_SIZE / Math.sqrt(zoom));
	}

	@Override
	public void mousePressed(Canvas canvas, MouseEvent e) {
		beforePressSelection = new ArrayList<>(canvas.getSelection().getSelected());
		beforePressHandle = canvas.getSelection().getSelectedHandle();
		int mx = e.getX();
		int my = e.getY();
		boolean shift = (e.getModifiersEx() & MouseEvent.SHIFT_DOWN_MASK) != 0;
		dragStart = new Location(mx, my);
		dragEffective = false;
		dragEnd = dragStart;
		lastMouseX = mx;
		lastMouseY = my;
		Selection selection = canvas.getSelection();
		selection.setHandleSelected(null);

		// see whether user is pressing within an existing handle
		int halfSize = getHandleSize(canvas) / 2;
		CanvasObject clicked = null;
		for (CanvasObject shape : selection.getSelected())
			for (Handle han : shape.getHandles(null)) {
				Location diff = han.getLocation().sub(dragStart).abs();
				if (diff.x() > halfSize || diff.y() > halfSize)
					continue;
				if (shape.canMoveHandle(han)) {
					curAction = MOVE_HANDLE;
					curGesture = new HandleGesture(han, diff, e.getModifiersEx());
					repaintArea(canvas);
					return;
				}
				else if (clicked == null)
					clicked = shape;
			}

		// see whether the user is clicking within a shape
		if (clicked == null)
			clicked = getObjectAt(canvas.getModel(), new Location(e.getX(), e.getY()), false);
		if (clicked != null) {
			if (shift && selection.isSelected(clicked)) {
				selection.setSelected(clicked, false);
				curAction = IDLE;
			} else {
				if (!shift && !selection.isSelected(clicked))
					selection.clearSelected();
				selection.setSelected(clicked, true);
				selection.setMovingShapes(selection.getSelected(), 0, 0);
				curAction = MOVE_ALL;
			}
			repaintArea(canvas);
			return;
		}

		clicked = getObjectAt(canvas.getModel(), new Location(e.getX(), e.getY()), true);
		if (clicked != null && selection.isSelected(clicked)) {
			if (shift) {
				selection.setSelected(clicked, false);
				curAction = IDLE;
			} else {
				selection.setMovingShapes(selection.getSelected(), 0, 0);
				curAction = MOVE_ALL;
			}
			repaintArea(canvas);
			return;
		}

		if (shift)
			curAction = RECT_TOGGLE;
		else {
			selection.clearSelected();
			curAction = RECT_SELECT;
		}
		repaintArea(canvas);
	}

	@Override
	public void cancelMousePress(Canvas canvas) {
		List<CanvasObject> before = beforePressSelection;
		Handle handle = beforePressHandle;
		beforePressSelection = null;
		beforePressHandle = null;
		if (before != null) {
			curAction = IDLE;
			Selection sel = canvas.getSelection();
			sel.clearDrawsSuppressed();
			sel.setMovingShapes(Collections.emptySet(), 0, 0);
			sel.clearSelected();
			sel.setSelected(before, true);
			sel.setHandleSelected(handle);
			repaintArea(canvas);
		}
	}

	@Override
	public void mouseDragged(Canvas canvas, MouseEvent e) {
		setMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
	}

	@Override
	public void mouseReleased(Canvas canvas, MouseEvent e) {
		beforePressSelection = null;
		beforePressHandle = null;
		setMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());

		CanvasModel model = canvas.getModel();
		Selection selection = canvas.getSelection();
		Set<CanvasObject> selected = selection.getSelected();
		int action = curAction;
		curAction = IDLE;

		if (!dragEffective) {
			Location loc = dragEnd;
			CanvasObject o = getObjectAt(model, loc, false);
			if (o != null) {
				Handle han = o.canDeleteHandle(loc);
				if (han != null)
					selection.setHandleSelected(han);
				else {
					han = o.canInsertHandle(loc);
					if (han != null)
						selection.setHandleSelected(han);
				}
			}
		}

		Location start = dragStart;
		Location mloc = new Location(e.getX(), e.getY());
		switch (action) {
		case MOVE_ALL:
			Location moveDelta = selection.getMovingDelta();
			if (dragEffective && !moveDelta.isZero())
				canvas.doAction(new ModelTranslateAction(model, selected, moveDelta));
			break;
		case MOVE_HANDLE:
			HandleGesture gesture = curGesture;
			curGesture = null;
			if (dragEffective && gesture != null) {
				ModelMoveHandleAction act = new ModelMoveHandleAction(model, gesture);
				canvas.doAction(act);
				Handle result = act.getNewHandle();
				if (result != null) {
					Handle h = result.getObject().canDeleteHandle(result.getLocation());
					selection.setHandleSelected(h);
				}
			}
			break;
		case RECT_SELECT:
			if (dragEffective) {
				Bounds bds = Bounds.create(start).add(mloc);
				selection.setSelected(canvas.getModel().getObjectsIn(bds), true);
			} else {
				CanvasObject clicked = getObjectAt(model, start, true);
				if (clicked != null) {
					selection.clearSelected();
					selection.setSelected(clicked, true);
				}
			}
			break;
		case RECT_TOGGLE:
			if (dragEffective) {
				Bounds bds = Bounds.create(start).add(mloc);
				selection.toggleSelected(canvas.getModel().getObjectsIn(bds));
			} else {
				CanvasObject clicked = getObjectAt(model, start, true);
				selection.setSelected(clicked, !selected.contains(clicked));
			}
			break;
		}
		selection.clearDrawsSuppressed();
		repaintArea(canvas);
	}

	@Override
	public void keyPressed(Canvas canvas, KeyEvent e) {
		int code = e.getKeyCode();
		if ((code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_CONTROL || code == KeyEvent.VK_ALT)
				&& curAction != IDLE) setMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
	}

	@Override
	public void keyReleased(Canvas canvas, KeyEvent e) {
		keyPressed(canvas, e);
	}

	@Override
	public void keyTyped(Canvas canvas, KeyEvent e) {
		char ch = e.getKeyChar();
		Selection selected = canvas.getSelection();
		if ((ch == '\u0008' || ch == '\u007F') && !selected.isEmpty()) {
			List<CanvasObject> toRemove = selected.getSelected().stream()
					.filter(CanvasObject::canRemove).toList();
			if (!toRemove.isEmpty()) {
				e.consume();
				canvas.doAction(new ModelRemoveAction(canvas.getModel(), toRemove));
				selected.clearSelected();
				repaintArea(canvas);
			}
		} else if (ch == '\u001b' && !selected.isEmpty()) {
			selected.clearSelected();
			repaintArea(canvas);
		}
	}

	private void setMouse(Canvas canvas, int mx, int my, int mods) {
		lastMouseX = mx;
		lastMouseY = my;
		boolean shift = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0;
		boolean ctrl = (mods & InputEvent.CTRL_DOWN_MASK) != 0;
		Location newEnd = new Location(mx, my);
		dragEnd = newEnd;

		Location start = dragStart;
		Location diff = newEnd.sub(start);
		int dx = newEnd.x() - start.x();
		int dy = newEnd.y() - start.y();
		if (!dragEffective)
			if (diff.abs().sum() > DRAG_TOLERANCE)
				dragEffective = true;
			else return;

		switch (curAction) {
		case MOVE_HANDLE:
			HandleGesture gesture = curGesture;
			if (ctrl) {
				Handle h = gesture.getHandle();
				diff = canvas.snapXY(h.getLocation().add(diff)).sub(h.getLocation());
			}
			curGesture = new HandleGesture(gesture.getHandle(), diff, mods);
			canvas.getSelection().setHandleGesture(curGesture);
			break;
		case MOVE_ALL:
			if (ctrl) {
				int minX = Integer.MAX_VALUE;
				int minY = Integer.MAX_VALUE;
				for (CanvasObject o : canvas.getSelection().getSelected()) {
					var handles = o.getHandles(null);
					int loaclemin = handles.stream().mapToInt(Handle::getX).min().orElse(Integer.MAX_VALUE);
					minX = Math.min(minX, loaclemin);
					loaclemin = handles.stream().mapToInt(Handle::getY).min().orElse(Integer.MAX_VALUE);
					minY = Math.min(minY, loaclemin);
				}
				Location min = new Location(minX, minY);
				diff = canvas.snapXY(min.add(diff)).sub(min);
			}
			if (shift) if (Math.abs(diff.x()) > Math.abs(diff.y()))
				diff = new Location(diff.x(), 0);
			else
				diff = new Location(0, diff.y());
			canvas.getSelection().setMovingDelta(diff.x(), diff.y());
			break;
		}
		repaintArea(canvas);
	}

	private void repaintArea(Canvas canvas) {
		canvas.repaint();
	}

	@Override
	public void draw(Canvas canvas, Graphics g) {
		Selection selection = canvas.getSelection();
		int action = curAction;

		Location start = dragStart;
		Location end = dragEnd;
		boolean drawHandles = switch (action) {
			case MOVE_ALL, MOVE_HANDLE -> !dragEffective;
			default -> true;
		};

		if (drawHandles) {
			// unscale the coordinate system so that the stroke width isn't scaled
			double zoom = 1.0;
			Graphics gCopy = g.create();
			if (gCopy instanceof Graphics2D) {
				zoom = canvas.getZoomFactor();
				if (zoom != 1.0)
					((Graphics2D) gCopy).scale(1.0 / zoom, 1.0 / zoom);
			}
			GraphicsUtil.switchToWidth(gCopy, 1);

			int size = (int) Math.ceil(HANDLE_SIZE * Math.sqrt(zoom));
			int offs = size / 2;
			for (CanvasObject obj : selection.getSelected()) {
				List<Handle> handles = obj.getHandles(null);
				for (Handle han : handles) {
					Location loc = han.getLocation();
					if (action == MOVE_ALL && dragEffective)
						loc = loc.add(selection.getMovingDelta());
					loc = loc.mul(zoom).sub(offs, offs);
					gCopy.clearRect(loc.x(), loc.y(), size, size);
					gCopy.drawRect(loc.x(), loc.y(), size, size);
				}
			}
			Handle selHandle = selection.getSelectedHandle();
			if (selHandle != null) {
				Location loc = selHandle.getLocation();
				if (action == MOVE_ALL && dragEffective)
					loc = loc.add(selection.getMovingDelta());
				loc = loc.mul(zoom);
				int[] xs = { loc.x() - offs, loc.x(), loc.x() + offs, loc.x() };
				int[] ys = { loc.y(), loc.y() - offs, loc.y(), loc.y() + offs };
				gCopy.setColor(Color.WHITE);
				gCopy.fillPolygon(xs, ys, 4);
				gCopy.setColor(Color.BLACK);
				gCopy.drawPolygon(xs, ys, 4);
			}
		}

		switch (action) {
		case RECT_SELECT:
		case RECT_TOGGLE:
			if (dragEffective) {
				// find rectangle currently to show
				int x0 = start.x();
				int y0 = start.y();
				int x1 = end.x();
				int y1 = end.y();
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

				// make the region that's not being selected darker
				int w = canvas.getWidth();
				int h = canvas.getHeight();
				g.setColor(RECT_SELECT_BACKGROUND);
				g.fillRect(0, 0, w, y0);
				g.fillRect(0, y0, x0, y1 - y0);
				g.fillRect(x1, y0, w - x1, y1 - y0);
				g.fillRect(0, y1, w, h - y1);

				// now draw the rectangle
				g.setColor(Color.GRAY);
				g.drawRect(x0, y0, x1 - x0, y1 - y0);
			}
			break;
		}
	}

	private static CanvasObject getObjectAt(CanvasModel model, Location loc, boolean assumeFilled) {
		return model.getObjectsFromTop().stream()
				.filter(o -> o.contains(loc, assumeFilled))
				.findFirst()
				.orElse(null);
	}
}
