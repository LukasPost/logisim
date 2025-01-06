/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import logisim.comp.ComponentUserEvent;

public interface ToolTipMaker {
	String getToolTip(ComponentUserEvent event);
}
