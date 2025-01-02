// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{

	using StringGetter = logisim.util.StringGetter;

	public class BitWidth : IComparable<BitWidth>
	{
		public static readonly BitWidth UNKNOWN = new BitWidth(0);
		public static readonly BitWidth ONE = new BitWidth(1);

		private static BitWidth[] prefab = null;

		internal class Attribute : logisim.data.Attribute<BitWidth>
		{
			internal BitWidth[] choices;

			public Attribute(string name, StringGetter disp) : base(name, disp)
			{
				ensurePrefab();
				choices = prefab;
			}

			public Attribute(string name, StringGetter disp, int min, int max) : base(name, disp)
			{
				choices = new BitWidth[max - min + 1];
				for (int i = 0; i < choices.Length; i++)
				{
					choices[i] = BitWidth.create(min + i);
				}
			}

			public override BitWidth parse(string value)
			{
				return BitWidth.parse(value);
			}

			public override java.awt.Component getCellEditor(BitWidth value)
			{
				JComboBox<BitWidth> combo = new JComboBox<BitWidth>(choices);
				if (value != null)
				{
					int wid = value.Width;
					if (wid <= 0 || wid > prefab.Length)
					{
						combo.addItem(value);
					}
					combo.setSelectedItem(value);
				}
				return combo;
			}
		}

		internal readonly int width;

		private BitWidth(int width)
		{
			this.width = width;
		}

		public virtual int Width
		{
			get
			{
				return width;
			}
		}

		public virtual int Mask
		{
			get
			{
				if (width == 0)
				{
					return 0;
				}
				else if (width == 32)
				{
					return -1;
				}
				else
				{
					return (1 << width) - 1;
				}
			}
		}

		public override bool Equals(object other_obj)
		{
			if (!(other_obj is BitWidth))
			{
				return false;
			}
			BitWidth other = (BitWidth) other_obj;
			return this.width == other.width;
		}

		public virtual int CompareTo(BitWidth other)
		{
			return this.width - other.width;
		}

		public override int GetHashCode()
		{
			return width;
		}

		public override string ToString()
		{
			return "" + width;
		}

		public static BitWidth create(int width)
		{
			ensurePrefab();
			if (width <= 0)
			{
				if (width == 0)
				{
					return UNKNOWN;
				}
				else
				{
					throw new System.ArgumentException("width " + width + " must be positive");
				}
			}
			else if (width - 1 < prefab.Length)
			{
				return prefab[width - 1];
			}
			else
			{
				return new BitWidth(width);
			}
		}

		public static BitWidth parse(string str)
		{
			if (string.ReferenceEquals(str, null) || str.Length == 0)
			{
				throw new System.FormatException("Width string cannot be null");
			}
			if (str[0] == '/')
			{
				str = str.Substring(1);
			}
			return create(int.Parse(str));
		}

		private static void ensurePrefab()
		{
			if (prefab == null)
			{
				prefab = new BitWidth[Math.Min(32, Value.MAX_WIDTH)];
				prefab[0] = ONE;
				for (int i = 1; i < prefab.Length; i++)
				{
					prefab[i] = new BitWidth(i + 1);
				}
			}
		}
	}

}
