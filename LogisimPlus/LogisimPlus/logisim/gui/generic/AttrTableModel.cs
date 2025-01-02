﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{
	public interface AttrTableModel
	{
		void addAttrTableModelListener(AttrTableModelListener listener);

		void removeAttrTableModelListener(AttrTableModelListener listener);

		string Title {get;}

		int RowCount {get;}

		AttrTableModelRow getRow(int rowIndex);
	}

}
