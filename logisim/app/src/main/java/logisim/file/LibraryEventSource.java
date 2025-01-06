/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

public interface LibraryEventSource {
	void addLibraryListener(LibraryListener listener);

	void removeLibraryListener(LibraryListener listener);
}
