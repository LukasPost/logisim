// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using logisim.data;
	using Direction = logisim.data.Direction;
	using StdAttr = logisim.instance.StdAttr;

	internal class GateAttributeList : System.Collections.ObjectModel.Collection<Attribute>
	{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final logisim.data.Attribute<?>[] BASE_ATTRIBUTES = { logisim.instance.StdAttr.FACING, logisim.instance.StdAttr.Width, GateAttributes.ATTR_SIZE, GateAttributes.ATTR_INPUTS, GateAttributes.ATTR_OUTPUT, logisim.instance.StdAttr.LABEL, logisim.instance.StdAttr.LABEL_FONT};
		private static readonly Attribute[] BASE_ATTRIBUTES = new Attribute[] {StdAttr.FACING, StdAttr.Width, GateAttributes.ATTR_SIZE, GateAttributes.ATTR_INPUTS, GateAttributes.ATTR_OUTPUT, StdAttr.LABEL, StdAttr.LABEL_FONT};

		private GateAttributes attrs;

		public GateAttributeList(GateAttributes attrs)
		{
			this.attrs = attrs;
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public logisim.data.Attribute<?> get(int index)
		public override Attribute get(int index)
		{
			int len = BASE_ATTRIBUTES.Length;
			if (index < len)
			{
				return BASE_ATTRIBUTES[index];
			}
			index -= len;
			if (attrs.xorBehave != null)
			{
				index--;
				if (index < 0)
				{
					return GateAttributes.ATTR_XOR;
				}
			}
			Direction facing = attrs.facing;
			int inputs = attrs.inputs;
			if (index == 0)
			{
				if (facing == Direction.East || facing == Direction.West)
				{
					return new NegateAttribute(index, Direction.North);
				}
				else
				{
					return new NegateAttribute(index, Direction.West);
				}
			}
			else if (index == inputs - 1)
			{
				if (facing == Direction.East || facing == Direction.West)
				{
					return new NegateAttribute(index, Direction.South);
				}
				else
				{
					return new NegateAttribute(index, Direction.East);
				}
			}
			else if (index < inputs)
			{
				return new NegateAttribute(index, null);
			}
			return null;
		}

		public override int size()
		{
			int ret = BASE_ATTRIBUTES.Length;
			if (attrs.xorBehave != null)
			{
				ret++;
			}
			ret += attrs.inputs;
			return ret;
		}
	}

}
