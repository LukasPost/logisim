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

	public class CircuitTransactionResult
	{
		private CircuitMutatorImpl mutator;

		internal CircuitTransactionResult(CircuitMutatorImpl mutator)
		{
			this.mutator = mutator;
		}

		public virtual CircuitTransaction ReverseTransaction
		{
			get
			{
				return mutator.ReverseTransaction;
			}
		}

		public virtual ReplacementMap getReplacementMap(Circuit circuit)
		{
			ReplacementMap ret = mutator.getReplacementMap(circuit);
			return ret == null ? new ReplacementMap() : ret;
		}

		public virtual ICollection<Circuit> ModifiedCircuits
		{
			get
			{
				return mutator.ModifiedCircuits;
			}
		}
	}

}
