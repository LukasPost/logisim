// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{


	using Toolbar = draw.toolbar.Toolbar;
	using ToolbarModel = draw.toolbar.ToolbarModel;
	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using Component = logisim.comp.Component;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeSet = logisim.data.AttributeSet;
	using Direction = logisim.data.Direction;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using AppearanceView = logisim.gui.appear.AppearanceView;
	using AttrTable = logisim.gui.generic.AttrTable;
	using AttrTableModel = logisim.gui.generic.AttrTableModel;
	using BasicZoomModel = logisim.gui.generic.BasicZoomModel;
	using CanvasPane = logisim.gui.generic.CanvasPane;
	using CardPanel = logisim.gui.generic.CardPanel;
	using LFrame = logisim.gui.generic.LFrame;
	using ZoomControl = logisim.gui.generic.ZoomControl;
	using ZoomModel = logisim.gui.generic.ZoomModel;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;
	using ProjectActions = logisim.proj.ProjectActions;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using Projects = logisim.proj.Projects;
	using Tool = logisim.tools.Tool;
	using HorizontalSplitPane = logisim.util.HorizontalSplitPane;
	using JFileChoosers = logisim.util.JFileChoosers;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using StringUtil = logisim.util.StringUtil;
	using VerticalSplitPane = logisim.util.VerticalSplitPane;

	public class Frame : LFrame, LocaleListener
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myProjectListener = new MyProjectListener(this);
		}

		public const string EDITOR_VIEW = "editorView";
		public const string EXPLORER_VIEW = "explorerView";
		public const string EDIT_LAYOUT = "layout";
		public const string EDIT_APPEARANCE = "appearance";
		public const string VIEW_TOOLBOX = "toolbox";
		public const string VIEW_SIMULATION = "simulation";

		private static readonly double[] ZOOM_OPTIONS = new double[] {20, 50, 75, 100, 133, 150, 200, 250, 300, 400};

		internal class MyProjectListener : ProjectListener, LibraryListener, CircuitListener, PropertyChangeListener, ChangeListener
		{
			private readonly Frame outerInstance;

			public MyProjectListener(Frame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void projectChanged(ProjectEvent @event)
			{
				int action = @event.Action;

				if (action == ProjectEvent.ACTION_SET_FILE)
				{
					outerInstance.computeTitle();
					outerInstance.proj.Tool = outerInstance.proj.Options.ToolbarData.FirstTool;
					outerInstance.placeToolbar();
				}
				else if (action == ProjectEvent.ACTION_SET_CURRENT)
				{
					outerInstance.EditorView = EDIT_LAYOUT;
					if (outerInstance.appearance != null)
					{
						outerInstance.appearance.setCircuit(outerInstance.proj, outerInstance.proj.CircuitState);
					}
					outerInstance.viewAttributes(outerInstance.proj.Tool);
					outerInstance.computeTitle();
				}
				else if (action == ProjectEvent.ACTION_SET_TOOL)
				{
					if (outerInstance.attrTable == null)
					{
						return; // for startup
					}
					Tool oldTool = (Tool) @event.OldData;
					Tool newTool = (Tool) @event.Data;
					outerInstance.viewAttributes(oldTool, newTool, false);
				}
			}

			public virtual void libraryChanged(LibraryEvent e)
			{
				if (e.Action == LibraryEvent.SET_NAME)
				{
					outerInstance.computeTitle();
				}
				else if (e.Action == LibraryEvent.DIRTY_STATE)
				{
					enableSave();
				}
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				if (@event.Action == CircuitEvent.ACTION_SET_NAME)
				{
					outerInstance.computeTitle();
				}
			}

			internal virtual void enableSave()
			{
				Project proj = outerInstance.Project;
				bool ok = proj.FileDirty;
				getRootPane().putClientProperty("windowModified", Convert.ToBoolean(ok));
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				if (AppPreferences.TOOLBAR_PLACEMENT.isSource(@event))
				{
					outerInstance.placeToolbar();
				}
			}

			public virtual void stateChanged(ChangeEvent @event)
			{
				object source = @event.getSource();
				if (source == outerInstance.explorerPane)
				{
					firePropertyChange(EXPLORER_VIEW, "???", outerInstance.ExplorerView);
				}
				else if (source == outerInstance.mainPanel)
				{
					firePropertyChange(EDITOR_VIEW, "???", outerInstance.EditorView);
				}
			}
		}

		internal class MyWindowListener : WindowAdapter
		{
			private readonly Frame outerInstance;

			public MyWindowListener(Frame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void windowClosing(WindowEvent e)
			{
				if (outerInstance.confirmClose(Strings.get("confirmCloseTitle")))
				{
					outerInstance.layoutCanvas.closeCanvas();
					outerInstance.dispose();
				}
			}

			public override void windowOpened(WindowEvent e)
			{
				outerInstance.layoutCanvas.computeSize(true);
			}
		}

		private Project proj;
		private MyProjectListener myProjectListener;

		// GUI elements shared between views
		private LogisimMenuBar menubar;
		private MenuListener menuListener;
		private Toolbar toolbar;
		private HorizontalSplitPane leftRegion;
		private VerticalSplitPane mainRegion;
		private JPanel mainPanelSuper;
		private CardPanel mainPanel;
		// left-side elements
		private Toolbar projectToolbar;
		private CardPanel explorerPane;
		private Toolbox toolbox;
		private SimulationExplorer simExplorer;
		private AttrTable attrTable;
		private ZoomControl zoom;

		// for the Layout view
		private LayoutToolbarModel layoutToolbarModel;
		private Canvas layoutCanvas;
		private ZoomModel layoutZoomModel;
		private LayoutEditHandler layoutEditHandler;
		private AttrTableSelectionModel attrTableSelectionModel;

		// for the Appearance view
		private AppearanceView appearance;

		public Frame(Project proj)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.proj = proj;

			setBackground(Color.White);
			setDefaultCloseOperation(WindowConstants.DO_NOTHING_ON_CLOSE);
			addWindowListener(new MyWindowListener(this));

			proj.addProjectListener(myProjectListener);
			proj.addLibraryListener(myProjectListener);
			proj.addCircuitListener(myProjectListener);
			computeTitle();

			// set up elements for the Layout view
			layoutToolbarModel = new LayoutToolbarModel(this, proj);
			layoutCanvas = new Canvas(proj);
			layoutZoomModel = new BasicZoomModel(AppPreferences.LAYOUT_SHOW_GRID, AppPreferences.LAYOUT_ZOOM, ZOOM_OPTIONS);

			layoutCanvas.GridPainter.ZoomModel = layoutZoomModel;
			layoutEditHandler = new LayoutEditHandler(this);
			attrTableSelectionModel = new AttrTableSelectionModel(proj, this);

			// set up menu bar and toolbar
			menubar = new LogisimMenuBar(this, proj);
			menuListener = new MenuListener(this, menubar);
			menuListener.EditHandler = layoutEditHandler;
			setJMenuBar(menubar);
			toolbar = new Toolbar(layoutToolbarModel);

			// set up the left-side components
			ToolbarModel projectToolbarModel = new ExplorerToolbarModel(this, menuListener);
			projectToolbar = new Toolbar(projectToolbarModel);
			toolbox = new Toolbox(proj, menuListener);
			simExplorer = new SimulationExplorer(proj, menuListener);
			explorerPane = new CardPanel();
			explorerPane.addView(VIEW_TOOLBOX, toolbox);
			explorerPane.addView(VIEW_SIMULATION, simExplorer);
			explorerPane.View = VIEW_TOOLBOX;
			attrTable = new AttrTable(this);
			zoom = new ZoomControl(layoutZoomModel);

			// set up the central area
			CanvasPane canvasPane = new CanvasPane(layoutCanvas);
			mainPanelSuper = new JPanel(new BorderLayout());
			canvasPane.ZoomModel = layoutZoomModel;
			mainPanel = new CardPanel();
			mainPanel.addView(EDIT_LAYOUT, canvasPane);
			mainPanel.View = EDIT_LAYOUT;
			mainPanelSuper.add(mainPanel, BorderLayout.CENTER);

			// set up the contents, split down the middle, with the canvas
			// on the right and a split pane on the left containing the
			// explorer and attribute values.
			JPanel explPanel = new JPanel(new BorderLayout());
			explPanel.add(projectToolbar, BorderLayout.NORTH);
			explPanel.add(explorerPane, BorderLayout.CENTER);
			JPanel attrPanel = new JPanel(new BorderLayout());
			attrPanel.add(attrTable, BorderLayout.CENTER);
			attrPanel.add(zoom, BorderLayout.SOUTH);

			leftRegion = new HorizontalSplitPane(explPanel, attrPanel, AppPreferences.WINDOW_LEFT_SPLIT.get().doubleValue());
			mainRegion = new VerticalSplitPane(leftRegion, mainPanelSuper, AppPreferences.WINDOW_MAIN_SPLIT.get().doubleValue());

			getContentPane().add(mainRegion, BorderLayout.CENTER);

			computeTitle();

			this.setSize(AppPreferences.WINDOW_WIDTH.get().intValue(), AppPreferences.WINDOW_HEIGHT.get().intValue());
			Point prefPoint = InitialLocation;
			if (prefPoint != null)
			{
				this.setLocation(prefPoint);
			}
			this.setExtendedState(AppPreferences.WINDOW_STATE.get().intValue());

			menuListener.register(mainPanel);
			KeyboardToolSelection.register(toolbar);

			proj.Frame = this;
			if (proj.Tool == null)
			{
				proj.Tool = proj.Options.ToolbarData.FirstTool;
			}
			mainPanel.addChangeListener(myProjectListener);
			explorerPane.addChangeListener(myProjectListener);
			AppPreferences.TOOLBAR_PLACEMENT.addPropertyChangeListener(myProjectListener);
			placeToolbar();
			((MenuListener.EnabledListener) projectToolbarModel).menuEnableChanged(menuListener);

			LocaleManager.addLocaleListener(this);
		}

		private void placeToolbar()
		{
			string loc = AppPreferences.TOOLBAR_PLACEMENT.get();
			Container contents = getContentPane();
			contents.remove(toolbar);
			mainPanelSuper.remove(toolbar);
			if (AppPreferences.TOOLBAR_HIDDEN.Equals(loc))
			{
				; // don't place value anywhere
			}
			else if (AppPreferences.TOOLBAR_DOWN_MIDDLE.Equals(loc))
			{
				toolbar.Orientation = Toolbar.VERTICAL;
				mainPanelSuper.add(toolbar, BorderLayout.WEST);
			}
			else
			{ // it is a BorderLayout constant
				object value = BorderLayout.NORTH;
				foreach (Direction dir in Direction.cardinals)
				{
					if (dir.ToString().Equals(loc))
					{
						if (dir == Direction.East)
						{
							value = BorderLayout.EAST;
						}
						else if (dir == Direction.South)
						{
							value = BorderLayout.SOUTH;
						}
						else if (dir == Direction.West)
						{
							value = BorderLayout.WEST;
						}
						else
						{
							value = BorderLayout.NORTH;
						}
					}
				}

				contents.add(toolbar, value);
				bool vertical = value == BorderLayout.WEST || value == BorderLayout.EAST;
				toolbar.Orientation = vertical ? Toolbar.VERTICAL : Toolbar.HORIZONTAL;
			}
			contents.validate();
		}

		public virtual Project Project
		{
			get
			{
				return proj;
			}
		}

		public virtual void viewComponentAttributes(Circuit circ, Component comp)
		{
			if (comp == null)
			{
				AttrTableModel = null;
			}
			else
			{
				AttrTableModel = new AttrTableComponentModel(proj, circ, comp);
			}
		}

		internal virtual AttrTableModel AttrTableModel
		{
			set
			{
				attrTable.AttrTableModel = value;
				if (value is AttrTableToolModel)
				{
					Tool tool = ((AttrTableToolModel) value).Tool;
					toolbox.HaloedTool = tool;
					layoutToolbarModel.HaloedTool = tool;
				}
				else
				{
					toolbox.HaloedTool = null;
					layoutToolbarModel.HaloedTool = null;
				}
				if (value is AttrTableComponentModel)
				{
					Circuit circ = ((AttrTableComponentModel) value).Circuit;
					Component comp = ((AttrTableComponentModel) value).Component;
					layoutCanvas.setHaloedComponent(circ, comp);
				}
				else
				{
					layoutCanvas.setHaloedComponent(null, null);
				}
			}
		}

		public virtual string ExplorerView
		{
			set
			{
				explorerPane.View = value;
			}
			get
			{
				return explorerPane.View;
			}
		}


		public virtual string EditorView
		{
			set
			{
				string curView = mainPanel.View;
				if (curView.Equals(value))
				{
					return;
				}
    
				if (value.Equals(EDIT_APPEARANCE))
				{ // appearance value
					AppearanceView app = appearance;
					if (app == null)
					{
						app = new AppearanceView();
						app.setCircuit(proj, proj.CircuitState);
						mainPanel.addView(EDIT_APPEARANCE, app.CanvasPane);
						appearance = app;
					}
					toolbar.ToolbarModel = app.ToolbarModel;
					app.getAttrTableDrawManager(attrTable).attributesSelected();
					zoom.ZoomModel = app.ZoomModel;
					menuListener.EditHandler = app.EditHandler;
					mainPanel.View = value;
					app.Canvas.requestFocus();
				}
				else
				{ // layout value
					toolbar.ToolbarModel = layoutToolbarModel;
					zoom.ZoomModel = layoutZoomModel;
					menuListener.EditHandler = layoutEditHandler;
					viewAttributes(proj.Tool, true);
					mainPanel.View = value;
					layoutCanvas.requestFocus();
				}
			}
			get
			{
				return mainPanel.View;
			}
		}


		public virtual Canvas Canvas
		{
			get
			{
				return layoutCanvas;
			}
		}

		private void computeTitle()
		{
			string s;
			Circuit circuit = proj.CurrentCircuit;
			string name = proj.LogisimFile.Name;
			if (circuit != null)
			{
				s = StringUtil.format(Strings.get("titleCircFileKnown"), circuit.Name, name);
			}
			else
			{
				s = StringUtil.format(Strings.get("titleFileKnown"), name);
			}
			this.setTitle(s);
			myProjectListener.enableSave();
		}

		internal virtual void viewAttributes(Tool newTool)
		{
			viewAttributes(null, newTool, false);
		}

		private void viewAttributes(Tool newTool, bool force)
		{
			viewAttributes(null, newTool, force);
		}

		private void viewAttributes(Tool oldTool, Tool newTool, bool force)
		{
			AttributeSet newAttrs;
			if (newTool == null)
			{
				newAttrs = null;
				if (!force)
				{
					return;
				}
			}
			else
			{
				newAttrs = newTool.getAttributeSet(layoutCanvas);
			}
			if (newAttrs == null)
			{
				AttrTableModel oldModel = attrTable.AttrTableModel;
				bool same = oldModel is AttrTableToolModel && ((AttrTableToolModel) oldModel).Tool == oldTool;
				if (!force && !same && !(oldModel is AttrTableCircuitModel))
				{
					return;
				}
			}
			if (newAttrs == null)
			{
				Circuit circ = proj.CurrentCircuit;
				if (circ != null)
				{
					AttrTableModel = new AttrTableCircuitModel(proj, circ);
				}
				else if (force)
				{
					AttrTableModel = null;
				}
			}
			else if (newAttrs is SelectionAttributes)
			{
				AttrTableModel = attrTableSelectionModel;
			}
			else
			{
				AttrTableModel = new AttrTableToolModel(proj, newTool);
			}
		}

		public virtual void localeChanged()
		{
			computeTitle();
		}

		public virtual void savePreferences()
		{
			AppPreferences.TICK_FREQUENCY.set(Convert.ToDouble(proj.Simulator.TickFrequency));
			AppPreferences.LAYOUT_SHOW_GRID.Boolean = layoutZoomModel.ShowGrid;
			AppPreferences.LAYOUT_ZOOM.set(Convert.ToDouble(layoutZoomModel.ZoomFactor));
			if (appearance != null)
			{
				ZoomModel aZoom = appearance.ZoomModel;
				AppPreferences.APPEARANCE_SHOW_GRID.Boolean = aZoom.ShowGrid;
				AppPreferences.APPEARANCE_ZOOM.set(Convert.ToDouble(aZoom.ZoomFactor));
			}
			int state = getExtendedState() & ~JFrame.ICONIFIED;
			AppPreferences.WINDOW_STATE.set(Convert.ToInt32(state));
			Size dim = getSize();
			AppPreferences.WINDOW_WIDTH.set(Convert.ToInt32(dim.Width));
			AppPreferences.WINDOW_HEIGHT.set(Convert.ToInt32(dim.Height));
			Point loc;
			try
			{
				loc = getLocationOnScreen();
			}
			catch (IllegalComponentStateException)
			{
				loc = Projects.getLocation(this);
			}
			if (loc != null)
			{
				AppPreferences.WINDOW_LOCATION.set(loc.x + "," + loc.y);
			}
			AppPreferences.WINDOW_LEFT_SPLIT.set(Convert.ToDouble(leftRegion.Fraction));
			AppPreferences.WINDOW_MAIN_SPLIT.set(Convert.ToDouble(mainRegion.Fraction));
			AppPreferences.DIALOG_DIRECTORY.set(JFileChoosers.CurrentDirectory);
		}

		public virtual bool confirmClose()
		{
			return confirmClose(Strings.get("confirmCloseTitle"));
		}

		// returns true if user is OK with proceeding
		public virtual bool confirmClose(string title)
		{
			string message = StringUtil.format(Strings.get("confirmDiscardMessage"), proj.LogisimFile.Name);

			if (!proj.FileDirty)
			{
				return true;
			}
			toFront();
			string[] options = new string[] {Strings.get("saveOption"), Strings.get("discardOption"), Strings.get("cancelOption")};
			int result = JOptionPane.showOptionDialog(this, message, title, 0, JOptionPane.QUESTION_MESSAGE, null, options, options[0]);
			bool ret;
			if (result == 0)
			{
				ret = ProjectActions.doSave(proj);
			}
			else if (result == 1)
			{
				ret = true;
			}
			else
			{
				ret = false;
			}
			if (ret)
			{
				dispose();
			}
			return ret;
		}

		private static Point InitialLocation
		{
			get
			{
				string s = AppPreferences.WINDOW_LOCATION.get();
				if (string.ReferenceEquals(s, null))
				{
					return null;
				}
				int comma = s.IndexOf(',');
				if (comma < 0)
				{
					return null;
				}
				try
				{
					int x = int.Parse(s.Substring(0, comma));
					int y = int.Parse(s.Substring(comma + 1));
					while (isProjectFrameAt(x, y))
					{
						x += 20;
						y += 20;
					}
					Rectangle desired = new Rectangle(x, y, 50, 50);
    
					int gcBestSize = 0;
					Point gcBestPoint = null;
					JGraphicsEnvironment ge;
					ge = JGraphicsEnvironment.getLocalJGraphicsEnvironment();
					foreach (JGraphicsDevice gd in ge.getScreenDevices())
					{
						foreach (JGraphicsConfiguration gc in gd.getConfigurations())
						{
							Rectangle gcBounds = gc.getBounds();
							if (gcBounds.intersects(desired))
							{
								Rectangle inter = gcBounds.intersection(desired);
								int size = inter.Width * inter.Height;
								if (size > gcBestSize)
								{
									gcBestSize = size;
									int x2 = Math.Max(gcBounds.x, Math.Min(inter.X, inter.X + inter.Width - 50));
									int y2 = Math.Max(gcBounds.y, Math.Min(inter.Y, inter.Y + inter.Height - 50));
									gcBestPoint = new Point(x2, y2);
								}
							}
						}
					}
					if (gcBestPoint != null)
					{
						if (isProjectFrameAt(gcBestpoint.X, gcBestpoint.Y))
						{
							gcBestPoint = null;
						}
					}
					return gcBestPoint;
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		private static bool isProjectFrameAt(int x, int y)
		{
			foreach (Project current in Projects.OpenProjects)
			{
				Frame frame = current.Frame;
				if (frame != null)
				{
					Point loc = frame.getLocationOnScreen();
					int d = Math.Abs(loc.x - x) + Math.Abs(loc.y - y);
					if (d <= 3)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

}
