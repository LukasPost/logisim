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

	using Location = logisim.data.Location;

	public class WireSet
	{
		private static readonly ISet<Wire> NULL_WIRES = Collections.emptySet();
		public static readonly WireSet EMPTY = new WireSet(NULL_WIRES);

		private ISet<Wire> wires;
		private ISet<Location> points;

		internal WireSet(ISet<Wire> wires)
		{
			if (wires.Count == 0)
			{
				this.wires = NULL_WIRES;
				points = Collections.emptySet();
			}
			else
			{
				this.wires = wires;
				points = new HashSet<Location>();
				foreach (Wire w in wires)
				{
					points.Add(w.e0);
					points.Add(w.e1);
				}
			}
		}

		public virtual bool containsWire(Wire w)
		{
			return wires.Contains(w);
		}

		public virtual bool containsLocation(Location loc)
		{
			return points.Contains(loc);
		}
	}

}
