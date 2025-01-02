// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{
	using Library = logisim.tools.Library;

	internal interface LibraryLoader
	{
		Library loadLibrary(string desc);

		string getDescriptor(Library lib);

		void showError(string description);
	}

}
