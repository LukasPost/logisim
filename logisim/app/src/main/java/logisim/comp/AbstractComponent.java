/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.comp;

import java.awt.Graphics;
import java.util.List;

import logisim.circuit.CircuitState;
import logisim.data.Bounds;
import logisim.data.Location;

public abstract class AbstractComponent implements Component {
	protected AbstractComponent() {
	}

	//
	// basic information methods
	//
	public abstract ComponentFactory getFactory();

	//
	// location/extent methods
	//
	public abstract Location getLocation();

	public abstract Bounds getBounds();

	public Bounds getBounds(Graphics g) {
		return getBounds();
	}

	public boolean contains(Location pt) {
		Bounds bds = getBounds();
		return bds != null && bds.contains(pt, 1);
	}

	public boolean contains(Location pt, Graphics g) {
		Bounds bds = getBounds(g);
		return bds != null && bds.contains(pt, 1);
	}

	//
	// propagation methods
	//
	public abstract List<EndData> getEnds();

	public EndData getEnd(int index) {
		return getEnds().get(index);
	}

	public boolean endsAt(Location pt) {
		for (EndData data : getEnds()) if (data.getLocation().equals(pt)) return true;
		return false;
	}

	public abstract void propagate(CircuitState state);
}
