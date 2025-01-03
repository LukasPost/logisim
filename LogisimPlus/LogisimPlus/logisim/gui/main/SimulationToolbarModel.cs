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
	using Simulator = logisim.circuit.Simulator;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using Project = logisim.proj.Project;
	using logisim.util;

	internal class SimulationToolbarModel : AbstractToolbarModel, ChangeListener
	{
		private Project project;
		private LogisimToolbarItem simEnable;
		private LogisimToolbarItem simStep;
		private LogisimToolbarItem tickEnable;
		private LogisimToolbarItem tickStep;
		private List<ToolbarItem> items;

		public SimulationToolbarModel(Project project, MenuListener menu)
		{
			this.project = project;

			simEnable = new LogisimToolbarItem(menu, "simplay.png", LogisimMenuBar.SIMULATE_ENABLE, Strings.getter("simulateEnableStepsTip"));
			simStep = new LogisimToolbarItem(menu, "simstep.png", LogisimMenuBar.SIMULATE_STEP, Strings.getter("simulateStepTip"));
			tickEnable = new LogisimToolbarItem(menu, "simtplay.png", LogisimMenuBar.TICK_ENABLE, Strings.getter("simulateEnableTicksTip"));
			tickStep = new LogisimToolbarItem(menu, "simtstep.png", LogisimMenuBar.TICK_STEP, Strings.getter("simulateTickTip"));

			items = UnmodifiableList.create(new ToolbarItem[] {simEnable, simStep, tickEnable, tickStep});

			menu.MenuBar.addEnableListener(this);
			stateChanged(null);
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
		// ChangeListener methods
		//
		public virtual void stateChanged(ChangeEvent e)
		{
			Simulator sim = project.Simulator;
			bool running = sim != null && sim.Running;
			bool ticking = sim != null && sim.Ticking;
			simEnable.Icon = running ? "simstop.png" : "simplay.png";
			simEnable.ToolTip = running ? Strings.getter("simulateDisableStepsTip") : Strings.getter("simulateEnableStepsTip");
			tickEnable.Icon = ticking ? "simtstop.png" : "simtplay.png";
			tickEnable.ToolTip = ticking ? Strings.getter("simulateDisableTicksTip") : Strings.getter("simulateEnableTicksTip");
			fireToolbarAppearanceChanged();
		}
	}

}
