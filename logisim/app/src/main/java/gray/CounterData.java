/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package gray;

import logisim.data.BitWidth;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.InstanceData;
import logisim.instance.InstanceState;

/** Represents the state of a counter. */
class CounterData implements InstanceData, Cloneable {
	/**
	 * Retrieves the state associated with this counter in the circuit state, generating the state if necessary.
	 */
	public static CounterData get(InstanceState state, BitWidth width) {
		CounterData ret = (CounterData) state.getData();
		if (ret == null) {
			// If it doesn't yet exist, then we'll set it up with our default
			// values and put it into the circuit state so it can be retrieved
			// in future propagations.
			ret = new CounterData(null, WireValue.Companion.createKnown(width, 0));
			state.setData(ret);
		} else if (!ret.value.getBitWidth().equals(width))
			ret.value = ret.value.extendWidth(width.getWidth(), WireValues.FALSE);
		return ret;
	}

	/** The last clock input value observed. */
	private WireValue lastClock;

	/** The current value emitted by the counter. */
	private WireValue value;

	/** Constructs a state with the given values. */
	public CounterData(WireValue lastClock, WireValue value) {
		this.lastClock = lastClock;
		this.value = value;
	}

	/** Returns a copy of this object. */
	@Override
	public Object clone() {
		// We can just use what super.clone() returns: The only instance variables are
		// Value objects, which are immutable, so we don't care that both the copy
		// and the copied refer to the same Value objects. If we had mutable instance
		// variables, then of course we would need to clone them.
		try {
			return super.clone();
		}
		catch (CloneNotSupportedException e) {
			return null;
		}
	}

	/** Updates the last clock observed, returning true if triggered. */
	public boolean updateClock(WireValue value) {
		WireValue old = lastClock;
		lastClock = value;
		return old == WireValues.FALSE && value == WireValues.TRUE;
	}

	/** Returns the current value emitted by the counter. */
	public WireValue getValue() {
		return value;
	}

	/** Updates the current value emitted by the counter. */
	public void setValue(WireValue value) {
		this.value = value;
	}
}
