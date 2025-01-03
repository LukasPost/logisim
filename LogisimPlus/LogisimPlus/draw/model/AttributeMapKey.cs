// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{
	using logisim.data;

	public class AttributeMapKey
	{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private logisim.data.Attribute<?> attr;
		private Attribute attr;
		private CanvasObject @object;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: public AttributeMapKey(logisim.data.Attribute<?> attr, CanvasObject object)
		public AttributeMapKey(Attribute attr, CanvasObject @object)
		{
			this.attr = attr;
			this.@object = @object;
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public logisim.data.Attribute<?> getAttribute()
		public virtual Attribute Attribute
		{
			get
			{
				return attr;
			}
		}

		public virtual CanvasObject Object
		{
			get
			{
				return @object;
			}
		}

		public override int GetHashCode()
		{
			int a = attr == null ? 0 : attr.GetHashCode();
			int b = @object == null ? 0 : @object.GetHashCode();
			return a ^ b;
		}

		public override bool Equals(object other)
		{
			if (!(other is AttributeMapKey))
			{
				return false;
			}
			AttributeMapKey o = (AttributeMapKey) other;
			return (attr == null ? o.attr == null : attr.Equals(o.attr)) && (@object == null ? o.@object == null : @object.Equals(o.@object));
		}
	}

}
