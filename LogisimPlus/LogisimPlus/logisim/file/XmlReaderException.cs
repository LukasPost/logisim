// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{

	internal class XmlReaderException : Exception
	{
		private List<string> messages;

		public XmlReaderException(string message) : this(Collections.singletonList(message))
		{
		}

		public XmlReaderException(List<string> messages)
		{
			this.messages = messages;
		}

		public override string Message
		{
			get
			{
				return messages[0];
			}
		}

		public virtual List<string> Messages
		{
			get
			{
				return messages;
			}
		}
	}

}
