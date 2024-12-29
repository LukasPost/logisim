/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import logisim.util.LocaleManager;
import logisim.util.StringGetter;

class Strings {
	private static LocaleManager source
		= new LocaleManager("tools");

	public static String get(String key) {
		return source.get(key);
	}
	public static StringGetter getter(String key) {
		return source.getter(key);
	}
	public static StringGetter getter(String key, StringGetter arg) {
		return source.getter(key, arg);
	}
}
