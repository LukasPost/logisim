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

	public class TFlipFlop : AbstractFlipFlop
	{
		public TFlipFlop() : base("T Flip-Flop", "tFlipFlop.gif", Strings.getter("tFlipFlopComponent"), 1, false)
		{
		}

		protected internal override string getInputName(int index)
		{
			return "T";
		}

		protected internal override Value computeValue(Value[] inputs, Value curValue)
		{
			if (curValue == Value.UNKNOWN)
			{
				curValue = Value.FALSE;
			}
			if (inputs[0] == Value.TRUE)
			{
				return curValue.not();
			}
			else
			{
				return curValue;
			}
		}
	}

}
