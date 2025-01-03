// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{

	public class TruthTable
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private static readonly Entry DEFAULT_ENTRY = Entry.DONT_CARE;

		private class MyListener : VariableListListener
		{
			private readonly TruthTable outerInstance;

			public MyListener(TruthTable outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void listChanged(VariableListEvent @event)
			{
				if (@event.Source == outerInstance.model.Inputs)
				{
					inputsChanged(@event);
				}
				else
				{
					outputsChanged(@event);
				}
				outerInstance.fireStructureChanged(@event);
			}

			internal virtual void inputsChanged(VariableListEvent @event)
			{
				int action = @event.Type;
				if (action == VariableListEvent.ADD)
				{
					foreach (KeyValuePair<string, Entry[]> curEntry in outerInstance.outputColumns.SetOfKeyValuePairs())
					{
						string output = curEntry.Key;
						Entry[] column = curEntry.Value;
						Entry[] newColumn = new Entry[2 * column.Length];
						for (int i = 0; i < column.Length; i++)
						{
							newColumn[2 * i] = column[i];
							newColumn[2 * i + 1] = column[i];
						}
						outerInstance.outputColumns[output] = newColumn;
					}
				}
				else if (action == VariableListEvent.REMOVE)
				{
					int index = ((int?) @event.Data).Value;
					foreach (KeyValuePair<string, Entry[]> curEntry in outerInstance.outputColumns.SetOfKeyValuePairs())
					{
						string output = curEntry.Key;
						Entry[] column = curEntry.Value;
						Entry[] newColumn = removeInput(column, index);
						outerInstance.outputColumns[output] = newColumn;
					}
				}
				else if (action == VariableListEvent.MOVE)
				{
					int delta = ((int?) @event.Data).Value;
					int newIndex = outerInstance.model.Inputs.indexOf(@event.Variable);
					foreach (KeyValuePair<string, Entry[]> curEntry in outerInstance.outputColumns.SetOfKeyValuePairs())
					{
						string output = curEntry.Key;
						Entry[] column = curEntry.Value;
						Entry[] newColumn = moveInput(column, newIndex - delta, newIndex);
						outerInstance.outputColumns[output] = newColumn;
					}
				}
			}

			internal virtual void outputsChanged(VariableListEvent @event)
			{
				int action = @event.Type;
				if (action == VariableListEvent.ALL_REPLACED)
				{
					outerInstance.outputColumns.Clear();
				}
				else if (action == VariableListEvent.REMOVE)
				{
					outerInstance.outputColumns.Remove(@event.Variable);
				}
				else if (action == VariableListEvent.REPLACE)
				{
					Entry[] column = outerInstance.outputColumns.Remove(@event.Variable);
					if (column != null)
					{
						int index = ((int?) @event.Data).Value;
						string newVariable = outerInstance.model.Outputs.get(index);
						outerInstance.outputColumns[newVariable] = column;
					}
				}
			}

			internal virtual Entry[] removeInput(Entry[] old, int index)
			{
				int oldInputCount = outerInstance.model.Inputs.size() + 1;
				Entry[] ret = new Entry[old.Length / 2];
				int j = 0;
				int mask = 1 << (oldInputCount - 1 - index);
				for (int i = 0; i < old.Length; i++)
				{
					if ((i & mask) == 0)
					{
						Entry e0 = old[i];
						Entry e1 = old[i | mask];
						ret[j] = (e0 == e1 ? e0 : Entry.DONT_CARE);
						j++;
					}
				}
				return ret;
			}

			internal virtual Entry[] moveInput(Entry[] old, int oldIndex, int newIndex)
			{
				int inputs = outerInstance.model.Inputs.size();
				oldIndex = inputs - 1 - oldIndex;
				newIndex = inputs - 1 - newIndex;
				Entry[] ret = new Entry[old.Length];
				int sameMask = (old.Length - 1) ^ ((1 << (1 + Math.Max(oldIndex, newIndex))) - 1) ^ ((1 << Math.Min(oldIndex, newIndex)) - 1); // bits that don't change
				int moveMask = 1 << oldIndex; // bit that moves
				int moveDist = Math.Abs(newIndex - oldIndex);
				bool moveLeft = newIndex > oldIndex;
				int blockMask = (old.Length - 1) ^ sameMask ^ moveMask; // bits that move by one
				for (int i = 0; i < old.Length; i++)
				{
					int j; // new index
					if (moveLeft)
					{
						j = (i & sameMask) | ((i & moveMask) << moveDist) | ((i & blockMask) >> 1);
					}
					else
					{
						j = (i & sameMask) | ((i & moveMask) >> moveDist) | ((i & blockMask) << 1);
					}
					ret[j] = old[i];
				}
				return ret;
			}
		}

		private MyListener myListener;
		private List<TruthTableListener> listeners = new List<TruthTableListener>();
		private AnalyzerModel model;
		private Dictionary<string, Entry[]> outputColumns = new Dictionary<string, Entry[]>();

		public TruthTable(AnalyzerModel model)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.model = model;
			model.Inputs.addVariableListListener(myListener);
			model.Outputs.addVariableListListener(myListener);
		}

		public virtual void addTruthTableListener(TruthTableListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeTruthTableListener(TruthTableListener l)
		{
			listeners.Remove(l);
		}

		private void fireCellsChanged(int column)
		{
			TruthTableEvent @event = new TruthTableEvent(this, column);
			foreach (TruthTableListener l in listeners)
			{
				l.cellsChanged(@event);
			}
		}

		private void fireStructureChanged(VariableListEvent cause)
		{
			TruthTableEvent @event = new TruthTableEvent(this, cause);
			foreach (TruthTableListener l in listeners)
			{
				l.structureChanged(@event);
			}
		}

		public virtual int RowCount
		{
			get
			{
				int sz = model.Inputs.size();
				return 1 << sz;
			}
		}

		public virtual int InputColumnCount
		{
			get
			{
				return model.Inputs.size();
			}
		}

		public virtual int OutputColumnCount
		{
			get
			{
				return model.Outputs.size();
			}
		}

		public virtual string getInputHeader(int column)
		{
			return model.Inputs.get(column);
		}

		public virtual string getOutputHeader(int column)
		{
			return model.Outputs.get(column);
		}

		public virtual int getInputIndex(string input)
		{
			return model.Inputs.IndexOf(input);
		}

		public virtual int getOutputIndex(string output)
		{
			return model.Outputs.IndexOf(output);
		}

		public virtual Entry getInputEntry(int row, int column)
		{
			int rows = RowCount;
			int inputs = model.Inputs.size();
			if (row < 0 || row >= rows)
			{
				throw new System.ArgumentException("row index: " + row + " size: " + rows);
			}
			if (column < 0 || column >= inputs)
			{
				throw new System.ArgumentException("column index: " + column + " size: " + inputs);
			}

			return isInputSet(row, column, inputs) ? Entry.ONE : Entry.ZERO;
		}

		public virtual Entry getOutputEntry(int row, int column)
		{
			int outputs = model.Outputs.size();
			if (row < 0 || row >= RowCount || column < 0 || column >= outputs)
			{
				return Entry.DONT_CARE;
			}
			else
			{
				string outputName = model.Outputs.get(column);
				Entry[] columnData = outputColumns[outputName];
				if (columnData == null)
				{
					return DEFAULT_ENTRY;
				}
				if (row < 0 || row >= columnData.Length)
				{
					return Entry.DONT_CARE;
				}
				return columnData[row];
			}
		}

		public virtual void setOutputEntry(int row, int column, Entry value)
		{
			int rows = RowCount;
			int outputs = model.Outputs.size();
			if (row < 0 || row >= rows)
			{
				throw new System.ArgumentException("row index: " + row + " size: " + rows);
			}
			if (column < 0 || column >= outputs)
			{
				throw new System.ArgumentException("column index: " + column + " size: " + outputs);
			}

			string outputName = model.Outputs.get(column);
			Entry[] columnData = outputColumns[outputName];

			if (columnData == null)
			{
				if (value == DEFAULT_ENTRY)
				{
					return;
				}
				columnData = new Entry[RowCount];
				outputColumns[outputName] = columnData;
				Arrays.Fill(columnData, DEFAULT_ENTRY);
				columnData[row] = value;
			}
			else
			{
				if (columnData[row] == value)
				{
					return;
				}
				columnData[row] = value;
			}

			fireCellsChanged(column);
		}

		public virtual Entry[] getOutputColumn(int column)
		{
			int outputs = model.Outputs.size();
			if (column < 0 || column >= outputs)
			{
				throw new System.ArgumentException("index: " + column + " size: " + outputs);
			}

			string outputName = model.Outputs.get(column);
			Entry[] columnData = outputColumns[outputName];
			if (columnData == null)
			{
				columnData = new Entry[RowCount];
				Arrays.Fill(columnData, DEFAULT_ENTRY);
				outputColumns[outputName] = columnData;
			}
			return columnData;
		}

		public virtual void setOutputColumn(int column, Entry[] values)
		{
			if (values != null && values.Length != RowCount)
			{
				throw new System.ArgumentException("argument to setOutputColumn is wrong length");
			}

			int outputs = model.Outputs.size();
			if (column < 0 || column >= outputs)
			{
				throw new System.ArgumentException("index: " + column + " size: " + outputs);
			}

			string outputName = model.Outputs.get(column);
			Entry[] oldValues = outputColumns[outputName];
			if (oldValues == values)
			{
				return;
			}
			else if (values == null)
			{
				outputColumns.Remove(outputName);
			}
			else
			{
				outputColumns[outputName] = values;
			}
			fireCellsChanged(column);
		}

		public static bool isInputSet(int row, int column, int inputs)
		{
			return ((row >> (inputs - 1 - column)) & 0x1) == 1;
		}
	}

}
