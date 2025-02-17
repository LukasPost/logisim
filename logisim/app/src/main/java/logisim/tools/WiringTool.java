/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import java.awt.Cursor;
import java.awt.Color;
import java.awt.Graphics;
import java.awt.Rectangle;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;

import javax.swing.Icon;

import logisim.circuit.CircuitMutation;
import logisim.circuit.Wire;
import logisim.comp.Component;
import logisim.comp.ComponentDrawContext;
import logisim.data.Location;
import logisim.gui.main.Canvas;
import logisim.prefs.AppPreferences;
import logisim.proj.Action;
import logisim.util.GraphicsUtil;
import logisim.util.Icons;
import logisim.util.StringGetter;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Set;

public class WiringTool extends Tool {
	private static Cursor cursor = Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
	private static final Icon toolIcon = Icons.getIcon("wiring.gif");

	private static final int HORIZONTAL = 1;
	private static final int VERTICAL = 2;

	private boolean exists;
	private boolean inCanvas;
	private Location start = new Location(0, 0);
	private Location cur = new Location(0, 0);
	private boolean hasDragged;
	private boolean startShortening;
	private Wire shortening;
	private Action lastAction;
	private int direction;

	public WiringTool() {
		super.select(null);
	}

	@Override
	public void select(Canvas canvas) {
		super.select(canvas);
		lastAction = null;
		reset();
	}

	private void reset() {
		exists = false;
		inCanvas = false;
		start = new Location(0, 0);
		cur = new Location(0, 0);
		startShortening = false;
		shortening = null;
		direction = 0;
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof WiringTool;
	}

	@Override
	public int hashCode() {
		return WiringTool.class.hashCode();
	}

	@Override
	public String getName() {
		return "Wiring Tool";
	}

	@Override
	public String getDisplayName() {
		return Strings.get("wiringTool");
	}

	@Override
	public String getDescription() {
		return Strings.get("wiringToolDesc");
	}

	private boolean computeMove(int newX, int newY) {
		if (cur.x() == newX && cur.y() == newY)
			return false;
		Location start = this.start;
		if (direction == 0) {
			if (newX != start.x())
				direction = HORIZONTAL;
			else if (newY != start.y())
				direction = VERTICAL;
		} else if (direction == HORIZONTAL && newX == start.x()) if (newY == start.y())
			direction = 0;
		else
			direction = VERTICAL;
		else if (direction == VERTICAL && newY == start.y()) if (newX == start.x())
			direction = 0;
		else
			direction = HORIZONTAL;
		return true;
	}

	@Override
	public Set<Component> getHiddenComponents(Canvas canvas) {
		Component shorten = willShorten(start, cur);
		if (shorten != null) return Collections.singleton(shorten);
		else return null;
	}

	@Override
	public void draw(Canvas canvas, ComponentDrawContext context) {
		Graphics g = context.getGraphics();
		if (exists) {
			Location e0 = start;
			Location e1 = cur;
			Wire shortenBefore = willShorten(start, cur);
			if (shortenBefore != null) {
				Wire shorten = getShortenResult(shortenBefore, start, cur);
				if (shorten == null) return;
				else {
					e0 = shorten.getEnd0();
					e1 = shorten.getEnd1();
				}
			}
			int x0 = e0.x();
			int y0 = e0.y();
			int x1 = e1.x();
			int y1 = e1.y();

			g.setColor(Color.BLACK);
			GraphicsUtil.switchToWidth(g, 3);
			if (direction == HORIZONTAL) {
				if (x0 != x1)
					g.drawLine(x0, y0, x1, y0);
				if (y0 != y1)
					g.drawLine(x1, y0, x1, y1);
			} else if (direction == VERTICAL) {
				if (y0 != y1)
					g.drawLine(x0, y0, x0, y1);
				if (x0 != x1)
					g.drawLine(x0, y1, x1, y1);
			}
		} else if (AppPreferences.ADD_SHOW_GHOSTS.getBoolean() && inCanvas) {
			g.setColor(Color.GRAY);
			g.fillOval(cur.x() - 2, cur.y() - 2, 5, 5);
		}
	}

	@Override
	public void mouseEntered(Canvas canvas, Graphics g, MouseEvent e) {
		inCanvas = true;
		canvas.getProject().repaintCanvas();
	}

	@Override
	public void mouseExited(Canvas canvas, Graphics g, MouseEvent e) {
		inCanvas = false;
		canvas.getProject().repaintCanvas();
	}

	@Override
	public void mouseMoved(Canvas canvas, Graphics g, MouseEvent e) {
		if (exists) mouseDragged(canvas, g, e);
		else {
			Canvas.snapToGrid(e);
			inCanvas = true;
			int curX = e.getX();
			int curY = e.getY();
			if (cur.x() != curX || cur.y() != curY) cur = new Location(curX, curY);
			canvas.getProject().repaintCanvas();
		}
	}

	@Override
	public void mousePressed(Canvas canvas, Graphics g, MouseEvent e) {
		if (!canvas.getProject().getLogisimFile().contains(canvas.getCircuit())) {
			exists = false;
			canvas.setErrorMessage(Strings.getter("cannotModifyError"));
			return;
		}

		if (exists) mouseDragged(canvas, g, e);
		else {
			Canvas.snapToGrid(e);
			start = new Location(e.getX(), e.getY());
			cur = start;
			exists = true;
			hasDragged = false;

			startShortening = !canvas.getCircuit().getWires(start).isEmpty();
			shortening = null;

			super.mousePressed(canvas, g, e);
			canvas.getProject().repaintCanvas();
		}
	}

	@Override
	public void mouseDragged(Canvas canvas, Graphics g, MouseEvent e) {
		if (exists) {
			Canvas.snapToGrid(e);
			int curX = e.getX();
			int curY = e.getY();
			if (!computeMove(curX, curY))
				return;
			hasDragged = true;

			Rectangle rect = new Rectangle();
			rect.add(start.x(), start.y());
			rect.add(cur.x(), cur.y());
			rect.add(curX, curY);
			rect.grow(3, 3);

			cur = new Location(curX, curY);
			super.mouseDragged(canvas, g, e);

			Wire shorten = null;
			if (startShortening) for (Wire w : canvas.getCircuit().getWires(start))
				if (w.contains(cur)) {
					shorten = w;
					break;
				}
			if (shorten == null) for (Wire w : canvas.getCircuit().getWires(cur))
				if (w.contains(start)) {
					shorten = w;
					break;
				}
			shortening = shorten;

			canvas.repaint(rect);
		}
	}

	void resetClick() {
		exists = false;
	}

	@Override
	public void mouseReleased(Canvas canvas, Graphics g, MouseEvent e) {
		if (!exists)
			return;

		Canvas.snapToGrid(e);
		int curX = e.getX();
		int curY = e.getY();
		if (computeMove(curX, curY)) cur = new Location(curX, curY);
		if (hasDragged) {
			exists = false;
			super.mouseReleased(canvas, g, e);

			ArrayList<Wire> ws = new ArrayList<>(2);
			if (cur.y() == start.y() || cur.x() == start.x()) {
				Wire w = Wire.create(cur, start);
				w = checkForRepairs(canvas, w, w.getEnd0());
				w = checkForRepairs(canvas, w, w.getEnd1());
				if (performShortening(canvas, start, cur)) return;
				if (w.getLength() > 0)
					ws.add(w);
			} else {
				Location m;
				if (direction == HORIZONTAL) m = new Location(cur.x(), start.y());
				else m = new Location(start.x(), cur.y());
				Wire w0 = Wire.create(start, m);
				Wire w1 = Wire.create(m, cur);
				w0 = checkForRepairs(canvas, w0, start);
				w1 = checkForRepairs(canvas, w1, cur);
				if (w0.getLength() > 0)
					ws.add(w0);
				if (w1.getLength() > 0)
					ws.add(w1);
			}
			if (ws.size() > 0) {
				CircuitMutation mutation = new CircuitMutation(canvas.getCircuit());
				mutation.addAll(ws);
				StringGetter desc;
				if (ws.size() == 1)
					desc = Strings.getter("addWireAction");
				else
					desc = Strings.getter("addWiresAction");
				Action act = mutation.toAction(desc);
				canvas.getProject().doAction(act);
				lastAction = act;
			}
		}
	}

	private Wire checkForRepairs(Canvas canvas, Wire w, Location end) {
		if (w.getLength() <= 10)
			return w; // don't repair a short wire to nothing
		if (!canvas.getCircuit().getNonWires(end).isEmpty())
			return w;

		int delta = (end.equals(w.getEnd0()) ? 10 : -10);
		Location cand;
		if (w.isVertical()) cand = new Location(end.x(), end.y() + delta);
		else cand = new Location(end.x() + delta, end.y());

		for (Component comp : canvas.getCircuit().getNonWires(cand))
			if (comp.getBounds().contains(end)) {
				WireRepair repair = (WireRepair) comp.getFeature(WireRepair.class);
				if (repair != null && repair.shouldRepairWire(new WireRepairData(w, cand))) {
					w = Wire.create(w.getOtherEnd(end), cand);
					canvas.repaint(end.x() - 13, end.y() - 13, 26, 26);
					return w;
				}
			}
		return w;
	}

	private Wire willShorten(Location drag0, Location drag1) {
		Wire shorten = shortening;
		if (shorten == null) return null;
		else if (shorten.endsAt(drag0) || shorten.endsAt(drag1)) return shorten;
		else return null;
	}

	private Wire getShortenResult(Wire shorten, Location drag0, Location drag1) {
		if (shorten == null) return null;
		else {
			Location e0;
			Location e1;
			if (shorten.endsAt(drag0)) {
				e0 = drag1;
				e1 = shorten.getOtherEnd(drag0);
			} else if (shorten.endsAt(drag1)) {
				e0 = drag0;
				e1 = shorten.getOtherEnd(drag1);
			} else return null;
			return e0.equals(e1) ? null : Wire.create(e0, e1);
		}
	}

	private boolean performShortening(Canvas canvas, Location drag0, Location drag1) {
		Wire shorten = willShorten(drag0, drag1);
		if (shorten == null) return false;
		else {
			CircuitMutation xn = new CircuitMutation(canvas.getCircuit());
			StringGetter actName;
			Wire result = getShortenResult(shorten, drag0, drag1);
			if (result == null) {
				xn.remove(shorten);
				actName = Strings.getter("removeComponentAction", shorten.getFactory().getDisplayGetter());
			} else {
				xn.replace(shorten, result);
				actName = Strings.getter("shortenWireAction");
			}
			canvas.getProject().doAction(xn.toAction(actName));
			return true;
		}
	}

	@Override
	public void keyPressed(Canvas canvas, KeyEvent event) {
		if (event.getKeyCode() == KeyEvent.VK_BACK_SPACE && lastAction != null && canvas.getProject().getLastAction() == lastAction) {
			canvas.getProject().undoAction();
			lastAction = null;
		}
	}

	@Override
	public void paintIcon(ComponentDrawContext c, int x, int y) {
		Graphics g = c.getGraphics();
		if (toolIcon != null) toolIcon.paintIcon(c.getDestination(), g, x + 2, y + 2);
		else {
			g.setColor(Color.black);
			g.drawLine(x + 3, y + 13, x + 17, y + 7);
			g.fillOval(x + 1, y + 11, 5, 5);
			g.fillOval(x + 15, y + 5, 5, 5);
		}
	}

	@Override
	public Cursor getCursor() {
		return cursor;
	}
}
