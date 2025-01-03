// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using CircuitAppearance = logisim.circuit.appear.CircuitAppearance;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentEvent = logisim.comp.ComponentEvent;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using ComponentListener = logisim.comp.ComponentListener;
	using EndData = logisim.comp.EndData;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Clock = logisim.std.wiring.Clock;
	using CollectionUtil = logisim.util.CollectionUtil;
	using logisim.util;
    using LogisimPlus.Java;

    public class Circuit
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myComponentListener = new MyComponentListener(this);
		}

		private class EndChangedTransaction : CircuitTransaction
		{
			private readonly Circuit outerInstance;

			internal Component comp;
			internal Dictionary<Location, EndData> toRemove;
			internal Dictionary<Location, EndData> toAdd;

			internal EndChangedTransaction(Circuit outerInstance, Component comp, Dictionary<Location, EndData> toRemove, Dictionary<Location, EndData> toAdd)
			{
				this.outerInstance = outerInstance;
				this.comp = comp;
				this.toRemove = toRemove;
				this.toAdd = toAdd;
			}

			protected internal override Dictionary<Circuit, int> AccessedCircuits
			{
				get
				{
					return Collections.singletonMap(outerInstance, READ_WRITE);
				}
			}

			protected internal override void run(CircuitMutator mutator)
			{
				foreach (Location loc in toRemove.Keys)
				{
					EndData removed = toRemove[loc];
					EndData replaced = toAdd.Remove(loc);
					if (replaced == null)
					{
						outerInstance.wires.remove(comp, removed);
					}
					else if (!replaced.Equals(removed))
					{
						outerInstance.wires.replace(comp, removed, replaced);
					}
				}
				foreach (EndData end in toAdd.Values)
				{
					outerInstance.wires.add(comp, end);
				}
				((CircuitMutatorImpl) mutator).markModified(outerInstance);
			}
		}

		private class MyComponentListener : ComponentListener
		{
			private readonly Circuit outerInstance;

			public MyComponentListener(Circuit outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void endChanged(ComponentEvent e)
			{
				outerInstance.locker.checkForWritePermission("ends changed");
				Component comp = e.Source;
				Dictionary<Location, EndData> toRemove = toMap(e.OldData);
				Dictionary<Location, EndData> toAdd = toMap(e.Data);
				EndChangedTransaction xn = new EndChangedTransaction(outerInstance, comp, toRemove, toAdd);
				outerInstance.locker.execute(xn);
				outerInstance.fireEvent(CircuitEvent.ACTION_INVALIDATE, comp);
			}

			internal virtual Dictionary<Location, EndData> toMap(object val)
			{
				Dictionary<Location, EndData> map = new Dictionary<Location, EndData>();
				if (val is System.Collections.List)
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<logisim.comp.EndData> valList = (java.util.List<logisim.comp.EndData>) val;
					List<EndData> valList = (List<EndData>) val;
					foreach (EndData end in valList)
					{
						if (end != null)
						{
							map[end.Location] = end;
						}
					}
				}
				else if (val is EndData)
				{
					EndData end = (EndData) val;
					map[end.Location] = end;
				}
				return map;
			}

			public virtual void componentInvalidated(ComponentEvent e)
			{
				outerInstance.fireEvent(CircuitEvent.ACTION_INVALIDATE, e.Source);
			}
		}

		private MyComponentListener myComponentListener;
		private CircuitAppearance appearance;
		private AttributeSet staticAttrs;
		private SubcircuitFactory subcircuitFactory;
		private EventSourceWeakSupport<CircuitListener> listeners = new EventSourceWeakSupport<CircuitListener>();
		private HashSet<Component> comps = new HashSet<Component>(); // doesn't include wires
		internal CircuitWires wires = new CircuitWires();
		// wires is package-protected for CircuitState and Analyze only.
		private List<Component> clocks = new List<Component>();
		private CircuitLocker locker;
		private WeakHashMap<Component, Circuit> circuitsUsingThis;

		public Circuit(string name)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			appearance = new CircuitAppearance(this);
			staticAttrs = CircuitAttributes.createBaseAttrs(this, name);
			subcircuitFactory = new SubcircuitFactory(this);
			locker = new CircuitLocker();
			circuitsUsingThis = new WeakHashMap<Component, Circuit>();
		}

		internal virtual CircuitLocker Locker
		{
			get
			{
				return locker;
			}
		}

		public virtual ICollection<Circuit> CircuitsUsingThis
		{
			get
			{
				return circuitsUsingThis.values();
			}
		}

		public virtual void mutatorClear()
		{
			locker.checkForWritePermission("clear");

			HashSet<Component> oldComps = comps;
			comps = new HashSet<Component>();
			wires = new CircuitWires();
			clocks.Clear();
			foreach (Component comp in oldComps)
			{
				if (comp.Factory is SubcircuitFactory)
				{
					SubcircuitFactory sub = (SubcircuitFactory) comp.Factory;
					sub.Subcircuit.circuitsUsingThis.remove(comp);
				}
			}
			fireEvent(CircuitEvent.ACTION_CLEAR, oldComps);
		}

		public override string ToString()
		{
			return staticAttrs.getValue(CircuitAttributes.NAME_ATTR);
		}

		public virtual AttributeSet StaticAttributes
		{
			get
			{
				return staticAttrs;
			}
		}

		//
		// Listener methods
		//
		public virtual void addCircuitListener(CircuitListener what)
		{
			listeners.add(what);
		}

		public virtual void removeCircuitListener(CircuitListener what)
		{
			listeners.remove(what);
		}

		internal virtual void fireEvent(int action, object data)
		{
			fireEvent(new CircuitEvent(action, this, data));
		}

		private void fireEvent(CircuitEvent @event)
		{
			foreach (CircuitListener l in listeners)
			{
				l.circuitChanged(@event);
			}
		}

		//
		// access methods
		//
		public virtual string Name
		{
			get
			{
				return staticAttrs.getValue(CircuitAttributes.NAME_ATTR);
			}
			set
			{
				staticAttrs.setValue(CircuitAttributes.NAME_ATTR, value);
			}
		}

		public virtual CircuitAppearance Appearance
		{
			get
			{
				return appearance;
			}
		}

		public virtual SubcircuitFactory SubcircuitFactory
		{
			get
			{
				return subcircuitFactory;
			}
		}

		public virtual HashSet<WidthIncompatibilityData> WidthIncompatibilityData
		{
			get
			{
				return wires.WidthIncompatibilityData;
			}
		}

		public virtual BitWidth getWidth(Location p)
		{
			return wires.getWidth(p);
		}

		public virtual Location getWidthDeterminant(Location p)
		{
			return wires.getWidthDeterminant(p);
		}

		public virtual bool hasConflict(Component comp)
		{
			return wires.points.hasConflict(comp);
		}

		public virtual Component getExclusive(Location loc)
		{
			return wires.points.getExclusive(loc);
		}

		private HashSet<Component> Components
		{
			get
			{
				return CollectionUtil.createUnmodifiableSetUnion(comps, wires.Wires);
			}
		}

		public virtual bool contains(Component c)
		{
			return comps.Contains(c) || wires.Wires.Contains(c);
		}

		public virtual HashSet<Wire> Wires
		{
			get
			{
				return wires.Wires;
			}
		}

		public virtual HashSet<Component> NonWires
		{
			get
			{
				return comps;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.Collection<? extends logisim.comp.Component> getComponents(logisim.data.Location loc)
		public virtual ICollection<Component> getComponents(Location loc)
		{
			return wires.points.getComponents(loc);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.Collection<? extends logisim.comp.Component> getSplitCauses(logisim.data.Location loc)
		public virtual ICollection<Component> getSplitCauses(Location loc)
		{
			return wires.points.getSplitCauses(loc);
		}

		public virtual ICollection<Wire> getWires(Location loc)
		{
			return wires.points.getWires(loc);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.Collection<? extends logisim.comp.Component> getNonWires(logisim.data.Location loc)
		public virtual ICollection<Component> getNonWires(Location loc)
		{
			return wires.points.getNonWires(loc);
		}

		public virtual bool isConnected(Location loc, Component ignore)
		{
			foreach (Component o in wires.points.getComponents(loc))
			{
				if (o != ignore)
				{
					return true;
				}
			}
			return false;
		}

		public virtual HashSet<Location> SplitLocations
		{
			get
			{
				return wires.points.SplitLocations;
			}
		}

		public virtual ICollection<Component> getAllContaining(Location pt)
		{
			HashSet<Component> ret = new HashSet<Component>();
			foreach (Component comp in Components)
			{
				if (comp.contains(pt))
				{
					ret.Add(comp);
				}
			}
			return ret;
		}

		public virtual ICollection<Component> getAllContaining(Location pt, JGraphics g)
		{
			HashSet<Component> ret = new HashSet<Component>();
			foreach (Component comp in Components)
			{
				if (comp.contains(pt, g))
				{
					ret.Add(comp);
				}
			}
			return ret;
		}

		public virtual ICollection<Component> getAllWithin(Bounds bds)
		{
			HashSet<Component> ret = new HashSet<Component>();
			foreach (Component comp in Components)
			{
				if (bds.contains(comp.Bounds))
				{
					ret.Add(comp);
				}
			}
			return ret;
		}

		public virtual ICollection<Component> getAllWithin(Bounds bds, JGraphics g)
		{
			HashSet<Component> ret = new HashSet<Component>();
			foreach (Component comp in Components)
			{
				if (bds.contains(comp.getBounds(g)))
				{
					ret.Add(comp);
				}
			}
			return ret;
		}

		public virtual WireSet getWireSet(Wire start)
		{
			return wires.getWireSet(start);
		}

		public virtual Bounds Bounds
		{
			get
			{
				Bounds wireBounds = wires.WireBounds;
				IEnumerator<Component> it = comps.GetEnumerator();
	// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (!it.hasNext())
				{
					return wireBounds;
				}
	// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Component first = it.next();
				Bounds firstBounds = first.Bounds;
				int xMin = firstBounds.X;
				int yMin = firstBounds.Y;
				int xMax = xMin + firstBounds.Width;
				int yMax = yMin + firstBounds.Height;
				while (it.MoveNext())
				{
					Component c = it.Current;
					Bounds bds = c.Bounds;
					int x0 = bds.X;
					int x1 = x0 + bds.Width;
					int y0 = bds.Y;
					int y1 = y0 + bds.Height;
					if (x0 < xMin)
					{
						xMin = x0;
					}
					if (x1 > xMax)
					{
						xMax = x1;
					}
					if (y0 < yMin)
					{
						yMin = y0;
					}
					if (y1 > yMax)
					{
						yMax = y1;
					}
				}
				Bounds compBounds = Bounds.create(xMin, yMin, xMax - xMin, yMax - yMin);
				if (wireBounds.Width == 0 || wireBounds.Height == 0)
				{
					return compBounds;
				}
				else
				{
					return compBounds.add(wireBounds);
				}
			}
		}

		public virtual Bounds getBounds(JGraphics g)
		{
			Bounds ret = wires.WireBounds;
			int xMin = ret.X;
			int yMin = ret.Y;
			int xMax = xMin + ret.Width;
			int yMax = yMin + ret.Height;
			if (ret == Bounds.EMPTY_BOUNDS)
			{
				xMin = int.MaxValue;
				yMin = int.MaxValue;
				xMax = int.MinValue;
				yMax = int.MinValue;
			}
			foreach (Component c in comps)
			{
				Bounds bds = c.getBounds(g);
				if (bds != null && bds != Bounds.EMPTY_BOUNDS)
				{
					int x0 = bds.X;
					int x1 = x0 + bds.Width;
					int y0 = bds.Y;
					int y1 = y0 + bds.Height;
					if (x0 < xMin)
					{
						xMin = x0;
					}
					if (x1 > xMax)
					{
						xMax = x1;
					}
					if (y0 < yMin)
					{
						yMin = y0;
					}
					if (y1 > yMax)
					{
						yMax = y1;
					}
				}
			}
			if (xMin > xMax || yMin > yMax)
			{
				return Bounds.EMPTY_BOUNDS;
			}
			return Bounds.create(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		internal virtual List<Component> Clocks
		{
			get
			{
				return clocks;
			}
		}


		internal virtual void mutatorAdd(Component c)
		{
			locker.checkForWritePermission("add");

			if (c is Wire)
			{
				Wire w = (Wire) c;
				if (w.End0.Equals(w.End1))
				{
					return;
				}
				bool added = wires.add(w);
				if (!added)
				{
					return;
				}
			}
			else
			{
				// add it into the circuit
				bool added = comps.Add(c);
				if (!added)
				{
					return;
				}

				wires.add(c);
				ComponentFactory factory = c.Factory;
				if (factory is Clock)
				{
					clocks.Add(c);
				}
				else if (factory is SubcircuitFactory)
				{
					SubcircuitFactory subcirc = (SubcircuitFactory) factory;
					subcirc.Subcircuit.circuitsUsingThis.put(c, this);
				}
				c.addComponentListener(myComponentListener);
			}
			fireEvent(CircuitEvent.ACTION_ADD, c);
		}

		internal virtual void mutatorRemove(Component c)
		{
			locker.checkForWritePermission("remove");

			if (c is Wire)
			{
				wires.remove(c);
			}
			else
			{
				wires.remove(c);
				comps.Remove(c);
				ComponentFactory factory = c.Factory;
				if (factory is Clock)
				{
					clocks.Remove(c);
				}
				else if (factory is SubcircuitFactory)
				{
					SubcircuitFactory subcirc = (SubcircuitFactory) factory;
					subcirc.Subcircuit.circuitsUsingThis.remove(c);
				}
				c.removeComponentListener(myComponentListener);
			}
			fireEvent(CircuitEvent.ACTION_REMOVE, c);
		}

		//
		// JGraphics methods
		//
		public virtual void draw(ComponentDrawContext context, ICollection<Component> hidden)
		{
			JGraphics g = context.Graphics;
			JGraphics g_copy = g.create();
			context.Graphics = g_copy;
			wires.draw(context, hidden);

			if (hidden == null || hidden.Count == 0)
			{
				foreach (Component c in comps)
				{
					JGraphics g_new = g.create();
					context.Graphics = g_new;
					g_copy.dispose();
					g_copy = g_new;

					c.draw(context);
				}
			}
			else
			{
				foreach (Component c in comps)
				{
					if (!hidden.Contains(c))
					{
						JGraphics g_new = g.create();
						context.Graphics = g_new;
						g_copy.dispose();
						g_copy = g_new;

						try
						{
							c.draw(context);
						}
						catch (Exception e)
						{
							// this is a JAR developer error - display it and move on
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}
					}
				}
			}
			context.Graphics = g;
			g_copy.dispose();
		}

		//
		// helper methods for other classes in package
		//
		public static bool isInput(Component comp)
		{
			return comp.getEnd(0).Type != EndData.INPUT_ONLY;
		}
	}

}
