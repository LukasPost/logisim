// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{
	using BitWidth = logisim.data.BitWidth;
	using Location = logisim.data.Location;

	public class EndData
	{
		public const int INPUT_ONLY = 1;
		public const int OUTPUT_ONLY = 2;
		public const int INPUT_OUTPUT = 3;

		private Location loc;
		private BitWidth width;
		private int i_o;
		private bool exclusive;

		public EndData(Location loc, BitWidth width, int type, bool exclusive)
		{
			this.loc = loc;
			this.width = width;
			this.i_o = type;
			this.exclusive = exclusive;
		}

		public EndData(Location loc, BitWidth width, int type) : this(loc, width, type, type == OUTPUT_ONLY)
		{
		}

		public virtual bool Exclusive
		{
			get
			{
				return exclusive;
			}
		}

		public virtual bool Input
		{
			get
			{
				return (i_o & INPUT_ONLY) != 0;
			}
		}

		public virtual bool Output
		{
			get
			{
				return (i_o & OUTPUT_ONLY) != 0;
			}
		}

		public virtual Location Location
		{
			get
			{
				return loc;
			}
		}

		public virtual BitWidth Width
		{
			get
			{
				return width;
			}
		}

		public virtual int Type
		{
			get
			{
				return i_o;
			}
		}

		public override bool Equals(object other)
		{
			if (!(other is EndData))
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			EndData o = (EndData) other;
			return o.loc.Equals(this.loc) && o.width.Equals(this.width) && o.i_o == this.i_o && o.exclusive == this.exclusive;
		}
	}

}
