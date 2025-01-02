﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.toolbar
{

	public class ToolbarSeparator : ToolbarItem
	{
		private int size;

		public ToolbarSeparator(int size)
		{
			this.size = size;
		}

		public virtual bool Selectable
		{
			get
			{
				return false;
			}
		}

		public virtual void paintIcon(Component destination, Graphics g)
		{
			Dimension dim = destination.getSize();
			g.setColor(Color.GRAY);
			int x = 0;
			int y = 0;
			int w = dim.width;
			int h = dim.height;
			if (h >= w)
			{ // separator is a vertical line in horizontal toolbar
				h -= 8;
				y = 2;
				x = (w - 2) / 2;
				w = 2;
			}
			else
			{ // separator is a horizontal line in vertical toolbar
				w -= 8;
				x = 2;
				y = (h - 2) / 2;
				h = 2;
			}
			g.fillRect(x, y, w, h);
		}

		public virtual string ToolTip
		{
			get
			{
				return null;
			}
		}

		public virtual Dimension getDimension(object orientation)
		{
			return new Dimension(size, size);
		}
	}

}