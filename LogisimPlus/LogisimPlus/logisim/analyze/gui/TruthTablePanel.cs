// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{
    using LogisimPlus.Java;
    using Entry = logisim.analyze.model.Entry;
	using TruthTable = logisim.analyze.model.TruthTable;

	internal interface TruthTablePanel
	{
		public static Color ERROR_COLOR = Color.FromArgb(255, 255, 128, 128);

		TruthTable TruthTable {get;}

		int getOutputColumn(MouseEvent @event);

		int getRow(MouseEvent @event);

		void setEntryProvisional(int row, int col, Entry value);
	}

}
