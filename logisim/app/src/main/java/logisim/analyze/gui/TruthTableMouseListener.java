/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.gui;

import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;

import logisim.analyze.model.Entry;
import logisim.analyze.model.TruthTable;

class TruthTableMouseListener implements MouseListener {
	private int cellX;
	private int cellY;
	private Entry newValue;

	public void mousePressed(MouseEvent event) {
		TruthTablePanel source = (TruthTablePanel) event.getSource();
		TruthTable model = source.getTruthTable();
		int cols = model.getInputColumnCount() + model.getOutputColumnCount();
		int rows = model.getRowCount();
		cellX = source.getOutputColumn(event);
		cellY = source.getRow(event);
		if (cellX < 0 || cellY < 0 || cellX >= cols || cellY >= rows)
			return;
		Entry oldValue = source.getTruthTable().getOutputEntry(cellY, cellX);
		if (oldValue == Entry.ZERO)
			newValue = Entry.ONE;
		else if (oldValue == Entry.ONE)
			newValue = Entry.DONT_CARE;
		else
			newValue = Entry.ZERO;
		source.setEntryProvisional(cellY, cellX, newValue);
	}

	public void mouseReleased(MouseEvent event) {
		TruthTablePanel source = (TruthTablePanel) event.getSource();
		TruthTable model = source.getTruthTable();
		int cols = model.getInputColumnCount() + model.getOutputColumnCount();
		int rows = model.getRowCount();
		if (cellX < 0 || cellY < 0 || cellX >= cols || cellY >= rows)
			return;

		int x = source.getOutputColumn(event);
		int y = source.getRow(event);
		TruthTable table = source.getTruthTable();
		if (x == cellX && y == cellY) table.setOutputEntry(y, x, newValue);
		source.setEntryProvisional(cellY, cellX, null);
		cellX = -1;
		cellY = -1;
	}

	public void mouseClicked(MouseEvent e) {
	}

	public void mouseEntered(MouseEvent e) {
	}

	public void mouseExited(MouseEvent e) {
	}
}
