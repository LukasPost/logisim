/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.canvas;

import draw.undo.Action;

public interface ActionDispatcher {
	void doAction(Action action);
}
