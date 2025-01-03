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

	internal class TruthTableMouseListener : MouseListener
	{
		private int cellX;
		private int cellY;
		private Entry oldValue;
		private Entry newValue;

		public virtual void mousePressed(MouseEvent @event)
		{
			TruthTablePanel source = (TruthTablePanel) @event.getSource();
			TruthTable model = source.TruthTable;
			int cols = model.InputColumnCount + model.OutputColumnCount;
			int rows = model.RowCount;
			cellX = source.getOutputColumn(@event);
			cellY = source.getRow(@event);
			if (cellX < 0 || cellY < 0 || cellX >= cols || cellY >= rows)
			{
				return;
			}
			oldValue = source.TruthTable.getOutputEntry(cellY, cellX);
			if (oldValue == Entry.ZERO)
			{
				newValue = Entry.ONE;
			}
			else if (oldValue == Entry.ONE)
			{
				newValue = Entry.DONT_CARE;
			}
			else
			{
				newValue = Entry.ZERO;
			}
			source.setEntryProvisional(cellY, cellX, newValue);
		}

		public virtual void mouseReleased(MouseEvent @event)
		{
			TruthTablePanel source = (TruthTablePanel) @event.getSource();
			TruthTable model = source.TruthTable;
			int cols = model.InputColumnCount + model.OutputColumnCount;
			int rows = model.RowCount;
			if (cellX < 0 || cellY < 0 || cellX >= cols || cellY >= rows)
			{
				return;
			}

			int x = source.getOutputColumn(@event);
			int y = source.getRow(@event);
			TruthTable table = source.TruthTable;
			if (x == cellX && y == cellY)
			{
				table.setOutputEntry(y, x, newValue);
			}
			source.setEntryProvisional(cellY, cellX, null);
			cellX = -1;
			cellY = -1;
		}

		public virtual void mouseClicked(MouseEvent e)
		{
		}

		public virtual void mouseEntered(MouseEvent e)
		{
		}

		public virtual void mouseExited(MouseEvent e)
		{
		}
	}

}
