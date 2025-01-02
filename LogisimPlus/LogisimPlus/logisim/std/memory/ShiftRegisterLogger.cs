// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{
	using BitWidth = logisim.data.BitWidth;
	using Value = logisim.data.Value;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstanceState = logisim.instance.InstanceState;
	using StdAttr = logisim.instance.StdAttr;

	public class ShiftRegisterLogger : InstanceLogger
	{
		public override object[] getLogOptions(InstanceState state)
		{
			int? stages = state.getAttributeValue(ShiftRegister.ATTR_LENGTH);
			object[] ret = new object[stages.Value];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = Convert.ToInt32(i);
			}
			return ret;
		}

		public override string getLogName(InstanceState state, object option)
		{
			string inName = state.getAttributeValue(StdAttr.LABEL);
			if (string.ReferenceEquals(inName, null) || inName.Equals(""))
			{
				inName = Strings.get("shiftRegisterComponent") + state.Instance.Location;
			}
			if (option is int?)
			{
				return inName + "[" + option + "]";
			}
			else
			{
				return inName;
			}
		}

		public override Value getLogValue(InstanceState state, object option)
		{
			BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
			if (dataWidth == null)
			{
				dataWidth = BitWidth.create(0);
			}
			ShiftRegisterData data = (ShiftRegisterData) state.Data;
			if (data == null)
			{
				return Value.createKnown(dataWidth, 0);
			}
			else
			{
				int index = option == null ? 0 : ((int?) option).Value;
				return data.get(index);
			}
		}
	}

}
