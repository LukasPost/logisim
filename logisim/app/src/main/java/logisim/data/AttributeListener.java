/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

public interface AttributeListener {
	void attributeListChanged(AttributeEvent e);

	void attributeValueChanged(AttributeEvent e);
}
