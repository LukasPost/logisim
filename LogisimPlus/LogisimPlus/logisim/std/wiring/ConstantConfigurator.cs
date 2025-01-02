// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using StdAttr = logisim.instance.StdAttr;
	using IntegerConfigurator = logisim.tools.key.IntegerConfigurator;

	internal class ConstantConfigurator : IntegerConfigurator
	{
		public ConstantConfigurator() : base(Constant.ATTR_VALUE, 0, 0, 0, 16)
		{
		}

		public override int getMaximumValue(AttributeSet attrs)
		{
			BitWidth width = attrs.getValue(StdAttr.WIDTH);
			int ret = width.Mask;
			if (ret >= 0)
			{
				return ret;
			}
			else
			{
				return int.MaxValue;
			}
		}

		public override int getMinimumValue(AttributeSet attrs)
		{
			BitWidth width = attrs.getValue(StdAttr.WIDTH);
			if (width.Width < 32)
			{
				return 0;
			}
			else
			{
				return int.MinValue;
			}
		}
	}

}
