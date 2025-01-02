// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{
	using LocaleManager = logisim.util.LocaleManager;
	using StringGetter = logisim.util.StringGetter;
	using StringUtil = logisim.util.StringUtil;

	internal class Strings
	{
		private static LocaleManager source = new LocaleManager("file");

		public static string get(string key)
		{
			return source.get(key);
		}

		public static string get(string key, string arg)
		{
			return StringUtil.format(source.get(key), arg);
		}

		public static string get(string key, string arg0, string arg1)
		{
			return StringUtil.format(source.get(key), arg0, arg1);
		}

		public static StringGetter getter(string key)
		{
			return source.getter(key);
		}
	}

}
