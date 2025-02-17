/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import logisim.data.BitWidth;
import logisim.data.WireValue.WireValue;
import logisim.instance.InstanceLogger;
import logisim.instance.InstanceState;
import logisim.instance.StdAttr;

public class ShiftRegisterLogger extends InstanceLogger {
	@Override
	public Object[] getLogOptions(InstanceState state) {
		Integer stages = state.getAttributeValue(ShiftRegister.ATTR_LENGTH);
		Object[] ret = new Object[stages];
		for (int i = 0; i < ret.length; i++) ret[i] = i;
		return ret;
	}

	@Override
	public String getLogName(InstanceState state, Object option) {
		String inName = state.getAttributeValue(StdAttr.LABEL);
		if (inName == null || inName.isEmpty())
			inName = Strings.get("shiftRegisterComponent") + state.getInstance().getLocation();
		if (option instanceof Integer) return inName + "[" + option + "]";
		else return inName;
	}

	@Override
	public WireValue getLogValue(InstanceState state, Object option) {
		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
		if (dataWidth == null)
			dataWidth = BitWidth.create(0);
		ShiftRegisterData data = (ShiftRegisterData) state.getData();
		if (data == null) return WireValue.Companion.createKnown(dataWidth, 0);
		else {
			int index = option == null ? 0 : (Integer) option;
			return data.get(index);
		}
	}
}
