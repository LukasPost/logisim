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
			for (CanvasObject o : selection.getSelected())
				newSel.put(o.getAttributeSet(), o);
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
			if (attrsSet.isEmpty())
				return;

			Set<Attribute<?>> attrSet = new LinkedHashSet<>(attrsSet.stream().findFirst().get().getAttributes());
			attrsSet.stream().skip(1)
					.forEach(duplicate ->
							attrSet.removeIf(attr -> !duplicate.containsAttribute(attr)));

			selAttrs = attrSet.stream().map(attr -> new AttributeWithValue(attr, getSelectionValue(attr, attrsSet))).toArray(AttributeWithValue[]::new);
			attrsView = new ArrayList<>(attrSet);
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
			if (!selected.containsKey(e.getSource()))
				return;
			Attribute<?> attr = e.getAttribute();
			for (AttributeWithValue selAttr : selAttrs)
				if (selAttr.attr == attr)
					selAttr.value = getSelectionValue(attr, selected.keySet());
		}
	}

	public static class AttributeWithValue {
		public Attribute<?> attr;
		public Object value;
		public AttributeWithValue(Attribute<?> attr, Object value) {
			this.attr = attr;
			this.value = value;
		}
		public AttributeWithValue() {}
	}

	private Selection selection;
	private Listener listener;
	private Map<AttributeSet, CanvasObject> selected;
	private AttributeWithValue[] selAttrs;
	private List<Attribute<?>> attrsView;

	public SelectionAttributes(Selection selection) {
		this.selection = selection;
		listener = new Listener();
		selected = Collections.emptyMap();
		selAttrs = new AttributeWithValue[0];
		attrsView = new ArrayList<>(0);

		selection.addSelectionListener(listener);
		listener.selectionChanged(null);
	}

	public Iterable<Entry<AttributeSet, CanvasObject>> entries() {
		return new ArrayList<>(selected.entrySet());
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
		return  Arrays.stream(selAttrs)
				.filter(awv -> awv.attr.equals(attr))
				.findFirst()
				.map(awv->(V) awv.value)
				.orElse(null);
	}

	@Override
	public <V> void setValue(Attribute<V> attr, V value) {
		var different = Arrays.stream(selAttrs)
				.filter(awv -> awv.attr.equals(attr) && !Objects.equals(value, awv.value))
				.findFirst()
				.orElse(null);
		if(different == null)
			return;
		different.value = value;
		for (AttributeSet objAttrs : selected.keySet())
			objAttrs.setValue(attr, value);
	}

	private static <V> V getSelectionValue(Attribute<V> attr, Set<AttributeSet> sel) {
		V ret = null;
		for (AttributeSet attrs : sel)
			if (attrs.containsAttribute(attr)) {
				V val = attrs.getValue(attr);
				if (ret == null) ret = val;
				else if (ret.equals(val)) ; // keep on, making sure everything else matches
				else return null;
			}
		return ret;
	}
}
