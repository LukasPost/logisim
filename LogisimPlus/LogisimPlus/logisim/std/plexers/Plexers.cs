// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.plexers
{

	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using FactoryDescription = logisim.tools.FactoryDescription;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class Plexers : Library
	{
		public static readonly Attribute<BitWidth> ATTR_SELECT = Attributes.forBitWidth("select", Strings.getter("plexerSelectBitsAttr"), 1, 5);
		public static readonly object DEFAULT_SELECT = BitWidth.create(1);

		public static readonly Attribute<bool> ATTR_TRISTATE = Attributes.forBoolean("tristate", Strings.getter("plexerThreeStateAttr"));
		public const object DEFAULT_TRISTATE = false;

		public static readonly AttributeOption DISABLED_FLOATING = new AttributeOption("Z", Strings.getter("plexerDisabledFloating"));
		public static readonly AttributeOption DISABLED_ZERO = new AttributeOption("0", Strings.getter("plexerDisabledZero"));
		public static readonly Attribute<AttributeOption> ATTR_DISABLED = Attributes.forOption("disabled", Strings.getter("plexerDisabledAttr"), new AttributeOption[] {DISABLED_FLOATING, DISABLED_ZERO});

		public static readonly Attribute<bool> ATTR_ENABLE = Attributes.forBoolean("enable", Strings.getter("plexerEnableAttr"));

		internal static readonly AttributeOption SELECT_BOTTOM_LEFT = new AttributeOption("bl", Strings.getter("plexerSelectBottomLeftOption"));
		internal static readonly AttributeOption SELECT_TOP_RIGHT = new AttributeOption("tr", Strings.getter("plexerSelectTopRightOption"));
		internal static readonly Attribute<AttributeOption> ATTR_SELECT_LOC = Attributes.forOption("selloc", Strings.getter("plexerSelectLocAttr"), new AttributeOption[] {SELECT_BOTTOM_LEFT, SELECT_TOP_RIGHT});

		protected internal const int DELAY = 3;

		private static FactoryDescription[] DESCRIPTIONS = new FactoryDescription[]
		{
			new FactoryDescription("Multiplexer", Strings.getter("multiplexerComponent"), "multiplexer.gif", "Multiplexer"),
			new FactoryDescription("Demultiplexer", Strings.getter("demultiplexerComponent"), "demultiplexer.gif", "Demultiplexer"),
			new FactoryDescription("Decoder", Strings.getter("decoderComponent"), "decoder.gif", "Decoder"),
			new FactoryDescription("Priority Encoder", Strings.getter("priorityEncoderComponent"), "priencod.gif", "PriorityEncoder"),
			new FactoryDescription("BitSelector", Strings.getter("bitSelectorComponent"), "bitSelector.gif", "BitSelector")
		};

		private IList<Tool> tools = null;

		public Plexers()
		{
		}

		public override string Name
		{
			get
			{
				return "Plexers";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("plexerLibrary");
			}
		}

		public override IList<Tool> Tools
		{
			get
			{
				if (tools == null)
				{
					tools = FactoryDescription.getTools(typeof(Plexers), DESCRIPTIONS);
				}
				return tools;
			}
		}

		internal static void drawTrapezoid(Graphics g, Bounds bds, Direction facing, int facingLean)
		{
			int wid = bds.Width;
			int ht = bds.Height;
			int x0 = bds.X;
			int x1 = x0 + wid;
			int y0 = bds.Y;
			int y1 = y0 + ht;
			int[] xp = new int[] {x0, x1, x1, x0};
			int[] yp = new int[] {y0, y0, y1, y1};
			if (facing == Direction.West)
			{
				yp[0] += facingLean;
				yp[3] -= facingLean;
			}
			else if (facing == Direction.North)
			{
				xp[0] += facingLean;
				xp[1] -= facingLean;
			}
			else if (facing == Direction.South)
			{
				xp[2] -= facingLean;
				xp[3] += facingLean;
			}
			else
			{
				yp[1] += facingLean;
				yp[2] -= facingLean;
			}
			GraphicsUtil.switchToWidth(g, 2);
			g.drawPolygon(xp, yp, 4);
		}

		internal static bool contains(Location loc, Bounds bds, Direction facing)
		{
			if (bds.contains(loc, 1))
			{
				int x = loc.X;
				int y = loc.Y;
				int x0 = bds.X;
				int x1 = x0 + bds.Width;
				int y0 = bds.Y;
				int y1 = y0 + bds.Height;
				if (facing == Direction.North || facing == Direction.South)
				{
					if (x < x0 + 5 || x > x1 - 5)
					{
						if (facing == Direction.South)
						{
							return y < y0 + 5;
						}
						else
						{
							return y > y1 - 5;
						}
					}
					else
					{
						return true;
					}
				}
				else
				{
					if (y < y0 + 5 || y > y1 - 5)
					{
						if (facing == Direction.East)
						{
							return x < x0 + 5;
						}
						else
						{
							return x > x1 - 5;
						}
					}
					else
					{
						return true;
					}
				}
			}
			else
			{
				return false;
			}
		}
	}

}
