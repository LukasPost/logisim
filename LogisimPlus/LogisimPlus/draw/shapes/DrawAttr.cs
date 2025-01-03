// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.shapes
{

	using EditableLabel = draw.util.EditableLabel;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using logisim.util;

	public class DrawAttr
	{
		public static readonly Font DEFAULT_FONT = new Font("SansSerif", Font.PLAIN, 12);

		public static readonly AttributeOption ALIGN_LEFT = new AttributeOption(Convert.ToInt32(EditableLabel.LEFT), Strings.getter("alignStart"));
		public static readonly AttributeOption ALIGN_CENTER = new AttributeOption(Convert.ToInt32(EditableLabel.CENTER), Strings.getter("alignMiddle"));
		public static readonly AttributeOption ALIGN_RIGHT = new AttributeOption(Convert.ToInt32(EditableLabel.RIGHT), Strings.getter("alignEnd"));

		public static readonly AttributeOption PAINT_STROKE = new AttributeOption("stroke", Strings.getter("paintStroke"));
		public static readonly AttributeOption PAINT_FILL = new AttributeOption("fill", Strings.getter("paintFill"));
		public static readonly AttributeOption PAINT_STROKE_FILL = new AttributeOption("both", Strings.getter("paintBoth"));

		public static readonly Attribute FONT = Attributes.forFont("font", Strings.getter("attrFont"));
		public static readonly Attribute ALIGNMENT = Attributes.forOption("align", Strings.getter("attrAlign"), new AttributeOption[] {ALIGN_LEFT, ALIGN_CENTER, ALIGN_RIGHT});
		public static readonly Attribute PAINT_TYPE = Attributes.forOption("paintType", Strings.getter("attrPaint"), new AttributeOption[] {PAINT_STROKE, PAINT_FILL, PAINT_STROKE_FILL});
		public static readonly Attribute STROKE_WIDTH = Attributes.forIntegerRange("stroke-width", Strings.getter("attrStrokeWidth"), 1, 8);
		public static readonly Attribute STROKE_COLOR = Attributes.forColor("stroke", Strings.getter("attrStroke"));
		public static readonly Attribute FILL_COLOR = Attributes.forColor("fill", Strings.getter("attrFill"));
		public static readonly Attribute TEXT_DEFAULT_FILL = Attributes.forColor("fill", Strings.getter("attrFill"));
		public static readonly Attribute CORNER_RADIUS = Attributes.forIntegerRange("rx", Strings.getter("attrRx"), 1, 1000);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public static final java.util.List<logisim.data.Attribute<?>> ATTRS_TEXT = createAttributes(new logisim.data.Attribute[] { FONT, ALIGNMENT, FILL_COLOR });
		public static readonly List<Attribute> ATTRS_TEXT = createAttributes(new Attribute[] {FONT, ALIGNMENT, FILL_COLOR});
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public static final java.util.List<logisim.data.Attribute<?>> ATTRS_TEXT_TOOL = createAttributes(new logisim.data.Attribute[] { FONT, ALIGNMENT, TEXT_DEFAULT_FILL });
		public static readonly List<Attribute> ATTRS_TEXT_TOOL = createAttributes(new Attribute[] {FONT, ALIGNMENT, TEXT_DEFAULT_FILL});
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public static final java.util.List<logisim.data.Attribute<?>> ATTRS_STROKE = createAttributes(new logisim.data.Attribute[] { STROKE_WIDTH, STROKE_COLOR });
		public static readonly List<Attribute> ATTRS_STROKE = createAttributes(new Attribute[] {STROKE_WIDTH, STROKE_COLOR});

		// attribute lists for rectangle, oval, polygon
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRS_FILL_STROKE = createAttributes(new logisim.data.Attribute[] { PAINT_TYPE, STROKE_WIDTH, STROKE_COLOR });
		private static readonly List<Attribute> ATTRS_FILL_STROKE = createAttributes(new Attribute[] {PAINT_TYPE, STROKE_WIDTH, STROKE_COLOR});
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRS_FILL_FILL = createAttributes(new logisim.data.Attribute[] { PAINT_TYPE, FILL_COLOR });
		private static readonly List<Attribute> ATTRS_FILL_FILL = createAttributes(new Attribute[] {PAINT_TYPE, FILL_COLOR});
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRS_FILL_BOTH = createAttributes(new logisim.data.Attribute[] { PAINT_TYPE, STROKE_WIDTH, STROKE_COLOR, FILL_COLOR });
		private static readonly List<Attribute> ATTRS_FILL_BOTH = createAttributes(new Attribute[] {PAINT_TYPE, STROKE_WIDTH, STROKE_COLOR, FILL_COLOR});

		// attribute lists for rounded rectangle
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRS_RRECT_STROKE = createAttributes(new logisim.data.Attribute[] { PAINT_TYPE, STROKE_WIDTH, STROKE_COLOR, CORNER_RADIUS });
		private static readonly List<Attribute> ATTRS_RRECT_STROKE = createAttributes(new Attribute[] {PAINT_TYPE, STROKE_WIDTH, STROKE_COLOR, CORNER_RADIUS});
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRS_RRECT_FILL = createAttributes(new logisim.data.Attribute[] { PAINT_TYPE, FILL_COLOR, CORNER_RADIUS });
		private static readonly List<Attribute> ATTRS_RRECT_FILL = createAttributes(new Attribute[] {PAINT_TYPE, FILL_COLOR, CORNER_RADIUS});
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRS_RRECT_BOTH = createAttributes(new logisim.data.Attribute[] { PAINT_TYPE, STROKE_WIDTH, STROKE_COLOR, FILL_COLOR, CORNER_RADIUS });
		private static readonly List<Attribute> ATTRS_RRECT_BOTH = createAttributes(new Attribute[] {PAINT_TYPE, STROKE_WIDTH, STROKE_COLOR, FILL_COLOR, CORNER_RADIUS});

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static java.util.List<logisim.data.Attribute<?>> createAttributes(logisim.data.Attribute<?>[] values)
		private static List<Attribute> createAttributes(Attribute[] values)
		{
			return values.ToArray();
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public static java.util.List<logisim.data.Attribute<?>> getFillAttributes(logisim.data.AttributeOption paint)
		public static List<Attribute> getFillAttributes(AttributeOption paint)
		{
			if (paint == PAINT_STROKE)
			{
				return ATTRS_FILL_STROKE;
			}
			else if (paint == PAINT_FILL)
			{
				return ATTRS_FILL_FILL;
			}
			else
			{
				return ATTRS_FILL_BOTH;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public static java.util.List<logisim.data.Attribute<?>> getRoundRectAttributes(logisim.data.AttributeOption paint)
		public static List<Attribute> getRoundRectAttributes(AttributeOption paint)
		{
			if (paint == PAINT_STROKE)
			{
				return ATTRS_RRECT_STROKE;
			}
			else if (paint == PAINT_FILL)
			{
				return ATTRS_RRECT_FILL;
			}
			else
			{
				return ATTRS_RRECT_BOTH;
			}
		}
	}

}
