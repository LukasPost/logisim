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

	using CanvasModelEvent = draw.model.CanvasModelEvent;
	using CanvasModelListener = draw.model.CanvasModelListener;
	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using Simulator = logisim.circuit.Simulator;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using LogisimFile = logisim.file.LogisimFile;
	using RevertAppearanceAction = logisim.gui.appear.RevertAppearanceAction;
	using CardPanel = logisim.gui.generic.CardPanel;
	using LogisimMenuItem = logisim.gui.menu.LogisimMenuItem;
	using ProjectCircuitActions = logisim.gui.menu.ProjectCircuitActions;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using SimulateListener = logisim.gui.menu.SimulateListener;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;

	internal class MenuListener
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			fileListener = new FileListener(this);
			editListener = new EditListener(this);
			projectListener = new ProjectMenuListener(this);
			simulateListener = new SimulateMenuListener(this);
		}

		internal interface EnabledListener
		{
			void menuEnableChanged(MenuListener source);
		}

		private class FileListener : ActionListener
		{
			private readonly MenuListener outerInstance;

			public FileListener(MenuListener outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal virtual void register()
			{
				outerInstance.menubar.addActionListener(LogisimMenuBar.EXPORT_IMAGE, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.PRINT, this);
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				Project proj = outerInstance.frame.Project;
				if (src == LogisimMenuBar.EXPORT_IMAGE)
				{
					ExportImage.doExport(proj);
				}
				else if (src == LogisimMenuBar.PRINT)
				{
					Print.doPrint(proj);
				}
			}
		}

		private class EditListener : ActionListener, EditHandler.Listener
		{
			private readonly MenuListener outerInstance;

			public EditListener(MenuListener outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal EditHandler handler = null;

			internal virtual EditHandler Handler
			{
				set
				{
					handler = value;
					value.Listener = this;
					handler.computeEnabled();
				}
			}

			internal virtual void register()
			{
				outerInstance.menubar.addActionListener(LogisimMenuBar.CUT, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.COPY, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.PASTE, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.DELETE, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.DUPLICATE, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.SELECT_ALL, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.RAISE, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.LOWER, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.RAISE_TOP, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.LOWER_BOTTOM, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.ADD_CONTROL, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.REMOVE_CONTROL, this);
				if (handler != null)
				{
					handler.computeEnabled();
				}
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				EditHandler h = handler;
				if (src == LogisimMenuBar.CUT)
				{
					if (h != null)
					{
						h.cut();
					}
				}
				else if (src == LogisimMenuBar.COPY)
				{
					if (h != null)
					{
						h.copy();
					}
				}
				else if (src == LogisimMenuBar.PASTE)
				{
					if (h != null)
					{
						h.paste();
					}
				}
				else if (src == LogisimMenuBar.DELETE)
				{
					if (h != null)
					{
						h.delete();
					}
				}
				else if (src == LogisimMenuBar.DUPLICATE)
				{
					if (h != null)
					{
						h.duplicate();
					}
				}
				else if (src == LogisimMenuBar.SELECT_ALL)
				{
					if (h != null)
					{
						h.selectAll();
					}
				}
				else if (src == LogisimMenuBar.RAISE)
				{
					if (h != null)
					{
						h.raise();
					}
				}
				else if (src == LogisimMenuBar.LOWER)
				{
					if (h != null)
					{
						h.lower();
					}
				}
				else if (src == LogisimMenuBar.RAISE_TOP)
				{
					if (h != null)
					{
						h.raHashSetop();
					}
				}
				else if (src == LogisimMenuBar.LOWER_BOTTOM)
				{
					if (h != null)
					{
						h.lowerBottom();
					}
				}
				else if (src == LogisimMenuBar.ADD_CONTROL)
				{
					if (h != null)
					{
						h.addControlPoint();
					}
				}
				else if (src == LogisimMenuBar.REMOVE_CONTROL)
				{
					if (h != null)
					{
						h.removeControlPoint();
					}
				}
			}

			public virtual void enableChanged(EditHandler handler, LogisimMenuItem action, bool value)
			{
				if (handler == this.handler)
				{
					outerInstance.menubar.setEnabled(action, value);
					outerInstance.fireEnableChanged();
				}
			}
		}

		internal class ProjectMenuListener : ProjectListener, LibraryListener, ActionListener, PropertyChangeListener, CanvasModelListener
		{
			private readonly MenuListener outerInstance;

			public ProjectMenuListener(MenuListener outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal virtual void register()
			{
				Project proj = outerInstance.frame.Project;
				if (proj == null)
				{
					return;
				}

				proj.addProjectListener(this);
				proj.addLibraryListener(this);
				outerInstance.frame.addPropertyChangeListener(Frame.EDITOR_VIEW, this);
				outerInstance.frame.addPropertyChangeListener(Frame.EXPLORER_VIEW, this);
				Circuit circ = proj.CurrentCircuit;
				if (circ != null)
				{
					circ.Appearance.addCanvasModelListener(this);
				}

				outerInstance.menubar.addActionListener(LogisimMenuBar.ADD_CIRCUIT, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.MOVE_CIRCUIT_UP, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.MOVE_CIRCUIT_DOWN, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.SET_MAIN_CIRCUIT, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.REMOVE_CIRCUIT, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.EDIT_LAYOUT, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.EDIT_APPEARANCE, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.VIEW_TOOLBOX, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.VIEW_SIMULATION, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.REVERT_APPEARANCE, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.ANALYZE_CIRCUIT, this);
				outerInstance.menubar.addActionListener(LogisimMenuBar.CIRCUIT_STATS, this);

				computeEnabled();
			}

			public virtual void modelChanged(CanvasModelEvent @event)
			{
				computeRevertEnabled();
			}

			public virtual void projectChanged(ProjectEvent @event)
			{
				int action = @event.Action;
				if (action == ProjectEvent.ACTION_SET_CURRENT)
				{
					Circuit old = (Circuit) @event.OldData;
					if (old != null)
					{
						old.Appearance.removeCanvasModelListener(this);
					}
					Circuit circ = (Circuit) @event.Data;
					if (circ != null)
					{
						circ.Appearance.addCanvasModelListener(this);
					}
					computeEnabled();
				}
				else if (action == ProjectEvent.ACTION_SET_FILE)
				{
					computeEnabled();
				}
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				computeEnabled();
			}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") public void actionPerformed(java.awt.event.ActionEvent event)
			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				Project proj = outerInstance.frame.Project;
				Circuit cur = proj == null ? null : proj.CurrentCircuit;
				if (src == LogisimMenuBar.ADD_CIRCUIT)
				{
					ProjectCircuitActions.doAddCircuit(proj);
				}
				else if (src == LogisimMenuBar.MOVE_CIRCUIT_UP)
				{
					ProjectCircuitActions.doMoveCircuit(proj, cur, -1);
				}
				else if (src == LogisimMenuBar.MOVE_CIRCUIT_DOWN)
				{
					ProjectCircuitActions.doMoveCircuit(proj, cur, 1);
				}
				else if (src == LogisimMenuBar.SET_MAIN_CIRCUIT)
				{
					ProjectCircuitActions.doSetAsMainCircuit(proj, cur);
				}
				else if (src == LogisimMenuBar.REMOVE_CIRCUIT)
				{
					ProjectCircuitActions.doRemoveCircuit(proj, cur);
				}
				else if (src == LogisimMenuBar.EDIT_LAYOUT)
				{
					outerInstance.frame.EditorView = Frame.EDIT_LAYOUT;
				}
				else if (src == LogisimMenuBar.EDIT_APPEARANCE)
				{
					outerInstance.frame.EditorView = Frame.EDIT_APPEARANCE;
				}
				else if (src == LogisimMenuBar.VIEW_TOOLBOX)
				{
					outerInstance.frame.ExplorerView = Frame.VIEW_TOOLBOX;
				}
				else if (src == LogisimMenuBar.VIEW_SIMULATION)
				{
					outerInstance.frame.ExplorerView = Frame.VIEW_SIMULATION;
				}
				else if (src == LogisimMenuBar.REVERT_APPEARANCE)
				{
					proj.doAction(new RevertAppearanceAction(cur));
				}
				else if (src == LogisimMenuBar.ANALYZE_CIRCUIT)
				{
					ProjectCircuitActions.doAnalyze(proj, cur);
				}
				else if (src == LogisimMenuBar.CIRCUIT_STATS)
				{
					StatisticsDialog.show(outerInstance.frame, proj.LogisimFile, cur);
				}
			}

			internal virtual void computeEnabled()
			{
				Project proj = outerInstance.frame.Project;
				LogisimFile file = proj.LogisimFile;
				Circuit cur = proj.CurrentCircuit;
				int curIndex = file.Circuits.IndexOf(cur);
				bool isProjectCircuit = curIndex >= 0;
				string editorView = outerInstance.frame.EditorView;
				string explorerView = outerInstance.frame.ExplorerView;
				bool canSetMain = false;
				bool canMoveUp = false;
				bool canMoveDown = false;
				bool canRemove = false;
				bool canRevert = false;
				bool viewAppearance = editorView.Equals(Frame.EDIT_APPEARANCE);
				bool viewLayout = editorView.Equals(Frame.EDIT_LAYOUT);
				bool viewToolbox = explorerView.Equals(Frame.VIEW_TOOLBOX);
				bool viewSimulation = explorerView.Equals(Frame.VIEW_SIMULATION);
				if (isProjectCircuit)
				{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<?> tools = proj.getLogisimFile().getTools();
					List<object> tools = proj.LogisimFile.Tools;

					canSetMain = proj.LogisimFile.MainCircuit != cur;
					canMoveUp = curIndex > 0;
					canMoveDown = curIndex < tools.Count - 1;
					canRemove = tools.Count > 1;
					canRevert = viewAppearance && !cur.Appearance.DefaultAppearance;
				}

				outerInstance.menubar.setEnabled(LogisimMenuBar.ADD_CIRCUIT, true);
				outerInstance.menubar.setEnabled(LogisimMenuBar.MOVE_CIRCUIT_UP, canMoveUp);
				outerInstance.menubar.setEnabled(LogisimMenuBar.MOVE_CIRCUIT_DOWN, canMoveDown);
				outerInstance.menubar.setEnabled(LogisimMenuBar.SET_MAIN_CIRCUIT, canSetMain);
				outerInstance.menubar.setEnabled(LogisimMenuBar.REMOVE_CIRCUIT, canRemove);
				outerInstance.menubar.setEnabled(LogisimMenuBar.VIEW_TOOLBOX, !viewToolbox);
				outerInstance.menubar.setEnabled(LogisimMenuBar.VIEW_SIMULATION, !viewSimulation);
				outerInstance.menubar.setEnabled(LogisimMenuBar.EDIT_LAYOUT, !viewLayout);
				outerInstance.menubar.setEnabled(LogisimMenuBar.EDIT_APPEARANCE, !viewAppearance);
				outerInstance.menubar.setEnabled(LogisimMenuBar.REVERT_APPEARANCE, canRevert);
				outerInstance.menubar.setEnabled(LogisimMenuBar.ANALYZE_CIRCUIT, true);
				outerInstance.menubar.setEnabled(LogisimMenuBar.CIRCUIT_STATS, true);
				outerInstance.fireEnableChanged();
			}

			internal virtual void computeRevertEnabled()
			{
				// do this separately since it can happen rather often
				Project proj = outerInstance.frame.Project;
				LogisimFile file = proj.LogisimFile;
				Circuit cur = proj.CurrentCircuit;
				bool isProjectCircuit = file.contains(cur);
				bool viewAppearance = outerInstance.frame.EditorView.Equals(Frame.EDIT_APPEARANCE);
				bool canRevert = isProjectCircuit && viewAppearance && !cur.Appearance.DefaultAppearance;
				bool oldValue = outerInstance.menubar.isEnabled(LogisimMenuBar.REVERT_APPEARANCE);
				if (canRevert != oldValue)
				{
					outerInstance.menubar.setEnabled(LogisimMenuBar.REVERT_APPEARANCE, canRevert);
					outerInstance.fireEnableChanged();
				}
			}

			public virtual void propertyChange(PropertyChangeEvent e)
			{
				computeEnabled();
			}
		}

		internal class SimulateMenuListener : ProjectListener, SimulateListener
		{
			private readonly MenuListener outerInstance;

			public SimulateMenuListener(MenuListener outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal virtual void register()
			{
				Project proj = outerInstance.frame.Project;
				proj.addProjectListener(this);
				outerInstance.menubar.SimulateListener = this;
				outerInstance.menubar.setCircuitState(proj.Simulator, proj.CircuitState);
			}

			public virtual void projectChanged(ProjectEvent @event)
			{
				if (@event.Action == ProjectEvent.ACTION_SET_STATE)
				{
					outerInstance.menubar.setCircuitState(outerInstance.frame.Project.Simulator, outerInstance.frame.Project.CircuitState);
				}
			}

			public virtual void stateChangeRequested(Simulator sim, CircuitState state)
			{
				if (state != null)
				{
					outerInstance.frame.Project.CircuitState = state;
				}
			}
		}

		private Frame frame;
		private LogisimMenuBar menubar;
		private List<EnabledListener> listeners;
		private FileListener fileListener;
		private EditListener editListener;
		private ProjectMenuListener projectListener;
		private SimulateMenuListener simulateListener;

		public MenuListener(Frame frame, LogisimMenuBar menubar)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.frame = frame;
			this.menubar = menubar;
			this.listeners = new List<EnabledListener>();
		}

		internal virtual LogisimMenuBar MenuBar
		{
			get
			{
				return menubar;
			}
		}

		public virtual void register(CardPanel mainPanel)
		{
			fileListener.register();
			editListener.register();
			projectListener.register();
			simulateListener.register();
		}

		public virtual EditHandler EditHandler
		{
			set
			{
				editListener.Handler = value;
			}
		}

		public virtual void addEnabledListener(EnabledListener listener)
		{
			listeners.Add(listener);
		}

		public virtual void removeEnabledListener(EnabledListener listener)
		{
			listeners.Remove(listener);
		}

		public virtual void doAction(LogisimMenuItem item)
		{
			menubar.doAction(item);
		}

		public virtual bool isEnabled(LogisimMenuItem item)
		{
			return menubar.isEnabled(item);
		}

		private void fireEnableChanged()
		{
			foreach (EnabledListener listener in listeners)
			{
				listener.menuEnableChanged(this);
			}
		}
	}

}
