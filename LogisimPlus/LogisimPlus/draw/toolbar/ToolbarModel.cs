// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.toolbar
{

	public interface ToolbarModel
	{
		void addToolbarModelListener(ToolbarModelListener listener);

		void removeToolbarModelListener(ToolbarModelListener listener);

		IList<ToolbarItem> Items {get;}

		bool isSelected(ToolbarItem item);

		void itemSelected(ToolbarItem item);
	}

}
