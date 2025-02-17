/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools.move;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import logisim.circuit.Circuit;
import logisim.circuit.Wire;
import logisim.comp.Component;
import logisim.comp.EndData;
import logisim.data.Direction;
import logisim.data.Location;

public class MoveGesture {
	private MoveRequestListener listener;
	private Circuit circuit;
	private HashSet<Component> selected;

	private transient Set<ConnectionData> connections;
	private transient AvoidanceMap initAvoid;
	private final HashMap<MoveRequest, MoveResult> cachedResults;

	public MoveGesture(MoveRequestListener listener, Circuit circuit, Collection<Component> selected) {
		this.listener = listener;
		this.circuit = circuit;
		this.selected = new HashSet<>(selected);
		connections = null;
		initAvoid = null;
		cachedResults = new HashMap<>();
	}

	HashSet<Component> getSelected() {
		return selected;
	}

	AvoidanceMap getFixedAvoidanceMap() {
		AvoidanceMap ret = initAvoid;
		if (ret == null) {
			HashSet<Component> comps = new HashSet<>(circuit.getNonWires());
			comps.addAll(circuit.getWires());
			comps.removeAll(selected);
			ret = AvoidanceMap.create(comps, 0, 0);
			initAvoid = ret;
		}
		return ret;
	}

	Set<ConnectionData> getConnections() {
		Set<ConnectionData> ret = connections;
		if (ret == null) {
			ret = computeConnections(circuit, selected);
			connections = ret;
		}
		return ret;
	}

	public MoveResult findResult(int dx, int dy) {
		MoveRequest request = new MoveRequest(this, dx, dy);
		synchronized (cachedResults) {
			return cachedResults.get(request);
		}
	}

	public boolean enqueueRequest(int dx, int dy) {
		MoveRequest request = new MoveRequest(this, dx, dy);
		synchronized (cachedResults) {
			Object result = cachedResults.get(request);
			if (result == null) {
				ConnectorThread.enqueueRequest(request, false);
				return true;
			} else return false;
		}
	}

	public MoveResult forceRequest(int dx, int dy) {
		MoveRequest request = new MoveRequest(this, dx, dy);
		ConnectorThread.enqueueRequest(request, true);
		synchronized (cachedResults) {
			MoveResult result = cachedResults.get(request);
			while (result == null) {
				try {
					cachedResults.wait();
				}
				catch (InterruptedException e) {
					Thread.currentThread().interrupt();
					return null;
				}
				result = cachedResults.get(request);
			}
			return result;
		}
	}

	void notifyResult(MoveRequest request, MoveResult result) {
		synchronized (cachedResults) {
			cachedResults.put(request, result);
			cachedResults.notifyAll();
		}
		if (listener != null) listener.requestSatisfied(this, request.getDeltaX(), request.getDeltaY());
	}

	private static Set<ConnectionData> computeConnections(Circuit circuit, Set<Component> selected) {
		if (selected == null || selected.isEmpty())
			return Collections.emptySet();

		// first identify locations that might be connected
		Set<Location> locs = new HashSet<>();
		for (Component comp : selected) for (EndData end : comp.getEnds()) locs.add(end.getLocation());

		// now see which of them require connection
		Set<ConnectionData> conns = new HashSet<>();
		for (Location loc : locs) {
			boolean found = false;
			for (Component comp : circuit.getComponents(loc))
				if (!selected.contains(comp)) {
					found = true;
					break;
				}
			if (found) {
				List<Wire> wirePath;
				Location wirePathStart;
				Wire lastOnPath = findWire(circuit, loc, selected, null);
				if (lastOnPath == null) {
					wirePath = Collections.emptyList();
					wirePathStart = loc;
				} else {
					wirePath = new ArrayList<>();
					Location cur = loc;
					for (Wire w = lastOnPath; w != null; w = findWire(circuit, cur, selected, w)) {
						wirePath.add(w);
						cur = w.getOtherEnd(cur);
					}
					Collections.reverse(wirePath);
					wirePathStart = cur;
				}

				Direction dir = null;
				if (lastOnPath != null) {
					Location other = lastOnPath.getOtherEnd(loc);
					int dx = loc.x() - other.x();
					int dy = loc.y() - other.y();
					if (Math.abs(dx) > Math.abs(dy)) dir = dx > 0 ? Direction.East : Direction.West;
					else dir = dy > 0 ? Direction.South : Direction.North;
				}
				conns.add(new ConnectionData(loc, dir, wirePath, wirePathStart));
			}
		}
		return conns;
	}

	private static Wire findWire(Circuit circ, Location loc, Set<Component> ignore, Wire ignoreW) {
		Wire ret = null;
		for (Component comp : circ.getComponents(loc))
			if (!ignore.contains(comp) && comp != ignoreW) if (ret == null && comp instanceof Wire) ret = (Wire) comp;
			else return null;
		return ret;
	}

}
