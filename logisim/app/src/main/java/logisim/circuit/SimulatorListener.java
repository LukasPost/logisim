/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

public interface SimulatorListener {
	public void propagationCompleted(SimulatorEvent e);
	public void tickCompleted(SimulatorEvent e);
	public void simulatorStateChanged(SimulatorEvent e);
}
