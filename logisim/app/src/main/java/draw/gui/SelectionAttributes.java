/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.gui;

import java.util.*;
import java.util.Map.Entry;

import draw.canvas.Selection;
import draw.canvas.SelectionEvent;
import draw.canvas.SelectionListener;
import draw.model.CanvasObject;
import logisim.data.AbstractAttributeSet;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;

public class SelectionAttributes extends AbstractAttributeSet {
	private class Listener implements SelectionListener, AttributeListener {
		//
		// SelectionListener
		//
		public void selectionChanged(SelectionEvent ex) {
			Map<AttributeSet, CanvasObject> oldSel = selected;
			Map<AttributeSet, CanvasObject> newSel = new HashMap<>();
			for (CanvasObject o : selection.getSelected()) newSel.put(o.getAttributeSet(), o);
			selected = newSel;
			boolean change = false;
			for (AttributeSet attrs : oldSel.keySet())
				if (!newSel.containsKey(attrs)) {
					change = true;
					attrs.removeAttributeListener(this);
				}
			for (AttributeSet attrs : newSel.keySet())
				if (!oldSel.containsKey(attrs)) {
					change = true;
					attrs.addAttributeListener(this);
				}
			if (change) {
				computeAttributeList(newSel.keySet());
				fireAttributeListChanged();
			}
		}

		private void computeAttributeList(Set<AttributeSet> attrsSet) {
			Set<Attribute<?>> attrSet = new LinkedHashSet<>();
			Iterator<AttributeSet> sit = attrsSet.iterator();
			if (sit.hasNext()) {
				AttributeSet first = sit.next();
				attrSet.addAll(first.getAttributes());
				while (sit.hasNext()) {
					AttributeSet next = sit.next();
					attrSet.removeIf(attr -> !next.containsAttribute(attr));
				}
			}

			Attribute<?>[] attrs = new Attribute[attrSet.size()];
			Object[] values = new Object[attrs.length];
			int i = 0;
			for (Attribute<?> attr : attrSet) {
				attrs[i] = attr;
				values[i] = getSelectionValue(attr, attrsSet);
				i++;
			}
			selAttrs = attrs;
			selValues = values;
			attrsView = List.of(attrs);
			fireAttributeListChanged();
		}

		//
		// AttributeSet listener
		//
		public void attributeListChanged(AttributeEvent e) {
			// show selection attributes
			computeAttributeList(selected.keySet());
		}

		public void attributeValueChanged(AttributeEvent e) {
			if (selected.containsKey(e.getSource())) {
				@SuppressWarnings("unchecked")
				Attribute<Object> attr = (Attribute<Object>) e.getAttribute();
				Attribute<?>[] attrs = selAttrs;
				Object[] values = selValues;
				for (int i = 0; i < attrs.length; i++)
					if (attrs[i] == attr) values[i] = getSelectionValue(attr, selected.keySet());
			}
		}
	}

	private Selection selection;
	private Listener listener;
	private Map<AttributeSet, CanvasObject> selected;
	private Attribute<?>[] selAttrs;
	private Object[] selValues;
	private List<Attribute<?>> attrsView;

	public SelectionAttributes(Selection selection) {
		this.selection = selection;
		listener = new Listener();
		selected = Collections.emptyMap();
		selAttrs = new Attribute<?>[0];
		selValues = new Object[0];
		attrsView = List.of(selAttrs);

		selection.addSelectionListener(listener);
		listener.selectionChanged(null);
	}

	public Iterable<Entry<AttributeSet, CanvasObject>> entries() {
		Set<Entry<AttributeSet, CanvasObject>> raw = selected.entrySet();
		return new ArrayList<>(raw);
	}

	//
	// AbstractAttributeSet methods
	//
	@Override
	protected void copyInto(AbstractAttributeSet dest) {
		listener = new Listener();
		selection.addSelectionListener(listener);
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return attrsView;
	}

	@Override
	public <V> V getValue(Attribute<V> attr) {
		Attribute<?>[] attrs = selAttrs;
		Object[] values = selValues;
		for (int i = 0; i < attrs.length; i++)
			if (attrs[i] == attr) {
				@SuppressWarnings("unchecked")
				V ret = (V) values[i];
				return ret;
			}
		return null;
	}

	@Override
	public <V> void setValue(Attribute<V> attr, V value) {
		Attribute<?>[] attrs = selAttrs;
		Object[] values = selValues;
		for (int i = 0; i < attrs.length; i++)
			if (attrs[i] == attr) {
				boolean same = Objects.equals(value, values[i]);
				if (!same) {
					values[i] = value;
					for (AttributeSet objAttrs : selected.keySet()) objAttrs.setValue(attr, value);
				}
				break;
			}
	}

	private static Object getSelectionValue(Attribute<?> attr, Set<AttributeSet> sel) {
		Object ret = null;
		for (AttributeSet attrs : sel)
			if (attrs.containsAttribute(attr)) {
				Object val = attrs.getValue(attr);
				if (ret == null) ret = val;
				else if (ret.equals(val)) ; // keep on, making sure everything else matches
				else return null;
			}
		return ret;
	}
}
