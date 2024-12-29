/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import logisim.circuit.Wire;
import logisim.data.Location;

public class WireRepairData {
	private Wire wire;
	private Location point;

	public WireRepairData(Wire wire, Location point) {
		this.wire = wire;
		this.point = point;
	}

	public Location getPoint() {
		return point;
	}

	public Wire getWire() {
		return wire;
	}
}
