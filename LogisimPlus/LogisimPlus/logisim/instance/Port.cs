// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{
	using EndData = logisim.comp.EndData;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Location = logisim.data.Location;
	using StringGetter = logisim.util.StringGetter;

	public class Port
	{
		public const string INPUT = "input";
		public const string OUTPUT = "output";
		public const string INOUT = "inout";

		public const string EXCLUSIVE = "exclusive";
		public const string SHARED = "shared";

		private int dx;
		private int dy;
		private int type;
		private BitWidth widthFixed;
		private Attribute<BitWidth> widthAttr;
		private bool exclude;
		private StringGetter toolTip;

		public Port(int dx, int dy, string type, BitWidth bits) : this(dx, dy, type, bits, defaultExclusive(type))
		{
		}

		public Port(int dx, int dy, string type, int bits) : this(dx, dy, type, BitWidth.create(bits), defaultExclusive(type))
		{
		}

		public Port(int dx, int dy, string type, int bits, string exclude) : this(dx, dy, type, BitWidth.create(bits), exclude)
		{
		}

		public Port(int dx, int dy, string type, BitWidth bits, string exclude)
		{
			this.dx = dx;
			this.dy = dy;
			this.type = toType(type);
			this.widthFixed = bits;
			this.widthAttr = null;
			this.exclude = toExclusive(exclude);
			this.toolTip = null;
		}

		public Port(int dx, int dy, string type, Attribute<BitWidth> attr) : this(dx, dy, type, attr, defaultExclusive(type))
		{
		}

		public Port(int dx, int dy, string type, Attribute<BitWidth> attr, string exclude)
		{
			this.dx = dx;
			this.dy = dy;
			this.type = toType(type);
			this.widthFixed = null;
			this.widthAttr = attr;
			this.exclude = toExclusive(exclude);
			this.toolTip = null;
		}

		public virtual StringGetter ToolTip
		{
			set
			{
				toolTip = value;
			}
			get
			{
				StringGetter getter = toolTip;
				return getter == null ? null : getter.get();
			}
		}


		public virtual Attribute<BitWidth> WidthAttribute
		{
			get
			{
				return widthAttr;
			}
		}

		public virtual EndData toEnd(Location loc, AttributeSet attrs)
		{
			Location pt = loc.translate(dx, dy);
			if (widthFixed != null)
			{
				return new EndData(pt, widthFixed, type, exclude);
			}
			else
			{
				object val = attrs.getValue(widthAttr);
				if (!(val is BitWidth))
				{
					throw new System.ArgumentException("Width attribute not set");
				}
				return new EndData(pt, (BitWidth) val, type, exclude);
			}
		}

		private static int toType(string s)
		{
			if (string.ReferenceEquals(s, null))
			{
				throw new System.ArgumentException("Null port type");
			}
			else if (s.Equals(INPUT))
			{
				return EndData.INPUT_ONLY;
			}
			else if (s.Equals(OUTPUT))
			{
				return EndData.OUTPUT_ONLY;
			}
			else if (s.Equals(INOUT))
			{
				return EndData.INPUT_OUTPUT;
			}
			else
			{
				throw new System.ArgumentException("Not recognized port type");
			}
		}

		private static string defaultExclusive(string s)
		{
			if (string.ReferenceEquals(s, null))
			{
				throw new System.ArgumentException("Null port type");
			}
			else if (s.Equals(INPUT))
			{
				return SHARED;
			}
			else if (s.Equals(OUTPUT))
			{
				return EXCLUSIVE;
			}
			else if (s.Equals(INOUT))
			{
				return SHARED;
			}
			else
			{
				throw new System.ArgumentException("Not recognized port type");
			}
		}

		private static bool toExclusive(string s)
		{
			if (string.ReferenceEquals(s, null))
			{
				throw new System.ArgumentException("Null exclusion type");
			}
			else if (s.Equals(EXCLUSIVE))
			{
				return true;
			}
			else if (s.Equals(SHARED))
			{
				return false;
			}
			else
			{
				throw new System.ArgumentException("Not recognized exclusion type");
			}
		}
	}

}
