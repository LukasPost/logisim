// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit.appear
{
	using Circuit = logisim.circuit.Circuit;

	public class CircuitAppearanceEvent
	{
		public const int APPEARANCE = 1;
		public const int BOUNDS = 2;
		public const int PORTS = 4;
		public const int ALL_TYPES = 7;

		private Circuit circuit;
		private int affects;

		internal CircuitAppearanceEvent(Circuit circuit, int affects)
		{
			this.circuit = circuit;
			this.affects = affects;
		}

		public virtual Circuit Source
		{
			get
			{
				return circuit;
			}
		}

		public virtual bool isConcerning(int type)
		{
			return (affects & type) != 0;
		}
	}

}
