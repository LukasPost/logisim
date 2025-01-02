// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.arith
{

	using FactoryDescription = logisim.tools.FactoryDescription;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class Arithmetic : Library
	{
		private static FactoryDescription[] DESCRIPTIONS = new FactoryDescription[]
		{
			new FactoryDescription("Adder", Strings.getter("adderComponent"), "adder.gif", "Adder"),
			new FactoryDescription("Subtractor", Strings.getter("subtractorComponent"), "subtractor.gif", "Subtractor"),
			new FactoryDescription("Multiplier", Strings.getter("multiplierComponent"), "multiplier.gif", "Multiplier"),
			new FactoryDescription("Divider", Strings.getter("dividerComponent"), "divider.gif", "Divider"),
			new FactoryDescription("Negator", Strings.getter("negatorComponent"), "negator.gif", "Negator"),
			new FactoryDescription("Comparator", Strings.getter("comparatorComponent"), "comparator.gif", "Comparator"),
			new FactoryDescription("Shifter", Strings.getter("shifterComponent"), "shifter.gif", "Shifter"),
			new FactoryDescription("BitAdder", Strings.getter("bitAdderComponent"), "bitadder.gif", "BitAdder"),
			new FactoryDescription("BitFinder", Strings.getter("bitFinderComponent"), "bitfindr.gif", "BitFinder")
		};

		private IList<Tool> tools = null;

		public Arithmetic()
		{
		}

		public override string Name
		{
			get
			{
				return "Arithmetic";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("arithmeticLibrary");
			}
		}

		public override IList<Tool> Tools
		{
			get
			{
				if (tools == null)
				{
					tools = FactoryDescription.getTools(typeof(Arithmetic), DESCRIPTIONS);
				}
				return tools;
			}
		}
	}

}
