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

	public class JKFlipFlop : AbstractFlipFlop
	{
		public JKFlipFlop() : base("J-K Flip-Flop", "jkFlipFlop.gif", Strings.getter("jkFlipFlopComponent"), 2, false)
		{
		}

		protected internal override string getInputName(int index)
		{
			return index == 0 ? "J" : "K";
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
					return curValue.not();
				}
			}
			return Value.UNKNOWN;
		}
	}

}
