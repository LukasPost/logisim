/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.util.*;

import logisim.circuit.CircuitWires.State;
import logisim.circuit.Propagator.SetData;
import logisim.comp.Component;
import logisim.comp.ComponentDrawContext;
import logisim.comp.ComponentState;
import logisim.data.BitWidth;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceData;
import logisim.instance.InstanceFactory;
import logisim.instance.InstanceState;
import logisim.proj.Project;
import logisim.std.wiring.Clock;
import logisim.std.wiring.Pin;
import logisim.util.ArraySet;

public class CircuitState implements InstanceData {
	private class MyCircuitListener implements CircuitListener {
		public void circuitChanged(CircuitEvent event) {
			int action = event.getAction();
			if (action == CircuitEvent.ACTION_ADD) {
				Component comp = (Component) event.getData();
				if (comp instanceof Wire w) {
                    markPointAsDirty(w.getEnd0());
					markPointAsDirty(w.getEnd1());
				} else markComponentAsDirty(comp);
			} else if (action == CircuitEvent.ACTION_REMOVE) {
				Component comp = (Component) event.getData();
				if (comp.getFactory() instanceof SubcircuitFactory) {
					// disconnect from tree
					CircuitState substate = (CircuitState) getData(comp);
					if (substate != null && substate.parentComp == comp) {
						substates.remove(substate);
						substate.parentState = null;
						substate.parentComp = null;
					}
				}

				if (comp instanceof Wire w) {
                    markPointAsDirty(w.getEnd0());
					markPointAsDirty(w.getEnd1());
				} else {
					if (base != null)
						base.checkComponentEnds(CircuitState.this, comp);
					dirtyComponents.remove(comp);
				}
			} else if (action == CircuitEvent.ACTION_CLEAR) {
				substates.clear();
				wireData = null;
				componentData.clear();
				values.clear();
				dirtyComponents.clear();
				dirtyPoints.clear();
				causes.clear();
			} else if (action == CircuitEvent.ACTION_CHANGE) {
				Object data = event.getData();
				if (data instanceof Collection) {
					@SuppressWarnings("unchecked")
					Collection<Component> comps = (Collection<Component>) data;
					markComponentsDirty(comps);
					if (base != null) for (Component comp : comps) base.checkComponentEnds(CircuitState.this, comp);
				} else {
					Component comp = (Component) event.getData();
					markComponentAsDirty(comp);
					if (base != null)
						base.checkComponentEnds(CircuitState.this, comp);
				}
			} else if (action == CircuitEvent.ACTION_INVALIDATE) {
				Component comp = (Component) event.getData();
				markComponentAsDirty(comp);
				// TODO determine if this should really be missing if (base != null)
				// base.checkComponentEnds(CircuitState.this, comp);
			} else if (action == CircuitEvent.TRANSACTION_DONE) {
				ReplacementMap map = event.getResult().getReplacementMap(circuit);
				if (map != null) for (Component comp : map.getReplacedComponents()) {
					Object compState = componentData.remove(comp);
					if (compState != null) {
						Class<?> compFactory = comp.getFactory().getClass();
						boolean found = false;
						for (Component repl : map.get(comp))
							if (repl.getFactory().getClass() == compFactory) {
								found = true;
								setData(repl, compState);
								break;
							}
						if (!found && compState instanceof CircuitState sub) {
							sub.parentState = null;
							substates.remove(sub);
						}
					}
				}
			}
		}
	}

	private Propagator base; // base of tree of CircuitStates
	private Project proj; // project where circuit lies
	private Circuit circuit; // circuit being simulated

	private CircuitState parentState; // parent in tree of CircuitStates
	private Component parentComp; // subcircuit component containing this state
	private ArraySet<CircuitState> substates = new ArraySet<>();

	private State wireData;
	private HashMap<Component, Object> componentData = new HashMap<>();
	private Map<Location, WireValue> values = new HashMap<>();
	private ArrayList<Component> dirtyComponents = new ArrayList<>();
	private ArrayList<Location> dirtyPoints = new ArrayList<>();
	HashMap<Location, SetData> causes = new HashMap<>();

	private static int lastId;
	private int id = lastId++;

	public CircuitState(Project proj, Circuit circuit) {
		this.proj = proj;
		this.circuit = circuit;
		MyCircuitListener myCircuitListener = new MyCircuitListener();
		circuit.addCircuitListener(myCircuitListener);
	}

	public Project getProject() {
		return proj;
	}

	Component getSubcircuit() {
		return parentComp;
	}

	@Override
	public CircuitState clone() {
		return cloneState();
	}

	public CircuitState cloneState() {
		CircuitState ret = new CircuitState(proj, circuit);
		ret.copyFrom(this, new Propagator(ret));
		ret.parentComp = null;
		ret.parentState = null;
		return ret;
	}

	private void copyFrom(CircuitState src, Propagator base) {
		this.base = base;
		parentComp = src.parentComp;
		parentState = src.parentState;
		HashMap<CircuitState, CircuitState> substateData = new HashMap<>();
		substates = new ArraySet<>();
		for (CircuitState oldSub : src.substates) {
			CircuitState newSub = new CircuitState(src.proj, oldSub.circuit);
			newSub.copyFrom(oldSub, base);
			newSub.parentState = this;
			substates.add(newSub);
			substateData.put(oldSub, newSub);
		}
		for (Component key : src.componentData.keySet()) {
			Object oldValue = src.componentData.get(key);
			if (oldValue instanceof CircuitState) {
				Object newValue = substateData.get(oldValue);
				if (newValue != null)
					componentData.put(key, newValue);
				else
					componentData.remove(key);
			} else {
				Object newValue;
				if (oldValue instanceof ComponentState) newValue = ((ComponentState) oldValue).clone();
				else newValue = oldValue;
				componentData.put(key, newValue);
			}
		}
		for (Location key : src.causes.keySet()) {
			SetData oldValue = src.causes.get(key);
			SetData newValue = oldValue.cloneFor(this);
			causes.put(key, newValue);
		}
		if (src.wireData != null) wireData = (State) src.wireData.clone();
		values.putAll(src.values);
		dirtyComponents.addAll(src.dirtyComponents);
		dirtyPoints.addAll(src.dirtyPoints);
	}

	@Override
	public String toString() {
		return "State" + id + "[" + circuit.getName() + "]";
	}

	//
	// public methods
	//
	public Circuit getCircuit() {
		return circuit;
	}

	public CircuitState getParentState() {
		return parentState;
	}

	public Set<CircuitState> getSubstates() { // returns Set of CircuitStates
		return substates;
	}

	public Propagator getPropagator() {
		if (base == null) {
			base = new Propagator(this);
			markAllComponentsDirty();
		}
		return base;
	}

	public void drawOscillatingPoints(ComponentDrawContext context) {
		if (base != null)
			base.drawOscillatingPoints(context);
	}

	public Object getData(Component comp) {
		return componentData.get(comp);
	}

	public void setData(Component comp, Object data) {
		if (data instanceof CircuitState newState) {
			CircuitState oldState = (CircuitState) componentData.get(comp);
            if (oldState != newState) {
				// There's something new going on with this subcircuit.
				// Maybe the subcircuit is new, or perhaps it's being
				// removed.
				if (oldState != null && oldState.parentComp == comp) {
					// it looks like it's being removed
					substates.remove(oldState);
					oldState.parentState = null;
					oldState.parentComp = null;
				}
				if (newState.parentState != this) {
					// this is the first time I've heard about this CircuitState
					substates.add(newState);
					newState.base = base;
					newState.parentState = this;
					newState.parentComp = comp;
					newState.markAllComponentsDirty();
				}
			}
		}
		componentData.put(comp, data);
	}

	public WireValue getValue(Location pt) {
		WireValue ret = values.get(pt);
		if (ret != null)
			return ret;

		BitWidth wid = circuit.getWidth(pt);
		return WireValue.Companion.createUnknown(wid);
	}

	public void setValue(Location pt, WireValue val, Component cause, int delay) {
		if (base != null)
			base.setValue(this, pt, val, cause, delay);
	}

	public void markComponentAsDirty(Component comp) {
		try {
			dirtyComponents.add(comp);
		}
		catch (RuntimeException e) {
			ArrayList<Component> set = new ArrayList<>();
			set.add(comp);
			dirtyComponents = set;
		}
	}

	public void markComponentsDirty(Collection<Component> comps) {
		dirtyComponents.addAll(comps);
	}

	public void markPointAsDirty(Location pt) {
		dirtyPoints.add(pt);
	}

	public InstanceState getInstanceState(Component comp) {
		Object factory = comp.getFactory();
		if (factory instanceof InstanceFactory) return ((InstanceFactory) factory).createInstanceState(this, comp);
		else throw new RuntimeException("getInstanceState requires instance component");
	}

	public InstanceState getInstanceState(Instance instance) {
		InstanceFactory factory = instance.getFactory();
		return factory.createInstanceState(this, instance);
	}

	//
	// methods for other classes within package
	//
	public boolean isSubstate() {
		return parentState != null;
	}

	void processDirtyComponents() {
		if (!dirtyComponents.isEmpty()) {
			// This seeming wasted copy is to avoid ConcurrentModifications
			// if we used an iterator instead.
			Object[] toProcess;
			RuntimeException firstException = null;
			for (int tries = 4; true; tries--)
				try {
					toProcess = dirtyComponents.toArray();
					break;
				} catch (RuntimeException e) {
					if (firstException == null)
						firstException = e;
					if (tries == 0) {
						dirtyComponents.clear();
						throw firstException;
					}
				}
			dirtyComponents.clear();
			for (Object compObj : toProcess)
				if (compObj instanceof Component comp) {
					comp.propagate(this);
					// should be propagated in superstate
					if (comp.getFactory() instanceof Pin && parentState != null) parentComp.propagate(parentState);
				}
		}

		CircuitState[] subs = new CircuitState[substates.size()];
		for (CircuitState substate : substates.toArray(subs)) substate.processDirtyComponents();
	}

	void processDirtyPoints() {
		ArrayList<Location> dirty = dirtyPoints;
		dirtyPoints = new ArrayList<>();
		if (circuit.wires.isMapVoided()) for (int i = 3; i >= 0; i--)
			try {
				dirty.addAll(circuit.wires.points.getSplitLocations());
				break;
			} catch (ConcurrentModificationException e) {
				// try again...
				try {
					Thread.sleep(1);
				} catch (InterruptedException e2) {
				}
				if (i == 0)
					e.printStackTrace();
			}
		if (!dirty.isEmpty()) circuit.wires.propagate(this, dirty);

		CircuitState[] subs = new CircuitState[substates.size()];
		subs = substates.toArray(subs);
		for (CircuitState substate : subs) substate.processDirtyPoints();
	}

	void reset() {
		wireData = null;
        componentData.keySet().removeIf(comp -> !(comp.getFactory() instanceof SubcircuitFactory));
		values.clear();
		dirtyComponents.clear();
		dirtyPoints.clear();
		causes.clear();
		markAllComponentsDirty();

		for (CircuitState sub : substates) sub.reset();
	}

	boolean tick(int ticks) {
		boolean ret = false;
		for (Component clock : circuit.getClocks()) ret |= Clock.tick(this, ticks, clock);

		CircuitState[] subs = new CircuitState[substates.size()];
		for (CircuitState substate : substates.toArray(subs)) ret |= substate.tick(ticks);
		return ret;
	}

	State getWireData() {
		return wireData;
	}

	void setWireData(State data) {
		wireData = data;
	}

	WireValue getComponentOutputAt(Location p) {
		// for CircuitWires - to get values, ignoring wires' contributions
		SetData cause_list = causes.get(p);
		return Propagator.computeValue(cause_list);
	}

	WireValue getValueByWire(Location p) {
		return values.get(p);
	}

	void setValueByWire(Location p, WireValue v) {
		// for CircuitWires - to set value at point
		boolean changed;
		if (v == WireValues.NIL) {
			Object old = values.remove(p);
			changed = (old != null && old != WireValues.NIL);
		} else {
			Object old = values.put(p, v);
			changed = !v.equals(old);
		}
		if (changed) {
			boolean found = false;
			for (Component comp : circuit.getComponents(p))
				if (!(comp instanceof Wire) && !(comp instanceof Splitter)) {
					found = true;
					markComponentAsDirty(comp);
				}
			// NOTE: this will cause a double-propagation on components
			// whose outputs have just changed.

			if (found && base != null)
				base.locationTouched(this, p);
		}
	}

	//
	// private methods
	//
	private void markAllComponentsDirty() {
		dirtyComponents.addAll(circuit.getNonWires());
	}
}
