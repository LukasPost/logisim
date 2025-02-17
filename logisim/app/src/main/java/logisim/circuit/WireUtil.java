/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.util.Collection;
import java.util.HashSet;
import java.util.Iterator;

import logisim.comp.Component;
import logisim.data.Location;

public class WireUtil {
	private WireUtil() {
	}

	static CircuitPoints computeCircuitPoints(Collection<? extends Component> components) {
		CircuitPoints points = new CircuitPoints();
		for (Component comp : components) points.add(comp);
		return points;
	}

	// Merge all parallel endpoint-to-endpoint wires within the given set.
	public static Collection<? extends Component> mergeExclusive(Collection<? extends Component> toMerge) {
		if (toMerge.size() <= 1)
			return toMerge;

		HashSet<Component> ret = new HashSet<>(toMerge);
		CircuitPoints points = computeCircuitPoints(toMerge);

		HashSet<Wire> wires = new HashSet<>();
		for (Location loc : points.getSplitLocations()) {
			Collection<? extends Component> at = points.getComponents(loc);
			if (at.size() == 2) {
				Iterator<? extends Component> atIt = at.iterator();
				Component o0 = atIt.next();
				Component o1 = atIt.next();
				if (o0 instanceof Wire w0 && o1 instanceof Wire w1) if (w0.is_x_equal == w1.is_x_equal) {
					wires.add(w0);
					wires.add(w1);
				}
			}
		}

		ret.removeAll(wires);
		while (!wires.isEmpty()) {
			Iterator<Wire> it = wires.iterator();
			Wire w = it.next();
			Location e0 = w.e0;
			Location e1 = w.e1;
			it.remove();
			boolean found;
			do {
				found = false;
				for (it = wires.iterator(); it.hasNext();) {
					Wire cand = it.next();
					if (cand.e0.equals(e1)) {
						e1 = cand.e1;
						found = true;
						it.remove();
					} else if (cand.e1.equals(e0)) {
						e0 = cand.e0;
						found = true;
						it.remove();
					}
				}
			} while (found);
			ret.add(Wire.create(e0, e1));
		}

		return ret;
	}
}
