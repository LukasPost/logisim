// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using FactoryDescription = logisim.tools.FactoryDescription;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class Memory : Library
	{
		protected internal const int DELAY = 5;

		private static FactoryDescription[] DESCRIPTIONS = new FactoryDescription[]
		{
			new FactoryDescription("D Flip-Flop", Strings.getter("dFlipFlopComponent"), "dFlipFlop.gif", "DFlipFlop"),
			new FactoryDescription("T Flip-Flop", Strings.getter("tFlipFlopComponent"), "tFlipFlop.gif", "TFlipFlop"),
			new FactoryDescription("J-K Flip-Flop", Strings.getter("jkFlipFlopComponent"), "jkFlipFlop.gif", "JKFlipFlop"),
			new FactoryDescription("S-R Flip-Flop", Strings.getter("srFlipFlopComponent"), "srFlipFlop.gif", "SRFlipFlop"),
			new FactoryDescription("Register", Strings.getter("registerComponent"), "register.gif", "Register"),
			new FactoryDescription("Counter", Strings.getter("counterComponent"), "counter.gif", "Counter"),
			new FactoryDescription("Shift Register", Strings.getter("shiftRegisterComponent"), "shiftreg.gif", "ShiftRegister"),
			new FactoryDescription("Random", Strings.getter("randomComponent"), "random.gif", "Random"),
			new FactoryDescription("RAM", Strings.getter("ramComponent"), "ram.gif", "Ram"),
			new FactoryDescription("ROM", Strings.getter("romComponent"), "rom.gif", "Rom")
		};

		private IList<Tool> tools = null;

		public Memory()
		{
		}

		public override string Name
		{
			get
			{
				return "Memory";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("memoryLibrary");
			}
		}

		public override IList<Tool> Tools
		{
			get
			{
				if (tools == null)
				{
					tools = FactoryDescription.getTools(typeof(Memory), DESCRIPTIONS);
				}
				return tools;
			}
		}
	}

}
