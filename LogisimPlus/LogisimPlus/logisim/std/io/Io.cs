// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using Direction = logisim.data.Direction;
	using FactoryDescription = logisim.tools.FactoryDescription;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class Io : Library
	{
		internal static readonly AttributeOption LABEL_CENTER = new AttributeOption("center", "center", Strings.getter("ioLabelCenter"));

		internal static readonly Attribute ATTR_COLOR = Attributes.forColor("color", Strings.getter("ioColorAttr"));
		internal static readonly Attribute ATTR_ON_COLOR = Attributes.forColor("color", Strings.getter("ioOnColor"));
		internal static readonly Attribute ATTR_OFF_COLOR = Attributes.forColor("offcolor", Strings.getter("ioOffColor"));
		internal static readonly Attribute ATTR_BACKGROUND = Attributes.forColor("bg", Strings.getter("ioBackgroundColor"));
		internal static readonly Attribute ATTR_LABEL_LOC = Attributes.forOption("labelloc", Strings.getter("ioLabelLocAttr"), new object[] {LABEL_CENTER, Direction.North, Direction.South, Direction.East, Direction.West});
		internal static readonly Attribute ATTR_LABEL_COLOR = Attributes.forColor("labelcolor", Strings.getter("ioLabelColorAttr"));
		internal static readonly Attribute ATTR_ACTIVE = Attributes.forBoolean("active", Strings.getter("ioActiveAttr"));

		internal static readonly Color DEFAULT_BACKGROUND = Color.FromArgb(0, 255, 255, 255);

		private static FactoryDescription[] DESCRIPTIONS = new FactoryDescription[]
		{
			new FactoryDescription("Button", Strings.getter("buttonComponent"), "button.gif", "Button"),
			new FactoryDescription("Joystick", Strings.getter("joystickComponent"), "joystick.gif", "Joystick"),
			new FactoryDescription("Keyboard", Strings.getter("keyboardComponent"), "keyboard.gif", "Keyboard"),
			new FactoryDescription("LED", Strings.getter("ledComponent"), "led.gif", "Led"),
			new FactoryDescription("7-Segment Display", Strings.getter("sevenSegmentComponent"), "7seg.gif", "SevenSegment"),
			new FactoryDescription("Hex Digit Display", Strings.getter("hexDigitComponent"), "hexdig.gif", "HexDigit"),
			new FactoryDescription("DotMatrix", Strings.getter("dotMatrixComponent"), "dotmat.gif", "DotMatrix"),
			new FactoryDescription("TTY", Strings.getter("ttyComponent"), "tty.gif", "Tty")
		};

		private List<Tool> tools = null;

		public Io()
		{
		}

		public override string Name
		{
			get
			{
				return "I/O";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("ioLibrary");
			}
		}

		public override List<Tool> Tools
		{
			get
			{
				if (tools == null)
				{
					tools = FactoryDescription.getTools(typeof(Io), DESCRIPTIONS);
				}
				return tools;
			}
		}
	}

}
