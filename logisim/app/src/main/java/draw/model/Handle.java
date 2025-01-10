/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import logisim.data.Location;

public class Handle {
	private CanvasObject object;
	public Location location;

	public Handle(CanvasObject object, int x, int y) {
		this.object = object;
		location = new Location(x, y);
	}
	public Handle(CanvasObject object, Location loc) {
		this.object = object;
		location = loc;
	}

	public CanvasObject getObject() {
		return object;
	}

	public int getX() { return location.x(); }

	public int getY() {
		return location.y();
	}

	public Location getLocation() {
		return location;
	}

	public boolean isAt(Location loc) {
		return location.equals(loc);
	}

	public boolean isAt(int xq, int yq) {
		return new Location(xq, yq).equals(location);
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof Handle that && object.equals(that.object) && location.equals(that.location);
	}

	@Override
	public int hashCode() {
		return (object.hashCode() * 31 + location.x()) * 31 + location.y();
	}
}
