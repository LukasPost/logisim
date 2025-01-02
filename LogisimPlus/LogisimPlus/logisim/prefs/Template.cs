// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.IO;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.prefs
{

	public class Template
	{
		public static Template createEmpty()
		{
			string circName = Strings.get("newCircuitName");
			StringBuilder buf = new StringBuilder();
			buf.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			buf.Append("<project version=\"1.0\">");
			buf.Append(" <circuit name=\"" + circName + "\" />");
			buf.Append("</project>");
			return new Template(buf.ToString());
		}

		public static Template create(Stream @in)
		{
			StreamReader reader = new StreamReader(@in);
			char[] buf = new char[4096];
			StringBuilder dest = new StringBuilder();
			while (true)
			{
				try
				{
					int nbytes = reader.Read(buf, 0, buf.Length);
					if (nbytes < 0)
					{
						break;
					}
					dest.Append(buf, 0, nbytes);
				}
				catch (IOException)
				{
					break;
				}
			}
			return new Template(dest.ToString());
		}

		private string contents;

		private Template(string contents)
		{
			this.contents = contents;
		}

		public virtual Stream createStream()
		{
			try
			{
				return new MemoryStream(contents.GetBytes(Encoding.UTF8));
			}
			catch (UnsupportedEncodingException)
			{
				Console.Error.WriteLine("warning: UTF-8 is not supported"); // OK
				return new MemoryStream(contents.GetBytes());
			}
		}
	}

}
