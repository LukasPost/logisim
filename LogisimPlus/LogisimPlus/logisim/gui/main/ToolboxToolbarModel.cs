// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using AbstractToolbarModel = draw.toolbar.AbstractToolbarModel;
	using ToolbarItem = draw.toolbar.ToolbarItem;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using logisim.util;

	internal class ToolboxToolbarModel : AbstractToolbarModel, MenuListener.EnabledListener
	{
		private LogisimToolbarItem itemAdd;
		private LogisimToolbarItem itemUp;
		private LogisimToolbarItem itemDown;
		private LogisimToolbarItem itemDelete;
		private List<ToolbarItem> items;

		public ToolboxToolbarModel(MenuListener menu)
		{
			itemAdd = new LogisimToolbarItem(menu, "projadd.gif", LogisimMenuBar.ADD_CIRCUIT, Strings.getter("projectAddCircuitTip"));
			itemUp = new LogisimToolbarItem(menu, "projup.gif", LogisimMenuBar.MOVE_CIRCUIT_UP, Strings.getter("projectMoveCircuitUpTip"));
			itemDown = new LogisimToolbarItem(menu, "projdown.gif", LogisimMenuBar.MOVE_CIRCUIT_DOWN, Strings.getter("projectMoveCircuitDownTip"));
			itemDelete = new LogisimToolbarItem(menu, "projdel.gif", LogisimMenuBar.REMOVE_CIRCUIT, Strings.getter("projectRemoveCircuitTip"));

			items = UnmodifiableList.create(new ToolbarItem[] {itemAdd, itemUp, itemDown, itemDelete});

			menu.addEnabledListener(this);
		}

		public override List<ToolbarItem> Items
		{
			get
			{
				return items;
			}
		}

		public override bool isSelected(ToolbarItem item)
		{
			return false;
		}

		public override void itemSelected(ToolbarItem item)
		{
			if (item is LogisimToolbarItem)
			{
				((LogisimToolbarItem) item).doAction();
			}
		}

		//
		// EnabledListener methods
		//
		public virtual void menuEnableChanged(MenuListener source)
		{
			fireToolbarAppearanceChanged();
		}
	}

}
