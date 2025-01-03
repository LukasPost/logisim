// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{
	using LogisimVersion = logisim.LogisimVersion;

	public interface AttributeDefaultProvider
	{
		bool isAllDefaultValues(AttributeSet attrs, LogisimVersion ver);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Object getDefaultAttributeValue(Attribute<?> attr, logisim.LogisimVersion ver);
		object getDefaultAttributeValue<T1>(Attribute attr, LogisimVersion ver);
	}

}
