// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{
	public class AttributeEvent
	{
		private AttributeSet source;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private Attribute<?> attr;
		private Attribute<object> attr;
		private object value;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: public AttributeEvent(AttributeSet source, Attribute<?> attr, Object value)
		public AttributeEvent(AttributeSet source, Attribute<T1> attr, object value)
		{
			this.source = source;
			this.attr = attr;
			this.value = value;
		}

		public AttributeEvent(AttributeSet source) : this(source, null, null)
		{
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Attribute<?> getAttribute()
		public virtual Attribute<object> Attribute
		{
			get
			{
				return attr;
			}
		}

		public virtual AttributeSet Source
		{
			get
			{
				return source;
			}
		}

		public virtual object Value
		{
			get
			{
				return value;
			}
		}
	}

}
