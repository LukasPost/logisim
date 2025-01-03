// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{


	using Entry = logisim.analyze.model.Entry;
	using TruthTable = logisim.analyze.model.TruthTable;
	using TruthTableEvent = logisim.analyze.model.TruthTableEvent;
	using TruthTableListener = logisim.analyze.model.TruthTableListener;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	internal class TableTab : JPanel, TruthTablePanel, TabInterface
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private static readonly Font HEAD_FONT = new Font("Serif", Font.BOLD, 14);
		private static readonly Font BODY_FONT = new Font("Serif", Font.PLAIN, 14);
		private const int COLUMN_SEP = 8;
		private const int HEADER_SEP = 4;

		private class MyListener : TruthTableListener
		{
			private readonly TableTab outerInstance;

			public MyListener(TableTab outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void cellsChanged(TruthTableEvent @event)
			{
				repaint();
			}

			public virtual void structureChanged(TruthTableEvent @event)
			{
				outerInstance.computePreferredSize();
			}
		}

		private MyListener myListener;
		private TruthTable table;
		private int cellWidth = 25; // reasonable start values
		private int cellHeight = 15;
		private int tableWidth;
		private int tableHeight;
		private int provisionalX;
		private int provisionalY;
		private Entry provisionalValue = null;
		private TableTabCaret caret;
		private TableTabClip clip;

		public TableTab(TruthTable table)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.table = table;
			table.addTruthTableListener(myListener);
			setToolTipText(" ");
			caret = new TableTabCaret(this);
			clip = new TableTabClip(this);
		}

		public virtual TruthTable TruthTable
		{
			get
			{
				return table;
			}
		}

		internal virtual TableTabCaret Caret
		{
			get
			{
				return caret;
			}
		}

		internal virtual void localeChanged()
		{
			computePreferredSize();
			repaint();
		}

		public virtual int getColumn(MouseEvent @event)
		{
			int x = @event.getX() - (getWidth() - tableWidth) / 2;
			if (x < 0)
			{
				return -1;
			}
			int inputs = table.InputColumnCount;
			int cols = inputs + table.OutputColumnCount;
			int ret = (x + COLUMN_SEP / 2) / (cellWidth + COLUMN_SEP);
			if (inputs == 0)
			{
				ret--;
			}
			return ret >= 0 ? ret < cols ? ret : cols : -1;
		}

		internal virtual int ColumnCount
		{
			get
			{
				int inputs = table.InputColumnCount;
				int outputs = table.OutputColumnCount;
				return inputs + outputs;
			}
		}

		public virtual int getOutputColumn(MouseEvent @event)
		{
			int inputs = table.InputColumnCount;
			if (inputs == 0)
			{
				inputs = 1;
			}
			int ret = getColumn(@event);
			return ret >= inputs ? ret - inputs : -1;
		}

		public virtual int getRow(MouseEvent @event)
		{
			int y = @event.getY() - (getHeight() - tableHeight) / 2;
			if (y < cellHeight + HEADER_SEP)
			{
				return -1;
			}
			int ret = (y - cellHeight - HEADER_SEP) / cellHeight;
			int rows = table.RowCount;
			return ret >= 0 ? ret < rows ? ret : rows : -1;
		}

		public virtual void setEntryProvisional(int y, int x, Entry value)
		{
			provisionalY = y;
			provisionalX = x;
			provisionalValue = value;

			int top = (getHeight() - tableHeight) / 2 + cellHeight + HEADER_SEP + y * cellHeight;
			repaint(0, top, getWidth(), cellHeight);
		}

		public override string getToolTipText(MouseEvent @event)
		{
			int row = getRow(@event);
			int col = getOutputColumn(@event);
			Entry entry = table.getOutputEntry(row, col);
			return entry.ErrorMessage;
		}

		public override void paintComponent(JGraphics g)
		{
			base.paintComponent(g);

			caret.paintBackground(g);

			Size sz = getSize();
			int top = Math.Max(0, (sz.height - tableHeight) / 2);
			int left = Math.Max(0, (sz.width - tableWidth) / 2);
			int inputs = table.InputColumnCount;
			int outputs = table.OutputColumnCount;
			if (inputs == 0 && outputs == 0)
			{
				g.setFont(BODY_FONT);
				JGraphicsUtil.drawCenteredText(g, Strings.get("tableEmptyMessage"), sz.width / 2, sz.height / 2);
				return;
			}

			g.setColor(Color.Gray);
			int lineX = left + (cellWidth + COLUMN_SEP) * inputs - COLUMN_SEP / 2;
			if (inputs == 0)
			{
				lineX = left + cellWidth + COLUMN_SEP / 2;
			}
			int lineY = top + cellHeight + HEADER_SEP / 2;
			g.drawLine(left, lineY, left + tableWidth, lineY);
			g.drawLine(lineX, top, lineX, top + tableHeight);

			g.setColor(Color.Black);
			g.setFont(HEAD_FONT);
			FontMetrics headerMetric = g.getFontMetrics();
			int x = left;
			int y = top + headerMetric.getAscent() + 1;
			if (inputs == 0)
			{
				x = paintHeader(Strings.get("tableNullHeader"), x, y, g, headerMetric);
			}
			else
			{
				for (int i = 0; i < inputs; i++)
				{
					x = paintHeader(table.getInputHeader(i), x, y, g, headerMetric);
				}
			}
			if (outputs == 0)
			{
				x = paintHeader(Strings.get("tableNullHeader"), x, y, g, headerMetric);
			}
			else
			{
				for (int i = 0; i < outputs; i++)
				{
					x = paintHeader(table.getOutputHeader(i), x, y, g, headerMetric);
				}
			}

			g.setFont(BODY_FONT);
			FontMetrics bodyMetric = g.getFontMetrics();
			y = top + cellHeight + HEADER_SEP;
			Rectangle clip = g.getClipBounds();
			int firstRow = Math.Max(0, (clip.y - y) / cellHeight);
			int lastRow = Math.Min(table.RowCount, 2 + (clip.y + clip.height - y) / cellHeight);
			y += firstRow * cellHeight;
			if (inputs == 0)
			{
				left += cellWidth + COLUMN_SEP;
			}
			bool provisional = false;
			for (int i = firstRow; i < lastRow; i++)
			{
				x = left;
				for (int j = 0; j < inputs + outputs; j++)
				{
					Entry entry = j < inputs ? table.getInputEntry(i, j) : table.getOutputEntry(i, j - inputs);
					if (provisionalValue != null && i == provisionalY && j - inputs == provisionalX)
					{
						provisional = true;
						entry = provisionalValue;
					}
					if (entry.Error)
					{
						g.setColor(ERROR_COLOR);
						g.fillRect(x, y, cellWidth, cellHeight);
						g.setColor(Color.Black);
					}
					string label = entry.Description;
					int width = bodyMetric.stringWidth(label);
					if (provisional)
					{
						provisional = false;
						g.setColor(Color.GREEN);
						g.drawString(label, x + (cellWidth - width) / 2, y + bodyMetric.getAscent());
						g.setColor(Color.Black);
					}
					else
					{
						g.drawString(label, x + (cellWidth - width) / 2, y + bodyMetric.getAscent());
					}
					x += cellWidth + COLUMN_SEP;
				}
				y += cellHeight;
			}

			caret.paintForeground(g);
		}

		internal virtual int CellWidth
		{
			get
			{
				return cellWidth;
			}
		}

		internal virtual int CellHeight
		{
			get
			{
				return cellHeight;
			}
		}

		internal virtual int getX(int col)
		{
			Size sz = getSize();
			int left = Math.Max(0, (sz.width - tableWidth) / 2);
			int inputs = table.InputColumnCount;
			if (inputs == 0)
			{
				left += cellWidth + COLUMN_SEP;
			}
			return left + col * (cellWidth + COLUMN_SEP);
		}

		internal virtual int getY(int row)
		{
			Size sz = getSize();
			int top = Math.Max(0, (sz.height - tableHeight) / 2);
			return top + cellHeight + HEADER_SEP + row * cellHeight;
		}

		private int paintHeader(string header, int x, int y, JGraphics g, FontMetrics fm)
		{
			int width = fm.stringWidth(header);
			g.drawString(header, x + (cellWidth - width) / 2, y);
			return x + cellWidth + COLUMN_SEP;
		}

		private void computePreferredSize()
		{
			int inputs = table.InputColumnCount;
			int outputs = table.OutputColumnCount;
			if (inputs == 0 && outputs == 0)
			{
				setPreferredSize(new Size(0, 0));
				return;
			}

			JGraphics g = getJGraphics();
			if (g == null)
			{
				cellHeight = 16;
				cellWidth = 24;
			}
			else
			{
				FontMetrics fm = g.getFontMetrics(HEAD_FONT);
				cellHeight = fm.getHeight();
				cellWidth = 24;
				if (inputs == 0 || outputs == 0)
				{
					cellWidth = Math.Max(cellWidth, fm.stringWidth(Strings.get("tableNullHeader")));
				}
				for (int i = 0; i < inputs + outputs; i++)
				{
					string header = i < inputs ? table.getInputHeader(i) : table.getOutputHeader(i - inputs);
					cellWidth = Math.Max(cellWidth, fm.stringWidth(header));
				}
			}

			if (inputs == 0)
			{
				inputs = 1;
			}
			if (outputs == 0)
			{
				outputs = 1;
			}
			tableWidth = (cellWidth + COLUMN_SEP) * (inputs + outputs) - COLUMN_SEP;
			tableHeight = cellHeight * (1 + table.RowCount) + HEADER_SEP;
			setPreferredSize(new Size(tableWidth, tableHeight));
			revalidate();
			repaint();
		}

		internal virtual JScrollBar VerticalScrollBar
		{
			get
			{
				return new JScrollBarAnonymousInnerClass(this);
			}
		}

		private class JScrollBarAnonymousInnerClass : JScrollBar
		{
			private readonly TableTab outerInstance;

			public JScrollBarAnonymousInnerClass(TableTab outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override int getUnitIncrement(int direction)
			{
				int curY = getValue();
				if (direction > 0)
				{
					return curY > 0 ? outerInstance.cellHeight : outerInstance.cellHeight + HEADER_SEP;
				}
				else
				{
					return curY > outerInstance.cellHeight + HEADER_SEP ? outerInstance.cellHeight : outerInstance.cellHeight + HEADER_SEP;
				}
			}

			public override int getBlockIncrement(int direction)
			{
				int curY = getValue();
				int curHeight = getVisibleAmount();
				int numCells = curHeight / outerInstance.cellHeight - 1;
				if (numCells <= 0)
				{
					numCells = 1;
				}
				if (direction > 0)
				{
					return curY > 0 ? numCells * outerInstance.cellHeight : numCells * outerInstance.cellHeight + HEADER_SEP;
				}
				else
				{
					return curY > outerInstance.cellHeight + HEADER_SEP ? numCells * outerInstance.cellHeight : numCells * outerInstance.cellHeight + HEADER_SEP;
				}
			}
		}

		public virtual void copy()
		{
			requestFocus();
			clip.copy();
		}

		public virtual void paste()
		{
			requestFocus();
			clip.paste();
		}

		public virtual void delete()
		{
			requestFocus();
			int r0 = caret.CursorRow;
			int r1 = caret.MarkRow;
			int c0 = caret.CursorCol;
			int c1 = caret.MarkCol;
			if (r0 < 0 || r1 < 0)
			{
				return;
			}
			if (r1 < r0)
			{
				int t = r0;
				r0 = r1;
				r1 = t;
			}
			if (c1 < c0)
			{
				int t = c0;
				c0 = c1;
				c1 = t;
			}
			int inputs = table.InputColumnCount;
			for (int c = c0; c <= c1; c++)
			{
				if (c >= inputs)
				{
					for (int r = r0; r <= r1; r++)
					{
						table.setOutputEntry(r, c - inputs, Entry.DONT_CARE);
					}
				}
			}
		}

		public virtual void selectAll()
		{
			caret.selectAll();
		}
	}

}
