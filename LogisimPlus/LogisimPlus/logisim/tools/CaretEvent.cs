// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{
	public class CaretEvent
	{
		private Caret caret;
		private string oldtext;
		private string newtext;

		public CaretEvent(Caret caret, string oldtext, string newtext)
		{
			this.caret = caret;
			this.oldtext = oldtext;
			this.newtext = newtext;
		}

		public virtual Caret Caret
		{
			get
			{
				return caret;
			}
		}

		public virtual string OldText
		{
			get
			{
				return oldtext;
			}
		}

		public virtual string Text
		{
			get
			{
				return newtext;
			}
		}
	}

}
