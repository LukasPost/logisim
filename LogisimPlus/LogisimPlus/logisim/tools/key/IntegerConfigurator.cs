// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.key
{
	using logisim.data;

	public class IntegerConfigurator : NumericConfigurator<int>
	{
		public IntegerConfigurator(Attribute attr, int min, int max, int modifiersEx) : base(attr, min, max, modifiersEx)
		{
		}

		public IntegerConfigurator(Attribute attr, int min, int max, int modifiersEx, int radix) : base(attr, min, max, modifiersEx, radix)
		{
		}

		protected internal override int? createValue(int val)
		{
			return Convert.ToInt32(val);
		}
	}

}
