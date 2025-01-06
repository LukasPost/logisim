/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.menu;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

interface MenuItem {
	boolean hasListeners();

	void addActionListener(ActionListener l);

	void removeActionListener(ActionListener l);

	boolean isEnabled();

	void setEnabled(boolean value);

	void actionPerformed(ActionEvent event);
}
