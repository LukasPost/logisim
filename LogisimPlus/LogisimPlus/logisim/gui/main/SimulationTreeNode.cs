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

	using ComponentFactory = logisim.comp.ComponentFactory;

	public abstract class SimulationTreeNode : TreeNode
	{
		public abstract ComponentFactory ComponentFactory {get;}

		public virtual bool isCurrentView(SimulationTreeModel model)
		{
			return false;
		}

		public abstract IEnumerator<TreeNode> children();

		public abstract bool AllowsChildren {get;}

		public abstract TreeNode getChildAt(int childIndex);

		public abstract int ChildCount {get;}

		public abstract int getIndex(TreeNode node);

		public abstract TreeNode Parent {get;}

		public abstract bool Leaf {get;}
	}

}
