// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{

	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Direction = logisim.data.Direction;

	public interface StdAttr
	{
		public static Attribute FACING = Attributes.forDirection("facing", Strings.getter("stdFacingAttr"));

		public static Attribute WIDTH = Attributes.forBitWidth("width", Strings.getter("stdDataWidthAttr"));

		public static AttributeOption TRIG_RISING = new AttributeOption("rising", Strings.getter("stdTriggerRising"));
		public static AttributeOption TRIG_FALLING = new AttributeOption("falling", Strings.getter("stdTriggerFalling"));
		public static AttributeOption TRIG_HIGH = new AttributeOption("high", Strings.getter("stdTriggerHigh"));
		public static AttributeOption TRIG_LOW = new AttributeOption("low", Strings.getter("stdTriggerLow"));
		public static Attribute TRIGGER = Attributes.forOption("trigger", Strings.getter("stdTriggerAttr"), new AttributeOption[] {TRIG_RISING, TRIG_FALLING, TRIG_HIGH, TRIG_LOW});
		public static Attribute EDGE_TRIGGER = Attributes.forOption("trigger", Strings.getter("stdTriggerAttr"), new AttributeOption[] {TRIG_RISING, TRIG_FALLING});

		public static Attribute LABEL = Attributes.forString("label", Strings.getter("stdLabelAttr"));

		public static Attribute LABEL_FONT = Attributes.forFont("labelfont", Strings.getter("stdLabelFontAttr"));
		public static Font DEFAULT_LABEL_FONT = new Font("SansSerif", 12);
	}

}
