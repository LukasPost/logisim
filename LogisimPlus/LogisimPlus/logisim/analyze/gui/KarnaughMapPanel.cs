// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{

	using AnalyzerModel = logisim.analyze.model.AnalyzerModel;
	using OutputExpressionsEvent = logisim.analyze.model.OutputExpressionsEvent;
	using OutputExpressionsListener = logisim.analyze.model.OutputExpressionsListener;
	using Entry = logisim.analyze.model.Entry;
	using Implicant = logisim.analyze.model.Implicant;
	using TruthTable = logisim.analyze.model.TruthTable;
	using TruthTableEvent = logisim.analyze.model.TruthTableEvent;
	using TruthTableListener = logisim.analyze.model.TruthTableListener;
	using VariableList = logisim.analyze.model.VariableList;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	internal class KarnaughMapPanel : JPanel, TruthTablePanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private static readonly Font HEAD_FONT = new Font("Serif", Font.BOLD, 14);
		private static readonly Font BODY_FONT = new Font("Serif", Font.PLAIN, 14);
		private static readonly Color[] IMP_COLORS = new Color[]
		{
			Color.FromArgb(255, 255, 0, 0, 128),
			Color.FromArgb(255, 0, 150, 0, 128),
			Color.FromArgb(255, 0, 0, 255, 128),
			Color.FromArgb(255, 255, 0, 255, 128)
		};

		private const int MAX_VARS = 4;

		private static readonly int[] ROW_VARS = new int[] {0, 0, 1, 1, 2};
		private static readonly int[] COL_VARS = new int[] {0, 1, 1, 2, 2};
		private const int CELL_HORZ_SEP = 10;
		private const int CELL_VERT_SEP = 10;
		private const int IMP_INSET = 4;
		private const int IMP_RADIUS = 5;

		private class MyListener : OutputExpressionsListener, TruthTableListener
		{
			private readonly KarnaughMapPanel outerInstance;

			public MyListener(KarnaughMapPanel outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void expressionChanged(OutputExpressionsEvent @event)
			{
				if (@event.Type == OutputExpressionsEvent.OUTPUT_MINIMAL && @event.Variable.Equals(outerInstance.output))
				{
					repaint();
				}
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
		private AnalyzerModel model;
		private string output;
		private int headHeight;
		private int cellWidth = 1;
		private int cellHeight = 1;
		private int tableWidth;
		private int tableHeight;
		private int provisionalX;
		private int provisionalY;
		private Entry provisionalValue = null;

		public KarnaughMapPanel(AnalyzerModel model)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.model = model;
			model.OutputExpressions.addOutputExpressionsListener(myListener);
			model.TruthTable.addTruthTableListener(myListener);
			setToolTipText(" ");
		}

		public virtual string Output
		{
			set
			{
				bool recompute = (string.ReferenceEquals(output, null) || string.ReferenceEquals(value, null)) && !string.ReferenceEquals(output, value);
				output = value;
				if (recompute)
				{
					computePreferredSize();
				}
				else
				{
					repaint();
				}
			}
		}

		public virtual TruthTable TruthTable
		{
			get
			{
				return model.TruthTable;
			}
		}

		public virtual int getRow(MouseEvent @event)
		{
			TruthTable table = model.TruthTable;
			int inputs = table.InputColumnCount;
			if (inputs >= ROW_VARS.Length)
			{
				return -1;
			}
			int left = computeMargin(getWidth(), tableWidth);
			int top = computeMargin(getHeight(), tableHeight);
			int x = @event.getX() - left - headHeight - cellWidth;
			int y = @event.getY() - top - headHeight - cellHeight;
			if (x < 0 || y < 0)
			{
				return -1;
			}
			int row = y / cellHeight;
			int col = x / cellWidth;
			int rows = 1 << ROW_VARS[inputs];
			int cols = 1 << COL_VARS[inputs];
			if (row >= rows || col >= cols)
			{
				return -1;
			}
			return getTableRow(row, col, rows, cols);
		}

		public virtual int getOutputColumn(MouseEvent @event)
		{
			return model.Outputs.indexOf(output);
		}

		public virtual void setEntryProvisional(int y, int x, Entry value)
		{
			provisionalY = y;
			provisionalX = x;
			provisionalValue = value;
			repaint();
		}

		public override string getToolTipText(MouseEvent @event)
		{
			TruthTable table = model.TruthTable;
			int row = getRow(@event);
			int col = getOutputColumn(@event);
			Entry entry = table.getOutputEntry(row, col);
			return entry.ErrorMessage;
		}

		internal virtual void localeChanged()
		{
			computePreferredSize();
			repaint();
		}

		private void computePreferredSize()
		{
			JGraphics g = getJGraphics();
			TruthTable table = model.TruthTable;

			string message = null;
			if (string.ReferenceEquals(output, null))
			{
				message = Strings.get("karnaughNoOutputError");
			}
			else if (table.InputColumnCount > MAX_VARS)
			{
				message = Strings.get("karnaughTooManyInputsError");
			}
			if (!string.ReferenceEquals(message, null))
			{
				if (g == null)
				{
					tableHeight = 15;
					tableWidth = 100;
				}
				else
				{
					FontMetrics fm = g.getFontMetrics(BODY_FONT);
					tableHeight = fm.getHeight();
					tableWidth = fm.stringWidth(message);
				}
				setPreferredSize(new Size(tableWidth, tableHeight));
				repaint();
				return;
			}

			if (g == null)
			{
				headHeight = 16;
				cellHeight = 16;
				cellWidth = 24;
			}
			else
			{
				FontMetrics headFm = g.getFontMetrics(HEAD_FONT);
				headHeight = headFm.getHeight();

				FontMetrics fm = g.getFontMetrics(BODY_FONT);
				cellHeight = fm.getAscent() + CELL_VERT_SEP;
				cellWidth = fm.stringWidth("00") + CELL_HORZ_SEP;
			}

			int rows = 1 << ROW_VARS[table.InputColumnCount];
			int cols = 1 << COL_VARS[table.InputColumnCount];
			tableWidth = headHeight + cellWidth * (cols + 1);
			tableHeight = headHeight + cellHeight * (rows + 1);
			setPreferredSize(new Size(tableWidth, tableHeight));
			invalidate();
			repaint();
		}

		public override void paintComponent(JGraphics g)
		{
			base.paintComponent(g);

			TruthTable table = model.TruthTable;
			int inputCount = table.InputColumnCount;
			Size sz = getSize();
			string message = null;
			if (string.ReferenceEquals(output, null))
			{
				message = Strings.get("karnaughNoOutputError");
			}
			else if (inputCount > MAX_VARS)
			{
				message = Strings.get("karnaughTooManyInputsError");
			}
			if (!string.ReferenceEquals(message, null))
			{
				g.setFont(BODY_FONT);
				JGraphicsUtil.drawCenteredText(g, message, sz.width / 2, sz.height / 2);
				return;
			}

			int left = computeMargin(sz.width, tableWidth);
			int top = computeMargin(sz.height, tableHeight);
			int x = left;
			int y = top;
			int rowVars = ROW_VARS[inputCount];
			int colVars = COL_VARS[inputCount];
			int rows = 1 << rowVars;
			int cols = 1 << colVars;

			g.setFont(HEAD_FONT);
			FontMetrics headFm = g.getFontMetrics();
			string rowHeader = header(0, rowVars);
			string colHeader = header(rowVars, rowVars + colVars);
			int xoffs = (tableWidth + headHeight + cellWidth - headFm.stringWidth(colHeader)) / 2;
			g.drawString(colHeader, x + xoffs, y + headFm.getAscent());
			int headerWidth = headFm.stringWidth(rowHeader);
			if (headerWidth <= headHeight)
			{
				int headX = x + (headHeight - headerWidth) / 2;
				int headY = y + (tableHeight + headHeight + cellHeight + headFm.getAscent()) / 2;
				g.drawString(rowHeader, headX, headY);
			}
			else if (g is JGraphics2D)
			{
				JGraphics2D g2 = (JGraphics2D) g.create();
				int yoffs = (tableHeight + headHeight + cellHeight + headerWidth) / 2;
				int headX = x + headFm.getAscent();
				int headY = y + yoffs;
				g2.rotate(-Math.PI / 2.0);
				g2.drawString(rowHeader, -headY, headX);
				g2.dispose();
			}

			x += headHeight;
			y += headHeight;
			g.setFont(BODY_FONT);
			FontMetrics fm = g.getFontMetrics();
			int dy = (cellHeight + fm.getAscent()) / 2;
			for (int i = 0; i < cols; i++)
			{
				string label = this.label(i, cols);
				g.drawString(label, x + (i + 1) * cellWidth + (cellWidth - fm.stringWidth(label)) / 2, y + dy);
			}
			for (int i = 0; i < rows; i++)
			{
				string label = this.label(i, rows);
				g.drawString(label, x + (cellWidth - fm.stringWidth(label)) / 2, y + (i + 1) * cellHeight + dy);
			}

			int outputColumn = table.getOutputIndex(output);
			x += cellWidth;
			y += cellHeight;
			g.setColor(ERROR_COLOR);
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					int row = getTableRow(i, j, rows, cols);
					Entry entry = table.getOutputEntry(row, outputColumn);
					if (provisionalValue != null && row == provisionalY && outputColumn == provisionalX)
					{
						entry = provisionalValue;
					}
					if (entry.Error)
					{
						g.fillRect(x + j * cellWidth, y + i * cellHeight, cellWidth, cellHeight);
					}
				}
			}

			List<Implicant> implicants = model.OutputExpressions.getMinimalImplicants(output);
			if (implicants != null)
			{
				int index = 0;
				foreach (Implicant imp in implicants)
				{
					g.setColor(IMP_COLORS[index % IMP_COLORS.Length]);
					paintImplicant(g, imp, x, y, rows, cols);
					index++;
				}
			}

			g.setColor(Color.Gray);
			if (cols > 1 || inputCount == 0)
			{
				g.drawLine(x, y, left + tableWidth, y);
			}
			if (rows > 1 || inputCount == 0)
			{
				g.drawLine(x, y, x, top + tableHeight);
			}
			if (outputColumn < 0)
			{
				return;
			}

			g.setColor(Color.Black);
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					int row = getTableRow(i, j, rows, cols);
					if (provisionalValue != null && row == provisionalY && outputColumn == provisionalX)
					{
						string text = provisionalValue.Description;
						g.setColor(Color.GREEN);
						g.drawString(text, x + j * cellWidth + (cellWidth - fm.stringWidth(text)) / 2, y + i * cellHeight + dy);
						g.setColor(Color.Black);
					}
					else
					{
						Entry entry = table.getOutputEntry(row, outputColumn);
						string text = entry.Description;
						g.drawString(text, x + j * cellWidth + (cellWidth - fm.stringWidth(text)) / 2, y + i * cellHeight + dy);
					}
				}
			}
		}

		private void paintImplicant(JGraphics g, Implicant imp, int x, int y, int rows, int cols)
		{
			int rowMax = -1;
			int rowMin = rows;
			int colMax = -1;
			int colMin = cols;
			bool oneRowFound = false;
			int count = 0;
			foreach (Implicant sq in imp.Terms)
			{
				int tableRow = sq.Row;
				int row = getRow(tableRow, rows, cols);
				int col = getCol(tableRow, rows, cols);
				if (row == 1)
				{
					oneRowFound = true;
				}
				if (row > rowMax)
				{
					rowMax = row;
				}
				if (row < rowMin)
				{
					rowMin = row;
				}
				if (col > colMax)
				{
					colMax = col;
				}
				if (col < colMin)
				{
					colMin = col;
				}
				++count;
			}

			int numCols = colMax - colMin + 1;
			int numRows = rowMax - rowMin + 1;
			int covered = numCols * numRows;
			int d = 2 * IMP_RADIUS;
			if (covered == count)
			{
				g.fillRoundRect(x + colMin * cellWidth + IMP_INSET, y + rowMin * cellHeight + IMP_INSET, numCols * cellWidth - 2 * IMP_INSET, numRows * cellHeight - 2 * IMP_INSET, d, d);
			}
			else if (covered == 16)
			{
				if (count == 4)
				{
					int w = cellWidth - IMP_INSET;
					int h = cellHeight - IMP_INSET;
					int x1 = x + 3 * cellWidth + IMP_INSET;
					int y1 = y + 3 * cellHeight + IMP_INSET;
					g.fillRoundRect(x, y, w, h, d, d);
					g.fillRoundRect(x1, y, w, h, d, d);
					g.fillRoundRect(x, y1, w, h, d, d);
					g.fillRoundRect(x1, y1, w, h, d, d);
				}
				else if (oneRowFound)
				{ // first and last columns
					int w = cellWidth - IMP_INSET;
					int h = 4 * cellHeight - 2 * IMP_INSET;
					int x1 = x + 3 * cellWidth + IMP_INSET;
					g.fillRoundRect(x, y + IMP_INSET, w, h, d, d);
					g.fillRoundRect(x1, y + IMP_INSET, w, h, d, d);
				}
				else
				{ // first and last rows
					int w = 4 * cellWidth - 2 * IMP_INSET;
					int h = cellHeight - IMP_INSET;
					int y1 = y + 3 * cellHeight + IMP_INSET;
					g.fillRoundRect(x + IMP_INSET, y, w, h, d, d);
					g.fillRoundRect(x + IMP_INSET, y1, w, h, d, d);
				}
			}
			else if (numCols == 4)
			{
				int top = y + rowMin * cellHeight + IMP_INSET;
				int w = cellWidth - IMP_INSET;
				int h = numRows * cellHeight - 2 * IMP_INSET;
				// handle half going off left edge
				g.fillRoundRect(x, top, w, h, d, d);
				// handle half going off right edge
				g.fillRoundRect(x + 3 * cellWidth + IMP_INSET, top, w, h, d, d);
				/*
				 * This is the proper way, with no rounded rectangles along the table's edge; but I found that the different
				 * regions were liable to overlap, particularly the arcs with the rectangles. (Plus, I was too lazy to
				 * figure this out for the 16 case.) int y0 = y + rowMin * cellHeight + IMP_INSET; int y1 = y + rowMax *
				 * cellHeight + cellHeight - IMP_INSET; int dy = y1 - y0; int x0 = x + cellWidth - IMP_INSET; int x1 = x + 3
				 * * cellWidth + IMP_INSET;
				 * 
				 * // half going off left edge g.fillRect(x, y0, cellWidth - IMP_INSET - IMP_RADIUS, dy); g.fillRect(x0 -
				 * IMP_RADIUS, y0 + IMP_RADIUS, IMP_RADIUS, dy - d); g.fillArc(x0 - d, y0, d, d, 0, 90); g.fillArc(x0 - d,
				 * y1 - d, d, d, 0, -90);
				 * 
				 * // half going off right edge g.fillRect(x1 + IMP_RADIUS, y0, cellWidth - IMP_INSET - IMP_RADIUS, dy);
				 * g.fillRect(x1, y0 + IMP_RADIUS, IMP_RADIUS, dy - d); g.fillArc(x1, y0, d, d, 180, 90); g.fillArc(x1, y1 -
				 * d, d, d, 180, -90);
				 */
			}
			else
			{ // numRows == 4
				int left = x + colMin * cellWidth + IMP_INSET;
				int w = numCols * cellWidth - 2 * IMP_INSET;
				int h = cellHeight - IMP_INSET;
				// handle half going off top edge
				g.fillRoundRect(left, y, w, h, d, d);
				// handle half going off right edge
				g.fillRoundRect(left, y + 3 * cellHeight + IMP_INSET, w, h, d, d);
			}
		}

		private string header(int start, int stop)
		{
			if (start >= stop)
			{
				return "";
			}
			VariableList inputs = model.Inputs;
			StringBuilder ret = new StringBuilder(inputs.get(start));
			for (int i = start + 1; i < stop; i++)
			{
				ret.Append(", ");
				ret.Append(inputs.get(i));
			}
			return ret.ToString();
		}

		private string label(int row, int rows)
		{
			switch (rows)
			{
			case 2:
				return "" + row;
			case 4:
				switch (row)
				{
				case 0:
					return "00";
				case 1:
					return "01";
				case 2:
					return "11";
				case 3:
					return "10";
				}
			default:
				return "";
			}
		}

		private int getTableRow(int row, int col, int rows, int cols)
		{
			return toRow(row, rows) * cols + toRow(col, cols);
		}

		private int toRow(int row, int rows)
		{
			if (rows == 4)
			{
				switch (row)
				{
				case 2:
					return 3;
				case 3:
					return 2;
				default:
					return row;
				}
			}
			else
			{
				return row;
			}
		}

		private int getRow(int tableRow, int rows, int cols)
		{
			int ret = tableRow / cols;
			switch (ret)
			{
			case 2:
				return 3;
			case 3:
				return 2;
			default:
				return ret;
			}
		}

		private int getCol(int tableRow, int rows, int cols)
		{
			int ret = tableRow % cols;
			switch (ret)
			{
			case 2:
				return 3;
			case 3:
				return 2;
			default:
				return ret;
			}
		}

		private int computeMargin(int compDim, int tableDim)
		{
			int ret = (compDim - tableDim) / 2;
			return ret >= 0 ? ret : Math.Max(-headHeight, compDim - tableDim);
		}

	}

}
