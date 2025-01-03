// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{


	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using CircuitState = logisim.circuit.CircuitState;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Component = logisim.comp.Component;
	using StdAttr = logisim.instance.StdAttr;

	internal class ComponentSelector : JTree
	{
		private class CompareByName : IComparer<object>
		{
			public virtual int Compare(object a, object b)
			{
				return string.Compare(a.ToString(), b.ToString(), StringComparison.OrdinalIgnoreCase);
			}
		}

		private class CircuitNode : TreeNode, CircuitListener, IComparer<Component>
		{
			private readonly ComponentSelector outerInstance;

			internal CircuitNode parent;
			internal CircuitState circuitState;
			internal Component subcircComp;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
			internal List<TreeNode> children_Conflict;

			public CircuitNode(ComponentSelector outerInstance, CircuitNode parent, CircuitState circuitState, Component subcircComp)
			{
				this.outerInstance = outerInstance;
				this.parent = parent;
				this.circuitState = circuitState;
				this.subcircComp = subcircComp;
				this.children_Conflict = new List<TreeNode>();
				circuitState.Circuit.addCircuitListener(this);
				computeChildren();
			}

			public override string ToString()
			{
				if (subcircComp != null)
				{
					string label = subcircComp.AttributeSet.getValue(StdAttr.LABEL);
					if (!string.ReferenceEquals(label, null) && !label.Equals(""))
					{
						return label;
					}
				}
				string ret = circuitState.Circuit.Name;
				if (subcircComp != null)
				{
					ret += subcircComp.Location;
				}
				return ret;
			}

			public virtual TreeNode getChildAt(int index)
			{
				return children_Conflict[index];
			}

			public virtual int ChildCount
			{
				get
				{
					return children_Conflict.Count;
				}
			}

			public virtual TreeNode Parent
			{
				get
				{
					return parent;
				}
			}

			public virtual int getIndex(TreeNode node)
			{
				return children_Conflict.IndexOf(node);
			}

			public virtual bool AllowsChildren
			{
				get
				{
					return true;
				}
			}

			public virtual bool Leaf
			{
				get
				{
					return false;
				}
			}

			public virtual IEnumerator<TreeNode> children()
			{
				return Collections.enumeration(children_Conflict);
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				int action = @event.Action;
				DefaultTreeModel model = (DefaultTreeModel) getModel();
				if (action == CircuitEvent.ACTION_SET_NAME)
				{
					model.nodeChanged(this);
				}
				else
				{
					if (computeChildren())
					{
						model.nodeStructureChanged(this);
					}
					else if (action == CircuitEvent.ACTION_INVALIDATE)
					{
						object o = @event.Data;
						for (int i = children_Conflict.Count - 1; i >= 0; i--)
						{
							object o2 = children_Conflict[i];
							if (o2 is ComponentNode)
							{
								ComponentNode n = (ComponentNode) o2;
								if (n.comp == o)
								{
									int[] changed = new int[] {i};
									children_Conflict.RemoveAt(i);
									model.nodesWereRemoved(this, changed, new object[] {n});
									children_Conflict.Insert(i, new ComponentNode(outerInstance, this, n.comp));
									model.nodesWereInserted(this, changed);
								}
							}
						}
					}
				}
			}

			// returns true if changed
			internal virtual bool computeChildren()
			{
				List<TreeNode> newChildren = new List<TreeNode>();
				List<Component> subcircs = new List<Component>();
				foreach (Component comp in circuitState.Circuit.NonWires)
				{
					if (comp.Factory is SubcircuitFactory)
					{
						subcircs.Add(comp);
					}
					else
					{
						object o = comp.getFeature(typeof(Loggable));
						if (o != null)
						{
							ComponentNode toAdd = null;
							foreach (TreeNode o2 in children_Conflict)
							{
								if (o2 is ComponentNode)
								{
									ComponentNode n = (ComponentNode) o2;
									if (n.comp == comp)
									{
										toAdd = n;
										break;
									}
								}
							}
							if (toAdd == null)
							{
								toAdd = new ComponentNode(outerInstance, this, comp);
							}
							newChildren.Add(toAdd);
						}
					}
				}
				newChildren.Sort(new CompareByName());
				subcircs.Sort(this);
				foreach (Component comp in subcircs)
				{
					SubcircuitFactory factory = (SubcircuitFactory) comp.Factory;
					CircuitState state = factory.getSubstate(circuitState, comp);
					CircuitNode toAdd = null;
					foreach (TreeNode o in children_Conflict)
					{
						if (o is CircuitNode)
						{
							CircuitNode n = (CircuitNode) o;
							if (n.circuitState == state)
							{
								toAdd = n;
								break;
							}
						}
					}
					if (toAdd == null)
					{
						toAdd = new CircuitNode(outerInstance, this, state, comp);
					}
					newChildren.Add(toAdd);
				}

// JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
// ORIGINAL LINE: if (!children.equals(newChildren))
				if (!children_Conflict.SequenceEqual(newChildren))
				{
					children_Conflict = newChildren;
					return true;
				}
				else
				{
					return false;
				}
			}

			public virtual int Compare(Component a, Component b)
			{
				if (a != b)
				{
					string aName = a.Factory.DisplayName;
					string bName = b.Factory.DisplayName;
					int ret = string.Compare(aName, bName, StringComparison.OrdinalIgnoreCase);
					if (ret != 0)
					{
						return ret;
					}
				}
				return string.CompareOrdinal(a.Location.ToString(), b.Location.ToString());
			}
		}

		private class ComponentNode : TreeNode
		{
			private readonly ComponentSelector outerInstance;

			internal CircuitNode parent;
			internal Component comp;
			internal OptionNode[] opts;

			public ComponentNode(ComponentSelector outerInstance, CircuitNode parent, Component comp)
			{
				this.outerInstance = outerInstance;
				this.parent = parent;
				this.comp = comp;
				this.opts = null;

				Loggable log = (Loggable) comp.getFeature(typeof(Loggable));
				if (log != null)
				{
					object[] opts = log.getLogOptions(parent.circuitState);
					if (opts != null && opts.Length > 0)
					{
						this.opts = new OptionNode[opts.Length];
						for (int i = 0; i < opts.Length; i++)
						{
							this.opts[i] = new OptionNode(outerInstance, this, opts[i]);
						}
					}
				}
			}

			public override string ToString()
			{
				Loggable log = (Loggable) comp.getFeature(typeof(Loggable));
				if (log != null)
				{
					string ret = log.getLogName(null);
					if (!string.ReferenceEquals(ret, null) && !ret.Equals(""))
					{
						return ret;
					}
				}
				return comp.Factory.DisplayName + " " + comp.Location;
			}

			public virtual TreeNode getChildAt(int index)
			{
				return opts[index];
			}

			public virtual int ChildCount
			{
				get
				{
					return opts == null ? 0 : opts.Length;
				}
			}

			public virtual TreeNode Parent
			{
				get
				{
					return parent;
				}
			}

			public virtual int getIndex(TreeNode n)
			{
				for (int i = 0; i < opts.Length; i++)
				{
					if (opts[i] == n)
					{
						return i;
					}
				}
				return -1;
			}

			public virtual bool AllowsChildren
			{
				get
				{
					return false;
				}
			}

			public virtual bool Leaf
			{
				get
				{
					return opts == null || opts.Length == 0;
				}
			}

			public virtual IEnumerator<OptionNode> children()
			{
				return Collections.enumeration(opts);
			}
		}

		private class OptionNode : TreeNode
		{
			private readonly ComponentSelector outerInstance;

			internal ComponentNode parent;
			internal object option;

			public OptionNode(ComponentSelector outerInstance, ComponentNode parent, object option)
			{
				this.outerInstance = outerInstance;
				this.parent = parent;
				this.option = option;
			}

			public override string ToString()
			{
				return option.ToString();
			}

			public virtual TreeNode getChildAt(int arg0)
			{
				return null;
			}

			public virtual int ChildCount
			{
				get
				{
					return 0;
				}
			}

			public virtual TreeNode Parent
			{
				get
				{
					return parent;
				}
			}

			public virtual int getIndex(TreeNode n)
			{
				return -1;
			}

			public virtual bool AllowsChildren
			{
				get
				{
					return false;
				}
			}

			public virtual bool Leaf
			{
				get
				{
					return true;
				}
			}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public java.util.Enumeration children()
			public virtual System.Collections.IEnumerator children()
			{
				return Collections.emptyEnumeration();
			}
		}

		private class MyCellRenderer : DefaultTreeCellRenderer
		{
			private readonly ComponentSelector outerInstance;

			public MyCellRenderer(ComponentSelector outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override java.awt.Component getTreeCellRendererComponent(JTree tree, object value, bool selected, bool expanded, bool leaf, int row, bool hasFocus)
			{
				java.awt.Component ret = base.getTreeCellRendererComponent(tree, value, selected, expanded, leaf, row, hasFocus);
				if (ret is JLabel && value is ComponentNode)
				{
					ComponentNode node = (ComponentNode) value;
					ComponentIcon icon = new ComponentIcon(node.comp);
					if (node.ChildCount > 0)
					{
						icon.TriangleState = expanded ? ComponentIcon.TRIANGLE_OPEN : ComponentIcon.TRIANGLE_CLOSED;
					}
					((JLabel) ret).setIcon(icon);
				}
				return ret;
			}
		}

		private Model logModel;

		public ComponentSelector(Model logModel)
		{
			DefaultTreeModel model = new DefaultTreeModel(null);
			model.setAsksAllowsChildren(false);
			setModel(model);
			setRootVisible(false);
			LogModel = logModel;
			setCellRenderer(new MyCellRenderer(this));
		}

		public virtual Model LogModel
		{
			set
			{
				this.logModel = value;
    
				DefaultTreeModel model = (DefaultTreeModel) getModel();
				CircuitNode curRoot = (CircuitNode) model.getRoot();
				CircuitState state = logModel == null ? null : logModel.CircuitState;
				if (state == null)
				{
					if (curRoot != null)
					{
						model.setRoot(null);
					}
					return;
				}
				if (curRoot == null || curRoot.circuitState != state)
				{
					curRoot = new CircuitNode(this, null, state, null);
					model.setRoot(curRoot);
				}
			}
		}

		public virtual List<SelectionItem> SelectedItems
		{
			get
			{
				TreePath[] sel = getSelectionPaths();
				if (sel == null || sel.Length == 0)
				{
					return [];
				}
    
				List<SelectionItem> ret = new List<SelectionItem>();
				for (int i = 0; i < sel.Length; i++)
				{
					TreePath path = sel[i];
					object last = path.getLastPathComponent();
					ComponentNode n = null;
					object opt = null;
					if (last is OptionNode)
					{
						OptionNode o = (OptionNode) last;
						n = o.parent;
						opt = o.option;
					}
					else if (last is ComponentNode)
					{
						n = (ComponentNode) last;
						if (n.opts != null)
						{
							n = null;
						}
					}
					if (n != null)
					{
						int count = 0;
						for (CircuitNode cur = n.parent; cur != null; cur = cur.parent)
						{
							count++;
						}
						Component[] nPath = new Component[count - 1];
						CircuitNode cur = n.parent;
						for (int j = nPath.Length - 1; j >= 0; j--)
						{
							nPath[j] = cur.subcircComp;
							cur = cur.parent;
						}
						ret.Add(new SelectionItem(logModel, nPath, n.comp, opt));
					}
				}
				return ret.Count == 0 ? null : ret;
			}
		}

		public virtual bool hasSelectedItems()
		{
			TreePath[] sel = getSelectionPaths();
			if (sel == null || sel.Length == 0)
			{
				return false;
			}

			for (int i = 0; i < sel.Length; i++)
			{
				object last = sel[i].getLastPathComponent();
				if (last is OptionNode)
				{
					return true;
				}
				else if (last is ComponentNode)
				{
					if (((ComponentNode) last).opts == null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual void localeChanged()
		{
			repaint();
		}
	}

}
