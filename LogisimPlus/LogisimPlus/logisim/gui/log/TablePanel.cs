// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{


	using Value = logisim.data.Value;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class TablePanel : LogPanel
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

		private class MyListener : ModelListener
		{
			private readonly TablePanel outerInstance;

			public MyListener(TablePanel outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void selectionChanged(ModelEvent @event)
			{
				computeRowCount();
			}

			public virtual void entryAdded(ModelEvent @event, Value[] values)
			{
				int oldCount = outerInstance.rowCount;
				computeRowCount();
				if (oldCount == outerInstance.rowCount)
				{
					int value = outerInstance.vsb.getValue();
					if (value > outerInstance.vsb.getMinimum() && value < outerInstance.vsb.getMaximum() - outerInstance.vsb.getVisibleAmount())
					{
						outerInstance.vsb.setValue(outerInstance.vsb.getValue() - outerInstance.vsb.getUnitIncrement(-1));
					}
					else
					{
						repaint();
					}
				}
			}

			public virtual void filePropertyChanged(ModelEvent @event)
			{
			}

			internal virtual void computeRowCount()
			{
				Model model = outerInstance.Model;
				Selection sel = model.Selection;
				int rows = 0;
				for (int i = sel.size() - 1; i >= 0; i--)
				{
					int x = model.getValueLog(sel.get(i)).size();
					if (x > rows)
					{
						rows = x;
					}
				}
				if (outerInstance.rowCount != rows)
				{
					outerInstance.rowCount = rows;
					outerInstance.computePreferredSize();
				}
			}
		}

		private class VerticalScrollBar : JScrollBar, ChangeListener
		{
			private readonly TablePanel outerInstance;

			internal int oldMaximum = -1;
			internal int oldExtent = -1;

			public VerticalScrollBar(TablePanel outerInstance)
			{
				this.outerInstance = outerInstance;
				outerInstance.Model.addChangeListener(this);
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

			public virtual void stateChanged(ChangeEvent @event)
			{
				int newMaximum = getMaximum();
				int newExtent = getVisibleAmount();
				if (oldMaximum != newMaximum || oldExtent != newExtent)
				{
					if (getValue() + oldExtent >= oldMaximum)
					{
						setValue(newMaximum - newExtent);
					}
					oldMaximum = newMaximum;
					oldExtent = newExtent;
				}
			}
		}

		private MyListener myListener;
		private int cellWidth = 25; // reasonable start values
		private int cellHeight = 15;
		private int rowCount = 0;
		private int tableWidth;
		private int tableHeight;
		private VerticalScrollBar vsb;

		public TablePanel(LogFrame frame) : base(frame)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			vsb = new VerticalScrollBar(this);
			modelChanged(null, Model);
		}

		public override string Title
		{
			get
			{
				return Strings.get("tableTab");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("tableHelp");
			}
		}

		public override void localeChanged()
		{
			computePreferredSize();
			repaint();
		}

		public override void modelChanged(Model oldModel, Model newModel)
		{
			if (oldModel != null)
			{
				oldModel.removeModelListener(myListener);
			}
			if (newModel != null)
			{
				newModel.addModelListener(myListener);
			}
		}

		public virtual int getColumn(MouseEvent @event)
		{
			int x = @event.getX() - (getWidth() - tableWidth) / 2;
			if (x < 0)
			{
				return -1;
			}
			Selection sel = Model.Selection;
			int ret = (x + COLUMN_SEP / 2) / (cellWidth + COLUMN_SEP);
			return ret >= 0 && ret < sel.size() ? ret : -1;
		}

		public virtual int getRow(MouseEvent @event)
		{
			int y = @event.getY() - (getHeight() - tableHeight) / 2;
			if (y < cellHeight + HEADER_SEP)
			{
				return -1;
			}
			int ret = (y - cellHeight - HEADER_SEP) / cellHeight;
			return ret >= 0 && ret < rowCount ? ret : -1;
		}

		public override void paintComponent(Graphics g)
		{
			base.paintComponent(g);

			Dimension sz = getSize();
			int top = Math.Max(0, (sz.height - tableHeight) / 2);
			int left = Math.Max(0, (sz.width - tableWidth) / 2);
			Model model = Model;
			if (model == null)
			{
				return;
			}
			Selection sel = model.Selection;
			int columns = sel.size();
			if (columns == 0)
			{
				g.setFont(BODY_FONT);
				GraphicsUtil.drawCenteredText(g, Strings.get("tableEmptyMessage"), sz.width / 2, sz.height / 2);
				return;
			}

			g.setColor(Color.GRAY);
			int lineY = top + cellHeight + HEADER_SEP / 2;
			g.drawLine(left, lineY, left + tableWidth, lineY);

			g.setColor(Color.BLACK);
			g.setFont(HEAD_FONT);
			FontMetrics headerMetric = g.getFontMetrics();
			int x = left;
			int y = top + headerMetric.getAscent() + 1;
			for (int i = 0; i < columns; i++)
			{
				x = paintHeader(sel.get(i).toShortString(), x, y, g, headerMetric);
			}

			g.setFont(BODY_FONT);
			FontMetrics bodyMetric = g.getFontMetrics();
			Rectangle clip = g.getClipBounds();
			int firstRow = Math.Max(0, (clip.y - y) / cellHeight - 1);
			int lastRow = Math.Min(rowCount, 2 + (clip.y + clip.height - y) / cellHeight);
			int y0 = top + cellHeight + HEADER_SEP;
			x = left;
			for (int col = 0; col < columns; col++)
			{
				SelectionItem item = sel.get(col);
				ValueLog log = model.getValueLog(item);
				int radix = item.Radix;
				int offs = rowCount - log.size();
				y = y0 + Math.Max(offs, firstRow) * cellHeight;
				for (int row = Math.Max(offs, firstRow); row < lastRow; row++)
				{
					Value val = log.get(row - offs);
					string label = val.toDisplayString(radix);
					int width = bodyMetric.stringWidth(label);
					g.drawString(label, x + (cellWidth - width) / 2, y + bodyMetric.getAscent());
					y += cellHeight;
				}
				x += cellWidth + COLUMN_SEP;
			}
		}

		private int paintHeader(string header, int x, int y, Graphics g, FontMetrics fm)
		{
			int width = fm.stringWidth(header);
			g.drawString(header, x + (cellWidth - width) / 2, y);
			return x + cellWidth + COLUMN_SEP;
		}

		private void computePreferredSize()
		{
			Model model = Model;
			Selection sel = model.Selection;
			int columns = sel.size();
			if (columns == 0)
			{
				setPreferredSize(new Dimension(0, 0));
				return;
			}

			Graphics g = getGraphics();
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
				for (int i = 0; i < columns; i++)
				{
					string header = sel.get(i).toShortString();
					cellWidth = Math.Max(cellWidth, fm.stringWidth(header));
				}
			}

			tableWidth = (cellWidth + COLUMN_SEP) * columns - COLUMN_SEP;
			tableHeight = cellHeight * (1 + rowCount) + HEADER_SEP;
			setPreferredSize(new Dimension(tableWidth, tableHeight));
			revalidate();
			repaint();
		}

		internal virtual JScrollBar VerticalScrollBar
		{
			get
			{
				return vsb;
			}
		}
	}

}
