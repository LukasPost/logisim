/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;

import logisim.circuit.CircuitState;
import logisim.data.Value;

public interface Loggable {
	public Object[] getLogOptions(CircuitState state);

	public String getLogName(Object option);

	public Value getLogValue(CircuitState state, Object option);
}
