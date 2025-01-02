// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{
	using Wire = logisim.circuit.Wire;
	using Location = logisim.data.Location;

	public class WireRepairData
	{
		private Wire wire;
		private Location point;

		public WireRepairData(Wire wire, Location point)
		{
			this.wire = wire;
			this.point = point;
		}

		public virtual Location Point
		{
			get
			{
				return point;
			}
		}

		public virtual Wire Wire
		{
			get
			{
				return wire;
			}
		}
	}

}
