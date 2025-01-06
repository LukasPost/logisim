/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.undo;

import java.util.EventListener;

public interface UndoLogListener extends EventListener {
	void undoLogChanged(UndoLogEvent e);
}
