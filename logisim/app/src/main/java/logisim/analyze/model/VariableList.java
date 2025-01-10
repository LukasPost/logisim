/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.model;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.NoSuchElementException;

public class VariableList extends ArrayList<String> {
	private ArrayList<VariableListListener> listeners = new ArrayList<>();
	private int maxSize;
	private List<String> dataView;

	public VariableList(int maxSize) {
		super(Math.max(maxSize,16));
		this.maxSize = maxSize;
		dataView = Collections.unmodifiableList(this);
	}

	//
	// listener methods
	//
	public void addVariableListListener(VariableListListener l) {
		listeners.add(l);
	}

	public void removeVariableListListener(VariableListListener l) {
		listeners.remove(l);
	}

	private void fireEvent(int type) {
		fireEvent(type, null, null);
	}

	private void fireEvent(int type, String variable) {
		fireEvent(type, variable, null);
	}

	private void fireEvent(int type, String variable, Object data) {
		if (listeners.size() == 0)
			return;
		VariableListEvent event = new VariableListEvent(this, type, variable, data);
		listeners.forEach(l->l.listChanged(event));
	}

	//
	// data methods
	//
	public int getMaximumSize() {
		return maxSize;
	}

	public List<String> getAll() {
		return dataView;
	}

	public boolean isFull() {
		return size() >= maxSize;
	}

	public void setAll(List<String> values) {
		if (values.size() > maxSize)
			throw new IllegalArgumentException("maximum size is " + maxSize);
		clear();
		addAll(values);
		fireEvent(VariableListEvent.ALL_REPLACED);
	}

	public boolean add(String name) {
		if (size() >= maxSize)
			throw new IllegalArgumentException("maximum size is " + maxSize);
		boolean ret = super.add(name);
		fireEvent(VariableListEvent.ADD, name);
		return ret;
	}

	public void remove(String name) {
		int index = indexOf(name);
		remove(index);
		fireEvent(VariableListEvent.REMOVE, name, index);
	}

	public void move(String name, int delta) {
		int index = indexOf(name);
		if (index < 0)
			throw new NoSuchElementException(name);
		int newIndex = index + delta;
		if (newIndex < 0)
			throw new IllegalArgumentException("cannot move index " + index + " by " + delta);
		if (newIndex > size() - 1)
			throw new IllegalArgumentException("cannot move index " + index + " by " + delta + ": size " + size());
		if (index == newIndex)
			return;
		remove(index);
		add(newIndex, name);
		fireEvent(VariableListEvent.MOVE, name, newIndex - index);
	}

	public void replace(String oldName, String newName) {
		int index = indexOf(oldName);
		set(index, newName);
		fireEvent(VariableListEvent.REPLACE, oldName, index);
	}

}
