/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import java.awt.Color;
import java.awt.Font;
import java.awt.Graphics;
import java.util.List;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import draw.model.CanvasObject;
import draw.model.AbstractCanvasObject;
import draw.model.Handle;
import draw.model.HandleGesture;
import draw.util.EditableLabel;
import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.UnmodifiableList;

public class Text extends AbstractCanvasObject {
	private EditableLabel label;

	public Text(Location loc, String text) {
		this(loc, EditableLabel.LEFT, EditableLabel.BASELINE, text, DrawAttr.DEFAULT_FONT, Color.BLACK);
	}

	private Text(Location loc, int halign, int valign, String text, Font font, Color color) {
		label = new EditableLabel(loc, text, font);
		label.setColor(color);
		label.setHorizontalAlignment(halign);
		label.setVerticalAlignment(valign);
	}

	@Override
	public Text clone() {
		Text ret = (Text) super.clone();
		ret.label = label.clone();
		return ret;
	}

	@Override
	public boolean matches(CanvasObject other) {
		return other instanceof Text that && label.equals(that.label);
	}

	@Override
	public int matchesHashCode() {
		return label.hashCode();
	}

	@Override
	public Element toSvgElement(Document doc) {
		return SvgCreator.createText(doc, this);
	}

	public Location getLocation() {
		return label.getLocation();
	}

	public String getText() {
		return label.getText();
	}

	public EditableLabel getLabel() {
		return label;
	}

	public void setText(String value) {
		label.setText(value);
	}

	@Override
	public String getDisplayName() {
		return Strings.get("shapeText");
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.ATTRS_TEXT;
	}

	@Override
	@SuppressWarnings("unchecked")
	public <V> V getValue(Attribute<V> attr) {
		if (attr == DrawAttr.FONT)
			return (V) label.getFont();
		else if (attr == DrawAttr.FILL_COLOR)
			return (V) label.getColor();
		else if (attr == DrawAttr.ALIGNMENT) {
			int halign = label.getHorizontalAlignment();
			AttributeOption h;
			if (halign == EditableLabel.LEFT)
				h = DrawAttr.ALIGN_LEFT;
			else if (halign == EditableLabel.RIGHT)
				h = DrawAttr.ALIGN_RIGHT;
			else
				h = DrawAttr.ALIGN_CENTER;
			return (V) h;
		} else
			return null;
	}

	@Override
	public void updateValue(Attribute<?> attr, Object value) {
		if (attr == DrawAttr.FONT)
			label.setFont((Font) value);
		else if (attr == DrawAttr.FILL_COLOR)
			label.setColor((Color) value);
		else if (attr == DrawAttr.ALIGNMENT)
			label.setHorizontalAlignment((Integer) ((AttributeOption) value).getValue());
	}

	@Override
	public Bounds getBounds() {
		return label.getBounds();
	}

	@Override
	public boolean contains(Location loc, boolean assumeFilled) {
		return label.contains(loc);
	}

	@Override
	public void translate(Location distance) {
		label.setLocation(label.getLocation().add(distance));
	}

	public List<Handle> getHandles() {
		Bounds bds = label.getBounds();
		int x = bds.getX();
		int y = bds.getY();
		int w = bds.getWidth();
		int h = bds.getHeight();
		return UnmodifiableList.create(new Handle[] {
				new Handle(this, x, y),
				new Handle(this, x + w, y),
				new Handle(this, x + w, y + h),
				new Handle(this, x, y + h)
		});
	}

	@Override
	public List<Handle> getHandles(HandleGesture gesture) {
		return getHandles();
	}

	@Override
	public void paint(Graphics g, HandleGesture gesture) {
		label.paint(g);
	}
}
