/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;

import logisim.data.Value;

interface ModelListener {
	public void selectionChanged(ModelEvent event);
	public void entryAdded(ModelEvent event, Value[] values);
	public void filePropertyChanged(ModelEvent event);
}
