// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.proj
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitListener = logisim.circuit.CircuitListener;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using CircuitState = logisim.circuit.CircuitState;
	using Simulator = logisim.circuit.Simulator;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Loader = logisim.file.Loader;
	using LogisimFile = logisim.file.LogisimFile;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using Options = logisim.file.Options;
	using LogFrame = logisim.gui.log.LogFrame;
	using Canvas = logisim.gui.main.Canvas;
	using Frame = logisim.gui.main.Frame;
	using Selection = logisim.gui.main.Selection;
	using SelectionActions = logisim.gui.main.SelectionActions;
	using OptionsFrame = logisim.gui.opts.OptionsFrame;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;
	using logisim.util;
	using JFileChoosers = logisim.util.JFileChoosers;

	public class Project
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private const int MAX_UNDO_SIZE = 64;

		private class ActionData
		{
			internal CircuitState circuitState;
			internal Action action;

			public ActionData(CircuitState circuitState, Action action)
			{
				this.circuitState = circuitState;
				this.action = action;
			}
		}

		private class MyListener : Selection.Listener, LibraryListener
		{
			private readonly Project outerInstance;

			public MyListener(Project outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void selectionChanged(Selection.Event e)
			{
				outerInstance.fireEvent(ProjectEvent.ACTION_SELECTION, e.Source);
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				int action = @event.Action;
				if (action == LibraryEvent.REMOVE_LIBRARY)
				{
					Library unloaded = (Library) @event.Data;
					if (outerInstance.tool != null && unloaded.containsFromSource(outerInstance.tool))
					{
						outerInstance.Tool = null;
					}
				}
				else if (action == LibraryEvent.REMOVE_TOOL)
				{
					object data = @event.Data;
					if (data is AddTool)
					{
						object factory = ((AddTool) data).Factory;
						if (factory is SubcircuitFactory)
						{
							SubcircuitFactory fact = (SubcircuitFactory) factory;
							if (fact.Subcircuit == outerInstance.CurrentCircuit)
							{
								outerInstance.CurrentCircuit = outerInstance.file.MainCircuit;
							}
						}
					}
				}
			}
		}

		private Simulator simulator = new Simulator();
		private LogisimFile file;
		private CircuitState circuitState;
		private Dictionary<Circuit, CircuitState> stateMap = new Dictionary<Circuit, CircuitState>();
		private Frame frame = null;
		private OptionsFrame optionsFrame = null;
		private LogFrame logFrame = null;
		private Tool tool = null;
		private LinkedList<ActionData> undoLog = new LinkedList<ActionData>();
		private int undoMods = 0;
		private EventSourceWeakSupport<ProjectListener> projectListeners = new EventSourceWeakSupport<ProjectListener>();
		private EventSourceWeakSupport<LibraryListener> fileListeners = new EventSourceWeakSupport<LibraryListener>();
		private EventSourceWeakSupport<CircuitListener> circuitListeners = new EventSourceWeakSupport<CircuitListener>();
		private Dependencies depends;
		private MyListener myListener;
		private bool startupScreen = false;

		public Project(LogisimFile file)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			addLibraryListener(myListener);
			LogisimFile = file;
		}

		public virtual Frame Frame
		{
			set
			{
				if (frame == value)
				{
					return;
				}
				Frame oldValue = frame;
				frame = value;
				Projects.windowCreated(this, oldValue, value);
				value.Canvas.Selection.addListener(myListener);
			}
			get
			{
				return frame;
			}
		}

		//
		// access methods
		//
		public virtual LogisimFile LogisimFile
		{
			get
			{
				return file;
			}
			set
			{
				LogisimFile old = this.file;
				if (old != null)
				{
					foreach (LibraryListener l in fileListeners)
					{
						old.removeLibraryListener(l);
					}
				}
				file = value;
				stateMap.Clear();
				depends = new Dependencies(file);
				undoLog.Clear();
				undoMods = 0;
				fireEvent(ProjectEvent.ACTION_SET_FILE, old, file);
				CurrentCircuit = file.MainCircuit;
				if (file != null)
				{
					foreach (LibraryListener l in fileListeners)
					{
						file.addLibraryListener(l);
					}
				}
				file.Dirty = true; // toggle it so that everybody hears the file is fresh
				file.Dirty = false;
			}
		}

		public virtual Simulator Simulator
		{
			get
			{
				return simulator;
			}
		}

		public virtual Options Options
		{
			get
			{
				return file.Options;
			}
		}

		public virtual Dependencies Dependencies
		{
			get
			{
				return depends;
			}
		}


		public virtual OptionsFrame getOptionsFrame(bool create)
		{
			if (optionsFrame == null || optionsFrame.LogisimFile != file)
			{
				if (create)
				{
					optionsFrame = new OptionsFrame(this);
				}
				else
				{
					optionsFrame = null;
				}
			}
			return optionsFrame;
		}

		public virtual LogFrame getLogFrame(bool create)
		{
			if (logFrame == null)
			{
				if (create)
				{
					logFrame = new LogFrame(this);
				}
			}
			return logFrame;
		}

		public virtual Circuit CurrentCircuit
		{
			get
			{
				return circuitState == null ? null : circuitState.Circuit;
			}
			set
			{
				CircuitState circState = stateMap[value];
				if (circState == null)
				{
					circState = new CircuitState(this, value);
				}
				CircuitState = circState;
			}
		}

		public virtual CircuitState CircuitState
		{
			get
			{
				return circuitState;
			}
			set
			{
				if (value == null || circuitState == value)
				{
					return;
				}
    
				CircuitState old = circuitState;
				Circuit oldCircuit = old == null ? null : old.Circuit;
				Circuit newCircuit = value.Circuit;
				bool circuitChanged = old == null || oldCircuit != newCircuit;
				if (circuitChanged)
				{
					Canvas canvas = frame == null ? null : frame.Canvas;
					if (canvas != null)
					{
						if (tool != null)
						{
							tool.deselect(canvas);
						}
						Selection selection = canvas.Selection;
						if (selection != null)
						{
							Action act = SelectionActions.dropAll(selection);
							if (act != null)
							{
								doAction(act);
							}
						}
						if (tool != null)
						{
							tool.select(canvas);
						}
					}
					if (oldCircuit != null)
					{
						foreach (CircuitListener l in circuitListeners)
						{
							oldCircuit.removeCircuitListener(l);
						}
					}
				}
				circuitState = value;
				stateMap[circuitState.Circuit] = circuitState;
				simulator.CircuitState = circuitState;
				if (circuitChanged)
				{
					fireEvent(ProjectEvent.ACTION_SET_CURRENT, oldCircuit, newCircuit);
					if (newCircuit != null)
					{
						foreach (CircuitListener l in circuitListeners)
						{
							newCircuit.addCircuitListener(l);
						}
					}
				}
				fireEvent(ProjectEvent.ACTION_SET_STATE, old, circuitState);
			}
		}

		public virtual CircuitState getCircuitState(Circuit circuit)
		{
			if (circuitState != null && circuitState.Circuit == circuit)
			{
				return circuitState;
			}
			else
			{
				CircuitState ret = stateMap[circuit];
				if (ret == null)
				{
					ret = new CircuitState(this, circuit);
					stateMap[circuit] = ret;
				}
				return ret;
			}
		}

		public virtual Action LastAction
		{
			get
			{
				if (undoLog.Count == 0)
				{
					return null;
				}
				else
				{
					return undoLog.Last.Value.action;
				}
			}
		}

		public virtual Tool Tool
		{
			get
			{
				return tool;
			}
			set
			{
				if (tool == value)
				{
					return;
				}
				Tool old = tool;
				Canvas canvas = frame.Canvas;
				if (old != null)
				{
					old.deselect(canvas);
				}
				Selection selection = canvas.Selection;
				if (selection != null && !selection.Empty)
				{
					Circuit circuit = canvas.Circuit;
					CircuitMutation xn = new CircuitMutation(circuit);
					if (value == null)
					{
						Action act = SelectionActions.dropAll(selection);
						if (act != null)
						{
							doAction(act);
						}
					}
					else if (!Options.MouseMappings.containsSelectTool())
					{
						Action act = SelectionActions.dropAll(selection);
						if (act != null)
						{
							doAction(act);
						}
					}
					if (!xn.Empty)
					{
						doAction(xn.toAction(null));
					}
				}
				startupScreen = false;
				tool = value;
				if (tool != null)
				{
					tool.select(frame.Canvas);
				}
				fireEvent(ProjectEvent.ACTION_SET_TOOL, old, tool);
			}
		}

		public virtual Selection Selection
		{
			get
			{
				if (frame == null)
				{
					return null;
				}
				Canvas canvas = frame.Canvas;
				if (canvas == null)
				{
					return null;
				}
				return canvas.Selection;
			}
		}

		public virtual bool FileDirty
		{
			get
			{
				return undoMods != 0;
			}
		}

		public virtual JFileChooser createChooser()
		{
			if (file == null)
			{
				return JFileChoosers.create();
			}
			Loader loader = file.Loader;
			return loader == null ? JFileChoosers.create() : loader.createChooser();
		}

		//
		// Listener methods
		//
		public virtual void addProjectListener(ProjectListener what)
		{
			projectListeners.add(what);
		}

		public virtual void removeProjectListener(ProjectListener what)
		{
			projectListeners.remove(what);
		}

		public virtual void addLibraryListener(LibraryListener value)
		{
			fileListeners.add(value);
			if (file != null)
			{
				file.addLibraryListener(value);
			}
		}

		public virtual void removeLibraryListener(LibraryListener value)
		{
			fileListeners.remove(value);
			if (file != null)
			{
				file.removeLibraryListener(value);
			}
		}

		public virtual void addCircuitListener(CircuitListener value)
		{
			circuitListeners.add(value);
			Circuit current = CurrentCircuit;
			if (current != null)
			{
				current.addCircuitListener(value);
			}
		}

		public virtual void removeCircuitListener(CircuitListener value)
		{
			circuitListeners.remove(value);
			Circuit current = CurrentCircuit;
			if (current != null)
			{
				current.removeCircuitListener(value);
			}
		}

		private void fireEvent(int action, object old, object data)
		{
			fireEvent(new ProjectEvent(action, this, old, data));
		}

		private void fireEvent(int action, object data)
		{
			fireEvent(new ProjectEvent(action, this, data));
		}

		private void fireEvent(ProjectEvent @event)
		{
			foreach (ProjectListener l in projectListeners)
			{
				l.projectChanged(@event);
			}
		}

		// We track whether this project is the empty project opened
		// at startup by default, because we want to close it
		// immediately as another project is opened, if there
		// haven't been any changes to it.
		public virtual bool StartupScreen
		{
			get
			{
				return startupScreen;
			}
			set
			{
				startupScreen = value;
			}
		}

		public virtual bool confirmClose(string title)
		{
			return frame.confirmClose(title);
		}






		public virtual void doAction(Action act)
		{
			if (act == null)
			{
				return;
			}
			Action toAdd = act;
			startupScreen = false;
			if (undoLog.Count > 0 && act.shouldAppendTo(LastAction))
			{
				ActionData firstData = undoLog.RemoveLast();
				Action first = firstData.action;
				if (first.Modification)
				{
					--undoMods;
				}
				toAdd = first.append(act);
				if (toAdd != null)
				{
					undoLog.AddLast(new ActionData(circuitState, toAdd));
					if (toAdd.Modification)
					{
						++undoMods;
					}
				}
				fireEvent(new ProjectEvent(ProjectEvent.ACTION_START, this, act));
				act.doIt(this);
				file.Dirty = FileDirty;
				fireEvent(new ProjectEvent(ProjectEvent.ACTION_COMPLETE, this, act));
				fireEvent(new ProjectEvent(ProjectEvent.ACTION_MERGE, this, first, toAdd));
				return;
			}
			undoLog.AddLast(new ActionData(circuitState, toAdd));
			fireEvent(new ProjectEvent(ProjectEvent.ACTION_START, this, act));
			act.doIt(this);
			while (undoLog.Count > MAX_UNDO_SIZE)
			{
				undoLog.RemoveFirst();
			}
			if (toAdd.Modification)
			{
				++undoMods;
			}
			file.Dirty = FileDirty;
			fireEvent(new ProjectEvent(ProjectEvent.ACTION_COMPLETE, this, act));
		}

		public virtual void undoAction()
		{
			if (undoLog != null && undoLog.Count > 0)
			{
				ActionData data = undoLog.RemoveLast();
				CircuitState = data.circuitState;
				Action action = data.action;
				if (action.Modification)
				{
					--undoMods;
				}
				fireEvent(new ProjectEvent(ProjectEvent.UNDO_START, this, action));
				action.undo(this);
				file.Dirty = FileDirty;
				fireEvent(new ProjectEvent(ProjectEvent.UNDO_COMPLETE, this, action));
			}
		}

		public virtual void setFileAsClean()
		{
			undoMods = 0;
			file.Dirty = FileDirty;
		}

		public virtual void repaintCanvas()
		{
			// for actions that ought not be logged (i.e., those that
			// change nothing, except perhaps the current values within
			// the circuit)
			fireEvent(new ProjectEvent(ProjectEvent.REPAINT_REQUEST, this, null));
		}
	}

}
