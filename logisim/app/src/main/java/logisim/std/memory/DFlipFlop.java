/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import logisim.data.Value;

public class DFlipFlop extends AbstractFlipFlop {
	public DFlipFlop() {
		super("D Flip-Flop", "dFlipFlop.gif",
				Strings.getter("dFlipFlopComponent"), 1, true);
	}

	@Override
	protected String getInputName(int index) {
		return "D";
	}

	@Override
	protected Value computeValue(Value[] inputs, Value curValue) {
		return inputs[0];
	}
}