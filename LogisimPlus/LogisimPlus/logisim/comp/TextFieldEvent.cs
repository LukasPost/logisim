// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{
	public class TextFieldEvent
	{
		private TextField field;
		private string oldval;
		private string newval;

		public TextFieldEvent(TextField field, string old, string val)
		{
			this.field = field;
			this.oldval = old;
			this.newval = val;
		}

		public virtual TextField TextField
		{
			get
			{
				return field;
			}
		}

		public virtual string OldText
		{
			get
			{
				return oldval;
			}
		}

		public virtual string Text
		{
			get
			{
				return newval;
			}
		}
	}

}
