// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{

	internal class Assignments
	{
		private Dictionary<string, bool> map = new Dictionary<string, bool>();

		public Assignments()
		{
		}

		public virtual bool get(string variable)
		{
			bool? value = map[variable];
			return value != null ? value.Value : false;
		}

		public virtual void put(string variable, bool value)
		{
			map[variable] = Convert.ToBoolean(value);
		}
	}

}
