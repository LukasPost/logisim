/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.instance;

import java.awt.Font;
import java.awt.Graphics;

import logisim.circuit.Circuit;
import logisim.comp.ComponentDrawContext;
import logisim.comp.ComponentUserEvent;
import logisim.comp.TextField;
import logisim.comp.TextFieldEvent;
import logisim.comp.TextFieldListener;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.gui.main.Canvas;
import logisim.proj.Action;
import logisim.tools.Caret;
import logisim.tools.SetAttributeAction;
import logisim.tools.TextEditable;

public class InstanceTextField implements AttributeListener, TextFieldListener, TextEditable {
	private InstanceComponent comp;
	private TextField field;
	private Attribute<String> labelAttr;
	private Attribute<Font> fontAttr;
	private int fieldX;
	private int fieldY;
	private int halign;
	private int valign;

	InstanceTextField(InstanceComponent comp) {
		this.comp = comp;
		field = null;
		labelAttr = null;
		fontAttr = null;
	}

	void update(Attribute<String> labelAttr, Attribute<Font> fontAttr, int x, int y, int halign, int valign) {
		boolean wasReg = shouldRegister();
		this.labelAttr = labelAttr;
		this.fontAttr = fontAttr;
		fieldX = x;
		fieldY = y;
		this.halign = halign;
		this.valign = valign;
		boolean shouldReg = shouldRegister();
		AttributeSet attrs = comp.getAttributeSet();
		if (!wasReg && shouldReg)
			attrs.addAttributeListener(this);
		if (wasReg && !shouldReg)
			attrs.removeAttributeListener(this);

		updateField(attrs);
	}

	private void updateField(AttributeSet attrs) {
		String text = attrs.getValue(labelAttr);
		if (text == null || text.isEmpty()) {
			if (field != null) {
				field.removeTextFieldListener(this);
				field = null;
			}
		} else if (field == null) createField(attrs, text);
		else {
			Font font = attrs.getValue(fontAttr);
			if (font != null)
				field.setFont(font);
			field.setLocation(fieldX, fieldY, halign, valign);
			field.setText(text);
		}
	}

	private void createField(AttributeSet attrs, String text) {
		Font font = attrs.getValue(fontAttr);
		field = new TextField(fieldX, fieldY, halign, valign, font);
		field.setText(text);
		field.addTextFieldListener(this);
	}

	private boolean shouldRegister() {
		return labelAttr != null || fontAttr != null;
	}

	Bounds getBounds(Graphics g) {
		return field == null ? Bounds.EMPTY_BOUNDS : field.getBounds(g);
	}

	void draw(ComponentDrawContext context) {
		if (field != null) {
			Graphics g = context.getGraphics().create();
			field.draw(g);
			g.dispose();
		}
	}

	public void attributeListChanged(AttributeEvent e) {
	}

	public void attributeValueChanged(AttributeEvent e) {
		Attribute<?> attr = e.getAttribute();
		if (attr == labelAttr) updateField(comp.getAttributeSet());
		else if (attr == fontAttr) if (field != null)
			field.setFont((Font) e.getValue());
	}

	public void textChanged(TextFieldEvent e) {
		String prev = e.getOldText();
		String next = e.getText();
		if (!next.equals(prev)) comp.getAttributeSet().setValue(labelAttr, next);
	}

	public Action getCommitAction(Circuit circuit, String oldText, String newText) {
		SetAttributeAction act = new SetAttributeAction(circuit, Strings.getter("changeLabelAction"));
		act.set(comp, labelAttr, newText);
		return act;
	}

	public Caret getTextCaret(ComponentUserEvent event) {
		Canvas canvas = event.getCanvas();
		Graphics g = canvas.getGraphics();

		// if field is absent, create it empty
		// and if it is empty, just return a caret at its beginning
		if (field == null)
			createField(comp.getAttributeSet(), "");
		String text = field.getText();
		if (text == null || text.isEmpty())
			return field.getCaret(g, 0);

		Bounds bds = field.getBounds(g);
		if (bds.getWidth() < 4 || bds.getHeight() < 4) {
			Location loc = comp.getLocation();
			bds = bds.add(Bounds.create(loc).expand(2));
		}

		int x = event.getX();
		int y = event.getY();
		if (bds.contains(x, y))
			return field.getCaret(g, x, y);
		else
			return null;
	}
}
