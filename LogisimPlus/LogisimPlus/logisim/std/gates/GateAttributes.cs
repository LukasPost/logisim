// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Direction = logisim.data.Direction;
	using StdAttr = logisim.instance.StdAttr;

	internal class GateAttributes : AbstractAttributeSet
	{
		internal const int MAX_INPUTS = 32;
		internal const int DELAY = 1;

		internal static readonly AttributeOption SIZE_NARROW = new AttributeOption(Convert.ToInt32(30), Strings.getter("gateSizeNarrowOpt"));
		internal static readonly AttributeOption SIZE_MEDIUM = new AttributeOption(Convert.ToInt32(50), Strings.getter("gateSizeNormalOpt"));
		internal static readonly AttributeOption SIZE_WIDE = new AttributeOption(Convert.ToInt32(70), Strings.getter("gateSizeWideOpt"));
		public static readonly Attribute<AttributeOption> ATTR_SIZE = Attributes.forOption("size", Strings.getter("gateSizeAttr"), new AttributeOption[] {SIZE_NARROW, SIZE_MEDIUM, SIZE_WIDE});

		public static readonly Attribute<int> ATTR_INPUTS = Attributes.forIntegerRange("inputs", Strings.getter("gateInputsAttr"), 2, MAX_INPUTS);

		internal static readonly AttributeOption XOR_ONE = new AttributeOption("1", Strings.getter("xorBehaviorOne"));
		internal static readonly AttributeOption XOR_ODD = new AttributeOption("odd", Strings.getter("xorBehaviorOdd"));
		public static readonly Attribute<AttributeOption> ATTR_XOR = Attributes.forOption("xor", Strings.getter("xorBehaviorAttr"), new AttributeOption[] {XOR_ONE, XOR_ODD});

		internal static readonly AttributeOption OUTPUT_01 = new AttributeOption("01", Strings.getter("gateOutput01"));
		internal static readonly AttributeOption OUTPUT_0Z = new AttributeOption("0Z", Strings.getter("gateOutput0Z"));
		internal static readonly AttributeOption OUTPUT_Z1 = new AttributeOption("Z1", Strings.getter("gateOutputZ1"));
		public static readonly Attribute<AttributeOption> ATTR_OUTPUT = Attributes.forOption("out", Strings.getter("gateOutputAttr"), new AttributeOption[] {OUTPUT_01, OUTPUT_0Z, OUTPUT_Z1});

		internal Direction facing = Direction.East;
		internal BitWidth width = BitWidth.ONE;
		internal AttributeOption size = SIZE_MEDIUM;
		internal int inputs = 5;
		internal int negated = 0;
		internal AttributeOption @out = OUTPUT_01;
		internal AttributeOption xorBehave;
		internal string label = "";
		internal Font labelFont = StdAttr.DEFAULT_LABEL_FONT;

		internal GateAttributes(bool isXor)
		{
			xorBehave = isXor ? XOR_ONE : null;
		}

		protected internal override void copyInto(AbstractAttributeSet dest)
		{
			; // nothing to do
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return new GateAttributeList(this);
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public override V getValue<V>(Attribute<V> attr)
		{
			if (attr == StdAttr.FACING)
			{
				return (V) facing;
			}
			if (attr == StdAttr.WIDTH)
			{
				return (V) width;
			}
			if (attr == StdAttr.LABEL)
			{
				return (V) label;
			}
			if (attr == StdAttr.LABEL_FONT)
			{
				return (V) labelFont;
			}
			if (attr == ATTR_SIZE)
			{
				return (V) size;
			}
			if (attr == ATTR_INPUTS)
			{
				return (V) Convert.ToInt32(inputs);
			}
			if (attr == ATTR_OUTPUT)
			{
				return (V) @out;
			}
			if (attr == ATTR_XOR)
			{
				return (V) xorBehave;
			}
			if (attr is NegateAttribute)
			{
				int index = ((NegateAttribute) attr).index;
				int bit = (negated >> index) & 1;
				return (V) Convert.ToBoolean(bit == 1);
			}
			return null;
		}

		public override void setValue<V>(Attribute<V> attr, V value)
		{
			if (attr == StdAttr.WIDTH)
			{
				width = (BitWidth) value;
				int bits = width.Width;
				int mask = bits >= 32 ? -1 : ((1 << inputs) - 1);
				negated &= mask;
			}
			else if (attr == StdAttr.FACING)
			{
				facing = (Direction) value;
			}
			else if (attr == StdAttr.LABEL)
			{
				label = (string) value;
			}
			else if (attr == StdAttr.LABEL_FONT)
			{
				labelFont = (Font) value;
			}
			else if (attr == ATTR_SIZE)
			{
				size = (AttributeOption) value;
			}
			else if (attr == ATTR_INPUTS)
			{
				inputs = ((int?) value).Value;
				fireAttributeListChanged();
			}
			else if (attr == ATTR_XOR)
			{
				xorBehave = (AttributeOption) value;
			}
			else if (attr == ATTR_OUTPUT)
			{
				@out = (AttributeOption) value;
			}
			else if (attr is NegateAttribute)
			{
				int index = ((NegateAttribute) attr).index;
				if (((bool?) value).Value)
				{
					negated |= 1 << index;
				}
				else
				{
					negated &= ~(1 << index);
				}
			}
			else
			{
				throw new System.ArgumentException("unrecognized argument");
			}
			fireAttributeValueChanged(attr, value);
		}
	}

}
