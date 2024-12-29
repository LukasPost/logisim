/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.model;

public interface TruthTableListener {
	public void cellsChanged(TruthTableEvent event);

	public void structureChanged(TruthTableEvent event);
}
