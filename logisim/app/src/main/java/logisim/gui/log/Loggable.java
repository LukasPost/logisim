/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;

import logisim.circuit.CircuitState;
import logisim.data.Value;

public interface Loggable {
	Object[] getLogOptions(CircuitState state);

	String getLogName(Object option);

	Value getLogValue(CircuitState state, Object option);
}
