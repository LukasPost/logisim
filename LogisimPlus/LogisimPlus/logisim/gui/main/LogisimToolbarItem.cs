// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using ToolbarItem = draw.toolbar.ToolbarItem;
	using LogisimMenuItem = logisim.gui.menu.LogisimMenuItem;
	using Icons = logisim.util.Icons;
	using StringGetter = logisim.util.StringGetter;

	internal class LogisimToolbarItem : ToolbarItem
	{
		private MenuListener menu;
		private Icon icon;
		private LogisimMenuItem action;
		private StringGetter toolTip;

		public LogisimToolbarItem(MenuListener menu, string iconName, LogisimMenuItem action, StringGetter toolTip)
		{
			this.menu = menu;
			this.icon = Icons.getIcon(iconName);
			this.action = action;
			this.toolTip = toolTip;
		}

		public virtual string Icon
		{
			set
			{
				this.icon = Icons.getIcon(value);
			}
		}

		public virtual StringGetter ToolTip
		{
			set
			{
				this.toolTip = value;
			}
			get
			{
				if (toolTip != null)
				{
					return toolTip.get();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void doAction()
		{
			if (menu != null && menu.isEnabled(action))
			{
				menu.doAction(action);
			}
		}

		public virtual bool Selectable
		{
			get
			{
				return menu != null && menu.isEnabled(action);
			}
		}

		public virtual void paintIcon(Component destination, Graphics g)
		{
			if (!Selectable && g is Graphics2D)
			{
				Composite c = AlphaComposite.getInstance(AlphaComposite.SRC_OVER, 0.3f);
				((Graphics2D) g).setComposite(c);
			}

			if (icon == null)
			{
				g.setColor(new Color(255, 128, 128));
				g.fillRect(4, 4, 8, 8);
				g.setColor(Color.BLACK);
				g.drawLine(4, 4, 12, 12);
				g.drawLine(4, 12, 12, 4);
				g.drawRect(4, 4, 8, 8);
			}
			else
			{
				icon.paintIcon(destination, g, 0, 1);
			}
		}


		public virtual Size getSize(object orientation)
		{
			if (icon == null)
			{
				return new Size(16, 16);
			}
			else
			{
				int w = icon.getIconWidth();
				int h = icon.getIconHeight();
				return new Size(w, h + 2);
			}
		}
	}

}
