// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.toolbar
{
    using LogisimPlus.Java;
    using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class ToolbarButton : JComponent, MouseListener
	{
		private const int BORDER = 2;

		private Toolbar toolbar;
		private ToolbarItem item;

		internal ToolbarButton(Toolbar toolbar, ToolbarItem item)
		{
			this.toolbar = toolbar;
			this.item = item;
			addMouseListener(this);
			setFocusable(true);
			setToolTipText("");
		}

		public virtual ToolbarItem Item
		{
			get
			{
				return item;
			}
		}

		public override Size MinimumSize
        {
			get
			{
				Size dim = item.getSize(toolbar.Orientation);
				dim.Width += 2 * BORDER;
				dim.Height += 2 * BORDER;
				return dim;
			}
		}

		public override void paintComponent(JGraphics g)
		{
			if (toolbar.Pressed == this)
			{
				Size dim = item.getSize(toolbar.Orientation);
				Color defaultColor = g.getColor();
				GraphicsUtil.switchToWidth(g, 2);
				g.setColor(Color.Gray);
				g.fillRect(BORDER, BORDER, dim.Width, dim.Height);
				GraphicsUtil.switchToWidth(g, 1);
				g.setColor(defaultColor);
			}

			JGraphics g2 = g.create();
			g2.translate(BORDER, BORDER);
			item.paintIcon(this, g2);
			//g2.dispose();

			// draw selection indicator
			if (toolbar.ToolbarModel.isSelected(item))
			{
				Size dim = item.getSize(toolbar.Orientation);
				GraphicsUtil.switchToWidth(g, 2);
				g.setColor(Color.Black);
				g.drawRect(BORDER, BORDER, dim.Width, dim.Height);
				GraphicsUtil.switchToWidth(g, 1);
			}
		}

		public override string getToolTipText(MouseEvent e)
		{
			return item.ToolTip;
		}

		public virtual void mousePressed(MouseEvent e)
		{
			if (item != null && item.Selectable)
			{
				toolbar.Pressed = this;
			}
		}

		public virtual void mouseReleased(MouseEvent e)
		{
			if (toolbar.Pressed == this)
			{
				toolbar.ToolbarModel.itemSelected(item);
				toolbar.Pressed = null;
			}
		}

		public virtual void mouseClicked(MouseEvent e)
		{
		}

		public virtual void mouseEntered(MouseEvent e)
		{
		}

		public virtual void mouseExited(MouseEvent e)
		{
			toolbar.Pressed = null;
		}
	}

}
