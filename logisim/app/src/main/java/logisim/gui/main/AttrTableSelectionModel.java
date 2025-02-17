/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import logisim.circuit.Circuit;
import logisim.circuit.Wire;
import logisim.comp.Component;
import logisim.comp.ComponentFactory;
import logisim.data.Attribute;
import logisim.gui.generic.AttrTableSetException;
import logisim.gui.generic.AttributeSetTableModel;
import logisim.gui.main.Selection.Event;
import logisim.gui.main.Selection.Listener;
import logisim.proj.Project;
import logisim.tools.SetAttributeAction;

class AttrTableSelectionModel extends AttributeSetTableModel implements Listener {
	private Project project;
	private Frame frame;

	public AttrTableSelectionModel(Project project, Frame frame) {
		super(frame.getCanvas().getSelection().getAttributeSet());
		this.project = project;
		this.frame = frame;
		frame.getCanvas().getSelection().addListener(this);
	}

	@SuppressWarnings("null")
	@Override
	public String getTitle() {
		ComponentFactory wireFactory = null;
		ComponentFactory factory = null;
		int factoryCount = 0;
		int totalCount = 0;
		boolean variousFound = false;

		Selection selection = frame.getCanvas().getSelection();
		for (Component comp : selection.getComponents()) {
			ComponentFactory fact = comp.getFactory();
			if (fact == factory) factoryCount++;
			else if (comp instanceof Wire) {
				wireFactory = fact;
				if (factory == null) factoryCount++;
			} else if (factory == null) {
				factory = fact;
				factoryCount = 1;
			} else variousFound = true;
			if (!(comp instanceof Wire)) totalCount++;
		}

		if (factory == null) factory = wireFactory;

		if (variousFound) return Strings.get("selectionVarious", "" + totalCount);
		else if (factoryCount == 0) {
			String circName = frame.getCanvas().getCircuit().getName();
			return Strings.get("circuitAttrTitle", circName);
		} else if (factoryCount == 1) return Strings.get("selectionOne", factory.getDisplayName());
		else return Strings.get("selectionMultiple", factory.getDisplayName(), "" + factoryCount);
	}

	@Override
	public void setValueRequested(Attribute<Object> attr, Object value) throws AttrTableSetException {
		Selection selection = frame.getCanvas().getSelection();
		Circuit circuit = frame.getCanvas().getCircuit();
		if (selection.isEmpty() && circuit != null) {
			AttrTableCircuitModel circuitModel = new AttrTableCircuitModel(project, circuit);
			circuitModel.setValueRequested(attr, value);
		} else {
			SetAttributeAction act = new SetAttributeAction(circuit, Strings.getter("selectionAttributeAction"));
			for (Component comp : selection.getComponents()) if (!(comp instanceof Wire)) act.set(comp, attr, value);
			project.doAction(act);
		}
	}

	//
	// Selection.Listener methods
	public void selectionChanged(Event event) {
		fireTitleChanged();
		frame.setAttrTableModel(this);
	}
}
