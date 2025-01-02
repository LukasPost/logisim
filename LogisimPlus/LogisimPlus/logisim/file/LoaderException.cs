// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{
	public class LoaderException : Exception
	{
		private bool shown;

		internal LoaderException(string desc) : this(desc, false)
		{
		}

		internal LoaderException(string desc, bool shown) : base(desc)
		{
			this.shown = shown;
		}

		public virtual bool Shown
		{
			get
			{
				return shown;
			}
		}
	}
}
