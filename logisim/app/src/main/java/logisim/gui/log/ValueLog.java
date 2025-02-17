/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;


import logisim.data.WireValue.WireValue;

class ValueLog {
	private static final int LOG_SIZE = 400;

	private WireValue[] log;
	private short curSize;
	private short firstIndex;

	public ValueLog() {
		log = new WireValue[LOG_SIZE];
		curSize = 0;
		firstIndex = 0;
	}

	public int size() {
		return curSize;
	}

	public WireValue get(int index) {
		int i = firstIndex + index;
		if (i >= LOG_SIZE)
			i -= LOG_SIZE;
		return log[i];
	}

	public WireValue getLast() {
		if (curSize < LOG_SIZE)
			return curSize == 0 ? null : log[curSize - 1];
		return firstIndex == 0 ? log[curSize - 1] : log[firstIndex - 1];
	}

	public void append(WireValue val) {
		if (curSize < LOG_SIZE) {
			log[curSize] = val;
			curSize++;
		} else {
			log[firstIndex] = val;
			firstIndex++;
			if (firstIndex >= LOG_SIZE)
				firstIndex = 0;
		}
	}
}
