// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{


	using CircuitState = logisim.circuit.CircuitState;
	using Simulator = logisim.circuit.Simulator;
	using SimulatorEvent = logisim.circuit.SimulatorEvent;
	using SimulatorListener = logisim.circuit.SimulatorListener;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using LFrame = logisim.gui.generic.LFrame;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using StringUtil = logisim.util.StringUtil;
	using WindowMenuItemManager = logisim.util.WindowMenuItemManager;

	public class LogFrame : LFrame
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		// TODO should automatically repaint icons when component attr change
		// TODO ? moving a component using Select tool removes it from selection
		private class WindowMenuManager : WindowMenuItemManager, LocaleListener, ProjectListener, LibraryListener
		{
			private readonly LogFrame outerInstance;

			internal WindowMenuManager(LogFrame outerInstance) : base(Strings.get("logFrameMenuItem"), false)
			{
				this.outerInstance = outerInstance;
				outerInstance.project.addProjectListener(this);
				outerInstance.project.addLibraryListener(this);
			}

			public override JFrame getJFrame(bool create)
			{
				return outerInstance;
			}

			public virtual void localeChanged()
			{
				string title = outerInstance.project.LogisimFile.DisplayName;
				Text = StringUtil.format(Strings.get("logFrameMenuItem"), title);
			}

			public virtual void projectChanged(ProjectEvent @event)
			{
				if (@event.Action == ProjectEvent.ACTION_SET_FILE)
				{
					localeChanged();
				}
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				if (@event.Action == LibraryEvent.SET_NAME)
				{
					localeChanged();
				}
			}
		}

		private class MyListener : ActionListener, ProjectListener, LibraryListener, SimulatorListener, LocaleListener
		{
			private readonly LogFrame outerInstance;

			public MyListener(LogFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.close)
				{
					WindowEvent e = new WindowEvent(outerInstance, WindowEvent.WINDOW_CLOSING);
					outerInstance.processWindowEvent(e);
				}
			}

			public virtual void projectChanged(ProjectEvent @event)
			{
				int action = @event.Action;
				if (action == ProjectEvent.ACTION_SET_STATE)
				{
					outerInstance.setSimulator(@event.Project.Simulator, @event.Project.CircuitState);
				}
				else if (action == ProjectEvent.ACTION_SET_FILE)
				{
					setTitle(computeTitle(outerInstance.curModel, outerInstance.project));
				}
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				int action = @event.Action;
				if (action == LibraryEvent.SET_NAME)
				{
					setTitle(computeTitle(outerInstance.curModel, outerInstance.project));
				}
			}

			public virtual void localeChanged()
			{
				setTitle(computeTitle(outerInstance.curModel, outerInstance.project));
				for (int i = 0; i < outerInstance.panels.Length; i++)
				{
					outerInstance.tabbedPane.setTitleAt(i, outerInstance.panels[i].Title);
					outerInstance.tabbedPane.setToolTipTextAt(i, outerInstance.panels[i].getToolTipText());
					outerInstance.panels[i].localeChanged();
				}
				outerInstance.close.setText(Strings.get("closeButton"));
				outerInstance.windowManager.localeChanged();
			}

			public virtual void propagationCompleted(SimulatorEvent e)
			{
				outerInstance.curModel.propagationCompleted();
			}

			public virtual void tickCompleted(SimulatorEvent e)
			{
			}

			public virtual void simulatorStateChanged(SimulatorEvent e)
			{
			}
		}

		private Project project;
		private Simulator curSimulator = null;
		private Model curModel;
		private Dictionary<CircuitState, Model> modelMap = new Dictionary<CircuitState, Model>();
		private MyListener myListener;
		private WindowMenuManager windowManager;

		private LogPanel[] panels;
		private JTabbedPane tabbedPane;
		private JButton close = new JButton();

		public LogFrame(Project project)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.project = project;
			this.windowManager = new WindowMenuManager(this);
			project.addProjectListener(myListener);
			project.addLibraryListener(myListener);
			setDefaultCloseOperation(HIDE_ON_CLOSE);
			setJMenuBar(new LogisimMenuBar(this, project));
			setSimulator(project.Simulator, project.CircuitState);

			panels = new LogPanel[]
			{
				new SelectionPanel(this),
				new ScrollPanel(this),
				new FilePanel(this)
			};
			tabbedPane = new JTabbedPane();
			for (int index = 0; index < panels.Length; index++)
			{
				LogPanel panel = panels[index];
				tabbedPane.addTab(panel.Title, null, panel, panel.getToolTipText());
			}

			JPanel buttonPanel = new JPanel();
			buttonPanel.add(close);
			close.addActionListener(myListener);

			Container contents = getContentPane();
			tabbedPane.setPreferredSize(new Size(450, 300));
			contents.add(tabbedPane, BorderLayout.CENTER);
			contents.add(buttonPanel, BorderLayout.SOUTH);

			LocaleManager.addLocaleListener(myListener);
			myListener.localeChanged();
			pack();
		}

		public virtual Project Project
		{
			get
			{
				return project;
			}
		}

		internal virtual Model Model
		{
			get
			{
				return curModel;
			}
		}

		private void setSimulator(Simulator value, CircuitState state)
		{
			if ((value == null) == (curModel == null))
			{
				if (value == null || value.CircuitState == curModel.CircuitState)
				{
					return;
				}
			}

			LogisimMenuBar menubar = (LogisimMenuBar) getJMenuBar();
			menubar.setCircuitState(value, state);

			if (curSimulator != null)
			{
				curSimulator.removeSimulatorListener(myListener);
			}
			if (curModel != null)
			{
				curModel.setSelected(this, false);
			}

			Model oldModel = curModel;
			Model data = null;
			if (value != null)
			{
				data = modelMap[value.CircuitState];
				if (data == null)
				{
					data = new Model(value.CircuitState);
					modelMap[data.CircuitState] = data;
				}
			}
			curSimulator = value;
			curModel = data;

			if (curSimulator != null)
			{
				curSimulator.addSimulatorListener(myListener);
			}
			if (curModel != null)
			{
				curModel.setSelected(this, true);
			}
			setTitle(computeTitle(curModel, project));
			if (panels != null)
			{
				for (int i = 0; i < panels.Length; i++)
				{
					panels[i].modelChanged(oldModel, curModel);
				}
			}
		}

		public override bool Visible
		{
			set
			{
				if (value)
				{
					windowManager.frameOpened(this);
				}
				base.setVisible(value);
			}
		}

		internal virtual LogPanel[] PrefPanels
		{
			get
			{
				return panels;
			}
		}

		private static string computeTitle(Model data, Project proj)
		{
			string name = data == null ? "???" : data.CircuitState.Circuit.Name;
			return StringUtil.format(Strings.get("logFrameTitle"), name, proj.LogisimFile.DisplayName);
		}
	}

}
