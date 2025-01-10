/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;


import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;

public class TFlipFlop extends AbstractFlipFlop {
	public TFlipFlop() {
		super("T Flip-Flop", "tFlipFlop.gif", Strings.getter("tFlipFlopComponent"), 1, false);
	}

	@Override
	protected String getInputName(int index) {
		return "T";
	}

	@Override
	protected WireValue computeValue(WireValue[] inputs, WireValue curValue) {
		if (curValue == WireValues.UNKNOWN)
			curValue = WireValues.FALSE;
		if (inputs[0] == WireValues.TRUE) return curValue.not();
		else return curValue;
	}
}
