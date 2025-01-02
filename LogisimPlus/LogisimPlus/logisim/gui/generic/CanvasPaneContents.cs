// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{

	public interface CanvasPaneContents : Scrollable
	{
		CanvasPane CanvasPane {set;}

		void recomputeSize();

		// from Scrollable
		Size PreferredScrollableViewportSize {get;}

		int getScrollableBlockIncrement(Rectangle visibleRect, int orientation, int direction);

		bool ScrollableTracksViewportHeight {get;}

		bool ScrollableTracksViewportWidth {get;}

		int getScrollableUnitIncrement(Rectangle visibleRect, int orientation, int direction);
	}

}
