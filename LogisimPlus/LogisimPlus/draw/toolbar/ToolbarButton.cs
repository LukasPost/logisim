// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.toolbar
{
    using LogisimPlus.draw.gui;
    using System;
    using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class ToolbarButton : Control
	{
		private const int BORDER = 2;

		private Toolbar toolbar;
		private ToolbarItem item;

		internal ToolbarButton(Toolbar toolbar, ToolbarItem item)
		{
			this.toolbar = toolbar;
			this.item = item;
			//CanFocus = true;
			AutoToolTip.AttachToolTip(this, "");
		}

        public virtual ToolbarItem Item => item;

        public override Size MinimumSize
        {
            get
            {
                Size dim = item.getDimension(toolbar.Orientation);
                dim.Width += 2 * BORDER;
                dim.Height += 2 * BORDER;
                return dim;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (toolbar.Pressed == this)
			{
                Size dim = item.getDimension(toolbar.Orientation);
				GraphicsUtil.switchToWidth(g, 2);
				g.FillRectangle(Brushes.Gray, BORDER, BORDER, dim.Width, dim.Height);
				GraphicsUtil.switchToWidth(g, 1);
			}

			Graphics g2 = g.create();
			g2.translate(BORDER, BORDER);
			item.paintIcon(this, g);
			g2.dispose();

			// draw selection indicator
			if (toolbar.ToolbarModel.isSelected(item))
			{
                Size dim = item.getDimension(toolbar.Orientation);
				using Pen p = new Pen(Brushes.Black, 2);
				g.DrawRectangle(p, BORDER, BORDER, dim.Width, dim.Height);
			}
		}

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (item != null && item.Selectable)
                toolbar.Pressed = this;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (toolbar.Pressed == this)
            {
                toolbar.ToolbarModel.itemSelected(item);
                toolbar.Pressed = null;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (toolbar.Pressed == this)
				toolbar.Pressed = null;
        }
	}

}
