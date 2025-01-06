/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import logisim.tools.Library;

interface LibraryLoader {
	Library loadLibrary(String desc);

	String getDescriptor(Library lib);

	void showError(String description);
}
