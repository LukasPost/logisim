// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.move
{

	using Circuit = logisim.circuit.Circuit;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using EndData = logisim.comp.EndData;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;

	public class MoveGesture
	{
		private MoveRequestListener listener;
		private Circuit circuit;
		private HashSet<Component> selected;

		[NonSerialized]
		private ISet<ConnectionData> connections;
		[NonSerialized]
		private AvoidanceMap initAvoid;
		private Dictionary<MoveRequest, MoveResult> cachedResults;

		public MoveGesture(MoveRequestListener listener, Circuit circuit, ICollection<Component> selected)
		{
			this.listener = listener;
			this.circuit = circuit;
			this.selected = new HashSet<Component>(selected);
			this.connections = null;
			this.initAvoid = null;
			this.cachedResults = new Dictionary<MoveRequest, MoveResult>();
		}

		internal virtual HashSet<Component> Selected
		{
			get
			{
				return selected;
			}
		}

		internal virtual AvoidanceMap FixedAvoidanceMap
		{
			get
			{
				AvoidanceMap ret = initAvoid;
				if (ret == null)
				{
					HashSet<Component> comps = new HashSet<Component>(circuit.NonWires);
					comps.addAll(circuit.Wires);
					comps.RemoveAll(selected);
					ret = AvoidanceMap.create(comps, 0, 0);
					initAvoid = ret;
				}
				return ret;
			}
		}

		internal virtual ISet<ConnectionData> Connections
		{
			get
			{
				ISet<ConnectionData> ret = connections;
				if (ret == null)
				{
					ret = computeConnections(circuit, selected);
					connections = ret;
				}
				return ret;
			}
		}

		public virtual MoveResult findResult(int dx, int dy)
		{
			MoveRequest request = new MoveRequest(this, dx, dy);
			lock (cachedResults)
			{
				return cachedResults[request];
			}
		}

		public virtual bool enqueueRequest(int dx, int dy)
		{
			MoveRequest request = new MoveRequest(this, dx, dy);
			lock (cachedResults)
			{
				object result = cachedResults[request];
				if (result == null)
				{
					ConnectorThread.enqueueRequest(request, false);
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public virtual MoveResult forceRequest(int dx, int dy)
		{
			MoveRequest request = new MoveRequest(this, dx, dy);
			ConnectorThread.enqueueRequest(request, true);
			lock (cachedResults)
			{
				object result = cachedResults[request];
				while (result == null)
				{
					try
					{
						Monitor.Wait(cachedResults);
					}
					catch (InterruptedException)
					{
						Thread.CurrentThread.Interrupt();
						return null;
					}
					result = cachedResults[request];
				}
				return (MoveResult) result;
			}
		}

		internal virtual void notifyResult(MoveRequest request, MoveResult result)
		{
			lock (cachedResults)
			{
				cachedResults[request] = result;
				Monitor.PulseAll(cachedResults);
			}
			if (listener != null)
			{
				listener.requestSatisfied(this, request.DeltaX, request.DeltaY);
			}
		}

		private static ISet<ConnectionData> computeConnections(Circuit circuit, ISet<Component> selected)
		{
			if (selected == null || selected.Count == 0)
			{
				return Collections.emptySet();
			}

			// first identify locations that might be connected
			ISet<Location> locs = new HashSet<Location>();
			foreach (Component comp in selected)
			{
				foreach (EndData end in comp.Ends)
				{
					locs.Add(end.Location);
				}
			}

			// now see which of them require connection
			ISet<ConnectionData> conns = new HashSet<ConnectionData>();
			foreach (Location loc in locs)
			{
				bool found = false;
				foreach (Component comp in circuit.getComponents(loc))
				{
					if (!selected.Contains(comp))
					{
						found = true;
						break;
					}
				}
				if (found)
				{
					IList<Wire> wirePath;
					Location wirePathStart;
					Wire lastOnPath = findWire(circuit, loc, selected, null);
					if (lastOnPath == null)
					{
						wirePath = Collections.emptyList();
						wirePathStart = loc;
					}
					else
					{
						wirePath = new List<Wire>();
						Location cur = loc;
						for (Wire w = lastOnPath; w != null; w = findWire(circuit, cur, selected, w))
						{
							wirePath.Add(w);
							cur = w.getOtherEnd(cur);
						}
						wirePath.Reverse();
						wirePathStart = cur;
					}

					Direction dir = null;
					if (lastOnPath != null)
					{
						Location other = lastOnPath.getOtherEnd(loc);
						int dx = loc.X - other.X;
						int dy = loc.Y - other.Y;
						if (Math.Abs(dx) > Math.Abs(dy))
						{
							dir = dx > 0 ? Direction.East : Direction.West;
						}
						else
						{
							dir = dy > 0 ? Direction.South : Direction.North;
						}
					}
					conns.Add(new ConnectionData(loc, dir, wirePath, wirePathStart));
				}
			}
			return conns;
		}

		private static Wire findWire(Circuit circ, Location loc, ISet<Component> ignore, Wire ignoreW)
		{
			Wire ret = null;
			foreach (Component comp in circ.getComponents(loc))
			{
				if (!ignore.Contains(comp) && comp != ignoreW)
				{
					if (ret == null && comp is Wire)
					{
						ret = (Wire) comp;
					}
					else
					{
						return null;
					}
				}
			}
			return ret;
		}

	}

}
