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

	public class SRFlipFlop : AbstractFlipFlop
	{
		public SRFlipFlop() : base("S-R Flip-Flop", "srFlipFlop.gif", Strings.getter("srFlipFlopComponent"), 2, true)
		{
		}

		protected internal override string getInputName(int index)
		{
			return index == 0 ? "S" : "R";
		}

		protected internal override Value computeValue(Value[] inputs, Value curValue)
		{
			if (inputs[0] == Value.FALSE)
			{
				if (inputs[1] == Value.FALSE)
				{
					return curValue;
				}
				else if (inputs[1] == Value.TRUE)
				{
					return Value.FALSE;
				}
			}
			else if (inputs[0] == Value.TRUE)
			{
				if (inputs[1] == Value.FALSE)
				{
					return Value.TRUE;
				}
				else if (inputs[1] == Value.TRUE)
				{
					return Value.ERROR;
				}
			}
			return Value.UNKNOWN;
		}
	}

}
