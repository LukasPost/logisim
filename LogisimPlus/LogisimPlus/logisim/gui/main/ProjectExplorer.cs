// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{


	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using LibraryEventSource = logisim.file.LibraryEventSource;
	using LogisimFile = logisim.file.LogisimFile;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;
	using JTreeDragController = logisim.util.JTreeDragController;
	using JTreeUtil = logisim.util.JTreeUtil;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;

	public class ProjectExplorer : JTree, LocaleListener
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
			subListener = new SubListener(this);
			model = new MyModel(this);
			renderer = new MyCellRenderer(this);
			deleteAction = new DeleteAction(this);
		}

		private const string DIRTY_MARKER = "*";

		internal static readonly Color MAGNIFYING_INTERIOR = Color.FromArgb(255, 200, 200, 255, 64);

		public class Event
		{
			internal TreePath path;

			internal Event(TreePath path)
			{
				this.path = path;
			}

			public virtual TreePath TreePath
			{
				get
				{
					return path;
				}
			}

			public virtual object Target
			{
				get
				{
					return path == null ? null : path.getLastPathComponent();
				}
			}
		}

		public interface Listener
		{
			void selectionChanged(Event @event);

			void doubleClicked(Event @event);

			void moveRequested(Event @event, AddTool dragged, AddTool target);

			void deleteRequested(Event @event);

			JPopupMenu menuRequested(Event @event);
		}

		private class MyModel : TreeModel
		{
			private readonly ProjectExplorer outerInstance;

			public MyModel(ProjectExplorer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal List<TreeModelListener> listeners = new List<TreeModelListener>();

			public virtual void addTreeModelListener(TreeModelListener l)
			{
				listeners.Add(l);
			}

			public virtual void removeTreeModelListener(TreeModelListener l)
			{
				listeners.Remove(l);
			}

			public virtual object Root
			{
				get
				{
					return outerInstance.proj.LogisimFile;
				}
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<?> getChildren(Object parent)
			internal virtual List<object> getChildren(object parent)
			{
				if (parent == outerInstance.proj.LogisimFile)
				{
					return ((Library) parent).Elements;
				}
				else if (parent is Library)
				{
					return ((Library) parent).Tools;
				}
				else
				{
					return Collections.EMPTY_LIST;
				}
			}

			public virtual object getChild(object parent, int index)
			{
				return getChildren(parent)[index];
			}

			public virtual int getChildCount(object parent)
			{
				return getChildren(parent).Count;
			}

			public virtual int getIndexOfChild(object parent, object query)
			{
				if (parent == null || query == null)
				{
					return -1;
				}
				int index = -1;
				foreach (object child in getChildren(parent))
				{
					index++;
					if (child == query)
					{
						return index;
					}
				}
				return -1;
			}

			public virtual bool isLeaf(object node)
			{
				return node != outerInstance.proj && !(node is Library);
			}

			public virtual void valueForPathChanged(TreePath path, object value)
			{
				TreeModelEvent e = new TreeModelEvent(outerInstance, path);
				fireNodesChanged(Collections.singletonList(e));
			}

			internal virtual void fireNodesChanged(List<TreeModelEvent> events)
			{
				foreach (TreeModelEvent e in events)
				{
					foreach (TreeModelListener l in listeners)
					{
						l.treeNodesChanged(e);
					}
				}
			}

			internal virtual void fireStructureChanged()
			{
				TreeModelEvent e = new TreeModelEvent(outerInstance, new object[] {outerInstance.model.Root});
				foreach (TreeModelListener l in listeners)
				{
					l.treeStructureChanged(e);
				}
				outerInstance.repaint();
			}

			internal virtual List<TreeModelEvent> findPaths(object value)
			{
				List<TreeModelEvent> ret = new List<TreeModelEvent>();
				List<object> stack = new List<object>();
				findPathsSub(value, Root, stack, ret);
				return ret;
			}

			internal virtual void findPathsSub(object value, object node, List<object> stack, List<TreeModelEvent> paths)
			{
				stack.Add(node);
				if (node == value)
				{
					TreePath path = new TreePath(stack.ToArray());
					paths.Add(new TreeModelEvent(outerInstance, path));
				}
				foreach (object child in getChildren(node))
				{
					findPathsSub(value, child, stack, paths);
				}
				stack.RemoveAt(stack.Count - 1);
			}

			internal virtual List<TreeModelEvent> findPathsForTools(Library value)
			{
				List<TreeModelEvent> ret = new List<TreeModelEvent>();
				List<object> stack = new List<object>();
				findPathsForToolsSub(value, Root, stack, ret);
				return ret;
			}

			internal virtual void findPathsForToolsSub(Library value, object node, List<object> stack, List<TreeModelEvent> paths)
			{
				stack.Add(node);
				if (node == value)
				{
					TreePath path = new TreePath(stack.ToArray());
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<? extends logisim.tools.Tool> toolList = value.getTools();
					List<Tool> toolList = value.Tools;
					int[] indices = new int[toolList.Count];
					object[] tools = new object[indices.Length];
					for (int i = 0; i < indices.Length; i++)
					{
						indices[i] = i;
						tools[i] = toolList[i];
					}
					paths.Add(new TreeModelEvent(outerInstance, path, indices, tools));
				}
				foreach (object child in getChildren(node))
				{
					findPathsForToolsSub(value, child, stack, paths);
				}
				stack.RemoveAt(stack.Count - 1);
			}
		}

		private class ToolIcon : Icon
		{
			private readonly ProjectExplorer outerInstance;

			internal Tool tool;
			internal Circuit circ = null;

			internal ToolIcon(ProjectExplorer outerInstance, Tool tool)
			{
				this.outerInstance = outerInstance;
				this.tool = tool;
				if (tool is AddTool)
				{
					ComponentFactory fact = ((AddTool) tool).getFactory(false);
					if (fact is SubcircuitFactory)
					{
						circ = ((SubcircuitFactory) fact).Subcircuit;
					}
				}
			}

			public virtual int IconHeight
			{
				get
				{
					return 20;
				}
			}

			public virtual int IconWidth
			{
				get
				{
					return 20;
				}
			}

			public virtual void paintIcon(JComponent c, JGraphics g, int x, int y)
			{
				// draw halo if appropriate
				if (tool == outerInstance.haloedTool && AppPreferences.ATTRIBUTE_HALO.Boolean)
				{
					g.setColor(Canvas.HALO_COLOR);
					g.fillRoundRect(x, y, 20, 20, 10, 10);
					g.setColor(Color.Black);
				}

				// draw tool icon
				JGraphics gIcon = g.create();
				ComponentDrawContext context = new ComponentDrawContext(outerInstance, null, null, g, gIcon);
				tool.paintIcon(context, x, y);
				gIcon.dispose();

				// draw magnifying glass if appropriate
				if (circ == outerInstance.proj.CurrentCircuit)
				{
					int tx = x + 13;
					int ty = y + 13;
					int[] xp = new int[] {tx - 1, x + 18, x + 20, tx + 1};
					int[] yp = new int[] {ty + 1, y + 20, y + 18, ty - 1};
					g.setColor(MAGNIFYING_INTERIOR);
					g.fillOval(x + 5, y + 5, 10, 10);
					g.setColor(Color.Black);
					g.drawOval(x + 5, y + 5, 10, 10);
					g.fillPolygon(xp, yp, xp.Length);
				}
			}
		}

		private class MyCellRenderer : DefaultTreeCellRenderer
		{
			private readonly ProjectExplorer outerInstance;

			public MyCellRenderer(ProjectExplorer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override java.awt.Component getTreeCellRendererComponent(JTree tree, object value, bool selected, bool expanded, bool leaf, int row, bool hasFocus)
			{
				java.awt.Component ret;
				ret = base.getTreeCellRendererComponent(tree, value, selected, expanded, leaf, row, hasFocus);

				if (ret is JComponent)
				{
					JComponent comp = (JComponent) ret;
					comp.setToolTipText(null);
				}
				if (value is Tool)
				{
					Tool tool = (Tool) value;
					if (ret is JLabel)
					{
						((JLabel) ret).setText(tool.DisplayName);
						((JLabel) ret).setIcon(new ToolIcon(outerInstance, tool));
						((JLabel) ret).setToolTipText(tool.Description);
					}
				}
				else if (value is Library)
				{
					if (ret is JLabel)
					{
						Library lib = (Library) value;
						string text = lib.DisplayName;
						if (lib.Dirty)
						{
							text += DIRTY_MARKER;
						}
						((JLabel) ret).setText(text);
					}
				}
				return ret;
			}
		}

		private class MySelectionModel : DefaultTreeSelectionModel
		{
			private readonly ProjectExplorer outerInstance;

			public MySelectionModel(ProjectExplorer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void addSelectionPath(TreePath path)
			{
				if (isPathValid(path))
				{
					base.addSelectionPath(path);
				}
			}

			public override TreePath SelectionPath
			{
				set
				{
					if (isPathValid(value))
					{
						base.setSelectionPath(value);
					}
				}
			}

			public override void addSelectionPaths(TreePath[] paths)
			{
				paths = getValidPaths(paths);
				if (paths != null)
				{
					base.addSelectionPaths(paths);
				}
			}

			public override TreePath[] SelectionPaths
			{
				set
				{
					value = getValidPaths(value);
					if (value != null)
					{
						base.setSelectionPaths(value);
					}
				}
			}

			internal virtual TreePath[] getValidPaths(TreePath[] paths)
			{
				int count = 0;
				for (int i = 0; i < paths.Length; i++)
				{
					if (isPathValid(paths[i]))
					{
						++count;
					}
				}
				if (count == 0)
				{
					return null;
				}
				else if (count == paths.Length)
				{
					return paths;
				}
				else
				{
					TreePath[] ret = new TreePath[count];
					int j = 0;
					for (int i = 0; i < paths.Length; i++)
					{
						if (isPathValid(paths[i]))
						{
							ret[j++] = paths[i];
						}
					}
					return ret;
				}
			}

			internal virtual bool isPathValid(TreePath path)
			{
				if (path == null || path.getPathCount() > 3)
				{
					return false;
				}
				object last = path.getLastPathComponent();
				return last is Tool;
			}
		}

		private class DragController : JTreeDragController
		{
			private readonly ProjectExplorer outerInstance;

			public DragController(ProjectExplorer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual bool canPerformAction(JTree targetTree, object draggedNode, int action, Point location)
			{
				TreePath pathTarget = targetTree.getPathForLocation(location.x, location.y);
				if (pathTarget == null)
				{
					targetTree.setSelectionPath(null);
					return false;
				}
				targetTree.setSelectionPath(pathTarget);
				if (action == DnDConstants.ACTION_COPY)
				{
					return false;
				}
				else if (action == DnDConstants.ACTION_MOVE)
				{
					object targetNode = pathTarget.getLastPathComponent();
					return canMove(draggedNode, targetNode);
				}
				else
				{
					return false;
				}
			}

			public virtual bool executeDrop(JTree targetTree, object draggedNode, object targetNode, int action)
			{
				if (action == DnDConstants.ACTION_COPY)
				{
					return false;
				}
				else if (action == DnDConstants.ACTION_MOVE)
				{
					if (canMove(draggedNode, targetNode))
					{
						if (draggedNode == targetNode)
						{
							return true;
						}
						outerInstance.listener.moveRequested(new Event(null), (AddTool) draggedNode, (AddTool) targetNode);
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

			internal virtual bool canMove(object draggedNode, object targetNode)
			{
				if (outerInstance.listener == null)
				{
					return false;
				}
				if (!(draggedNode is AddTool) || !(targetNode is AddTool))
				{
					return false;
				}
				LogisimFile file = outerInstance.proj.LogisimFile;
				AddTool dragged = (AddTool) draggedNode;
				AddTool target = (AddTool) targetNode;
				int draggedIndex = file.Tools.IndexOf(dragged);
				int targetIndex = file.Tools.IndexOf(target);
				if (targetIndex < 0 || draggedIndex < 0)
				{
					return false;
				}
				return true;
			}
		}

		private class DeleteAction : AbstractAction
		{
			private readonly ProjectExplorer outerInstance;

			public DeleteAction(ProjectExplorer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				TreePath path = getSelectionPath();
				if (outerInstance.listener != null && path != null && path.getPathCount() == 2)
				{
					outerInstance.listener.deleteRequested(new Event(path));
				}
				outerInstance.requestFocus();
			}
		}

		private class MyListener : MouseListener, TreeSelectionListener, ProjectListener, LibraryListener, CircuitListener, PropertyChangeListener
		{
			private readonly ProjectExplorer outerInstance;

			public MyListener(ProjectExplorer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			//
			// MouseListener methods
			//
			public virtual void mouseEntered(MouseEvent e)
			{
			}

			public virtual void mouseExited(MouseEvent e)
			{
			}

			public virtual void mousePressed(MouseEvent e)
			{
				outerInstance.requestFocus();
				checkForPopup(e);
			}

			public virtual void mouseReleased(MouseEvent e)
			{
				checkForPopup(e);
			}

			internal virtual void checkForPopup(MouseEvent e)
			{
				if (e.isPopupTrigger())
				{
					TreePath path = getPathForLocation(e.getX(), e.getY());
					if (path != null && outerInstance.listener != null)
					{
						JPopupMenu menu = outerInstance.listener.menuRequested(new Event(path));
						if (menu != null)
						{
							menu.show(outerInstance, e.getX(), e.getY());
						}
					}
				}
			}

			public virtual void mouseClicked(MouseEvent e)
			{
				if (e.getClickCount() == 2)
				{
					TreePath path = getPathForLocation(e.getX(), e.getY());
					if (path != null && outerInstance.listener != null)
					{
						outerInstance.listener.doubleClicked(new Event(path));
					}
				}
			}

			//
			// TreeSelectionListener methods
			//
			public virtual void valueChanged(TreeSelectionEvent e)
			{
				TreePath path = e.getNewLeadSelectionPath();
				if (outerInstance.listener != null)
				{
					outerInstance.listener.selectionChanged(new Event(path));
				}
			}

			//
			// project/library file/circuit listener methods
			//
			public virtual void projectChanged(ProjectEvent @event)
			{
				int act = @event.Action;
				if (act == ProjectEvent.ACTION_SET_TOOL)
				{
					TreePath path = getSelectionPath();
					if (path != null && path.getLastPathComponent() != @event.Tool)
					{
						clearSelection();
					}
				}
				else if (act == ProjectEvent.ACTION_SET_FILE)
				{
					File = @event.LogisimFile;
				}
				else if (act == ProjectEvent.ACTION_SET_CURRENT)
				{
					outerInstance.repaint();
				}
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				int act = @event.Action;
				if (act == LibraryEvent.ADD_TOOL)
				{
					if (@event.Data is AddTool)
					{
						AddTool tool = (AddTool) @event.Data;
						if (tool.Factory is SubcircuitFactory)
						{
							SubcircuitFactory fact = (SubcircuitFactory) tool.Factory;
							fact.Subcircuit.addCircuitListener(this);
						}
					}
				}
				else if (act == LibraryEvent.REMOVE_TOOL)
				{
					if (@event.Data is AddTool)
					{
						AddTool tool = (AddTool) @event.Data;
						if (tool.Factory is SubcircuitFactory)
						{
							SubcircuitFactory fact = (SubcircuitFactory) tool.Factory;
							fact.Subcircuit.removeCircuitListener(this);
						}
					}
				}
				else if (act == LibraryEvent.ADD_LIBRARY)
				{
					if (@event.Data is LibraryEventSource)
					{
						((LibraryEventSource) @event.Data).addLibraryListener(outerInstance.subListener);
					}
				}
				else if (act == LibraryEvent.REMOVE_LIBRARY)
				{
					if (@event.Data is LibraryEventSource)
					{
						((LibraryEventSource) @event.Data).removeLibraryListener(outerInstance.subListener);
					}
				}
				Library lib = @event.Source;
				switch (act)
				{
				case LibraryEvent.DIRTY_STATE:
				case LibraryEvent.SET_NAME:
					outerInstance.model.fireNodesChanged(outerInstance.model.findPaths(lib));
					break;
				case LibraryEvent.MOVE_TOOL:
					outerInstance.model.fireNodesChanged(outerInstance.model.findPathsForTools(lib));
					break;
				case LibraryEvent.SET_MAIN:
					break;
				default:
					outerInstance.model.fireStructureChanged();
				break;
				}
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				int act = @event.Action;
				if (act == CircuitEvent.ACTION_SET_NAME)
				{
					outerInstance.model.fireStructureChanged();
					// The following almost works - but the labels aren't made
					// bigger, so you get "..." behavior with longer names.
					// model.fireNodesChanged(model.findPaths(event.getCircuit()));
				}
			}

			internal virtual LogisimFile File
			{
				set
				{
					outerInstance.model.fireStructureChanged();
					expandRow(0);
    
					foreach (Circuit circ in value.Circuits)
					{
						circ.addCircuitListener(this);
					}
    
					outerInstance.subListener = new SubListener(outerInstance); // create new one so that old listeners die away
					foreach (Library sublib in value.Libraries)
					{
						if (sublib is LibraryEventSource)
						{
							((LibraryEventSource) sublib).addLibraryListener(outerInstance.subListener);
						}
					}
				}
			}

			//
			// PropertyChangeListener methods
			//
			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				if (AppPreferences.GATE_SHAPE.isSource(@event))
				{
					repaint();
				}
			}
		}

		private class SubListener : LibraryListener
		{
			private readonly ProjectExplorer outerInstance;

			public SubListener(ProjectExplorer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				outerInstance.model.fireStructureChanged();
			}
		}

		private Project proj;
		private MyListener myListener;
		private SubListener subListener;
		private MyModel model;
		private MyCellRenderer renderer;
		private DeleteAction deleteAction;
		private Listener listener = null;
		private Tool haloedTool = null;

		public ProjectExplorer(Project proj) : base()
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.proj = proj;

			setModel(model);
			setRootVisible(true);
			addMouseListener(myListener);
			ToolTipManager.sharedInstance().registerComponent(this);

			MySelectionModel selector = new MySelectionModel(this);
			selector.setSelectionMode(TreeSelectionModel.SINGLE_TREE_SELECTION);
			setSelectionModel(selector);
			setCellRenderer(renderer);
			JTreeUtil.configureDragAndDrop(this, new DragController(this));
			addTreeSelectionListener(myListener);

			InputMap imap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT);
			imap.put(KeyStroke.getKeyStroke(KeyEvent.VK_BACK_SPACE, 0), deleteAction);
			ActionMap amap = getActionMap();
			amap.put(deleteAction, deleteAction);

			proj.addProjectListener(myListener);
			proj.addLibraryListener(myListener);
			AppPreferences.GATE_SHAPE.addPropertyChangeListener(myListener);
			myListener.File = proj.LogisimFile;
			LocaleManager.addLocaleListener(this);
		}

		public virtual Tool SelectedTool
		{
			get
			{
				TreePath path = getSelectionPath();
				if (path == null)
				{
					return null;
				}
				object last = path.getLastPathComponent();
				return last is Tool ? (Tool) last : null;
			}
		}

		public virtual Listener Listener
		{
			set
			{
				listener = value;
			}
		}

		public virtual Tool HaloedTool
		{
			set
			{
				if (haloedTool == value)
				{
					return;
				}
				haloedTool = value;
				repaint();
			}
		}

		public virtual void localeChanged()
		{
			model.fireStructureChanged();
		}
	}

}
