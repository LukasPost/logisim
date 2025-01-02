// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	using BitWidth = logisim.data.BitWidth;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using logisim.util;

	internal class WireBundle
	{
		private BitWidth width = BitWidth.UNKNOWN;
		private Value pullValue = Value.UNKNOWN;
		private WireBundle parent;
		private Location widthDeterminant = null;
		internal WireThread[] threads = null;
		internal SmallSet<Location> points = new SmallSet<Location>(); // points bundle hits
		private WidthIncompatibilityData incompatibilityData = null;

		internal WireBundle()
		{
			parent = this;
		}

		internal virtual bool Valid
		{
			get
			{
				return incompatibilityData == null;
			}
		}

		internal virtual void setWidth(BitWidth width, Location det)
		{
			if (width == BitWidth.UNKNOWN)
			{
				return;
			}
			if (incompatibilityData != null)
			{
				incompatibilityData.add(det, width);
				return;
			}
			if (this.width != BitWidth.UNKNOWN)
			{
				if (width.Equals(this.width))
				{
					return; // the widths match, and the bundle is already set; nothing to do
				}
				else
				{ // the widths are broken: Create incompatibilityData holding this info
					incompatibilityData = new WidthIncompatibilityData();
					incompatibilityData.add(widthDeterminant, this.width);
					incompatibilityData.add(det, width);
					return;
				}
			}
			this.width = width;
			this.widthDeterminant = det;
			this.threads = new WireThread[width.Width];
			for (int i = 0; i < threads.Length; i++)
			{
				threads[i] = new WireThread();
			}
		}

		internal virtual BitWidth Width
		{
			get
			{
				if (incompatibilityData != null)
				{
					return BitWidth.UNKNOWN;
				}
				else
				{
					return width;
				}
			}
		}

		internal virtual Location WidthDeterminant
		{
			get
			{
				if (incompatibilityData != null)
				{
					return null;
				}
				else
				{
					return widthDeterminant;
				}
			}
		}

		internal virtual WidthIncompatibilityData WidthIncompatibilityData
		{
			get
			{
				return incompatibilityData;
			}
		}

		internal virtual void isolate()
		{
			parent = this;
		}

		internal virtual void unite(WireBundle other)
		{
			WireBundle group = this.find();
			WireBundle group2 = other.find();
			if (group != group2)
			{
				group.parent = group2;
			}
		}

		internal virtual WireBundle find()
		{
			WireBundle ret = this;
			if (ret.parent != ret)
			{
				do
				{
					ret = ret.parent;
				} while (ret.parent != ret);
				this.parent = ret;
			}
			return ret;
		}

		internal virtual void addPullValue(Value val)
		{
			pullValue = pullValue.combine(val);
		}

		internal virtual Value PullValue
		{
			get
			{
				return pullValue;
			}
		}
	}

}
