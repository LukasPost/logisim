/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

public interface SimulatorListener {
	void propagationCompleted(SimulatorEvent e);

	void tickCompleted(SimulatorEvent e);

	void simulatorStateChanged(SimulatorEvent e);
}
