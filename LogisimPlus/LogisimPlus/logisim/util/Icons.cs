// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{


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

		public static void paintRotated(Graphics g, int x, int y, Direction dir, Icon icon, Component dest)
		{
			if (!(g is Graphics2D) || dir == Direction.East)
			{
				icon.paintIcon(dest, g, x, y);
				return;
			}

			Graphics2D g2 = (Graphics2D) g.create();
			double cx = x + icon.getIconWidth() / 2.0;
			double cy = y + icon.getIconHeight() / 2.0;
			if (dir == Direction.West)
			{
				g2.rotate(Math.PI, cx, cy);
			}
			else if (dir == Direction.North)
			{
				g2.rotate(-Math.PI / 2.0, cx, cy);
			}
			else if (dir == Direction.South)
			{
				g2.rotate(Math.PI / 2.0, cx, cy);
			}
			else
			{
				g2.translate(-x, -y);
			}
			icon.paintIcon(dest, g2, x, y);
			g2.dispose();
		}
	}

}
