// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{

	public interface AttributeSet
	{
		object Clone();

		void addAttributeListener(AttributeListener l);

		void removeAttributeListener(AttributeListener l);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.List<Attribute<?>> getAttributes();
		List<Attribute> Attributes {get;}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public boolean containsAttribute(Attribute<?> attr);
		bool containsAttribute(Attribute attr);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Attribute<?> getAttribute(String name);
		Attribute getAttribute(string name);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public boolean isReadOnly(Attribute<?> attr);
		bool isReadOnly(Attribute attr);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public void setReadOnly(Attribute<?> attr, boolean value);
		void setReadOnly(Attribute attr, bool value); // optional

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public boolean isToSave(Attribute<?> attr);
		bool isToSave(Attribute attr);

		object getValue(Attribute attr);

		void setValue(Attribute attr, object value);
	}

}
