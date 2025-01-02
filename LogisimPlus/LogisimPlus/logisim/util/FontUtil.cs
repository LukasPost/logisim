// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class FontUtil
	{
		public static string toStyleStandardString(int style)
		{
			switch (style)
			{
			case Font.PLAIN:
				return "plain";
			case Font.ITALIC:
				return "italic";
			case Font.BOLD:
				return "bold";
			case Font.BOLD | Font.ITALIC:
				return "bolditalic";
			default:
				return "??";
			}
		}

		public static string toStyleDisplayString(int style)
		{
			switch (style)
			{
			case Font.PLAIN:
				return Strings.get("fontPlainStyle");
			case Font.ITALIC:
				return Strings.get("fontItalicStyle");
			case Font.BOLD:
				return Strings.get("fontBoldStyle");
			case Font.BOLD | Font.ITALIC:
				return Strings.get("fontBoldItalicStyle");
			default:
				return "??";
			}
		}

	}

}
