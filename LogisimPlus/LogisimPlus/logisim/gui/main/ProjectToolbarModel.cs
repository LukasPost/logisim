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
	using ToolbarSeparator = draw.toolbar.ToolbarSeparator;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using logisim.util;

	internal class ProjectToolbarModel : AbstractToolbarModel, MenuListener.EnabledListener
	{
		private Frame frame;
		private LogisimToolbarItem itemAdd;
		private LogisimToolbarItem itemUp;
		private LogisimToolbarItem itemDown;
		private LogisimToolbarItem itemDelete;
		private LogisimToolbarItem itemLayout;
		private LogisimToolbarItem itemAppearance;
		private IList<ToolbarItem> items;

		public ProjectToolbarModel(Frame frame, MenuListener menu)
		{
			this.frame = frame;

			itemAdd = new LogisimToolbarItem(menu, "projadd.gif", LogisimMenuBar.ADD_CIRCUIT, Strings.getter("projectAddCircuitTip"));
			itemUp = new LogisimToolbarItem(menu, "projup.gif", LogisimMenuBar.MOVE_CIRCUIT_UP, Strings.getter("projectMoveCircuitUpTip"));
			itemDown = new LogisimToolbarItem(menu, "projdown.gif", LogisimMenuBar.MOVE_CIRCUIT_DOWN, Strings.getter("projectMoveCircuitDownTip"));
			itemDelete = new LogisimToolbarItem(menu, "projdel.gif", LogisimMenuBar.REMOVE_CIRCUIT, Strings.getter("projectRemoveCircuitTip"));
			itemLayout = new LogisimToolbarItem(menu, "projlayo.gif", LogisimMenuBar.EDIT_LAYOUT, Strings.getter("projectEditLayoutTip"));
			itemAppearance = new LogisimToolbarItem(menu, "projapp.gif", LogisimMenuBar.EDIT_APPEARANCE, Strings.getter("projectEditAppearanceTip"));

			items = UnmodifiableList.create(new ToolbarItem[] {itemAdd, itemUp, itemDown, itemDelete, new ToolbarSeparator(4), itemLayout, itemAppearance});

			menu.addEnabledListener(this);
		}

		public override IList<ToolbarItem> Items
		{
			get
			{
				return items;
			}
		}

		public override bool isSelected(ToolbarItem item)
		{
			string view = frame.EditorView;
			if (item == itemLayout)
			{
				return view.Equals(Frame.EDIT_LAYOUT);
			}
			else if (item == itemAppearance)
			{
				return view.Equals(Frame.EDIT_APPEARANCE);
			}
			else
			{
				return false;
			}
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
