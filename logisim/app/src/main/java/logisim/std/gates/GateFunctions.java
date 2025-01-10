/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;

class GateFunctions {
	private GateFunctions() {
	}

	static WireValue computeOr(WireValue[] inputs, int numInputs) {
		WireValue ret = inputs[0];
		for (int i = 1; i < numInputs; i++) ret = ret.or(inputs[i]);
		return ret;
	}

	static WireValue computeAnd(WireValue[] inputs, int numInputs) {
		WireValue ret = inputs[0];
		for (int i = 1; i < numInputs; i++) ret = ret.and(inputs[i]);
		return ret;
	}

	static WireValue computeOddParity(WireValue[] inputs, int numInputs) {
		WireValue ret = inputs[0];
		for (int i = 1; i < numInputs; i++) ret = ret.xor(inputs[i]);
		return ret;
	}

	static WireValue computeExactlyOne(WireValue[] inputs, int numInputs) {
		int width = inputs[0].getWidth();
		WireValue[] ret = new WireValue[width];
		for (int i = 0; i < width; i++) {
			int count = 0;
			for (int j = 0; j < numInputs; j++) {
				WireValue v = inputs[j].get(i);
				if (v == WireValues.TRUE) count++;
				else if (v == WireValues.FALSE) ; // do nothing
				else {
					count = -1;
					break;
				}
			}
			if (count < 0) ret[i] = WireValues.ERROR;
			else if (count == 1) ret[i] = WireValues.TRUE;
			else ret[i] = WireValues.FALSE;
		}
		return WireValue.Companion.create(ret);
	}
}
