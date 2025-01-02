// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace hex
{
	public interface HexModel
	{
		/// <summary>
		/// Registers a listener for changes to the values. </summary>
		void addHexModelListener(HexModelListener l);

		/// <summary>
		/// Unregisters a listener for changes to the values. </summary>
		void removeHexModelListener(HexModelListener l);

		/// <summary>
		/// Returns the offset of the initial value to be displayed. </summary>
		long FirstOffset {get;}

		/// <summary>
		/// Returns the number of values to be displayed. </summary>
		long LastOffset {get;}

		/// <summary>
		/// Returns number of bits in each value. </summary>
		int ValueWidth {get;}

		/// <summary>
		/// Returns the value at the given address. </summary>
		int get(long address);

		/// <summary>
		/// Changes the value at the given address. </summary>
		void set(long address, int value);

		/// <summary>
		/// Changes a series of values at the given addresses. </summary>
		void set(long start, int[] values);

		/// <summary>
		/// Fills a series of values with the same value. </summary>
		void fill(long start, long length, int value);
	}

}
