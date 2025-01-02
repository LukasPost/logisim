// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class Dag<E>
	{
		private class Node<E>
		{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unused") E data;
			internal E data;
			internal HashSet<Node<E>> succs = new HashSet<Node<E>>(); // of Nodes
			internal int numPreds = 0;
			internal bool mark;

			internal Node(E data)
			{
				this.data = data;
			}
		}

		private Dictionary<E, Node<E>> nodes = new Dictionary<E, Node<E>>();

		public Dag()
		{
		}

		public virtual bool hasPredecessors(object data)
		{
			Node<E> from = findNode(data);
			return from != null && from.numPreds != 0;
		}

		public virtual bool hasSuccessors(object data)
		{
			Node<E> to = findNode(data);
			return to != null && to.succs.Count > 0;
		}

		public virtual bool canFollow(object query, object @base)
		{
			Node<E> queryNode = findNode(query);
			Node<E> baseNode = findNode(@base);
			if (baseNode == null || queryNode == null)
			{
				return !@base.Equals(query);
			}
			else
			{
				return canFollow(queryNode, baseNode);
			}
		}

		public virtual bool addEdge(E srcData, E dstData)
		{
			if (!canFollow(dstData, srcData))
			{
				return false;
			}

			Node<E> src = createNode(srcData);
			Node<E> dst = createNode(dstData);
			if (src.succs.Add(dst))
			{
				++dst.numPreds; // add since not already present
			}
			return true;
		}

		public virtual bool removeEdge(object srcData, object dstData)
		{
			// returns true if the edge could be removed
			Node<E> src = findNode(srcData);
			Node<E> dst = findNode(dstData);
			if (src == null || dst == null)
			{
				return false;
			}
			if (!src.succs.Remove(dst))
			{
				return false;
			}

			--dst.numPreds;
			if (dst.numPreds == 0 && dst.succs.Count == 0)
			{
				nodes.Remove(dstData);
			}
			if (src.numPreds == 0 && src.succs.Count == 0)
			{
				nodes.Remove(srcData);
			}
			return true;
		}

		public virtual void removeNode(object data)
		{
			Node<E> n = findNode(data);
			if (n == null)
			{
				return;
			}

			for (IEnumerator<Node<E>> it = n.succs.GetEnumerator(); it.MoveNext();)
			{
				Node<E> succ = it.Current;
				--(succ.numPreds);
				if (succ.numPreds == 0 && succ.succs.Count == 0)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
			}

			if (n.numPreds > 0)
			{
				for (IEnumerator<Node<E>> it = nodes.Values.GetEnumerator(); it.MoveNext();)
				{
					Node<E> q = it.Current;
					if (q.succs.Remove(n) && q.numPreds == 0 && q.succs.Count == 0)
					{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
						it.remove();
					}
				}
			}
		}

		private Node<E> findNode(object data)
		{
			if (data == null)
			{
				return null;
			}
			return nodes[data];
		}

		private Node<E> createNode(E data)
		{
			Node<E> ret = findNode(data);
			if (ret != null)
			{
				return ret;
			}
			if (data == null)
			{
				return null;
			}

			ret = new Node<E>(data);
			nodes[data] = ret;
			return ret;
		}

		private bool canFollow(Node<E> query, Node<E> @base)
		{
			if (@base == query)
			{
				return false;
			}

			// mark all as unvisited
			foreach (Node<E> n in nodes.Values)
			{
				n.mark = false; // will become true once reached
			}

			// Search starting at query: If base is found, then it follows
			// the query already, and so query cannot follow base.
			LinkedList<Node<E>> fringe = new LinkedList<Node<E>>();
			fringe.AddLast(query);
			while (fringe.Count > 0)
			{
				Node<E> n = fringe.RemoveFirst();
				foreach (Node<E> next in n.succs)
				{
					if (!next.mark)
					{
						if (next == @base)
						{
							return false;
						}
						next.mark = true;
						fringe.AddLast(next);
					}
				}
			}
			return true;
		}
	}

}
