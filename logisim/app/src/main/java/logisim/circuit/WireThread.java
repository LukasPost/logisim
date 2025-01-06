/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import logisim.circuit.CircuitWires.ThreadBundle;
import logisim.util.SmallSet;

class WireThread {
	private WireThread parent;
	private SmallSet<ThreadBundle> bundles = new SmallSet<>();

	WireThread() {
		parent = this;
	}

	SmallSet<ThreadBundle> getBundles() {
		return bundles;
	}

	void unite(WireThread other) {
		WireThread group = find();
		WireThread group2 = other.find();
		if (group != group2)
			group.parent = group2;
	}

	WireThread find() {
		WireThread ret = this;
		if (ret.parent != ret) {
			do
				ret = ret.parent;
			while (ret.parent != ret);
			parent = ret;
		}
		return ret;
	}
}
