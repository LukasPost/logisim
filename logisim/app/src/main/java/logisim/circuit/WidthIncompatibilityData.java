/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.util.ArrayList;

import logisim.data.BitWidth;
import logisim.data.Location;

public class WidthIncompatibilityData {
	private ArrayList<Location> points;
	private ArrayList<BitWidth> widths;

	public WidthIncompatibilityData() {
		points = new ArrayList<>();
		widths = new ArrayList<>();
	}

	public void add(Location p, BitWidth w) {
		for (int i = 0; i < points.size(); i++)
			if (p.equals(points.get(i)) && w.equals(widths.get(i)))
				return;
		points.add(p);
		widths.add(w);
	}

	public int size() {
		return points.size();
	}

	public Location getPoint(int i) {
		return points.get(i);
	}

	public BitWidth getBitWidth(int i) {
		return widths.get(i);
	}

	@Override
	public boolean equals(Object other) {
		if (!(other instanceof WidthIncompatibilityData o))
			return false;
		if (this == other)
			return true;

		if (size() != o.size())
			return false;
		for (int i = 0; i < size(); i++) {
			Location p = getPoint(i);
			BitWidth w = getBitWidth(i);
			boolean matched = false;
			for (int j = 0; j < o.size(); j++) {
				Location q = getPoint(j);
				BitWidth x = getBitWidth(j);
				if (p.equals(q) && w.equals(x)) {
					matched = true;
					break;
				}
			}
			if (!matched)
				return false;
		}
		return true;
	}
}
