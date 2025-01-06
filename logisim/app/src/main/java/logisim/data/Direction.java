/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

public enum Direction implements AttributeOptionInterface {

	East(0), North(1), West(2), South(3);

	private final int id;

	Direction(int id) {
		this.id = id;
	}

	/**
	 * @deprecated Please use valueOf instead
	 */
	@Deprecated
	public static Direction parse(String str) {
		String cap = str.substring(0, 1).toUpperCase() + str.substring(1);
		return Direction.valueOf(cap);
	}

	public static final Direction[] cardinals = { East, North, West, South };

	public static Direction[] getCardinals() {
		return cardinals;
	}

	public static Direction fromInt(int x) {
		return cardinals[x];
	}

	private static final String[] displayStrings = { Strings.get("directionEastOption"), Strings.get("directionNorthOption"),
			Strings.get("directionWestOption"), Strings.get("directionSouthOption") };

	public String toDisplayString() {
		return displayStrings[id];
	}

	private static final String[] displayStringsVertical = { Strings.get("directionEastVertical"),
			Strings.get("directionNorthVertical"), Strings.get("directionWestVertical"),
			Strings.get("directionSouthVertical") };

	public String toVerticalDisplayString() {
		return displayStringsVertical[id];
	}

	public double toRadians() {
		return id * Math.PI / 2.0;
	}

	public int toDegrees() {
		return id * 90;
	}

	public Direction reverse() {
		return fromInt((id + 2) & 3);
	}

	public Direction rotateCW() {
		return fromInt((id - 1) & 3);
	}

	public Direction rotateCCW() {
		return fromInt((id + 1) & 3);
	}

	// for AttributeOptionInterface
	public Object getValue() {
		return this;
	}
}
