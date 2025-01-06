/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;

import java.io.File;
import java.util.HashMap;
import java.util.Objects;

import logisim.circuit.CircuitState;
import logisim.data.Value;
import logisim.util.EventSourceWeakSupport;

class Model {
	private EventSourceWeakSupport<ModelListener> listeners;
	private Selection selection;
	private HashMap<SelectionItem, ValueLog> log;
	private boolean fileEnabled;
	private File file;
	private boolean fileHeader = true;
	private boolean selected;
	private LogThread logger;

	public Model(CircuitState circuitState) {
		listeners = new EventSourceWeakSupport<>();
		selection = new Selection(circuitState, this);
		log = new HashMap<>();
	}

	public boolean isSelected() {
		return selected;
	}

	public void addModelListener(ModelListener l) {
		listeners.add(l);
	}

	public void removeModelListener(ModelListener l) {
		listeners.remove(l);
	}

	public CircuitState getCircuitState() {
		return selection.getCircuitState();
	}

	public Selection getSelection() {
		return selection;
	}

	public ValueLog getValueLog(SelectionItem item) {
		ValueLog ret = log.get(item);
		if (ret == null && selection.indexOf(item) >= 0) {
			ret = new ValueLog();
			log.put(item, ret);
		}
		return ret;
	}

	public boolean isFileEnabled() {
		return fileEnabled;
	}

	public File getFile() {
		return file;
	}

	public boolean getFileHeader() {
		return fileHeader;
	}

	public void setFileEnabled(boolean value) {
		if (fileEnabled == value)
			return;
		fileEnabled = value;
		fireFilePropertyChanged(new ModelEvent());
	}

	public void setFile(File value) {
		if (Objects.equals(file, value))
			return;
		file = value;
		fileEnabled = file != null;
		fireFilePropertyChanged(new ModelEvent());
	}

	public void setFileHeader(boolean value) {
		if (fileHeader == value)
			return;
		fileHeader = value;
		fireFilePropertyChanged(new ModelEvent());
	}

	public void propagationCompleted() {
		CircuitState circuitState = getCircuitState();
		Value[] vals = new Value[selection.size()];
		boolean changed = false;
		for (int i = selection.size() - 1; i >= 0; i--) {
			SelectionItem item = selection.get(i);
			vals[i] = item.fetchValue(circuitState);
			if (!changed) {
				Value v = getValueLog(item).getLast();
				changed = !Objects.equals(v, vals[i]);
			}
		}
		if (changed) {
			for (int i = selection.size() - 1; i >= 0; i--) {
				SelectionItem item = selection.get(i);
				getValueLog(item).append(vals[i]);
			}
			fireEntryAdded(new ModelEvent(), vals);
		}
	}

	public void setSelected(boolean value) {
		if (selected == value)
			return;
		selected = value;
		if (selected) {
			logger = new LogThread(this);
			logger.start();
		} else {
			if (logger != null)
				logger.cancel();
			logger = null;
			fileEnabled = false;
		}
		fireFilePropertyChanged(new ModelEvent());
	}

	void fireSelectionChanged(ModelEvent e) {
		log.keySet().removeIf(i -> selection.indexOf(i) < 0);

		for (ModelListener l : listeners) l.selectionChanged(e);
	}

	private void fireEntryAdded(ModelEvent e, Value[] values) {
		for (ModelListener l : listeners) l.entryAdded(e, values);
	}

	private void fireFilePropertyChanged(ModelEvent e) {
		for (ModelListener l : listeners) l.filePropertyChanged(e);
	}
}
