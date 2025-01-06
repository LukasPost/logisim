/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import logisim.data.Location;

public class Handle {
	private CanvasObject object;
	private int x;
	private int y;

	public Handle(CanvasObject object, int x, int y) {
		this.object = object;
		this.x = x;
		this.y = y;
	}

	public Handle(CanvasObject object, Location loc) {
		this(object, loc.x(), loc.y());
	}

	public CanvasObject getObject() {
		return object;
	}

	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}

	public Location getLocation() {
		return new Location(x, y);
	}

	public boolean isAt(Location loc) {
		return x == loc.x() && y == loc.y();
	}

	public boolean isAt(int xq, int yq) {
		return x == xq && y == yq;
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof Handle that && object.equals(that.object) && x == that.x && y == that.y;
	}

	@Override
	public int hashCode() {
		return (object.hashCode() * 31 + x) * 31 + y;
	}
}
