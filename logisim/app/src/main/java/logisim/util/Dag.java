/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.Iterator;

public class Dag<E> {
	private static class Node<E> {
		@SuppressWarnings("unused")
		E data;
		HashSet<Node<E>> succs = new HashSet<Node<E>>(); // of Nodes
		int numPreds = 0;
		boolean mark;

		Node(E data) {
			this.data = data;
		}
	}

	private HashMap<E, Node<E>> nodes = new HashMap<E, Node<E>>();

	public Dag() {
	}

	public boolean hasPredecessors(Object data) {
		Node<E> from = findNode(data);
		return from != null && from.numPreds != 0;
	}

	public boolean hasSuccessors(Object data) {
		Node<E> to = findNode(data);
		return to != null && !to.succs.isEmpty();
	}

	public boolean canFollow(Object query, Object base) {
		Node<E> queryNode = findNode(query);
		Node<E> baseNode = findNode(base);
		if (baseNode == null || queryNode == null) {
			return !base.equals(query);
		} else {
			return canFollow(queryNode, baseNode);
		}
	}

	public boolean addEdge(E srcData, E dstData) {
		if (!canFollow(dstData, srcData))
			return false;

		Node<E> src = createNode(srcData);
		Node<E> dst = createNode(dstData);
		if (src.succs.add(dst))
			++dst.numPreds; // add since not already present
		return true;
	}

	public boolean removeEdge(Object srcData, Object dstData) {
		// returns true if the edge could be removed
		Node<E> src = findNode(srcData);
		Node<E> dst = findNode(dstData);
		if (src == null || dst == null)
			return false;
		if (!src.succs.remove(dst))
			return false;

		--dst.numPreds;
		if (dst.numPreds == 0 && dst.succs.isEmpty())
			nodes.remove(dstData);
		if (src.numPreds == 0 && src.succs.isEmpty())
			nodes.remove(srcData);
		return true;
	}

	public void removeNode(Object data) {
		Node<E> n = findNode(data);
		if (n == null)
			return;

		for (Iterator<Node<E>> it = n.succs.iterator(); it.hasNext();) {
			Node<E> succ = it.next();
			--(succ.numPreds);
			if (succ.numPreds == 0 && succ.succs.isEmpty())
				it.remove();
		}

		if (n.numPreds > 0) {
			for (Iterator<Node<E>> it = nodes.values().iterator(); it.hasNext();) {
				Node<E> q = it.next();
				if (q.succs.remove(n) && q.numPreds == 0 && q.succs.isEmpty())
					it.remove();
			}
		}
	}

	private Node<E> findNode(Object data) {
		if (data == null)
			return null;
		return nodes.get(data);
	}

	private Node<E> createNode(E data) {
		Node<E> ret = findNode(data);
		if (ret != null)
			return ret;
		if (data == null)
			return null;

		ret = new Node<>(data);
		nodes.put(data, ret);
		return ret;
	}

	private boolean canFollow(Node<E> query, Node<E> base) {
		if (base == query)
			return false;

		// mark all as unvisited
		for (Node<E> n : nodes.values()) {
			n.mark = false; // will become true once reached
		}

		// Search starting at query: If base is found, then it follows
		// the query already, and so query cannot follow base.
		LinkedList<Node<E>> fringe = new LinkedList<>();
		fringe.add(query);
		while (!fringe.isEmpty()) {
			Node<E> n = fringe.removeFirst();
			for (Node<E> next : n.succs) {
				if (!next.mark) {
					if (next == base)
						return false;
					next.mark = true;
					fringe.addLast(next);
				}
			}
		}
		return true;
	}
}
