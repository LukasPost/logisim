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
	using Attributes = logisim.data.Attributes;
	using Direction = logisim.data.Direction;
	using StringUtil = logisim.util.StringUtil;

	internal class NegateAttribute : Attribute
	{
		private static Attribute BOOLEAN_ATTR = Attributes.forBoolean("negateDummy");

		internal int index;
		private Direction side;

		public NegateAttribute(int index, Direction side) : base("negate" + index, null)
		{
			this.index = index;
			this.side = side;
		}

		public override bool Equals(object other)
		{
			if (other is NegateAttribute)
			{
				NegateAttribute o = (NegateAttribute) other;
				return this.index == o.index && this.side == o.side;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return index * 31 + (side == null ? 0 : side.GetHashCode());
		}

		public override string DisplayName
		{
			get
			{
				string ret = StringUtil.format(Strings.get("gateNegateAttr"), "" + (index + 1));
				if (side != null)
				{
					ret += " (" + side.toVerticalDisplayString() + ")";
				}
				return ret;
			}
		}

		public override string toDisplayString(bool? value)
		{
			return BOOLEAN_ATTR.toDisplayString(value.Value);
		}

		public override bool? parse(string value)
		{
			return BOOLEAN_ATTR.parse(value);
		}

		public override java.awt.Component getCellEditor(bool? value)
		{
			return BOOLEAN_ATTR.getCellEditor(null, value.Value);
		}

	}

}
