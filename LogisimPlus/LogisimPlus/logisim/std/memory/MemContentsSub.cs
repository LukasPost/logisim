// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	internal class MemContentsSub
	{
		private MemContentsSub()
		{
		}

		internal static ContentsInterface createContents(int size, int bits)
		{
			if (bits <= 8)
			{
				return new ByteContents(size);
			}
			else if (bits <= 16)
			{
				return new ShortContents(size);
			}
			else
			{
				return new IntContents(size);
			}
		}

		internal abstract class ContentsInterface : ICloneable
		{
			public override ContentsInterface clone()
			{
				try
				{
					return (ContentsInterface) base.clone();
				}
				catch (CloneNotSupportedException)
				{
					return this;
				}
			}

			internal abstract int Length {get;}

			internal abstract int get(int addr);

			internal abstract void set(int addr, int value);

			internal abstract void clear();

			internal abstract void load(int start, int[] values, int mask);

			internal virtual bool matches(int[] values, int start, int mask)
			{
				for (int i = 0; i < values.Length; i++)
				{
					if (get(start + i) != (values[i] & mask))
					{
						return false;
					}
				}
				return true;
			}

			internal virtual int[] get(int start, int len)
			{
				int[] ret = new int[len];
				for (int i = 0; i < ret.Length; i++)
				{
					ret[i] = get(start + i);
				}
				return ret;
			}

			internal virtual bool Clear
			{
				get
				{
					for (int i = 0, n = Length; i < n; i++)
					{
						if (get(i) != 0)
						{
							return false;
						}
					}
					return true;
				}
			}
		}

		private class ByteContents : ContentsInterface
		{
			internal sbyte[] data;

			public ByteContents(int size)
			{
				data = new sbyte[size];
			}

			public override ByteContents clone()
			{
				ByteContents ret = (ByteContents) base.clone();
				ret.data = new sbyte[this.data.Length];
				Array.Copy(this.data, 0, ret.data, 0, this.data.Length);
				return ret;
			}

			//
			// methods for accessing data within memory
			//
			internal override int Length
			{
				get
				{
					return data.Length;
				}
			}

			internal override int get(int addr)
			{
				return addr >= 0 && addr < data.Length ? data[addr] : 0;
			}

			internal override void set(int addr, int value)
			{
				if (addr >= 0 && addr < data.Length)
				{
					sbyte oldValue = data[addr];
					if (value != oldValue)
					{
						data[addr] = (sbyte) value;
					}
				}
			}

			internal override void clear()
			{
				Arrays.Fill(data, (sbyte) 0);
			}

			internal override void load(int start, int[] values, int mask)
			{
				int n = Math.Min(values.Length, data.Length - start);
				for (int i = 0; i < n; i++)
				{
					data[start + i] = (sbyte)(values[i] & mask);
				}
			}
		}

		private class ShortContents : ContentsInterface
		{
			internal short[] data;

			public ShortContents(int size)
			{
				data = new short[size];
			}

			public override ShortContents clone()
			{
				ShortContents ret = (ShortContents) base.clone();
				ret.data = new short[this.data.Length];
				Array.Copy(this.data, 0, ret.data, 0, this.data.Length);
				return ret;
			}

			//
			// methods for accessing data within memory
			//
			internal override int Length
			{
				get
				{
					return data.Length;
				}
			}

			internal override int get(int addr)
			{
				return addr >= 0 && addr < data.Length ? data[addr] : 0;
			}

			internal override void set(int addr, int value)
			{
				if (addr >= 0 && addr < data.Length)
				{
					short oldValue = data[addr];
					if (value != oldValue)
					{
						data[addr] = (short) value;
					}
				}
			}

			internal override void clear()
			{
				Arrays.Fill(data, (short) 0);
			}

			internal override void load(int start, int[] values, int mask)
			{
				int n = Math.Min(values.Length, data.Length - start);
				for (int i = 0; i < n; i++)
				{
					data[start + i] = (short)(values[i] & mask);
				}
			}
		}

		private class IntContents : ContentsInterface
		{
			internal int[] data;

			public IntContents(int size)
			{
				data = new int[size];
			}

			public override IntContents clone()
			{
				IntContents ret = (IntContents) base.clone();
				ret.data = new int[this.data.Length];
				Array.Copy(this.data, 0, ret.data, 0, this.data.Length);
				return ret;
			}

			//
			// methods for accessing data within memory
			//
			internal override int Length
			{
				get
				{
					return data.Length;
				}
			}

			internal override int get(int addr)
			{
				return addr >= 0 && addr < data.Length ? data[addr] : 0;
			}

			internal override void set(int addr, int value)
			{
				if (addr >= 0 && addr < data.Length)
				{
					int oldValue = data[addr];
					if (value != oldValue)
					{
						data[addr] = value;
					}
				}
			}

			internal override void clear()
			{
				Arrays.Fill(data, 0);
			}

			internal override void load(int start, int[] values, int mask)
			{
				int n = Math.Min(values.Length, data.Length - start);
				for (int i = 0; i < n; i++)
				{
					data[i] = values[i] & mask;
				}
			}
		}
	}

}
