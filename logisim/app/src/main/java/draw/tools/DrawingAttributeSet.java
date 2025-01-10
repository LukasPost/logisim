/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import draw.gui.SelectionAttributes.AttributeWithValue;
import draw.model.AbstractCanvasObject;
import draw.shapes.DrawAttr;
import logisim.data.AbstractAttributeSet;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;
import logisim.util.EventSourceWeakSupport;
import logisim.util.UnmodifiableList;

import java.awt.Color;
import java.util.*;
import java.util.stream.Collectors;

public class DrawingAttributeSet implements AttributeSet, Cloneable {
	static final List<AttributeWithValue> ATTRS_ALL = UnmodifiableList
			.create(new AttributeWithValue[] {
					new AttributeWithValue(DrawAttr.FONT, DrawAttr.DEFAULT_FONT),
					new AttributeWithValue(DrawAttr.ALIGNMENT, DrawAttr.ALIGN_CENTER),
					new AttributeWithValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_STROKE),
					new AttributeWithValue(DrawAttr.STROKE_WIDTH, 1),
					new AttributeWithValue(DrawAttr.STROKE_COLOR, Color.BLACK),
					new AttributeWithValue(DrawAttr.FILL_COLOR, Color.WHITE),
					new AttributeWithValue(DrawAttr.TEXT_DEFAULT_FILL, Color.BLACK),
					new AttributeWithValue(DrawAttr.CORNER_RADIUS, 10)
			});

	private class Restriction extends AbstractAttributeSet implements AttributeListener {
		private AbstractTool tool;
		private List<Attribute<?>> selectedAttrs;
		private List<Attribute<?>> selectedView;

		Restriction(AbstractTool tool) {
			this.tool = tool;
			updateAttributes();
		}

		private void updateAttributes() {
			List<Attribute<?>> toolAttrs = tool == null ? Collections.emptyList() : tool.getAttributes();
			if (toolAttrs.equals(selectedAttrs))
				return;
			selectedAttrs = new ArrayList<>(toolAttrs);
			selectedView = Collections.unmodifiableList(selectedAttrs);
			DrawingAttributeSet.this.addAttributeListener(this);
			fireAttributeListChanged();
		}

		@Override
		protected void copyInto(AbstractAttributeSet dest) {
			DrawingAttributeSet.this.addAttributeListener(this);
		}

		@Override
		public List<Attribute<?>> getAttributes() {
			return selectedView;
		}

		@Override
		public <V> V getValue(Attribute<V> attr) {
			return DrawingAttributeSet.this.getValue(attr);
		}

		@Override
		public <V> void setValue(Attribute<V> attr, V value) {
			DrawingAttributeSet.this.setValue(attr, value);
			updateAttributes();
		}

		//
		// AttributeListener methods
		//
		public void attributeListChanged(AttributeEvent e) {
			fireAttributeListChanged();
		}

		public void attributeValueChanged(AttributeEvent e) {
			if (selectedAttrs.contains(e.getAttribute())) {
				@SuppressWarnings("unchecked")
				Attribute<Object> attr = (Attribute<Object>) e.getAttribute();
				fireAttributeValueChanged(attr, e.getValue());
			}
			updateAttributes();
		}
	}

	private EventSourceWeakSupport<AttributeListener> listeners;
	private List<AttributeWithValue> attrs;

	public DrawingAttributeSet() {
		listeners = new EventSourceWeakSupport<>();
		attrs = ATTRS_ALL;
	}

	public AttributeSet createSubset(AbstractTool tool) {
		return new Restriction(tool);
	}

	public void addAttributeListener(AttributeListener l) {
		listeners.add(l);
	}

	public void removeAttributeListener(AttributeListener l) {
		listeners.remove(l);
	}

	@Override
	public Object clone() {
		try {
			DrawingAttributeSet ret = (DrawingAttributeSet) super.clone();
			ret.listeners = new EventSourceWeakSupport<>();
			ret.attrs = new ArrayList<>(attrs);
			return ret;
		}
		catch (CloneNotSupportedException e) {
			return null;
		}
	}

	public List<Attribute<?>> getAttributes() {
		return attrs.stream().map(a-> a.attr).collect(Collectors.toList());
	}

	public boolean containsAttribute(Attribute<?> attr) {
		return attrs.stream().anyMatch(a->a.attr.equals(attr));
	}

	public Attribute<?> getAttribute(String name) {
		return attrs.stream()
				.filter(attr -> attr.attr.getName().equals(name))
				.findFirst()
				.map(attr -> attr.attr)
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

	public <V> V getValue(Attribute<V> attr) {
		return attrs.stream()
				.filter(awv -> awv.attr.equals(attr))
				.findFirst()
				.map(a -> (V) a.value)
				.orElse(null);
	}

	public <V> void setValue(Attribute<V> attr, V value) {
		Optional<AttributeWithValue> tochange = attrs.stream()
				.filter(awv -> awv.attr.equals(attr))
				.findFirst();
		if (tochange.isEmpty())
			throw new IllegalArgumentException(attr.toString());

		tochange.get().value = value;
		AttributeEvent e = new AttributeEvent(this, attr, value);
		listeners.forEach(l->l.attributeValueChanged(e));
		if (attr == DrawAttr.PAINT_TYPE) {
			AttributeEvent e2 = new AttributeEvent(this);
			listeners.forEach(l->l.attributeListChanged(e2));
		}
	}

	public <E extends AbstractCanvasObject> E applyTo(E drawable) {
		// use a for(i...) loop since the attribute list may change as we go on
		for (int i = 0; i < drawable.getAttributes().size(); i++) {
			Attribute<?> attr = drawable.getAttributes().get(i);
			@SuppressWarnings("unchecked")
			Attribute<Object> a = (Attribute<Object>) attr;
			if (attr == DrawAttr.FILL_COLOR && containsAttribute(DrawAttr.TEXT_DEFAULT_FILL))
				drawable.setValue(a, getValue(DrawAttr.TEXT_DEFAULT_FILL));
			else if (containsAttribute(a)) {
				Object value = getValue(a);
				drawable.setValue(a, value);
			}
		}
		return drawable;
	}
}
