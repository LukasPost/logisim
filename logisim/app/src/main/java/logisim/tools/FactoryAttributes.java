/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import java.util.ArrayList;
import java.util.List;

import logisim.comp.ComponentFactory;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;
import logisim.data.AttributeSets;

class FactoryAttributes implements AttributeSet, AttributeListener, Cloneable {
	private Class<? extends Library> descBase;
	private FactoryDescription desc;
	private ComponentFactory factory;
	private AttributeSet baseAttrs;
	private ArrayList<AttributeListener> listeners;

	public FactoryAttributes(Class<? extends Library> descBase, FactoryDescription desc) {
		this.descBase = descBase;
		this.desc = desc;
		factory = null;
		baseAttrs = null;
		listeners = new ArrayList<>();
	}

	public FactoryAttributes(ComponentFactory factory) {
		descBase = null;
		desc = null;
		this.factory = factory;
		baseAttrs = null;
		listeners = new ArrayList<>();
	}

	boolean isFactoryInstantiated() {
		return baseAttrs != null;
	}

	AttributeSet getBase() {
		AttributeSet ret = baseAttrs;
		if (ret == null) {
			ComponentFactory fact = factory;
			if (fact == null) {
				fact = desc.getFactory(descBase);
				factory = fact;
			}
			if (fact == null) ret = AttributeSets.EMPTY;
			else {
				ret = fact.createAttributeSet();
				ret.addAttributeListener(this);
			}
			baseAttrs = ret;
		}
		return ret;
	}

	public void addAttributeListener(AttributeListener l) {
		listeners.add(l);
	}

	public void removeAttributeListener(AttributeListener l) {
		listeners.remove(l);
	}

	@Override
	public AttributeSet clone() {
		return (AttributeSet) getBase().clone();
	}

	public boolean containsAttribute(Attribute<?> attr) {
		return getBase().containsAttribute(attr);
	}

	public Attribute<?> getAttribute(String name) {
		return getBase().getAttribute(name);
	}

	public List<Attribute<?>> getAttributes() {
		return getBase().getAttributes();
	}

	public <V> V getValue(Attribute<V> attr) {
		return getBase().getValue(attr);
	}

	public boolean isReadOnly(Attribute<?> attr) {
		return getBase().isReadOnly(attr);
	}

	public boolean isToSave(Attribute<?> attr) {
		return getBase().isToSave(attr);
	}

	public void setReadOnly(Attribute<?> attr, boolean value) {
		getBase().setReadOnly(attr, value);
	}

	public <V> void setValue(Attribute<V> attr, V value) {
		getBase().setValue(attr, value);
	}

	public void attributeListChanged(AttributeEvent baseEvent) {
		AttributeEvent e = null;
		for (AttributeListener l : listeners) {
			if (e == null) e = new AttributeEvent(this, baseEvent.getAttribute(), baseEvent.getValue());
			l.attributeListChanged(e);
		}
	}

	public void attributeValueChanged(AttributeEvent baseEvent) {
		AttributeEvent e = null;
		for (AttributeListener l : listeners) {
			if (e == null) e = new AttributeEvent(this, baseEvent.getAttribute(), baseEvent.getValue());
			l.attributeValueChanged(e);
		}
	}
}
