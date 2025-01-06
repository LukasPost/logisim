/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.toolbar;

import java.util.List;

public interface ToolbarModel {
	void addToolbarModelListener(ToolbarModelListener listener);

	void removeToolbarModelListener(ToolbarModelListener listener);

	List<ToolbarItem> getItems();

	boolean isSelected(ToolbarItem item);

	void itemSelected(ToolbarItem item);
}
