/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.canvas;

import java.awt.Cursor;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.awt.event.MouseMotionListener;
import java.util.List;

import draw.model.CanvasModelEvent;
import draw.model.CanvasModelListener;
import draw.model.CanvasObject;
import logisim.data.Location;

class CanvasListener implements MouseListener, MouseMotionListener, KeyListener, CanvasModelListener {
	private Canvas canvas;
	private CanvasTool tool;

	public CanvasListener(Canvas canvas) {
		this.canvas = canvas;
		tool = null;
	}

	public CanvasTool getTool() {
		return tool;
	}

	public void setTool(CanvasTool value) {
		CanvasTool oldValue = tool;
		if (value != oldValue) {
			tool = value;
			if (oldValue != null)
				oldValue.toolDeselected(canvas);
			if (value != null) {
				value.toolSelected(canvas);
				canvas.setCursor(value.getCursor(canvas));
			} else
				canvas.setCursor(Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR));
		}
	}

	public void mouseMoved(MouseEvent e) {
		if (tool != null)
			tool.mouseMoved(canvas, e);
	}

	public void mousePressed(MouseEvent e) {
		canvas.requestFocus();
		if (e.isPopupTrigger())
			handlePopupTrigger(e);
		else if (e.getButton() == 1 && tool != null)
			tool.mousePressed(canvas, e);
	}

	public void mouseDragged(MouseEvent e) {
		if(tool == null)
			return;
		if (isButton1(e))
			tool.mouseDragged(canvas, e);
		else
			tool.mouseMoved(canvas, e);
	}

	public void mouseReleased(MouseEvent e) {
		if (e.isPopupTrigger()) {
			if (tool != null)
				tool.cancelMousePress(canvas);
			handlePopupTrigger(e);
		} else if (e.getButton() == 1 && tool != null)
			tool.mouseReleased(canvas, e);
	}

	public void mouseClicked(MouseEvent e) {
	}

	public void mouseEntered(MouseEvent e) {
		if (tool != null)
			tool.mouseEntered(canvas, e);
	}

	public void mouseExited(MouseEvent e) {
		if (tool != null)
			tool.mouseExited(canvas, e);
	}

	public void keyPressed(KeyEvent e) {
		if (tool != null)
			tool.keyPressed(canvas, e);
	}

	public void keyReleased(KeyEvent e) {
		if (tool != null)
			tool.keyReleased(canvas, e);
	}

	public void keyTyped(KeyEvent e) {
		if (tool != null)
			tool.keyTyped(canvas, e);
	}

	public void modelChanged(CanvasModelEvent event) {
		canvas.getSelection().modelChanged(event);
		canvas.repaint();
	}

	private boolean isButton1(MouseEvent e) {
		return (e.getModifiersEx() & MouseEvent.BUTTON1_DOWN_MASK) != 0;
	}

	private void handlePopupTrigger(MouseEvent e) {
		Location loc = new Location(e.getX(), e.getY());
		List<CanvasObject> objects = canvas.getModel().getObjectsFromTop();
		CanvasObject clicked = objects.stream()
				.filter(o -> o.contains(loc, false))
				.findFirst()
				.orElse(objects.stream()
						.filter(o -> o.contains(loc, true))
						.findFirst()
						.orElse(null));
		canvas.showPopupMenu(e, clicked);
	}
}
