/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.util.Collection;
import java.util.Map;
import java.util.concurrent.locks.Lock;

import logisim.circuit.appear.CircuitPins;

public abstract class CircuitTransaction {
	public static final Integer READ_ONLY = 1;
	public static final Integer READ_WRITE = 2;

	protected abstract Map<Circuit, Integer> getAccessedCircuits();

	protected abstract void run(CircuitMutator mutator);

	public final CircuitTransactionResult execute() {
		CircuitMutatorImpl mutator = new CircuitMutatorImpl();
		Map<Circuit, Lock> locks = CircuitLocker.acquireLocks(this, mutator);
		CircuitTransactionResult result;
		try {
			run(mutator);

			// Let the port locations of each subcircuit's appearance be
			// updated to reflect the changes - this needs to happen before
			// wires are repaired because it could lead to some wires being
			// split
			Collection<Circuit> modified = mutator.getModifiedCircuits();
			for (Circuit circuit : modified) {
				CircuitMutatorImpl circMutator = circuit.getLocker().getMutator();
				if (circMutator == mutator) {
					CircuitPins pins = circuit.getAppearance().getCircuitPins();
					ReplacementMap repl = mutator.getReplacementMap(circuit);
					if (repl != null) pins.transactionCompleted(repl);
				}
			}

			// Now go through each affected circuit and repair its wires
			for (Circuit circuit : modified) {
				CircuitMutatorImpl circMutator = circuit.getLocker().getMutator();
				// this is a transaction executed within a transaction -
				// wait to repair wires until overall transaction is done
				if (circMutator == mutator) {
					WireRepair repair = new WireRepair(circuit);
					repair.run(mutator);
				} else circMutator.markModified(circuit);
			}

			result = new CircuitTransactionResult(mutator);
			for (Circuit circuit : result.getModifiedCircuits())
				circuit.fireEvent(CircuitEvent.TRANSACTION_DONE, result);
			return result;
		} finally {
			CircuitLocker.releaseLocks(locks);
		}
	}

}
