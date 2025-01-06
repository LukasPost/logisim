/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

public interface CaretListener {
	void editingCanceled(CaretEvent e);

	void editingStopped(CaretEvent e);
}
