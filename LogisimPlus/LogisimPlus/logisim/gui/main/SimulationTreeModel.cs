// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{


	using CircuitState = logisim.circuit.CircuitState;
	using Component = logisim.comp.Component;

	public class SimulationTreeModel : TreeModel
	{
		private List<TreeModelListener> listeners;
		private SimulationTreeCircuitNode root;
		private CircuitState currentView;

		public SimulationTreeModel(CircuitState rootState)
		{
			this.listeners = new List<TreeModelListener>();
			this.root = new SimulationTreeCircuitNode(this, null, rootState, null);
			this.currentView = null;
		}

		public virtual CircuitState RootState
		{
			get
			{
				return root.CircuitState;
			}
		}

		public virtual CircuitState CurrentView
		{
			get
			{
				return currentView;
			}
			set
			{
				CircuitState oldView = currentView;
				if (oldView != value)
				{
					currentView = value;
    
					SimulationTreeCircuitNode node1 = mapToNode(oldView);
					if (node1 != null)
					{
						fireNodeChanged(node1);
					}
    
					SimulationTreeCircuitNode node2 = mapToNode(value);
					if (node2 != null)
					{
						fireNodeChanged(node2);
					}
				}
			}
		}


		private SimulationTreeCircuitNode mapToNode(CircuitState state)
		{
			TreePath path = mapToPath(state);
			if (path == null)
			{
				return null;
			}
			else
			{
				return (SimulationTreeCircuitNode) path.getLastPathComponent();
			}
		}

		public virtual TreePath mapToPath(CircuitState state)
		{
			if (state == null)
			{
				return null;
			}
			List<CircuitState> path = new List<CircuitState>();
			CircuitState current = state;
			CircuitState parent = current.ParentState;
			while (parent != null && parent != state)
			{
				path.Add(current);
				current = parent;
				parent = current.ParentState;
			}

			object[] pathNodes = new object[path.Count + 1];
			pathNodes[0] = root;
			int pathPos = 1;
			SimulationTreeCircuitNode node = root;
			for (int i = path.Count - 1; i >= 0; i--)
			{
				current = path[i];
				SimulationTreeCircuitNode oldNode = node;
				for (int j = 0, n = node.ChildCount; j < n; j++)
				{
					object child = node.getChildAt(j);
					if (child is SimulationTreeCircuitNode)
					{
						SimulationTreeCircuitNode circNode = (SimulationTreeCircuitNode) child;
						if (circNode.CircuitState == current)
						{
							node = circNode;
							break;
						}
					}
				}
				if (node == oldNode)
				{
					return null;
				}
				pathNodes[pathPos] = node;
				pathPos++;
			}
			return new TreePath(pathNodes);
		}

		protected internal virtual SimulationTreeNode mapComponentToNode(Component comp)
		{
			return null;
		}

		public virtual void addTreeModelListener(TreeModelListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeTreeModelListener(TreeModelListener l)
		{
			listeners.Remove(l);
		}

		protected internal virtual void fireNodeChanged(object node)
		{
			TreeModelEvent e = new TreeModelEvent(this, findPath(node));
			foreach (TreeModelListener l in listeners)
			{
				l.treeNodesChanged(e);
			}
		}

		protected internal virtual void fireStructureChanged(object node)
		{
			TreeModelEvent e = new TreeModelEvent(this, findPath(node));
			foreach (TreeModelListener l in listeners)
			{
				l.treeStructureChanged(e);
			}
		}

		private TreePath findPath(object node)
		{
			List<object> path = new List<object>();
			object current = node;
			while (current is TreeNode)
			{
				path.Insert(0, current);
				current = ((TreeNode) current).getParent();
			}
			if (current != null)
			{
				path.Insert(0, current);
			}
			return new TreePath(path.ToArray());
		}

		public virtual object Root
		{
			get
			{
				return root;
			}
		}

		public virtual int getChildCount(object parent)
		{
			if (parent is TreeNode)
			{
				return ((TreeNode) parent).getChildCount();
			}
			else
			{
				return 0;
			}
		}

		public virtual object getChild(object parent, int index)
		{
			if (parent is TreeNode)
			{
				return ((TreeNode) parent).getChildAt(index);
			}
			else
			{
				return null;
			}
		}

		public virtual int getIndexOfChild(object parent, object child)
		{
			if (parent is TreeNode && child is TreeNode)
			{
				return ((TreeNode) parent).getIndex((TreeNode) child);
			}
			else
			{
				return -1;
			}
		}

		public virtual bool isLeaf(object node)
		{
			if (node is TreeNode)
			{
				return ((TreeNode) node).getChildCount() == 0;
			}
			else
			{
				return true;
			}
		}

		public virtual void valueForPathChanged(TreePath path, object newValue)
		{
			throw new System.NotSupportedException();
		}
	}

}
