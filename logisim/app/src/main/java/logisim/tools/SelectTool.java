/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import java.awt.Color;
import java.awt.Cursor;
import java.awt.Graphics;
import java.awt.event.InputEvent;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;

import javax.swing.Icon;

import logisim.circuit.Circuit;
import logisim.circuit.ReplacementMap;
import logisim.circuit.Wire;
import logisim.comp.Component;
import logisim.comp.ComponentDrawContext;
import logisim.comp.ComponentFactory;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.gui.main.Canvas;
import logisim.gui.main.Selection;
import logisim.gui.main.SelectionActions;
import logisim.gui.main.Selection.Event;
import logisim.prefs.AppPreferences;
import logisim.proj.Action;
import logisim.proj.Project;
import logisim.tools.key.KeyConfigurationEvent;
import logisim.tools.key.KeyConfigurator;
import logisim.tools.key.KeyConfigurationResult;
import logisim.tools.move.MoveResult;
import logisim.tools.move.MoveGesture;
import logisim.tools.move.MoveRequestListener;
import logisim.util.GraphicsUtil;
import logisim.util.Icons;
import logisim.util.StringGetter;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

public class SelectTool extends Tool {
	private static final Cursor selectCursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR);
	private static final Cursor rectSelectCursor = Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
	private static final Cursor moveCursor = Cursor.getPredefinedCursor(Cursor.MOVE_CURSOR);

	private static final int IDLE = 0;
	private static final int MOVING = 1;
	private static final int RECT_SELECT = 2;
	private static final Icon toolIcon = Icons.getIcon("select.gif");

	private static final Color COLOR_UNMATCHED = new Color(192, 0, 0);
	private static final Color COLOR_COMPUTING = new Color(96, 192, 96);
	private static final Color COLOR_RECT_SELECT = new Color(0, 64, 128, 255);
	private static final Color BACKGROUND_RECT_SELECT = new Color(192, 192, 255, 192);

	private static class MoveRequestHandler implements MoveRequestListener {
		private Canvas canvas;

		MoveRequestHandler(Canvas canvas) {
			this.canvas = canvas;
		}

		public void requestSatisfied(MoveGesture gesture, int dx, int dy) {
			clearCanvasMessage(canvas, dx, dy);
		}
	}

	private class Listener implements Selection.Listener {
		public void selectionChanged(Event event) {
			keyHandlers = null;
		}
	}

	private Location start;
	private int state;
	private int curDx;
	private int curDy;
	private boolean drawConnections;
	private MoveGesture moveGesture;
	private HashMap<Component, KeyConfigurator> keyHandlers;
	private Listener selListener;

	public SelectTool() {
		start = null;
		state = IDLE;
		selListener = new Listener();
		keyHandlers = null;
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof SelectTool;
	}

	@Override
	public int hashCode() {
		return SelectTool.class.hashCode();
	}

	@Override
	public String getName() {
		return "Select Tool";
	}

	@Override
	public String getDisplayName() {
		return Strings.get("selectTool");
	}

	@Override
	public String getDescription() {
		return Strings.get("selectToolDesc");
	}

	@Override
	public AttributeSet getAttributeSet(Canvas canvas) {
		return canvas.getSelection().getAttributeSet();
	}

	@Override
	public boolean isAllDefaultValues(AttributeSet attrs) {
		return true;
	}

	@Override
	public void draw(Canvas canvas, ComponentDrawContext context) {
		Project proj = canvas.getProject();
		int dx = curDx;
		int dy = curDy;
		if (state == MOVING) {
			proj.getSelection().drawGhostsShifted(context, dx, dy);

			MoveGesture gesture = moveGesture;
			if (gesture != null && drawConnections && (dx != 0 || dy != 0)) {
				MoveResult result = gesture.findResult(dx, dy);
				if (result != null) {
					Collection<Wire> wiresToAdd = result.getWiresToAdd();
					Graphics g = context.getGraphics();
					GraphicsUtil.switchToWidth(g, 3);
					g.setColor(Color.GRAY);
					for (Wire w : wiresToAdd) {
						Location loc0 = w.getEnd0();
						Location loc1 = w.getEnd1();
						g.drawLine(loc0.x(), loc0.y(), loc1.x(), loc1.y());
					}
					GraphicsUtil.switchToWidth(g, 1);
					g.setColor(COLOR_UNMATCHED);
					for (Location conn : result.getUnconnectedLocations()) {
						int connX = conn.x();
						int connY = conn.y();
						g.fillOval(connX - 3, connY - 3, 6, 6);
						g.fillOval(connX + dx - 3, connY + dy - 3, 6, 6);
					}
				}
			}
		} else if (state == RECT_SELECT) {
			int left = start.x();
			int right = left + dx;
			if (left > right) {
				int i = left;
				left = right;
				right = i;
			}
			int top = start.y();
			int bot = top + dy;
			if (top > bot) {
				int i = top;
				top = bot;
				bot = i;
			}

			Graphics gBase = context.getGraphics();
			int w = right - left - 1;
			int h = bot - top - 1;
			if (w > 2 && h > 2) {
				gBase.setColor(BACKGROUND_RECT_SELECT);
				gBase.fillRect(left + 1, top + 1, w - 1, h - 1);
			}

			Circuit circ = canvas.getCircuit();
			Bounds bds = Bounds.create(left, top, right - left, bot - top);
			for (Component c : circ.getAllWithin(bds)) {
				Location cloc = c.getLocation();
				Graphics gDup = gBase.create();
				context.setGraphics(gDup);
				c.getFactory().drawGhost(context, COLOR_RECT_SELECT, cloc.x(), cloc.y(), c.getAttributeSet());
				gDup.dispose();
			}

			gBase.setColor(COLOR_RECT_SELECT);
			GraphicsUtil.switchToWidth(gBase, 2);
			if (w < 0)
				w = 0;
			if (h < 0)
				h = 0;
			gBase.drawRect(left, top, w, h);
		}
	}

	@Override
	public void select(Canvas canvas) {
		Selection sel = canvas.getSelection();
		sel.addListener(selListener);
	}

	@Override
	public void deselect(Canvas canvas) {
		moveGesture = null;
	}

	@Override
	public void mouseEntered(Canvas canvas, Graphics g, MouseEvent e) {
		canvas.requestFocusInWindow();
	}

	@Override
	public void mousePressed(Canvas canvas, Graphics g, MouseEvent e) {
		Project proj = canvas.getProject();
		Selection sel = proj.getSelection();
		Circuit circuit = canvas.getCircuit();
		start = new Location(e.getX(), e.getY());
		curDx = 0;
		curDy = 0;
		moveGesture = null;

		// if the user clicks into the selection,
		// selection is being modified
		Collection<Component> in_sel = sel.getComponentsContaining(start, g);
		if (!in_sel.isEmpty()) if ((e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) == 0) {
			setState(proj, MOVING);
			proj.repaintCanvas();
			return;
		}
		else {
			Action act = SelectionActions.drop(sel, in_sel);
			if (act != null) proj.doAction(act);
		}

		// if the user clicks into a component outside selection, user
		// wants to add/reset selection
		Collection<Component> clicked = circuit.getAllContaining(start, g);
		if (!clicked.isEmpty()) {
			if ((e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) == 0)
				if (sel.getComponentsContaining(start).isEmpty()) {
					Action act = SelectionActions.dropAll(sel);
					if (act != null) proj.doAction(act);
				}
			for (Component comp : clicked) if (!in_sel.contains(comp)) sel.add(comp);
			setState(proj, MOVING);
			proj.repaintCanvas();
			return;
		}

		// The user clicked on the background. This is a rectangular
		// selection (maybe with the shift key down).
		if ((e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) == 0) {
			Action act = SelectionActions.dropAll(sel);
			if (act != null) proj.doAction(act);
		}
		setState(proj, RECT_SELECT);
		proj.repaintCanvas();
	}

	@Override
	public void mouseDragged(Canvas canvas, Graphics g, MouseEvent e) {
		if (state == MOVING) {
			Project proj = canvas.getProject();
			computeDxDy(proj, e, g);
			handleMoveDrag(canvas, curDx, curDy, e.getModifiersEx());
		} else if (state == RECT_SELECT) {
			Project proj = canvas.getProject();
			curDx = e.getX() - start.x();
			curDy = e.getY() - start.y();
			proj.repaintCanvas();
		}
	}

	private void handleMoveDrag(Canvas canvas, int dx, int dy, int modsEx) {
		boolean connect = shouldConnect(modsEx);
		drawConnections = connect;
		if (connect) {
			MoveGesture gesture = moveGesture;
			if (gesture == null) {
				gesture = new MoveGesture(new MoveRequestHandler(canvas), canvas.getCircuit(),
						canvas.getSelection().getAnchoredComponents());
				moveGesture = gesture;
			}
			if (dx != 0 || dy != 0) {
				boolean queued = gesture.enqueueRequest(dx, dy);
				if (queued) {
					canvas.setErrorMessage(new ComputingMessage(dx, dy), COLOR_COMPUTING);
					// maybe CPU scheduled led the request to be satisfied
					// just before the "if(queued)" statement. In any case, it
					// doesn't hurt to check to ensure the message belongs.
					if (gesture.findResult(dx, dy) != null) clearCanvasMessage(canvas, dx, dy);
				}
			}
		}
		canvas.repaint();
	}

	private boolean shouldConnect(int modsEx) {
		boolean shiftReleased = (modsEx & MouseEvent.SHIFT_DOWN_MASK) == 0;
		boolean dflt = AppPreferences.MOVE_KEEP_CONNECT.getBoolean();
		return shiftReleased == dflt;
	}

	@Override
	public void mouseReleased(Canvas canvas, Graphics g, MouseEvent e) {
		Project proj = canvas.getProject();
		if (state == MOVING) {
			setState(proj, IDLE);
			computeDxDy(proj, e, g);
			int dx = curDx;
			int dy = curDy;
			if (dx != 0 || dy != 0) if (!proj.getLogisimFile().contains(canvas.getCircuit()))
				canvas.setErrorMessage(Strings.getter("cannotModifyError"));
			else if (proj.getSelection().hasConflictWhenMoved(dx, dy))
				canvas.setErrorMessage(Strings.getter("exclusiveError"));
			else {
				boolean connect = shouldConnect(e.getModifiersEx());
				drawConnections = false;
				ReplacementMap repl;
				if (connect) {
					MoveGesture gesture = moveGesture;
					if (gesture == null)
						gesture = new MoveGesture(new MoveRequestHandler(canvas), canvas.getCircuit(),
								canvas.getSelection().getAnchoredComponents());
					canvas.setErrorMessage(new ComputingMessage(dx, dy), COLOR_COMPUTING);
					MoveResult result = gesture.forceRequest(dx, dy);
					clearCanvasMessage(canvas, dx, dy);
					repl = result.getReplacementMap();
				}
				else repl = null;
				Selection sel = proj.getSelection();
				proj.doAction(SelectionActions.translate(sel, dx, dy, repl));
			}
			moveGesture = null;
			proj.repaintCanvas();
		} else if (state == RECT_SELECT) {
			Bounds bds = Bounds.create(start).add(start.x() + curDx, start.y() + curDy);
			Circuit circuit = canvas.getCircuit();
			Selection sel = proj.getSelection();
			Collection<Component> in_sel = sel.getComponentsWithin(bds, g);
			for (Component comp : circuit.getAllWithin(bds, g))
				if (!in_sel.contains(comp))
					sel.add(comp);
			Action act = SelectionActions.drop(sel, in_sel);
			if (act != null) proj.doAction(act);
			setState(proj, IDLE);
			proj.repaintCanvas();
		}
	}

	@Override
	public void keyPressed(Canvas canvas, KeyEvent e) {
		if (state == MOVING && e.getKeyCode() == KeyEvent.VK_SHIFT)
			handleMoveDrag(canvas, curDx, curDy, e.getModifiersEx());
		else switch (e.getKeyCode()) {
			case KeyEvent.VK_BACK_SPACE:
			case KeyEvent.VK_DELETE:
				if (!canvas.getSelection().isEmpty()) {
					Action act = SelectionActions.clear(canvas.getSelection());
					canvas.getProject().doAction(act);
					e.consume();
				}
				break;
			default:
				processKeyEvent(canvas, e, KeyConfigurationEvent.KEY_PRESSED);
		}
	}

	@Override
	public void keyReleased(Canvas canvas, KeyEvent e) {
		if (state == MOVING && e.getKeyCode() == KeyEvent.VK_SHIFT)
			handleMoveDrag(canvas, curDx, curDy, e.getModifiersEx());
		else processKeyEvent(canvas, e, KeyConfigurationEvent.KEY_RELEASED);
	}

	@Override
	public void keyTyped(Canvas canvas, KeyEvent e) {
		processKeyEvent(canvas, e, KeyConfigurationEvent.KEY_TYPED);
	}

	private void processKeyEvent(Canvas canvas, KeyEvent e, int type) {
		HashMap<Component, KeyConfigurator> handlers = keyHandlers;
		if (handlers == null) {
			handlers = new HashMap<>();
			Selection sel = canvas.getSelection();
			for (Component comp : sel.getComponents()) {
				ComponentFactory factory = comp.getFactory();
				AttributeSet attrs = comp.getAttributeSet();
				Object handler = factory.getFeature(KeyConfigurator.class, attrs);
				if (handler != null) {
					KeyConfigurator base = (KeyConfigurator) handler;
					handlers.put(comp, base.clone());
				}
			}
			keyHandlers = handlers;
		}

		if (!handlers.isEmpty()) {
			boolean consume = false;
			ArrayList<KeyConfigurationResult> results = new ArrayList<>();
			for (Entry<Component, KeyConfigurator> entry : handlers.entrySet()) {
				Component comp = entry.getKey();
				KeyConfigurator handler = entry.getValue();
				KeyConfigurationEvent event = new KeyConfigurationEvent(type, comp.getAttributeSet(), e, comp);
				KeyConfigurationResult result = handler.keyEventReceived(event);
				consume |= event.isConsumed();
				if (result != null) results.add(result);
			}
			if (consume) e.consume();
			if (!results.isEmpty()) {
				SetAttributeAction act = new SetAttributeAction(canvas.getCircuit(),
						Strings.getter("changeComponentAttributesAction"));
				for (KeyConfigurationResult result : results) {
					Component comp = (Component) result.getEvent().getData();
					Map<Attribute<?>, Object> newValues = result.getAttributeValues();
					for (Entry<Attribute<?>, Object> entry : newValues.entrySet())
						act.set(comp, entry.getKey(), entry.getValue());
				}
				if (!act.isEmpty()) canvas.getProject().doAction(act);
			}
		}
	}

	private void computeDxDy(Project proj, MouseEvent e, Graphics g) {
		Bounds bds = proj.getSelection().getBounds(g);
		int dx;
		int dy;
		if (bds == Bounds.EMPTY_BOUNDS) {
			dx = e.getX() - start.x();
			dy = e.getY() - start.y();
		} else {
			dx = Math.max(e.getX() - start.x(), -bds.getX());
			dy = Math.max(e.getY() - start.y(), -bds.getY());
		}

		Selection sel = proj.getSelection();
		if (sel.shouldSnap()) {
			dx = Canvas.snapXToGrid(dx);
			dy = Canvas.snapYToGrid(dy);
		}
		curDx = dx;
		curDy = dy;
	}

	@Override
	public void paintIcon(ComponentDrawContext c, int x, int y) {
		Graphics g = c.getGraphics();
		if (toolIcon != null) toolIcon.paintIcon(c.getDestination(), g, x + 2, y + 2);
		else {
			int[] xp = { x + 5, x + 5, x + 9, x + 12, x + 14, x + 11, x + 16 };
			int[] yp = { y, y + 17, y + 12, y + 18, y + 18, y + 12, y + 12 };
			g.setColor(Color.black);
			g.fillPolygon(xp, yp, xp.length);
		}
	}

	@Override
	public Cursor getCursor() {
		return state == IDLE ? selectCursor : (state == RECT_SELECT ? rectSelectCursor : moveCursor);
	}

	@Override
	public Set<Component> getHiddenComponents(Canvas canvas) {
		if (state == MOVING) {
			int dx = curDx;
			int dy = curDy;
			if (dx == 0 && dy == 0) return null;

			Set<Component> sel = canvas.getSelection().getComponents();
			MoveGesture gesture = moveGesture;
			if (gesture != null && drawConnections) {
				MoveResult result = gesture.findResult(dx, dy);
				if (result != null) {
					HashSet<Component> ret = new HashSet<>(sel);
					ret.addAll(result.getReplacementMap().getRemovals());
					return ret;
				}
			}
			return sel;
		} else return null;
	}

	private void setState(Project proj, int new_state) {
		if (state == new_state)
			return; // do nothing if state not new

		state = new_state;
		proj.getFrame().getCanvas().setCursor(getCursor());
	}

	private static void clearCanvasMessage(Canvas canvas, int dx, int dy) {
		Object getter = canvas.getErrorMessage();
		if (getter instanceof ComputingMessage msg) if (msg.dx == dx && msg.dy == dy) {
			canvas.setErrorMessage(null);
			canvas.repaint();
		}
	}

	private static class ComputingMessage implements StringGetter {
		private int dx;
		private int dy;

		public ComputingMessage(int dx, int dy) {
			this.dx = dx;
			this.dy = dy;
		}

		public String get() {
			return Strings.get("moveWorkingMsg");
		}
	}
}
