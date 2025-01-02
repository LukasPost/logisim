// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using RadixOption = logisim.circuit.RadixOption;
	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using BitWidth = logisim.data.BitWidth;
	using Direction = logisim.data.Direction;
	using StdAttr = logisim.instance.StdAttr;

	internal class ProbeAttributes : AbstractAttributeSet
	{
		public static ProbeAttributes instance = new ProbeAttributes();

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRIBUTES = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { logisim.instance.StdAttr.FACING, logisim.circuit.RadixOption.ATTRIBUTE, logisim.instance.StdAttr.LABEL, Pin.ATTR_LABEL_LOC, logisim.instance.StdAttr.LABEL_FONT});
		private static readonly IList<Attribute<object>> ATTRIBUTES = new List<Attribute<object>> {StdAttr.FACING, RadixOption.ATTRIBUTE, StdAttr.LABEL, Pin.ATTR_LABEL_LOC, StdAttr.LABEL_FONT};

		internal Direction facing = Direction.East;
		internal string label = "";
		internal Direction labelloc = Direction.West;
		internal Font labelfont = StdAttr.DEFAULT_LABEL_FONT;
		internal RadixOption radix = RadixOption.RADIX_2;
		internal BitWidth width = BitWidth.ONE;

		public ProbeAttributes()
		{
		}

		protected internal override void copyInto(AbstractAttributeSet destObj)
		{
			; // nothing to do
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return ATTRIBUTES;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <E> E getValue(logisim.data.Attribute<E> attr)
		public override E getValue<E>(Attribute<E> attr)
		{
			if (attr == StdAttr.FACING)
			{
				return (E) facing;
			}
			if (attr == StdAttr.LABEL)
			{
				return (E) label;
			}
			if (attr == Pin.ATTR_LABEL_LOC)
			{
				return (E) labelloc;
			}
			if (attr == StdAttr.LABEL_FONT)
			{
				return (E) labelfont;
			}
			if (attr == RadixOption.ATTRIBUTE)
			{
				return (E) radix;
			}
			return null;
		}

		public override void setValue<V>(Attribute<V> attr, V value)
		{
			if (attr == StdAttr.FACING)
			{
				facing = (Direction) value;
			}
			else if (attr == StdAttr.LABEL)
			{
				label = (string) value;
			}
			else if (attr == Pin.ATTR_LABEL_LOC)
			{
				labelloc = (Direction) value;
			}
			else if (attr == StdAttr.LABEL_FONT)
			{
				labelfont = (Font) value;
			}
			else if (attr == RadixOption.ATTRIBUTE)
			{
				radix = (RadixOption) value;
			}
			else
			{
				throw new System.ArgumentException("unknown attribute");
			}
			fireAttributeValueChanged(attr, value);
		}
	}

}
