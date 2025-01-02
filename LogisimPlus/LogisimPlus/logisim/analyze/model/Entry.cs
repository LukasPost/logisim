// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{
	using StringGetter = logisim.util.StringGetter;

	public class Entry
	{
		public static readonly Entry ZERO = new Entry("0");
		public static readonly Entry ONE = new Entry("1");
		public static readonly Entry DONT_CARE = new Entry("x");
		public static readonly Entry BUS_ERROR = new Entry(Strings.getter("busError"));
		public static readonly Entry OSCILLATE_ERROR = new Entry(Strings.getter("oscillateError"));

		public static Entry parse(string description)
		{
			if (ZERO.description.Equals(description))
			{
				return ZERO;
			}
			if (ONE.description.Equals(description))
			{
				return ONE;
			}
			if (DONT_CARE.description.Equals(description))
			{
				return DONT_CARE;
			}
			if (BUS_ERROR.description.Equals(description))
			{
				return BUS_ERROR;
			}
			return null;
		}

		private string description;
		private StringGetter errorMessage;

		private Entry(string description)
		{
			this.description = description;
			this.errorMessage = null;
		}

		private Entry(StringGetter errorMessage)
		{
			this.description = "!!";
			this.errorMessage = errorMessage;
		}

		public virtual string Description
		{
			get
			{
				return description;
			}
		}

		public virtual bool Error
		{
			get
			{
				return errorMessage != null;
			}
		}

		public virtual string ErrorMessage
		{
			get
			{
				return errorMessage == null ? null : errorMessage.get();
			}
		}

		public override string ToString()
		{
			return "Entry[" + description + "]";
		}
	}

}
