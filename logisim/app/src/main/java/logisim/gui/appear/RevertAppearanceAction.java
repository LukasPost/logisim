/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.appear;

import java.util.ArrayList;

import draw.model.CanvasObject;
import logisim.circuit.Circuit;
import logisim.circuit.appear.CircuitAppearance;
import logisim.proj.Action;
import logisim.proj.Project;

public class RevertAppearanceAction extends Action {
	private Circuit circuit;
	private ArrayList<CanvasObject> old;
	private boolean wasDefault;

	public RevertAppearanceAction(Circuit circuit) {
		this.circuit = circuit;
	}

	@Override
	public String getName() {
		return Strings.get("revertAppearanceAction");
	}

	@Override
	public void doIt(Project proj) {
		CircuitAppearance appear = circuit.getAppearance();
		wasDefault = appear.isDefaultAppearance();
		old = new ArrayList<>(appear.getObjectsFromBottom());
		appear.setDefaultAppearance(true);
	}

	@Override
	public void undo(Project proj) {
		CircuitAppearance appear = circuit.getAppearance();
		appear.setObjectsForce(old);
		appear.setDefaultAppearance(wasDefault);
	}
}
