/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

import java.awt.Font;

public class FontUtil {
	public static String toStyleStandardString(int style) {
		return switch (style) {
			case Font.PLAIN -> "plain";
			case Font.ITALIC -> "italic";
			case Font.BOLD -> "bold";
			case Font.BOLD | Font.ITALIC -> "bolditalic";
			default -> "??";
		};
	}

	public static String toStyleDisplayString(int style) {
		return switch (style) {
			case Font.PLAIN -> Strings.get("fontPlainStyle");
			case Font.ITALIC -> Strings.get("fontItalicStyle");
			case Font.BOLD -> Strings.get("fontBoldStyle");
			case Font.BOLD | Font.ITALIC -> Strings.get("fontBoldItalicStyle");
			default -> "??";
		};
	}

}
