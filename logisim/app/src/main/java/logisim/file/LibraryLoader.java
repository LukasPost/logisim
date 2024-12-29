/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import logisim.tools.Library;

interface LibraryLoader {
	public Library loadLibrary(String desc);

	public String getDescriptor(Library lib);

	public void showError(String description);
}
