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

	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using EndData = logisim.comp.EndData;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Options = logisim.file.Options;

	public class Propagator
	{
		internal class SetData : IComparable<SetData>
		{
			internal int time;
			internal int serialNumber;
			internal CircuitState state; // state of circuit containing component
			internal Component cause; // component emitting the value
			internal Location loc; // the location at which value is emitted
			internal Value val; // value being emitted
			internal SetData next = null;

			internal SetData(int time, int serialNumber, CircuitState state, Location loc, Component cause, Value val)
			{
				this.time = time;
				this.serialNumber = serialNumber;
				this.state = state;
				this.cause = cause;
				this.loc = loc;
				this.val = val;
			}

			public virtual int CompareTo(SetData o)
			{
				// Yes, these subtractions may overflow. This is intentional, as it
				// avoids potential wraparound problems as the counters increment.
				int ret = this.time - o.time;
				if (ret != 0)
				{
					return ret;
				}
				return this.serialNumber - o.serialNumber;
			}

			public virtual SetData cloneFor(CircuitState newState)
			{
				Propagator newProp = newState.Propagator;
				int dtime = newProp.clock - state.Propagator.clock;
				SetData ret = new SetData(time + dtime, newProp.setDataSerialNumber, newState, loc, cause, val);
				newProp.setDataSerialNumber++;
				if (this.next != null)
				{
					ret.next = this.next.cloneFor(newState);
				}
				return ret;
			}

			public override string ToString()
			{
				return loc + ":" + val + "(" + cause + ")";
			}
		}

		private class ComponentPoint
		{
			internal Component cause;
			internal Location loc;

			public ComponentPoint(Component cause, Location loc)
			{
				this.cause = cause;
				this.loc = loc;
			}

			public override int GetHashCode()
			{
				return 31 * cause.GetHashCode() + loc.GetHashCode();
			}

			public override bool Equals(object other)
			{
				if (!(other is ComponentPoint))
				{
					return false;
				}
				ComponentPoint o = (ComponentPoint) other;
				return this.cause.Equals(o.cause) && this.loc.Equals(o.loc);
			}
		}

		private class Listener : AttributeListener
		{
			internal WeakReference<Propagator> prop;

			public Listener(Propagator propagator)
			{
				prop = new WeakReference<Propagator>(propagator);
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				Propagator p = prop.get();
				if (p == null)
				{
					e.Source.removeAttributeListener(this);
				}
				else if (e.Attribute.Equals(Options.sim_rand_attr))
				{
					p.updateRandomness();
				}
			}
		}

		private CircuitState root; // root of state tree

		/// <summary>
		/// The number of clock cycles to let pass before deciding that the circuit is oscillating.
		/// </summary>
		private int simLimit = 1000;

		/// <summary>
		/// On average, one out of every 2**simRandomShift propagations through a component is delayed one step more than the
		/// component requests. This noise is intended to address some circuits that would otherwise oscillate within Logisim
		/// (though they wouldn't oscillate in practice).
		/// </summary>
		private volatile int simRandomShift;

		private PriorityQueue<SetData> toProcess = new PriorityQueue<SetData>();
		private int clock = 0;
		private bool isOscillating = false;
		private bool oscAdding = false;
		private PropagationPoints oscPoints = new PropagationPoints();
		private int ticks = 0;
		private Random noiseSource = new Random();
		private int noiseCount = 0;
		private int setDataSerialNumber = 0;

		internal static int lastId = 0;
		internal int id = lastId++;

		public Propagator(CircuitState root)
		{
			this.root = root;
			Listener l = new Listener(this);
			root.Project.Options.AttributeSet.addAttributeListener(l);
			updateRandomness();
		}

		private void updateRandomness()
		{
			Options opts = root.Project.Options;
			object rand = opts.AttributeSet.getValue(Options.sim_rand_attr);
			int val = ((int?) rand).Value;
			int logVal = 0;
			while ((1 << logVal) < val)
			{
				logVal++;
			}
			simRandomShift = logVal;
		}

		public virtual bool Oscillating
		{
			get
			{
				return isOscillating;
			}
		}

		public override string ToString()
		{
			return "Prop" + id;
		}

		public virtual void drawOscillatingPoints(ComponentDrawContext context)
		{
			if (isOscillating)
			{
				oscPoints.draw(context);
			}
		}

		//
		// public methods
		//
		internal virtual CircuitState RootState
		{
			get
			{
				return root;
			}
		}

		internal virtual void reset()
		{
			toProcess.clear();
			root.reset();
			isOscillating = false;
		}

		public virtual void propagate()
		{
			oscPoints.clear();
			clearDirtyPoints();
			clearDirtyComponents();

			int oscThreshold = simLimit;
			int logThreshold = 3 * oscThreshold / 4;
			int iters = 0;
			while (!toProcess.isEmpty())
			{
				iters++;

				if (iters < logThreshold)
				{
					stepInternal(null);
				}
				else if (iters < oscThreshold)
				{
					oscAdding = true;
					stepInternal(oscPoints);
				}
				else
				{
					isOscillating = true;
					oscAdding = false;
					return;
				}
			}
			isOscillating = false;
			oscAdding = false;
			oscPoints.clear();
		}

		internal virtual void step(PropagationPoints changedPoints)
		{
			oscPoints.clear();
			clearDirtyPoints();
			clearDirtyComponents();

			PropagationPoints oldOsc = oscPoints;
			oscAdding = changedPoints != null;
			oscPoints = changedPoints;
			stepInternal(changedPoints);
			oscAdding = false;
			oscPoints = oldOsc;
		}

		private void stepInternal(PropagationPoints changedPoints)
		{
			if (toProcess.isEmpty())
			{
				return;
			}

			// update clock
			clock = toProcess.peek().time;

			// propagate all values for this clock tick
			Dictionary<CircuitState, HashSet<ComponentPoint>> visited = new Dictionary<CircuitState, HashSet<ComponentPoint>>();
			while (true)
			{
				SetData data = toProcess.peek();
				if (data == null || data.time != clock)
				{
					break;
				}
				toProcess.remove();
				CircuitState state = data.state;

				// if it's already handled for this clock tick, continue
				HashSet<ComponentPoint> handled = visited[state];
				if (handled != null)
				{
					if (!handled.Add(new ComponentPoint(data.cause, data.loc)))
					{
						continue;
					}
				}
				else
				{
					handled = new HashSet<ComponentPoint>();
					visited[state] = handled;
					handled.Add(new ComponentPoint(data.cause, data.loc));
				}

				/*
				 * DEBUGGING - comment out Simulator.log(data.time + ": proc " + data.loc + " in " + data.state + " to " +
				 * data.val + " by " + data.cause); //
				 */

				if (changedPoints != null)
				{
					changedPoints.add(state, data.loc);
				}

				// change the information about value
				SetData oldHead = state.causes[data.loc];
				Value oldVal = computeValue(oldHead);
				SetData newHead = addCause(state, oldHead, data);
				Value newVal = computeValue(newHead);

				// if the value at point has changed, propagate it
				if (!newVal.Equals(oldVal))
				{
					state.markPointAsDirty(data.loc);
				}
			}

			clearDirtyPoints();
			clearDirtyComponents();
		}

		internal virtual bool Pending
		{
			get
			{
				return !toProcess.isEmpty();
			}
		}

		/*
		 * TODO for the SimulatorPrototype class void step() { clock++;
		 * 
		 * // propagate all values for this clock tick HashMap visited = new HashMap(); // State -> set of ComponentPoints
		 * handled while (!toProcess.isEmpty()) { SetData data; data = (SetData) toProcess.peek(); if (data.time != clock)
		 * break; toProcess.remove(); CircuitState state = data.state;
		 * 
		 * // if it's already handled for this clock tick, continue HashSet handled = (HashSet) visited.get(state); if
		 * (handled != null) { if (!handled.add(new ComponentPoint(data.cause, data.loc))) continue; } else { handled = new
		 * HashSet(); visited.put(state, handled); handled.add(new ComponentPoint(data.cause, data.loc)); }
		 * 
		 * if (oscAdding) oscPoints.add(state, data.loc);
		 * 
		 * // change the information about value SetData oldHead = (SetData) state.causes.get(data.loc); Value oldVal =
		 * computeValue(oldHead); SetData newHead = addCause(state, oldHead, data); Value newVal = computeValue(newHead);
		 * 
		 * // if the value at point has changed, propagate it if (!newVal.equals(oldVal)) {
		 * state.markPointAsDirty(data.loc); } }
		 * 
		 * clearDirtyPoints(); clearDirtyComponents(); }
		 */

		internal virtual void locationTouched(CircuitState state, Location loc)
		{
			if (oscAdding)
			{
				oscPoints.add(state, loc);
			}
		}

		//
		// package-protected helper methods
		//
		internal virtual void setValue(CircuitState state, Location pt, Value val, Component cause, int delay)
		{
			if (cause is Wire || cause is Splitter)
			{
				return;
			}
			if (delay <= 0)
			{
				delay = 1;
			}
			int randomShift = simRandomShift;
			if (randomShift > 0)
			{ // random noise is turned on
				// multiply the delay by 32 so that the random noise
				// only changes the delay by 3%.
				delay <<= randomShift;
				if (!(cause.Factory is SubcircuitFactory))
				{
					if (noiseCount > 0)
					{
						noiseCount--;
					}
					else
					{
						delay++;
						noiseCount = noiseSource.Next(1 << randomShift);
					}
				}
			}
			toProcess.add(new SetData(clock + delay, setDataSerialNumber, state, pt, cause, val));
			/*
			 * DEBUGGING - comment out Simulator.log(clock + ": set " + pt + " in " + state + " to " + val + " by " + cause
			 * + " after " + delay); //
			 */

			setDataSerialNumber++;
		}

		public virtual bool tick()
		{
			ticks++;
			return root.tick(ticks);
		}

		public virtual int TickCount
		{
			get
			{
				return ticks;
			}
		}

		//
		// private methods
		//
		internal virtual void checkComponentEnds(CircuitState state, Component comp)
		{
			foreach (EndData end in comp.Ends)
			{
				Location loc = end.Location;
				SetData oldHead = state.causes[loc];
				Value oldVal = computeValue(oldHead);
				SetData newHead = removeCause(state, oldHead, loc, comp);
				Value newVal = computeValue(newHead);
				Value wireVal = state.getValueByWire(loc);

				if (!newVal.Equals(oldVal) || wireVal != null)
				{
					state.markPointAsDirty(loc);
				}
				if (wireVal != null)
				{
					state.setValueByWire(loc, Value.NIL);
				}
			}
		}

		private void clearDirtyPoints()
		{
			root.processDirtyPoints();
		}

		private void clearDirtyComponents()
		{
			root.processDirtyComponents();
		}

		private SetData addCause(CircuitState state, SetData head, SetData data)
		{
			if (data.val == null)
			{ // actually, it should be removed
				return removeCause(state, head, data.loc, data.cause);
			}

			Dictionary<Location, SetData> causes = state.causes;

			// first check whether this is change of previous info.
			bool replaced = false;
			for (SetData n = head; n != null; n = n.next)
			{
				if (n.cause == data.cause)
				{
					n.val = data.val;
					replaced = true;
					break;
				}
			}

			// otherwise, insert to list of causes
			if (!replaced)
			{
				if (head == null)
				{
					causes[data.loc] = data;
					head = data;
				}
				else
				{
					data.next = head.next;
					head.next = data;
				}
			}

			return head;
		}

		private SetData removeCause(CircuitState state, SetData head, Location loc, Component cause)
		{
			Dictionary<Location, SetData> causes = state.causes;
			if (head == null)
			{
				;
			}
			else if (head.cause == cause)
			{
				head = head.next;
				if (head == null)
				{
					causes.Remove(loc);
				}
				else
				{
					causes[loc] = head;
				}
			}
			else
			{
				SetData prev = head;
				SetData cur = head.next;
				while (cur != null)
				{
					if (cur.cause == cause)
					{
						prev.next = cur.next;
						break;
					}
					prev = cur;
					cur = cur.next;
				}
			}
			return head;
		}

		//
		// static methods
		//
		internal static Value computeValue(SetData causes)
		{
			if (causes == null)
			{
				return Value.NIL;
			}
			Value ret = causes.val;
			for (SetData n = causes.next; n != null; n = n.next)
			{
				ret = ret.combine(n.val);
			}
			return ret;
		}

	}

}
