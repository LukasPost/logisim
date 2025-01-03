// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.key
{

	using logisim.data;
	using BitWidth = logisim.data.BitWidth;
	using Value = logisim.data.Value;

	public class BitWidthConfigurator : NumericConfigurator<BitWidth>
	{
		public BitWidthConfigurator(Attribute attr, int min, int max, int modifiersEx) : base(attr, min, max, modifiersEx)
		{
		}

		public BitWidthConfigurator(Attribute attr, int min, int max) : base(attr, min, max, InputEvent.ALT_DOWN_MASK)
		{
		}

		public BitWidthConfigurator(Attribute attr) : base(attr, 1, Value.MAX_WIDTH, InputEvent.ALT_DOWN_MASK)
		{
		}

		protected internal override BitWidth createValue(int val)
		{
			return BitWidth.create(val);
		}
	}

}
