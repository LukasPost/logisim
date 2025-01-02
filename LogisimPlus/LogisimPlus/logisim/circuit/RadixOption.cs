// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Value = logisim.data.Value;
	using StringGetter = logisim.util.StringGetter;

	public abstract class RadixOption : AttributeOption
	{
		public static readonly RadixOption RADIX_2 = new Radix2();
		public static readonly RadixOption RADIX_8 = new Radix8();
		public static readonly RadixOption RADIX_10_UNSIGNED = new Radix10Unsigned();
		public static readonly RadixOption RADIX_10_SIGNED = new Radix10Signed();
		public static readonly RadixOption RADIX_16 = new Radix16();

		public static readonly RadixOption[] OPTIONS = new RadixOption[] {RADIX_2, RADIX_8, RADIX_10_SIGNED, RADIX_10_UNSIGNED, RADIX_16};
		public static readonly Attribute<RadixOption> ATTRIBUTE = Attributes.forOption("radix", Strings.getter("radixAttr"), OPTIONS);

		public static RadixOption decode(string value)
		{
			foreach (RadixOption opt in OPTIONS)
			{
				if (value.Equals(opt.saveName))
				{
					return opt;
				}
			}
			return RADIX_2;
		}

		private string saveName;
		private StringGetter displayGetter;

		private RadixOption(string saveName, StringGetter displayGetter) : base(saveName, displayGetter)
		{
			this.saveName = saveName;
			this.displayGetter = displayGetter;
		}

		public virtual StringGetter DisplayGetter
		{
			get
			{
				return displayGetter;
			}
		}

		public virtual string SaveString
		{
			get
			{
				return saveName;
			}
		}

		public override string toDisplayString()
		{
			return displayGetter.get();
		}

		public override string ToString()
		{
			return saveName;
		}

		public abstract string toString(Value value);

		public abstract int getMaxLength(BitWidth width);

		public virtual int getMaxLength(Value value)
		{
			return getMaxLength(value.BitWidth);
		}

		private class Radix2 : RadixOption
		{
			internal Radix2() : base("2", Strings.getter("radix2"))
			{
			}

			public override string toString(Value value)
			{
				return value.toDisplayString(2);
			}

			public override int getMaxLength(Value value)
			{
				return value.toDisplayString(2).Length;
			}

			public override int getMaxLength(BitWidth width)
			{
				int bits = width.Width;
				if (bits <= 1)
				{
					return 1;
				}
				return bits + ((bits - 1) / 4);
			}
		}

		private class Radix10Signed : RadixOption
		{
			internal Radix10Signed() : base("10signed", Strings.getter("radix10Signed"))
			{
			}

			public override string toString(Value value)
			{
				return value.toDecimalString(true);
			}

			public override int getMaxLength(BitWidth width)
			{
				switch (width.Width)
				{
				case 2:
				case 3:
				case 4:
					return 2; // 2..8
				case 5:
				case 6:
				case 7:
					return 3; // 16..64
				case 8:
				case 9:
				case 10:
					return 4; // 128..512
				case 11:
				case 12:
				case 13:
				case 14:
					return 5; // 1K..8K
				case 15:
				case 16:
				case 17:
					return 6; // 16K..64K
				case 18:
				case 19:
				case 20:
					return 7; // 128K..256K
				case 21:
				case 22:
				case 23:
				case 24:
					return 8; // 1M..8M
				case 25:
				case 26:
				case 27:
					return 9; // 16M..64M
				case 28:
				case 29:
				case 30:
					return 10; // 128M..512M
				case 31:
				case 32:
					return 11; // 1G..2G
				default:
					return 1;
				}
			}
		}

		private class Radix10Unsigned : RadixOption
		{
			internal Radix10Unsigned() : base("10unsigned", Strings.getter("radix10Unsigned"))
			{
			}

			public override string toString(Value value)
			{
				return value.toDecimalString(false);
			}

			public override int getMaxLength(BitWidth width)
			{
				switch (width.Width)
				{
				case 4:
				case 5:
				case 6:
					return 2;
				case 7:
				case 8:
				case 9:
					return 3;
				case 10:
				case 11:
				case 12:
				case 13:
					return 4;
				case 14:
				case 15:
				case 16:
					return 5;
				case 17:
				case 18:
				case 19:
					return 6;
				case 20:
				case 21:
				case 22:
				case 23:
					return 7;
				case 24:
				case 25:
				case 26:
					return 8;
				case 27:
				case 28:
				case 29:
					return 9;
				case 30:
				case 31:
				case 32:
					return 10;
				default:
					return 1;
				}
			}
		}

		private class Radix8 : RadixOption
		{
			internal Radix8() : base("8", Strings.getter("radix8"))
			{
			}

			public override string toString(Value value)
			{
				return value.toDisplayString(8);
			}

			public override int getMaxLength(Value value)
			{
				return value.toDisplayString(8).Length;
			}

			public override int getMaxLength(BitWidth width)
			{
				return Math.Max(1, (width.Width + 2) / 3);
			}
		}

		private class Radix16 : RadixOption
		{
			internal Radix16() : base("16", Strings.getter("radix16"))
			{
			}

			public override string toString(Value value)
			{
				return value.toDisplayString(16);
			}

			public override int getMaxLength(BitWidth width)
			{
				return Math.Max(1, (width.Width + 3) / 4);
			}
		}
	}

}
