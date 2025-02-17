/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit.appear;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import logisim.circuit.ReplacementMap;
import logisim.comp.Component;
import logisim.comp.ComponentEvent;
import logisim.comp.ComponentListener;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.instance.Instance;
import logisim.instance.StdAttr;
import logisim.std.wiring.Pin;

public class CircuitPins {
	private class MyComponentListener implements ComponentListener, AttributeListener {
		public void endChanged(ComponentEvent e) {
			appearanceManager.updatePorts();
		}

		public void componentInvalidated(ComponentEvent e) {
		}

		public void attributeListChanged(AttributeEvent e) {
		}

		public void attributeValueChanged(AttributeEvent e) {
			Attribute<?> attr = e.getAttribute();
			if (attr == StdAttr.FACING || attr == StdAttr.LABEL || attr == Pin.ATTR_TYPE)
				appearanceManager.updatePorts();
		}
	}

	private PortManager appearanceManager;
	private MyComponentListener myComponentListener;
	private Set<Instance> pins;

	CircuitPins(PortManager appearanceManager) {
		this.appearanceManager = appearanceManager;
		myComponentListener = new MyComponentListener();
		pins = new HashSet<>();
	}

	public void transactionCompleted(ReplacementMap repl) {
		// determine the changes
		Set<Instance> adds = new HashSet<>();
		Set<Instance> removes = new HashSet<>();
		Map<Instance, Instance> replaces = new HashMap<>();
		for (Component comp : repl.getAdditions())
			if (comp.getFactory() instanceof Pin) {
				Instance in = Instance.getInstanceFor(comp);
				boolean added = pins.add(in);
				if (added) {
					comp.addComponentListener(myComponentListener);
					in.getAttributeSet().addAttributeListener(myComponentListener);
					adds.add(in);
				}
			}
		for (Component comp : repl.getRemovals())
			if (comp.getFactory() instanceof Pin) {
				Instance in = Instance.getInstanceFor(comp);
				boolean removed = pins.remove(in);
				if (removed) {
					comp.removeComponentListener(myComponentListener);
					in.getAttributeSet().removeAttributeListener(myComponentListener);
					Collection<Component> rs = repl.getComponentsReplacing(comp);
					if (rs.isEmpty())
						removes.add(in);
					else {
						Component r = rs.iterator().next();
						Instance rin = Instance.getInstanceFor(r);
						adds.remove(rin);
						replaces.put(in, rin);
					}
				}
			}

		appearanceManager.updatePorts(adds, removes, replaces, getPins());
	}

	public Collection<Instance> getPins() {
		return new ArrayList<>(pins);
	}
}
