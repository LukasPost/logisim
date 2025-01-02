// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{

	public interface ZoomModel
	{
		public static string ZOOM = "zoom";
		public static string SHOW_GRID = "grid";

		void addPropertyChangeListener(string prop, PropertyChangeListener l);

		void removePropertyChangeListener(string prop, PropertyChangeListener l);

		bool ShowGrid {get;set;}

		double ZoomFactor {get;set;}

		double[] ZoomOptions {get;}


	}

}
