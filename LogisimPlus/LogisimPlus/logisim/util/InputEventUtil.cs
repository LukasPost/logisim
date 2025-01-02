// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class InputEventUtil
	{
		public static string CTRL = "Ctrl";
		public static string SHIFT = "Shift";
		public static string ALT = "Alt";
		public static string BUTTON1 = "Button1";
		public static string BUTTON2 = "Button2";
		public static string BUTTON3 = "Button3";

		private InputEventUtil()
		{
		}

		public static int fromString(string str)
		{
			int ret = 0;
			StringTokenizer toks = new StringTokenizer(str);
			while (toks.hasMoreTokens())
			{
				string s = toks.nextToken();
				if (s.Equals(CTRL))
				{
					ret |= InputEvent.CTRL_DOWN_MASK;
				}
				else if (s.Equals(SHIFT))
				{
					ret |= InputEvent.SHIFT_DOWN_MASK;
				}
				else if (s.Equals(ALT))
				{
					ret |= InputEvent.ALT_DOWN_MASK;
				}
				else if (s.Equals(BUTTON1))
				{
					ret |= InputEvent.BUTTON1_DOWN_MASK;
				}
				else if (s.Equals(BUTTON2))
				{
					ret |= InputEvent.BUTTON2_DOWN_MASK;
				}
				else if (s.Equals(BUTTON3))
				{
					ret |= InputEvent.BUTTON3_DOWN_MASK;
				}
				else
				{
					throw new System.FormatException("InputEventUtil");
				}
			}
			return ret;
		}

		public static string toString(int mods)
		{
			List<string> arr = new List<string>();
			if ((mods & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				arr.Add(CTRL);
			}
			if ((mods & InputEvent.ALT_DOWN_MASK) != 0)
			{
				arr.Add(ALT);
			}
			if ((mods & InputEvent.SHIFT_DOWN_MASK) != 0)
			{
				arr.Add(SHIFT);
			}
			if ((mods & InputEvent.BUTTON1_DOWN_MASK) != 0)
			{
				arr.Add(BUTTON1);
			}
			if ((mods & InputEvent.BUTTON2_DOWN_MASK) != 0)
			{
				arr.Add(BUTTON2);
			}
			if ((mods & InputEvent.BUTTON3_DOWN_MASK) != 0)
			{
				arr.Add(BUTTON3);
			}

			IEnumerator<string> it = arr.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (it.hasNext())
			{
				StringBuilder ret = new StringBuilder();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ret.Append(it.next());
				while (it.MoveNext())
				{
					ret.Append(" ");
					ret.Append(it.Current);
				}
				return ret.ToString();
			}
			else
			{
				return "";
			}
		}

		public static int fromDisplayString(string str)
		{
			int ret = 0;
			StringTokenizer toks = new StringTokenizer(str);
			while (toks.hasMoreTokens())
			{
				string s = toks.nextToken();
				if (s.Equals(Strings.get("ctrlMod")))
				{
					ret |= InputEvent.CTRL_DOWN_MASK;
				}
				else if (s.Equals(Strings.get("altMod")))
				{
					ret |= InputEvent.ALT_DOWN_MASK;
				}
				else if (s.Equals(Strings.get("shiftMod")))
				{
					ret |= InputEvent.SHIFT_DOWN_MASK;
				}
				else if (s.Equals(Strings.get("button1Mod")))
				{
					ret |= InputEvent.BUTTON1_DOWN_MASK;
				}
				else if (s.Equals(Strings.get("button2Mod")))
				{
					ret |= InputEvent.BUTTON2_DOWN_MASK;
				}
				else if (s.Equals(Strings.get("button3Mod")))
				{
					ret |= InputEvent.BUTTON3_DOWN_MASK;
				}
				else
				{
					throw new System.FormatException("InputEventUtil");
				}
			}
			return ret;
		}

		public static string toDisplayString(int mods)
		{
			List<string> arr = new List<string>();
			if ((mods & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				arr.Add(Strings.get("ctrlMod"));
			}
			if ((mods & InputEvent.ALT_DOWN_MASK) != 0)
			{
				arr.Add(Strings.get("altMod"));
			}
			if ((mods & InputEvent.SHIFT_DOWN_MASK) != 0)
			{
				arr.Add(Strings.get("shiftMod"));
			}
			if ((mods & InputEvent.BUTTON1_DOWN_MASK) != 0)
			{
				arr.Add(Strings.get("button1Mod"));
			}
			if ((mods & InputEvent.BUTTON2_DOWN_MASK) != 0)
			{
				arr.Add(Strings.get("button2Mod"));
			}
			if ((mods & InputEvent.BUTTON3_DOWN_MASK) != 0)
			{
				arr.Add(Strings.get("button3Mod"));
			}

			if (arr.Count == 0)
			{
				return "";
			}

			IEnumerator<string> it = arr.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (it.hasNext())
			{
				StringBuilder ret = new StringBuilder();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ret.Append(it.next());
				while (it.MoveNext())
				{
					ret.Append(" ");
					ret.Append(it.Current);
				}
				return ret.ToString();
			}
			else
			{
				return "";
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("deprecation") public static String toKeyDisplayString(int mods)
		public static string toKeyDisplayString(int mods)
		{
			List<string> arr = new List<string>();
			if ((mods & Event.META_MASK) != 0)
			{
				arr.Add(Strings.get("metaMod"));
			}
			if ((mods & Event.CTRL_MASK) != 0)
			{
				arr.Add(Strings.get("ctrlMod"));
			}
			if ((mods & Event.ALT_MASK) != 0)
			{
				arr.Add(Strings.get("altMod"));
			}
			if ((mods & Event.SHIFT_MASK) != 0)
			{
				arr.Add(Strings.get("shiftMod"));
			}

			IEnumerator<string> it = arr.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (it.hasNext())
			{
				StringBuilder ret = new StringBuilder();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ret.Append(it.next());
				while (it.MoveNext())
				{
					ret.Append(" ");
					ret.Append(it.Current);
				}
				return ret.ToString();
			}
			else
			{
				return "";
			}
		}
	}

}
