// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using CircuitState = logisim.circuit.CircuitState;
	using Simulator = logisim.circuit.Simulator;
	using Project = logisim.proj.Project;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using WindowMenu = logisim.util.WindowMenu;

	public class LogisimMenuBar : JMenuBar
	{
		public static readonly LogisimMenuItem PRINT = new LogisimMenuItem("Print");
		public static readonly LogisimMenuItem EXPORT_IMAGE = new LogisimMenuItem("ExportImage");
		public static readonly LogisimMenuItem CUT = new LogisimMenuItem("Cut");
		public static readonly LogisimMenuItem COPY = new LogisimMenuItem("Copy");
		public static readonly LogisimMenuItem PASTE = new LogisimMenuItem("Paste");
		public static readonly LogisimMenuItem DELETE = new LogisimMenuItem("Delete");
		public static readonly LogisimMenuItem DUPLICATE = new LogisimMenuItem("Duplicate");
		public static readonly LogisimMenuItem SELECT_ALL = new LogisimMenuItem("SelectAll");
		public static readonly LogisimMenuItem RAISE = new LogisimMenuItem("Raise");
		public static readonly LogisimMenuItem LOWER = new LogisimMenuItem("Lower");
		public static readonly LogisimMenuItem RAISE_TOP = new LogisimMenuItem("RaiseTop");
		public static readonly LogisimMenuItem LOWER_BOTTOM = new LogisimMenuItem("LowerBottom");
		public static readonly LogisimMenuItem ADD_CONTROL = new LogisimMenuItem("AddControl");
		public static readonly LogisimMenuItem REMOVE_CONTROL = new LogisimMenuItem("RemoveControl");

		public static readonly LogisimMenuItem ADD_CIRCUIT = new LogisimMenuItem("AddCircuit");
		public static readonly LogisimMenuItem MOVE_CIRCUIT_UP = new LogisimMenuItem("MoveCircuitUp");
		public static readonly LogisimMenuItem MOVE_CIRCUIT_DOWN = new LogisimMenuItem("MoveCircuitDown");
		public static readonly LogisimMenuItem SET_MAIN_CIRCUIT = new LogisimMenuItem("SetMainCircuit");
		public static readonly LogisimMenuItem REMOVE_CIRCUIT = new LogisimMenuItem("RemoveCircuit");
		public static readonly LogisimMenuItem EDIT_LAYOUT = new LogisimMenuItem("EditLayout");
		public static readonly LogisimMenuItem EDIT_APPEARANCE = new LogisimMenuItem("EditAppearance");
		public static readonly LogisimMenuItem VIEW_TOOLBOX = new LogisimMenuItem("ViewToolbox");
		public static readonly LogisimMenuItem VIEW_SIMULATION = new LogisimMenuItem("ViewSimulation");
		public static readonly LogisimMenuItem REVERT_APPEARANCE = new LogisimMenuItem("RevertAppearance");
		public static readonly LogisimMenuItem ANALYZE_CIRCUIT = new LogisimMenuItem("AnalyzeCircuit");
		public static readonly LogisimMenuItem CIRCUIT_STATS = new LogisimMenuItem("GetCircuitStatistics");

		public static readonly LogisimMenuItem SIMULATE_ENABLE = new LogisimMenuItem("SimulateEnable");
		public static readonly LogisimMenuItem SIMULATE_STEP = new LogisimMenuItem("SimulateStep");
		public static readonly LogisimMenuItem TICK_ENABLE = new LogisimMenuItem("TickEnable");
		public static readonly LogisimMenuItem TICK_STEP = new LogisimMenuItem("TickStep");

		private class MyListener : LocaleListener
		{
			private readonly LogisimMenuBar outerInstance;

			public MyListener(LogisimMenuBar outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void localeChanged()
			{
				outerInstance.file.localeChanged();
				outerInstance.edit.localeChanged();
				outerInstance.project.localeChanged();
				outerInstance.simulate.localeChanged();
				outerInstance.help.localeChanged();
			}
		}

		private JFrame parent;
		private MyListener listener;
		private Project proj;
		private SimulateListener simulateListener = null;
		private Dictionary<LogisimMenuItem, MenuItem> menuItems = new Dictionary<LogisimMenuItem, MenuItem>();
		private List<ChangeListener> enableListeners;

		private MenuFile file;
		private MenuEdit edit;
		private MenuProject project;
		private MenuSimulate simulate;
		private MenuHelp help;

		public LogisimMenuBar(JFrame parent, Project proj)
		{
			this.parent = parent;
			this.listener = new MyListener(this);
			this.proj = proj;
			this.enableListeners = new List<ChangeListener>();

			add(file = new MenuFile(this));
			add(edit = new MenuEdit(this));
			add(project = new MenuProject(this));
			add(simulate = new MenuSimulate(this));
			add(new WindowMenu(parent));
			add(help = new MenuHelp(this));

			LocaleManager.addLocaleListener(listener);
			listener.localeChanged();
		}

		public virtual void setEnabled(LogisimMenuItem which, bool value)
		{
			MenuItem item = menuItems[which];
			if (item != null)
			{
				item.Enabled = value;
			}
		}

		public virtual void addActionListener(LogisimMenuItem which, ActionListener l)
		{
			MenuItem item = menuItems[which];
			if (item != null)
			{
				item.addActionListener(l);
			}
		}

		public virtual void removeActionListener(LogisimMenuItem which, ActionListener l)
		{
			MenuItem item = menuItems[which];
			if (item != null)
			{
				item.removeActionListener(l);
			}
		}

		public virtual void addEnableListener(ChangeListener l)
		{
			enableListeners.Add(l);
		}

		public virtual void removeEnableListener(ChangeListener l)
		{
			enableListeners.Remove(l);
		}

		internal virtual void fireEnableChanged()
		{
			ChangeEvent e = new ChangeEvent(this);
			foreach (ChangeListener listener in enableListeners)
			{
				listener.stateChanged(e);
			}
		}

		public virtual SimulateListener SimulateListener
		{
			set
			{
				simulateListener = value;
			}
		}

		public virtual void setCircuitState(Simulator sim, CircuitState state)
		{
			simulate.setCurrentState(sim, state);
		}

		public virtual Project Project
		{
			get
			{
				return proj;
			}
		}

		internal virtual JFrame ParentWindow
		{
			get
			{
				return parent;
			}
		}

		internal virtual void registerItem(LogisimMenuItem which, MenuItem item)
		{
			menuItems[which] = item;
		}

		internal virtual void fireStateChanged(Simulator sim, CircuitState state)
		{
			if (simulateListener != null)
			{
				simulateListener.stateChangeRequested(sim, state);
			}
		}

		public virtual void doAction(LogisimMenuItem which)
		{
			MenuItem item = menuItems[which];
			item.actionPerformed(new ActionEvent(item, ActionEvent.ACTION_PERFORMED, which.ToString()));
		}

		public virtual bool isEnabled(LogisimMenuItem item)
		{
			MenuItem menuItem = menuItems[item];
			return menuItem != null && menuItem.Enabled;
		}
	}

}
