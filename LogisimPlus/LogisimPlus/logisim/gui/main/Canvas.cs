// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using CircuitState = logisim.circuit.CircuitState;
	using Propagator = logisim.circuit.Propagator;
	using SimulatorEvent = logisim.circuit.SimulatorEvent;
	using SimulatorListener = logisim.circuit.SimulatorListener;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using WidthIncompatibilityData = logisim.circuit.WidthIncompatibilityData;
	using WireSet = logisim.circuit.WireSet;
	using Component = logisim.comp.Component;
	using ComponentUserEvent = logisim.comp.ComponentUserEvent;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using LogisimFile = logisim.file.LogisimFile;
	using MouseMappings = logisim.file.MouseMappings;
	using Options = logisim.file.Options;
	using CanvasPane = logisim.gui.generic.CanvasPane;
	using CanvasPaneContents = logisim.gui.generic.CanvasPaneContents;
	using GridPainter = logisim.gui.generic.GridPainter;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using AddTool = logisim.tools.AddTool;
	using EditTool = logisim.tools.EditTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;
	using ToolTipMaker = logisim.tools.ToolTipMaker;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using StringGetter = logisim.util.StringGetter;
    using LogisimPlus.Java;

    public class Canvas : JPanel, LocaleListener, CanvasPaneContents
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
			viewport = new MyViewport(this);
			myProjectListener = new MyProjectListener(this);
		}

		internal static readonly Color HALO_COLOR = Color.FromArgb(255, 192, 255, 255);

		private const int BOUNDS_BUFFER = 70;
		// pixels shown in canvas beyond outermost boundaries
		private const int THRESH_SIZE_UPDATE = 10;
		// don't bother to update the size if it hasn't changed more than this
		internal static readonly double SQRT_2 = Math.Sqrt(2.0);
		private static readonly int BUTTONS_MASK = InputEvent.BUTTON1_DOWN_MASK | InputEvent.BUTTON2_DOWN_MASK | InputEvent.BUTTON3_DOWN_MASK;
		private static readonly Color DEFAULT_ERROR_COLOR = Color.FromArgb(255, 192, 0, 0);

		private static readonly Color TICK_RATE_COLOR = Color.FromArgb(255, 0, 0, 92, 92);
		private static readonly Font TICK_RATE_FONT = new Font("serif", Font.BOLD, 12);

		private class MyListener : MouseInputListener, KeyListener, PopupMenuListener, PropertyChangeListener
		{
			private readonly Canvas outerInstance;

			public MyListener(Canvas outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal bool menu_on = false;

			//
			// MouseListener methods
			//
			public virtual void mouseClicked(MouseEvent e)
			{
			}

			public virtual void mouseMoved(MouseEvent e)
			{
				if ((e.getModifiersEx() & BUTTONS_MASK) != 0)
				{
					// If the control key is down while the mouse is being
					// dragged, mouseMoved is called instead. This may well be
					// an issue specific to the MacOS Java implementation,
					// but it exists there in the 1.4 and 5.0 versions.
					mouseDragged(e);
					return;
				}

				Tool tool = getToolFor(e);
				if (tool != null)
				{
					tool.mouseMoved(outerInstance, getJGraphics(), e);
				}
			}

			public virtual void mouseDragged(MouseEvent e)
			{
				if (outerInstance.drag_tool != null)
				{
					outerInstance.drag_tool.mouseDragged(outerInstance, getJGraphics(), e);
				}
			}

			public virtual void mouseEntered(MouseEvent e)
			{
				if (outerInstance.drag_tool != null)
				{
					outerInstance.drag_tool.mouseEntered(outerInstance, getJGraphics(), e);
				}
				else
				{
					Tool tool = getToolFor(e);
					if (tool != null)
					{
						tool.mouseEntered(outerInstance, getJGraphics(), e);
					}
				}
			}

			public virtual void mouseExited(MouseEvent e)
			{
				if (outerInstance.drag_tool != null)
				{
					outerInstance.drag_tool.mouseExited(outerInstance, getJGraphics(), e);
				}
				else
				{
					Tool tool = getToolFor(e);
					if (tool != null)
					{
						tool.mouseExited(outerInstance, getJGraphics(), e);
					}
				}
			}

			public virtual void mousePressed(MouseEvent e)
			{
				outerInstance.viewport.setErrorMessage(null, null);
				outerInstance.proj.StartupScreen = false;
				outerInstance.requestFocus();
				outerInstance.drag_tool = getToolFor(e);
				if (outerInstance.drag_tool != null)
				{
					outerInstance.drag_tool.mousePressed(outerInstance, getJGraphics(), e);
				}

				outerInstance.completeAction();
			}

			public virtual void mouseReleased(MouseEvent e)
			{
				if (outerInstance.drag_tool != null)
				{
					outerInstance.drag_tool.mouseReleased(outerInstance, getJGraphics(), e);
					outerInstance.drag_tool = null;
				}

				Tool tool = outerInstance.proj.Tool;
				if (tool != null)
				{
					tool.mouseMoved(outerInstance, getJGraphics(), e);
				}

				outerInstance.completeAction();
			}

			internal virtual Tool getToolFor(MouseEvent e)
			{
				if (menu_on)
				{
					return null;
				}

				Tool ret = outerInstance.mappings.getToolFor(e);
				if (ret == null)
				{
					return outerInstance.proj.Tool;
				}
				else
				{
					return ret;
				}
			}

			//
			// KeyListener methods
			//
			public virtual void keyPressed(KeyEvent e)
			{
				Tool tool = outerInstance.proj.Tool;
				if (tool != null)
				{
					tool.keyPressed(outerInstance, e);
				}
			}

			public virtual void keyReleased(KeyEvent e)
			{
				Tool tool = outerInstance.proj.Tool;
				if (tool != null)
				{
					tool.keyReleased(outerInstance, e);
				}
			}

			public virtual void keyTyped(KeyEvent e)
			{
				Tool tool = outerInstance.proj.Tool;
				if (tool != null)
				{
					tool.keyTyped(outerInstance, e);
				}
			}

			//
			// PopupMenuListener mtehods
			//
			public virtual void popupMenuCanceled(PopupMenuEvent e)
			{
				menu_on = false;
			}

			public virtual void popupMenuWillBecomeInvisible(PopupMenuEvent e)
			{
				menu_on = false;
			}

			public virtual void popupMenuWillBecomeVisible(PopupMenuEvent e)
			{
			}

			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				if (AppPreferences.GATE_SHAPE.isSource(@event) || AppPreferences.SHOW_TICK_RATE.isSource(@event))
				{
					outerInstance.paintThread.requestRepaint();
				}
				else if (AppPreferences.COMPONENT_TIPS.isSource(@event))
				{
					bool showTips = AppPreferences.COMPONENT_TIPS.Boolean;
					setToolTipText(showTips ? "" : null);
				}
			}
		}

		private class MyProjectListener : ProjectListener, LibraryListener, CircuitListener, AttributeListener, SimulatorListener, Selection.Listener
		{
			private readonly Canvas outerInstance;

			public MyProjectListener(Canvas outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void projectChanged(ProjectEvent @event)
			{
				int act = @event.Action;
				if (act == ProjectEvent.ACTION_SET_CURRENT)
				{
					outerInstance.viewport.setErrorMessage(null, null);
					if (outerInstance.painter.HaloedComponent != null)
					{
						outerInstance.proj.Frame.viewComponentAttributes(null, null);
					}
				}
				else if (act == ProjectEvent.ACTION_SET_FILE)
				{
					LogisimFile old = (LogisimFile) @event.OldData;
					if (old != null)
					{
						old.Options.AttributeSet.removeAttributeListener(this);
					}
					LogisimFile file = (LogisimFile) @event.Data;
					if (file != null)
					{
						AttributeSet attrs = file.Options.AttributeSet;
						attrs.addAttributeListener(this);
						outerInstance.loadOptions(attrs);
						outerInstance.mappings = file.Options.MouseMappings;
					}
				}
				else if (act == ProjectEvent.ACTION_SET_TOOL)
				{
					outerInstance.viewport.setErrorMessage(null, null);

					Tool t = @event.Tool;
					if (t == null)
					{
						setCursor(Cursor.getDefaultCursor());
					}
					else
					{
						setCursor(t.Cursor);
					}
				}
				else if (act == ProjectEvent.ACTION_SET_STATE)
				{
					CircuitState oldState = (CircuitState) @event.OldData;
					CircuitState newState = (CircuitState) @event.Data;
					if (oldState != null && newState != null)
					{
						Propagator oldProp = oldState.Propagator;
						Propagator newProp = newState.Propagator;
						if (oldProp != newProp)
						{
							outerInstance.tickCounter.clear();
						}
					}
				}

				if (act != ProjectEvent.ACTION_SELECTION && act != ProjectEvent.ACTION_START && act != ProjectEvent.UNDO_START)
				{
					outerInstance.completeAction();
				}
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				if (@event.Action == LibraryEvent.REMOVE_TOOL)
				{
					object t = @event.Data;
					Circuit circ = null;
					if (t is AddTool)
					{
						t = ((AddTool) t).Factory;
						if (t is SubcircuitFactory)
						{
							circ = ((SubcircuitFactory) t).Subcircuit;
						}
					}

					if (t == outerInstance.proj.CurrentCircuit && t != null)
					{
						outerInstance.proj.CurrentCircuit = outerInstance.proj.LogisimFile.MainCircuit;
					}

					if (outerInstance.proj.Tool == @event.Data)
					{
						Tool next = findTool(outerInstance.proj.LogisimFile.Options.ToolbarData.Contents);
						if (next == null)
						{
							foreach (Library lib in outerInstance.proj.LogisimFile.Libraries)
							{
								next = findTool(lib.Tools);
								if (next != null)
								{
									break;
								}
							}
						}
						outerInstance.proj.Tool = next;
					}

					if (circ != null)
					{
						CircuitState state = outerInstance.CircuitState;
						CircuitState last = state;
						while (state != null && state.Circuit != circ)
						{
							last = state;
							state = state.ParentState;
						}
						if (state != null)
						{
							outerInstance.Project.CircuitState = last.cloneState();
						}
					}
				}
			}

			internal virtual Tool findTool<T1>(List<T1> opts) where T1 : logisim.tools.Tool
			{
				Tool ret = null;
				foreach (Tool o in opts)
				{
					if (ret == null && o != null)
					{
						ret = o;
					}
					else if (o is EditTool)
					{
						ret = o;
					}
				}
				return ret;
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				int act = @event.Action;
				if (act == CircuitEvent.ACTION_REMOVE)
				{
					Component c = (Component) @event.Data;
					if (c == outerInstance.painter.HaloedComponent)
					{
						outerInstance.proj.Frame.viewComponentAttributes(null, null);
					}
				}
				else if (act == CircuitEvent.ACTION_CLEAR)
				{
					if (outerInstance.painter.HaloedComponent != null)
					{
						outerInstance.proj.Frame.viewComponentAttributes(null, null);
					}
				}
				else if (act == CircuitEvent.ACTION_INVALIDATE)
				{
					outerInstance.completeAction();
				}
			}

			public virtual void propagationCompleted(SimulatorEvent e)
			{
				/*
				 * This was a good idea for a while... but it leads to problems when a repaint is done just before a user
				 * action takes place. // repaint - but only if it's been a while since the last one long now =
				 * System.currentTimeMillis(); if (now > lastRepaint + repaintDuration) { lastRepaint = now; // (ensure that
				 * multiple requests aren't made repaintDuration = 15 + (int) (20 * Math.random()); // repaintDuration is
				 * for jittering the repaints to // reduce aliasing effects repaint(); }
				 */
				outerInstance.paintThread.requestRepaint();
			}

			public virtual void tickCompleted(SimulatorEvent e)
			{
				outerInstance.waitForRepaintDone();
			}

			public virtual void simulatorStateChanged(SimulatorEvent e)
			{
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = e.getAttribute();
				Attribute attr = e.Attribute;
				if (attr == Options.ATTR_GATE_UNDEFINED)
				{
					CircuitState circState = outerInstance.CircuitState;
					circState.markComponentsDirty(outerInstance.Circuit.NonWires);
					// TODO actually, we'd want to mark all components in
					// subcircuits as dirty as well
				}
			}

			public virtual void selectionChanged(Selection.Event @event)
			{
				outerInstance.repaint();
			}
		}

		private class MyViewport : JViewport
		{
			private readonly Canvas outerInstance;

			internal StringGetter errorMessage = null;
			internal Color errorColor = DEFAULT_ERROR_COLOR;
			internal string widthMessage = null;
			internal bool isNorth = false;
			internal bool isSouth = false;
			internal bool isWest = false;
			internal bool isEast = false;
			internal bool isNortheast = false;
			internal bool isNorthwest = false;
			internal bool isSoutheast = false;
			internal bool isSouthwest = false;

			internal MyViewport(Canvas outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal virtual void setErrorMessage(StringGetter msg, Color color)
			{
				if (errorMessage != msg)
				{
					errorMessage = msg;
					errorColor = color == null ? DEFAULT_ERROR_COLOR : color;
					outerInstance.paintThread.requestRepaint();
				}
			}

			internal virtual string WidthMessage
			{
				set
				{
					widthMessage = value;
					isNorth = false;
					isSouth = false;
					isWest = false;
					isEast = false;
					isNortheast = false;
					isNorthwest = false;
					isSoutheast = false;
					isSouthwest = false;
				}
			}

			internal virtual bool North
			{
				set
				{
					isNorth = value;
				}
			}

			internal virtual bool South
			{
				set
				{
					isSouth = value;
				}
			}

			internal virtual bool East
			{
				set
				{
					isEast = value;
				}
			}

			internal virtual bool West
			{
				set
				{
					isWest = value;
				}
			}

			internal virtual bool Northeast
			{
				set
				{
					isNortheast = value;
				}
			}

			internal virtual bool Northwest
			{
				set
				{
					isNorthwest = value;
				}
			}

			internal virtual bool Southeast
			{
				set
				{
					isSoutheast = value;
				}
			}

			internal virtual bool Southwest
			{
				set
				{
					isSouthwest = value;
				}
			}

			public override void paintChildren(JGraphics g)
			{
				base.paintChildren(g);
				paintContents(g);
			}

			public override Color Background
			{
				get
				{
					return getView() == null ? base.getBackground() : getView().getBackground();
				}
			}

			internal virtual void paintContents(JGraphics g)
			{
				/*
				 * TODO this is for the SimulatorPrototype class int speed = proj.getSimulator().getSimulationSpeed();
				 * String speedStr; if (speed >= 10000000) { speedStr = (speed / 1000000) + " MHz"; } else if (speed >=
				 * 1000000) { speedStr = (speed / 100000) / 10.0 + " MHz"; } else if (speed >= 10000) { speedStr = (speed /
				 * 1000) + " KHz"; } else if (speed >= 10000) { speedStr = (speed / 100) / 10.0 + " KHz"; } else { speedStr
				 * = speed + " Hz"; } FontMetrics fm = g.getFontMetrics(); g.drawString(speedStr, getWidth() - 10 -
				 * fm.stringWidth(speedStr), getHeight() - 10);
				 */

				StringGetter message = errorMessage;
				if (message != null)
				{
					g.setColor(errorColor);
					paintString(g, message.get());
					return;
				}

				if (outerInstance.proj.Simulator.Oscillating)
				{
					g.setColor(DEFAULT_ERROR_COLOR);
					paintString(g, Strings.get("canvasOscillationError"));
					return;
				}

				if (outerInstance.proj.Simulator.ExceptionEncountered)
				{
					g.setColor(DEFAULT_ERROR_COLOR);
					paintString(g, Strings.get("canvasExceptionError"));
					return;
				}

				outerInstance.computeViewportContents();
				Size sz = getSize();
				g.setColor(Value.WIDTH_ERROR_COLOR);

				if (!string.ReferenceEquals(widthMessage, null))
				{
					paintString(g, widthMessage);
				}

				JGraphicsUtil.switchToWidth(g, 3);
				if (isNorth)
				{
					JGraphicsUtil.drawArrow(g, sz.width / 2, 20, sz.width / 2, 2, 10, 30);
				}
				if (isSouth)
				{
					JGraphicsUtil.drawArrow(g, sz.width / 2, sz.height - 20, sz.width / 2, sz.height - 2, 10, 30);
				}
				if (isEast)
				{
					JGraphicsUtil.drawArrow(g, sz.width - 20, sz.height / 2, sz.width - 2, sz.height / 2, 10, 30);
				}
				if (isWest)
				{
					JGraphicsUtil.drawArrow(g, 20, sz.height / 2, 2, sz.height / 2, 10, 30);
				}
				if (isNortheast)
				{
					JGraphicsUtil.drawArrow(g, sz.width - 14, 14, sz.width - 2, 2, 10, 30);
				}
				if (isNorthwest)
				{
					JGraphicsUtil.drawArrow(g, 14, 14, 2, 2, 10, 30);
				}
				if (isSoutheast)
				{
					JGraphicsUtil.drawArrow(g, sz.width - 14, sz.height - 14, sz.width - 2, sz.height - 2, 10, 30);
				}
				if (isSouthwest)
				{
					JGraphicsUtil.drawArrow(g, 14, sz.height - 14, 2, sz.height - 2, 10, 30);
				}

				if (AppPreferences.SHOW_TICK_RATE.Boolean)
				{
					string hz = outerInstance.tickCounter.TickRate;
					if (!string.ReferenceEquals(hz, null) && !hz.Equals(""))
					{
						g.setColor(TICK_RATE_COLOR);
						g.setFont(TICK_RATE_FONT);
						FontMetrics fm = g.getFontMetrics();
						int x = getWidth() - fm.stringWidth(hz) - 5;
						int y = fm.getAscent() + 5;
						g.drawString(hz, x, y);
					}
				}

				JGraphicsUtil.switchToWidth(g, 1);
				g.setColor(Color.Black);

			}

			internal virtual void paintString(JGraphics g, string msg)
			{
				Font old = g.getFont();
				g.setFont(old.deriveFont(Font.BOLD).deriveFont(18.0f));
				FontMetrics fm = g.getFontMetrics();
				int x = (getWidth() - fm.stringWidth(msg)) / 2;
				if (x < 0)
				{
					x = 0;
				}
				g.drawString(msg, x, getHeight() - 23);
				g.setFont(old);
				return;
			}
		}

		private Project proj;
		private Tool drag_tool;
		private Selection selection;
		private MouseMappings mappings;
		private CanvasPane canvasPane;
		private Bounds oldPreferredSize;
		private MyListener myListener;
		private MyViewport viewport;
		private MyProjectListener myProjectListener;
		private TickCounter tickCounter;

		private CanvasPaintThread paintThread;
		private CanvasPainter painter;
		private bool paintDirty = false; // only for within paintComponent
		private bool inPaint = false; // only for within paintComponent
		private object repaintLock = new object(); // for waitForRepaintDone

		public Canvas(Project proj)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.proj = proj;
			this.selection = new Selection(proj, this);
			this.painter = new CanvasPainter(this);
			this.oldPreferredSize = null;
			this.paintThread = new CanvasPaintThread(this);
			this.mappings = proj.Options.MouseMappings;
			this.canvasPane = null;
			this.tickCounter = new TickCounter();

			setBackground(Color.White);
			addMouseListener(myListener);
			addMouseMotionListener(myListener);
			addKeyListener(myListener);

			proj.addProjectListener(myProjectListener);
			proj.addLibraryListener(myProjectListener);
			proj.addCircuitListener(myProjectListener);
			proj.Simulator.addSimulatorListener(tickCounter);
			selection.addListener(myProjectListener);
			LocaleManager.addLocaleListener(this);

			AttributeSet options = proj.Options.AttributeSet;
			options.addAttributeListener(myProjectListener);
			AppPreferences.COMPONENT_TIPS.addPropertyChangeListener(myListener);
			AppPreferences.GATE_SHAPE.addPropertyChangeListener(myListener);
			AppPreferences.SHOW_TICK_RATE.addPropertyChangeListener(myListener);
			loadOptions(options);
			paintThread.Start();
		}

		public virtual void closeCanvas()
		{
			paintThread.requestStop();
		}

		private void loadOptions(AttributeSet options)
		{
			bool showTips = AppPreferences.COMPONENT_TIPS.Boolean;
			setToolTipText(showTips ? "" : null);

			proj.Simulator.removeSimulatorListener(myProjectListener);
			proj.Simulator.addSimulatorListener(myProjectListener);
		}

		public override void repaint()
		{
			if (inPaint)
			{
				paintDirty = true;
			}
			else
			{
				base.repaint();
			}
		}

		public virtual StringGetter ErrorMessage
		{
			get
			{
				return viewport.errorMessage;
			}
		}

		public virtual void setErrorMessage(StringGetter message)
		{
			viewport.setErrorMessage(message, null);
		}

		public virtual void setErrorMessage(StringGetter message, Color color)
		{
			viewport.setErrorMessage(message, color);
		}

		//
		// access methods
		//
		public virtual Circuit Circuit
		{
			get
			{
				return proj.CurrentCircuit;
			}
		}

		public virtual CircuitState CircuitState
		{
			get
			{
				return proj.CircuitState;
			}
		}

		public virtual Project Project
		{
			get
			{
				return proj;
			}
		}

		public virtual Selection Selection
		{
			get
			{
				return selection;
			}
		}

		internal virtual GridPainter GridPainter
		{
			get
			{
				return painter.GridPainter;
			}
		}

		internal virtual Tool DragTool
		{
			get
			{
				return drag_tool;
			}
		}

		internal virtual bool PopupMenuUp
		{
			get
			{
				return myListener.menu_on;
			}
		}

		//
		// JGraphics methods
		//
		internal virtual double ZoomFactor
		{
			get
			{
				CanvasPane pane = canvasPane;
				return pane == null ? 1.0 : pane.ZoomFactor;
			}
		}

		internal virtual Component HaloedComponent
		{
			get
			{
				return painter.HaloedComponent;
			}
		}

		internal virtual void setHaloedComponent(Circuit circ, Component comp)
		{
			painter.setHaloedComponent(circ, comp);
		}

		public virtual WireSet HighlightedWires
		{
			set
			{
				painter.HighlightedWires = value;
			}
		}

		public virtual void showPopupMenu(JPopupMenu menu, int x, int y)
		{
			double zoom = ZoomFactor;
			if (zoom != 1.0)
			{
				x = (int) (long)Math.Round(x * zoom, MidpointRounding.AwayFromZero);
				y = (int) (long)Math.Round(y * zoom, MidpointRounding.AwayFromZero);
			}
			myListener.menu_on = true;
			menu.addPopupMenuListener(myListener);
			menu.show(this, x, y);
		}

		private void completeAction()
		{
			computeSize(false);
			// TODO for SimulatorPrototype: proj.getSimulator().releaseUserEvents();
			proj.Simulator.requestPropagate();
			// repaint will occur after propagation completes
		}

		public virtual void computeSize(bool immediate)
		{
			Bounds bounds = proj.CurrentCircuit.Bounds;
			int width = bounds.X + bounds.Width + BOUNDS_BUFFER;
			int height = bounds.Y + bounds.Height + BOUNDS_BUFFER;
			Size dim;
			if (canvasPane == null)
			{
				dim = new Size(width, height);
			}
			else
			{
				dim = canvasPane.supportPreferredSize(width, height);
			}
			if (!immediate)
			{
				Bounds old = oldPreferredSize;
				if (old != null && Math.Abs(old.Width - dim.Width) < THRESH_SIZE_UPDATE && Math.Abs(old.Height - dim.Height) < THRESH_SIZE_UPDATE)
				{
					return;
				}
			}
			oldPreferredSize = Bounds.create(0, 0, dim.Width, dim.Height);
			setPreferredSize(dim);
			revalidate();
		}

		private void waitForRepaintDone()
		{
			lock (repaintLock)
			{
				try
				{
					while (inPaint)
					{
						Monitor.Wait(repaintLock);
					}
				}
				catch (InterruptedException)
				{
				}
			}
		}

		public override void paintComponent(JGraphics g)
		{
			inPaint = true;
			try
			{
				base.paintComponent(g);
				do
				{
					painter.paintContents(g, proj);
				} while (paintDirty);
				if (canvasPane == null)
				{
					viewport.paintContents(g);
				}
			}
			finally
			{
				inPaint = false;
				lock (repaintLock)
				{
					Monitor.PulseAll(repaintLock);
				}
			}
		}

		internal virtual bool ifPaintDirtyReset()
		{
			if (paintDirty)
			{
				paintDirty = false;
				return false;
			}
			else
			{
				return true;
			}
		}

		private void computeViewportContents()
		{
			HashSet<WidthIncompatibilityData> exceptions = proj.CurrentCircuit.WidthIncompatibilityData;
			if (exceptions == null || exceptions.Count == 0)
			{
				viewport.WidthMessage = null;
				return;
			}

			Rectangle viewableBase;
			Rectangle viewable;
			if (canvasPane != null)
			{
				viewableBase = canvasPane.getViewport().getViewRect();
			}
			else
			{
				Bounds bds = proj.CurrentCircuit.Bounds;
				viewableBase = new Rectangle(0, 0, bds.Width, bds.Height);
			}
			double zoom = ZoomFactor;
			if (zoom == 1.0)
			{
				viewable = viewableBase;
			}
			else
			{
				viewable = new Rectangle((int)(viewableBase.x / zoom), (int)(viewableBase.y / zoom), (int)(viewableBase.width / zoom), (int)(viewableBase.height / zoom));
			}

			viewport.WidthMessage = Strings.get("canvasWidthError") + (exceptions.Count == 1 ? "" : " (" + exceptions.Count + ")");
			foreach (WidthIncompatibilityData ex in exceptions)
			{
				// See whether any of the points are on the canvas.
				bool isWithin = false;
				for (int i = 0; i < ex.size(); i++)
				{
					Location p = ex.getPoint(i);
					int x = p.X;
					int y = p.Y;
					if (x >= viewable.x && x < viewable.x + viewable.width && y >= viewable.y && y < viewable.y + viewable.height)
					{
						isWithin = true;
						break;
					}
				}

				// If none are, insert an arrow.
				if (!isWithin)
				{
					Location p = ex.getPoint(0);
					int x = p.X;
					int y = p.Y;
					bool isWest = x < viewable.x;
					bool isEast = x >= viewable.x + viewable.width;
					bool isNorth = y < viewable.y;
					bool isSouth = y >= viewable.y + viewable.height;

					if (isNorth)
					{
						if (isEast)
						{
							viewport.Northeast = true;
						}
						else if (isWest)
						{
							viewport.Northwest = true;
						}
						else
						{
							viewport.North = true;
						}
					}
					else if (isSouth)
					{
						if (isEast)
						{
							viewport.Southeast = true;
						}
						else if (isWest)
						{
							viewport.Southwest = true;
						}
						else
						{
							viewport.South = true;
						}
					}
					else
					{
						if (isEast)
						{
							viewport.East = true;
						}
						else if (isWest)
						{
							viewport.West = true;
						}
					}
				}
			}
		}

		public override void repaint(Rectangle r)
		{
			double zoom = ZoomFactor;
			if (zoom == 1.0)
			{
				base.repaint(r);
			}
			else
			{
				this.repaint(r.X, r.Y, r.Width, r.Height);
			}
		}

		public override void repaint(int x, int y, int width, int height)
		{
			double zoom = ZoomFactor;
			if (zoom < 1.0)
			{
				int newX = (int) Math.Floor(x * zoom);
				int newY = (int) Math.Floor(y * zoom);
				width += x - newX;
				height += y - newY;
				x = newX;
				y = newY;
			}
			else if (zoom > 1.0)
			{
				int x1 = (int) Math.Ceiling((x + width) * zoom);
				int y1 = (int) Math.Ceiling((y + height) * zoom);
				width = x1 - x;
				height = y1 - y;
			}
			base.repaint(x, y, width, height);
		}

		public override string getToolTipText(MouseEvent @event)
		{
			bool showTips = AppPreferences.COMPONENT_TIPS.Boolean;
			if (showTips)
			{
				Canvas.snapToGrid(@event);
				Location loc = new Location(@event.getX(), @event.getY());
				ComponentUserEvent e = null;
				foreach (Component comp in Circuit.getAllContaining(loc))
				{
					object makerObj = comp.getFeature(typeof(ToolTipMaker));
					if (makerObj != null && makerObj is ToolTipMaker)
					{
						ToolTipMaker maker = (ToolTipMaker) makerObj;
						if (e == null)
						{
							e = new ComponentUserEvent(this, loc.X, loc.Y);
						}
						string ret = maker.getToolTip(e);
						if (!string.ReferenceEquals(ret, null))
						{
							unrepairMouseEvent(@event);
							return ret;
						}
					}
				}
			}
			return null;
		}

		protected internal override void processMouseEvent(MouseEvent e)
		{
			repairMouseEvent(e);
			base.processMouseEvent(e);
		}

		protected internal override void processMouseMotionEvent(MouseEvent e)
		{
			repairMouseEvent(e);
			base.processMouseMotionEvent(e);
		}

		private void repairMouseEvent(MouseEvent e)
		{
			double zoom = ZoomFactor;
			if (zoom != 1.0)
			{
				zoomEvent(e, zoom);
			}
		}

		private void unrepairMouseEvent(MouseEvent e)
		{
			double zoom = ZoomFactor;
			if (zoom != 1.0)
			{
				zoomEvent(e, 1.0 / zoom);
			}
		}

		private void zoomEvent(MouseEvent e, double zoom)
		{
			int oldx = e.getX();
			int oldy = e.getY();
			int newx = (int) (long)Math.Round(e.getX() / zoom, MidpointRounding.AwayFromZero);
			int newy = (int) (long)Math.Round(e.getY() / zoom, MidpointRounding.AwayFromZero);
			e.translatePoint(newx - oldx, newy - oldy);
		}

		//
		// CanvasPaneContents methods
		//
		public virtual CanvasPane CanvasPane
		{
			set
			{
				canvasPane = value;
				canvasPane.setViewport(viewport);
				viewport.setView(this);
				setOpaque(false);
				computeSize(true);
			}
		}

		public virtual void recomputeSize()
		{
			computeSize(true);
		}

		public virtual Size PreferredScrollableViewportSize
		{
			get
			{
				return getPreferredSize();
			}
		}

		public virtual int getScrollableBlockIncrement(Rectangle visibleRect, int orientation, int direction)
		{
			return canvasPane.supportScrollableBlockIncrement(visibleRect, orientation, direction);
		}

		public virtual bool ScrollableTracksViewportHeight
		{
			get
			{
				return false;
			}
		}

		public virtual bool ScrollableTracksViewportWidth
		{
			get
			{
				return false;
			}
		}

		public virtual int getScrollableUnitIncrement(Rectangle visibleRect, int orientation, int direction)
		{
			return canvasPane.supportScrollableUnitIncrement(visibleRect, orientation, direction);
		}

		//
		// static methods
		//
		public static int snapXToGrid(int x)
		{
			if (x < 0)
			{
				return -((-x + 5) / 10) * 10;
			}
			else
			{
				return ((x + 5) / 10) * 10;
			}
		}

		public static int snapYToGrid(int y)
		{
			if (y < 0)
			{
				return -((-y + 5) / 10) * 10;
			}
			else
			{
				return ((y + 5) / 10) * 10;
			}
		}

		public static void snapToGrid(MouseEvent e)
		{
			int old_x = e.getX();
			int old_y = e.getY();
			int new_x = snapXToGrid(old_x);
			int new_y = snapYToGrid(old_y);
			e.translatePoint(new_x - old_x, new_y - old_y);
		}

		public virtual void localeChanged()
		{
			paintThread.requestRepaint();
		}
	}

}
