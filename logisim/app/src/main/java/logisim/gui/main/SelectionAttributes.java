/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.util.*;
import java.util.Map.Entry;

import logisim.circuit.Circuit;
import logisim.circuit.Wire;
import logisim.comp.Component;
import logisim.data.AbstractAttributeSet;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;
import logisim.gui.main.Selection.Event;
import logisim.proj.Project;
import logisim.util.UnmodifiableList;

class SelectionAttributes extends AbstractAttributeSet {
	private static final Attribute<?>[] EMPTY_ATTRIBUTES = new Attribute<?>[0];
	private static final Object[] EMPTY_VALUES = new Object[0];

	private class Listener implements Selection.Listener, AttributeListener {
		public void selectionChanged(Event e) {
			updateList(true);
		}

		public void attributeListChanged(AttributeEvent e) {
			if (listening) updateList(false);
		}

		public void attributeValueChanged(AttributeEvent e) {
			if (listening) updateList(false);
		}
	}

	private Canvas canvas;
	private Selection selection;
	private Listener listener;
	private boolean listening;
	private Set<Component> selected;
	private Attribute<?>[] attrs;
	private boolean[] readOnly;
	private Object[] values;
	private List<Attribute<?>> attrsView;

	public SelectionAttributes(Canvas canvas, Selection selection) {
		this.canvas = canvas;
		this.selection = selection;
		listener = new Listener();
		listening = true;
		selected = Collections.emptySet();
		attrs = EMPTY_ATTRIBUTES;
		values = EMPTY_VALUES;
		attrsView = Collections.emptyList();

		selection.addListener(listener);
		updateList(true);
		setListening(true);
	}

	public Selection getSelection() {
		return selection;
	}

	void setListening(boolean value) {
		if (listening != value) {
			listening = value;
			if (value) updateList(false);
		}
	}

	@SuppressWarnings("null")
	private void updateList(boolean ignoreIfSelectionSame) {
		Selection sel = selection;
		Set<Component> oldSel = selected;
		Set<Component> newSel;
		if (sel == null)
			newSel = Collections.emptySet();
		else
			newSel = createSet(sel.getComponents());
		if (haveSameElements(newSel, oldSel)) {
			if (ignoreIfSelectionSame)
				return;
			newSel = oldSel;
		} else {
			for (Component o : oldSel) if (!newSel.contains(o)) o.getAttributeSet().removeAttributeListener(listener);
			for (Component o : newSel) if (!oldSel.contains(o)) o.getAttributeSet().addAttributeListener(listener);
		}

		LinkedHashMap<Attribute<Object>, Object> attrMap = computeAttributes(newSel);
		boolean same = isSame(attrMap, attrs, values);

		if (same) {
			if (newSel != oldSel)
				selected = newSel;
		} else {
			Attribute<?>[] oldAttrs = attrs;
			Object[] oldValues = values;
			Attribute<?>[] newAttrs = new Attribute[attrMap.size()];
			Object[] newValues = new Object[newAttrs.length];
			boolean[] newReadOnly = new boolean[newAttrs.length];
			int i = -1;
			for (Entry<Attribute<Object>, Object> entry : attrMap.entrySet()) {
				i++;
				newAttrs[i] = entry.getKey();
				newValues[i] = entry.getValue();
				newReadOnly[i] = computeReadOnly(newSel, newAttrs[i]);
			}
			if (newSel != oldSel)
				selected = newSel;
			attrs = newAttrs;
			attrsView = new UnmodifiableList<>(newAttrs);
			values = newValues;
			readOnly = newReadOnly;

			boolean listSame = oldAttrs != null && oldAttrs.length == newAttrs.length;
			if (listSame) for (i = 0; i < oldAttrs.length; i++)
				if (!oldAttrs[i].equals(newAttrs[i])) {
					listSame = false;
					break;
				}

			if (listSame) for (i = 0; i < oldValues.length; i++) {
				Object oldVal = oldValues[i];
				Object newVal = newValues[i];
				boolean sameVals = Objects.equals(oldVal, newVal);
				if (!sameVals) {
					@SuppressWarnings("unchecked")
					Attribute<Object> attr = (Attribute<Object>) oldAttrs[i];
					fireAttributeValueChanged(attr, newVal);
				}
			}
			else fireAttributeListChanged();
		}
	}

	private static Set<Component> createSet(Collection<Component> comps) {
		boolean includeWires = true;
		for (Component comp : comps)
			if (!(comp instanceof Wire)) {
				includeWires = false;
				break;
			}

		if (includeWires) return new HashSet<>(comps);
		else {
			HashSet<Component> ret = new HashSet<>();
			for (Component comp : comps)
				if (!(comp instanceof Wire))
					ret.add(comp);
			return ret;
		}
	}

	private static boolean haveSameElements(Collection<Component> a, Collection<Component> b) {
		if (a == null) return b == null || b.isEmpty();
		else if (b == null) return a.isEmpty();
		else if (a.size() != b.size()) return false;
		else {
			for (Component item : a)
				if (!b.contains(item))
					return false;
			return true;
		}
	}

	private static LinkedHashMap<Attribute<Object>, Object> computeAttributes(Collection<Component> newSel) {
		LinkedHashMap<Attribute<Object>, Object> attrMap = new LinkedHashMap<>();
		Iterator<Component> sit = newSel.iterator();
		if (sit.hasNext()) {
			AttributeSet first = sit.next().getAttributeSet();
			for (Attribute<?> attr : first.getAttributes()) {
				@SuppressWarnings("unchecked")
				Attribute<Object> attrObj = (Attribute<Object>) attr;
				attrMap.put(attrObj, first.getValue(attr));
			}
			while (sit.hasNext()) {
				AttributeSet next = sit.next().getAttributeSet();
				Iterator<Attribute<Object>> ait = attrMap.keySet().iterator();
				while (ait.hasNext()) {
					Attribute<Object> attr = ait.next();
					if (next.containsAttribute(attr)) {
						Object v = attrMap.get(attr);
						if (v != null && !v.equals(next.getValue(attr))) attrMap.put(attr, null);
					} else ait.remove();
				}
			}
		}
		return attrMap;
	}

	private static boolean isSame(LinkedHashMap<Attribute<Object>, Object> attrMap, Attribute<?>[] oldAttrs,
			Object[] oldValues) {
		if (oldAttrs.length != attrMap.size()) return false;
		else {
			int j = -1;
			for (Entry<Attribute<Object>, Object> entry : attrMap.entrySet()) {
				j++;

				Attribute<Object> a = entry.getKey();
				if (!oldAttrs[j].equals(a) || j >= oldValues.length)
					return false;
				Object ov = oldValues[j];
				Object nv = entry.getValue();
				if (!Objects.equals(ov, nv))
					return false;
			}
			return true;
		}
	}

	private static boolean computeReadOnly(Collection<Component> sel, Attribute<?> attr) {
		for (Component comp : sel) {
			AttributeSet attrs = comp.getAttributeSet();
			if (attrs.isReadOnly(attr))
				return true;
		}
		return false;
	}

	@Override
	protected void copyInto(AbstractAttributeSet dest) {
		throw new UnsupportedOperationException("SelectionAttributes.copyInto");
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		Circuit circ = canvas.getCircuit();
		if (selected.isEmpty() && circ != null) return circ.getStaticAttributes().getAttributes();
		else return attrsView;
	}

	@Override
	public boolean isReadOnly(Attribute<?> attr) {
		Project proj = canvas.getProject();
		Circuit circ = canvas.getCircuit();
		if (!proj.getLogisimFile().contains(circ)) return true;
		else if (selected.isEmpty() && circ != null) return circ.getStaticAttributes().isReadOnly(attr);
		else {
			int i = findIndex(attr);
			boolean[] ro = readOnly;
			return i < 0 || i >= ro.length || ro[i];
		}
	}

	@Override
	public boolean isToSave(Attribute<?> attr) {
		return false;
	}

	@Override
	public <V> V getValue(Attribute<V> attr) {
		Circuit circ = canvas.getCircuit();
		if (selected.isEmpty() && circ != null) return circ.getStaticAttributes().getValue(attr);
		else {
			int i = findIndex(attr);
			Object[] vs = values;
			@SuppressWarnings("unchecked")
			V ret = (V) (i >= 0 && i < vs.length ? vs[i] : null);
			return ret;
		}
	}

	@Override
	public <V> void setValue(Attribute<V> attr, V value) {
		Circuit circ = canvas.getCircuit();
		if (selected.isEmpty() && circ != null) circ.getStaticAttributes().setValue(attr, value);
		else {
			int i = findIndex(attr);
			Object[] vs = values;
			if (i >= 0 && i < vs.length) {
				vs[i] = value;
				for (Component comp : selected) comp.getAttributeSet().setValue(attr, value);
			}
		}
	}

	private int findIndex(Attribute<?> attr) {
		if (attr == null)
			return -1;
		Attribute<?>[] as = attrs;
		for (int i = 0; i < as.length; i++)
			if (attr.equals(as[i]))
				return i;
		return -1;
	}
}
