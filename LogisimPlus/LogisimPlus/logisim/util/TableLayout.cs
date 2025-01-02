// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class TableLayout : LayoutManager2
	{
		private int colCount;
		private List<Component[]> contents;
		private int curRow;
		private int curCol;
		private Size prefs;
		private int[] prefRow;
		private int[] prefCol;
		private double[] rowWeight;

		public TableLayout(int colCount)
		{
			this.colCount = colCount;
			this.contents = new List<Component[]>();
			this.curRow = 0;
			this.curCol = 0;
		}

		public virtual void setRowWeight(int rowIndex, double weight)
		{
			if (weight < 0)
			{
				throw new System.ArgumentException("weight must be nonnegative");
			}
			if (rowIndex < 0)
			{
				throw new System.ArgumentException("row index must be nonnegative");
			}
			if ((rowWeight == null || rowIndex >= rowWeight.Length) && weight != 0.0)
			{
				double[] a = new double[rowIndex + 10];
				if (rowWeight != null)
				{
					Array.Copy(rowWeight, 0, a, 0, rowWeight.Length);
				}
				rowWeight = a;
			}
			rowWeight[rowIndex] = weight;
		}

		public virtual void addLayoutComponent(string name, Component comp)
		{
			while (curRow >= contents.Count)
			{
				contents.Add(new Component[colCount]);
			}
			Component[] rowContents = contents[curRow];
			rowContents[curCol] = comp;
			++curCol;
			if (curCol == colCount)
			{
				++curRow;
				curCol = 0;
			}
			prefs = null;
		}

		public virtual void addLayoutComponent(Component comp, object constraints)
		{
			if (constraints is TableConstraints)
			{
				TableConstraints con = (TableConstraints) constraints;
				if (con.Row >= 0)
				{
					curRow = con.Row;
				}
				if (con.Col >= 0)
				{
					curCol = con.Col;
				}
			}
			addLayoutComponent("", comp);
		}

		public virtual void removeLayoutComponent(Component comp)
		{
			for (int i = 0, n = contents.Count; i < n; i++)
			{
				Component[] row = contents[i];
				for (int j = 0; j < row.Length; j++)
				{
					if (row[j] == comp)
					{
						row[j] = null;
						return;
					}
				}
			}
			prefs = null;
		}

		public virtual Size preferredLayoutSize(Container parent)
		{
			if (prefs == null)
			{
				int[] prefCol = new int[colCount];
				int[] prefRow = new int[contents.Count];
				int height = 0;
				for (int i = 0; i < prefRow.Length; i++)
				{
					Component[] row = contents[i];
					int rowHeight = 0;
					for (int j = 0; j < row.Length; j++)
					{
						if (row[j] != null)
						{
							Size dim = row[j].getPreferredSize();
							if (dim.height > rowHeight)
							{
								rowHeight = dim.height;
							}
							if (dim.width > prefCol[j])
							{
								prefCol[j] = dim.width;
							}
						}
					}
					prefRow[i] = rowHeight;
					height += rowHeight;
				}
				int width = 0;
				for (int i = 0; i < prefCol.Length; i++)
				{
					width += prefCol[i];
				}
				this.prefs = new Size(width, height);
				this.prefRow = prefRow;
				this.prefCol = prefCol;
			}
			return new Size(prefs);
		}

		public virtual Size minimumLayoutSize(Container parent)
		{
			return preferredLayoutSize(parent);
		}

		public virtual Size maximumLayoutSize(Container parent)
		{
			return new Size(int.MaxValue, int.MaxValue);
		}

		public virtual float getLayoutAlignmentX(Container parent)
		{
			return 0.5f;
		}

		public virtual float getLayoutAlignmentY(Container parent)
		{
			return 0.5f;
		}

		public virtual void layoutContainer(Container parent)
		{
			Size pref = preferredLayoutSize(parent);
			int[] prefRow = this.prefRow;
			int[] prefCol = this.prefCol;
			Size size = parent.getSize();

			double y0;
			int yRemaining = size.height - pref.height;
			double rowWeightTotal = 0.0;
			if (yRemaining != 0 && rowWeight != null)
			{
				foreach (double weight in rowWeight)
				{
					rowWeightTotal += weight;
				}
			}
			if (rowWeightTotal == 0.0 && yRemaining > 0)
			{
				y0 = yRemaining / 2.0;
			}
			else
			{
				y0 = 0;
			}

			int x0 = (size.width - pref.width) / 2;
			if (x0 < 0)
			{
				x0 = 0;
			}
			double y = y0;
			int i = -1;
			foreach (Component[] row in contents)
			{
				i++;
				int yRound = (int)(y + 0.5);
				int x = x0;
				for (int j = 0; j < row.Length; j++)
				{
					Component comp = row[j];
					if (comp != null)
					{
						row[j].setBounds(x, yRound, prefCol[j], prefRow[i]);
					}
					x += prefCol[j];
				}
				y += prefRow[i];
				if (rowWeightTotal > 0 && i < rowWeight.Length)
				{
					y += yRemaining * rowWeight[i] / rowWeightTotal;
				}
			}

		}

		public virtual void invalidateLayout(Container parent)
		{
			prefs = null;
		}

	}

}
