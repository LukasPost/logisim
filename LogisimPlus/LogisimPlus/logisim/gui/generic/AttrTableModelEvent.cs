// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{
	public class AttrTableModelEvent
	{
		private AttrTableModel model;
		private int index;

		public AttrTableModelEvent(AttrTableModel model) : this(model, -1)
		{
		}

		public AttrTableModelEvent(AttrTableModel model, int index)
		{
			this.model = model;
			this.index = index;
		}

		public virtual object Source
		{
			get
			{
				return model;
			}
		}

		public virtual AttrTableModel Model
		{
			get
			{
				return model;
			}
		}

		public virtual int RowIndex
		{
			get
			{
				return index;
			}
		}
	}

}
