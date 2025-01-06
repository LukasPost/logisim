/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.proj;

import logisim.circuit.Circuit;
import logisim.circuit.CircuitEvent;
import logisim.circuit.CircuitListener;
import logisim.circuit.SubcircuitFactory;
import logisim.comp.Component;
import logisim.comp.ComponentFactory;
import logisim.file.LibraryEvent;
import logisim.file.LibraryListener;
import logisim.file.LogisimFile;
import logisim.tools.AddTool;
import logisim.util.Dag;

public class Dependencies {
	private class MyListener implements LibraryListener, CircuitListener {
		public void libraryChanged(LibraryEvent e) {
			switch (e.getAction()) {
			case LibraryEvent.ADD_TOOL:
				if (e.getData() instanceof AddTool) {
					ComponentFactory factory = ((AddTool) e.getData()).getFactory();
					if (factory instanceof SubcircuitFactory circFact) processCircuit(circFact.getSubcircuit());
				}
				break;
			case LibraryEvent.REMOVE_TOOL:
				if (e.getData() instanceof AddTool) {
					ComponentFactory factory = ((AddTool) e.getData()).getFactory();
					if (factory instanceof SubcircuitFactory circFact) {
						Circuit circ = circFact.getSubcircuit();
						depends.removeNode(circ);
						circ.removeCircuitListener(this);
					}
				}
				break;
			}
		}

		public void circuitChanged(CircuitEvent e) {
			Component comp;
			switch (e.getAction()) {
			case CircuitEvent.ACTION_ADD:
				comp = (Component) e.getData();
				if (comp.getFactory() instanceof SubcircuitFactory factory)
					depends.addEdge(e.getCircuit(), factory.getSubcircuit());
				break;
			case CircuitEvent.ACTION_REMOVE:
				comp = (Component) e.getData();
				if (comp.getFactory() instanceof SubcircuitFactory factory) {
					boolean found = false;
					for (Component o : e.getCircuit().getNonWires())
						if (o.getFactory() == factory) {
							found = true;
							break;
						}
					if (!found)
						depends.removeEdge(e.getCircuit(), factory.getSubcircuit());
				}
				break;
			case CircuitEvent.ACTION_CLEAR:
				depends.removeNode(e.getCircuit());
				break;
			}
		}
	}

	private MyListener myListener = new MyListener();
	private Dag<Circuit> depends = new Dag<>();

	Dependencies(LogisimFile file) {
		addDependencies(file);
	}

	public boolean canRemove(Circuit circ) {
		return !depends.hasPredecessors(circ);
	}

	public boolean canAdd(Circuit circ, Circuit sub) {
		return depends.canFollow(sub, circ);
	}

	private void addDependencies(LogisimFile file) {
		file.addLibraryListener(myListener);
		for (Circuit circuit : file.getCircuits()) processCircuit(circuit);
	}

	private void processCircuit(Circuit circ) {
		circ.addCircuitListener(myListener);
		for (Component comp : circ.getNonWires())
			if (comp.getFactory() instanceof SubcircuitFactory factory) depends.addEdge(circ, factory.getSubcircuit());
	}

}
