// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using CircuitAttributes = logisim.circuit.CircuitAttributes;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using CircuitState = logisim.circuit.CircuitState;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using StdAttr = logisim.instance.StdAttr;

	internal class SimulationTreeCircuitNode : SimulationTreeNode, CircuitListener, AttributeListener, IComparer<Component>
	{
		private class CompareByName : IComparer<object>
		{
			public virtual int Compare(object a, object b)
			{
				return string.Compare(a.ToString(), b.ToString(), StringComparison.OrdinalIgnoreCase);
			}
		}

		private SimulationTreeModel model;
		private SimulationTreeCircuitNode parent;
		private CircuitState circuitState;
		private Component subcircComp;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private List<TreeNode> children_Conflict;

		public SimulationTreeCircuitNode(SimulationTreeModel model, SimulationTreeCircuitNode parent, CircuitState circuitState, Component subcircComp)
		{
			this.model = model;
			this.parent = parent;
			this.circuitState = circuitState;
			this.subcircComp = subcircComp;
			this.children_Conflict = new List<TreeNode>();
			circuitState.Circuit.addCircuitListener(this);
			if (subcircComp != null)
			{
				subcircComp.AttributeSet.addAttributeListener(this);
			}
			else
			{
				circuitState.Circuit.StaticAttributes.addAttributeListener(this);
			}
			computeChildren();
		}

		public virtual CircuitState CircuitState
		{
			get
			{
				return circuitState;
			}
		}

		public override ComponentFactory ComponentFactory
		{
			get
			{
				return circuitState.Circuit.SubcircuitFactory;
			}
		}

		public override bool isCurrentView(SimulationTreeModel model)
		{
			return model.CurrentView == circuitState;
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

		public override TreeNode getChildAt(int index)
		{
			return children_Conflict[index];
		}

		public override int ChildCount
		{
			get
			{
				return children_Conflict.Count;
			}
		}

		public override TreeNode Parent
		{
			get
			{
				return parent;
			}
		}

		public override int getIndex(TreeNode node)
		{
			return children_Conflict.IndexOf(node);
		}

		public override bool AllowsChildren
		{
			get
			{
				return true;
			}
		}

		public override bool Leaf
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator<TreeNode> children()
		{
			return Collections.enumeration(children_Conflict);
		}

		public virtual void circuitChanged(CircuitEvent @event)
		{
			int action = @event.Action;
			if (action == CircuitEvent.ACTION_SET_NAME)
			{
				model.fireNodeChanged(this);
			}
			else
			{
				if (computeChildren())
				{
					model.fireStructureChanged(this);
				}
			}
		}

		// returns true if changed
		private bool computeChildren()
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
					TreeNode toAdd = model.mapComponentToNode(comp);
					if (toAdd != null)
					{
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
				SimulationTreeCircuitNode toAdd = null;
				foreach (TreeNode o in children_Conflict)
				{
					if (o is SimulationTreeCircuitNode)
					{
						SimulationTreeCircuitNode n = (SimulationTreeCircuitNode) o;
						if (n.circuitState == state)
						{
							toAdd = n;
							break;
						}
					}
				}
				if (toAdd == null)
				{
					toAdd = new SimulationTreeCircuitNode(model, this, state, comp);
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

		//
		// AttributeListener methods
		public virtual void attributeListChanged(AttributeEvent e)
		{
		}

		public virtual void attributeValueChanged(AttributeEvent e)
		{
			object attr = e.Attribute;
			if (attr == CircuitAttributes.CIRCUIT_LABEL_ATTR || attr == StdAttr.LABEL)
			{
				model.fireNodeChanged(this);
			}
		}
	}

}
