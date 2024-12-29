/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.appear;

import java.util.HashMap;
import java.util.Map;

import draw.actions.ModelAction;
import draw.model.CanvasObject;
import draw.undo.Action;
import logisim.circuit.Circuit;
import logisim.circuit.CircuitMutator;
import logisim.circuit.CircuitTransaction;
import logisim.circuit.appear.AppearanceElement;
import logisim.proj.Project;

public class CanvasActionAdapter extends logisim.proj.Action {
	private Circuit circuit;
	private Action canvasAction;
	private boolean wasDefault;
	
	public CanvasActionAdapter(Circuit circuit, Action action) {
		this.circuit = circuit;
		this.canvasAction = action;
	}

	@Override
	public String getName() {
		return canvasAction.getName();
	}

	@Override
	public void doIt(Project proj) {
		wasDefault = circuit.getAppearance().isDefaultAppearance();
		if (affectsPorts()) {
			ActionTransaction xn = new ActionTransaction(true);
			xn.execute();
		} else {
			canvasAction.doIt();
		}
	}

	@Override
	public void undo(Project proj) {
		if (affectsPorts()) {
			ActionTransaction xn = new ActionTransaction(false);
			xn.execute();
		} else {
			canvasAction.undo();
		}
		circuit.getAppearance().setDefaultAppearance(wasDefault);
	}
	
	private boolean affectsPorts() {
		if (canvasAction instanceof ModelAction) {
			for (CanvasObject o : ((ModelAction) canvasAction).getObjects()) {
				if (o instanceof AppearanceElement) {
					return true;
				}
			}
		}
		return false;
	}
	
	private class ActionTransaction extends CircuitTransaction {
		private boolean forward;
		
		ActionTransaction(boolean forward) {
			this.forward = forward;
		}
		
		@Override
		protected Map<Circuit, Integer> getAccessedCircuits() {
			Map<Circuit, Integer> accessMap = new HashMap<Circuit, Integer>();
			for (Circuit supercirc : circuit.getCircuitsUsingThis()) {
				accessMap.put(supercirc, READ_WRITE);
			}
			return accessMap;
		}

		@Override
		protected void run(CircuitMutator mutator) {
			if (forward) {
				canvasAction.doIt();
			} else {
				canvasAction.undo();
			}
		}
		
	}
}
