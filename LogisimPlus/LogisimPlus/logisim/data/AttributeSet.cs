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
		object clone();

		void addAttributeListener(AttributeListener l);

		void removeAttributeListener(AttributeListener l);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.List<Attribute<?>> getAttributes();
		IList<Attribute<object>> Attributes {get;}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public boolean containsAttribute(Attribute<?> attr);
		bool containsAttribute<T1>(Attribute<T1> attr);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Attribute<?> getAttribute(String name);
		Attribute<object> getAttribute(string name);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public boolean isReadOnly(Attribute<?> attr);
		bool isReadOnly<T1>(Attribute<T1> attr);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public void setReadOnly(Attribute<?> attr, boolean value);
		void setReadOnly<T1>(Attribute<T1> attr, bool value); // optional

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public boolean isToSave(Attribute<?> attr);
		bool isToSave<T1>(Attribute<T1> attr);

		V getValue<V>(Attribute<V> attr);

		void setValue<V>(Attribute<V> attr, V value);
	}

}
