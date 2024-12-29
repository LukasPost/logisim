/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.plexers;

import logisim.util.LocaleManager;
import logisim.util.StringGetter;
import logisim.util.StringUtil;

class Strings {
	private static LocaleManager source = new LocaleManager("std");

	public static String get(String key) {
		return source.get(key);
	}

	public static String get(String key, String arg0) {
		return StringUtil.format(source.get(key), arg0);
	}

	public static StringGetter getter(String key) {
		return source.getter(key);
	}

	public static StringGetter getter(String key, String arg0) {
		return source.getter(key, arg0);
	}
}
