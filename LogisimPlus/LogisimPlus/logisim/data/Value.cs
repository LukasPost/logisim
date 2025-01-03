// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{

	public class Value
	{
		public static readonly Value FALSE = new Value(1, 0, 0, 0);
		public static readonly Value TRUE = new Value(1, 0, 0, 1);
		public static readonly Value UNKNOWN = new Value(1, 0, 1, 0);
		public static readonly Value ERROR = new Value(1, 1, 0, 0);
		public static readonly Value NIL = new Value(0, 0, 0, 0);

		public const int MAX_WIDTH = 32;

		public static readonly Color NIL_COLOR = Color.Gray;
		public static readonly Color FALSE_COLOR = Color.FromArgb(255, 0, 100, 0);
		public static readonly Color TRUE_COLOR = Color.FromArgb(255, 0, 210, 0);
		public static readonly Color UNKNOWN_COLOR = Color.FromArgb(255, 40, 40, 255);
		public static readonly Color ERROR_COLOR = Color.FromArgb(255, 192, 0, 0);
		public static readonly Color WIDTH_ERROR_COLOR = Color.FromArgb(255, 255, 123, 0);
		public static readonly Color MULTI_COLOR = Color.Black;

		public static Value create(Value[] values)
		{
			if (values.Length == 0)
			{
				return NIL;
			}
			if (values.Length == 1)
			{
				return values[0];
			}
			if (values.Length > MAX_WIDTH)
			{
				throw new Exception("Cannot have more than " + MAX_WIDTH + " bits in a value");
			}

			int width = values.Length;
			int value = 0;
			int unknown = 0;
			int error = 0;
			for (int i = 0; i < values.Length; i++)
			{
				int mask = 1 << i;
				if (values[i] == TRUE)
				{
					value |= mask;
				}
				else if (values[i] == FALSE)
				{
					/* do nothing */
					;
				}
				else if (values[i] == UNKNOWN)
				{
					unknown |= mask;
				}
				else if (values[i] == ERROR)
				{
					error |= mask;
				}
				else
				{
					throw new Exception("unrecognized value " + values[i]);
				}
			}
			return Value.create(width, error, unknown, value);
		}

		public static Value createKnown(BitWidth bits, int value)
		{
			return Value.create(bits.Width, 0, 0, value);
		}

		public static Value createUnknown(BitWidth bits)
		{
			return Value.create(bits.Width, 0, -1, 0);
		}

		public static Value createError(BitWidth bits)
		{
			return Value.create(bits.Width, -1, 0, 0);
		}

		private static Value create(int width, int error, int unknown, int value)
		{
			if (width == 0)
			{
				return Value.NIL;
			}
			else if (width == 1)
			{
				if ((error & 1) != 0)
				{
					return Value.ERROR;
				}
				else if ((unknown & 1) != 0)
				{
					return Value.UNKNOWN;
				}
				else if ((value & 1) != 0)
				{
					return Value.TRUE;
				}
				else
				{
					return Value.FALSE;
				}
			}
			else
			{
				int mask = (width == 32 ? -1 :~(-1 << width));
				error = error & mask;
				unknown = unknown & mask & ~error;
				value = value & mask & ~unknown & ~error;

				return new Value(width, error, unknown, value);
			}
		}

		public static Value repeat(Value @base, int bits)
		{
			if (@base.Width != 1)
			{
				throw new System.ArgumentException("first parameter must be one bit");
			}

			if (bits == 1)
			{
				return @base;
			}
			else
			{
				Value[] ret = new Value[bits];
				Arrays.Fill(ret, @base);
				return create(ret);
			}
		}

		private readonly int width;
		private readonly int error;
		private readonly int unknown;
		private readonly int value;

		private Value(int width, int error, int unknown, int value)
		{
			// To ensure that the one-bit values are unique, this should be called only
			// for the one-bit values and by the private create method
			this.width = width;
			this.error = error;
			this.unknown = unknown;
			this.value = value;
		}

		public virtual bool ErrorValue
		{
			get
			{
				return error != 0;
			}
		}

		public virtual Value extendWidth(int newWidth, Value others)
		{
			if (width == newWidth)
			{
				return this;
			}
			int maskInverse = (width == 32 ? 0 : (-1 << width));
			if (others == Value.ERROR)
			{
				return Value.create(newWidth, error | maskInverse, unknown, value);
			}
			else if (others == Value.FALSE)
			{
				return Value.create(newWidth, error, unknown, value);
			}
			else if (others == Value.TRUE)
			{
				return Value.create(newWidth, error, unknown, value | maskInverse);
			}
			else
			{
				return Value.create(newWidth, error, unknown | maskInverse, value);
			}
		}

		public virtual bool Unknown
		{
			get
			{
				if (width == 32)
				{
					return error == 0 && unknown == -1;
				}
				else
				{
					return error == 0 && unknown == ((1 << width) - 1);
				}
			}
		}

		public virtual bool FullyDefined
		{
			get
			{
				return width > 0 && error == 0 && unknown == 0;
			}
		}

		public virtual Value set(int which, Value val)
		{
			if (val.width != 1)
			{
				throw new Exception("Cannot set multiple values");
			}
			else if (which < 0 || which >= width)
			{
				throw new Exception("Attempt to set outside value's width");
			}
			else if (width == 1)
			{
				return val;
			}
			else
			{
				int mask = ~(1 << which);
				return Value.create(this.width, (this.error & mask) | (val.error << which), (this.unknown & mask) | (val.unknown << which), (this.value & mask) | (val.value << which));
			}
		}

		public virtual Value[] All
		{
			get
			{
				Value[] ret = new Value[width];
				for (int i = 0; i < ret.Length; i++)
				{
					ret[i] = get(i);
				}
				return ret;
			}
		}

		public virtual Value get(int which)
		{
			if (which < 0 || which >= width)
			{
				return ERROR;
			}
			int mask = 1 << which;
			if ((error & mask) != 0)
			{
				return ERROR;
			}
			else if ((unknown & mask) != 0)
			{
				return UNKNOWN;
			}
			else if ((value & mask) != 0)
			{
				return TRUE;
			}
			else
			{
				return FALSE;
			}
		}

		public virtual BitWidth BitWidth
		{
			get
			{
				return logisim.data.BitWidth.create(width);
			}
		}

		public virtual int Width
		{
			get
			{
				return width;
			}
		}

		public override bool Equals(object other_obj)
		{
			if (!(other_obj is Value))
			{
				return false;
			}
			Value other = (Value) other_obj;
			bool ret = this.width == other.Width && this.error == other.error && this.unknown == other.unknown && this.value == other.value;
			return ret;
		}

		public override int GetHashCode()
		{
			int ret = width;
			ret = 31 * ret + error;
			ret = 31 * ret + unknown;
			ret = 31 * ret + value;
			return ret;
		}

		public virtual int toIntValue()
		{
			if (error != 0)
			{
				return -1;
			}
			if (unknown != 0)
			{
				return -1;
			}
			return value;
		}

		public override string ToString()
		{
			switch (width)
			{
			case 0:
				return "-";
			case 1:
				if (error != 0)
				{
					return "E";
				}
				else if (unknown != 0)
				{
					return "x";
				}
				else if (value != 0)
				{
					return "1";
				}
				else
				{
					return "0";
				}
			default:
				StringBuilder ret = new StringBuilder();
				for (int i = width - 1; i >= 0; i--)
				{
					ret.Append(get(i).ToString());
					if (i % 4 == 0 && i != 0)
					{
						ret.Append(" ");
					}
				}
				return ret.ToString();
			}
		}

		public virtual string toOctalString()
		{
			if (width <= 1)
			{
				return ToString();
			}
			else
			{
				Value[] vals = All;
				char[] c = new char[(vals.Length + 2) / 3];
				for (int i = 0; i < c.Length; i++)
				{
					int k = c.Length - 1 - i;
					int frst = 3 * k;
					int last = Math.Min(vals.Length, 3 * (k + 1));
					int v = 0;
					c[i] = '?';
					for (int j = last - 1; j >= frst; j--)
					{
						if (vals[j] == Value.ERROR)
						{
							c[i] = 'E';
							break;
						}
						if (vals[j] == Value.UNKNOWN)
						{
							c[i] = 'x';
							break;
						}
						v = 2 * v;
						if (vals[j] == Value.TRUE)
						{
							v++;
						}
					}
					if (c[i] == '?')
					{
						c[i] = Character.forDigit(v, 8);
					}
				}
				return new string(c);
			}
		}

		public virtual string toHexString()
		{
			if (width <= 1)
			{
				return ToString();
			}
			else
			{
				Value[] vals = All;
				char[] c = new char[(vals.Length + 3) / 4];
				for (int i = 0; i < c.Length; i++)
				{
					int k = c.Length - 1 - i;
					int frst = 4 * k;
					int last = Math.Min(vals.Length, 4 * (k + 1));
					int v = 0;
					c[i] = '?';
					for (int j = last - 1; j >= frst; j--)
					{
						if (vals[j] == Value.ERROR)
						{
							c[i] = 'E';
							break;
						}
						if (vals[j] == Value.UNKNOWN)
						{
							c[i] = 'x';
							break;
						}
						v = 2 * v;
						if (vals[j] == Value.TRUE)
						{
							v++;
						}
					}
					if (c[i] == '?')
					{
						c[i] = Character.forDigit(v, 16);
					}
				}
				return new string(c);
			}
		}

		public virtual string toDecimalString(bool signed)
		{
			if (width == 0)
			{
				return "-";
			}
			if (ErrorValue)
			{
				return Strings.get("valueError");
			}
			if (!FullyDefined)
			{
				return Strings.get("valueUnknown");
			}

			int value = toIntValue();
			if (signed)
			{
				if (width < 32 && (value >> (width - 1)) != 0)
				{
					value |= (-1) << width;
				}
				return "" + value;
			}
			else
			{
				return "" + ((long) value & 0xFFFFFFFFL);
			}
		}

		public virtual string toDisplayString(int radix)
		{
			switch (radix)
			{
			case 2:
				return toDisplayString();
			case 8:
				return toOctalString();
			case 16:
				return toHexString();
			default:
				if (width == 0)
				{
					return "-";
				}
				if (ErrorValue)
				{
					return Strings.get("valueError");
				}
				if (!FullyDefined)
				{
					return Strings.get("valueUnknown");
				}
				return Convert.ToString(toIntValue(), radix);
			}
		}

		public virtual string toDisplayString()
		{
			switch (width)
			{
			case 0:
				return "-";
			case 1:
				if (error != 0)
				{
					return Strings.get("valueErrorSymbol");
				}
				else if (unknown != 0)
				{
					return Strings.get("valueUnknownSymbol");
				}
				else if (value != 0)
				{
					return "1";
				}
				else
				{
					return "0";
				}
			default:
				StringBuilder ret = new StringBuilder();
				for (int i = width - 1; i >= 0; i--)
				{
					ret.Append(get(i).ToString());
					if (i % 4 == 0 && i != 0)
					{
						ret.Append(" ");
					}
				}
				return ret.ToString();
			}
		}

		public virtual Value combine(Value other)
		{
			if (other == null)
			{
				return this;
			}
			if (this == NIL)
			{
				return other;
			}
			if (other == NIL)
			{
				return this;
			}
			if (this.width == 1 && other.Width == 1)
			{
				if (this == other)
				{
					return this;
				}
				if (this == UNKNOWN)
				{
					return other;
				}
				if (other == UNKNOWN)
				{
					return this;
				}
				return ERROR;
			}
			else
			{
				int disagree = (this.value ^ other.value) & ~(this.unknown | other.unknown);
				return Value.create(Math.Max(this.width, other.Width), this.error | other.error | disagree, this.unknown & other.unknown, (this.value & ~this.unknown) | (other.value & ~other.unknown));
			}
		}

		public virtual Value and(Value other)
		{
			if (other == null)
			{
				return this;
			}
			if (this.width == 1 && other.Width == 1)
			{
				if (this == FALSE || other == FALSE)
				{
					return FALSE;
				}
				if (this == TRUE && other == TRUE)
				{
					return TRUE;
				}
				return ERROR;
			}
			else
			{
				int false0 = ~this.value & ~this.error & ~this.unknown;
				int false1 = ~other.value & ~other.error & ~other.unknown;
				int falses = false0 | false1;
				return Value.create(Math.Max(this.width, other.Width), (this.error | other.error | this.unknown | other.unknown) & ~falses, 0, this.value & other.value);
			}
		}

		public virtual Value or(Value other)
		{
			if (other == null)
			{
				return this;
			}
			if (this.width == 1 && other.Width == 1)
			{
				if (this == TRUE || other == TRUE)
				{
					return TRUE;
				}
				if (this == FALSE && other == FALSE)
				{
					return FALSE;
				}
				return ERROR;
			}
			else
			{
				int true0 = this.value & ~this.error & ~this.unknown;
				int true1 = other.value & ~other.error & ~other.unknown;
				int trues = true0 | true1;
				return Value.create(Math.Max(this.width, other.Width), (this.error | other.error | this.unknown | other.unknown) & ~trues, 0, this.value | other.value);
			}
		}

		public virtual Value xor(Value other)
		{
			if (other == null)
			{
				return this;
			}
			if (this.width <= 1 && other.Width <= 1)
			{
				if (this == ERROR || other == ERROR)
				{
					return ERROR;
				}
				if (this == UNKNOWN || other == UNKNOWN)
				{
					return ERROR;
				}
				if (this == NIL || other == NIL)
				{
					return ERROR;
				}
				if ((this == TRUE) == (other == TRUE))
				{
					return FALSE;
				}
				return TRUE;
			}
			else
			{
				return Value.create(Math.Max(this.width, other.Width), this.error | other.error | this.unknown | other.unknown, 0, this.value ^ other.value);
			}
		}

		public virtual Value not()
		{
			if (width <= 1)
			{
				if (this == TRUE)
				{
					return FALSE;
				}
				if (this == FALSE)
				{
					return TRUE;
				}
				return ERROR;
			}
			else
			{
				return Value.create(this.width, this.error | this.unknown, 0, ~this.value);
			}
		}

		public virtual Color Color
		{
			get
			{
				if (error != 0)
				{
					return ERROR_COLOR;
				}
				else if (width == 0)
				{
					return NIL_COLOR;
				}
				else if (width == 1)
				{
					if (this == UNKNOWN)
					{
						return UNKNOWN_COLOR;
					}
					else if (this == TRUE)
					{
						return TRUE_COLOR;
					}
					else
					{
						return FALSE_COLOR;
					}
				}
				else
				{
					return MULTI_COLOR;
				}
			}
		}
	}

}
