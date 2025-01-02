// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{
	using BitWidth = logisim.data.BitWidth;
	using Value = logisim.data.Value;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstanceState = logisim.instance.InstanceState;
	using StdAttr = logisim.instance.StdAttr;

	public class RegisterLogger : InstanceLogger
	{
		public override string getLogName(InstanceState state, object option)
		{
			string ret = state.getAttributeValue(StdAttr.LABEL);
			return !string.ReferenceEquals(ret, null) && !ret.Equals("") ? ret : null;
		}

		public override Value getLogValue(InstanceState state, object option)
		{
			BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
			if (dataWidth == null)
			{
				dataWidth = BitWidth.create(0);
			}
			RegisterData data = (RegisterData) state.Data;
			if (data == null)
			{
				return Value.createKnown(dataWidth, 0);
			}
			return Value.createKnown(dataWidth, data.value);
		}
	}

}
