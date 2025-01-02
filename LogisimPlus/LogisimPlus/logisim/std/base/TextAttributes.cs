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

	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Bounds = logisim.data.Bounds;
	using StdAttr = logisim.instance.StdAttr;

	internal class TextAttributes : AbstractAttributeSet
	{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRIBUTES = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { Text.ATTR_TEXT, Text.ATTR_FONT, Text.ATTR_HALIGN, Text.ATTR_VALIGN });
		private static readonly IList<Attribute<object>> ATTRIBUTES = new List<Attribute<object>> {Text.ATTR_TEXT, Text.ATTR_FONT, Text.ATTR_HALIGN, Text.ATTR_VALIGN};

		private string text;
		private Font font;
		private AttributeOption halign;
		private AttributeOption valign;
		private Bounds offsetBounds;

		public TextAttributes()
		{
			text = "";
			font = StdAttr.DEFAULT_LABEL_FONT;
			halign = logisim.std.@base.Text.ATTR_HALIGN.parse("center");
			valign = logisim.std.@base.Text.ATTR_VALIGN.parse("base");
			offsetBounds = null;
		}

		internal virtual string Text
		{
			get
			{
				return text;
			}
		}

		internal virtual Font Font
		{
			get
			{
				return font;
			}
		}

		internal virtual int HorizontalAlign
		{
			get
			{
				return ((int?) halign.Value).Value;
			}
		}

		internal virtual int VerticalAlign
		{
			get
			{
				return ((int?) valign.Value).Value;
			}
		}

		internal virtual Bounds OffsetBounds
		{
			get
			{
				return offsetBounds;
			}
		}

		internal virtual bool setOffsetBounds(Bounds value)
		{
			Bounds old = offsetBounds;
			bool same = old == null ? value == null : old.Equals(value);
			if (!same)
			{
				offsetBounds = value;
			}
			return !same;
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
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public override V getValue<V>(Attribute<V> attr)
		{
			if (attr == logisim.std.@base.Text.ATTR_TEXT)
			{
				return (V) text;
			}
			if (attr == logisim.std.@base.Text.ATTR_FONT)
			{
				return (V) font;
			}
			if (attr == logisim.std.@base.Text.ATTR_HALIGN)
			{
				return (V) halign;
			}
			if (attr == logisim.std.@base.Text.ATTR_VALIGN)
			{
				return (V) valign;
			}
			return null;
		}

		public override void setValue<V>(Attribute<V> attr, V value)
		{
			if (attr == logisim.std.@base.Text.ATTR_TEXT)
			{
				text = (string) value;
			}
			else if (attr == logisim.std.@base.Text.ATTR_FONT)
			{
				font = (Font) value;
			}
			else if (attr == logisim.std.@base.Text.ATTR_HALIGN)
			{
				halign = (AttributeOption) value;
			}
			else if (attr == logisim.std.@base.Text.ATTR_VALIGN)
			{
				valign = (AttributeOption) value;
			}
			else
			{
				throw new System.ArgumentException("unknown attribute");
			}
			offsetBounds = null;
			fireAttributeValueChanged(attr, value);
		}

	}

}
