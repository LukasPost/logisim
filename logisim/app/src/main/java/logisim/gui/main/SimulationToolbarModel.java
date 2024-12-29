/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.util.List;

import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;

import draw.toolbar.AbstractToolbarModel;
import draw.toolbar.ToolbarItem;
import logisim.circuit.Simulator;
import logisim.gui.menu.LogisimMenuBar;
import logisim.proj.Project;
import logisim.util.UnmodifiableList;

class SimulationToolbarModel extends AbstractToolbarModel implements ChangeListener {
	private Project project;
	private LogisimToolbarItem simEnable;
	private LogisimToolbarItem simStep;
	private LogisimToolbarItem tickEnable;
	private LogisimToolbarItem tickStep;
	private List<ToolbarItem> items;

	public SimulationToolbarModel(Project project, MenuListener menu) {
		this.project = project;

		simEnable = new LogisimToolbarItem(menu, "simplay.png", LogisimMenuBar.SIMULATE_ENABLE,
				Strings.getter("simulateEnableStepsTip"));
		simStep = new LogisimToolbarItem(menu, "simstep.png", LogisimMenuBar.SIMULATE_STEP,
				Strings.getter("simulateStepTip"));
		tickEnable = new LogisimToolbarItem(menu, "simtplay.png", LogisimMenuBar.TICK_ENABLE,
				Strings.getter("simulateEnableTicksTip"));
		tickStep = new LogisimToolbarItem(menu, "simtstep.png", LogisimMenuBar.TICK_STEP,
				Strings.getter("simulateTickTip"));

		items = UnmodifiableList.create(new ToolbarItem[] { simEnable, simStep, tickEnable, tickStep, });

		menu.getMenuBar().addEnableListener(this);
		stateChanged(null);
	}

	@Override
	public List<ToolbarItem> getItems() {
		return items;
	}

	@Override
	public boolean isSelected(ToolbarItem item) {
		return false;
	}

	@Override
	public void itemSelected(ToolbarItem item) {
		if (item instanceof LogisimToolbarItem) {
			((LogisimToolbarItem) item).doAction();
		}
	}

	//
	// ChangeListener methods
	//
	public void stateChanged(ChangeEvent e) {
		Simulator sim = project.getSimulator();
		boolean running = sim != null && sim.isRunning();
		boolean ticking = sim != null && sim.isTicking();
		simEnable.setIcon(running ? "simstop.png" : "simplay.png");
		simEnable.setToolTip(
				running ? Strings.getter("simulateDisableStepsTip") : Strings.getter("simulateEnableStepsTip"));
		tickEnable.setIcon(ticking ? "simtstop.png" : "simtplay.png");
		tickEnable.setToolTip(
				ticking ? Strings.getter("simulateDisableTicksTip") : Strings.getter("simulateEnableTicksTip"));
		fireToolbarAppearanceChanged();
	}
}
