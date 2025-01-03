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

	internal class ExplorerToolbarModel : AbstractToolbarModel, MenuListener.EnabledListener
	{
		private Frame frame;
		private LogisimToolbarItem itemToolbox;
		private LogisimToolbarItem itemSimulation;
		private LogisimToolbarItem itemLayout;
		private LogisimToolbarItem itemAppearance;
		private List<ToolbarItem> items;

		public ExplorerToolbarModel(Frame frame, MenuListener menu)
		{
			this.frame = frame;

			itemToolbox = new LogisimToolbarItem(menu, "projtool.gif", LogisimMenuBar.VIEW_TOOLBOX, Strings.getter("projectViewToolboxTip"));
			itemSimulation = new LogisimToolbarItem(menu, "projsim.gif", LogisimMenuBar.VIEW_SIMULATION, Strings.getter("projectViewSimulationTip"));
			itemLayout = new LogisimToolbarItem(menu, "projlayo.gif", LogisimMenuBar.EDIT_LAYOUT, Strings.getter("projectEditLayoutTip"));
			itemAppearance = new LogisimToolbarItem(menu, "projapp.gif", LogisimMenuBar.EDIT_APPEARANCE, Strings.getter("projectEditAppearanceTip"));

			items = UnmodifiableList.create(new ToolbarItem[] {itemToolbox, itemSimulation, new ToolbarSeparator(4), itemLayout, itemAppearance});

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
			if (item == itemLayout)
			{
				return frame.EditorView.Equals(Frame.EDIT_LAYOUT);
			}
			else if (item == itemAppearance)
			{
				return frame.EditorView.Equals(Frame.EDIT_APPEARANCE);
			}
			else if (item == itemToolbox)
			{
				return frame.ExplorerView.Equals(Frame.VIEW_TOOLBOX);
			}
			else if (item == itemSimulation)
			{
				return frame.ExplorerView.Equals(Frame.VIEW_SIMULATION);
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
