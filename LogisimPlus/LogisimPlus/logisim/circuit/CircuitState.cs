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

	using SetData = logisim.circuit.Propagator.SetData;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentState = logisim.comp.ComponentState;
	using BitWidth = logisim.data.BitWidth;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceData = logisim.instance.InstanceData;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstanceState = logisim.instance.InstanceState;
	using Project = logisim.proj.Project;
	using Clock = logisim.std.wiring.Clock;
	using Pin = logisim.std.wiring.Pin;
	using logisim.util;
	using logisim.util;

	public class CircuitState : InstanceData
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myCircuitListener = new MyCircuitListener(this);
		}

		private class MyCircuitListener : CircuitListener
		{
			private readonly CircuitState outerInstance;

			public MyCircuitListener(CircuitState outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				int action = @event.Action;
				if (action == CircuitEvent.ACTION_ADD)
				{
					Component comp = (Component) @event.Data;
					if (comp is Wire)
					{
						Wire w = (Wire) comp;
						outerInstance.markPointAsDirty(w.End0);
						outerInstance.markPointAsDirty(w.End1);
					}
					else
					{
						outerInstance.markComponentAsDirty(comp);
					}
				}
				else if (action == CircuitEvent.ACTION_REMOVE)
				{
					Component comp = (Component) @event.Data;
					if (comp.Factory is SubcircuitFactory)
					{
						// disconnect from tree
						CircuitState substate = (CircuitState) outerInstance.getData(comp);
						if (substate != null && substate.parentComp == comp)
						{
							outerInstance.substates.remove(substate);
							substate.parentState = null;
							substate.parentComp = null;
						}
					}

					if (comp is Wire)
					{
						Wire w = (Wire) comp;
						outerInstance.markPointAsDirty(w.End0);
						outerInstance.markPointAsDirty(w.End1);
					}
					else
					{
						if (outerInstance.@base != null)
						{
							outerInstance.@base.checkComponentEnds(outerInstance, comp);
						}
						outerInstance.dirtyComponents.remove(comp);
					}
				}
				else if (action == CircuitEvent.ACTION_CLEAR)
				{
					outerInstance.substates.clear();
					outerInstance.wireData = null;
					outerInstance.componentData.Clear();
					outerInstance.values.Clear();
					outerInstance.dirtyComponents.clear();
					outerInstance.dirtyPoints.clear();
					outerInstance.causes.Clear();
				}
				else if (action == CircuitEvent.ACTION_CHANGE)
				{
					object data = @event.Data;
					if (data is System.Collections.ICollection)
					{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Collection<logisim.comp.Component> comps = (java.util.Collection<logisim.comp.Component>) data;
						ICollection<Component> comps = (ICollection<Component>) data;
						outerInstance.markComponentsDirty(comps);
						if (outerInstance.@base != null)
						{
							foreach (Component comp in comps)
							{
								outerInstance.@base.checkComponentEnds(outerInstance, comp);
							}
						}
					}
					else
					{
						Component comp = (Component) @event.Data;
						outerInstance.markComponentAsDirty(comp);
						if (outerInstance.@base != null)
						{
							outerInstance.@base.checkComponentEnds(outerInstance, comp);
						}
					}
				}
				else if (action == CircuitEvent.ACTION_INVALIDATE)
				{
					Component comp = (Component) @event.Data;
					outerInstance.markComponentAsDirty(comp);
					// TODO detemine if this should really be missing if (base != null)
					// base.checkComponentEnds(CircuitState.this, comp);
				}
				else if (action == CircuitEvent.TRANSACTION_DONE)
				{
					ReplacementMap map = @event.Result.getReplacementMap(outerInstance.circuit);
					if (map != null)
					{
						foreach (Component comp in map.ReplacedComponents)
						{
							object compState = outerInstance.componentData.Remove(comp);
							if (compState != null)
							{
								Type compFactory = comp.Factory.GetType();
								bool found = false;
								foreach (Component repl in map.get(comp))
								{
									if (repl.Factory.GetType() == compFactory)
									{
										found = true;
										outerInstance.setData(repl, compState);
										break;
									}
								}
								if (!found && compState is CircuitState)
								{
									CircuitState sub = (CircuitState) compState;
									sub.parentState = null;
									outerInstance.substates.Remove(sub);
								}
							}
						}
					}
				}
			}
		}

		private MyCircuitListener myCircuitListener;
		private Propagator @base = null; // base of tree of CircuitStates
		private Project proj; // project where circuit lies
		private Circuit circuit; // circuit being simulated

		private CircuitState parentState = null; // parent in tree of CircuitStates
		private Component parentComp = null; // subcircuit component containing this state
		private ArraySet<CircuitState> substates = new ArraySet<CircuitState>();

		private CircuitWires.State wireData = null;
		private Dictionary<Component, object> componentData = new Dictionary<Component, object>();
		private Dictionary<Location, Value> values = new Dictionary<Location, Value>();
		private SmallSet<Component> dirtyComponents = new SmallSet<Component>();
		private SmallSet<Location> dirtyPoints = new SmallSet<Location>();
		internal Dictionary<Location, SetData> causes = new Dictionary<Location, SetData>();

		private static int lastId = 0;
		private int id = lastId++;

		public CircuitState(Project proj, Circuit circuit)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.proj = proj;
			this.circuit = circuit;
			circuit.addCircuitListener(myCircuitListener);
		}

		public virtual Project Project
		{
			get
			{
				return proj;
			}
		}

		internal virtual Component Subcircuit
		{
			get
			{
				return parentComp;
			}
		}

		public virtual CircuitState clone()
		{
			return cloneState();
		}

		public virtual CircuitState cloneState()
		{
			CircuitState ret = new CircuitState(proj, circuit);
			ret.copyFrom(this, new Propagator(ret));
			ret.parentComp = null;
			ret.parentState = null;
			return ret;
		}

		private void copyFrom(CircuitState src, Propagator @base)
		{
			this.@base = @base;
			this.parentComp = src.parentComp;
			this.parentState = src.parentState;
			Dictionary<CircuitState, CircuitState> substateData = new Dictionary<CircuitState, CircuitState>();
			this.substates = new ArraySet<CircuitState>();
			foreach (CircuitState oldSub in src.substates)
			{
				CircuitState newSub = new CircuitState(src.proj, oldSub.circuit);
				newSub.copyFrom(oldSub, @base);
				newSub.parentState = this;
				this.substates.Add(newSub);
				substateData[oldSub] = newSub;
			}
			foreach (Component key in src.componentData.Keys)
			{
				object oldValue = src.componentData[key];
				if (oldValue is CircuitState cs)
				{
					object newValue = substateData[cs];
					if (newValue != null)
					{
						this.componentData[key] = newValue;
					}
					else
					{
						this.componentData.Remove(key);
					}
				}
				else
				{
					object newValue;
					if (oldValue is ComponentState)
					{
						newValue = ((ComponentState) oldValue).clone();
					}
					else
					{
						newValue = oldValue;
					}
					this.componentData[key] = newValue;
				}
			}
			foreach (Location key in src.causes.Keys)
			{
				Propagator.SetData oldValue = src.causes[key];
				Propagator.SetData newValue = oldValue.cloneFor(this);
				this.causes[key] = newValue;
			}
			if (src.wireData != null)
			{
				this.wireData = (CircuitWires.State) src.wireData.clone();
			}
			this.values.PutAll(src.values);
			this.dirtyComponents.AddRange(src.dirtyComponents);
			this.dirtyPoints.AddRange(src.dirtyPoints);
		}

		public override string ToString()
		{
			return "State" + id + "[" + circuit.Name + "]";
		}

		//
		// public methods
		//
		public virtual Circuit Circuit
		{
			get
			{
				return circuit;
			}
		}

		public virtual CircuitState ParentState
		{
			get
			{
				return parentState;
			}
		}

		public virtual ICollection<CircuitState> Substates
		{
			get
			{ // returns Set of CircuitStates
				return substates;
			}
		}

		public virtual Propagator Propagator
		{
			get
			{
				if (@base == null)
				{
					@base = new Propagator(this);
					markAllComponentsDirty();
				}
				return @base;
			}
		}

		public virtual void drawOscillatingPoints(ComponentDrawContext context)
		{
			if (@base != null)
			{
				@base.drawOscillatingPoints(context);
			}
		}

		public virtual object getData(Component comp)
		{
			return componentData[comp];
		}

		public virtual void setData(Component comp, object data)
		{
			if (data is CircuitState)
			{
				CircuitState oldState = (CircuitState) componentData[comp];
				CircuitState newState = (CircuitState) data;
				if (oldState != newState)
				{
					// There's something new going on with this subcircuit.
					// Maybe the subcircuit is new, or perhaps it's being
					// removed.
					if (oldState != null && oldState.parentComp == comp)
					{
						// it looks like it's being removed
						substates.Remove(oldState);
						oldState.parentState = null;
						oldState.parentComp = null;
					}
					if (newState != null && newState.parentState != this)
					{
						// this is the first time I've heard about this CircuitState
						substates.Add(newState);
						newState.@base = this.@base;
						newState.parentState = this;
						newState.parentComp = comp;
						newState.markAllComponentsDirty();
					}
				}
			}
			componentData[comp] = data;
		}

		public virtual Value getValue(Location pt)
		{
			Value ret = values[pt];
			if (ret != null)
			{
				return ret;
			}

			BitWidth wid = circuit.getWidth(pt);
			return Value.createUnknown(wid);
		}

		public virtual void setValue(Location pt, Value val, Component cause, int delay)
		{
			if (@base != null)
			{
				@base.setValue(this, pt, val, cause, delay);
			}
		}

		public virtual void markComponentAsDirty(Component comp)
		{
			try
			{
				dirtyComponents.Add(comp);
			}
			catch (Exception)
			{
				SmallSet<Component> set = new SmallSet<Component>();
				set.Add(comp);
				dirtyComponents = set;
			}
		}

		public virtual void markComponentsDirty(ICollection<Component> comps)
		{
			dirtyComponents.AddRange(comps);
		}

		public virtual void markPointAsDirty(Location pt)
		{
			dirtyPoints.Add(pt);
		}

		public virtual InstanceState getInstanceState(Component comp)
		{
			object factory = comp.Factory;
			if (factory is InstanceFactory)
			{
				return ((InstanceFactory) factory).createInstanceState(this, comp);
			}
			else
			{
				throw new Exception("getInstanceState requires instance component");
			}
		}

		public virtual InstanceState getInstanceState(Instance instance)
		{
			object factory = instance.Factory;
			if (factory is InstanceFactory)
			{
				return ((InstanceFactory) factory).createInstanceState(this, instance);
			}
			else
			{
				throw new Exception("getInstanceState requires instance component");
			}
		}

		//
		// methods for other classes within package
		//
		public virtual bool Substate
		{
			get
			{
				return parentState != null;
			}
		}

		internal virtual void processDirtyComponents()
		{
			if (dirtyComponents.Any())
			{
				// This seeming wasted copy is to avoid ConcurrentModifications
				// if we used an iterator instead.
				object[] toProcess;
				Exception firstException = null;
				for (int tries = 4; true; tries--)
				{
					try
					{
						toProcess = dirtyComponents.ToArray();
						break;
					}
					catch (Exception e)
					{
						if (firstException == null)
						{
							firstException = e;
						}
						if (tries == 0)
						{
							toProcess = new object[0];
							dirtyComponents = new SmallSet<Component>();
							throw firstException;
						}
					}
				}
				dirtyComponents.Clear();
				foreach (object compObj in toProcess)
				{
					if (compObj is Component)
					{
						Component comp = (Component) compObj;
						comp.propagate(this);
						if (comp.Factory is Pin && parentState != null)
						{
							// should be propagated in superstate
							parentComp.propagate(parentState);
						}
					}
				}
			}

			CircuitState[] subs = new CircuitState[substates.Count];
			foreach (CircuitState substate in substates.ToArray(subs))
			{
				substate.processDirtyComponents();
			}
		}

		internal virtual void processDirtyPoints()
		{
			HashSet<Location> dirty = new HashSet<Location>(dirtyPoints);
			dirtyPoints.Clear();
			if (circuit.wires.isMapVoided())
			{
				for (int i = 3; i >= 0; i--)
				{
					try
					{
						dirty.AddRange(circuit.wires.points.getSplitLocations());
						break;
					}
					catch (ConcurrentModificationException e)
					{
						// try again...
						try
						{
							Thread.Sleep(1);
						}
						catch (InterruptedException)
						{
						}
						if (i == 0)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}
					}
				}
			}
			if (dirty.Count > 0)
			{
				circuit.wires.propagate(this, dirty);
			}

			CircuitState[] subs = new CircuitState[substates.Count];
			foreach (CircuitState substate in substates.ToArray(subs))
			{
				substate.processDirtyPoints();
			}
		}

		internal virtual void reset()
		{
			wireData = null;
			for (IEnumerator<Component> it = componentData.Keys.GetEnumerator(); it.MoveNext();)
			{
				Component comp = it.Current;
				if (!(comp.Factory is SubcircuitFactory))
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
			}
			values.Clear();
			dirtyComponents.clear();
			dirtyPoints.clear();
			causes.Clear();
			markAllComponentsDirty();

			foreach (CircuitState sub in substates)
			{
				sub.reset();
			}
		}

		internal virtual bool tick(int ticks)
		{
			bool ret = false;
			foreach (Component clock in circuit.Clocks)
			{
				ret |= Clock.tick(this, ticks, clock);
			}

			CircuitState[] subs = new CircuitState[substates.size()];
			foreach (CircuitState substate in substates.toArray(subs))
			{
				ret |= substate.tick(ticks);
			}
			return ret;
		}

		internal virtual CircuitWires.State WireData
		{
			get
			{
				return wireData;
			}
			set
			{
				wireData = value;
			}
		}


		internal virtual Value getComponentOutputAt(Location p)
		{
			// for CircuitWires - to get values, ignoring wires' contributions
			Propagator.SetData cause_list = causes[p];
			return logisim.circuit.Propagator.computeValue(cause_list);
		}

		internal virtual Value getValueByWire(Location p)
		{
			return values[p];
		}

		internal virtual void setValueByWire(Location p, Value v)
		{
			// for CircuitWires - to set value at point
			bool changed;
			if (v == Value.NIL)
			{
				object old = values.Remove(p);
				changed = (old != null && old != Value.NIL);
			}
			else
			{
				object old = values[p] = v;
				changed = !v.Equals(old);
			}
			if (changed)
			{
				bool found = false;
				foreach (Component comp in circuit.getComponents(p))
				{
					if (!(comp is Wire) && !(comp is Splitter))
					{
						found = true;
						markComponentAsDirty(comp);
					}
				}
				// NOTE: this will cause a double-propagation on components
				// whose outputs have just changed.

				if (found && @base != null)
				{
					@base.locationTouched(this, p);
				}
			}
		}

		//
		// private methods
		//
		private void markAllComponentsDirty()
		{
			dirtyComponents.addAll(circuit.NonWires);
		}
	}

}
