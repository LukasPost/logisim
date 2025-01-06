/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.proj;

import logisim.circuit.Circuit;
import logisim.file.LogisimFile;
import logisim.tools.Tool;

public class ProjectEvent {
	public static final int ACTION_SET_FILE = 0; // change file
	public static final int ACTION_SET_CURRENT = 1; // change current
	public static final int ACTION_SET_TOOL = 2; // change tool
	public static final int ACTION_SELECTION = 3; // selection alterd
	public static final int ACTION_SET_STATE = 4; // circuit state changed
	public static final int ACTION_START = 5; // action about to start
	public static final int ACTION_COMPLETE = 6; // action has completed
	public static final int ACTION_MERGE = 7; // one action has been appended to another
	public static final int UNDO_START = 8; // undo about to start
	public static final int UNDO_COMPLETE = 9; // undo has completed
	public static final int REPAINT_REQUEST = 10; // canvas should be repainted

	private int action;
	private Project proj;
	private Object old_data;
	private Object data;

	ProjectEvent(int action, Project proj, Object old, Object data) {
		this.action = action;
		this.proj = proj;
		old_data = old;
		this.data = data;
	}

	ProjectEvent(int action, Project proj, Object data) {
		this.action = action;
		this.proj = proj;
		this.data = data;
	}

	// access methods
	public int getAction() {
		return action;
	}

	public Project getProject() {
		return proj;
	}

	public Object getOldData() {
		return old_data;
	}

	public Object getData() {
		return data;
	}

	// convenience methods
	public LogisimFile getLogisimFile() {
		return proj.getLogisimFile();
	}

	public Circuit getCircuit() {
		return proj.getCurrentCircuit();
	}

	public Tool getTool() {
		return proj.getTool();
	}

}
