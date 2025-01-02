// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.undo
{
	internal class ActionUnion : Action
	{
		internal Action first;
		internal Action second;

		internal ActionUnion(Action first, Action second)
		{
			this.first = first;
			this.second = second;
		}

		public override bool Modification
		{
			get
			{
				return first.Modification || second.Modification;
			}
		}

		public override string Name
		{
			get
			{
				return first.Name;
			}
		}

		public override void doIt()
		{
			first.doIt();
			second.doIt();
		}

		public override void undo()
		{
			second.undo();
			first.undo();
		}
	}

}
