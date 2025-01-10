/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import java.awt.Cursor;
import java.awt.Graphics;
import java.awt.Point;
import java.awt.event.ActionEvent;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import java.util.Collections;
import java.util.List;

import javax.swing.AbstractAction;
import javax.swing.ActionMap;
import javax.swing.Icon;
import javax.swing.InputMap;
import javax.swing.KeyStroke;

import draw.actions.ModelAddAction;
import draw.actions.ModelEditTextAction;
import draw.actions.ModelRemoveAction;
import draw.canvas.Canvas;
import draw.shapes.DrawAttr;
import draw.shapes.Text;
import draw.util.EditableLabelField;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.Location;
import logisim.util.Icons;

public class TextTool extends AbstractTool {
	private class FieldListener extends AbstractAction implements AttributeListener {
		public void actionPerformed(ActionEvent e) {
			commitText(curCanvas);
		}

		public void attributeListChanged(AttributeEvent e) {
			Text cur = curText;
			if (cur != null) {
				double zoom = curCanvas.getZoomFactor();
				cur.getLabel().configureTextField(field, zoom);
				curCanvas.repaint();
			}
		}

		public void attributeValueChanged(AttributeEvent e) {
			attributeListChanged(e);
		}
	}

	private class CancelListener extends AbstractAction {
		public void actionPerformed(ActionEvent e) {
			cancelText(curCanvas);
		}
	}

	private DrawingAttributeSet attrs;
	private EditableLabelField field;
	private FieldListener fieldListener;

	private Text curText;
	private Canvas curCanvas;
	private boolean isTextNew;

	public TextTool(DrawingAttributeSet attrs) {
		this.attrs = attrs;
		curText = null;
		isTextNew = false;
		field = new EditableLabelField();

		fieldListener = new FieldListener();
		InputMap fieldInput = field.getInputMap();
		fieldInput.put(KeyStroke.getKeyStroke(KeyEvent.VK_ENTER, 0), "commit");
		fieldInput.put(KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0), "cancel");
		ActionMap fieldAction = field.getActionMap();
		fieldAction.put("commit", fieldListener);
		fieldAction.put("cancel", new CancelListener());
	}

	@Override
	public Icon getIcon() {
		return Icons.getIcon("text.gif");
	}

	@Override
	public Cursor getCursor(Canvas canvas) {
		return Cursor.getPredefinedCursor(Cursor.TEXT_CURSOR);
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.ATTRS_TEXT_TOOL;
	}

	@Override
	public void toolSelected(Canvas canvas) {
		cancelText(canvas);
	}

	@Override
	public void toolDeselected(Canvas canvas) {
		commitText(canvas);
	}

	@Override
	public void mousePressed(Canvas canvas, MouseEvent e) {
		if (curText != null)
			commitText(canvas);

		Text clicked = null;
		boolean found = true;
		Location mloc = new Location(e.getX(), e.getY());
		clicked = (Text)canvas.getModel().getObjectsFromTop().stream()
				.filter(o->o instanceof Text text && o.contains(mloc, true))
				.findFirst()
				.orElse(null);
		if (clicked == null) {
			found = false;
			clicked = attrs.applyTo(new Text(mloc, ""));
		}
		curText = clicked;
		curCanvas = canvas;
		isTextNew = !found;
		clicked.getLabel().configureTextField(field, canvas.getZoomFactor());
		field.setText(clicked.getText());
		canvas.add(field);

		Point fieldLoc = field.getLocation();
		double zoom = canvas.getZoomFactor();
		Location zoomed = mloc.mul(zoom);
		fieldLoc.x = zoomed.x() - fieldLoc.x;
		fieldLoc.y = zoomed.y() - fieldLoc.y;
		int caret = field.viewToModel2D(fieldLoc);
		if (caret >= 0)
			field.setCaretPosition(caret);
		field.requestFocus();

		canvas.getSelection().setSelected(clicked, true);
		canvas.getSelection().setHidden(Collections.singleton(clicked), true);
		clicked.addAttributeListener(fieldListener);
		canvas.repaint();
	}

	@Override
	public void zoomFactorChanged(Canvas canvas) {
		if (curText != null)
			curText.getLabel().configureTextField(field, canvas.getZoomFactor());
	}

	@Override
	public void draw(Canvas canvas, Graphics g) {
		// actually, there's nothing to do here - it's handled by the field
	}

	private void cancelText(Canvas canvas) {
		Text cur = curText;
		if (cur == null)
			return;

		curText = null;
		cur.removeAttributeListener(fieldListener);
		canvas.remove(field);
		canvas.getSelection().clearSelected();
		canvas.repaint();
	}

	private void commitText(Canvas canvas) {
		Text cur = curText;
		boolean isNew = isTextNew;
		String newText = field.getText();
		if (cur == null)
			return;
		cancelText(canvas);

		if (isNew) {
			if (!newText.isEmpty()) {
				cur.setText(newText);
				canvas.doAction(new ModelAddAction(canvas.getModel(), cur));
			}
		} else {
			String oldText = cur.getText();
			if (newText.isEmpty())
				canvas.doAction(new ModelRemoveAction(canvas.getModel(), cur));
			else if (!oldText.equals(newText))
				canvas.doAction(new ModelEditTextAction(canvas.getModel(), cur, newText));
		}
	}
}
