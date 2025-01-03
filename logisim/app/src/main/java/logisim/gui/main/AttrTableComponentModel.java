/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import logisim.circuit.Circuit;
import logisim.comp.Component;
import logisim.data.Attribute;
import logisim.gui.generic.AttrTableSetException;
import logisim.gui.generic.AttributeSetTableModel;
import logisim.proj.Project;
import logisim.tools.SetAttributeAction;

class AttrTableComponentModel extends AttributeSetTableModel {
	Project proj;
	Circuit circ;
	Component comp;

	AttrTableComponentModel(Project proj, Circuit circ, Component comp) {
		super(comp.getAttributeSet());
		this.proj = proj;
		this.circ = circ;
		this.comp = comp;
	}

	public Circuit getCircuit() {
		return circ;
	}

	public Component getComponent() {
		return comp;
	}

	@Override
	public String getTitle() {
		return comp.getFactory().getDisplayName();
	}

	@Override
	public void setValueRequested(Attribute<Object> attr, Object value) throws AttrTableSetException {
		if (!proj.getLogisimFile().contains(circ)) {
			String msg = Strings.get("cannotModifyCircuitError");
			throw new AttrTableSetException(msg);
		} else {
			SetAttributeAction act = new SetAttributeAction(circ, Strings.getter("changeAttributeAction"));
			act.set(comp, attr, value);
			proj.doAction(act);
		}
	}
}
