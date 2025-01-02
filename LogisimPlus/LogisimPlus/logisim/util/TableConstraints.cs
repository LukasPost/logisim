// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{
	public class TableConstraints
	{
		public static TableConstraints at(int row, int col)
		{
			return new TableConstraints(row, col);
		}

		private int col;
		private int row;

		private TableConstraints(int row, int col)
		{
			this.col = col;
			this.row = row;
		}

		internal virtual int Row
		{
			get
			{
				return row;
			}
		}

		internal virtual int Col
		{
			get
			{
				return col;
			}
		}
	}

}
