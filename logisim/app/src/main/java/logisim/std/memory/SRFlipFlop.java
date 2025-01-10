/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;


import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;

public class SRFlipFlop extends AbstractFlipFlop {
	public SRFlipFlop() {
		super("S-R Flip-Flop", "srFlipFlop.gif", Strings.getter("srFlipFlopComponent"), 2, true);
	}

	@Override
	protected String getInputName(int index) {
		return index == 0 ? "S" : "R";
	}

	@Override
	protected WireValue computeValue(WireValue[] inputs, WireValue curValue) {
		if (inputs[0] == WireValues.FALSE) {
			if (inputs[1] == WireValues.FALSE) return curValue;
			else if (inputs[1] == WireValues.TRUE) return WireValues.FALSE;
		} else if (inputs[0] == WireValues.TRUE) if (inputs[1] == WireValues.FALSE) return WireValues.TRUE;
		else if (inputs[1] == WireValues.TRUE) return WireValues.ERROR;
		return WireValues.UNKNOWN;
	}
}
