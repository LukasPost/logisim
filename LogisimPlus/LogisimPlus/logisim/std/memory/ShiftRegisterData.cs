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

	using BitWidth = logisim.data.BitWidth;
	using Value = logisim.data.Value;
	using InstanceData = logisim.instance.InstanceData;

	internal class ShiftRegisterData : ClockState, InstanceData
	{
		private BitWidth width;
		private Value[] vs;
		private int vsPos;

		public ShiftRegisterData(BitWidth width, int len)
		{
			this.width = width;
			this.vs = new Value[len];
			Arrays.Fill(this.vs, Value.createKnown(width, 0));
			this.vsPos = 0;
		}

		public override ShiftRegisterData clone()
		{
			ShiftRegisterData ret = (ShiftRegisterData) base.clone();
			ret.vs = (Value[])this.vs.Clone();
			return ret;
		}

		public virtual int Length
		{
			get
			{
				return vs.Length;
			}
		}

		public virtual void setSizes(BitWidth newWidth, int newLength)
		{
			Value[] v = vs;
			BitWidth oldWidth = width;
			int oldW = oldWidth.Width;
			int newW = newWidth.Width;
			if (v.Length != newLength)
			{
				Value[] newV = new Value[newLength];
				int j = vsPos;
				int copy = Math.Min(newLength, v.Length);
				for (int i = 0; i < copy; i++)
				{
					newV[i] = v[j];
					j++;
					if (j == v.Length)
					{
						j = 0;
					}
				}
				Arrays.Fill(newV, copy, newLength, Value.createKnown(newWidth, 0));
				v = newV;
				vsPos = 0;
				vs = newV;
			}
			if (oldW != newW)
			{
				for (int i = 0; i < v.Length; i++)
				{
					Value vi = v[i];
					if (vi.Width != newW)
					{
						v[i] = vi.extendWidth(newW, Value.FALSE);
					}
				}
				width = newWidth;
			}
		}

		public virtual void clear()
		{
			Arrays.Fill(vs, Value.createKnown(width, 0));
			vsPos = 0;
		}

		public virtual void push(Value v)
		{
			int pos = vsPos;
			vs[pos] = v;
			vsPos = pos >= vs.Length - 1 ? 0 : pos + 1;
		}

		public virtual Value get(int index)
		{
			int i = vsPos + index;
			Value[] v = vs;
			if (i >= v.Length)
			{
				i -= v.Length;
			}
			return v[i];
		}

		public virtual void set(int index, Value val)
		{
			int i = vsPos + index;
			Value[] v = vs;
			if (i >= v.Length)
			{
				i -= v.Length;
			}
			v[i] = val;
		}
	}
}
