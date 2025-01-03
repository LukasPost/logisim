// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

using LogisimPlus.Java;

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

		public virtual void paintIcon(JComponent destination, JGraphics g)
		{
			Size dim = destination.getSize();
			g.setColor(Color.Gray);
			int x = 0;
			int y = 0;
			int w = dim.Width;
			int h = dim.Height;
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

		public virtual Size getSize(object orientation)
		{
			return new Size(size, size);
		}
	}

}
