/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import logisim.proj.Action;
import logisim.proj.Project;
import logisim.util.StringGetter;

public class CircuitAction extends Action {
	private StringGetter name;
	private CircuitTransaction forward;
	private CircuitTransaction reverse;

	CircuitAction(StringGetter name, CircuitMutation forward) {
		this.name = name;
		this.forward = forward;
	}

	@Override
	public String getName() {
		return name.get();
	}

	@Override
	public void doIt(Project proj) {
		CircuitTransactionResult result = forward.execute();
		if (result != null) reverse = result.getReverseTransaction();
	}

	@Override
	public void undo(Project proj) {
		if (reverse != null) reverse.execute();
	}
}
