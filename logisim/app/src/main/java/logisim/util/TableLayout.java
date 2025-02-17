/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

import java.awt.Component;
import java.awt.Container;
import java.awt.Dimension;
import java.awt.LayoutManager2;
import java.util.ArrayList;

public class TableLayout implements LayoutManager2 {
	private int colCount;
	private ArrayList<Component[]> contents;
	private int curRow;
	private int curCol;
	private Dimension prefs;
	private int[] prefRow;
	private int[] prefCol;
	private double[] rowWeight;

	public TableLayout(int colCount) {
		this.colCount = colCount;
		contents = new ArrayList<>();
		curRow = 0;
		curCol = 0;
	}

	public void setRowWeight(int rowIndex, double weight) {
		if (weight < 0) throw new IllegalArgumentException("weight must be nonnegative");
		if (rowIndex < 0) throw new IllegalArgumentException("row index must be nonnegative");
		if ((rowWeight == null || rowIndex >= rowWeight.length) && weight != 0.0) {
			double[] a = new double[rowIndex + 10];
			if (rowWeight != null)
				System.arraycopy(rowWeight, 0, a, 0, rowWeight.length);
			rowWeight = a;
		}
		rowWeight[rowIndex] = weight;
	}

	public void addLayoutComponent(String name, Component comp) {
		while (curRow >= contents.size()) contents.add(new Component[colCount]);
		Component[] rowContents = contents.get(curRow);
		rowContents[curCol] = comp;
		++curCol;
		if (curCol == colCount) {
			++curRow;
			curCol = 0;
		}
		prefs = null;
	}

	public void addLayoutComponent(Component comp, Object constraints) {
		if (constraints instanceof TableConstraints con) {
			if (con.getRow() >= 0)
				curRow = con.getRow();
			if (con.getCol() >= 0)
				curCol = con.getCol();
		}
		addLayoutComponent("", comp);
	}

	public void removeLayoutComponent(Component comp) {
		for (Component[] row : contents)
			for (int j = 0; j < row.length; j++)
				if (row[j] == comp) {
					row[j] = null;
					return;
				}
		prefs = null;
	}

	public Dimension preferredLayoutSize(Container parent) {
		if (prefs == null) {
			int[] prefCol = new int[colCount];
			int[] prefRow = new int[contents.size()];
			int height = 0;
			for (int i = 0; i < prefRow.length; i++) {
				Component[] row = contents.get(i);
				int rowHeight = 0;
				for (int j = 0; j < row.length; j++)
					if (row[j] != null) {
						Dimension dim = row[j].getPreferredSize();
						if (dim.height > rowHeight)
							rowHeight = dim.height;
						if (dim.width > prefCol[j])
							prefCol[j] = dim.width;
					}
				prefRow[i] = rowHeight;
				height += rowHeight;
			}
			int width = 0;
			for (int j : prefCol) width += j;
			prefs = new Dimension(width, height);
			this.prefRow = prefRow;
			this.prefCol = prefCol;
		}
		return new Dimension(prefs);
	}

	public Dimension minimumLayoutSize(Container parent) {
		return preferredLayoutSize(parent);
	}

	public Dimension maximumLayoutSize(Container parent) {
		return new Dimension(Integer.MAX_VALUE, Integer.MAX_VALUE);
	}

	public float getLayoutAlignmentX(Container parent) {
		return 0.5f;
	}

	public float getLayoutAlignmentY(Container parent) {
		return 0.5f;
	}

	public void layoutContainer(Container parent) {
		Dimension pref = preferredLayoutSize(parent);
		int[] prefRow = this.prefRow;
		int[] prefCol = this.prefCol;
		Dimension size = parent.getSize();

		double y0;
		int yRemaining = size.height - pref.height;
		double rowWeightTotal = 0.0;
		if (yRemaining != 0 && rowWeight != null) for (double weight : rowWeight) rowWeightTotal += weight;
		if (rowWeightTotal == 0.0 && yRemaining > 0) y0 = yRemaining / 2.0;
		else y0 = 0;

		int x0 = (size.width - pref.width) / 2;
		if (x0 < 0)
			x0 = 0;
		double y = y0;
		int i = -1;
		for (Component[] row : contents) {
			i++;
			int yRound = (int) (y + 0.5);
			int x = x0;
			for (int j = 0; j < row.length; j++) {
				Component comp = row[j];
				if (comp != null) row[j].setBounds(x, yRound, prefCol[j], prefRow[i]);
				x += prefCol[j];
			}
			y += prefRow[i];
			if (rowWeightTotal > 0 && i < rowWeight.length) y += yRemaining * rowWeight[i] / rowWeightTotal;
		}

	}

	public void invalidateLayout(Container parent) {
		prefs = null;
	}

}
