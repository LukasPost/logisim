// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{
	using Value = logisim.data.Value;

	internal interface ModelListener
	{
		void selectionChanged(ModelEvent @event);

		void entryAdded(ModelEvent @event, Value[] values);

		void filePropertyChanged(ModelEvent @event);
	}

}
