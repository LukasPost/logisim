// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.move
{

	using ReplacementMap = logisim.circuit.ReplacementMap;
	using Wire = logisim.circuit.Wire;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;

	internal class Connector
	{
		private const int MAX_SECONDS = 10;
		private const int MAX_ORDERING_TRIES = 10;
		private const int MAX_SEARCH_ITERATIONS = 20000;

		private Connector()
		{
		}

		internal const string ALLOW_NEITHER = "neither";
		internal const string ALLOW_VERTICAL = "vert";
		internal const string ALLOW_HORIZONTAL = "horz";

		internal static MoveResult computeWires(MoveRequest req)
		{
			MoveGesture gesture = req.MoveGesture;
			int dx = req.DeltaX;
			int dy = req.DeltaY;
			List<ConnectionData> baseConnects;
			baseConnects = new List<ConnectionData>(gesture.Connections);
			List<ConnectionData> impossible = pruneImpossible(baseConnects, gesture.FixedAvoidanceMap, dx, dy);

			AvoidanceMap selAvoid = AvoidanceMap.create(gesture.Selected, dx, dy);
			Dictionary<ConnectionData, ISet<Location>> pathLocs;
			pathLocs = new Dictionary<ConnectionData, ISet<Location>>();
			Dictionary<ConnectionData, IList<SearchNode>> initNodes;
			initNodes = new Dictionary<ConnectionData, IList<SearchNode>>();
			foreach (ConnectionData conn in baseConnects)
			{
				HashSet<Location> connLocs = new HashSet<Location>();
				List<SearchNode> connNodes = new List<SearchNode>();
				processConnection(conn, dx, dy, connLocs, connNodes, selAvoid);
				pathLocs[conn] = connLocs;
				initNodes[conn] = connNodes;
			}

			MoveResult bestResult = null;
			int tries;
			switch (baseConnects.Count)
			{
			case 0:
				tries = 0;
				break;
			case 1:
				tries = 1;
				break;
			case 2:
				tries = 2;
				break;
			case 3:
				tries = 8;
				break;
			default:
				tries = MAX_ORDERING_TRIES;
			break;
			}
			long stopTime = DateTimeHelper.CurrentUnixTimeMillis() + MAX_SECONDS * 1000;
			for (int tryNum = 0; tryNum < tries && stopTime - DateTimeHelper.CurrentUnixTimeMillis() > 0; tryNum++)
			{
				if (ConnectorThread.OverrideRequested)
				{
					return null;
				}
				List<ConnectionData> connects;
				connects = new List<ConnectionData>(baseConnects);
				if (tryNum < 2)
				{
					sortConnects(connects, dx, dy);
					if (tryNum == 1)
					{
						connects.Reverse();
					}
				}
				else
				{
					Collections.shuffle(connects);
				}

				MoveResult candidate = tryList(req, gesture, connects, dx, dy, pathLocs, initNodes, stopTime);
				if (candidate == null)
				{
					return null;
				}
				else if (bestResult == null)
				{
					bestResult = candidate;
				}
				else
				{
					int unsatisfied1 = bestResult.UnsatisifiedConnections.Count;
					int unsatisfied2 = candidate.UnsatisifiedConnections.Count;
					if (unsatisfied2 < unsatisfied1)
					{
						bestResult = candidate;
					}
					else if (unsatisfied2 == unsatisfied1)
					{
						int dist1 = bestResult.TotalDistance;
						int dist2 = candidate.TotalDistance;
						if (dist2 < dist1)
						{
							bestResult = candidate;
						}
					}
				}
			}
			if (bestResult == null)
			{ // should only happen for no connections
				bestResult = new MoveResult(req, new ReplacementMap(), impossible, 0);
			}
			else
			{
				bestResult.addUnsatisfiedConnections(impossible);
			}
			return bestResult;
		}

		private static List<ConnectionData> pruneImpossible(List<ConnectionData> connects, AvoidanceMap avoid, int dx, int dy)
		{
			List<Wire> pathWires = new List<Wire>();
			foreach (ConnectionData conn in connects)
			{
				foreach (Wire w in conn.WirePath)
				{
					pathWires.Add(w);
				}
			}

			List<ConnectionData> impossible = new List<ConnectionData>();
			for (IEnumerator<ConnectionData> it = connects.GetEnumerator(); it.MoveNext();)
			{
				ConnectionData conn = it.Current;
				Location dest = conn.Location.translate(dx, dy);
				if (avoid.get(dest) != null)
				{
					bool isInPath = false;
					foreach (Wire w in pathWires)
					{
						if (w.contains(dest))
						{
							isInPath = true;
							break;
						}
					}
					if (!isInPath)
					{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
						it.remove();
						impossible.Add(conn);
					}
				}
			}
			return impossible;
		}

		/// <summary>
		/// Creates a list of the connections to make, sorted according to their location. If, for example, we are moving an
		/// east-facing AND gate southeast, then we prefer to connect the inputs from the top down to minimize the chances
		/// that the created wires will interfere with each other - but if we are moving that gate northeast, we prefer to
		/// connect the inputs from the bottom up.
		/// </summary>
		private static void sortConnects(List<ConnectionData> connects, in int dx, in int dy)
		{
			connects.Sort(new ComparatorAnonymousInnerClass(dx, dy));
		}

		private class ComparatorAnonymousInnerClass : IComparer<ConnectionData>
		{
			private int dx;
			private int dy;

			public ComparatorAnonymousInnerClass(int dx, int dy)
			{
				this.dx = dx;
				this.dy = dy;
			}

			public int Compare(ConnectionData ac, ConnectionData bc)
			{
				Location a = ac.Location;
				Location b = bc.Location;
				int abx = a.X - b.X;
				int aby = a.Y - b.Y;
				return abx * dx + aby * dy;
			}
		}

		private static void processConnection(ConnectionData conn, int dx, int dy, HashSet<Location> connLocs, List<SearchNode> connNodes, AvoidanceMap selAvoid)
		{
			Location cur = conn.Location;
			Location dest = cur.translate(dx, dy);
			if (selAvoid.get(cur) == null)
			{
				Direction preferred = conn.Direction;
				if (preferred == null)
				{
					if (Math.Abs(dx) > Math.Abs(dy))
					{
						preferred = dx > 0 ? Direction.East : Direction.West;
					}
					else
					{
						preferred = dy > 0 ? Direction.South : Direction.North;
					}
				}

				connLocs.Add(cur);
				connNodes.Add(new SearchNode(conn, cur, preferred, dest));
			}

			foreach (Wire w in conn.WirePath)
			{
				foreach (Location loc in w)
				{
					if (selAvoid.get(loc) == null || loc.Equals(dest))
					{
						bool added = connLocs.Add(loc);
						if (added)
						{
							Direction dir = null;
							if (w.endsAt(loc))
							{
								if (w.Vertical)
								{
									int y0 = loc.Y;
									int y1 = w.getOtherEnd(loc).Y;
									dir = y0 < y1 ? Direction.North : Direction.South;
								}
								else
								{
									int x0 = loc.X;
									int x1 = w.getOtherEnd(loc).X;
									dir = x0 < x1 ? Direction.West : Direction.East;
								}
							}
							connNodes.Add(new SearchNode(conn, loc, dir, dest));
						}
					}
				}
			}
		}

		private static MoveResult tryList(MoveRequest req, MoveGesture gesture, List<ConnectionData> connects, int dx, int dy, Dictionary<ConnectionData, ISet<Location>> pathLocs, Dictionary<ConnectionData, IList<SearchNode>> initNodes, long stopTime)
		{
			AvoidanceMap avoid = gesture.FixedAvoidanceMap.cloneMap();
			avoid.markAll(gesture.Selected, dx, dy);

			ReplacementMap replacements = new ReplacementMap();
			List<ConnectionData> unconnected = new List<ConnectionData>();
			int totalDistance = 0;
			foreach (ConnectionData conn in connects)
			{
				if (ConnectorThread.OverrideRequested)
				{
					return null;
				}
				if (DateTimeHelper.CurrentUnixTimeMillis() - stopTime > 0)
				{
					unconnected.Add(conn);
					continue;
				}
				IList<SearchNode> connNodes = initNodes[conn];
				ISet<Location> connPathLocs = pathLocs[conn];
				SearchNode n = findShortestPath(connNodes, connPathLocs, avoid);
				if (n != null)
				{ // normal case - a path was found
					totalDistance += n.Distance;
					List<Location> path = convertToPath(n);
					processPath(path, conn, avoid, replacements, connPathLocs);
				}
				else if (ConnectorThread.OverrideRequested)
				{
					return null; // search was aborted: return null to indicate this
				}
				else
				{
					unconnected.Add(conn);
				}
			}
			return new MoveResult(req, replacements, unconnected, totalDistance);
		}

		private static SearchNode findShortestPath(IList<SearchNode> nodes, ISet<Location> pathLocs, AvoidanceMap avoid)
		{
			PriorityQueue<SearchNode> q = new PriorityQueue<SearchNode>(nodes);
			HashSet<SearchNode> visited = new HashSet<SearchNode>();
			int iters = 0;
			while (!q.isEmpty() && iters < MAX_SEARCH_ITERATIONS)
			{
				iters++;
				SearchNode n = q.remove();
				if (iters % 64 == 0 && ConnectorThread.OverrideRequested || n == null)
				{
					return null;
				}
				if (n.Destination)
				{
					return n;
				}
				bool added = visited.Add(n);
				if (!added)
				{
					continue;
				}
				Location loc = n.Location;
				Direction dir = n.Direction;
				int neighbors = 3;
				object allowed = avoid.get(loc);
				if (allowed != null && n.Start && pathLocs.Contains(loc))
				{
					allowed = null;
				}
				if (allowed == ALLOW_NEITHER)
				{
					neighbors = 0;
				}
				else if (allowed == ALLOW_VERTICAL)
				{
					if (dir == null)
					{
						dir = Direction.North;
						neighbors = 2;
					}
					else if (dir == Direction.North || dir == Direction.South)
					{
						neighbors = 1;
					}
					else
					{
						neighbors = 0;
					}
				}
				else if (allowed == ALLOW_HORIZONTAL)
				{
					if (dir == null)
					{
						dir = Direction.East;
						neighbors = 2;
					}
					else if (dir == Direction.East || dir == Direction.West)
					{
						neighbors = 1;
					}
					else
					{
						neighbors = 0;
					}
				}
				else
				{
					if (dir == null)
					{
						dir = Direction.North;
						neighbors = 4;
					}
					else
					{
						neighbors = 3;
					}
				}
				for (int i = 0; i < neighbors; i++)
				{
					Direction oDir;
					switch (i)
					{
					case 0:
						oDir = dir;
						break;
					case 1:
						oDir = neighbors == 2 ? dir.reverse() : dir.rotateCCW();
						break;
					case 2:
						oDir = dir.rotateCW();
						break;
					default: // must be 3
						oDir = dir.reverse();
					break;
					}
					SearchNode o = n.next(oDir, allowed != null);
					if (o != null && !visited.Contains(o))
					{
						q.add(o);
					}
				}
			}
			return null;
		}

		private static List<Location> convertToPath(SearchNode last)
		{
			SearchNode next = last;
			SearchNode prev = last.Previous;
			List<Location> ret = new List<Location>();
			ret.Add(next.Location);
			while (prev != null)
			{
				if (prev.Direction != next.Direction)
				{
					ret.Add(prev.Location);
				}
				next = prev;
				prev = prev.Previous;
			}
			if (!ret[ret.Count - 1].Equals(next.Location))
			{
				ret.Add(next.Location);
			}
			ret.Reverse();
			return ret;
		}

		private static void processPath(List<Location> path, ConnectionData conn, AvoidanceMap avoid, ReplacementMap repl, ISet<Location> unmarkable)
		{
			IEnumerator<Location> pathIt = path.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Location loc0 = pathIt.next();
			if (!loc0.Equals(conn.Location))
			{
				Location pathLoc = conn.WirePathStart;
				bool found = loc0.Equals(pathLoc);
				foreach (Wire w in conn.WirePath)
				{
					Location nextLoc = w.getOtherEnd(pathLoc);
					if (found)
					{ // existing wire will be removed
						repl.remove(w);
						avoid.unmarkWire(w, nextLoc, unmarkable);
					}
					else if (w.contains(loc0))
					{ // wires after this will be removed
						found = true;
						if (!loc0.Equals(nextLoc))
						{
							avoid.unmarkWire(w, nextLoc, unmarkable);
							Wire shortenedWire = Wire.create(pathLoc, loc0);
							repl.replace(w, shortenedWire);
							avoid.markWire(shortenedWire, 0, 0);
						}
					}
					pathLoc = nextLoc;
				}
			}
			while (pathIt.MoveNext())
			{
				Location loc1 = pathIt.Current;
				Wire newWire = Wire.create(loc0, loc1);
				repl.add(newWire);
				avoid.markWire(newWire, 0, 0);
				loc0 = loc1;
			}
		}
	}

}
