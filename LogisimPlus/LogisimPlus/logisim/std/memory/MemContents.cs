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
	using logisim.util;

	internal class MemContents : ICloneable, HexModel
	{
		private const int PAGE_SIZE_BITS = 12;
		private static readonly int PAGE_SIZE = 1 << PAGE_SIZE_BITS;
		private static readonly int PAGE_MASK = PAGE_SIZE - 1;

		internal static MemContents create(int addrBits, int width)
		{
			return new MemContents(addrBits, width);
		}

		private EventSourceWeakSupport<HexModelListener> listeners = null;
		private int width;
		private int addrBits;
		private int mask;
		private MemContentsSub.ContentsInterface[] pages;

		private MemContents(int addrBits, int width)
		{
			listeners = null;
			setSizes(addrBits, width);
		}

		//
		// HexModel methods
		//
		public virtual void addHexModelListener(HexModelListener l)
		{
			if (listeners == null)
			{
				listeners = new EventSourceWeakSupport<HexModelListener>();
			}
			listeners.add(l);
		}

		public virtual void removeHexModelListener(HexModelListener l)
		{
			if (listeners == null)
			{
				return;
			}
			listeners.add(l);
			if (listeners.Empty)
			{
				listeners = null;
			}
		}

		private void fireMetainfoChanged()
		{
			if (listeners == null)
			{
				return;
			}
			bool found = false;
			foreach (HexModelListener l in listeners)
			{
				found = true;
				l.metainfoChanged(this);
			}
			if (!found)
			{
				listeners = null;
			}
		}

		private void fireBytesChanged(long start, long numBytes, int[] oldValues)
		{
			if (listeners == null)
			{
				return;
			}
			bool found = false;
			foreach (HexModelListener l in listeners)
			{
				found = true;
				l.bytesChanged(this, start, numBytes, oldValues);
			}
			if (!found)
			{
				listeners = null;
			}
		}

        //
        // other methods
        //
        public virtual object Clone()
        {
            MemContents ret = (MemContents)base.MemberwiseClone();
            ret.listeners = null;
            ret.pages = new MemContentsSub.ContentsInterface[this.pages.Length];
            for (int i = 0; i < ret.pages.Length; i++)
            {
                if (this.pages[i] != null)
                {
                    ret.pages[i] = this.pages[i].clone();
                }
            }
            return ret;
        }

		public virtual int LogLength
		{
			get
			{
				return addrBits;
			}
		}

		public virtual int Width
		{
			get
			{
				return width;
			}
		}

		public virtual int get(long addr)
		{
			int page = (int)(addr >>> PAGE_SIZE_BITS);
			int offs = (int)(addr & PAGE_MASK);
			if (page < 0 || page >= pages.Length || pages[page] == null)
			{
				return 0;
			}
			return pages[page].get(offs) & mask;
		}

		public virtual bool Clear
		{
			get
			{
				for (int i = 0; i < pages.Length; i++)
				{
					MemContentsSub.ContentsInterface page = pages[i];
					if (page != null)
					{
						for (int j = page.Length - 1; j >= 0; j--)
						{
							if (page.get(j) != 0)
							{
								return false;
							}
						}
					}
				}
				return true;
			}
		}

		public virtual void set(long addr, int value)
		{
			int page = (int)(addr >>> PAGE_SIZE_BITS);
			int offs = (int)(addr & PAGE_MASK);
			int old = pages[page] == null ? 0 : pages[page].get(offs) & mask;
			int val = value & mask;
			if (old != val)
			{
				if (pages[page] == null)
				{
					pages[page] = MemContentsSub.createContents(PAGE_SIZE, width);
				}
				pages[page].set(offs, val);
				fireBytesChanged(addr, 1, new int[] {old});
			}
		}

		public virtual void set(long start, int[] values)
		{
			if (values.Length == 0)
			{
				return;
			}

			int pageStart = (int)(start >>> PAGE_SIZE_BITS);
			int startOffs = (int)(start & PAGE_MASK);
			int pageEnd = (int)((start + values.Length - 1) >>> PAGE_SIZE_BITS);
			int endOffs = (int)((start + values.Length - 1) & PAGE_MASK);

			if (pageStart == pageEnd)
			{
				ensurePage(pageStart);
				MemContentsSub.ContentsInterface page = pages[pageStart];
				if (!page.matches(values, startOffs, mask))
				{
					int[] oldValues = page.get(startOffs, values.Length);
					page.load(startOffs, values, mask);
					if (page.Clear)
					{
						pages[pageStart] = null;
					}
					fireBytesChanged(start, values.Length, oldValues);
				}
			}
			else
			{
				int nextOffs;
				if (startOffs == 0)
				{
					pageStart--;
					nextOffs = 0;
				}
				else
				{
					ensurePage(pageStart);
					int[] vals = new int[PAGE_SIZE - startOffs];
					Array.Copy(values, 0, vals, 0, vals.Length);
					MemContentsSub.ContentsInterface page = pages[pageStart];
					if (!page.matches(vals, startOffs, mask))
					{
						int[] oldValues = page.get(startOffs, vals.Length);
						page.load(startOffs, vals, mask);
						if (page.Clear)
						{
							pages[pageStart] = null;
						}
						fireBytesChanged(start, PAGE_SIZE - pageStart, oldValues);
					}
					nextOffs = vals.Length;
				}
				int[] vals = new int[PAGE_SIZE];
				int offs = nextOffs;
				for (int i = pageStart + 1; i < pageEnd; i++, offs += PAGE_SIZE)
				{
					MemContentsSub.ContentsInterface page = pages[i];
					if (page == null)
					{
						bool allZeroes = true;
						for (int j = 0; j < PAGE_SIZE; j++)
						{
							if ((values[offs + j] & mask) != 0)
							{
								allZeroes = false;
								break;
							}
						}
						if (!allZeroes)
						{
							page = MemContentsSub.createContents(PAGE_SIZE, width);
							pages[i] = page;
						}
					}
					if (page != null)
					{
						Array.Copy(values, offs, vals, 0, PAGE_SIZE);
						if (!page.matches(vals, startOffs, mask))
						{
							int[] oldValues = page.get(0, PAGE_SIZE);
							page.load(0, vals, mask);
							if (page.Clear)
							{
								pages[i] = null;
							}
							fireBytesChanged((long) i << PAGE_SIZE_BITS, PAGE_SIZE, oldValues);
						}
					}
				}
				if (endOffs >= 0)
				{
					ensurePage(pageEnd);
					vals = new int[endOffs + 1];
					Array.Copy(values, offs, vals, 0, endOffs + 1);
					MemContentsSub.ContentsInterface page = pages[pageEnd];
					if (!page.matches(vals, startOffs, mask))
					{
						int[] oldValues = page.get(0, endOffs + 1);
						page.load(0, vals, mask);
						if (page.Clear)
						{
							pages[pageEnd] = null;
						}
						fireBytesChanged((long) pageEnd << PAGE_SIZE_BITS, endOffs + 1, oldValues);
					}
				}
			}
		}

		public virtual void fill(long start, long len, int value)
		{
			if (len == 0)
			{
				return;
			}

			int pageStart = (int)(start >>> PAGE_SIZE_BITS);
			int startOffs = (int)(start & PAGE_MASK);
			int pageEnd = (int)((start + len - 1) >>> PAGE_SIZE_BITS);
			int endOffs = (int)((start + len - 1) & PAGE_MASK);
			value &= mask;

			if (pageStart == pageEnd)
			{
				ensurePage(pageStart);
				int[] vals = new int[(int) len];
				Arrays.Fill(vals, value);
				MemContentsSub.ContentsInterface page = pages[pageStart];
				if (!page.matches(vals, startOffs, mask))
				{
					int[] oldValues = page.get(startOffs, (int) len);
					page.load(startOffs, vals, mask);
					if (value == 0 && page.Clear)
					{
						pages[pageStart] = null;
					}
					fireBytesChanged(start, len, oldValues);
				}
			}
			else
			{
				if (startOffs == 0)
				{
					pageStart--;
				}
				else
				{
					if (value == 0 && pages[pageStart] == null)
					{
						// nothing to do
					}
					else
					{
						ensurePage(pageStart);
						int[] vals = new int[PAGE_SIZE - startOffs];
						Arrays.Fill(vals, value);
						MemContentsSub.ContentsInterface page = pages[pageStart];
						if (!page.matches(vals, startOffs, mask))
						{
							int[] oldValues = page.get(startOffs, vals.Length);
							page.load(startOffs, vals, mask);
							if (value == 0 && page.Clear)
							{
								pages[pageStart] = null;
							}
							fireBytesChanged(start, PAGE_SIZE - pageStart, oldValues);
						}
					}
				}
				if (value == 0)
				{
					for (int i = pageStart + 1; i < pageEnd; i++)
					{
						if (pages[i] != null)
						{
							clearPage(i);
						}
					}
				}
				else
				{
					int[] vals = new int[PAGE_SIZE];
					Arrays.Fill(vals, value);
					for (int i = pageStart + 1; i < pageEnd; i++)
					{
						ensurePage(i);
						MemContentsSub.ContentsInterface page = pages[i];
						if (!page.matches(vals, 0, mask))
						{
							int[] oldValues = page.get(0, PAGE_SIZE);
							page.load(0, vals, mask);
							fireBytesChanged((long) i << PAGE_SIZE_BITS, PAGE_SIZE, oldValues);
						}
					}
				}
				if (endOffs >= 0)
				{
					MemContentsSub.ContentsInterface page = pages[pageEnd];
					if (value == 0 && page == null)
					{
						// nothing to do
					}
					else
					{
						ensurePage(pageEnd);
						int[] vals = new int[endOffs + 1];
						Arrays.Fill(vals, value);
						if (!page.matches(vals, 0, mask))
						{
							int[] oldValues = page.get(0, endOffs + 1);
							page.load(0, vals, mask);
							if (value == 0 && page.Clear)
							{
								pages[pageEnd] = null;
							}
							fireBytesChanged((long) pageEnd << PAGE_SIZE_BITS, endOffs + 1, oldValues);
						}
					}
				}
			}
		}

		public virtual void clear()
		{
			for (int i = 0; i < pages.Length; i++)
			{
				if (pages[i] != null)
				{
					if (pages[i] != null)
					{
						clearPage(i);
					}
				}
			}
		}

		private void clearPage(int index)
		{
			MemContentsSub.ContentsInterface page = pages[index];
			int[] oldValues = new int[page.Length];
			bool changed = false;
			for (int j = 0; j < oldValues.Length; j++)
			{
				int val = page.get(j) & mask;
				oldValues[j] = val;
				if (val != 0)
				{
					changed = true;
				}
			}
			if (changed)
			{
				pages[index] = null;
				fireBytesChanged(index << PAGE_SIZE_BITS, oldValues.Length, oldValues);
			}
		}

		public virtual void setSizes(int addrBits, int width)
		{
			if (addrBits == this.addrBits && width == this.width)
			{
				return;
			}
			this.addrBits = addrBits;
			this.width = width;
			this.mask = width == 32 ? unchecked((int)0xffffffff) : ((1 << width) - 1);

			MemContentsSub.ContentsInterface[] oldPages = pages;
			int pageCount;
			int pageLength;
			if (addrBits < PAGE_SIZE_BITS)
			{
				pageCount = 1;
				pageLength = 1 << addrBits;
			}
			else
			{
				pageCount = 1 << (addrBits - PAGE_SIZE_BITS);
				pageLength = PAGE_SIZE;
			}
			pages = new MemContentsSub.ContentsInterface[pageCount];
			if (oldPages != null)
			{
				int n = Math.Min(oldPages.Length, pages.Length);
				for (int i = 0; i < n; i++)
				{
					if (oldPages[i] != null)
					{
						pages[i] = MemContentsSub.createContents(pageLength, width);
						int m = Math.Max(oldPages[i].Length, pageLength);
						for (int j = 0; j < m; j++)
						{
							pages[i].set(j, oldPages[i].get(j));
						}
					}
				}
			}
			if (pageCount == 0 && pages[0] == null)
			{
				pages[0] = MemContentsSub.createContents(pageLength, width);
			}
			fireMetainfoChanged();
		}

		public virtual long FirstOffset
		{
			get
			{
				return 0;
			}
		}

		public virtual long LastOffset
		{
			get
			{
				return (1L << addrBits) - 1;
			}
		}

		public virtual int ValueWidth
		{
			get
			{
				return width;
			}
		}

		private void ensurePage(int index)
		{
			if (pages[index] == null)
			{
				pages[index] = MemContentsSub.createContents(PAGE_SIZE, width);
			}
		}
	}

}
