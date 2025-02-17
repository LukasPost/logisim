/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

public class StringUtil {
	private StringUtil() {
	}

	public static String capitalize(String a) {
		return Character.toTitleCase(a.charAt(0)) + a.substring(1);
	}

	public static String format(String fmt, String a1) {
		return format(fmt, a1, null, null);
	}

	public static String format(String fmt, String a1, String a2) {
		return format(fmt, a1, a2, null);
	}

	public static String format(String fmt, String a1, String a2, String a3) {
		StringBuilder ret = new StringBuilder();
		if (a1 == null)
			a1 = "(null)";
		if (a2 == null)
			a2 = "(null)";
		if (a3 == null)
			a3 = "(null)";
		int arg = 0;
		int pos = 0;
		int next = fmt.indexOf('%');
		while (next >= 0) {
			ret.append(fmt, pos, next);
			char c = fmt.charAt(next + 1);
			if (c == 's') {
				pos = next + 2;
				switch (arg) {
				case 0:
					ret.append(a1);
					break;
				case 1:
					ret.append(a2);
					break;
				default:
					ret.append(a3);
				}
				++arg;
			} else if (c == '$') pos = switch (fmt.charAt(next + 2)) {
				case '1' -> {
					ret.append(a1);
					yield next + 3;
				}
				case '2' -> {
					ret.append(a2);
					yield next + 3;
				}
				case '3' -> {
					ret.append(a3);
					yield next + 3;
				}
				default -> {
					ret.append("%$");
					yield next + 2;
				}
			};
			else if (c == '%') {
				ret.append('%');
				pos = next + 2;
			} else {
				ret.append('%');
				pos = next + 1;
			}
			next = fmt.indexOf('%', pos);
		}
		ret.append(fmt.substring(pos));
		return ret.toString();
	}

	public static StringGetter formatter(final StringGetter base, final String arg) {
		return () -> format(base.get(), arg);
	}

	public static StringGetter formatter(final StringGetter base, final StringGetter arg) {
		return () -> format(base.get(), arg.get());
	}

	public static StringGetter constantGetter(final String value) {
		return () -> value;
	}

	public static String toHexString(int bits, int value) {
		if (bits < 32)
			value &= (1 << bits) - 1;
		StringBuilder ret = new StringBuilder(Integer.toHexString(value));
		int len = (bits + 3) / 4;
		while (ret.length() < len)
			ret.insert(0, "0");
		if (ret.length() > len)
			return ret.substring(ret.length() - len);
		return ret.toString();
	}
}
