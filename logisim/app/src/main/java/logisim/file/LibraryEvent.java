/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import logisim.tools.Library;

public class LibraryEvent {
	public static final int ADD_TOOL = 0;
	public static final int REMOVE_TOOL = 1;
	public static final int MOVE_TOOL = 2;
	public static final int ADD_LIBRARY = 3;
	public static final int REMOVE_LIBRARY = 4;
	public static final int SET_MAIN = 5;
	public static final int SET_NAME = 6;
	public static final int DIRTY_STATE = 7;

	private Library source;
	private int action;
	private Object data;

	LibraryEvent(Library source, int action, Object data) {
		this.source = source;
		this.action = action;
		this.data = data;
	}

	public Library getSource() {
		return source;
	}

	public int getAction() {
		return action;
	}

	public Object getData() {
		return data;
	}

}
