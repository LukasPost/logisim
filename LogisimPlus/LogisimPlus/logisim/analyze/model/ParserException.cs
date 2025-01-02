// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{
	using StringGetter = logisim.util.StringGetter;

	public class ParserException : Exception
	{
		private StringGetter message;
		private int start;
		private int length;

		public ParserException(StringGetter message, int start, int length) : base(message.get())
		{
			this.message = message;
			this.start = start;
			this.length = length;
		}

		public override string Message
		{
			get
			{
				return message.get();
			}
		}

		public virtual StringGetter MessageGetter
		{
			get
			{
				return message;
			}
		}

		public virtual int Offset
		{
			get
			{
				return start;
			}
		}

		public virtual int EndOffset
		{
			get
			{
				return start + length;
			}
		}
	}
}
