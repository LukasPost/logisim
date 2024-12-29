/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.proj;

import logisim.util.LocaleManager;
import logisim.util.StringGetter;
import logisim.util.StringUtil;

class Strings {
	private static LocaleManager source = new LocaleManager("proj");

	public static String get(String key) {
		return source.get(key);
	}

	public static String get(String key, String arg) {
		return StringUtil.format(source.get(key), arg);
	}

	public static StringGetter getter(String key) {
		return source.getter(key);
	}
}
