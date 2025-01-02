// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{
	using Value = logisim.data.Value;

	public class DFlipFlop : AbstractFlipFlop
	{
		public DFlipFlop() : base("D Flip-Flop", "dFlipFlop.gif", Strings.getter("dFlipFlopComponent"), 1, true)
		{
		}

		protected internal override string getInputName(int index)
		{
			return "D";
		}

		protected internal override Value computeValue(Value[] inputs, Value curValue)
		{
			return inputs[0];
		}
	}

}
