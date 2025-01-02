// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using Value = logisim.data.Value;
	using TtyInterface = logisim.gui.start.TtyInterface;
	using InstanceData = logisim.instance.InstanceData;

	internal class TtyState : InstanceData, ICloneable
	{
		private Value lastClock;
		private string[] rowData;
		private int colCount;
		private StringBuilder lastRow;
		private int row;
		private bool sendStdout;

		public TtyState(int rows, int cols)
		{
			lastClock = Value.UNKNOWN;
			rowData = new string[rows - 1];
			colCount = cols;
			lastRow = new StringBuilder(cols);
			sendStdout = false;
			clear();
		}

		public virtual TtyState clone()
		{
			try
			{
				TtyState ret = (TtyState) base.clone();
				ret.rowData = (string[])this.rowData.Clone();
				return ret;
			}
			catch (CloneNotSupportedException)
			{
				return null;
			}
		}

		public virtual Value setLastClock(Value newClock)
		{
			Value ret = lastClock;
			lastClock = newClock;
			return ret;
		}

		public virtual bool SendStdout
		{
			set
			{
				sendStdout = value;
			}
		}

		public virtual void clear()
		{
			Arrays.Fill(rowData, "");
			lastRow.Remove(0, lastRow.Length);
			row = 0;
		}

		public virtual string getRowString(int index)
		{
			if (index < row)
			{
				return rowData[index];
			}
			else if (index == row)
			{
				return lastRow.ToString();
			}
			else
			{
				return "";
			}
		}

		public virtual int CursorRow
		{
			get
			{
				return row;
			}
		}

		public virtual int CursorColumn
		{
			get
			{
				return lastRow.Length;
			}
		}

		public virtual void add(char c)
		{
			if (sendStdout)
			{
				TtyInterface.sendFromTty(c);
			}

			int lastLength = lastRow.Length;
			switch (c)
			{
			case 12: // control-L
				row = 0;
				lastRow.Remove(0, lastLength);
				Arrays.Fill(rowData, "");
				break;
			case '\b': // backspace
				if (lastLength > 0)
				{
					lastRow.Remove(lastLength - 1, lastLength - lastLength - 1);
				}
				break;
			case '\n':
			case '\r': // newline
				commit();
				break;
			default:
				if (!char.IsControl(c))
				{
					if (lastLength == colCount)
					{
						commit();
					}
					lastRow.Append(c);
				}
			break;
			}
		}

		private void commit()
		{
			if (row >= rowData.Length)
			{
				Array.Copy(rowData, 1, rowData, 0, rowData.Length - 1);
				rowData[row - 1] = lastRow.ToString();
			}
			else
			{
				rowData[row] = lastRow.ToString();
				row++;
			}
			lastRow.Remove(0, lastRow.Length);
		}

		public virtual void updateSize(int rows, int cols)
		{
			int oldRows = rowData.Length + 1;
			if (rows != oldRows)
			{
				string[] newData = new string[rows - 1];
				if (rows > oldRows || row < rows - 1)
				{ // or rows removed but filled rows fit
					Array.Copy(rowData, 0, newData, 0, row);
					Arrays.Fill(newData, row, rows - 1, "");
				}
				else
				{ // rows removed, and some filled rows must go
					Array.Copy(rowData, row - rows + 1, newData, 0, rows - 1);
					row = rows - 1;
				}
				rowData = newData;
			}

			int oldCols = colCount;
			if (cols != oldCols)
			{
				colCount = cols;
				if (cols < oldCols)
				{ // will need to trim any long rows
					for (int i = 0; i < rows - 1; i++)
					{
						string s = rowData[i];
						if (s.Length > cols)
						{
							rowData[i] = s.Substring(0, cols);
						}
					}
					if (lastRow.Length > cols)
					{
						lastRow.Remove(cols, lastRow.Length - cols);
					}
				}
			}
		}
	}

}
