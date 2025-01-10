/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

/**
 * Represents an immutable rectangular bounding box. This is analogous to
 * java.awt's <code>Point</code> class, except
 * that objects of this type are immutable.
 */
public record Location(int x, int y) implements Comparable<Location> {


	public int manhattanDistanceTo(Location o) {
		return Math.abs(o.x - x) + Math.abs(o.y - y);
	}

	public int manhattanDistanceTo(int x, int y) {
		return Math.abs(x - this.x) + Math.abs(y - this.y);
	}

	public Location translate(Direction dir, int dist) {
		return translate(dir, dist, 0);
	}

	public Location translate(Direction dir, int dist, int right) {
		if (dist == 0 && right == 0)
			return this;
		return switch (dir) {
			case East -> new Location(x + dist, y + right);
			case West -> new Location(x - dist, y - right);
			case South -> new Location(x - right, y + dist);
			case North -> new Location(x + right, y - dist);
			case null, default -> new Location(x + dist, y + right);
		};
	}

	// rotates this around (xc,yc) assuming that this is facing in the
	// from direction and the returned bounds should face in the to direction.
	public Location rotate(Direction from, Direction to, int xc, int yc) {
		int degrees = to.toDegrees() - from.toDegrees();
		while (degrees >= 360)
			degrees -= 360;
		while (degrees < 0)
			degrees += 360;

		int dx = x - xc;
		int dy = y - yc;
		if (degrees == 90) return new Location(xc + dy, yc - dx);
		else if (degrees == 180) return new Location(xc - dx, yc - dy);
		else if (degrees == 270) return new Location(xc - dy, yc + dx);
		else return this;
	}

	@Override
	public boolean equals(Object other_obj) {
		return other_obj instanceof Location(int x1, int y1) && x == x1 && y == y1;
	}

	public int compareTo(Location other) {
		if (x != other.x)
			return x - other.x;
		else
			return y - other.y;
	}

	@Override
	public String toString() {
		return "(" + x + "," + y + ")";
	}

	public static Location parse(String value) {
		String base = value;

		value = value.trim();
		if (value.charAt(0) == '(') {
			int len = value.length();
			if (value.charAt(len - 1) != ')') throw new NumberFormatException("invalid point '" + base + "'");
			value = value.substring(1, len - 1);
		}
		value = value.trim();
		int comma = value.indexOf(',');
		if (comma < 0) {
			comma = value.indexOf(' ');
			if (comma < 0) throw new NumberFormatException("invalid point '" + base + "'");
		}
		int x = Integer.parseInt(value.substring(0, comma).trim());
		int y = Integer.parseInt(value.substring(comma + 1).trim());
		return new Location(x, y);
	}

	public Location add(Location loc) {
		return new Location(x + loc.x, y + loc.y);
	}
	public Location add(int x, int y) {
		return new Location(this.x + x, this.y + y);
	}

	public Location sub(Location loc) {
		return new Location(x - loc.x, y - loc.y);
	}
	public Location sub(int x, int y) {
		return new Location(this.x + x, this.y + y);
	}
	public Location mul(double zoom) {
		return new Location((int) Math.round(zoom * x), (int) Math.round(zoom * y));
	}
	public Location abs() {
		return new Location(Math.abs(x), Math.abs(y));
	}
	public Location negate() {
		return new Location(-x, -y);
	}


	public int sum() {
		return x+y;
	}
	public boolean isZero(){
		return x == 0 && y == 0;
	}

	public static double[] toArray(Location loc) {
		return new double[] { loc.x(), loc.y() };
	}


}
