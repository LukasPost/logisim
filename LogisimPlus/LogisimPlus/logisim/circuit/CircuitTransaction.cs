// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using CircuitPins = logisim.circuit.appear.CircuitPins;

	public abstract class CircuitTransaction
	{
		public static readonly int READ_ONLY = Convert.ToInt32(1);
		public static readonly int READ_WRITE = Convert.ToInt32(2);

		protected internal abstract IDictionary<Circuit, int> AccessedCircuits {get;}

		protected internal abstract void run(CircuitMutator mutator);

		public CircuitTransactionResult execute()
		{
			CircuitMutatorImpl mutator = new CircuitMutatorImpl();
			IDictionary<Circuit, Lock> locks = CircuitLocker.acquireLocks(this, mutator);
			CircuitTransactionResult result;
			try
			{
				this.run(mutator);

				// Let the port locations of each subcircuit's appearance be
				// updated to reflect the changes - this needs to happen before
				// wires are repaired because it could lead to some wires being
				// split
				ICollection<Circuit> modified = mutator.ModifiedCircuits;
				foreach (Circuit circuit in modified)
				{
					CircuitMutatorImpl circMutator = circuit.Locker.getMutator();
					if (circMutator == mutator)
					{
						CircuitPins pins = circuit.Appearance.getCircuitPins();
						ReplacementMap repl = mutator.getReplacementMap(circuit);
						if (repl != null)
						{
							pins.transactionCompleted(repl);
						}
					}
				}

				// Now go through each affected circuit and repair its wires
				foreach (Circuit circuit in modified)
				{
					CircuitMutatorImpl circMutator = circuit.Locker.getMutator();
					if (circMutator == mutator)
					{
						WireRepair repair = new WireRepair(circuit);
						repair.run(mutator);
					}
					else
					{
						// this is a transaction executed within a transaction -
						// wait to repair wires until overall transaction is done
						circMutator.markModified(circuit);
					}
				}

				result = new CircuitTransactionResult(mutator);
				foreach (Circuit circuit in result.ModifiedCircuits)
				{
					circuit.fireEvent(CircuitEvent.TRANSACTION_DONE, result);
				}
			}
			finally
			{
				CircuitLocker.releaseLocks(locks);
			}
			return result;
		}

	}

}
