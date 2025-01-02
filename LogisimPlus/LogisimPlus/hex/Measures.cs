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

	internal class Measures
	{
		private HexEditor hex;
		private int headerChars;
		private int cellChars;
		private int headerWidth;
		private int spacerWidth;
		private int cellWidth;
		private int cellHeight;
		private int cols;
		private int baseX;
		private bool guessed;

		public Measures(HexEditor hex)
		{
			this.hex = hex;
			this.guessed = true;
			this.cols = 1;
			this.cellWidth = -1;
			this.cellHeight = -1;
			this.cellChars = 2;
			this.headerChars = 4;

			computeCellSize(null);
		}

		public virtual int ColumnCount
		{
			get
			{
				return cols;
			}
		}

		public virtual int BaseX
		{
			get
			{
				return baseX;
			}
		}

		public virtual int CellHeight
		{
			get
			{
				return cellHeight;
			}
		}

		public virtual int CellWidth
		{
			get
			{
				return cellWidth;
			}
		}

		public virtual int LabelWidth
		{
			get
			{
				return headerWidth;
			}
		}

		public virtual int LabelChars
		{
			get
			{
				return headerChars;
			}
		}

		public virtual int CellChars
		{
			get
			{
				return cellChars;
			}
		}

		public virtual int ValuesX
		{
			get
			{
				return baseX + spacerWidth;
			}
		}

		public virtual int ValuesWidth
		{
			get
			{
				return ((cols - 1) / 4) * spacerWidth + cols * cellWidth;
			}
		}

		public virtual long getBaseAddress(HexModel model)
		{
			if (model == null)
			{
				return 0;
			}
			else
			{
				long addr0 = model.FirstOffset;
				return addr0 - addr0 % cols;
			}
		}

		public virtual int toY(long addr)
		{
			long row = (addr - getBaseAddress(hex.Model)) / cols;
			long ret = row * cellHeight;
			return ret < int.MaxValue ? (int) ret : int.MaxValue;
		}

		public virtual int toX(long addr)
		{
			int col = (int)(addr % cols);
			return baseX + (1 + (col / 4)) * spacerWidth + col * cellWidth;
		}

		public virtual long toAddress(int x, int y)
		{
			HexModel model = hex.Model;
			if (model == null)
			{
				return int.MinValue;
			}
			long addr0 = model.FirstOffset;
			long addr1 = model.LastOffset;

			long @base = getBaseAddress(model) + ((long) y / cellHeight) * cols;
			int offs = (x - baseX) / (cellWidth + (spacerWidth + 2) / 4);
			if (offs < 0)
			{
				offs = 0;
			}
			if (offs >= cols)
			{
				offs = cols - 1;
			}

			long ret = @base + offs;
			if (ret > addr1)
			{
				ret = addr1;
			}
			if (ret < addr0)
			{
				ret = addr0;
			}
			return ret;
		}

		internal virtual void ensureComputed(Graphics g)
		{
			if (guessed || cellWidth < 0)
			{
				computeCellSize(g);
			}
		}

		internal virtual void recompute()
		{
			computeCellSize(hex.getGraphics());
		}

		internal virtual void widthChanged()
		{
			int oldCols = cols;
			int width;
			if (guessed || cellWidth < 0)
			{
				cols = 16;
				width = hex.getPreferredSize().width;
			}
			else
			{
				width = hex.getWidth();
				int ret = (width - headerWidth) / (cellWidth + (spacerWidth + 3) / 4);
				if (ret >= 16)
				{
					cols = 16;
				}
				else if (ret >= 8)
				{
					cols = 8;
				}
				else
				{
					cols = 4;
				}
			}
			int lineWidth = headerWidth + cols * cellWidth + ((cols / 4) - 1) * spacerWidth;
			int newBase = headerWidth + Math.Max(0, (width - lineWidth) / 2);
			if (baseX != newBase)
			{
				baseX = newBase;
				hex.repaint();
			}
			if (cols != oldCols)
			{
				recompute();
			}
		}

		private void computeCellSize(Graphics g)
		{
			HexModel model = hex.Model;

			// compute number of characters in headers and cells
			if (model == null)
			{
				headerChars = 4;
				cellChars = 2;
			}
			else
			{
				int logSize = 0;
				long addrEnd = model.LastOffset;
				while (addrEnd > (1L << logSize))
				{
					logSize++;
				}
				headerChars = (logSize + 3) / 4;
				cellChars = (model.ValueWidth + 3) / 4;
			}

			// compute character sizes
			FontMetrics fm = g == null ? null : g.getFontMetrics(hex.getFont());
			int charWidth;
			int spaceWidth;
			int lineHeight;
			if (fm == null)
			{
				charWidth = 8;
				spaceWidth = 6;
				Font font = hex.getFont();
				if (font == null)
				{
					lineHeight = 16;
				}
				else
				{
					lineHeight = font.getSize();
				}
			}
			else
			{
				guessed = false;
				charWidth = 0;
				for (int i = 0; i < 16; i++)
				{
					int width = fm.stringWidth(Convert.ToString(i, 16));
					if (width > charWidth)
					{
						charWidth = width;
					}
				}
				spaceWidth = fm.stringWidth(" ");
				lineHeight = fm.getHeight();
			}

			// update header and cell dimensions
			headerWidth = headerChars * charWidth + spaceWidth;
			spacerWidth = spaceWidth;
			cellWidth = cellChars * charWidth + spaceWidth;
			cellHeight = lineHeight;

			// compute preferred size
			int width = headerWidth + cols * cellWidth + (cols / 4) * spacerWidth;
			long height;
			if (model == null)
			{
				height = 16 * cellHeight;
			}
			else
			{
				long addr0 = getBaseAddress(model);
				long addr1 = model.LastOffset;
				long rows = (int)(((addr1 - addr0 + 1) + cols - 1) / cols);
				height = rows * cellHeight;
				if (height > int.MaxValue)
				{
					height = int.MaxValue;
				}
			}

			// update preferred size
			Dimension pref = hex.getPreferredSize();
			if (pref.width != width || pref.height != height)
			{
				pref.width = width;
				pref.height = (int) height;
				hex.setPreferredSize(pref);
				hex.revalidate();
			}

			widthChanged();
		}
	}

}
