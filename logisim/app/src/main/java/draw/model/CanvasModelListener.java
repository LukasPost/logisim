/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import java.util.EventListener;

public interface CanvasModelListener extends EventListener {
	void modelChanged(CanvasModelEvent event);
}
