// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{

	using CircuitState = logisim.circuit.CircuitState;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;

	public interface Component
	{
		// listener methods
		void addComponentListener(ComponentListener l);

		void removeComponentListener(ComponentListener l);

		// basic information methods
		ComponentFactory Factory {get;}

		AttributeSet AttributeSet { get; protected set; }

		// location/extent methods
		Location Location {get;}

		Bounds Bounds {get;}

		Bounds getBounds(JGraphics g);

		bool contains(Location pt);

		bool contains(Location pt, JGraphics g);

		// user interface methods
		void expose(ComponentDrawContext context);

		void draw(ComponentDrawContext context);

		/// <summary>
		/// Retrieves information about a special-purpose feature for this component. This technique allows future Logisim
		/// versions to add new features for components without requiring changes to existing components. It also removes the
		/// necessity for the Component API to directly declare methods for each individual feature. In most cases, the
		/// <code>key</code> is a <code>Class</code> object corresponding to an interface, and the method should return an
		/// implementation of that interface if it supports the feature.
		/// 
		/// As of this writing, possible values for <code>key</code> include: <code>Pokable.class</code>,
		/// <code>CustomHandles.class</code>, <code>WireRepair.class</code>, <code>TextEditable.class</code>,
		/// <code>MenuExtender.class</code>, <code>ToolTipMaker.class</code>, <code>ExpressionComputer.class</code>, and
		/// <code>Loggable.class</code>.
		/// </summary>
		/// <param name="key"> an object representing a feature. </param>
		/// <returns> an object representing information about how the component supports the feature, or <code>null</code> if
		///         it does not support the feature. </returns>
		object getFeature(object key);

		// propagation methods
		List<EndData> Ends {get;}

		EndData getEnd(int index);

		bool endsAt(Location pt);

		void propagate(CircuitState state);
	}

}
