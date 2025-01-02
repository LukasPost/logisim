// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std
{

	using Arithmetic = logisim.std.arith.Arithmetic;
	using Base = logisim.std.@base.Base;
	using Gates = logisim.std.gates.Gates;
	using Io = logisim.std.io.Io;
	using Memory = logisim.std.memory.Memory;
	using Plexers = logisim.std.plexers.Plexers;
	using Wiring = logisim.std.wiring.Wiring;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class Builtin : Library
	{
		private IList<Library> libraries = null;

		public Builtin()
		{
			libraries = new List<Library>
			{
				new Base(),
				new Gates(),
				new Wiring(),
				new Plexers(),
				new Arithmetic(),
				new Memory(),
				new Io()
			};
		}

		public override string Name
		{
			get
			{
				return "Builtin";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("builtinLibrary");
			}
		}

		public override IList<Tool> Tools
		{
			get
			{
				return Collections.emptyList();
			}
		}

		public override IList<Library> Libraries
		{
			get
			{
				return libraries;
			}
		}
	}

}
