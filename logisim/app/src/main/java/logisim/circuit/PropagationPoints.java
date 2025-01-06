/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.awt.Graphics;
import java.util.HashMap;
import java.util.HashSet;

import logisim.comp.Component;
import logisim.comp.ComponentDrawContext;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.GraphicsUtil;

class PropagationPoints {
	private static class Entry {
		private CircuitState state;
		private Location loc;

		private Entry(CircuitState state, Location loc) {
			this.state = state;
			this.loc = loc;
		}

		@Override
		public boolean equals(Object other) {
			return other instanceof Entry o && state.equals(o.state) && loc.equals(o.loc);
		}

		@Override
		public int hashCode() {
			return state.hashCode() * 31 + loc.hashCode();
		}
	}

	private HashSet<Entry> data;

	PropagationPoints() {
		data = new HashSet<>();
	}

	void add(CircuitState state, Location loc) {
		data.add(new Entry(state, loc));
	}

	void clear() {
		data.clear();
	}

	boolean isEmpty() {
		return data.isEmpty();
	}

	void draw(ComponentDrawContext context) {
		if (data.isEmpty())
			return;

		CircuitState state = context.getCircuitState();
		HashMap<CircuitState, CircuitState> stateMap = new HashMap<>();
		for (CircuitState s : state.getSubstates()) addSubstates(stateMap, s, s);

		Graphics g = context.getGraphics();
		GraphicsUtil.switchToWidth(g, 2);
		for (Entry e : data)
			if (e.state == state) {
				Location p = e.loc;
				g.drawOval(p.x() - 4, p.y() - 4, 8, 8);
			}
			else if (stateMap.containsKey(e.state)) {
				CircuitState substate = stateMap.get(e.state);
				Component subcirc = substate.getSubcircuit();
				Bounds b = subcirc.getBounds();
				g.drawRect(b.getX(), b.getY(), b.getWidth(), b.getHeight());
			}
		GraphicsUtil.switchToWidth(g, 1);
	}

	private void addSubstates(HashMap<CircuitState, CircuitState> map, CircuitState source, CircuitState value) {
		map.put(source, value);
		for (CircuitState s : source.getSubstates()) addSubstates(map, s, value);
	}
}
