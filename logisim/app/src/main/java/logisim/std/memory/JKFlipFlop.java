/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;


import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;

public class JKFlipFlop extends AbstractFlipFlop {
	public JKFlipFlop() {
		super("J-K Flip-Flop", "jkFlipFlop.gif", Strings.getter("jkFlipFlopComponent"), 2, false);
	}

	@Override
	protected String getInputName(int index) {
		return index == 0 ? "J" : "K";
	}

	@Override
	protected WireValue computeValue(WireValue[] inputs, WireValue curValue) {
		if (inputs[0] == WireValues.FALSE) {
			if (inputs[1] == WireValues.FALSE) return curValue;
			else if (inputs[1] == WireValues.TRUE) return WireValues.FALSE;
		} else if (inputs[0] == WireValues.TRUE) if (inputs[1] == WireValues.FALSE) return WireValues.TRUE;
		else if (inputs[1] == WireValues.TRUE) return curValue.not();
		return WireValues.UNKNOWN;
	}
}
