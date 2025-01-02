// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{

	using Circuit = logisim.circuit.Circuit;
	using Project = logisim.proj.Project;

	public class AnalyzerModel
	{
		public const int MAX_INPUTS = 12;
		public const int MAX_OUTPUTS = 12;

		public const int FORMAT_SUM_OF_PRODUCTS = 0;
		public const int FORMAT_PRODUCT_OF_SUMS = 1;

		private VariableList inputs = new VariableList(MAX_INPUTS);
		private VariableList outputs = new VariableList(MAX_OUTPUTS);
		private TruthTable table;
		private OutputExpressions outputExpressions;
		private Project currentProject = null;
		private Circuit currentCircuit = null;

		public AnalyzerModel()
		{
			// the order here is important, because the output expressions
			// need the truth table to exist for listening.
			table = new TruthTable(this);
			outputExpressions = new OutputExpressions(this);
		}

		//
		// access methods
		//
		public virtual Project CurrentProject
		{
			get
			{
				return currentProject;
			}
		}

		public virtual Circuit CurrentCircuit
		{
			get
			{
				return currentCircuit;
			}
		}

		public virtual VariableList Inputs
		{
			get
			{
				return inputs;
			}
		}

		public virtual VariableList Outputs
		{
			get
			{
				return outputs;
			}
		}

		public virtual TruthTable TruthTable
		{
			get
			{
				return table;
			}
		}

		public virtual OutputExpressions OutputExpressions
		{
			get
			{
				return outputExpressions;
			}
		}

		//
		// modifier methods
		//
		public virtual void setCurrentCircuit(Project value, Circuit circuit)
		{
			currentProject = value;
			currentCircuit = circuit;
		}

		public virtual void setVariables(IList<string> inputs, IList<string> outputs)
		{
			this.inputs.All = inputs;
			this.outputs.All = outputs;
		}
	}

}
