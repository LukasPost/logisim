/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import javax.swing.JPopupMenu;

import logisim.proj.Project;

public interface MenuExtender {
	public void configureMenu(JPopupMenu menu, Project proj);
}
