// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using StringGetter = logisim.util.StringGetter;

	public class CircuitAction : Action
	{
		private StringGetter name;
		private CircuitTransaction forward;
		private CircuitTransaction reverse;

		internal CircuitAction(StringGetter name, CircuitMutation forward)
		{
			this.name = name;
			this.forward = forward;
		}

		public override string Name
		{
			get
			{
				return name.get();
			}
		}

		public override void doIt(Project proj)
		{
			CircuitTransactionResult result = forward.execute();
			if (result != null)
			{
				reverse = result.ReverseTransaction;
			}
		}

		public override void undo(Project proj)
		{
			if (reverse != null)
			{
				reverse.execute();
			}
		}
	}

}
