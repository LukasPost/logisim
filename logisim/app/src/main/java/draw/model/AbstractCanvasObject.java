/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import draw.shapes.DrawAttr;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.EventSourceWeakSupport;
import logisim.util.GraphicsUtil;

import java.awt.Color;
import java.awt.Graphics;
import java.util.List;
import java.util.Objects;
import java.util.Random;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

public abstract class AbstractCanvasObject implements AttributeSet, CanvasObject, Cloneable {
	private static final int OVERLAP_TRIES = 50;
	private static final int GENERATE_RANDOM_TRIES = 20;

	private EventSourceWeakSupport<AttributeListener> listeners;

	public AbstractCanvasObject() {
		listeners = new EventSourceWeakSupport<>();
	}

	public AttributeSet getAttributeSet() {
		return this;
	}

	public abstract String getDisplayName();

	public abstract Element toSvgElement(Document doc);

	public abstract boolean matches(CanvasObject other);

	public abstract int matchesHashCode();

	public abstract Bounds getBounds();

	public abstract boolean contains(Location loc, boolean assumeFilled);

	public abstract void translate(Location distance);

	public abstract List<Handle> getHandles(HandleGesture gesture);

	protected abstract void updateValue(Attribute<?> attr, Object value);

	public abstract void paint(Graphics g, HandleGesture gesture);

	public boolean canRemove() {
		return true;
	}

	public boolean canMoveHandle(Handle handle) {
		return false;
	}

	public Handle canInsertHandle(Location desired) {
		return null;
	}

	public Handle canDeleteHandle(Location loc) {
		return null;
	}

	public Handle moveHandle(HandleGesture gesture) {
		throw new UnsupportedOperationException("moveHandle");
	}

	public void insertHandle(Handle desired, Handle previous) {
		throw new UnsupportedOperationException("insertHandle");
	}

	public Handle deleteHandle(Handle handle) {
		throw new UnsupportedOperationException("deleteHandle");
	}

	public boolean overlaps(CanvasObject other) {
		Bounds a = getBounds();
		Bounds b = other.getBounds();
		Bounds c = a.intersect(b);
		Random rand = new Random();
		if (c.getWidth() == 0 || c.getHeight() == 0)
			return false;
		if (other instanceof AbstractCanvasObject that)
			for (int i = 0; i < OVERLAP_TRIES; i++)
				if (i % 2 == 0) {
					Location loc = getRandomPoint(c, rand);
					if (loc != null && that.contains(loc, false))
						return true;
				}
				else {
					Location loc = that.getRandomPoint(c, rand);
					if (loc != null && contains(loc, false))
						return true;
				}
		else
			for (int i = 0; i < OVERLAP_TRIES; i++) {
				Location loc = getRandomPoint(c, rand);
				if (loc != null && other.contains(loc, false))
					return true;
			}
		return false;
	}

	protected Location getRandomPoint(Bounds bds, Random rand) {
		int x = bds.getX();
		int y = bds.getY();
		int w = bds.getWidth();
		int h = bds.getHeight();
		for (int i = 0; i < GENERATE_RANDOM_TRIES; i++) {
			Location loc = new Location(x + rand.nextInt(w), y + rand.nextInt(h));
			if (contains(loc, false))
				return loc;
		}
		return null;
	}

	// methods required by AttributeSet interface
	public abstract List<Attribute<?>> getAttributes();

	public abstract <V> V getValue(Attribute<V> attr);

	public void addAttributeListener(AttributeListener l) {
		listeners.add(l);
	}

	public void removeAttributeListener(AttributeListener l) {
		listeners.remove(l);
	}

	@Override
	public CanvasObject clone() {
		try {
			AbstractCanvasObject ret = (AbstractCanvasObject) super.clone();
			ret.listeners = new EventSourceWeakSupport<>();
			return ret;
		}
		catch (CloneNotSupportedException e) {
			return null;
		}
	}

	public boolean containsAttribute(Attribute<?> attr) {
		return getAttributes().contains(attr);
	}

	public Attribute<?> getAttribute(String name) {
		return getAttributes().stream()
				.filter(attr -> attr.getName().equals(name)).findFirst()
				.orElse(null);
	}

	public boolean isReadOnly(Attribute<?> attr) {
		return false;
	}

	public void setReadOnly(Attribute<?> attr, boolean value) {
		throw new UnsupportedOperationException("setReadOnly");
	}

	public boolean isToSave(Attribute<?> attr) {
		return true;
	}

	public final <V> void setValue(Attribute<V> attr, V value) {
		if (Objects.equals(getValue(attr), value))
			return;
		updateValue(attr, value);
		AttributeEvent e = new AttributeEvent(this, attr, value);
		listeners.forEach(listener -> listener.attributeValueChanged(e));
	}

	protected void fireAttributeListChanged() {
		AttributeEvent e = new AttributeEvent(this);
		listeners.forEach(listener -> listener.attributeListChanged(e));
	}

	protected boolean setForStroke(Graphics g) {
		if (getAttributes().contains(DrawAttr.PAINT_TYPE) && getValue(DrawAttr.PAINT_TYPE) == DrawAttr.PAINT_FILL)
			return false;

		Integer width = getValue(DrawAttr.STROKE_WIDTH);
		if (width == null || width <= 0)
			return false;

		Color color = getValue(DrawAttr.STROKE_COLOR);
		if (color != null && color.getAlpha() == 0)
			return false;

		GraphicsUtil.switchToWidth(g, width);
		if (color != null)
			g.setColor(color);
		return true;
	}

	protected boolean setForFill(Graphics g) {
		if (getAttributes().contains(DrawAttr.PAINT_TYPE) && getValue(DrawAttr.PAINT_TYPE) == DrawAttr.PAINT_STROKE)
			return false;

		Color color = getValue(DrawAttr.FILL_COLOR);
		if (color != null && color.getAlpha() == 0)
			return false;

		if (color != null)
			g.setColor(color);
		return true;
	}

}
