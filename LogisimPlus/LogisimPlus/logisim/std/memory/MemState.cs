// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using HexModel = hex.HexModel;
	using HexModelListener = hex.HexModelListener;
	using Bounds = logisim.data.Bounds;
	using InstanceData = logisim.instance.InstanceData;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using StringUtil = logisim.util.StringUtil;

	internal class MemState : InstanceData, Cloneable, HexModelListener
	{
		private const int ROWS = 4; // rows in memory display

		private const int TABLE_WIDTH12 = 80; // width of table for addr bits <= 12
		private const int TABLE_WIDTH32 = 65; // width of table for addr bits > 12

		private const int ENTRY_HEIGHT = 15; // pixels high per entry

		private const int ENTRY_XOFFS12 = 40; // x offset for entries for addr bits <= 12
		private const int ENTRY_XOFFS32 = 60; // x offset for entries for addr bits > 12

		private const int ENTRY_YOFFS = 5; // y offset for entries

		private const int ADDR_WIDTH_PER_CHAR = 10; // pixels wide per address character

		private MemContents contents;
		private int columns;
		private long curScroll = 0;
		private long cursorLoc = -1;
		private long curAddr = -1;

		internal MemState(MemContents contents)
		{
			this.contents = contents;
			setBits(contents.LogLength, contents.Width);
			contents.addHexModelListener(this);
		}

		public virtual MemState clone()
		{
			try
			{
				MemState ret = (MemState) base.clone();
				ret.contents = contents.clone();
				ret.contents.addHexModelListener(ret);
				return ret;
			}
			catch (CloneNotSupportedException)
			{
				return null;
			}
		}

		//
		// methods for accessing the address bits
		//
		private void setBits(int addrBits, int dataBits)
		{
			if (contents == null)
			{
				contents = MemContents.create(addrBits, dataBits);
			}
			else
			{
				contents.setSizes(addrBits, dataBits);
			}
			if (addrBits <= 12)
			{
				if (dataBits <= 8)
				{
					columns = dataBits <= 4 ? 8 : 4;
				}
				else
				{
					columns = dataBits <= 16 ? 2 : 1;
				}
			}
			else
			{
				columns = dataBits <= 8 ? 2 : 1;
			}
			long newLast = contents.LastOffset;
			// I do subtraction in the next two conditions to account for possibility of overflow
			if (cursorLoc > newLast)
			{
				cursorLoc = newLast;
			}
			if (curAddr - newLast > 0)
			{
				curAddr = -1;
			}
			long maxScroll = Math.Max(0, newLast + 1 - (ROWS - 1) * columns);
			if (curScroll > maxScroll)
			{
				curScroll = maxScroll;
			}
		}

		public virtual MemContents Contents
		{
			get
			{
				return contents;
			}
		}

		//
		// methods for accessing data within memory
		//
		internal virtual int AddrBits
		{
			get
			{
				return contents.LogLength;
			}
		}

		internal virtual int DataBits
		{
			get
			{
				return contents.Width;
			}
		}

		internal virtual long LastAddress
		{
			get
			{
				return (1L << contents.LogLength) - 1;
			}
		}

		internal virtual bool isValidAddr(long addr)
		{
			int addrBits = contents.LogLength;
			return addr >>> addrBits == 0;
		}

		internal virtual int Rows
		{
			get
			{
				return ROWS;
			}
		}

		internal virtual int Columns
		{
			get
			{
				return columns;
			}
		}

		//
		// methods for manipulating cursor and scroll location
		//
		internal virtual long Cursor
		{
			get
			{
				return cursorLoc;
			}
			set
			{
				cursorLoc = isValidAddr(value) ? value : -1L;
			}
		}

		internal virtual long Current
		{
			get
			{
				return curAddr;
			}
			set
			{
				curAddr = isValidAddr(value) ? value : -1L;
			}
		}

		internal virtual long Scroll
		{
			get
			{
				return curScroll;
			}
			set
			{
				long maxAddr = LastAddress - ROWS * columns;
				if (value > maxAddr)
				{
					value = maxAddr; // note: maxAddr could be negative
				}
				if (value < 0)
				{
					value = 0;
				}
				curScroll = value;
			}
		}



		internal virtual void scrollToShow(long addr)
		{
			if (isValidAddr(addr))
			{
				addr = addr / columns * columns;
				long curTop = curScroll / columns * columns;
				if (addr < curTop)
				{
					curScroll = addr;
				}
				else if (addr >= curTop + ROWS * columns)
				{
					curScroll = addr - (ROWS - 1) * columns;
					if (curScroll < 0)
					{
						curScroll = 0;
					}
				}
			}
		}


		//
		// graphical methods
		//
		public virtual long getAddressAt(int x, int y)
		{
			int addrBits = AddrBits;
			int boxX = addrBits <= 12 ? ENTRY_XOFFS12 : ENTRY_XOFFS32;
			int boxW = addrBits <= 12 ? TABLE_WIDTH12 : TABLE_WIDTH32;

			// See if outside box
			if (x < boxX || x >= boxX + boxW || y <= ENTRY_YOFFS || y >= ENTRY_YOFFS + ROWS * ENTRY_HEIGHT)
			{
				return -1;
			}

			int col = (x - boxX) / (boxW / columns);
			int row = (y - ENTRY_YOFFS) / ENTRY_HEIGHT;
			long ret = (curScroll / columns * columns) + columns * row + col;
			return isValidAddr(ret) ? ret : LastAddress;
		}

		public virtual Bounds getBounds(long addr, Bounds bds)
		{
			int addrBits = AddrBits;
			int boxX = bds.X + (addrBits <= 12 ? ENTRY_XOFFS12 : ENTRY_XOFFS32);
			int boxW = addrBits <= 12 ? TABLE_WIDTH12 : TABLE_WIDTH32;
			if (addr < 0)
			{
				int addrLen = (contents.Width + 3) / 4;
				int width = ADDR_WIDTH_PER_CHAR * addrLen;
				return Bounds.create(boxX - width, bds.Y + ENTRY_YOFFS, width, ENTRY_HEIGHT);
			}
			else
			{
				int bdsX = addrToX(bds, addr);
				int bdsY = addrToY(bds, addr);
				return Bounds.create(bdsX, bdsY, boxW / columns, ENTRY_HEIGHT);
			}
		}

		public virtual void paint(Graphics g, int leftX, int topY)
		{
			int addrBits = AddrBits;
			int dataBits = contents.Width;
			int boxX = leftX + (addrBits <= 12 ? ENTRY_XOFFS12 : ENTRY_XOFFS32);
			int boxY = topY + ENTRY_YOFFS;
			int boxW = addrBits <= 12 ? TABLE_WIDTH12 : TABLE_WIDTH32;
			int boxH = ROWS * ENTRY_HEIGHT;

			GraphicsUtil.switchToWidth(g, 1);
			g.drawRect(boxX, boxY, boxW, boxH);
			int entryWidth = boxW / columns;
			for (int row = 0; row < ROWS; row++)
			{
				long addr = (curScroll / columns * columns) + columns * row;
				int x = boxX;
				int y = boxY + ENTRY_HEIGHT * row;
				int yoffs = ENTRY_HEIGHT - 3;
				if (isValidAddr(addr))
				{
					g.setColor(Color.GRAY);
					GraphicsUtil.drawText(g, StringUtil.toHexString(AddrBits, (int) addr), x - 2, y + yoffs, GraphicsUtil.H_RIGHT, GraphicsUtil.V_BASELINE);
				}
				g.setColor(Color.BLACK);
				for (int col = 0; col < columns && isValidAddr(addr); col++)
				{
					int val = contents.get(addr);
					if (addr == curAddr)
					{
						g.fillRect(x, y, entryWidth, ENTRY_HEIGHT);
						g.setColor(Color.WHITE);
						GraphicsUtil.drawText(g, StringUtil.toHexString(dataBits, val), x + entryWidth / 2, y + yoffs, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
						g.setColor(Color.BLACK);
					}
					else
					{
						GraphicsUtil.drawText(g, StringUtil.toHexString(dataBits, val), x + entryWidth / 2, y + yoffs, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
					}
					addr++;
					x += entryWidth;
				}
			}
		}

		private int addrToX(Bounds bds, long addr)
		{
			int addrBits = AddrBits;
			int boxX = bds.X + (addrBits <= 12 ? ENTRY_XOFFS12 : ENTRY_XOFFS32);
			int boxW = addrBits <= 12 ? TABLE_WIDTH12 : TABLE_WIDTH32;

			long topRow = curScroll / columns;
			long row = addr / columns;
			if (row < topRow || row >= topRow + ROWS)
			{
				return -1;
			}
			int col = (int)(addr - row * columns);
			if (col < 0 || col >= columns)
			{
				return -1;
			}
			return boxX + boxW * col / columns;
		}

		private int addrToY(Bounds bds, long addr)
		{
			long topRow = curScroll / columns;
			long row = addr / columns;
			if (row < topRow || row >= topRow + ROWS)
			{
				return -1;
			}
			return (int)(bds.Y + ENTRY_YOFFS + ENTRY_HEIGHT * (row - topRow));
		}

		public virtual void metainfoChanged(HexModel source)
		{
			setBits(contents.LogLength, contents.Width);
		}

		public virtual void bytesChanged(HexModel source, long start, long numBytes, int[] oldValues)
		{
		}
	}

}
