// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

using LogisimPlus.Java;

namespace draw.toolbar
{

	public interface ToolbarItem
	{
		bool Selectable {get;}

		void paintIcon(JComponent destination, JGraphics g);

		string ToolTip {get;}

		Size getSize(object orientation);
	}

}
