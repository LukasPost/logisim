// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{
	using Value = logisim.data.Value;

	public abstract class InstanceLogger
	{
		public virtual object[] getLogOptions(InstanceState state)
		{
			return null;
		}

		public abstract string getLogName(InstanceState state, object option);

		public abstract Value getLogValue(InstanceState state, object option);
	}

}
