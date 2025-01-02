// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace gray
{

	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;

	/// <summary>
	/// The library of components that the user can access. </summary>
	public class Components : Library
	{
		/// <summary>
		/// The list of all tools contained in this library. Technically, libraries contain tools, which is a slightly more
		/// general concept than components; practically speaking, though, you'll most often want to create AddTools for new
		/// components that can be added into the circuit.
		/// </summary>
		private IList<AddTool> tools;

		/// <summary>
		/// Constructs an instance of this library. This constructor is how Logisim accesses first when it opens the JAR
		/// file: It looks for a no-arguments constructor method of the user-designated class.
		/// </summary>
		public Components()
		{
			tools = new List<AddTool>
			{
				new AddTool(new GrayIncrementer()),
				new AddTool(new SimpleGrayCounter()),
				new AddTool(new GrayCounter())
			};
		}

		/// <summary>
		/// Returns the name of the library that the user will see. </summary>
		public override string DisplayName
		{
			get
			{
				return "Gray Tools";
			}
		}

		/// <summary>
		/// Returns a list of all the tools available in this library. </summary>
		public override IList<AddTool> Tools
		{
			get
			{
				return tools;
			}
		}
	}

}
