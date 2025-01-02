// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{
	using LocaleManager = logisim.util.LocaleManager;
	using StringGetter = logisim.util.StringGetter;

	internal class Strings
	{
		private static LocaleManager source = new LocaleManager("analyze");

		public static string get(string key)
		{
			return source.get(key);
		}

		public static StringGetter getter(string key)
		{
			return source.getter(key);
		}
	}

}
