// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{
	public class TruthTableEvent
	{
		private TruthTable source;
		private int column;
		private object data;

		public TruthTableEvent(TruthTable source, VariableListEvent @event)
		{
			this.source = source;
			this.data = @event;
		}

		public TruthTableEvent(TruthTable source, int column)
		{
			this.source = source;
			this.column = column;
		}

		public virtual int Column
		{
			get
			{
				return column;
			}
		}

		public virtual TruthTable Source
		{
			get
			{
				return source;
			}
		}

		public virtual object Data
		{
			get
			{
				return data;
			}
		}
	}

}
