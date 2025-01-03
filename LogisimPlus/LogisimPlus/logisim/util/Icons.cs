// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{
    using LogisimPlus.Java;
    using Direction = logisim.data.Direction;

	public class Icons
	{
		private const string path = "logisim/icons";

		private Icons()
		{
		}

		public static Icon getIcon(string name)
		{
			java.net.URL url = typeof(Icons).getClassLoader().getResource(path + "/" + name);
			if (url == null)
			{
				return null;
			}
			return new ImageIcon(url);
		}

		public static void paintRotated(JGraphics g, int x, int y, Direction dir, Icon icon, JComponent dest)
		{
			if (!(g is JGraphics2D) || dir == Direction.East)
			{
				icon.paintIcon(dest, g, x, y);
				return;
			}

			double cx = x + icon.getIconWidth() / 2.0;
			double cy = y + icon.getIconHeight() / 2.0;
			if (dir == Direction.West)
			{
				g.rotate(Math.PI, cx, cy);
			}
			else if (dir == Direction.North)
			{
				g.rotate(-Math.PI / 2.0, cx, cy);
			}
			else if (dir == Direction.South)
			{
				g.rotate(Math.PI / 2.0, cx, cy);
			}
			else
			{
				g.translate(-x, -y);
			}
			icon.paintIcon(dest, g, x, y);
			g.dispose();
		}
	}

}
