// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{
	using StringGetter = logisim.util.StringGetter;

	public class AttributeOption : AttributeOptionInterface
	{
		private object value;
		private string name;
		private StringGetter desc;

		public AttributeOption(object value, StringGetter desc)
		{
			this.value = value;
			this.name = value.ToString();
			this.desc = desc;
		}

		public AttributeOption(object value, string name, StringGetter desc)
		{
			this.value = value;
			this.name = name;
			this.desc = desc;
		}

		public virtual object Value
		{
			get
			{
				return value;
			}
		}

		public override string ToString()
		{
			return name;
		}

		public virtual string toDisplayString()
		{
			return desc.get();
		}
	}

}
