// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.tools
{

	using ToolbarItem = draw.toolbar.ToolbarItem;

	public class ToolbarToolItem : ToolbarItem
	{
		private AbstractTool tool;
		private Icon icon;

		public ToolbarToolItem(AbstractTool tool)
		{
			this.tool = tool;
			this.icon = tool.Icon;
		}

		public virtual AbstractTool Tool
		{
			get
			{
				return tool;
			}
		}

		public virtual bool Selectable
		{
			get
			{
				return true;
			}
		}

		public virtual void paintIcon(Component destination, Graphics g)
		{
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
				icon.paintIcon(destination, g, 4, 4);
			}
		}

		public virtual string ToolTip
		{
			get
			{
				return tool.Description;
			}
		}

		public virtual Dimension getDimension(object orientation)
		{
			if (icon == null)
			{
				return new Dimension(16, 16);
			}
			else
			{
				return new Dimension(icon.getIconWidth() + 8, icon.getIconHeight() + 8);
			}
		}
	}

}
