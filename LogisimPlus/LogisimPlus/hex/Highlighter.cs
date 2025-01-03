// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace hex
{

	internal class Highlighter
	{
		private class Entry
		{
			internal long start;
			internal long end;
			internal Color color;

			internal Entry(long start, long end, Color color)
			{
				this.start = start;
				this.end = end;
				this.color = color;
			}
		}

		private HexEditor hex;
		private List<Entry> entries;

		internal Highlighter(HexEditor hex)
		{
			this.hex = hex;
			this.entries = new List<Entry>();
		}

		public virtual object add(long start, long end, Color color)
		{
			lock (this)
			{
				HexModel model = hex.Model;
				if (model == null)
				{
					return null;
				}
				if (start > end)
				{
					long t = start;
					start = end;
					end = t;
				}
				if (start < model.FirstOffset)
				{
					start = model.FirstOffset;
				}
				if (end > model.LastOffset)
				{
					end = model.LastOffset;
				}
				if (start >= end)
				{
					return null;
				}
        
				Entry entry = new Entry(start, end, color);
				entries.Add(entry);
				expose(entry);
				return entry;
			}
		}

		public virtual void remove(object tag)
		{
			lock (this)
			{
				if (entries.Remove(tag))
				{
					Entry entry = (Entry) tag;
					expose(entry);
				}
			}
		}

		public virtual void clear()
		{
			lock (this)
			{
				List<Entry> oldEntries = entries;
				entries = new List<Entry>();
				for (int n = oldEntries.Count; n >= 0; n--)
				{
					expose(oldEntries[n]);
				}
			}
		}

		private void expose(Entry entry)
		{
			Measures m = hex.Measures;
			int y0 = m.toY(entry.start);
			int y1 = m.toY(entry.end);
			int h = m.CellHeight;
			int cellWidth = m.CellWidth;
			if (y0 == y1)
			{
				int x0 = m.toX(entry.start);
				int x1 = m.toX(entry.end) + cellWidth;
				hex.repaint(x0, y0, x1 - x0, h);
			}
			else
			{
				int lineStart = m.ValuesX;
				int lineWidth = m.ValuesWidth;
				hex.repaint(lineStart, y0, lineWidth, y1 - y0 + h);
			}
		}

		internal virtual void paint(JGraphics g, long start, long end)
		{
			lock (this)
			{
				int size = entries.Count;
				if (size == 0)
				{
					return;
				}
				Measures m = hex.Measures;
				int lineStart = m.ValuesX;
				int lineWidth = m.ValuesWidth;
				int cellWidth = m.CellWidth;
				int cellHeight = m.CellHeight;
				foreach (Entry e in entries)
				{
					if (e.start <= end && e.end >= start)
					{
						int y0 = m.toY(e.start);
						int y1 = m.toY(e.end);
						int x0 = m.toX(e.start);
						int x1 = m.toX(e.end);
						g.setColor(e.color);
						if (y0 == y1)
						{
							g.fillRect(x0, y0, x1 - x0 + cellWidth, cellHeight);
						}
						else
						{
							int midHeight = y1 - (y0 + cellHeight);
							g.fillRect(x0, y0, lineStart + lineWidth - x0, cellHeight);
							if (midHeight > 0)
							{
								g.fillRect(lineStart, y0 + cellHeight, lineWidth, midHeight);
							}
							g.fillRect(lineStart, y1, x1 + cellWidth - lineStart, cellHeight);
						}
					}
				}
			}
		}
	}

}
