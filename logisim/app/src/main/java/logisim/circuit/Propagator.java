/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.lang.ref.WeakReference;
import java.util.HashMap;
import java.util.HashSet;
import java.util.PriorityQueue;
import java.util.Random;

import logisim.comp.Component;
import logisim.comp.ComponentDrawContext;
import logisim.comp.EndData;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.file.Options;

public class Propagator {
	static class SetData implements Comparable<SetData> {
		int time;
		int serialNumber;
		CircuitState state; // state of circuit containing component
		Component cause; // component emitting the value
		Location loc; // the location at which value is emitted
		WireValue val; // value being emitted
		SetData next;

		private SetData(int time, int serialNumber, CircuitState state, Location loc, Component cause, WireValue val) {
			this.time = time;
			this.serialNumber = serialNumber;
			this.state = state;
			this.cause = cause;
			this.loc = loc;
			this.val = val;
		}

		public int compareTo(SetData o) {
			// Yes, these subtractions may overflow. This is intentional, as it
			// avoids potential wraparound problems as the counters increment.
			int ret = time - o.time;
			if (ret != 0)
				return ret;
			return serialNumber - o.serialNumber;
		}

		public SetData cloneFor(CircuitState newState) {
			Propagator newProp = newState.getPropagator();
			int dtime = newProp.clock - state.getPropagator().clock;
			SetData ret = new SetData(time + dtime, newProp.setDataSerialNumber, newState, loc, cause, val);
			newProp.setDataSerialNumber++;
			if (next != null)
				ret.next = next.cloneFor(newState);
			return ret;
		}

		@Override
		public String toString() {
			return loc + ":" + val + "(" + cause + ")";
		}
	}

	private static class ComponentPoint {
		Component cause;
		Location loc;

		public ComponentPoint(Component cause, Location loc) {
			this.cause = cause;
			this.loc = loc;
		}

		@Override
		public int hashCode() {
			return 31 * cause.hashCode() + loc.hashCode();
		}

		@Override
		public boolean equals(Object other) {
			return other instanceof ComponentPoint o && cause.equals(o.cause) && loc.equals(o.loc);
		}
	}

	private static class Listener implements AttributeListener {
		WeakReference<Propagator> prop;

		public Listener(Propagator propagator) {
			prop = new WeakReference<>(propagator);
		}

		public void attributeListChanged(AttributeEvent e) {
		}

		public void attributeValueChanged(AttributeEvent e) {
			Propagator p = prop.get();
			if (p == null) e.getSource().removeAttributeListener(this);
			else if (e.getAttribute().equals(Options.sim_rand_attr)) p.updateRandomness();
		}
	}

	private CircuitState root; // root of state tree

	/**
	 * On average, one out of every 2**simRandomShift propagations through a component is delayed one step more than the
	 * component requests. This noise is intended to address some circuits that would otherwise oscillate within Logisim
	 * (though they wouldn't oscillate in practice).
	 */
	private volatile int simRandomShift;

	private PriorityQueue<SetData> toProcess = new PriorityQueue<>();
	private int clock;
	private boolean isOscillating;
	private boolean oscAdding;
	private PropagationPoints oscPoints = new PropagationPoints();
	private int ticks;
	private Random noiseSource = new Random();
	private int noiseCount;
	private int setDataSerialNumber;

	static int lastId;
	int id = lastId++;

	public Propagator(CircuitState root) {
		this.root = root;
		Listener l = new Listener(this);
		root.getProject().getOptions().getAttributeSet().addAttributeListener(l);
		updateRandomness();
	}

	private void updateRandomness() {
		Options opts = root.getProject().getOptions();
		int val = opts.getAttributeSet().getValue(Options.sim_rand_attr);
		int logVal = 0;
		while ((1 << logVal) < val)
			logVal++;
		simRandomShift = logVal;
	}

	public boolean isOscillating() {
		return isOscillating;
	}

	@Override
	public String toString() {
		return "Prop" + id;
	}

	public void drawOscillatingPoints(ComponentDrawContext context) {
		if (isOscillating)
			oscPoints.draw(context);
	}

	//
	// public methods
	//
	CircuitState getRootState() {
		return root;
	}

	void reset() {
		toProcess.clear();
		root.reset();
		isOscillating = false;
	}

	public void propagate() {
		oscPoints.clear();
		clearDirtyPoints();
		clearDirtyComponents();

		int oscThreshold = 1000;
		int logThreshold = 3 * oscThreshold / 4;
		int iters = 0;
		while (!toProcess.isEmpty()) {
			iters++;

			if (iters < logThreshold) stepInternal(null);
			else if (iters < oscThreshold) {
				oscAdding = true;
				stepInternal(oscPoints);
			} else {
				isOscillating = true;
				oscAdding = false;
				return;
			}
		}
		isOscillating = false;
		oscAdding = false;
		oscPoints.clear();
	}

	void step(PropagationPoints changedPoints) {
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

	private void stepInternal(PropagationPoints changedPoints) {
		if (toProcess.isEmpty())
			return;

		// update clock
		clock = toProcess.peek().time;

		// propagate all values for this clock tick
		HashMap<CircuitState, HashSet<ComponentPoint>> visited = new HashMap<>();
		while (true) {
			SetData data = toProcess.peek();
			if (data == null || data.time != clock)
				break;
			toProcess.remove();
			CircuitState state = data.state;

			// if it's already handled for this clock tick, continue
			HashSet<ComponentPoint> handled = visited.get(state);
			if (handled != null) {
				if (!handled.add(new ComponentPoint(data.cause, data.loc)))
					continue;
			} else {
				handled = new HashSet<>();
				visited.put(state, handled);
				handled.add(new ComponentPoint(data.cause, data.loc));
			}

			/*
			 * DEBUGGING - comment out Simulator.log(data.time + ": proc " + data.loc + " in " + data.state + " to " +
			 * data.val + " by " + data.cause); //
			 */

			if (changedPoints != null)
				changedPoints.add(state, data.loc);

			// change the information about value
			SetData oldHead = state.causes.get(data.loc);
			WireValue oldVal = computeValue(oldHead);
			SetData newHead = addCause(state, oldHead, data);
			WireValue newVal = computeValue(newHead);

			// if the value at point has changed, propagate it
			if (!newVal.equals(oldVal)) state.markPointAsDirty(data.loc);
		}

		clearDirtyPoints();
		clearDirtyComponents();
	}

	boolean isPending() {
		return !toProcess.isEmpty();
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

	void locationTouched(CircuitState state, Location loc) {
		if (oscAdding)
			oscPoints.add(state, loc);
	}

	//
	// package-protected helper methods
	//
	void setValue(CircuitState state, Location pt, WireValue val, Component cause, int delay) {
		if (cause instanceof Wire || cause instanceof Splitter)
			return;
		if (delay <= 0) delay = 1;
		int randomShift = simRandomShift;
		if (randomShift > 0) { // random noise is turned on
			// multiply the delay by 32 so that the random noise
			// only changes the delay by 3%.
			delay <<= randomShift;
			if (!(cause.getFactory() instanceof SubcircuitFactory)) if (noiseCount > 0) noiseCount--;
			else {
				delay++;
				noiseCount = noiseSource.nextInt(1 << randomShift);
			}
		}
		toProcess.add(new SetData(clock + delay, setDataSerialNumber, state, pt, cause, val));
		/*
		 * DEBUGGING - comment out Simulator.log(clock + ": set " + pt + " in " + state + " to " + val + " by " + cause
		 * + " after " + delay); //
		 */

		setDataSerialNumber++;
	}

	public boolean tick() {
		ticks++;
		return root.tick(ticks);
	}

	public int getTickCount() {
		return ticks;
	}

	//
	// private methods
	//
	void checkComponentEnds(CircuitState state, Component comp) {
		for (EndData end : comp.getEnds()) {
			Location loc = end.getLocation();
			SetData oldHead = state.causes.get(loc);
			WireValue oldVal = computeValue(oldHead);
			SetData newHead = removeCause(state, oldHead, loc, comp);
			WireValue newVal = computeValue(newHead);
			WireValue wireVal = state.getValueByWire(loc);

			if (!newVal.equals(oldVal) || wireVal != null) state.markPointAsDirty(loc);
			if (wireVal != null)
				state.setValueByWire(loc, logisim.data.WireValue.WireValues.NIL);
		}
	}

	private void clearDirtyPoints() {
		root.processDirtyPoints();
	}

	private void clearDirtyComponents() {
		root.processDirtyComponents();
	}

	private SetData addCause(CircuitState state, SetData head, SetData data) {
		// actually, it should be removed
		if (data.val == null) return removeCause(state, head, data.loc, data.cause);

		HashMap<Location, SetData> causes = state.causes;

		// first check whether this is change of previous info.
		boolean replaced = false;
		for (SetData n = head; n != null; n = n.next)
			if (n.cause == data.cause) {
				n.val = data.val;
				replaced = true;
				break;
			}

		// otherwise, insert to list of causes
		if (!replaced) if (head == null) {
			causes.put(data.loc, data);
			return data;
		}
		else {
			data.next = head.next;
			head.next = data;
		}

		return head;
	}

	private SetData removeCause(CircuitState state, SetData head, Location loc, Component cause) {
		HashMap<Location, SetData> causes = state.causes;
		if (head == null)
			return null;
		if (head.cause == cause) {
			head = head.next;
			if (head == null)
				causes.remove(loc);
			else
				causes.put(loc, head);
		} else {
			SetData prev = head;
			SetData cur = head.next;
			while (cur != null) {
				if (cur.cause == cause) {
					prev.next = cur.next;
					return head;
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
	static WireValue computeValue(SetData causes) {
		if (causes == null)
			return WireValues.NIL;
		WireValue ret = causes.val;
		for (SetData n = causes.next; n != null; n = n.next) ret = ret.combine(n.val);
		return ret;
	}

}
