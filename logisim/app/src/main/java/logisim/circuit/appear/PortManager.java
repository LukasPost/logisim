/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit.appear;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

import draw.model.CanvasObject;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.instance.Instance;
import logisim.instance.StdAttr;

class PortManager {
	private CircuitAppearance appearance;
	private boolean doingUpdate;

	PortManager(CircuitAppearance appearance) {
		this.appearance = appearance;
		doingUpdate = false;
	}

	void updatePorts() {
		appearance.recomputePorts();
	}

	void updatePorts(Set<Instance> adds, Set<Instance> removes, Map<Instance, Instance> replaces,
			Collection<Instance> allPins) {
		if (appearance.isDefaultAppearance()) appearance.recomputePorts();
		else // "doingUpdate" ensures infinite recursion doesn't happen
			if (!doingUpdate) try {
				doingUpdate = true;
				performUpdate(adds, removes, replaces, allPins);
				appearance.recomputePorts();
			} finally {
				doingUpdate = false;
			}
	}

	private void performUpdate(Set<Instance> adds, Set<Instance> removes, Map<Instance, Instance> replaces,
			Collection<Instance> allPins) {
		// Find the current objects corresponding to pins
		Map<Instance, AppearancePort> oldObjects = new HashMap<>();
		AppearanceAnchor anchor = null;
		for (CanvasObject o : appearance.getObjectsFromBottom())
			if (o instanceof AppearancePort port) oldObjects.put(port.getPin(), port);
			else if (o instanceof AppearanceAnchor) anchor = (AppearanceAnchor) o;

		// ensure we have the anchor in the circuit
		if (anchor == null) {
			for (CanvasObject o : DefaultAppearance.build(allPins))
				if (o instanceof AppearanceAnchor) anchor = (AppearanceAnchor) o;
			if (anchor == null) anchor = new AppearanceAnchor(new Location(100, 100));
			int dest = appearance.getObjectsFromBottom().size();
			appearance.addObjects(dest, Collections.singleton(anchor));
		}

		// Compute how the ports should change
		ArrayList<AppearancePort> portRemoves = new ArrayList<>(removes.size());
		ArrayList<AppearancePort> portAdds = new ArrayList<>(adds.size());

		// handle removals
		for (Instance pin : removes) {
			AppearancePort port = oldObjects.remove(pin);
			if (port != null) portRemoves.add(port);
		}
		// handle replacements
		ArrayList<Instance> addsCopy = new ArrayList<>(adds);
		for (Entry<Instance, Instance> entry : replaces.entrySet()) {
			AppearancePort port = oldObjects.remove(entry.getKey());
			// this really shouldn't happen, but just to make sure...
			if (port != null) {
				port.setPin(entry.getValue());
				oldObjects.put(entry.getValue(), port);
			} else addsCopy.add(entry.getValue());
		}
		// handle additions
		DefaultAppearance.sortPinList(addsCopy, Direction.East);
		// They're probably not really all facing east.
		// I'm just sorting them so it works predictably.
		for (Instance pin : addsCopy)
			if (!oldObjects.containsKey(pin)) {
				Location loc = computeDefaultLocation(appearance, pin, oldObjects);
				AppearancePort o = new AppearancePort(loc, pin);
				portAdds.add(o);
				oldObjects.put(pin, o);
			}

		// Now update the appearance
		appearance.replaceAutomatically(portRemoves, portAdds);
	}

	private static Location computeDefaultLocation(CircuitAppearance appear, Instance pin,
			Map<Instance, AppearancePort> others) {
		// Determine which locations are being used in canvas, and look for
		// which instances facing the same way in layout
		Set<Location> usedLocs = new HashSet<>();
		List<Instance> sameWay = new ArrayList<>();
		Direction facing = pin.getAttributeValue(StdAttr.FACING);
		for (Entry<Instance, AppearancePort> entry : others.entrySet()) {
			Instance pin2 = entry.getKey();
			Location loc = entry.getValue().getLocation();
			usedLocs.add(loc);
			if (pin2.getAttributeValue(StdAttr.FACING) == facing) sameWay.add(pin2);
		}

		// if at least one faces the same way, place pin relative to that
		if (sameWay.size() > 0) {
			sameWay.add(pin);
			DefaultAppearance.sortPinList(sameWay, facing);
			Instance neighbor = null; // (preferably previous in map)
			for (Instance p : sameWay)
				if (p == pin) break;
				else neighbor = p;
			// pin must have been first in list
			if (neighbor == null) neighbor = sameWay.get(1);
			int dx;
			int dy;
			if (facing == Direction.East || facing == Direction.West) {
				dx = 0;
				dy = 10;
			} else {
				dx = 10;
				dy = 0;
			}
			Location loc = others.get(neighbor).getLocation();
			do loc = loc.translate(dx, dy); while (usedLocs.contains(loc));
			if (loc.x() >= 0 && loc.y() >= 0) return loc;
			do loc = loc.translate(-dx, -dy); while (usedLocs.contains(loc));
			return loc;
		}

		// otherwise place it on the boundary of the bounding rectangle
		Bounds bds = appear.getAbsoluteBounds();
		int x;
		int y;
		int dx = 0;
		int dy = 0;
		if (facing == Direction.East) { // on west side by default
			x = bds.getX() - 7;
			y = bds.getY() + 5;
			dy = 10;
		} else if (facing == Direction.West) { // on east side by default
			x = bds.getX() + bds.getWidth() - 3;
			y = bds.getY() + 5;
			dy = 10;
		} else if (facing == Direction.South) { // on north side by default
			x = bds.getX() + 5;
			y = bds.getY() - 7;
			dx = 10;
		} else { // on south side by default
			x = bds.getX() + 5;
			y = bds.getY() + bds.getHeight() - 3;
			dx = 10;
		}
		x = (x + 9) / 10 * 10; // round coordinates up to ensure they're on grid
		y = (y + 9) / 10 * 10;
		Location loc = new Location(x, y);
		while (usedLocs.contains(loc)) loc = loc.translate(dx, dy);
		return loc;
	}
}
