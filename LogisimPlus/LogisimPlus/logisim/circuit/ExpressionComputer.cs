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

	using Expression = logisim.analyze.model.Expression;
	using Location = logisim.data.Location;

	public interface ExpressionComputer
	{
		/// <summary>
		/// Propagates expression computation through a circuit. The parameter is a map from <code>Point</code>s to
		/// <code>Expression</code>s. The method will use this to determine the expressions coming into the component, and it
		/// should place any output expressions into the component.
		/// 
		/// If, in fact, no valid expression exists for the component, it throws <code>UnsupportedOperationException</code>.
		/// </summary>
		void computeExpression(Dictionary<Location, Expression> expressionMap);
	}

}
