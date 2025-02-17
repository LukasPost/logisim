﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.actions
{
	using LocaleManager = logisim.util.LocaleManager;
	using StringGetter = logisim.util.StringGetter;
	using StringUtil = logisim.util.StringUtil;

	internal class Strings
	{
		private static LocaleManager source = new LocaleManager("draw");

		public static string get(string key)
		{
			return source.get(key);
		}

		public static string get(string key, string arg)
		{
			return StringUtil.format(source.get(key), arg);
		}

		public static StringGetter getter(string key)
		{
			return source.getter(key);
		}

		public static StringGetter getter(string key, string arg)
		{
			return source.getter(key, arg);
		}
	}

}
