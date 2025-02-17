/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

import javax.swing.JComboBox;

import logisim.data.WireValue.WireValue;
import logisim.util.StringGetter;

import java.awt.Component;

public class BitWidth implements Comparable<BitWidth> {
	public static final BitWidth UNKNOWN = new BitWidth(0);
	public static final BitWidth ONE = new BitWidth(1);

	private static BitWidth[] prefab;

	static class Attribute extends logisim.data.Attribute<BitWidth> {
		private BitWidth[] choices;

		public Attribute(String name, StringGetter disp) {
			super(name, disp);
			ensurePrefab();
			choices = prefab;
		}

		public Attribute(String name, StringGetter disp, int min, int max) {
			super(name, disp);
			choices = new BitWidth[max - min + 1];
			for (int i = 0; i < choices.length; i++) choices[i] = BitWidth.create(min + i);
		}

		@Override
		public BitWidth parse(String value) {
			return BitWidth.parse(value);
		}

		@Override
		public Component getCellEditor(BitWidth value) {
			JComboBox<BitWidth> combo = new JComboBox<>(choices);
			if (value != null) {
				int wid = value.getWidth();
				if (wid <= 0 || wid > prefab.length) combo.addItem(value);
				combo.setSelectedItem(value);
			}
			return combo;
		}
	}

	final int width;

	private BitWidth(int width) {
		this.width = width;
	}

	public int getWidth() {
		return width;
	}

	public int getMask() {
		if (width == 0)
			return 0;
		else if (width == 32)
			return -1;
		else
			return (1 << width) - 1;
	}

	@Override
	public boolean equals(Object other_obj) {
		return other_obj instanceof BitWidth other && width == other.width;
	}

	public int compareTo(BitWidth other) {
		return width - other.width;
	}

	@Override
	public int hashCode() {
		return width;
	}

	@Override
	public String toString() {
		return "" + width;
	}

	public static BitWidth create(int width) {
		ensurePrefab();
		if (width <= 0) if (width == 0) return UNKNOWN;
		else throw new IllegalArgumentException("width " + width + " must be positive");
		else if (width - 1 < prefab.length) return prefab[width - 1];
		else return new BitWidth(width);
	}

	public static BitWidth parse(String str) {
		if (str == null || str.isEmpty()) throw new NumberFormatException("Width string cannot be null");
		if (str.charAt(0) == '/')
			str = str.substring(1);
		return create(Integer.parseInt(str));
	}

	private static void ensurePrefab() {
		if (prefab == null) {
			prefab = new BitWidth[Math.min(32, WireValue.MAX_WIDTH)];
			prefab[0] = ONE;
			for (int i = 1; i < prefab.length; i++) prefab[i] = new BitWidth(i + 1);
		}
	}
}
