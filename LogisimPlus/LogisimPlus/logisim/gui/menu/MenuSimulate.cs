// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using CircuitState = logisim.circuit.CircuitState;
	using Simulator = logisim.circuit.Simulator;
	using SimulatorEvent = logisim.circuit.SimulatorEvent;
	using SimulatorListener = logisim.circuit.SimulatorListener;
	using LogFrame = logisim.gui.log.LogFrame;
	using Project = logisim.proj.Project;
	using StringUtil = logisim.util.StringUtil;

	internal class MenuSimulate : Menu
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
			tickFreqs = new TickFrequencyChoice[]
			{
				new TickFrequencyChoice(this, 4096),
				new TickFrequencyChoice(this, 2048),
				new TickFrequencyChoice(this, 1024),
				new TickFrequencyChoice(this, 512),
				new TickFrequencyChoice(this, 256),
				new TickFrequencyChoice(this, 128),
				new TickFrequencyChoice(this, 64),
				new TickFrequencyChoice(this, 32),
				new TickFrequencyChoice(this, 16),
				new TickFrequencyChoice(this, 8),
				new TickFrequencyChoice(this, 4),
				new TickFrequencyChoice(this, 2),
				new TickFrequencyChoice(this, 1),
				new TickFrequencyChoice(this, 0.5),
				new TickFrequencyChoice(this, 0.25)
			};
		}

		private class TickFrequencyChoice : JRadioButtonMenuItem, ActionListener
		{
			private readonly MenuSimulate outerInstance;

			internal double freq;

			public TickFrequencyChoice(MenuSimulate outerInstance, double value)
			{
				this.outerInstance = outerInstance;
				freq = value;
				addActionListener(this);
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				if (outerInstance.currentSim != null)
				{
					outerInstance.currentSim.TickFrequency = freq;
				}
			}

			public virtual void localeChanged()
			{
				double f = freq;
				if (f < 1000)
				{
					string hzStr;
					if (Math.Abs(f - (long)Math.Round(f, MidpointRounding.AwayFromZero)) < 0.0001)
					{
						hzStr = "" + (int) (long)Math.Round(f, MidpointRounding.AwayFromZero);
					}
					else
					{
						hzStr = "" + f;
					}
					setText(StringUtil.format(Strings.get("simulateTickFreqItem"), hzStr));
				}
				else
				{
					string kHzStr;
					double kf = (long)Math.Round(f / 100, MidpointRounding.AwayFromZero) / 10.0;
					if (kf == (long)Math.Round(kf, MidpointRounding.AwayFromZero))
					{
						kHzStr = "" + (int) kf;
					}
					else
					{
						kHzStr = "" + kf;
					}
					setText(StringUtil.format(Strings.get("simulateTickKFreqItem"), kHzStr));
				}
			}
		}

		private class CircuitStateMenuItem : JMenuItem, CircuitListener, ActionListener
		{
			private readonly MenuSimulate outerInstance;

			internal CircuitState circuitState;

			public CircuitStateMenuItem(MenuSimulate outerInstance, CircuitState circuitState)
			{
				this.outerInstance = outerInstance;
				this.circuitState = circuitState;

				Circuit circuit = circuitState.Circuit;
				circuit.addCircuitListener(this);
				this.setText(circuit.Name);
				addActionListener(this);
			}

			internal virtual void unregister()
			{
				Circuit circuit = circuitState.Circuit;
				circuit.removeCircuitListener(this);
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				if (@event.Action == CircuitEvent.ACTION_SET_NAME)
				{
					this.setText(circuitState.Circuit.Name);
				}
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				outerInstance.menubar.fireStateChanged(outerInstance.currentSim, circuitState);
			}
		}

		private class MyListener : ActionListener, SimulatorListener, ChangeListener
		{
			private readonly MenuSimulate outerInstance;

			public MyListener(MenuSimulate outerInstance)
			{
				this.outerInstance = outerInstance;
			}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") public void actionPerformed(java.awt.event.ActionEvent e)
			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				Project proj = outerInstance.menubar.Project;
				Simulator sim = proj == null ? null : proj.Simulator;
				if (src == outerInstance.run || src == LogisimMenuBar.SIMULATE_ENABLE)
				{
					if (sim != null)
					{
						sim.IsRunning = !sim.Running;
						proj.repaintCanvas();
					}
				}
				else if (src == outerInstance.reset)
				{
					if (sim != null)
					{
						sim.requestReset();
					}
				}
				else if (src == outerInstance.step || src == LogisimMenuBar.SIMULATE_STEP)
				{
					if (sim != null)
					{
						sim.step();
					}
				}
				else if (src == outerInstance.tickOnce || src == LogisimMenuBar.TICK_STEP)
				{
					if (sim != null)
					{
						sim.tick();
					}
				}
				else if (src == outerInstance.ticksEnabled || src == LogisimMenuBar.TICK_ENABLE)
				{
					if (sim != null)
					{
						sim.IsTicking = !sim.Ticking;
					}
				}
				else if (src == outerInstance.log)
				{
					LogFrame frame = outerInstance.menubar.Project.getLogFrame(true);
					frame.Visible = true;
				}
			}

			public virtual void propagationCompleted(SimulatorEvent e)
			{
			}

			public virtual void tickCompleted(SimulatorEvent e)
			{
			}

			public virtual void simulatorStateChanged(SimulatorEvent e)
			{
				Simulator sim = e.Source;
				if (sim != outerInstance.currentSim)
				{
					return;
				}
				outerInstance.computeEnabled();
				outerInstance.run.setSelected(sim.Running);
				outerInstance.ticksEnabled.setSelected(sim.Ticking);
				double freq = sim.TickFrequency;
				for (int i = 0; i < outerInstance.tickFreqs.Length; i++)
				{
					TickFrequencyChoice item = outerInstance.tickFreqs[i];
					item.setSelected(freq == item.freq);
				}
			}

			public virtual void stateChanged(ChangeEvent e)
			{
				outerInstance.step.Enabled = outerInstance.run.Enabled && !outerInstance.run.isSelected();
			}
		}

		private LogisimMenuBar menubar;
		private MyListener myListener;
		private CircuitState currentState = null;
		private CircuitState bottomState = null;
		private Simulator currentSim = null;

		private MenuItemCheckImpl run;
		private JMenuItem reset = new JMenuItem();
		private MenuItemImpl step;
		private MenuItemCheckImpl ticksEnabled;
		private MenuItemImpl tickOnce;
		private JMenu tickFreq = new JMenu();
		private TickFrequencyChoice[] tickFreqs;
		private JMenu downStateMenu = new JMenu();
		private List<CircuitStateMenuItem> downStateItems = new List<CircuitStateMenuItem>();
		private JMenu upStateMenu = new JMenu();
		private List<CircuitStateMenuItem> upStateItems = new List<CircuitStateMenuItem>();
		private JMenuItem log = new JMenuItem();

		public MenuSimulate(LogisimMenuBar menubar)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.menubar = menubar;

			run = new MenuItemCheckImpl(this, LogisimMenuBar.SIMULATE_ENABLE);
			step = new MenuItemImpl(this, LogisimMenuBar.SIMULATE_STEP);
			ticksEnabled = new MenuItemCheckImpl(this, LogisimMenuBar.TICK_ENABLE);
			tickOnce = new MenuItemImpl(this, LogisimMenuBar.TICK_STEP);

			menubar.registerItem(LogisimMenuBar.SIMULATE_ENABLE, run);
			menubar.registerItem(LogisimMenuBar.SIMULATE_STEP, step);
			menubar.registerItem(LogisimMenuBar.TICK_ENABLE, ticksEnabled);
			menubar.registerItem(LogisimMenuBar.TICK_STEP, tickOnce);

			int menuMask = getToolkit().getMenuShortcutKeyMaskEx();
			run.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_E, menuMask));
			reset.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_R, menuMask));
			step.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_I, menuMask));
			tickOnce.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_T, menuMask));
			ticksEnabled.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_K, menuMask));

			ButtonGroup bgroup = new ButtonGroup();
			for (int i = 0; i < tickFreqs.Length; i++)
			{
				bgroup.add(tickFreqs[i]);
				tickFreq.add(tickFreqs[i]);
			}

			add(run);
			add(reset);
			add(step);
			addSeparator();
			add(upStateMenu);
			add(downStateMenu);
			addSeparator();
			add(tickOnce);
			add(ticksEnabled);
			add(tickFreq);
			addSeparator();
			add(log);

			setEnabled(false);
			run.Enabled = false;
			reset.setEnabled(false);
			step.Enabled = false;
			upStateMenu.setEnabled(false);
			downStateMenu.setEnabled(false);
			tickOnce.Enabled = false;
			ticksEnabled.Enabled = false;
			tickFreq.setEnabled(false);

			run.addChangeListener(myListener);
			menubar.addActionListener(LogisimMenuBar.SIMULATE_ENABLE, myListener);
			menubar.addActionListener(LogisimMenuBar.SIMULATE_STEP, myListener);
			menubar.addActionListener(LogisimMenuBar.TICK_ENABLE, myListener);
			menubar.addActionListener(LogisimMenuBar.TICK_STEP, myListener);
			// run.addActionListener(myListener);
			reset.addActionListener(myListener);
			// step.addActionListener(myListener);
			// tickOnce.addActionListener(myListener);
			// ticksEnabled.addActionListener(myListener);
			log.addActionListener(myListener);

			computeEnabled();
		}

		public virtual void localeChanged()
		{
			this.setText(Strings.get("simulateMenu"));
			run.setText(Strings.get("simulateRunItem"));
			reset.setText(Strings.get("simulateResetItem"));
			step.setText(Strings.get("simulateStepItem"));
			tickOnce.setText(Strings.get("simulateTickOnceItem"));
			ticksEnabled.setText(Strings.get("simulateTickItem"));
			tickFreq.setText(Strings.get("simulateTickFreqMenu"));
			for (int i = 0; i < tickFreqs.Length; i++)
			{
				tickFreqs[i].localeChanged();
			}
			downStateMenu.setText(Strings.get("simulateDownStateMenu"));
			upStateMenu.setText(Strings.get("simulateUpStateMenu"));
			log.setText(Strings.get("simulateLogItem"));
		}

		public virtual void setCurrentState(Simulator sim, CircuitState value)
		{
			if (currentState == value)
			{
				return;
			}
			Simulator oldSim = currentSim;
			CircuitState oldState = currentState;
			currentSim = sim;
			currentState = value;
			if (bottomState == null)
			{
				bottomState = currentState;
			}
			else if (currentState == null)
			{
				bottomState = null;
			}
			else
			{
				CircuitState cur = bottomState;
				while (cur != null && cur != currentState)
				{
					cur = cur.ParentState;
				}
				if (cur == null)
				{
					bottomState = currentState;
				}
			}

			bool oldPresent = oldState != null;
			bool present = currentState != null;
			if (oldPresent != present)
			{
				computeEnabled();
			}

			if (currentSim != oldSim)
			{
				double freq = currentSim == null ? 1.0 : currentSim.TickFrequency;
				for (int i = 0; i < tickFreqs.Length; i++)
				{
					tickFreqs[i].setSelected(Math.Abs(tickFreqs[i].freq - freq) < 0.001);
				}

				if (oldSim != null)
				{
					oldSim.removeSimulatorListener(myListener);
				}
				if (currentSim != null)
				{
					currentSim.addSimulatorListener(myListener);
				}
				myListener.simulatorStateChanged(new SimulatorEvent(sim));
			}

			clearItems(downStateItems);
			CircuitState cur = bottomState;
			while (cur != null && cur != currentState)
			{
				downStateItems.Add(new CircuitStateMenuItem(this, cur));
				cur = cur.ParentState;
			}
			if (cur != null)
			{
				cur = cur.ParentState;
			}
			clearItems(upStateItems);
			while (cur != null)
			{
				upStateItems.Insert(0, new CircuitStateMenuItem(this, cur));
				cur = cur.ParentState;
			}
			recreateStateMenus();
		}

		private void clearItems(List<CircuitStateMenuItem> items)
		{
			foreach (CircuitStateMenuItem item in items)
			{
				item.unregister();
			}
			items.Clear();
		}

		private void recreateStateMenus()
		{
			recreateStateMenu(downStateMenu, downStateItems, KeyEvent.VK_RIGHT);
			recreateStateMenu(upStateMenu, upStateItems, KeyEvent.VK_LEFT);
		}

		private void recreateStateMenu(JMenu menu, List<CircuitStateMenuItem> items, int code)
		{
			menu.removeAll();
			menu.setEnabled(items.Count > 0);
			bool first = true;
			int mask = getToolkit().getMenuShortcutKeyMaskEx();
			for (int i = items.Count - 1; i >= 0; i--)
			{
				JMenuItem item = items[i];
				menu.add(item);
				if (first)
				{
					item.setAccelerator(KeyStroke.getKeyStroke(code, mask));
					first = false;
				}
				else
				{
					item.setAccelerator(null);
				}
			}
		}

		internal override void computeEnabled()
		{
			bool present = currentState != null;
			Simulator sim = this.currentSim;
			bool simRunning = sim != null && sim.Running;
			setEnabled(present);
			run.Enabled = present;
			reset.setEnabled(present);
			step.Enabled = present && !simRunning;
			upStateMenu.setEnabled(present);
			downStateMenu.setEnabled(present);
			tickOnce.Enabled = present;
			ticksEnabled.Enabled = present && simRunning;
			tickFreq.setEnabled(present);
			menubar.fireEnableChanged();
		}
	}

}
