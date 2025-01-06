/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools.move;

class MoveRequest {
	private MoveGesture gesture;
	private int dx;
	private int dy;

	public MoveRequest(MoveGesture gesture, int dx, int dy) {
		this.gesture = gesture;
		this.dx = dx;
		this.dy = dy;
	}

	public MoveGesture getMoveGesture() {
		return gesture;
	}

	public int getDeltaX() {
		return dx;
	}

	public int getDeltaY() {
		return dy;
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof MoveRequest o && gesture == o.gesture && dx == o.dx && dy == o.dy;
	}

	@Override
	public int hashCode() {
		return (gesture.hashCode() * 31 + dx) * 31 + dy;
	}
}
