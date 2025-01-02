// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	using logisim.util;

	internal class WireThread
	{
		private WireThread parent;
		private SmallSet<CircuitWires.ThreadBundle> bundles = new SmallSet<CircuitWires.ThreadBundle>();

		internal WireThread()
		{
			parent = this;
		}

		internal virtual SmallSet<CircuitWires.ThreadBundle> Bundles
		{
			get
			{
				return bundles;
			}
		}

		internal virtual void unite(WireThread other)
		{
			WireThread group = this.find();
			WireThread group2 = other.find();
			if (group != group2)
			{
				group.parent = group2;
			}
		}

		internal virtual WireThread find()
		{
			WireThread ret = this;
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
	}

}
