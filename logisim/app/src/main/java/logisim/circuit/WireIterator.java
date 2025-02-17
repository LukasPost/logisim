/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.util.Iterator;

import logisim.data.Location;

class WireIterator implements Iterator<Location> {
	private int curX;
	private int curY;
	private int destX;
	private int destY;
	private int deltaX;
	private int deltaY;
	private boolean destReturned;

	public WireIterator(Location e0, Location e1) {
		curX = e0.x();
		curY = e0.y();
		destX = e1.x();
		destY = e1.y();
		destReturned = false;
		if (curX < destX)
			deltaX = 10;
		else if (curX > destX)
			deltaX = -10;
		else
			deltaX = 0;
		if (curY < destY)
			deltaY = 10;
		else if (curY > destY)
			deltaY = -10;
		else
			deltaY = 0;

		int offX = (destX - curX) % 10;
		// should not happen, but in case it does...
		if (offX != 0) destX = curX + deltaX * ((destX - curX) / 10);
		int offY = (destY - curY) % 10;
		// should not happen, but in case it does...
		if (offY != 0) destY = curY + deltaY * ((destY - curY) / 10);
	}

	public boolean hasNext() {
		return !destReturned;
	}

	public Location next() {
		Location ret = new Location(curX, curY);
		destReturned |= curX == destX && curY == destY;
		curX += deltaX;
		curY += deltaY;
		return ret;
	}

	public void remove() {
		throw new UnsupportedOperationException();
	}
}
