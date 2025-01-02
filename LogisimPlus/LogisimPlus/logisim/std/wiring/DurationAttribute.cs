// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using logisim.data;
	using StringGetter = logisim.util.StringGetter;
	using StringUtil = logisim.util.StringUtil;

	public class DurationAttribute : Attribute<int>
	{
		private int min;
		private int max;

		public DurationAttribute(string name, StringGetter disp, int min, int max) : base(name, disp)
		{
			this.min = min;
			this.max = max;
		}

		public override int? parse(string value)
		{
			try
			{
				int? ret = Convert.ToInt32(value);
				if (ret.Value < min)
				{
					throw new System.FormatException(StringUtil.format(Strings.get("durationSmallMessage"), "" + min));
				}
				else if (ret.Value > max)
				{
					throw new System.FormatException(StringUtil.format(Strings.get("durationLargeMessage"), "" + max));
				}
				return ret;
			}
			catch (System.FormatException)
			{
				throw new System.FormatException(Strings.get("freqInvalidMessage"));
			}
		}

		public override string toDisplayString(int? value)
		{
			if (value.Equals(Convert.ToInt32(1)))
			{
				return Strings.get("clockDurationOneValue");
			}
			else
			{
				return StringUtil.format(Strings.get("clockDurationValue"), value.ToString());
			}
		}

		public override java.awt.Component getCellEditor(int? value)
		{
			JTextField field = new JTextField();
			field.setText(value.ToString());
			return field;
		}

	}

}
