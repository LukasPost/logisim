// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.key
{

	using logisim.data;

	public class KeyConfigurationResult
	{
		private KeyConfigurationEvent @event;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.Map<logisim.data.Attribute<?>, Object> attrValueMap;
		private Dictionary<Attribute, object> attrValueMap;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: public KeyConfigurationResult(KeyConfigurationEvent event, logisim.data.Attribute<?> attr, Object value)
		public KeyConfigurationResult(KeyConfigurationEvent @event, Attribute attr, object value)
		{
			this.@event = @event;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Map<logisim.data.Attribute<?>, Object> singleMap = new java.util.HashMap<logisim.data.Attribute<?>, Object>(1);
			Dictionary<Attribute, object> singleMap = new Dictionary<Attribute, object>(1);
			singleMap[attr] = value;
			this.attrValueMap = singleMap;
		}

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: public KeyConfigurationResult(KeyConfigurationEvent event, java.util.Map<logisim.data.Attribute<?>, Object> values)
		public KeyConfigurationResult(KeyConfigurationEvent @event, Dictionary<Attribute, object> values)
		{
			this.@event = @event;
			this.attrValueMap = values;
		}

		public virtual KeyConfigurationEvent Event
		{
			get
			{
				return @event;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.Map<logisim.data.Attribute<?>, Object> getAttributeValues()
		public virtual Dictionary<Attribute, object> AttributeValues
		{
			get
			{
				return attrValueMap;
			}
		}
	}

}
