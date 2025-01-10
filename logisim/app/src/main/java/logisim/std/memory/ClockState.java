/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.StdAttr;

class ClockState implements Cloneable {
	private WireValue lastClock;

	public ClockState() {
		lastClock = WireValues.FALSE;
	}

	@Override
	public ClockState clone() {
		try {
			return (ClockState) super.clone();
		}
		catch (CloneNotSupportedException e) {
			return null;
		}
	}

	public boolean updateClock(WireValue newClock, Object trigger) {
		WireValue oldClock = lastClock;
		lastClock = newClock;
		if (trigger == null || trigger == StdAttr.TRIG_RISING) return oldClock == WireValues.FALSE && newClock == WireValues.TRUE;
		else if (trigger == StdAttr.TRIG_FALLING) return oldClock == WireValues.TRUE && newClock == WireValues.FALSE;
		else if (trigger == StdAttr.TRIG_HIGH) return newClock == WireValues.TRUE;
		else if (trigger == StdAttr.TRIG_LOW) return newClock == WireValues.FALSE;
		else return oldClock == WireValues.FALSE && newClock == WireValues.TRUE;
	}
}
