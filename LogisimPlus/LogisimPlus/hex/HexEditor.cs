// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace hex
{


	public class HexEditor : JComponent, Scrollable
	{
		private class Listener : HexModelListener
		{
			private readonly HexEditor outerInstance;

			public Listener(HexEditor outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void metainfoChanged(HexModel source)
			{
				outerInstance.measures.recompute();
				repaint();
			}

			public virtual void bytesChanged(HexModel source, long start, long numBytes, int[] oldValues)
			{
				repaint(0, outerInstance.measures.toY(start), getWidth(), outerInstance.measures.toY(start + numBytes) + outerInstance.measures.CellHeight);
			}
		}

		private HexModel model;
		private Listener listener;
		private Measures measures;
		private Caret caret;
		private Highlighter highlighter;

		public HexEditor(HexModel model)
		{
			this.model = model;
			this.listener = new Listener(this);
			this.measures = new Measures(this);
			this.caret = new Caret(this);
			this.highlighter = new Highlighter(this);

			setOpaque(true);
			setBackground(Color.WHITE);
			if (model != null)
			{
				model.addHexModelListener(listener);
			}

			measures.recompute();
		}

		internal virtual Measures Measures
		{
			get
			{
				return measures;
			}
		}

		internal virtual Highlighter Highlighter
		{
			get
			{
				return highlighter;
			}
		}

		public virtual HexModel Model
		{
			get
			{
				return model;
			}
			set
			{
				if (model == value)
				{
					return;
				}
				if (model != null)
				{
					model.removeHexModelListener(listener);
				}
				model = value;
				highlighter.clear();
				caret.setDot(-1, false);
				if (model != null)
				{
					model.addHexModelListener(listener);
				}
				measures.recompute();
			}
		}

		public virtual Caret Caret
		{
			get
			{
				return caret;
			}
		}

		public virtual object addHighlight(int start, int end, Color color)
		{
			return highlighter.add(start, end, color);
		}

		public virtual void removeHighlight(object tag)
		{
			highlighter.remove(tag);
		}


		public virtual void scrollAddressToVisible(int start, int end)
		{
			if (start < 0 || end < 0)
			{
				return;
			}
			int x0 = measures.toX(start);
			int x1 = measures.toX(end) + measures.CellWidth;
			int y0 = measures.toY(start);
			int y1 = measures.toY(end);
			int h = measures.CellHeight;
			if (y0 == y1)
			{
				scrollRectToVisible(new Rectangle(x0, y0, x1 - x0, h));
			}
			else
			{
				scrollRectToVisible(new Rectangle(x0, y0, x1 - x0, (y1 + h) - y0));
			}
		}

		public override Font Font
		{
			set
			{
				base.setFont(value);
				measures.recompute();
			}
		}

		public override void setBounds(int x, int y, int width, int height)
		{
			base.setBounds(x, y, width, height);
			measures.widthChanged();
		}

		protected internal override void paintComponent(Graphics g)
		{
			measures.ensureComputed(g);

			Rectangle clip = g.getClipBounds();
			if (isOpaque())
			{
				g.setColor(getBackground());
				g.fillRect(clip.x, clip.y, clip.width, clip.height);
			}

			long addr0 = model.FirstOffset;
			long addr1 = model.LastOffset;

			long xaddr0 = measures.toAddress(0, clip.y);
			if (xaddr0 == addr0)
			{
				xaddr0 = measures.getBaseAddress(model);
			}
			long xaddr1 = measures.toAddress(getWidth(), clip.y + clip.height) + 1;
			highlighter.paint(g, xaddr0, xaddr1);

			g.setColor(getForeground());
			Font baseFont = g.getFont();
			FontMetrics baseFm = g.getFontMetrics(baseFont);
			Font labelFont = baseFont.deriveFont(Font.ITALIC);
			FontMetrics labelFm = g.getFontMetrics(labelFont);
			int cols = measures.ColumnCount;
			int baseX = measures.BaseX;
			int baseY = measures.toY(xaddr0) + baseFm.getAscent() + baseFm.getLeading() / 2;
			int dy = measures.CellHeight;
			int labelWidth = measures.LabelWidth;
			int labelChars = measures.LabelChars;
			int cellWidth = measures.CellWidth;
			int cellChars = measures.CellChars;
			for (long a = (int)xaddr0; a < xaddr1; a += cols, baseY += dy)
			{
				string label = toHex(a, labelChars);
				g.setFont(labelFont);
				g.drawString(label, baseX - labelWidth + (labelWidth - labelFm.stringWidth(label)) / 2, baseY);
				g.setFont(baseFont);
				long b = a;
				for (int j = 0; j < cols; j++, b++)
				{
					if (b >= addr0 && b <= addr1)
					{
						string val = toHex(model.get(b), cellChars);
						int x = measures.toX(b) + (cellWidth - baseFm.stringWidth(val)) / 2;
						g.drawString(val, x, baseY);
					}
				}
			}

			caret.paintForeground(g, xaddr0, xaddr1);
		}

		private string toHex(long value, int chars)
		{
			string ret = Convert.ToString(value, 16);
			int retLen = ret.Length;
			if (retLen < chars)
			{
				ret = "0" + ret;
				for (int i = retLen + 1; i < chars; i++)
				{
					ret = "0" + ret;
				}
				return ret;
			}
			else if (retLen == chars)
			{
				return ret;
			}
			else
			{
				return ret.Substring(retLen - chars);
			}
		}

		//
		// selection methods
		//
		public virtual bool selectionExists()
		{
			return caret.Mark >= 0 && caret.Dot >= 0;
		}

		public virtual void selectAll()
		{
			caret.setDot(model.LastOffset, false);
			caret.setDot(0, true);
		}

		public virtual void delete()
		{
			long p0 = caret.Mark;
			long p1 = caret.Dot;
			if (p0 < 0 || p1 < 0)
			{
				return;
			}
			if (p0 > p1)
			{
				long t = p0;
				p0 = p1;
				p1 = t;
			}
			model.fill(p0, p1 - p0 + 1, 0);
		}

		//
		// Scrollable methods
		//
		public virtual Dimension PreferredScrollableViewportSize
		{
			get
			{
				return getPreferredSize();
			}
		}

		public virtual int getScrollableUnitIncrement(Rectangle vis, int orientation, int direction)
		{
			if (orientation == SwingConstants.VERTICAL)
			{
				int ret = measures.CellHeight;
				if (ret < 1)
				{
					measures.recompute();
					ret = measures.CellHeight;
					if (ret < 1)
					{
						return 1;
					}
				}
				return ret;
			}
			else
			{
				return Math.Max(1, vis.width / 20);
			}
		}

		public virtual int getScrollableBlockIncrement(Rectangle vis, int orientation, int direction)
		{
			if (orientation == SwingConstants.VERTICAL)
			{
				int height = measures.CellHeight;
				if (height < 1)
				{
					measures.recompute();
					height = measures.CellHeight;
					if (height < 1)
					{
						return 19 * vis.height / 20;
					}
				}
				int lines = Math.Max(1, (vis.height / height) - 1);
				return lines * height;
			}
			else
			{
				return 19 * vis.width / 20;
			}
		}

		public virtual bool ScrollableTracksViewportWidth
		{
			get
			{
				return true;
			}
		}

		public virtual bool ScrollableTracksViewportHeight
		{
			get
			{
				return false;
			}
		}
	}

}
