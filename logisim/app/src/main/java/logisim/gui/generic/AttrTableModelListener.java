/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.generic;

public interface AttrTableModelListener {
	void attrTitleChanged(AttrTableModelEvent event);

	void attrStructureChanged(AttrTableModelEvent event);

	void attrValueChanged(AttrTableModelEvent event);
}
