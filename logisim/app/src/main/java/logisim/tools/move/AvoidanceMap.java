/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools.move;

import java.io.PrintStream;
import java.util.*;

import logisim.circuit.Wire;
import logisim.comp.Component;
import logisim.data.Bounds;
import logisim.data.Location;

class AvoidanceMap {
	static AvoidanceMap create(Collection<Component> elements, int dx, int dy) {
		AvoidanceMap ret = new AvoidanceMap(new HashMap<>());
		ret.markAll(elements, dx, dy);
		return ret;
	}

	private final HashMap<Location, String> avoid;

	private AvoidanceMap(HashMap<Location, String> map) {
		avoid = map;
	}

	public AvoidanceMap cloneMap() {
		return new AvoidanceMap(new HashMap<>(avoid));
	}

	public Object get(Location loc) {
		return avoid.get(loc);
	}

	public void markAll(Collection<Component> elements, int dx, int dy) {
		// first we go through the components, saying that we should not
		// intersect with any point that lies within a component
		for (Component el : elements)
			if (el instanceof Wire) markWire((Wire) el, dx, dy);
			else markComponent(el, dx, dy);
	}

	public void markComponent(Component comp, int dx, int dy) {
		HashMap<Location, String> avoid = this.avoid;
		boolean translated = dx != 0 || dy != 0;
		Bounds bds = comp.getBounds();
		int x0 = bds.getX() + dx;
		int y0 = bds.getY() + dy;
		int x1 = x0 + bds.getWidth();
		int y1 = y0 + bds.getHeight();
		x0 += 9 - (x0 + 9) % 10;
		y0 += 9 - (y0 + 9) % 10;
		for (int x = x0; x <= x1; x += 10)
			for (int y = y0; y <= y1; y += 10) {
				Location loc = new Location(x, y);
				// loc is most likely in the component, so go ahead and
				// put it into the map as if it is - and in the rare event
				// that loc isn't in the component, we can remove it.
				String prev = avoid.put(loc, Connector.ALLOW_NEITHER);
				if (!Objects.equals(prev, Connector.ALLOW_NEITHER)) {
					Location baseLoc = translated ? loc.add(-dx, -dy) : loc;
					if (!comp.contains(baseLoc)) if (prev == null) avoid.remove(loc);
					else avoid.put(loc, prev);
				}
			}
	}

	public void markWire(Wire w, int dx, int dy) {
		HashMap<Location, String> avoid = this.avoid;
		boolean translated = dx != 0 || dy != 0;
		Location loc0 = w.getEnd0();
		Location loc1 = w.getEnd1();
		if (translated) {
			loc0 = loc0.add(dx, dy);
			loc1 = loc1.add(dx, dy);
		}
		avoid.put(loc0, Connector.ALLOW_NEITHER);
		avoid.put(loc1, Connector.ALLOW_NEITHER);
		int x0 = loc0.x();
		int y0 = loc0.y();
		int x1 = loc1.x();
		int y1 = loc1.y();
		// vertical wire
		if (x0 == x1) for (Location loc : Wire.create(loc0, loc1)) {
			Object prev = avoid.put(loc, Connector.ALLOW_HORIZONTAL);
			if (prev == Connector.ALLOW_NEITHER || prev == Connector.ALLOW_VERTICAL)
				avoid.put(loc, Connector.ALLOW_NEITHER);
		}
		else // diagonal - shouldn't happen
			// horizontal wire
			if (y0 == y1) for (Location loc : Wire.create(loc0, loc1)) {
				Object prev = avoid.put(loc, Connector.ALLOW_VERTICAL);
				if (prev == Connector.ALLOW_NEITHER || prev == Connector.ALLOW_HORIZONTAL)
					avoid.put(loc, Connector.ALLOW_NEITHER);
			}
			else throw new RuntimeException("diagonal wires not supported");
	}

	public void unmarkLocation(Location loc) {
		avoid.remove(loc);
	}

	public void unmarkWire(Wire w, Location deletedEnd, Set<Location> unmarkable) {
		Location loc0 = w.getEnd0();
		Location loc1 = w.getEnd1();
		if (unmarkable == null || unmarkable.contains(deletedEnd)) avoid.remove(deletedEnd);
		int x0 = loc0.x();
		int y0 = loc0.y();
		int x1 = loc1.x();
		int y1 = loc1.y();
		// vertical wire
		if (x0 == x1) for (Location loc : w) {
			if (unmarkable == null || unmarkable.contains(deletedEnd)) {
				Object prev = avoid.remove(loc);
				if (prev != Connector.ALLOW_HORIZONTAL && prev != null) avoid.put(loc, Connector.ALLOW_VERTICAL);
			}
		}
		else // diagonal - shouldn't happen
			// horizontal wire
			if (y0 == y1) for (Location loc : w)
				if (unmarkable == null || unmarkable.contains(deletedEnd)) {
					Object prev = avoid.remove(loc);
					if (prev != Connector.ALLOW_VERTICAL && prev != null) avoid.put(loc, Connector.ALLOW_HORIZONTAL);
				}
			else throw new RuntimeException("diagonal wires not supported");
	}

	public void print(PrintStream stream) {
		ArrayList<Location> list = new ArrayList<>(avoid.keySet());
		Collections.sort(list);
		for (Location location : list) stream.println(location + ": " + avoid.get(location));
	}
}
