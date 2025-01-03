// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class Gates : Library
	{
		private List<Tool> tools = null;

		public Gates()
		{
			tools = new List<Tool>
			{
				new AddTool(NotGate.FACTORY),
				new AddTool(Buffer.FACTORY),
				new AddTool(AndGate.FACTORY),
				new AddTool(OrGate.FACTORY),
				new AddTool(NandGate.FACTORY),
				new AddTool(NorGate.FACTORY),
				new AddTool(XorGate.FACTORY),
				new AddTool(XnorGate.FACTORY),
				new AddTool(OddParityGate.FACTORY),
				new AddTool(EvenParityGate.FACTORY),
				new AddTool(ControlledBuffer.FACTORY_BUFFER),
				new AddTool(ControlledBuffer.FACTORY_INVERTER)
			};
		}

		public override string Name
		{
			get
			{
				return "Gates";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("gatesLibrary");
			}
		}

		public override List<Tool> Tools
		{
			get
			{
				return tools;
			}
		}
	}

}
