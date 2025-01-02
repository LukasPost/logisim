// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{
	using InstanceData = logisim.instance.InstanceData;

	internal class RegisterData : ClockState, InstanceData
	{
		internal int value;

		public RegisterData()
		{
			value = 0;
		}

		public virtual int Value
		{
			set
			{
				this.value = value;
			}
			get
			{
				return value;
			}
		}

	}
}
