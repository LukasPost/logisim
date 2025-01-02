// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{

	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;

	internal class ComponentIcon : Icon
	{
		public const int TRIANGLE_NONE = 0;
		public const int TRIANGLE_CLOSED = 1;
		public const int TRIANGLE_OPEN = 2;

		private Component comp;
		private int triangleState = TRIANGLE_NONE;

		internal ComponentIcon(Component comp)
		{
			this.comp = comp;
		}

		public virtual int TriangleState
		{
			set
			{
				triangleState = value;
			}
		}

		public virtual int IconHeight
		{
			get
			{
				return 20;
			}
		}

		public virtual int IconWidth
		{
			get
			{
				return 20;
			}
		}

		public virtual void paintIcon(java.awt.Component c, Graphics g, int x, int y)
		{
			// draw tool icon
			Graphics gIcon = g.create();
			ComponentDrawContext context = new ComponentDrawContext(c, null, null, g, gIcon);
			comp.Factory.paintIcon(context, x, y, comp.AttributeSet);
			gIcon.dispose();

			if (triangleState != TRIANGLE_NONE)
			{
				int[] xp;
				int[] yp;
				if (triangleState == TRIANGLE_CLOSED)
				{
					xp = new int[] {x + 13, x + 13, x + 17};
					yp = new int[] {y + 11, y + 19, y + 15};
				}
				else
				{
					xp = new int[] {x + 11, x + 19, x + 15};
					yp = new int[] {y + 13, y + 13, y + 17};
				}
				g.setColor(Color.LIGHT_GRAY);
				g.fillPolygon(xp, yp, 3);
				g.setColor(Color.DARK_GRAY);
				g.drawPolygon(xp, yp, 3);
			}
		}
	}

}
