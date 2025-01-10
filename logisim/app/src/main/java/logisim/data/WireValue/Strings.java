package logisim.data.WireValue;

import logisim.util.LocaleManager;
import logisim.util.StringGetter;

public class Strings {
	private static LocaleManager source = new LocaleManager("data");

	public static String get(String key) {
		return source.get(key);
	}

	public static StringGetter getter(String key) {
		return source.getter(key);
	}
}
