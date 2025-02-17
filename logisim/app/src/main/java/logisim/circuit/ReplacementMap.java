/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.io.PrintStream;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map.Entry;

import logisim.comp.Component;

public class ReplacementMap {
	private boolean frozen;
	private HashMap<Component, HashSet<Component>> map;
	private HashMap<Component, HashSet<Component>> inverse;

	public ReplacementMap(Component oldComp, Component newComp) {
		this(new HashMap<>(), new HashMap<>());
		HashSet<Component> oldSet = new HashSet<>(3);
		oldSet.add(oldComp);
		HashSet<Component> newSet = new HashSet<>(3);
		newSet.add(newComp);
		map.put(oldComp, newSet);
		inverse.put(newComp, oldSet);
	}

	public ReplacementMap() {
		this(new HashMap<>(), new HashMap<>());
	}

	private ReplacementMap(HashMap<Component, HashSet<Component>> map, HashMap<Component, HashSet<Component>> inverse) {
		this.map = map;
		this.inverse = inverse;
	}

	public void reset() {
		map.clear();
		inverse.clear();
	}

	public boolean isEmpty() {
		return map.isEmpty() && inverse.isEmpty();
	}

	public Collection<Component> getReplacedComponents() {
		return map.keySet();
	}

	public Collection<Component> get(Component prev) {
		return map.get(prev);
	}

	void freeze() {
		frozen = true;
	}

	public void add(Component comp) {
		if (frozen) throw new IllegalStateException("cannot change map after frozen");
		inverse.put(comp, new HashSet<>(3));
	}

	public void remove(Component comp) {
		if (frozen) throw new IllegalStateException("cannot change map after frozen");
		map.put(comp, new HashSet<>(3));
	}

	public void replace(Component prev, Component next) {
		put(prev, Collections.singleton(next));
	}

	public void put(Component prev, Collection<? extends Component> next) {
		if (frozen) throw new IllegalStateException("cannot change map after frozen");

		HashSet<Component> repl = map.computeIfAbsent(prev, k -> new HashSet<>(next.size()));
		repl.addAll(next);

		for (Component n : next) {
			repl = inverse.computeIfAbsent(n, k -> new HashSet<>(3));
			repl.add(prev);
		}
	}

	void append(ReplacementMap next) {
		for (Entry<Component, HashSet<Component>> e : next.map.entrySet()) {
			Component b = e.getKey();
			HashSet<Component> cs = e.getValue(); // what b is replaced by
			HashSet<Component> as = inverse.remove(b); // what was replaced to get b
			if (as == null) { // b pre-existed replacements so
				as = new HashSet<>(3); // we say it replaces itself.
				as.add(b);
			}

			for (Component a : as) {
				HashSet<Component> aDst = map.computeIfAbsent(a, k -> new HashSet<>(cs.size()));
				// should happen when b pre-existed only
				aDst.remove(b);
				aDst.addAll(cs);
			}

			for (Component c : cs) {
				HashSet<Component> cSrc = inverse.get(c); // should always be null
				if (cSrc == null) {
					cSrc = new HashSet<>(as.size());
					inverse.put(c, cSrc);
				}
				cSrc.addAll(as);
			}
		}

		for (Entry<Component, HashSet<Component>> e : next.inverse.entrySet()) {
			Component c = e.getKey();
			if (!inverse.containsKey(c)) {
				HashSet<Component> bs = e.getValue();
				if (!bs.isEmpty()) System.err.println("internal error: component replaced but not represented"); // OK
				inverse.put(c, new HashSet<>(3));
			}
		}
	}

	ReplacementMap getInverseMap() {
		return new ReplacementMap(inverse, map);
	}

	public Collection<Component> getComponentsReplacing(Component comp) {
		return map.get(comp);
	}

	public Collection<? extends Component> getRemovals() {
		return map.keySet();
	}

	public Collection<? extends Component> getAdditions() {
		return inverse.keySet();
	}

	public void print(PrintStream out) {
		boolean found = false;
		for (Component c : getRemovals()) {
			if (!found)
				out.println("  removals:");
			found = true;
			out.println("    " + c.toString());
		}
		if (!found)
			out.println("  removals: none");

		found = false;
		for (Component c : getAdditions()) {
			if (!found)
				out.println("  additions:");
			found = true;
			out.println("    " + c.toString());
		}
		if (!found)
			out.println("  additions: none");
	}
}
