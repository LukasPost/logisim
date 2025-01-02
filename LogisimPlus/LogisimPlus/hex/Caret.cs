// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace hex
{


	public class Caret
	{
		private static Color SELECT_COLOR = new Color(192, 192, 255);

		private class Listener : MouseListener, MouseMotionListener, KeyListener, FocusListener
		{
			private readonly Caret outerInstance;

			public Listener(Caret outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void mouseClicked(MouseEvent e)
			{
			}

			public virtual void mousePressed(MouseEvent e)
			{
				Measures measures = outerInstance.hex.Measures;
				long loc = measures.toAddress(e.getX(), e.getY());
				outerInstance.setDot(loc, (e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) != 0);
				if (!outerInstance.hex.isFocusOwner())
				{
					outerInstance.hex.requestFocus();
				}
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
				Measures measures = outerInstance.hex.Measures;
				long loc = measures.toAddress(e.getX(), e.getY());
				outerInstance.setDot(loc, true);

				// TODO should repeat dragged events when mouse leaves the
				// component
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
				int cols = outerInstance.hex.Measures.ColumnCount;
				switch (c)
				{
				case ' ':
					if (outerInstance.cursor >= 0)
					{
						outerInstance.setDot(outerInstance.cursor + 1, (mask & InputEvent.SHIFT_DOWN_MASK) != 0);
					}
					break;
				case '\n':
					if (outerInstance.cursor >= 0)
					{
						outerInstance.setDot(outerInstance.cursor + cols, (mask & InputEvent.SHIFT_DOWN_MASK) != 0);
					}
					break;
				case '\u0008':
				case '\u007f':
					outerInstance.hex.delete();
					// setDot(cursor - 1, (mask & InputEvent.SHIFT_MASK) != 0);
					break;
				default:
					int digit = Character.digit(e.getKeyChar(), 16);
					if (digit >= 0)
					{
						HexModel model = outerInstance.hex.Model;
						if (model != null && outerInstance.cursor >= model.FirstOffset && outerInstance.cursor <= model.LastOffset)
						{
							int curValue = model.get(outerInstance.cursor);
							int newValue = 16 * curValue + digit;
							model.set(outerInstance.cursor, newValue);
						}
					}
				break;
				}
			}

			public virtual void keyPressed(KeyEvent e)
			{
				int cols = outerInstance.hex.Measures.ColumnCount;
				int rows;
				bool shift = (e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) != 0;
				switch (e.getKeyCode())
				{
				case KeyEvent.VK_UP:
					if (outerInstance.cursor >= cols)
					{
						outerInstance.setDot(outerInstance.cursor - cols, shift);
					}
					break;
				case KeyEvent.VK_LEFT:
					if (outerInstance.cursor >= 1)
					{
						outerInstance.setDot(outerInstance.cursor - 1, shift);
					}
					break;
				case KeyEvent.VK_DOWN:
					if (outerInstance.cursor >= outerInstance.hex.Model.FirstOffset && outerInstance.cursor <= outerInstance.hex.Model.LastOffset - cols)
					{
						outerInstance.setDot(outerInstance.cursor + cols, shift);
					}
					break;
				case KeyEvent.VK_RIGHT:
					if (outerInstance.cursor >= outerInstance.hex.Model.FirstOffset && outerInstance.cursor <= outerInstance.hex.Model.LastOffset - 1)
					{
						outerInstance.setDot(outerInstance.cursor + 1, shift);
					}
					break;
				case KeyEvent.VK_HOME:
					if (outerInstance.cursor >= 0)
					{
						int dist = (int)(outerInstance.cursor % cols);
						if (dist == 0)
						{
							outerInstance.setDot(0, shift);
						}
						else
						{
							outerInstance.setDot(outerInstance.cursor - dist, shift);
						}
						break;
					}
				case KeyEvent.VK_END:
					if (outerInstance.cursor >= 0)
					{
						HexModel model = outerInstance.hex.Model;
						long dest = (outerInstance.cursor / cols * cols) + cols - 1;
						if (model != null)
						{
							long end = model.LastOffset;
							if (dest > end || dest == outerInstance.cursor)
							{
								dest = end;
							}
							outerInstance.setDot(dest, shift);
						}
						else
						{
							outerInstance.setDot(dest, shift);
						}
					}
					break;
				case KeyEvent.VK_PAGE_DOWN:
					rows = outerInstance.hex.getVisibleRect().height / outerInstance.hex.Measures.CellHeight;
					if (rows > 2)
					{
						rows--;
					}
					if (outerInstance.cursor >= 0)
					{
						long max = outerInstance.hex.Model.LastOffset;
						if (outerInstance.cursor + rows * cols <= max)
						{
							outerInstance.setDot(outerInstance.cursor + rows * cols, shift);
						}
						else
						{
							long n = outerInstance.cursor;
							while (n + cols < max)
							{
								n += cols;
							}
							outerInstance.setDot(n, shift);
						}
					}
					break;
				case KeyEvent.VK_PAGE_UP:
					rows = outerInstance.hex.getVisibleRect().height / outerInstance.hex.Measures.CellHeight;
					if (rows > 2)
					{
						rows--;
					}
					if (outerInstance.cursor >= rows * cols)
					{
						outerInstance.setDot(outerInstance.cursor - rows * cols, shift);
					}
					else if (outerInstance.cursor >= cols)
					{
						outerInstance.setDot(outerInstance.cursor % cols, shift);
					}
					break;
				}
			}

			public virtual void keyReleased(KeyEvent e)
			{
			}

			public virtual void focusGained(FocusEvent e)
			{
				outerInstance.expose(outerInstance.cursor, false);
			}

			public virtual void focusLost(FocusEvent e)
			{
				outerInstance.expose(outerInstance.cursor, false);
			}
		}

		private static readonly Stroke CURSOR_STROKE = new BasicStroke(2.0f);

		private HexEditor hex;
		private List<ChangeListener> listeners;
		private long mark;
		private long cursor;
		private object highlight;

		internal Caret(HexEditor hex)
		{
			this.hex = hex;
			this.listeners = new List<ChangeListener>();
			this.cursor = -1;

			Listener l = new Listener(this);
			hex.addMouseListener(l);
			hex.addMouseMotionListener(l);
			hex.addKeyListener(l);
			hex.addFocusListener(l);

			InputMap imap = hex.getInputMap();
			ActionMap amap = hex.getActionMap();
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
			private readonly Caret outerInstance;

			public AbstractActionAnonymousInnerClass(Caret outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
			}
		}

		public virtual void addChangeListener(ChangeListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeChangeListener(ChangeListener l)
		{
			listeners.Remove(l);
		}

		public virtual long Mark
		{
			get
			{
				return mark;
			}
		}

		public virtual long Dot
		{
			get
			{
				return cursor;
			}
		}

		public virtual void setDot(long value, bool keepMark)
		{
			HexModel model = hex.Model;
			if (model == null || value < model.FirstOffset || value > model.LastOffset)
			{
				value = -1;
			}
			if (cursor != value)
			{
				long oldValue = cursor;
				if (highlight != null)
				{
					hex.Highlighter.remove(highlight);
					highlight = null;
				}
				if (!keepMark)
				{
					mark = value;
				}
				else if (mark != value)
				{
					highlight = hex.Highlighter.add(mark, value, SELECT_COLOR);
				}
				cursor = value;
				expose(oldValue, false);
				expose(value, true);
				if (listeners.Count > 0)
				{
					ChangeEvent @event = new ChangeEvent(this);
					foreach (ChangeListener l in listeners)
					{
						l.stateChanged(@event);
					}
				}
			}
		}

		private void expose(long loc, bool scrollTo)
		{
			if (loc >= 0)
			{
				Measures measures = hex.Measures;
				int x = measures.toX(loc);
				int y = measures.toY(loc);
				int w = measures.CellWidth;
				int h = measures.CellHeight;
				hex.repaint(x - 1, y - 1, w + 2, h + 2);
				if (scrollTo)
				{
					hex.scrollRectToVisible(new Rectangle(x, y, w, h));
				}
			}
		}

		internal virtual void paintForeground(Graphics g, long start, long end)
		{
			if (cursor >= start && cursor < end && hex.isFocusOwner())
			{
				Measures measures = hex.Measures;
				int x = measures.toX(cursor);
				int y = measures.toY(cursor);
				Graphics2D g2 = (Graphics2D) g;
				Stroke oldStroke = g2.getStroke();
				g2.setColor(hex.getForeground());
				g2.setStroke(CURSOR_STROKE);
				g2.drawRect(x, y, measures.CellWidth - 1, measures.CellHeight - 1);
				g2.setStroke(oldStroke);
			}
		}
	}

}
