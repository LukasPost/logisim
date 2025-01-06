/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.util.ArrayList;

import javax.swing.event.TreeModelEvent;
import javax.swing.event.TreeModelListener;
import javax.swing.tree.TreeModel;
import javax.swing.tree.TreeNode;
import javax.swing.tree.TreePath;

import logisim.circuit.CircuitState;

public class SimulationTreeModel implements TreeModel {
	private ArrayList<TreeModelListener> listeners;
	private SimulationTreeCircuitNode root;
	private CircuitState currentView;

	public SimulationTreeModel(CircuitState rootState) {
		listeners = new ArrayList<>();
		root = new SimulationTreeCircuitNode(this, null, rootState, null);
		currentView = null;
	}

	public CircuitState getRootState() {
		return root.getCircuitState();
	}

	public CircuitState getCurrentView() {
		return currentView;
	}

	public void setCurrentView(CircuitState value) {
		CircuitState oldView = currentView;
		if (oldView != value) {
			currentView = value;

			SimulationTreeCircuitNode node1 = mapToNode(oldView);
			if (node1 != null)
				fireNodeChanged(node1);

			SimulationTreeCircuitNode node2 = mapToNode(value);
			if (node2 != null)
				fireNodeChanged(node2);
		}
	}

	private SimulationTreeCircuitNode mapToNode(CircuitState state) {
		TreePath path = mapToPath(state);
		if (path == null) return null;
		else return (SimulationTreeCircuitNode) path.getLastPathComponent();
	}

	public TreePath mapToPath(CircuitState state) {
		if (state == null)
			return null;
		ArrayList<CircuitState> path = new ArrayList<>();
		CircuitState current = state;
		CircuitState parent = current.getParentState();
		while (parent != null && parent != state) {
			path.add(current);
			current = parent;
			parent = current.getParentState();
		}

		Object[] pathNodes = new Object[path.size() + 1];
		pathNodes[0] = root;
		int pathPos = 1;
		SimulationTreeCircuitNode node = root;
		for (int i = path.size() - 1; i >= 0; i--) {
			current = path.get(i);
			SimulationTreeCircuitNode oldNode = node;
			for (int j = 0, n = node.getChildCount(); j < n; j++) {
				Object child = node.getChildAt(j);
				if (child instanceof SimulationTreeCircuitNode circNode) if (circNode.getCircuitState() == current) {
					node = circNode;
					break;
				}
			}
			if (node == oldNode) return null;
			pathNodes[pathPos] = node;
			pathPos++;
		}
		return new TreePath(pathNodes);
	}

	protected SimulationTreeNode mapComponentToNode() {
		return null;
	}

	public void addTreeModelListener(TreeModelListener l) {
		listeners.add(l);
	}

	public void removeTreeModelListener(TreeModelListener l) {
		listeners.remove(l);
	}

	protected void fireNodeChanged(Object node) {
		TreeModelEvent e = new TreeModelEvent(this, findPath(node));
		for (TreeModelListener l : listeners) l.treeNodesChanged(e);
	}

	protected void fireStructureChanged(Object node) {
		TreeModelEvent e = new TreeModelEvent(this, findPath(node));
		for (TreeModelListener l : listeners) l.treeStructureChanged(e);
	}

	private TreePath findPath(Object node) {
		ArrayList<Object> path = new ArrayList<>();
		Object current = node;
		while (current instanceof TreeNode) {
			path.addFirst(current);
			current = ((TreeNode) current).getParent();
		}
		if (current != null) path.addFirst(current);
		return new TreePath(path.toArray());
	}

	public Object getRoot() {
		return root;
	}

	public int getChildCount(Object parent) {
		if (parent instanceof TreeNode) return ((TreeNode) parent).getChildCount();
		else return 0;
	}

	public Object getChild(Object parent, int index) {
		if (parent instanceof TreeNode) return ((TreeNode) parent).getChildAt(index);
		else return null;
	}

	public int getIndexOfChild(Object parent, Object child) {
		if (parent instanceof TreeNode && child instanceof TreeNode)
			return ((TreeNode) parent).getIndex((TreeNode) child);
		else return -1;
	}

	public boolean isLeaf(Object node) {
		return !(node instanceof TreeNode) || ((TreeNode) node).getChildCount() == 0;
	}

	public void valueForPathChanged(TreePath path, Object newValue) {
		throw new UnsupportedOperationException();
	}
}
