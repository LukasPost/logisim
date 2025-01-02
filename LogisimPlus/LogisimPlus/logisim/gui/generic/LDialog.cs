// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{

	public class LDialog : JDialog
	{
		public LDialog(Frame owner, string title, bool modal) : base(owner, title, modal)
		{
			init();
		}

		private void init()
		{
			LFrame.attachIcon(this);
		}
	}

}
