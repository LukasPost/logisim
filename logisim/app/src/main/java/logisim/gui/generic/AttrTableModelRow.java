/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.generic;

import java.awt.Component;
import java.awt.Window;

public interface AttrTableModelRow {
	String getLabel();

	String getValue();

	boolean isValueEditable();

	Component getEditor(Window parent);

	void setValue(Object value) throws AttrTableSetException;
}
