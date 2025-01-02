// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{


	using Entry = logisim.analyze.model.Entry;
	using TruthTable = logisim.analyze.model.TruthTable;
	using TruthTableEvent = logisim.analyze.model.TruthTableEvent;
	using TruthTableListener = logisim.analyze.model.TruthTableListener;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class TableTabCaret
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			listener = new Listener(this);
		}

		private static Color SELECT_COLOR = new Color(192, 192, 255);

		private Listener listener;
		private TableTab table;
		private int cursorRow;
		private int cursorCol;
		private int markRow;
		private int markCol;

		internal TableTabCaret(TableTab table)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.table = table;
			cursorRow = 0;
			cursorCol = 0;
			markRow = 0;
			markCol = 0;
			table.TruthTable.addTruthTableListener(listener);
			table.addMouseListener(listener);
			table.addMouseMotionListener(listener);
			table.addKeyListener(listener);
			table.addFocusListener(listener);

			InputMap imap = table.getInputMap();
			ActionMap amap = table.getActionMap();
			AbstractAction nullAction = new AbstractActionAnonymousInnerClass(this);
			string nullKey = "null";
			amap.put(nullKey, nullAction);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_DOWN, 0), nullKey);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_UP, 0), nullKey);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_LEFT, 0), nullKey);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_RIGHT, 0), nullKey);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_PAGE_DOWN, 0), nullKey);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_PAGE_UP, 0), nullKey);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_HOME, 0), nullKey);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_END, 0), nullKey);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_ENTER, 0), nullKey);
		}

		private class AbstractActionAnonymousInnerClass : AbstractAction
		{
			private readonly TableTabCaret outerInstance;

			public AbstractActionAnonymousInnerClass(TableTabCaret outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
			}
		}

		internal virtual int CursorRow
		{
			get
			{
				return cursorRow;
			}
		}

		internal virtual int CursorCol
		{
			get
			{
				return cursorCol;
			}
		}

		internal virtual int MarkRow
		{
			get
			{
				return markRow;
			}
		}

		internal virtual int MarkCol
		{
			get
			{
				return markCol;
			}
		}

		internal virtual void selectAll()
		{
			table.requestFocus();
			TruthTable model = table.TruthTable;
			setCursor(model.RowCount, model.InputColumnCount + model.OutputColumnCount, false);
			setCursor(0, 0, true);
		}

		private void setCursor(int row, int col, bool keepMark)
		{
			TruthTable model = table.TruthTable;
			int rows = model.RowCount;
			int cols = model.InputColumnCount + model.OutputColumnCount;
			if (row < 0)
			{
				row = 0;
			}
			if (col < 0)
			{
				col = 0;
			}
			if (row >= rows)
			{
				row = rows - 1;
			}
			if (col >= cols)
			{
				col = cols - 1;
			}

			if (row == cursorRow && col == cursorCol && (keepMark || (row == markRow && col == markCol)))
			{
				; // nothing is changing, so do nothing
			}
			else if (!keepMark && markRow == cursorRow && markCol == cursorCol)
			{
				int oldRow = cursorRow;
				int oldCol = cursorCol;
				cursorRow = row;
				cursorCol = col;
				markRow = row;
				markCol = col;
				expose(oldRow, oldCol);
				expose(cursorRow, cursorCol);
			}
			else
			{
				int r0 = Math.Min(row, Math.Min(cursorRow, markRow));
				int r1 = Math.Max(row, Math.Max(cursorRow, markRow));
				int c0 = Math.Min(col, Math.Min(cursorCol, markCol));
				int c1 = Math.Max(col, Math.Max(cursorCol, markCol));
				cursorRow = row;
				cursorCol = col;
				if (!keepMark)
				{
					markRow = row;
					markCol = col;
				}

				int x0 = table.getX(c0);
				int x1 = table.getX(c1) + table.CellWidth;
				int y0 = table.getY(r0);
				int y1 = table.getY(r1) + table.CellHeight;
				table.repaint(x0 - 2, y0 - 2, (x1 - x0) + 4, (y1 - y0) + 4);
			}
			int cx = table.getX(cursorCol);
			int cy = table.getY(cursorRow);
			int cw = table.CellWidth;
			int ch = table.CellHeight;
			if (cursorRow == 0)
			{
				ch += cy;
				cy = 0;
			}
			table.scrollRectToVisible(new Rectangle(cx, cy, cw, ch));
		}

		private void expose(int row, int col)
		{
			if (row >= 0)
			{
				int x0 = table.getX(0);
				int x1 = table.getX(table.ColumnCount - 1) + table.CellWidth;
				table.repaint(x0 - 2, table.getY(row) - 2, (x1 - x0) + 4, table.CellHeight + 4);
			}
		}

		internal virtual void paintBackground(Graphics g)
		{
			if (cursorRow >= 0 && cursorCol >= 0 && (cursorRow != markRow || cursorCol != markCol))
			{
				g.setColor(SELECT_COLOR);

				int r0 = cursorRow;
				int c0 = cursorCol;
				int r1 = markRow;
				int c1 = markCol;
				if (r1 < r0)
				{
					int t = r1;
					r1 = r0;
					r0 = t;
				}
				if (c1 < c0)
				{
					int t = c1;
					c1 = c0;
					c0 = t;
				}
				int x0 = table.getX(c0);
				int y0 = table.getY(r0);
				int x1 = table.getX(c1) + table.CellWidth;
				int y1 = table.getY(r1) + table.CellHeight;
				g.fillRect(x0, y0, x1 - x0, y1 - y0);
			}
		}

		internal virtual void paintForeground(Graphics g)
		{
			if (!table.isFocusOwner())
			{
				return;
			}
			if (cursorRow >= 0 && cursorCol >= 0)
			{
				int x = table.getX(cursorCol);
				int y = table.getY(cursorRow);
				GraphicsUtil.switchToWidth(g, 2);
				g.drawRect(x, y, table.CellWidth, table.CellHeight);
				GraphicsUtil.switchToWidth(g, 2);
			}
		}

		private class Listener : MouseListener, MouseMotionListener, KeyListener, FocusListener, TruthTableListener
		{
			private readonly TableTabCaret outerInstance;

			public Listener(TableTabCaret outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void mouseClicked(MouseEvent e)
			{
			}

			public virtual void mousePressed(MouseEvent e)
			{
				outerInstance.table.requestFocus();
				int row = outerInstance.table.getRow(e);
				int col = outerInstance.table.getColumn(e);
				outerInstance.setCursor(row, col, (e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) != 0);
			}

			public virtual void mouseReleased(MouseEvent e)
			{
				mouseDragged(e);
			}

			public virtual void mouseEntered(MouseEvent e)
			{
			}

			public virtual void mouseExited(MouseEvent e)
			{
			}

			public virtual void mouseDragged(MouseEvent e)
			{
				int row = outerInstance.table.getRow(e);
				int col = outerInstance.table.getColumn(e);
				outerInstance.setCursor(row, col, true);
			}

			public virtual void mouseMoved(MouseEvent e)
			{
			}

			public virtual void keyTyped(KeyEvent e)
			{
				int mask = e.getModifiersEx();
				if ((mask & ~InputEvent.SHIFT_DOWN_MASK) != 0)
				{
					return;
				}

				char c = e.getKeyChar();
				Entry newEntry = null;
				switch (c)
				{
				case ' ':
					if (outerInstance.cursorRow >= 0)
					{
						TruthTable model = outerInstance.table.TruthTable;
						int inputs = model.InputColumnCount;
						if (outerInstance.cursorCol >= inputs)
						{
							Entry cur = model.getOutputEntry(outerInstance.cursorRow, outerInstance.cursorCol - inputs);
							if (cur == Entry.ZERO)
							{
								cur = Entry.ONE;
							}
							else if (cur == Entry.ONE)
							{
								cur = Entry.DONT_CARE;
							}
							else
							{
								cur = Entry.ZERO;
							}
							model.setOutputEntry(outerInstance.cursorRow, outerInstance.cursorCol - inputs, cur);
						}
					}
					break;
				case '0':
					newEntry = Entry.ZERO;
					break;
				case '1':
					newEntry = Entry.ONE;
					break;
				case 'x':
					newEntry = Entry.DONT_CARE;
					break;
				case '\n':
					outerInstance.setCursor(outerInstance.cursorRow + 1, outerInstance.table.TruthTable.InputColumnCount, (mask & InputEvent.SHIFT_DOWN_MASK) != 0);
					break;
				case '\u0008':
				case '\u007f':
					outerInstance.setCursor(outerInstance.cursorRow, outerInstance.cursorCol - 1, (mask & InputEvent.SHIFT_DOWN_MASK) != 0);
					break;
				default:
				}
				if (newEntry != null)
				{
					TruthTable model = outerInstance.table.TruthTable;
					int inputs = model.InputColumnCount;
					int outputs = model.OutputColumnCount;
					if (outerInstance.cursorCol >= inputs)
					{
						model.setOutputEntry(outerInstance.cursorRow, outerInstance.cursorCol - inputs, newEntry);
						if (outerInstance.cursorCol >= inputs + outputs - 1)
						{
							outerInstance.setCursor(outerInstance.cursorRow + 1, inputs, false);
						}
						else
						{
							outerInstance.setCursor(outerInstance.cursorRow, outerInstance.cursorCol + 1, false);
						}
					}
				}
			}

			public virtual void keyPressed(KeyEvent e)
			{
				if (outerInstance.cursorRow < 0)
				{
					return;
				}
				TruthTable model = outerInstance.table.TruthTable;
				int rows = model.RowCount;
				int inputs = model.InputColumnCount;
				int outputs = model.OutputColumnCount;
				int cols = inputs + outputs;
				bool shift = (e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) != 0;
				switch (e.getKeyCode())
				{
				case KeyEvent.VK_UP:
					outerInstance.setCursor(outerInstance.cursorRow - 1, outerInstance.cursorCol, shift);
					break;
				case KeyEvent.VK_LEFT:
					outerInstance.setCursor(outerInstance.cursorRow, outerInstance.cursorCol - 1, shift);
					break;
				case KeyEvent.VK_DOWN:
					outerInstance.setCursor(outerInstance.cursorRow + 1, outerInstance.cursorCol, shift);
					break;
				case KeyEvent.VK_RIGHT:
					outerInstance.setCursor(outerInstance.cursorRow, outerInstance.cursorCol + 1, shift);
					break;
				case KeyEvent.VK_HOME:
					if (outerInstance.cursorCol == 0)
					{
						outerInstance.setCursor(0, 0, shift);
					}
					else
					{
						outerInstance.setCursor(outerInstance.cursorRow, 0, shift);
					}
					break;
				case KeyEvent.VK_END:
					if (outerInstance.cursorCol == cols - 1)
					{
						outerInstance.setCursor(rows - 1, cols - 1, shift);
					}
					else
					{
						outerInstance.setCursor(outerInstance.cursorRow, cols - 1, shift);
					}
					break;
				case KeyEvent.VK_PAGE_DOWN:
					rows = outerInstance.table.getVisibleRect().height / outerInstance.table.CellHeight;
					if (rows > 2)
					{
						rows--;
					}
					outerInstance.setCursor(outerInstance.cursorRow + rows, outerInstance.cursorCol, shift);
					break;
				case KeyEvent.VK_PAGE_UP:
					rows = outerInstance.table.getVisibleRect().height / outerInstance.table.CellHeight;
					if (rows > 2)
					{
						rows--;
					}
					outerInstance.setCursor(outerInstance.cursorRow - rows, outerInstance.cursorCol, shift);
					break;
				}
			}

			public virtual void keyReleased(KeyEvent e)
			{
			}

			public virtual void focusGained(FocusEvent e)
			{
				if (outerInstance.cursorRow >= 0)
				{
					outerInstance.expose(outerInstance.cursorRow, outerInstance.cursorCol);
				}
			}

			public virtual void focusLost(FocusEvent e)
			{
				if (outerInstance.cursorRow >= 0)
				{
					outerInstance.expose(outerInstance.cursorRow, outerInstance.cursorCol);
				}
			}

			public virtual void cellsChanged(TruthTableEvent @event)
			{
			}

			public virtual void structureChanged(TruthTableEvent @event)
			{
				TruthTable model = @event.Source;
				int inputs = model.InputColumnCount;
				int outputs = model.OutputColumnCount;
				int rows = model.RowCount;
				int cols = inputs + outputs;
				bool changed = false;
				if (outerInstance.cursorRow >= rows)
				{
					outerInstance.cursorRow = rows - 1;
					changed = true;
				}
				if (outerInstance.cursorCol >= cols)
				{
					outerInstance.cursorCol = cols - 1;
					changed = true;
				}
				if (outerInstance.markRow >= rows)
				{
					outerInstance.markRow = rows - 1;
					changed = true;
				}
				if (outerInstance.markCol >= cols)
				{
					outerInstance.markCol = cols - 1;
					changed = true;
				}
				if (changed)
				{
					outerInstance.table.repaint();
				}
			}
		}
	}

}
