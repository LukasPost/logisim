/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

import logisim.util.LocaleManager;
import logisim.util.StringGetter;

class Strings {
	private static LocaleManager source = new LocaleManager("data");

	public static String get(String key) {
		return source.get(key);
	}

	public static StringGetter getter(String key) {
		return source.getter(key);
	}
}
