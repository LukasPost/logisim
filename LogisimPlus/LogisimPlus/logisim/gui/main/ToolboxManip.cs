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
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryEventSource = logisim.file.LibraryEventSource;
	using LibraryListener = logisim.file.LibraryListener;
	using LogisimFile = logisim.file.LogisimFile;
	using LogisimFileActions = logisim.file.LogisimFileActions;
	using AttrTableModel = logisim.gui.generic.AttrTableModel;
	using ProjectCircuitActions = logisim.gui.menu.ProjectCircuitActions;
	using ProjectLibraryActions = logisim.gui.menu.ProjectLibraryActions;
	using Popups = logisim.gui.menu.Popups;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	internal class ToolboxManip : ProjectExplorer.Listener
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class MyListener : ProjectListener, LibraryListener, AttributeListener
		{
			private readonly ToolboxManip outerInstance;

			public MyListener(ToolboxManip outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal LogisimFile curFile = null;

			public virtual void projectChanged(ProjectEvent @event)
			{
				int action = @event.Action;
				if (action == ProjectEvent.ACTION_SET_FILE)
				{
					setFile((LogisimFile) @event.OldData, (LogisimFile) @event.Data);
					outerInstance.explorer.repaint();
				}
			}

			internal virtual void setFile(LogisimFile oldFile, LogisimFile newFile)
			{
				if (oldFile != null)
				{
					removeLibrary(oldFile);
					foreach (Library lib in oldFile.Libraries)
					{
						removeLibrary(lib);
					}
				}
				curFile = newFile;
				if (newFile != null)
				{
					addLibrary(newFile);
					foreach (Library lib in newFile.Libraries)
					{
						addLibrary(lib);
					}
				}
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				int action = @event.Action;
				if (action == LibraryEvent.ADD_LIBRARY)
				{
					if (@event.Source == curFile)
					{
						addLibrary((Library) @event.Data);
					}
				}
				else if (action == LibraryEvent.REMOVE_LIBRARY)
				{
					if (@event.Source == curFile)
					{
						removeLibrary((Library) @event.Data);
					}
				}
				else if (action == LibraryEvent.ADD_TOOL)
				{
					Tool tool = (Tool) @event.Data;
					AttributeSet attrs = tool.AttributeSet;
					if (attrs != null)
					{
						attrs.addAttributeListener(this);
					}
				}
				else if (action == LibraryEvent.REMOVE_TOOL)
				{
					Tool tool = (Tool) @event.Data;
					AttributeSet attrs = tool.AttributeSet;
					if (attrs != null)
					{
						attrs.removeAttributeListener(this);
					}
				}
				outerInstance.explorer.repaint();
			}

			internal virtual void addLibrary(Library lib)
			{
				if (lib is LibraryEventSource)
				{
					((LibraryEventSource) lib).addLibraryListener(this);
				}
				foreach (Tool tool in lib.Tools)
				{
					AttributeSet attrs = tool.AttributeSet;
					if (attrs != null)
					{
						attrs.addAttributeListener(this);
					}
				}
			}

			internal virtual void removeLibrary(Library lib)
			{
				if (lib is LibraryEventSource)
				{
					((LibraryEventSource) lib).removeLibraryListener(this);
				}
				foreach (Tool tool in lib.Tools)
				{
					AttributeSet attrs = tool.AttributeSet;
					if (attrs != null)
					{
						attrs.removeAttributeListener(this);
					}
				}
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				outerInstance.explorer.repaint();
			}

		}

		private Project proj;
		private ProjectExplorer explorer;
		private MyListener myListener;
		private Tool lastSelected = null;

		internal ToolboxManip(Project proj, ProjectExplorer explorer)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.proj = proj;
			this.explorer = explorer;
			proj.addProjectListener(myListener);
			myListener.setFile(null, proj.LogisimFile);
		}

		public virtual void selectionChanged(ProjectExplorer.Event @event)
		{
			object selected = @event.Target;
			if (selected is Tool)
			{
				if (selected is AddTool)
				{
					AddTool addTool = (AddTool) selected;
					ComponentFactory source = addTool.Factory;
					if (source is SubcircuitFactory)
					{
						SubcircuitFactory circFact = (SubcircuitFactory) source;
						Circuit circ = circFact.Subcircuit;
						if (proj.CurrentCircuit == circ)
						{
							AttrTableModel m = new AttrTableCircuitModel(proj, circ);
							proj.Frame.setAttrTableModel(m);
							return;
						}
					}
				}

				lastSelected = proj.Tool;
				Tool tool = (Tool) selected;
				proj.Tool = tool;
				proj.Frame.viewAttributes(tool);
			}
		}

		public virtual void doubleClicked(ProjectExplorer.Event @event)
		{
			object clicked = @event.Target;
			if (clicked is AddTool)
			{
				AddTool tool = (AddTool) clicked;
				ComponentFactory source = tool.Factory;
				if (source is SubcircuitFactory)
				{
					SubcircuitFactory circFact = (SubcircuitFactory) source;
					proj.CurrentCircuit = circFact.Subcircuit;
					proj.Frame.setEditorView(Frame.EDIT_LAYOUT);
					if (lastSelected != null)
					{
						proj.Tool = lastSelected;
					}
				}
			}
		}

		public virtual void moveRequested(ProjectExplorer.Event @event, AddTool dragged, AddTool target)
		{
			LogisimFile file = proj.LogisimFile;
			int draggedIndex = file.Tools.IndexOf(dragged);
			int targetIndex = file.Tools.IndexOf(target);
			if (targetIndex > draggedIndex)
			{
				targetIndex++;
			}
			proj.doAction(LogisimFileActions.moveCircuit(dragged, targetIndex));
		}

		public virtual void deleteRequested(ProjectExplorer.Event @event)
		{
			object request = @event.Target;
			if (request is Library)
			{
				ProjectLibraryActions.doUnloadLibrary(proj, (Library) request);
			}
			else if (request is AddTool)
			{
				ComponentFactory factory = ((AddTool) request).Factory;
				if (factory is SubcircuitFactory)
				{
					SubcircuitFactory circFact = (SubcircuitFactory) factory;
					ProjectCircuitActions.doRemoveCircuit(proj, circFact.Subcircuit);
				}
			}
		}

		public virtual JPopupMenu menuRequested(ProjectExplorer.Event @event)
		{
			object clicked = @event.Target;
			if (clicked is AddTool tool)
			{
				ComponentFactory source = tool.Factory;
				if (source is SubcircuitFactory)
				{
					Circuit circ = ((SubcircuitFactory) source).Subcircuit;
					return Popups.forCircuit(proj, circ);
				}
				else
				{
					return null;
				}
			}
			else if (clicked is Tool)
			{
				return null;
			}
			else if (clicked == proj.LogisimFile)
			{
				return Popups.forProject(proj);
			}
			else if (clicked is Library)
			{
				bool is_top = @event.TreePath.getPathCount() <= 2;
				return Popups.forLibrary(proj, (Library) clicked, is_top);
			}
			else
			{
				return null;
			}
		}

	}

}
