/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.IdentityHashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Set;
import java.util.TreeSet;

import logisim.comp.Component;
import logisim.data.Location;

class WireRepair extends CircuitTransaction {

	private static class MergeSets {
		private final HashMap<Wire, ArrayList<Wire>> map;

		MergeSets() {
			map = new HashMap<>();
		}

		void merge(Wire a, Wire b) {
			ArrayList<Wire> set0 = map.get(a);
			ArrayList<Wire> set1 = map.get(b);
			if (set0 == set1)
				return;

			if (set0 != null && set1 != null) {
				if (set0.size() > set1.size()) { // ensure set1 is the larger
					ArrayList<Wire> temp = set0;
					set0 = set1;
					set1 = temp;
				}
				set1.addAll(set0);
				for (Wire w : set0) 
					map.put(w, set1);

			} else if (set0 == null) {
				set1.add(a);
				map.put(a, set1);
			} else {
				set0.add(b);
				map.put(b, set0);
			}
		}

		Collection<ArrayList<Wire>> getMergeSets() {
			IdentityHashMap<ArrayList<Wire>, Boolean> lists = new IdentityHashMap<>();
			for (ArrayList<Wire> list : map.values()) 
				lists.put(list, Boolean.TRUE);
			
			return lists.keySet();
		}
	}

	private Circuit circuit;

	public WireRepair(Circuit circuit) {
		this.circuit = circuit;
	}

	@Override
	protected Map<Circuit, Integer> getAccessedCircuits() {
		return Collections.singletonMap(circuit, READ_WRITE);
	}

	@Override
	protected void run(CircuitMutator mutator) {
		doMerges(mutator);
		doOverlaps(mutator);
		doSplits(mutator);
	}

	/*
	 * for debugging: private void printWires(String prefix, PrintStream out) {
	 * boolean first = true; for (Wire w :
	 * circuit.getWires()) { if (first) { out.println(prefix + ": " + w); first =
	 * false; } else { out.println("      " +
	 * w); } } out.println(prefix + ": none"); }
	 */

	private void doMerges(CircuitMutator mutator) {
		MergeSets sets = new MergeSets();
		for (Location loc : circuit.wires.points.getSplitLocations()) {
			Collection<?> at = circuit.getComponents(loc);
			if (at.size() == 2) {
				Iterator<?> atit = at.iterator();
				Object at0 = atit.next();
				Object at1 = atit.next();
				if (at0 instanceof Wire w0 && at1 instanceof Wire w1) if (w0.isParallel(w1)) sets.merge(w0, w1);
			}
		}

		ReplacementMap repl = new ReplacementMap();
		for (ArrayList<Wire> mergeSet : sets.getMergeSets())
			if (mergeSet.size() > 1) {
				ArrayList<Location> locs = new ArrayList<>(2 * mergeSet.size());
				for (Wire w : mergeSet) {
					locs.add(w.getEnd0());
					locs.add(w.getEnd1());
				}
				Collections.sort(locs);
				Location e0 = locs.getFirst();
				Location e1 = locs.getLast();
				Wire wnew = Wire.create(e0, e1);
				Collection<Wire> wset = Collections.singleton(wnew);

				for (Wire w : mergeSet)
					repl.put(w, wset);
			}
		mutator.replace(circuit, repl);
	}

	private void doOverlaps(CircuitMutator mutator) {
		HashMap<Location, ArrayList<Wire>> wirePoints = new HashMap<>();
		for (Wire w : circuit.getWires())
			for (Location loc : w) {
				ArrayList<Wire> locWires = wirePoints.computeIfAbsent(loc, k -> new ArrayList<>(3));
				locWires.add(w);
			}

		MergeSets mergeSets = new MergeSets();
		for (ArrayList<Wire> locWires : wirePoints.values())
			if (locWires.size() > 1) for (int i = 0, n = locWires.size(); i < n; i++) {
				Wire w0 = locWires.get(i);
				for (int j = i + 1; j < n; j++) {
					Wire w1 = locWires.get(j);
					if (w0.overlaps(w1, false)) mergeSets.merge(w0, w1);
				}
			}

		ReplacementMap replacements = new ReplacementMap();
		Set<Location> splitLocs = circuit.wires.points.getSplitLocations();
		for (ArrayList<Wire> mergeSet : mergeSets.getMergeSets())
			if (mergeSet.size() > 1) doMergeSet(mergeSet, replacements, splitLocs);
		mutator.replace(circuit, replacements);
	}

	private void doMergeSet(ArrayList<Wire> mergeSet, ReplacementMap replacements, Set<Location> splitLocs) {
		TreeSet<Location> ends = new TreeSet<>();
		for (Wire w : mergeSet) {
			ends.add(w.getEnd0());
			ends.add(w.getEnd1());
		}
		Wire whole = Wire.create(ends.first(), ends.last());

		TreeSet<Location> mids = new TreeSet<>();
		mids.add(whole.getEnd0());
		mids.add(whole.getEnd1());
		for (Location loc : whole)
			if (splitLocs.contains(loc)) for (Component comp : circuit.getComponents(loc))
				if (comp instanceof Wire w && !mergeSet.contains(w)) {
					mids.add(loc);
					break;
				}

		ArrayList<Wire> mergeResult = new ArrayList<>();
		if (mids.size() == 2) mergeResult.add(whole);
		else {
			Location e0 = mids.first();
			for (Location e1 : mids) {
				mergeResult.add(Wire.create(e0, e1));
				e0 = e1;
			}
		}

		for (Wire w : mergeSet) {
			ArrayList<Component> wRepl = new ArrayList<>(2);
			for (Wire w2 : mergeResult) if (w2.overlaps(w, false)) wRepl.add(w2);
			replacements.put(w, wRepl);
		}
	}

	private void doSplits(CircuitMutator mutator) {
		Set<Location> splitLocs = circuit.wires.points.getSplitLocations();
		ReplacementMap repl = new ReplacementMap();
		for (Wire w : circuit.getWires()) {
			Location w0 = w.getEnd0();
			Location w1 = w.getEnd1();
			ArrayList<Location> splits = null;
			for (Location loc : splitLocs)
				if (w.contains(loc) && !loc.equals(w0) && !loc.equals(w1)) {
					if (splits == null)
						splits = new ArrayList<>();
					splits.add(loc);
				}
			if (splits != null) {
				splits.add(w1);
				Collections.sort(splits);
				Location e0 = w0;
				ArrayList<Wire> subs = new ArrayList<>(splits.size());
				for (Location e1 : splits) {
					subs.add(Wire.create(e0, e1));
					e0 = e1;
				}
				repl.put(w, subs);
			}
		}
		mutator.replace(circuit, repl);
	}
}
