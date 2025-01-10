/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import logisim.data.BitWidth;
import logisim.data.WireValue.WireValue;
import logisim.instance.InstanceLogger;
import logisim.instance.InstanceState;
import logisim.instance.StdAttr;

public class RegisterLogger extends InstanceLogger {
	@Override
	public String getLogName(InstanceState state, Object option) {
		String ret = state.getAttributeValue(StdAttr.LABEL);
		return ret != null && !ret.isEmpty() ? ret : null;
	}

	@Override
	public WireValue getLogValue(InstanceState state, Object option) {
		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
		if (dataWidth == null)
			dataWidth = BitWidth.create(0);
		RegisterData data = (RegisterData) state.getData();
		if (data == null)
			return WireValue.Companion.createKnown(dataWidth, 0);
		return WireValue.Companion.createKnown(dataWidth, data.value);
	}
}
