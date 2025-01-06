/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import java.awt.Cursor;
import java.awt.Graphics;
import java.awt.event.MouseEvent;
import java.awt.event.KeyEvent;

import logisim.circuit.Circuit;
import logisim.circuit.CircuitEvent;
import logisim.circuit.CircuitListener;
import logisim.circuit.CircuitMutation;
import logisim.comp.Component;
import logisim.comp.ComponentDrawContext;
import logisim.comp.ComponentUserEvent;
import logisim.data.AttributeSet;
import logisim.data.Location;
import logisim.gui.main.Canvas;
import logisim.proj.Action;
import logisim.proj.Project;
import logisim.std.base.Text;

public class TextTool extends Tool {
	private class MyListener implements CaretListener, CircuitListener {
		public void editingCanceled(CaretEvent e) {
			if (e.getCaret() != caret) {
				e.getCaret().removeCaretListener(this);
				return;
			}
			caret.removeCaretListener(this);
			caretCircuit.removeCircuitListener(this);

			caretCircuit = null;
			caretComponent = null;
			caretCreatingText = false;
			caret = null;
		}

		public void editingStopped(CaretEvent e) {
			if (e.getCaret() != caret) {
				e.getCaret().removeCaretListener(this);
				return;
			}
			caret.removeCaretListener(this);
			caretCircuit.removeCircuitListener(this);

			String val = caret.getText();
			boolean isEmpty = (val == null || val.isEmpty());
			Action a;
			Project proj = caretCanvas.getProject();
			if (caretCreatingText) if (!isEmpty) {
				CircuitMutation xn = new CircuitMutation(caretCircuit);
				xn.add(caretComponent);
				a = xn.toAction(Strings.getter("addComponentAction", Text.FACTORY.getDisplayGetter()));
			}
			else a = null; // don't add the blank text field
			else if (isEmpty && caretComponent.getFactory() instanceof Text) {
				CircuitMutation xn = new CircuitMutation(caretCircuit);
				xn.add(caretComponent);
				a = xn.toAction(Strings.getter("removeComponentAction", Text.FACTORY.getDisplayGetter()));
			}
			else {
				Object obj = caretComponent.getFeature(TextEditable.class);
				// should never happen
				if (obj == null) a = null;
				else {
					TextEditable editable = (TextEditable) obj;
					a = editable.getCommitAction(caretCircuit, e.getOldText(), e.getText());
				}
			}

			caretCircuit = null;
			caretComponent = null;
			caretCreatingText = false;
			caret = null;

			if (a != null)
				proj.doAction(a);
		}

		public void circuitChanged(CircuitEvent event) {
			if (event.getCircuit() != caretCircuit) {
				event.getCircuit().removeCircuitListener(this);
				return;
			}
			int action = event.getAction();
			if (action == CircuitEvent.ACTION_REMOVE) {
				if (event.getData() == caretComponent) caret.cancelEditing();
			} else if (action == CircuitEvent.ACTION_CLEAR) if (caretComponent != null) caret.cancelEditing();
		}
	}

	private static Cursor cursor = Cursor.getPredefinedCursor(Cursor.TEXT_CURSOR);

	private MyListener listener = new MyListener();
	private AttributeSet attrs;
	private Caret caret;
	private boolean caretCreatingText;
	private Canvas caretCanvas;
	private Circuit caretCircuit;
	private Component caretComponent;

	public TextTool() {
		attrs = Text.FACTORY.createAttributeSet();
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof TextTool;
	}

	@Override
	public int hashCode() {
		return TextTool.class.hashCode();
	}

	@Override
	public String getName() {
		return "Text Tool";
	}

	@Override
	public String getDisplayName() {
		return Strings.get("textTool");
	}

	@Override
	public String getDescription() {
		return Strings.get("textToolDesc");
	}

	@Override
	public AttributeSet getAttributeSet() {
		return attrs;
	}

	@Override
	public void paintIcon(ComponentDrawContext c, int x, int y) {
		Text.FACTORY.paintIcon(c, x, y, null);
	}

	@Override
	public void draw(Canvas canvas, ComponentDrawContext context) {
		if (caret != null)
			caret.draw(context.getGraphics());
	}

	@Override
	public void deselect(Canvas canvas) {
		if (caret != null) {
			caret.stopEditing();
			caret = null;
		}
	}

	@Override
	public void mousePressed(Canvas canvas, Graphics g, MouseEvent e) {
		Project proj = canvas.getProject();
		Circuit circ = canvas.getCircuit();

		if (!proj.getLogisimFile().contains(circ)) {
			if (caret != null)
				caret.cancelEditing();
			canvas.setErrorMessage(Strings.getter("cannotModifyError"));
			return;
		}

		// Maybe user is clicking within the current caret.
		// No. End the current caret.
		if (caret != null) if (caret.getBounds(g).contains(e.getX(), e.getY())) { // Yes
			caret.mousePressed(e);
			proj.repaintCanvas();
			return;
		}
		else caret.stopEditing();
		// caret will be null at this point

		// Otherwise search for a new caret.
		int x = e.getX();
		int y = e.getY();
		Location loc = new Location(x, y);
		ComponentUserEvent event = new ComponentUserEvent(canvas, x, y);

		// First search in selection.
		for (Component comp : proj.getSelection().getComponentsContaining(loc, g)) {
			TextEditable editable = (TextEditable) comp.getFeature(TextEditable.class);
			if (editable != null) {
				caret = editable.getTextCaret(event);
				if (caret != null) {
					proj.getFrame().viewComponentAttributes(circ, comp);
					caretComponent = comp;
					caretCreatingText = false;
					break;
				}
			}
		}

		// Then search in circuit
		if (caret == null) for (Component comp : circ.getAllContaining(loc, g)) {
			TextEditable editable = (TextEditable) comp.getFeature(TextEditable.class);
			if (editable != null) {
				caret = editable.getTextCaret(event);
				if (caret != null) {
					proj.getFrame().viewComponentAttributes(circ, comp);
					caretComponent = comp;
					caretCreatingText = false;
					break;
				}
			}
		}

		// if nothing found, create a new label
		if (caret == null) {
			if (loc.x() < 0 || loc.y() < 0)
				return;
			AttributeSet copy = (AttributeSet) attrs.clone();
			caretComponent = Text.FACTORY.createComponent(loc, copy);
			caretCreatingText = true;
			TextEditable editable = (TextEditable) caretComponent.getFeature(TextEditable.class);
			if (editable != null) {
				caret = editable.getTextCaret(event);
				proj.getFrame().viewComponentAttributes(circ, caretComponent);
			}
		}

		if (caret != null) {
			caretCanvas = canvas;
			caretCircuit = canvas.getCircuit();
			caret.addCaretListener(listener);
			caretCircuit.addCircuitListener(listener);
		}
		proj.repaintCanvas();
	}

	@Override
	public void mouseDragged(Canvas canvas, Graphics g, MouseEvent e) {
		// TODO: enhance label editing
	}

	@Override
	public void mouseReleased(Canvas canvas, Graphics g, MouseEvent e) {
		// TODO: enhance label editing
	}

	@Override
	public void keyPressed(Canvas canvas, KeyEvent e) {
		if (caret != null) {
			caret.keyPressed(e);
			canvas.getProject().repaintCanvas();
		}
	}

	@Override
	public void keyReleased(Canvas canvas, KeyEvent e) {
		if (caret != null) {
			caret.keyReleased(e);
			canvas.getProject().repaintCanvas();
		}
	}

	@Override
	public void keyTyped(Canvas canvas, KeyEvent e) {
		if (caret != null) {
			caret.keyTyped(e);
			canvas.getProject().repaintCanvas();
		}
	}

	@Override
	public Cursor getCursor() {
		return cursor;
	}
}
