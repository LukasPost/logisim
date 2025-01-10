/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.Attributes;
import logisim.data.BitWidth;
import logisim.data.WireValue.WireValue;
import logisim.util.StringGetter;

public abstract class RadixOption extends AttributeOption {
	public static final RadixOption RADIX_2 = new Radix2();
	public static final RadixOption RADIX_8 = new Radix8();
	public static final RadixOption RADIX_10_UNSIGNED = new Radix10Unsigned();
	public static final RadixOption RADIX_10_SIGNED = new Radix10Signed();
	public static final RadixOption RADIX_16 = new Radix16();

	public static final RadixOption[] OPTIONS = { RADIX_2, RADIX_8, RADIX_10_SIGNED, RADIX_10_UNSIGNED, RADIX_16 };
	public static final Attribute<RadixOption> ATTRIBUTE = Attributes.forOption("radix", Strings.getter("radixAttr"),
			OPTIONS);

	public static RadixOption decode(String value) {
		for (RadixOption opt : OPTIONS) if (value.equals(opt.saveName)) return opt;
		return RADIX_2;
	}

	private String saveName;
	private StringGetter displayGetter;

	private RadixOption(String saveName, StringGetter displayGetter) {
		super(saveName, displayGetter);
		this.saveName = saveName;
		this.displayGetter = displayGetter;
	}

	public StringGetter getDisplayGetter() {
		return displayGetter;
	}

	public String getSaveString() {
		return saveName;
	}

	@Override
	public String toDisplayString() {
		return displayGetter.get();
	}

	@Override
	public String toString() {
		return saveName;
	}

	public abstract String toString(WireValue value);

	public abstract int getMaxLength(BitWidth width);

	public int getMaxLength(WireValue value) {
		return getMaxLength(value.getBitWidth());
	}

	private static class Radix2 extends RadixOption {
		private Radix2() {
			super("2", Strings.getter("radix2"));
		}

		@Override
		public String toString(WireValue value) {
			return value.toDisplayString(2);
		}

		@Override
		public int getMaxLength(WireValue value) {
			return value.toDisplayString(2).length();
		}

		@Override
		public int getMaxLength(BitWidth width) {
			int bits = width.getWidth();
			if (bits <= 1)
				return 1;
			return bits + ((bits - 1) / 4);
		}
	}

	private static class Radix10Signed extends RadixOption {
		private Radix10Signed() {
			super("10signed", Strings.getter("radix10Signed"));
		}

		@Override
		public String toString(WireValue value) {
			return value.toDecimalString(true);
		}

		@Override
		public int getMaxLength(BitWidth width) {
			return switch (width.getWidth()) {
				case 2, 3, 4 -> 2; // 2..8
				case 5, 6, 7 -> 3; // 16..64
				case 8, 9, 10 -> 4; // 128..512
				case 11, 12, 13, 14 -> 5; // 1K..8K
				case 15, 16, 17 -> 6; // 16K..64K
				case 18, 19, 20 -> 7; // 128K..256K
				case 21, 22, 23, 24 -> 8; // 1M..8M
				case 25, 26, 27 -> 9; // 16M..64M
				case 28, 29, 30 -> 10; // 128M..512M
				case 31, 32 -> 11; // 1G..2G
				default -> 1;
			};
		}
	}

	private static class Radix10Unsigned extends RadixOption {
		private Radix10Unsigned() {
			super("10unsigned", Strings.getter("radix10Unsigned"));
		}

		@Override
		public String toString(WireValue value) {
			return value.toDecimalString(false);
		}

		@Override
		public int getMaxLength(BitWidth width) {
			return switch (width.getWidth()) {
				case 4, 5, 6 -> 2;
				case 7, 8, 9 -> 3;
				case 10, 11, 12, 13 -> 4;
				case 14, 15, 16 -> 5;
				case 17, 18, 19 -> 6;
				case 20, 21, 22, 23 -> 7;
				case 24, 25, 26 -> 8;
				case 27, 28, 29 -> 9;
				case 30, 31, 32 -> 10;
				default -> 1;
			};
		}
	}

	private static class Radix8 extends RadixOption {
		private Radix8() {
			super("8", Strings.getter("radix8"));
		}

		@Override
		public String toString(WireValue value) {
			return value.toDisplayString(8);
		}

		@Override
		public int getMaxLength(WireValue value) {
			return value.toDisplayString(8).length();
		}

		@Override
		public int getMaxLength(BitWidth width) {
			return Math.max(1, (width.getWidth() + 2) / 3);
		}
	}

	private static class Radix16 extends RadixOption {
		private Radix16() {
			super("16", Strings.getter("radix16"));
		}

		@Override
		public String toString(WireValue value) {
			return value.toDisplayString(16);
		}

		@Override
		public int getMaxLength(BitWidth width) {
			return Math.max(1, (width.getWidth() + 3) / 4);
		}
	}
}
