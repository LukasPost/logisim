// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{
	public class LogisimMenuItem
	{
		private string name;

		internal LogisimMenuItem(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}
	}

}
