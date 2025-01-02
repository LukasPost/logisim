// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{
	using CircuitState = logisim.circuit.CircuitState;
	using Simulator = logisim.circuit.Simulator;

	public interface SimulateListener
	{
		void stateChangeRequested(Simulator sim, CircuitState state);
	}

}
