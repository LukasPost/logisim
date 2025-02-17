/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.prefs;

import java.util.Locale;

import javax.swing.JComponent;

import logisim.util.LocaleManager;
import logisim.util.StringGetter;

class Strings {
	private static LocaleManager source = new LocaleManager("prefs");

	public static String get(String key) {
		return source.get(key);
	}

	public static StringGetter getter(String key) {
		return source.getter(key);
	}

	public static Locale[] getLocaleOptions() {
		return source.getLocaleOptions();
	}

	public static JComponent createLocaleSelector() {
		return source.createLocaleSelector();
	}
}
