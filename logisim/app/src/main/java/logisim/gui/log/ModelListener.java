/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;

import logisim.data.WireValue.WireValue;

interface ModelListener {
	void selectionChanged(ModelEvent event);

	void entryAdded(ModelEvent event, WireValue[] values);

	void filePropertyChanged(ModelEvent event);
}
