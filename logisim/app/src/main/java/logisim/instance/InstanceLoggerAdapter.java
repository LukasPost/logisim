/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.instance;

import logisim.circuit.CircuitState;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.gui.log.Loggable;

class InstanceLoggerAdapter implements Loggable {
	private InstanceComponent comp;
	private InstanceLogger logger;
	private InstanceStateImpl state;

	public InstanceLoggerAdapter(InstanceComponent comp, Class<? extends InstanceLogger> loggerClass) {
		try {
			this.comp = comp;
			logger = loggerClass.getConstructor().newInstance();
			state = new InstanceStateImpl(null, comp);
		}
		catch (Throwable t) {
			handleError(t, loggerClass);
			logger = null;
		}
	}

	private void handleError(Throwable t, Class<? extends InstanceLogger> loggerClass) {
		String className = loggerClass.getName();
		System.err.println("error while instantiating logger " + className // OK
				+ ": " + t.getClass().getName());
		String msg = t.getMessage();
		if (msg != null)
			System.err.println("  (" + msg + ")"); // OK
	}

	public Object[] getLogOptions(CircuitState circState) {
		if (logger != null) {
			updateState(circState);
			return logger.getLogOptions(state);
		} else return null;
	}

	public String getLogName(Object option) {
		if (logger != null) return logger.getLogName(state, option);
		else return null;
	}

	public WireValue getLogValue(CircuitState circuitState, Object option) {
		if (logger != null) {
			updateState(circuitState);
			return logger.getLogValue(state, option);
		} else return WireValues.UNKNOWN;
	}

	private void updateState(CircuitState circuitState) {
		if (state.getCircuitState() != circuitState) state.repurpose(circuitState, comp);
	}
}
