/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.toolbar;

public interface ToolbarModelListener {
	void toolbarContentsChanged(ToolbarModelEvent event);

	void toolbarAppearanceChanged(ToolbarModelEvent event);
}
