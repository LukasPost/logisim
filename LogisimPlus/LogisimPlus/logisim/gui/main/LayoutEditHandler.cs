// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using Circuit = logisim.circuit.Circuit;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using Base = logisim.std.@base.Base;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class LayoutEditHandler : EditHandler, ProjectListener, LibraryListener, PropertyChangeListener
	{
		private Frame frame;

		internal LayoutEditHandler(Frame frame)
		{
			this.frame = frame;

			Project proj = frame.Project;
			Clipboard.addPropertyChangeListener(Clipboard.contentsProperty, this);
			proj.addProjectListener(this);
			proj.addLibraryListener(this);
		}

		public override void computeEnabled()
		{
			Project proj = frame.Project;
			if (proj == null)
			{
				return;
			}
			Selection sel = proj.Selection;
			bool selEmpty = sel == null || sel.Empty;
			bool canChange = proj.LogisimFile.contains(proj.CurrentCircuit);

			bool selectAvailable = false;
			foreach (Library lib in proj.LogisimFile.Libraries)
			{
				if (lib is Base)
				{
					selectAvailable = true;
				}
			}

			setEnabled(LogisimMenuBar.CUT, !selEmpty && selectAvailable && canChange);
			setEnabled(LogisimMenuBar.COPY, !selEmpty && selectAvailable);
			setEnabled(LogisimMenuBar.PASTE, selectAvailable && canChange && !Clipboard.Empty);
			setEnabled(LogisimMenuBar.DELETE, !selEmpty && selectAvailable && canChange);
			setEnabled(LogisimMenuBar.DUPLICATE, !selEmpty && selectAvailable && canChange);
			setEnabled(LogisimMenuBar.SELECT_ALL, selectAvailable);
			setEnabled(LogisimMenuBar.RAISE, false);
			setEnabled(LogisimMenuBar.LOWER, false);
			setEnabled(LogisimMenuBar.RAISE_TOP, false);
			setEnabled(LogisimMenuBar.LOWER_BOTTOM, false);
			setEnabled(LogisimMenuBar.ADD_CONTROL, false);
			setEnabled(LogisimMenuBar.REMOVE_CONTROL, false);
		}

		public override void cut()
		{
			Project proj = frame.Project;
			Selection sel = frame.Canvas.Selection;
			proj.doAction(SelectionActions.cut(sel));
		}

		public override void copy()
		{
			Project proj = frame.Project;
			Selection sel = frame.Canvas.Selection;
			proj.doAction(SelectionActions.copy(sel));
		}

		public override void paste()
		{
			Project proj = frame.Project;
			Selection sel = frame.Canvas.Selection;
			selectSelectTool(proj);
			Action action = SelectionActions.pasteMaybe(proj, sel);
			if (action != null)
			{
				proj.doAction(action);
			}
		}

		public override void delete()
		{
			Project proj = frame.Project;
			Selection sel = frame.Canvas.Selection;
			proj.doAction(SelectionActions.clear(sel));
		}

		public override void duplicate()
		{
			Project proj = frame.Project;
			Selection sel = frame.Canvas.Selection;
			proj.doAction(SelectionActions.duplicate(sel));
		}

		public override void selectAll()
		{
			Project proj = frame.Project;
			Selection sel = frame.Canvas.Selection;
			selectSelectTool(proj);
			Circuit circ = proj.CurrentCircuit;
			sel.addAll(circ.Wires);
			sel.addAll(circ.NonWires);
			proj.repaintCanvas();
		}

		public override void raise()
		{
			; // not yet supported in layout mode
		}

		public override void lower()
		{
			; // not yet supported in layout mode
		}

		public override void raiseTop()
		{
			; // not yet supported in layout mode
		}

		public override void lowerBottom()
		{
			; // not yet supported in layout mode
		}

		public override void addControlPoint()
		{
			; // not yet supported in layout mode
		}

		public override void removeControlPoint()
		{
			; // not yet supported in layout mode
		}

		private void selectSelectTool(Project proj)
		{
			foreach (Library sub in proj.LogisimFile.Libraries)
			{
				if (sub is Base)
				{
					Base @base = (Base) sub;
					Tool tool = @base.getTool("Edit Tool");
					if (tool != null)
					{
						proj.Tool = tool;
					}
				}
			}
		}

		public virtual void projectChanged(ProjectEvent e)
		{
			int action = e.Action;
			if (action == ProjectEvent.ACTION_SET_FILE)
			{
				computeEnabled();
			}
			else if (action == ProjectEvent.ACTION_SET_CURRENT)
			{
				computeEnabled();
			}
			else if (action == ProjectEvent.ACTION_SELECTION)
			{
				computeEnabled();
			}
		}

		public virtual void libraryChanged(LibraryEvent e)
		{
			int action = e.Action;
			if (action == LibraryEvent.ADD_LIBRARY)
			{
				computeEnabled();
			}
			else if (action == LibraryEvent.REMOVE_LIBRARY)
			{
				computeEnabled();
			}
		}

		public virtual void propertyChange(PropertyChangeEvent @event)
		{
			if (@event.getPropertyName().Equals(Clipboard.contentsProperty))
			{
				computeEnabled();
			}
		}
	}

}
