/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.model;

public interface TruthTableListener {
	void cellsChanged(TruthTableEvent event);

	void structureChanged(TruthTableEvent event);
}
