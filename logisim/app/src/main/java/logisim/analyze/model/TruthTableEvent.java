/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.model;

public class TruthTableEvent {
	private TruthTable source;
	private int column;
	private Object data;

	public TruthTableEvent(TruthTable source, VariableListEvent event) {
		this.source = source;
		data = event;
	}

	public TruthTableEvent(TruthTable source, int column) {
		this.source = source;
		this.column = column;
	}

	public int getColumn() {
		return column;
	}

	public TruthTable getSource() {
		return source;
	}

	public Object getData() {
		return data;
	}
}
