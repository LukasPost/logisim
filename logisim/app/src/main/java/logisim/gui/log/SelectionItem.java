/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;

import logisim.circuit.Circuit;
import logisim.circuit.CircuitEvent;
import logisim.circuit.CircuitListener;
import logisim.circuit.CircuitState;
import logisim.circuit.SubcircuitFactory;
import logisim.comp.Component;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.StdAttr;

class SelectionItem implements AttributeListener, CircuitListener {
	private Model model;
	private Component[] path;
	private Component comp;
	private Object option;
	private int radix = 2;
	private String shortDescriptor;
	private String longDescriptor;

	public SelectionItem(Model model, Component[] path, Component comp, Object option) {
		this.model = model;
		this.path = path;
		this.comp = comp;
		this.option = option;
		computeDescriptors();

		if (path != null) {
			model.getCircuitState().getCircuit().addCircuitListener(this);
			for (Component component : path) {
				component.getAttributeSet().addAttributeListener(this);
				SubcircuitFactory circFact = (SubcircuitFactory) component.getFactory();
				circFact.getSubcircuit().addCircuitListener(this);
			}
		}
		comp.getAttributeSet().addAttributeListener(this);
	}

	private boolean computeDescriptors() {
		boolean changed = false;

		Loggable log = (Loggable) comp.getFeature(Loggable.class);
		String newShort = log.getLogName(option);
		if (newShort == null || newShort.isEmpty()) {
			newShort = comp.getFactory().getDisplayName() + comp.getLocation().toString();
			if (option != null) newShort += "." + option;
		}
		if (!newShort.equals(shortDescriptor)) {
			changed = true;
			shortDescriptor = newShort;
		}

		StringBuilder buf = new StringBuilder();
		for (int i = 0; i < path.length; i++) {
			if (i > 0)
				buf.append(".");
			String label = path[i].getAttributeSet().getValue(StdAttr.LABEL);
			if (label != null && !label.isEmpty()) buf.append(label);
			else {
				buf.append(path[i].getFactory().getDisplayName());
				buf.append(path[i].getLocation());
			}
			buf.append(".");
		}
		buf.append(shortDescriptor);
		String newLong = buf.toString();
		if (!newLong.equals(longDescriptor)) {
			changed = true;
			longDescriptor = newLong;
		}

		return changed;
	}

	public Component[] getPath() {
		return path;
	}

	public Component getComponent() {
		return comp;
	}

	public Object getOption() {
		return option;
	}

	public int getRadix() {
		return radix;
	}

	public void setRadix(int value) {
		radix = value;
		model.fireSelectionChanged(new ModelEvent());
	}

	public String toShortString() {
		return shortDescriptor;
	}

	@Override
	public String toString() {
		return longDescriptor;
	}

	public WireValue fetchValue(CircuitState root) {
		CircuitState cur = root;
		for (Component component : path) {
			SubcircuitFactory circFact = (SubcircuitFactory) component.getFactory();
			cur = circFact.getSubstate(cur, component);
		}
		Loggable log = (Loggable) comp.getFeature(Loggable.class);
		return log == null ? WireValues.NIL : log.getLogValue(cur, option);
	}

	public void attributeListChanged(AttributeEvent e) {
	}

	public void attributeValueChanged(AttributeEvent e) {
		if (computeDescriptors()) model.fireSelectionChanged(new ModelEvent());
	}

	public void circuitChanged(CircuitEvent event) {
		int action = event.getAction();
		if (action == CircuitEvent.ACTION_CLEAR || action == CircuitEvent.ACTION_REMOVE) {
			Circuit circ = event.getCircuit();
			Component circComp = null;
			if (circ == model.getCircuitState().getCircuit())
				circComp = path != null && path.length > 0 ? path[0] : comp;
			else if (path != null) for (int i = 0; i < path.length; i++) {
				SubcircuitFactory circFact = (SubcircuitFactory) path[i].getFactory();
				if (circ == circFact.getSubcircuit()) circComp = i + 1 < path.length ? path[i + 1] : comp;
			}
			if (circComp == null)
				return;

			if (action == CircuitEvent.ACTION_REMOVE && event.getData() != circComp) return;

			int index = model.getSelection().indexOf(this);
			if (index < 0)
				return;
			model.getSelection().remove(index);
		}
	}
}
