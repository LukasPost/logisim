/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.generic;

public interface AttrTableModel {
	void addAttrTableModelListener(AttrTableModelListener listener);

	void removeAttrTableModelListener(AttrTableModelListener listener);

	String getTitle();

	int getRowCount();

	AttrTableModelRow getRow(int rowIndex);
}
