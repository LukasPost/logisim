// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.@base
{

	using Library = logisim.tools.Library;
	using MenuTool = logisim.tools.MenuTool;
	using PokeTool = logisim.tools.PokeTool;
	using SelectTool = logisim.tools.SelectTool;
	using TextTool = logisim.tools.TextTool;
	using AddTool = logisim.tools.AddTool;
	using EditTool = logisim.tools.EditTool;
	using Tool = logisim.tools.Tool;
	using WiringTool = logisim.tools.WiringTool;

	public class Base : Library
	{
		private IList<Tool> tools = null;

		public Base()
		{
			SelectTool select = new SelectTool();
			WiringTool wiring = new WiringTool();

			tools = new List<Tool>
			{
				new PokeTool(),
				new EditTool(select, wiring),
				select,
				wiring,
				new TextTool(),
				new MenuTool(),
				new AddTool(Text.FACTORY)
			};
		}

		public override string Name
		{
			get
			{
				return "Base";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("baseLibrary");
			}
		}

		public override IList<Tool> Tools
		{
			get
			{
				return tools;
			}
		}
	}

}
