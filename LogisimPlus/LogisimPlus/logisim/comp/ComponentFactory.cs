// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{

	using LogisimVersion = logisim.LogisimVersion;
	using logisim.data;
	using AttributeDefaultProvider = logisim.data.AttributeDefaultProvider;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using StringGetter = logisim.util.StringGetter;

	/// <summary>
	/// Represents a category of components that appear in a circuit. This class and <code>Component</code> share the same
	/// sort of relationship as the relation between <em>classes</em> and <em>instances</em> in Java. Normally, there is only
	/// one ComponentFactory created for any particular category.
	/// </summary>
	public interface ComponentFactory : AttributeDefaultProvider
	{
		public static object SHOULD_SNAP = new object();
		public static object TOOL_TIP = new object();
		public static object FACING_ATTRIBUTE_KEY = new object();

		string Name {get;}

		string DisplayName {get;}

		StringGetter DisplayGetter {get;}

		Component createComponent(Location loc, AttributeSet attrs);

		Bounds getOffsetBounds(AttributeSet attrs);

		AttributeSet createAttributeSet();

		bool isAllDefaultValues(AttributeSet attrs, LogisimVersion ver);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Object getDefaultAttributeValue(logisim.data.Attribute<?> attr, logisim.LogisimVersion ver);
		object getDefaultAttributeValue<T1>(Attribute<T1> attr, LogisimVersion ver);

		void drawGhost(ComponentDrawContext context, Color color, int x, int y, AttributeSet attrs);

		void paintIcon(ComponentDrawContext context, int x, int y, AttributeSet attrs);

		/// <summary>
		/// Retrieves special-purpose features for this factory. This technique allows for future Logisim versions to add new
		/// features for components without requiring changes to existing components. It also removes the necessity for the
		/// Component API to directly declare methods for each individual feature. In most cases, the <code>key</code> is a
		/// <code>Class</code> object corresponding to an interface, and the method should return an implementation of that
		/// interface if it supports the feature.
		/// 
		/// As of this writing, possible values for <code>key</code> include: <code>TOOL_TIP</code> (return a
		/// <code>String</code>) and <code>SHOULD_SNAP</code> (return a <code>Boolean</code>).
		/// </summary>
		/// <param name="key"> an object representing a feature. </param>
		/// <returns> an object representing information about how the component supports the feature, or <code>null</code> if
		///         it does not support the feature. </returns>
		object getFeature(object key, AttributeSet attrs);
	}

}
