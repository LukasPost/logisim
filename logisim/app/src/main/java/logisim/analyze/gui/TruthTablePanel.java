/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.gui;

import java.awt.Color;
import java.awt.event.MouseEvent;

import logisim.analyze.model.Entry;
import logisim.analyze.model.TruthTable;

interface TruthTablePanel {
	Color ERROR_COLOR = new Color(255, 128, 128);

	TruthTable getTruthTable();

	int getOutputColumn(MouseEvent event);

	int getRow(MouseEvent event);

	void setEntryProvisional(int row, int col, Entry value);
}
