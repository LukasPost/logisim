/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

public class CircuitEvent {
	public static final int ACTION_SET_NAME = 0; // name changed
	public static final int ACTION_ADD = 1; // component added
	public static final int ACTION_REMOVE = 2; // component removed
	public static final int ACTION_CHANGE = 3; // component changed
	public static final int ACTION_INVALIDATE = 4; // component invalidated (pin types changed)
	public static final int ACTION_CLEAR = 5; // entire circuit cleared
	public static final int TRANSACTION_DONE = 6;

	private int action;
	private Circuit circuit;
	private Object data;

	CircuitEvent(int action, Circuit circuit, Object data) {
		this.action = action;
		this.circuit = circuit;
		this.data = data;
	}

	// access methods
	public int getAction() {
		return action;
	}

	public Circuit getCircuit() {
		return circuit;
	}

	public Object getData() {
		return data;
	}

	public CircuitTransactionResult getResult() {
		return (CircuitTransactionResult) data;
	}
}
