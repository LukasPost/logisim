// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{

	internal class MenuItemImpl : JMenuItem, MenuItem
	{
		private MenuItemHelper helper;

		public MenuItemImpl(Menu menu, LogisimMenuItem menuItem)
		{
			helper = new MenuItemHelper(this, menu, menuItem);
			base.addActionListener(helper);
			Enabled = true;
		}

		public virtual bool hasListeners()
		{
			return helper.hasListeners();
		}

		public virtual void addActionListener(ActionListener l)
		{
			helper.addActionListener(l);
		}

		public virtual void removeActionListener(ActionListener l)
		{
			helper.removeActionListener(l);
		}

		public virtual bool Enabled
		{
			set
			{
				helper.Enabled = value;
				base.setEnabled(value && helper.hasListeners());
			}
		}

		public virtual void actionPerformed(ActionEvent @event)
		{
			helper.actionPerformed(@event);
		}
	}

}
