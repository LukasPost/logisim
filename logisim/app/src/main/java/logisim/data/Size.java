/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

/**
 * Represents the dimensions of a rectangle. This is analogous to java.awt's <code>Dimension</code> class, except that
 * objects of this type are immutable.
 */
public class Size {
	private final int width;
	private final int height;

	public Size(int wid, int ht) {
		this.width = wid;
		this.height = ht;
	}

	@Override
	public boolean equals(Object other_obj) {
		return other_obj instanceof Size other && width == other.width && height == other.height;
	}

	@Override
	public String toString() {
		return width + "x" + height;
	}

	public int getWidth() {
		return width;
	}

	public int getHeight() {
		return height;
	}

	public java.awt.Dimension toAwtDimension() {
		return new java.awt.Dimension(width, height);
	}

	public boolean contains(Location p) {
		return contains(p.getX(), p.getY());
	}

	public boolean contains(int x, int y) {
		return x >= 0 && y >= 0 && x < this.width && y < this.height;
	}

	public boolean contains(int x, int y, int wid, int ht) {
		int oth_x = (wid <= 0 ? x : x + wid - 1);
		int oth_y = (ht <= 0 ? y : y + wid - 1);
		return contains(x, y) && contains(oth_x, oth_y);
	}

	public boolean contains(Size bd) {
		return contains(bd.width, bd.height);
	}

}
