// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{
	public class StringUtil
	{
		private StringUtil()
		{
		}

		public static string capitalize(string a)
		{
			return Character.toTitleCase(a[0]) + a.Substring(1);
		}

		public static string format(string fmt, string a1)
		{
			return format(fmt, a1, null, null);
		}

		public static string format(string fmt, string a1, string a2)
		{
			return format(fmt, a1, a2, null);
		}

		public static string format(string fmt, string a1, string a2, string a3)
		{
			StringBuilder ret = new StringBuilder();
			if (string.ReferenceEquals(a1, null))
			{
				a1 = "(null)";
			}
			if (string.ReferenceEquals(a2, null))
			{
				a2 = "(null)";
			}
			if (string.ReferenceEquals(a3, null))
			{
				a3 = "(null)";
			}
			int arg = 0;
			int pos = 0;
			int next = fmt.IndexOf('%');
			while (next >= 0)
			{
				ret.Append(fmt.Substring(pos, next - pos));
				char c = fmt[next + 1];
				if (c == 's')
				{
					pos = next + 2;
					switch (arg)
					{
					case 0:
						ret.Append(a1);
						break;
					case 1:
						ret.Append(a2);
						break;
					default:
						ret.Append(a3);
					break;
					}
					++arg;
				}
				else if (c == '$')
				{
					switch (fmt[next + 2])
					{
					case '1':
						ret.Append(a1);
						pos = next + 3;
						break;
					case '2':
						ret.Append(a2);
						pos = next + 3;
						break;
					case '3':
						ret.Append(a3);
						pos = next + 3;
						break;
					default:
						ret.Append("%$");
						pos = next + 2;
					break;
					}
				}
				else if (c == '%')
				{
					ret.Append('%');
					pos = next + 2;
				}
				else
				{
					ret.Append('%');
					pos = next + 1;
				}
				next = fmt.IndexOf('%', pos);
			}
			ret.Append(fmt.Substring(pos));
			return ret.ToString();
		}

		public static StringGetter formatter(in StringGetter @base, in string arg)
		{
			return new StringGetterAnonymousInnerClass(@base, arg);
		}

		private class StringGetterAnonymousInnerClass : StringGetter
		{
			private logisim.util.StringGetter @base;
			private string arg;

			public StringGetterAnonymousInnerClass(logisim.util.StringGetter @base, string arg)
			{
				this.@base = @base;
				this.arg = arg;
			}

			public string get()
			{
				return format(@base.get(), arg);
			}
		}

		public static StringGetter formatter(in StringGetter @base, in StringGetter arg)
		{
			return new StringGetterAnonymousInnerClass2(@base, arg);
		}

		private class StringGetterAnonymousInnerClass2 : StringGetter
		{
			private logisim.util.StringGetter @base;
			private logisim.util.StringGetter arg;

			public StringGetterAnonymousInnerClass2(logisim.util.StringGetter @base, logisim.util.StringGetter arg)
			{
				this.@base = @base;
				this.arg = arg;
			}

			public string get()
			{
				return format(@base.get(), arg.get());
			}
		}

		public static StringGetter constantGetter(in string value)
		{
			return new StringGetterAnonymousInnerClass3(value);
		}

		private class StringGetterAnonymousInnerClass3 : StringGetter
		{
			private string value;

			public StringGetterAnonymousInnerClass3(string value)
			{
				this.value = value;
			}

			public string get()
			{
				return value;
			}
		}

		public static string toHexString(int bits, int value)
		{
			if (bits < 32)
			{
				value &= (1 << bits) - 1;
			}
			string ret = Convert.ToString(value, 16);
			int len = (bits + 3) / 4;
			while (ret.Length < len)
			{
				ret = "0" + ret;
			}
			if (ret.Length > len)
			{
				ret = ret.Substring(ret.Length - len);
			}
			return ret;
		}
	}

}
