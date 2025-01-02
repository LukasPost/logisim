// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	using StringUtil = logisim.util.StringUtil;

	public class AnalyzeException : Exception
	{
		public class Circular : AnalyzeException
		{
			public Circular() : base(Strings.get("analyzeCircularError"))
			{
			}
		}

		public class Conflict : AnalyzeException
		{
			public Conflict() : base(Strings.get("analyzeConflictError"))
			{
			}
		}

		public class CannotHandle : AnalyzeException
		{
			public CannotHandle(string reason) : base(StringUtil.format(Strings.get("analyzeCannotHandleError"), reason))
			{
			}
		}

		public AnalyzeException()
		{
		}

		public AnalyzeException(string message) : base(message)
		{
		}
	}

}
