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

namespace logisim.circuit
{

	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using EndData = logisim.comp.EndData;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using StdAttr = logisim.instance.StdAttr;
	using PullResistor = logisim.std.wiring.PullResistor;
	using Tunnel = logisim.std.wiring.Tunnel;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using IteratorUtil = logisim.util.IteratorUtil;
	using logisim.util;

	internal class CircuitWires
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			tunnelListener = new TunnelListener(this);
		}

		internal class SplitterData
		{
			internal WireBundle[] end_bundle; // PointData associated with each end

			internal SplitterData(int fan_out)
			{
				end_bundle = new WireBundle[fan_out + 1];
			}
		}

		internal class ThreadBundle
		{
			internal int loc;
			internal WireBundle b;

			internal ThreadBundle(int loc, WireBundle b)
			{
				this.loc = loc;
				this.b = b;
			}
		}

		internal class State
		{
			internal BundleMap bundleMap;
			internal Dictionary<WireThread, Value> thr_values = new Dictionary<WireThread, Value>();

			internal State(BundleMap bundleMap)
			{
				this.bundleMap = bundleMap;
			}

			public override object clone()
			{
				State ret = new State(this.bundleMap);
				ret.thr_values.PutAll(this.thr_values);
				return ret;
			}
		}

		private class TunnelListener : AttributeListener
		{
			private readonly CircuitWires outerInstance;

			public TunnelListener(CircuitWires outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = e.getAttribute();
				Attribute<object> attr = e.Attribute;
				if (attr == StdAttr.LABEL || attr == PullResistor.ATTR_PULL_TYPE)
				{
					outerInstance.voidBundleMap();
				}
			}
		}

		internal class BundleMap
		{
			internal bool computed = false;
			internal Dictionary<Location, WireBundle> pointBundles = new Dictionary<Location, WireBundle>();
			internal HashSet<WireBundle> bundles = new HashSet<WireBundle>();
			internal bool isValid = true;
			// NOTE: It would make things more efficient if we also had
			// a set of just the first bundle in each tree.
			internal HashSet<WidthIncompatibilityData> incompatibilityData = null;

			internal virtual HashSet<WidthIncompatibilityData> WidthIncompatibilityData
			{
				get
				{
					return incompatibilityData;
				}
			}

			internal virtual void addWidthIncompatibilityData(WidthIncompatibilityData e)
			{
				if (incompatibilityData == null)
				{
					incompatibilityData = new HashSet<WidthIncompatibilityData>();
				}
				incompatibilityData.Add(e);
			}

			internal virtual WireBundle getBundleAt(Location p)
			{
				return pointBundles[p];
			}

			internal virtual WireBundle createBundleAt(Location p)
			{
				WireBundle ret = pointBundles[p];
				if (ret == null)
				{
					ret = new WireBundle();
					pointBundles[p] = ret;
					ret.points.add(p);
					bundles.Add(ret);
				}
				return ret;
			}

			internal virtual bool Valid
			{
				get
				{
					return isValid;
				}
			}

			internal virtual void invalidate()
			{
				isValid = false;
			}

			internal virtual void setBundleAt(Location p, WireBundle b)
			{
				pointBundles[p] = b;
			}

			internal virtual ISet<Location> BundlePoints
			{
				get
				{
					return pointBundles.Keys;
				}
			}

			internal virtual ISet<WireBundle> Bundles
			{
				get
				{
					return bundles;
				}
			}

			internal virtual void markComputed()
			{
				lock (this)
				{
					computed = true;
					Monitor.PulseAll(this);
				}
			}

			internal virtual void waitUntilComputed()
			{
				lock (this)
				{
					while (!computed)
					{
						try
						{
							Monitor.Wait(this);
						}
						catch (InterruptedException)
						{
						}
					}
				}
			}
		}

		// user-given data
		private HashSet<Wire> wires = new HashSet<Wire>();
		private HashSet<Splitter> splitters = new HashSet<Splitter>();
		private HashSet<Component> tunnels = new HashSet<Component>(); // of Components with Tunnel factory
		private TunnelListener tunnelListener;
		private HashSet<Component> pulls = new HashSet<Component>(); // of Components with PullResistor factory
		internal readonly CircuitPoints points = new CircuitPoints();

		// derived data
		private Bounds bounds = Bounds.EMPTY_BOUNDS;
		private BundleMap bundleMap = null;

		internal CircuitWires()
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
		}

		//
		// query methods
		//
		internal virtual bool MapVoided
		{
			get
			{
				return bundleMap == null;
			}
		}

		internal virtual ISet<WidthIncompatibilityData> WidthIncompatibilityData
		{
			get
			{
				return BundleMap.WidthIncompatibilityData;
			}
		}

		internal virtual void ensureComputed()
		{
			BundleMap;
		}

		internal virtual BitWidth getWidth(Location q)
		{
			BitWidth det = points.getWidth(q);
			if (det != BitWidth.UNKNOWN)
			{
				return det;
			}

			BundleMap bmap = BundleMap;
			if (!bmap.Valid)
			{
				return BitWidth.UNKNOWN;
			}
			WireBundle qb = bmap.getBundleAt(q);
			if (qb != null && qb.Valid)
			{
				return qb.Width;
			}

			return BitWidth.UNKNOWN;
		}

		internal virtual Location getWidthDeterminant(Location q)
		{
			BitWidth det = points.getWidth(q);
			if (det != BitWidth.UNKNOWN)
			{
				return q;
			}

			WireBundle qb = BundleMap.getBundleAt(q);
			if (qb != null && qb.Valid)
			{
				return qb.WidthDeterminant;
			}

			return q;
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Iterator<? extends logisim.comp.Component> getComponents()
		internal virtual IEnumerator<Component> Components
		{
			get
			{
				return IteratorUtil.createJoinedIterator(splitters.GetEnumerator(), wires.GetEnumerator());
			}
		}

		internal virtual ISet<Wire> Wires
		{
			get
			{
				return wires;
			}
		}

		internal virtual Bounds WireBounds
		{
			get
			{
				Bounds bds = bounds;
				if (bds == Bounds.EMPTY_BOUNDS)
				{
					bds = recomputeBounds();
				}
				return bds;
			}
		}

		internal virtual WireBundle getWireBundle(Location query)
		{
			BundleMap bmap = BundleMap;
			return bmap.getBundleAt(query);
		}

		internal virtual WireSet getWireSet(Wire start)
		{
			WireBundle bundle = getWireBundle(start.e0);
			if (bundle == null)
			{
				return WireSet.EMPTY;
			}
			HashSet<Wire> wires = new HashSet<Wire>();
			foreach (Location loc in bundle.points)
			{
				wires.addAll(points.getWires(loc));
			}
			return new WireSet(wires);
		}

		//
		// action methods
		//
		// NOTE: this could be made much more efficient in most cases to
		// avoid voiding the bundle map.
		internal virtual bool add(Component comp)
		{
			bool added = true;
			if (comp is Wire)
			{
				added = addWire((Wire) comp);
			}
			else if (comp is Splitter)
			{
				splitters.Add((Splitter) comp);
			}
			else
			{
				object factory = comp.Factory;
				if (factory is Tunnel)
				{
					tunnels.Add(comp);
					comp.AttributeSet.addAttributeListener(tunnelListener);
				}
				else if (factory is PullResistor)
				{
					pulls.Add(comp);
					comp.AttributeSet.addAttributeListener(tunnelListener);
				}
			}
			if (added)
			{
				points.add(comp);
				voidBundleMap();
			}
			return added;
		}

		internal virtual void remove(Component comp)
		{
			if (comp is Wire)
			{
				removeWire((Wire) comp);
			}
			else if (comp is Splitter)
			{
				splitters.Remove(comp);
			}
			else
			{
				object factory = comp.Factory;
				if (factory is Tunnel)
				{
					tunnels.Remove(comp);
					comp.AttributeSet.removeAttributeListener(tunnelListener);
				}
				else if (factory is PullResistor)
				{
					pulls.Remove(comp);
					comp.AttributeSet.removeAttributeListener(tunnelListener);
				}
			}
			points.remove(comp);
			voidBundleMap();
		}

		internal virtual void add(Component comp, EndData end)
		{
			points.add(comp, end);
			voidBundleMap();
		}

		internal virtual void remove(Component comp, EndData end)
		{
			points.remove(comp, end);
			voidBundleMap();
		}

		internal virtual void replace(Component comp, EndData oldEnd, EndData newEnd)
		{
			points.remove(comp, oldEnd);
			points.add(comp, newEnd);
			voidBundleMap();
		}

		private bool addWire(Wire w)
		{
			bool added = wires.Add(w);
			if (!added)
			{
				return false;
			}

			if (bounds != Bounds.EMPTY_BOUNDS)
			{ // update bounds
				bounds = bounds.add(w.e0).add(w.e1);
			}
			return true;
		}

		private void removeWire(Wire w)
		{
			bool removed = wires.Remove(w);
			if (!removed)
			{
				return;
			}

			if (bounds != Bounds.EMPTY_BOUNDS)
			{
				// bounds is valid - invalidate if endpoint on border
				Bounds smaller = bounds.expand(-2);
				if (!smaller.contains(w.e0) || !smaller.contains(w.e1))
				{
					bounds = Bounds.EMPTY_BOUNDS;
				}
			}
		}

		//
		// utility methods
		//
		internal virtual void propagate(CircuitState circState, ISet<Location> points)
		{
			BundleMap map = BundleMap;
			SmallSet<WireThread> dirtyThreads = new SmallSet<WireThread>(); // affected threads

			// get state, or create a new one if current state is outdated
			State s = circState.WireData;
			if (s == null || s.bundleMap != map)
			{
				// if it is outdated, we need to compute for all threads
				s = new State(map);
				foreach (WireBundle b in map.Bundles)
				{
					WireThread[] th = b.threads;
					if (b.Valid && th != null)
					{
						foreach (WireThread t in th)
						{
							dirtyThreads.add(t);
						}
					}
				}
				circState.WireData = s;
			}

			// determine affected threads, and set values for unwired points
			foreach (Location p in points)
			{
				WireBundle pb = map.getBundleAt(p);
				if (pb == null)
				{ // point is not wired
					circState.setValueByWire(p, circState.getComponentOutputAt(p));
				}
				else
				{
					WireThread[] th = pb.threads;
					if (!pb.Valid || th == null)
					{
						// immediately propagate NILs across invalid bundles
						SmallSet<Location> pbPoints = pb.points;
						if (pbPoints == null)
						{
							circState.setValueByWire(p, Value.NIL);
						}
						else
						{
							foreach (Location loc2 in pbPoints)
							{
								circState.setValueByWire(loc2, Value.NIL);
							}
						}
					}
					else
					{
						foreach (WireThread t in th)
						{
							dirtyThreads.add(t);
						}
					}
				}
			}

			if (dirtyThreads.Empty)
			{
				return;
			}

			// determine values of affected threads
			HashSet<ThreadBundle> bundles = new HashSet<ThreadBundle>();
			foreach (WireThread t in dirtyThreads)
			{
				Value v = getThreadValue(circState, t);
				s.thr_values[t] = v;
				bundles.addAll(t.Bundles);
			}

			// now propagate values through circuit
			foreach (ThreadBundle tb in bundles)
			{
				WireBundle b = tb.b;

				Value bv = null;
				if (!b.Valid || b.threads == null)
				{
					; // do nothing
				}
				else if (b.threads.Length == 1)
				{
					bv = s.thr_values[b.threads[0]];
				}
				else
				{
					Value[] tvs = new Value[b.threads.Length];
					bool tvs_valid = true;
					for (int i = 0; i < tvs.Length; i++)
					{
						Value tv = s.thr_values[b.threads[i]];
						if (tv == null)
						{
							tvs_valid = false;
							break;
						}
						tvs[i] = tv;
					}
					if (tvs_valid)
					{
						bv = Value.create(tvs);
					}
				}

				if (bv != null)
				{
					foreach (Location p in b.points)
					{
						circState.setValueByWire(p, bv);
					}
				}
			}
		}

		internal virtual void draw(ComponentDrawContext context, ICollection<Component> hidden)
		{
			bool showState = context.ShowState;
			CircuitState state = context.CircuitState;
			Graphics g = context.Graphics;
			g.setColor(Color.BLACK);
			GraphicsUtil.switchToWidth(g, Wire.WIDTH);
			WireSet highlighted = context.HighlightedWires;

			BundleMap bmap = BundleMap;
			bool isValid = bmap.Valid;
			if (hidden == null || hidden.Count == 0)
			{
				foreach (Wire w in wires)
				{
					Location s = w.e0;
					Location t = w.e1;
					WireBundle wb = bmap.getBundleAt(s);
					if (!wb.Valid)
					{
						g.setColor(Value.WIDTH_ERROR_COLOR);
					}
					else if (showState)
					{
						if (!isValid)
						{
							g.setColor(Value.NIL_COLOR);
						}
						else
						{
							g.setColor(state.getValue(s).Color);
						}
					}
					else
					{
						g.setColor(Color.BLACK);
					}
					if (highlighted.containsWire(w))
					{
						GraphicsUtil.switchToWidth(g, Wire.WIDTH + 2);
						g.drawLine(s.X, s.Y, t.X, t.Y);
						GraphicsUtil.switchToWidth(g, Wire.WIDTH);
					}
					else
					{
						g.drawLine(s.X, s.Y, t.X, t.Y);
					}
				}

				foreach (Location loc in points.SplitLocations)
				{
					if (points.getComponentCount(loc) > 2)
					{
						WireBundle wb = bmap.getBundleAt(loc);
						if (wb != null)
						{
							if (!wb.Valid)
							{
								g.setColor(Value.WIDTH_ERROR_COLOR);
							}
							else if (showState)
							{
								if (!isValid)
								{
									g.setColor(Value.NIL_COLOR);
								}
								else
								{
									g.setColor(state.getValue(loc).Color);
								}
							}
							else
							{
								g.setColor(Color.BLACK);
							}
							if (highlighted.containsLocation(loc))
							{
								g.fillOval(loc.X - 5, loc.Y - 5, 10, 10);
							}
							else
							{
								g.fillOval(loc.X - 4, loc.Y - 4, 8, 8);
							}
						}
					}
				}
			}
			else
			{
				foreach (Wire w in wires)
				{
					if (!hidden.Contains(w))
					{
						Location s = w.e0;
						Location t = w.e1;
						WireBundle wb = bmap.getBundleAt(s);
						if (!wb.Valid)
						{
							g.setColor(Value.WIDTH_ERROR_COLOR);
						}
						else if (showState)
						{
							if (!isValid)
							{
								g.setColor(Value.NIL_COLOR);
							}
							else
							{
								g.setColor(state.getValue(s).Color);
							}
						}
						else
						{
							g.setColor(Color.BLACK);
						}
						if (highlighted.containsWire(w))
						{
							GraphicsUtil.switchToWidth(g, Wire.WIDTH + 2);
							g.drawLine(s.X, s.Y, t.X, t.Y);
							GraphicsUtil.switchToWidth(g, Wire.WIDTH);
						}
						else
						{
							g.drawLine(s.X, s.Y, t.X, t.Y);
						}
					}
				}

				// this is just an approximation, but it's good enough since
				// the problem is minor, and hidden only exists for a short
				// while at a time anway.
				foreach (Location loc in points.SplitLocations)
				{
					if (points.getComponentCount(loc) > 2)
					{
						int icount = 0;
						foreach (Component comp in points.getComponents(loc))
						{
							if (!hidden.Contains(comp))
							{
								++icount;
							}
						}
						if (icount > 2)
						{
							WireBundle wb = bmap.getBundleAt(loc);
							if (wb != null)
							{
								if (!wb.Valid)
								{
									g.setColor(Value.WIDTH_ERROR_COLOR);
								}
								else if (showState)
								{
									if (!isValid)
									{
										g.setColor(Value.NIL_COLOR);
									}
									else
									{
										g.setColor(state.getValue(loc).Color);
									}
								}
								else
								{
									g.setColor(Color.BLACK);
								}
								if (highlighted.containsLocation(loc))
								{
									g.fillOval(loc.X - 5, loc.Y - 5, 10, 10);
								}
								else
								{
									g.fillOval(loc.X - 4, loc.Y - 4, 8, 8);
								}
							}
						}
					}
				}
			}
		}

		//
		// helper methods
		//
		private void voidBundleMap()
		{
			bundleMap = null;
		}

		private BundleMap BundleMap
		{
			get
			{
				// Maybe we already have a valid bundle map (or maybe
				// one is in progress).
				BundleMap ret = bundleMap;
				if (ret != null)
				{
					ret.waitUntilComputed();
					return ret;
				}
				try
				{
					// Ok, we have to create our own.
					for (int tries = 4; tries >= 0; tries--)
					{
						try
						{
							ret = new BundleMap();
							computeBundleMap(ret);
							bundleMap = ret;
							break;
						}
						catch (Exception t)
						{
							if (tries == 0)
							{
								Console.WriteLine(t.ToString());
								Console.Write(t.StackTrace);
								bundleMap = ret;
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (ret != null)
					{
						ret.invalidate();
						ret.markComputed();
					}
					throw ex;
				}
				finally
				{
					// Mark the BundleMap as computed in case anybody is waiting for the result.
					if (ret != null)
					{
						ret.markComputed();
					}
				}
				return ret;
			}
		}

		// To be called by getBundleMap only
		private void computeBundleMap(BundleMap ret)
		{
			// create bundles corresponding to wires and tunnels
			connectWires(ret);
			connectTunnels(ret);
			connectPullResistors(ret);

			// merge any WireBundle objects united by previous steps
			for (IEnumerator<WireBundle> it = ret.Bundles.GetEnumerator(); it.MoveNext();)
			{
				WireBundle b = it.Current;
				WireBundle bpar = b.find();
				if (bpar != b)
				{ // b isn't group's representative
					foreach (Location pt in b.points)
					{
						ret.setBundleAt(pt, bpar);
						bpar.points.add(pt);
					}
					bpar.addPullValue(b.PullValue);
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
			}

			// make a WireBundle object for each end of a splitter
			foreach (Splitter spl in splitters)
			{
				IList<EndData> ends = new List<EndData>(spl.Ends);
				foreach (EndData end in ends)
				{
					Location p = end.Location;
					WireBundle pb = ret.createBundleAt(p);
					pb.setWidth(end.Width, p);
				}
			}

			// set the width for each bundle whose size is known
			// based on components
			foreach (Location p in ret.BundlePoints)
			{
				WireBundle pb = ret.getBundleAt(p);
				BitWidth width = points.getWidth(p);
				if (width != BitWidth.UNKNOWN)
				{
					pb.setWidth(width, p);
				}
			}

			// determine the bundles at the end of each splitter
			foreach (Splitter spl in splitters)
			{
				IList<EndData> ends = new List<EndData>(spl.Ends);
				int index = -1;
				foreach (EndData end in ends)
				{
					index++;
					Location p = end.Location;
					WireBundle pb = ret.getBundleAt(p);
					if (pb != null)
					{
						pb.setWidth(end.Width, p);
						spl.wire_data.end_bundle[index] = pb;
					}
				}
			}

			// unite threads going through splitters
			foreach (Splitter spl in splitters)
			{
				lock (spl)
				{
					SplitterAttributes spl_attrs = (SplitterAttributes) spl.AttributeSet;
					sbyte[] bit_end = spl_attrs.bit_end;
					SplitterData spl_data = spl.wire_data;
					WireBundle from_bundle = spl_data.end_bundle[0];
					if (from_bundle == null || !from_bundle.Valid)
					{
						continue;
					}

					for (int i = 0; i < bit_end.Length; i++)
					{
						int j = bit_end[i];
						if (j > 0)
						{
							int thr = spl.bit_thread[i];
							WireBundle to_bundle = spl_data.end_bundle[j];
							WireThread[] to_threads = to_bundle.threads;
							if (to_threads != null && to_bundle.Valid)
							{
								WireThread[] from_threads = from_bundle.threads;
								if (i >= from_threads.Length)
								{
									throw new System.IndexOutOfRangeException("from " + i + " of " + from_threads.Length);
								}
								if (thr >= to_threads.Length)
								{
									throw new System.IndexOutOfRangeException("to " + thr + " of " + to_threads.Length);
								}
								from_threads[i].unite(to_threads[thr]);
							}
						}
					}
				}
			}

			// merge any threads united by previous step
			foreach (WireBundle b in ret.Bundles)
			{
				if (b.Valid && b.threads != null)
				{
					for (int i = 0; i < b.threads.Length; i++)
					{
						WireThread thr = b.threads[i].find();
						b.threads[i] = thr;
						thr.Bundles.add(new ThreadBundle(i, b));
					}
				}
			}

			// All threads are sewn together! Compute the exception set before leaving
			ICollection<WidthIncompatibilityData> exceptions = points.WidthIncompatibilityData;
			if (exceptions != null && exceptions.Count > 0)
			{
				foreach (WidthIncompatibilityData wid in exceptions)
				{
					ret.addWidthIncompatibilityData(wid);
				}
			}
			foreach (WireBundle b in ret.Bundles)
			{
				WidthIncompatibilityData e = b.WidthIncompatibilityData;
				if (e != null)
				{
					ret.addWidthIncompatibilityData(e);
				}
			}
		}

		private void connectWires(BundleMap ret)
		{
			// make a WireBundle object for each tree of connected wires
			foreach (Wire w in wires)
			{
				WireBundle b0 = ret.getBundleAt(w.e0);
				if (b0 == null)
				{
					WireBundle b1 = ret.createBundleAt(w.e1);
					b1.points.add(w.e0);
					ret.setBundleAt(w.e0, b1);
				}
				else
				{
					WireBundle b1 = ret.getBundleAt(w.e1);
					if (b1 == null)
					{ // t1 doesn't exist
						b0.points.add(w.e1);
						ret.setBundleAt(w.e1, b0);
					}
					else
					{
						b1.unite(b0); // unite b0 and b1
					}
				}
			}
		}

		private void connectTunnels(BundleMap ret)
		{
			// determine the sets of tunnels
			Dictionary<string, List<Location>> tunnelSets = new Dictionary<string, List<Location>>();
			foreach (Component comp in tunnels)
			{
				string label = comp.AttributeSet.getValue(StdAttr.LABEL);
				label = label.Trim();
				if (!label.Equals(""))
				{
					List<Location> tunnelSet = tunnelSets[label];
					if (tunnelSet == null)
					{
						tunnelSet = new List<Location>(3);
						tunnelSets[label] = tunnelSet;
					}
					tunnelSet.Add(comp.Location);
				}
			}

			// now connect the bundles that are tunnelled together
			foreach (List<Location> tunnelSet in tunnelSets.Values)
			{
				WireBundle foundBundle = null;
				Location foundLocation = null;
				foreach (Location loc in tunnelSet)
				{
					WireBundle b = ret.getBundleAt(loc);
					if (b != null)
					{
						foundBundle = b;
						foundLocation = loc;
						break;
					}
				}
				if (foundBundle == null)
				{
					foundLocation = tunnelSet[0];
					foundBundle = ret.createBundleAt(foundLocation);
				}
				foreach (Location loc in tunnelSet)
				{
					if (loc != foundLocation)
					{
						WireBundle b = ret.getBundleAt(loc);
						if (b == null)
						{
							foundBundle.points.add(loc);
							ret.setBundleAt(loc, foundBundle);
						}
						else
						{
							b.unite(foundBundle);
						}
					}
				}
			}
		}

		private void connectPullResistors(BundleMap ret)
		{
			foreach (Component comp in pulls)
			{
				Location loc = comp.getEnd(0).Location;
				WireBundle b = ret.getBundleAt(loc);
				if (b == null)
				{
					b = ret.createBundleAt(loc);
					b.points.add(loc);
					ret.setBundleAt(loc, b);
				}
				Instance instance = Instance.getInstanceFor(comp);
				b.addPullValue(PullResistor.getPullValue(instance));
			}
		}

		private Value getThreadValue(CircuitState state, WireThread t)
		{
			Value ret = Value.UNKNOWN;
			Value pull = Value.UNKNOWN;
			foreach (ThreadBundle tb in t.Bundles)
			{
				foreach (Location p in tb.b.points)
				{
					Value val = state.getComponentOutputAt(p);
					if (val != null && val != Value.NIL)
					{
						ret = ret.combine(val.get(tb.loc));
					}
				}
				Value pullHere = tb.b.PullValue;
				if (pullHere != Value.UNKNOWN)
				{
					pull = pull.combine(pullHere);
				}
			}
			if (pull != Value.UNKNOWN)
			{
				ret = pullValue(ret, pull);
			}
			return ret;
		}

		private static Value pullValue(Value @base, Value pullTo)
		{
			if (@base.FullyDefined)
			{
				return @base;
			}
			else if (@base.Width == 1)
			{
				if (@base == Value.UNKNOWN)
				{
					return pullTo;
				}
				else
				{
					return @base;
				}
			}
			else
			{
				Value[] ret = @base.All;
				for (int i = 0; i < ret.Length; i++)
				{
					if (ret[i] == Value.UNKNOWN)
					{
						ret[i] = pullTo;
					}
				}
				return Value.create(ret);
			}
		}

		private Bounds recomputeBounds()
		{
			IEnumerator<Wire> it = wires.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (!it.hasNext())
			{
				bounds = Bounds.EMPTY_BOUNDS;
				return Bounds.EMPTY_BOUNDS;
			}

// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Wire w = it.next();
			int xmin = w.e0.X;
			int ymin = w.e0.Y;
			int xmax = w.e1.X;
			int ymax = w.e1.Y;
			while (it.MoveNext())
			{
				w = it.Current;
				int x0 = w.e0.X;
				if (x0 < xmin)
				{
					xmin = x0;
				}
				int x1 = w.e1.X;
				if (x1 > xmax)
				{
					xmax = x1;
				}
				int y0 = w.e0.Y;
				if (y0 < ymin)
				{
					ymin = y0;
				}
				int y1 = w.e1.Y;
				if (y1 > ymax)
				{
					ymax = y1;
				}
			}
			Bounds bds = Bounds.create(xmin, ymin, xmax - xmin + 1, ymax - ymin + 1);
			bounds = bds;
			return bds;
		}
	}

}
