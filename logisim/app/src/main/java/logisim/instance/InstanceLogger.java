/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.instance;

import logisim.data.WireValue.WireValue;

public abstract class InstanceLogger {
	public Object[] getLogOptions(InstanceState state) {
		return null;
	}

	public abstract String getLogName(InstanceState state, Object option);

	public abstract WireValue getLogValue(InstanceState state, Object option);
}
