// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{

	using Bounds = logisim.data.Bounds;
	using Caret = logisim.tools.Caret;
	using CaretEvent = logisim.tools.CaretEvent;
	using CaretListener = logisim.tools.CaretListener;

	internal class TextFieldCaret : Caret, TextFieldListener
	{
		private LinkedList<CaretListener> listeners = new LinkedList<CaretListener>();
		private TextField field;
		private Graphics g;
		private string oldText;
		private string curText;
		private int pos;

		public TextFieldCaret(TextField field, Graphics g, int pos)
		{
			this.field = field;
			this.g = g;
			this.oldText = field.Text;
			this.curText = field.Text;
			this.pos = pos;

			field.addTextFieldListener(this);
		}

		public TextFieldCaret(TextField field, Graphics g, int x, int y) : this(field, g, 0)
		{
			moveCaret(x, y);
		}

		public virtual void addCaretListener(CaretListener l)
		{
			listeners.AddLast(l);
		}

		public virtual void removeCaretListener(CaretListener l)
		{
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
			listeners.remove(l);
		}

		public virtual string Text
		{
			get
			{
				return curText;
			}
		}

		public virtual void commitText(string text)
		{
			curText = text;
			pos = curText.Length;
			field.Text = text;
		}

		public virtual void draw(Graphics g)
		{
			if (field.Font != null)
			{
				g.setFont(field.Font);
			}

			// draw boundary
			Bounds bds = getBounds(g);
			g.setColor(Color.white);
			g.fillRect(bds.X, bds.Y, bds.Width, bds.Height);
			g.setColor(Color.black);
			g.drawRect(bds.X, bds.Y, bds.Width, bds.Height);

			// draw text
			int x = field.X;
			int y = field.Y;
			FontMetrics fm = g.getFontMetrics();
			int width = fm.stringWidth(curText);
			int ascent = fm.getAscent();
			int descent = fm.getDescent();
			switch (field.HAlign)
			{
			case TextField.H_CENTER:
				x -= width / 2;
				break;
			case TextField.H_RIGHT:
				x -= width;
				break;
			default:
				break;
			}
			switch (field.VAlign)
			{
			case TextField.V_TOP:
				y += ascent;
				break;
			case TextField.V_CENTER:
				y += (ascent - descent) / 2;
				break;
			case TextField.V_BOTTOM:
				y -= descent;
				break;
			default:
				break;
			}
			g.drawString(curText, x, y);

			// draw cursor
			if (pos > 0)
			{
				x += fm.stringWidth(curText.Substring(0, pos));
			}
			g.drawLine(x, y, x, y - ascent);
		}

		public virtual Bounds getBounds(Graphics g)
		{
			int x = field.X;
			int y = field.Y;
			Font font = field.Font;
			FontMetrics fm;
			if (font == null)
			{
				fm = g.getFontMetrics();
			}
			else
			{
				fm = g.getFontMetrics(font);
			}
			int width = fm.stringWidth(curText);
			int ascent = fm.getAscent();
			int descent = fm.getDescent();
			int height = ascent + descent;
			switch (field.HAlign)
			{
			case TextField.H_CENTER:
				x -= width / 2;
				break;
			case TextField.H_RIGHT:
				x -= width;
				break;
			default:
				break;
			}
			switch (field.VAlign)
			{
			case TextField.V_TOP:
				y += ascent;
				break;
			case TextField.V_CENTER:
				y += (ascent - descent) / 2;
				break;
			case TextField.V_BOTTOM:
				y -= descent;
				break;
			default:
				break;
			}
			return Bounds.create(x, y - ascent, width, height).add(field.getBounds(g)).expand(3);
		}

		public virtual void cancelEditing()
		{
			CaretEvent e = new CaretEvent(this, oldText, oldText);
			curText = oldText;
			pos = curText.Length;
			foreach (CaretListener l in new List<CaretListener>(listeners))
			{
				l.editingCanceled(e);
			}
			field.removeTextFieldListener(this);
		}

		public virtual void stopEditing()
		{
			CaretEvent e = new CaretEvent(this, oldText, curText);
			field.Text = curText;
			foreach (CaretListener l in new List<CaretListener>(listeners))
			{
				l.editingStopped(e);
			}
			field.removeTextFieldListener(this);
		}

		public virtual void mousePressed(MouseEvent e)
		{
			// TODO: enhance label editing
			moveCaret(e.getX(), e.getY());
		}

		public virtual void mouseDragged(MouseEvent e)
		{
			// TODO: enhance label editing
		}

		public virtual void mouseReleased(MouseEvent e)
		{
			// TODO: enhance label editing
			moveCaret(e.getX(), e.getY());
		}

		public virtual void keyPressed(KeyEvent e)
		{
			int ign = InputEvent.ALT_DOWN_MASK | InputEvent.CTRL_DOWN_MASK | InputEvent.META_DOWN_MASK;
			if ((e.getModifiersEx() & ign) != 0)
			{
				return;
			}
			switch (e.getKeyCode())
			{
			case KeyEvent.VK_LEFT:
			case KeyEvent.VK_KP_LEFT:
				if (pos > 0)
				{
					--pos;
				}
				break;
			case KeyEvent.VK_RIGHT:
			case KeyEvent.VK_KP_RIGHT:
				if (pos < curText.Length)
				{
					++pos;
				}
				break;
			case KeyEvent.VK_HOME:
				pos = 0;
				break;
			case KeyEvent.VK_END:
				pos = curText.Length;
				break;
			case KeyEvent.VK_ESCAPE:
			case KeyEvent.VK_CANCEL:
				cancelEditing();
				break;
			case KeyEvent.VK_CLEAR:
				curText = "";
				pos = 0;
				break;
			case KeyEvent.VK_ENTER:
				stopEditing();
				break;
			case KeyEvent.VK_BACK_SPACE:
				if (pos > 0)
				{
					curText = curText.Substring(0, pos - 1) + curText.Substring(pos);
					--pos;
				}
				break;
			case KeyEvent.VK_DELETE:
				if (pos < curText.Length)
				{
					curText = curText.Substring(0, pos) + curText.Substring(pos + 1);
				}
				break;
			case KeyEvent.VK_INSERT:
			case KeyEvent.VK_COPY:
			case KeyEvent.VK_CUT:
			case KeyEvent.VK_PASTE:
				// TODO: enhance label editing
				break;
			default:
				; // ignore
			break;
			}
		}

		public virtual void keyReleased(KeyEvent e)
		{
		}

		public virtual void keyTyped(KeyEvent e)
		{
			int ign = InputEvent.ALT_DOWN_MASK | InputEvent.CTRL_DOWN_MASK | InputEvent.META_DOWN_MASK;
			if ((e.getModifiersEx() & ign) != 0)
			{
				return;
			}

			char c = e.getKeyChar();
			if (c == '\n')
			{
				stopEditing();
			}
			else if (c != KeyEvent.CHAR_UNDEFINED && !char.IsControl(c))
			{
				if (pos < curText.Length)
				{
					curText = curText.Substring(0, pos) + c + curText.Substring(pos);
				}
				else
				{
					curText += c;
				}
				++pos;
			}
		}

		private void moveCaret(int x, int y)
		{
			Bounds bds = getBounds(g);
			FontMetrics fm = g.getFontMetrics();
			x -= bds.X;
			int last = 0;
			for (int i = 0; i < curText.Length; i++)
			{
				int cur = fm.stringWidth(curText.Substring(0, i + 1));
				if (x <= (last + cur) / 2)
				{
					pos = i;
					return;
				}
				last = cur;
			}
			pos = curText.Length;
		}

		public virtual void textChanged(TextFieldEvent e)
		{
			curText = field.Text;
			oldText = curText;
			pos = curText.Length;
		}
	}

}
