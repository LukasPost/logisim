// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using SplitterFactory = logisim.circuit.SplitterFactory;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using AddTool = logisim.tools.AddTool;
	using FactoryDescription = logisim.tools.FactoryDescription;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class Wiring : Library
	{

		internal static readonly AttributeOption GATE_TOP_LEFT = new AttributeOption("tl", Strings.getter("wiringGateTopLeftOption"));
		internal static readonly AttributeOption GATE_BOTTOM_RIGHT = new AttributeOption("br", Strings.getter("wiringGateBottomRightOption"));
		internal static readonly Attribute<AttributeOption> ATTR_GATE = Attributes.forOption("gate", Strings.getter("wiringGateAttr"), new AttributeOption[] {GATE_TOP_LEFT, GATE_BOTTOM_RIGHT});

		private static Tool[] ADD_TOOLS = new Tool[]
		{
			new AddTool(SplitterFactory.instance),
			new AddTool(Pin.FACTORY),
			new AddTool(Probe.FACTORY),
			new AddTool(Tunnel.FACTORY),
			new AddTool(PullResistor.FACTORY),
			new AddTool(Clock.FACTORY),
			new AddTool(Constant.FACTORY)
		};

		private static FactoryDescription[] DESCRIPTIONS = new FactoryDescription[]
		{
			new FactoryDescription("Power", Strings.getter("powerComponent"), "power.gif", "Power"),
			new FactoryDescription("Ground", Strings.getter("groundComponent"), "ground.gif", "Ground"),
			new FactoryDescription("Transistor", Strings.getter("transistorComponent"), "trans0.gif", "Transistor"),
			new FactoryDescription("Transmission Gate", Strings.getter("transmissionGateComponent"), "transmis.gif", "TransmissionGate"),
			new FactoryDescription("Bit Extender", Strings.getter("extenderComponent"), "extender.gif", "BitExtender")
		};

		private IList<Tool> tools = null;

		public Wiring()
		{
		}

		public override string Name
		{
			get
			{
				return "Wiring";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("wiringLibrary");
			}
		}

		public override IList<Tool> Tools
		{
			get
			{
				if (tools == null)
				{
					IList<Tool> ret = new List<Tool>(ADD_TOOLS.Length + DESCRIPTIONS.Length);
					foreach (Tool a in ADD_TOOLS)
					{
						ret.Add(a);
					}
					((List<Tool>)ret).AddRange(FactoryDescription.getTools(typeof(Wiring), DESCRIPTIONS));
					tools = ret;
				}
				return tools;
			}
		}
	}

}
