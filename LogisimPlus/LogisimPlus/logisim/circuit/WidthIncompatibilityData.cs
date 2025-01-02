// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using BitWidth = logisim.data.BitWidth;
	using Location = logisim.data.Location;

	public class WidthIncompatibilityData
	{
		private List<Location> points;
		private List<BitWidth> widths;

		public WidthIncompatibilityData()
		{
			points = new List<Location>();
			widths = new List<BitWidth>();
		}

		public virtual void add(Location p, BitWidth w)
		{
			for (int i = 0; i < points.Count; i++)
			{
				if (p.Equals(points[i]) && w.Equals(widths[i]))
				{
					return;
				}
			}
			points.Add(p);
			widths.Add(w);
		}

		public virtual int size()
		{
			return points.Count;
		}

		public virtual Location getPoint(int i)
		{
			return points[i];
		}

		public virtual BitWidth getBitWidth(int i)
		{
			return widths[i];
		}

		public override bool Equals(object other)
		{
			if (!(other is WidthIncompatibilityData))
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}

			WidthIncompatibilityData o = (WidthIncompatibilityData) other;
			if (this.size() != o.size())
			{
				return false;
			}
			for (int i = 0; i < this.size(); i++)
			{
				Location p = this.getPoint(i);
				BitWidth w = this.getBitWidth(i);
				bool matched = false;
				for (int j = 0; j < o.size(); j++)
				{
					Location q = this.getPoint(j);
					BitWidth x = this.getBitWidth(j);
					if (p.Equals(q) && w.Equals(x))
					{
						matched = true;
						break;
					}
				}
				if (!matched)
				{
					return false;
				}
			}
			return true;
		}
	}

}
