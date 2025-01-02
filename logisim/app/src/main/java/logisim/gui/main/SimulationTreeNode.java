/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.util.Enumeration;

import javax.swing.tree.TreeNode;

import logisim.comp.ComponentFactory;

public abstract class SimulationTreeNode implements TreeNode {
	public abstract ComponentFactory getComponentFactory();

	public boolean isCurrentView(SimulationTreeModel model) {
		return false;
	}

	public abstract Enumeration<TreeNode> children();

	public abstract boolean getAllowsChildren();

	public abstract TreeNode getChildAt(int childIndex);

	public abstract int getChildCount();

	public abstract int getIndex(TreeNode node);

	public abstract TreeNode getParent();

	public abstract boolean isLeaf();
}
