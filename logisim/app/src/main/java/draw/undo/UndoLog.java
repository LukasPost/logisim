/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.undo;

import java.util.LinkedList;

import logisim.util.EventSourceWeakSupport;

public class UndoLog {
	private static final int MAX_UNDO_SIZE = 64;

	private EventSourceWeakSupport<UndoLogListener> listeners;
	private LinkedList<Action> undoLog;
	private LinkedList<Action> redoLog;
	private int modCount;

	public UndoLog() {
		listeners = new EventSourceWeakSupport<>();
		undoLog = new LinkedList<>();
		redoLog = new LinkedList<>();
		modCount = 0;
	}

	//
	// listening methods
	//
	public void addProjectListener(UndoLogListener what) {
		listeners.add(what);
	}

	public void removeProjectListener(UndoLogListener what) {
		listeners.remove(what);
	}

	private void fireEvent(int action, Action actionObject) {
		UndoLogEvent e = new UndoLogEvent(this, action, actionObject);
		listeners.forEach(l -> l.undoLogChanged(e));
	}

	//
	// accessor methods
	//
	public Action getUndoAction() {
		return undoLog.size() == 0 ? null : undoLog.getLast();
	}

	public Action getRedoAction() {
		return redoLog.size() == 0 ? null : redoLog.getLast();
	}

	public boolean isModified() {
		return modCount != 0;
	}

	//
	// mutator methods
	//
	public void doAction(Action act) {
		if (act == null)
			return;
		act.doIt();
		logAction(act);
	}

	public void logAction(Action act) {
		redoLog.clear();
		if (!undoLog.isEmpty()) {
			Action prev = undoLog.getLast();
			if (act.shouldAppendTo(prev)) {
				if (prev.isModification())
					--modCount;
				Action joined = prev.append(act);
				if (joined == null) {
					fireEvent(UndoLogEvent.ACTION_DONE, act);
					return;
				}
				act = joined;
			}
			while (undoLog.size() > MAX_UNDO_SIZE)
				undoLog.removeFirst();
		}
		undoLog.add(act);
		if (act.isModification())
			++modCount;
		fireEvent(UndoLogEvent.ACTION_DONE, act);
	}

	public void undoAction() {
		if (undoLog.isEmpty())
			return;
		Action action = undoLog.removeLast();
		if (action.isModification())
			--modCount;
		action.undo();
		redoLog.add(action);
		fireEvent(UndoLogEvent.ACTION_UNDONE, action);
	}

	public void redoAction() {
		if (redoLog.isEmpty())
			return;
		Action action = redoLog.removeLast();
		if (action.isModification())
			++modCount;
		action.doIt();
		undoLog.add(action);
		fireEvent(UndoLogEvent.ACTION_DONE, action);
	}

	public void clearModified() {
		modCount = 0;
	}
}
