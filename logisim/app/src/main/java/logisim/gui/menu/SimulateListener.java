/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.menu;

import logisim.circuit.CircuitState;
import logisim.circuit.Simulator;

public interface SimulateListener {
	void stateChangeRequested(Simulator sim, CircuitState state);
}
