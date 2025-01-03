// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using EndData = logisim.comp.EndData;
	using logisim.data;
	using BitWidth = logisim.data.BitWidth;
	using StdAttr = logisim.instance.StdAttr;

	internal class PinAttributes : ProbeAttributes
	{
		public static new PinAttributes instance = new PinAttributes();

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRIBUTES = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { logisim.instance.StdAttr.FACING, Pin.ATTR_TYPE, logisim.instance.StdAttr.Width, Pin.ATTR_TRISTATE, Pin.ATTR_PULL, logisim.instance.StdAttr.LABEL, Pin.ATTR_LABEL_LOC, logisim.instance.StdAttr.LABEL_FONT });
		private static readonly List<Attribute> ATTRIBUTES = new List<Attribute> {StdAttr.FACING, Pin.ATTR_TYPE, StdAttr.Width, Pin.ATTR_TRISTATE, Pin.ATTR_PULL, StdAttr.LABEL, Pin.ATTR_LABEL_LOC, StdAttr.LABEL_FONT};

		internal new BitWidth width = BitWidth.ONE;
		internal bool threeState = true;
		internal int type = EndData.INPUT_ONLY;
		internal object pull = Pin.PULL_NONE;

		public PinAttributes()
		{
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return ATTRIBUTES;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public override object getValue(Attribute attr)
		{
			if (attr == StdAttr.Width)
			{
				return width;
			}
			if (attr == Pin.ATTR_TRISTATE)
			{
				return Convert.ToBoolean(threeState);
			}
			if (attr == Pin.ATTR_TYPE)
			{
				return Convert.ToBoolean(type == EndData.OUTPUT_ONLY);
			}
			if (attr == Pin.ATTR_PULL)
			{
				return pull;
			}
			return base.getValue(attr);
		}

		internal virtual bool Output
		{
			get
			{
				return type != EndData.INPUT_ONLY;
			}
		}

		internal virtual bool Input
		{
			get
			{
				return type != EndData.OUTPUT_ONLY;
			}
		}

		public override void setValue(Attribute attr, object value)
		{
			if (attr == StdAttr.Width)
			{
				width = (BitWidth) value;
			}
			else if (attr == Pin.ATTR_TRISTATE)
			{
				threeState = ((bool?) value).Value;
			}
			else if (attr == Pin.ATTR_TYPE)
			{
				type = ((bool?) value).Value ? EndData.OUTPUT_ONLY : EndData.INPUT_ONLY;
			}
			else if (attr == Pin.ATTR_PULL)
			{
				pull = value;
			}
			else
			{
				base.setValue(attr, value);
				return;
			}
			fireAttributeValueChanged(attr, value);
		}
	}

}
