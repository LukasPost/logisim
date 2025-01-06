/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.plexers;

import java.awt.Graphics;
import java.util.List;

import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.Attributes;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.tools.FactoryDescription;
import logisim.tools.Library;
import logisim.tools.Tool;
import logisim.util.GraphicsUtil;

public class Plexers extends Library {
	public static final Attribute<BitWidth> ATTR_SELECT = Attributes.forBitWidth("select",
			Strings.getter("plexerSelectBitsAttr"), 1, 5);
	public static final Object DEFAULT_SELECT = BitWidth.create(1);

	public static final Attribute<Boolean> ATTR_TRISTATE = Attributes.forBoolean("tristate",
			Strings.getter("plexerThreeStateAttr"));
	public static final Object DEFAULT_TRISTATE = Boolean.FALSE;

	public static final AttributeOption DISABLED_FLOATING = new AttributeOption("Z",
			Strings.getter("plexerDisabledFloating"));
	public static final AttributeOption DISABLED_ZERO = new AttributeOption("0", Strings.getter("plexerDisabledZero"));
	public static final Attribute<AttributeOption> ATTR_DISABLED = Attributes.forOption("disabled",
			Strings.getter("plexerDisabledAttr"), new AttributeOption[] { DISABLED_FLOATING, DISABLED_ZERO });

	public static final Attribute<Boolean> ATTR_ENABLE = Attributes.forBoolean("enable",
			Strings.getter("plexerEnableAttr"));

	static final AttributeOption SELECT_BOTTOM_LEFT = new AttributeOption("bl",
			Strings.getter("plexerSelectBottomLeftOption"));
	static final AttributeOption SELECT_TOP_RIGHT = new AttributeOption("tr",
			Strings.getter("plexerSelectTopRightOption"));
	static final Attribute<AttributeOption> ATTR_SELECT_LOC = Attributes.forOption("selloc",
			Strings.getter("plexerSelectLocAttr"), new AttributeOption[] { SELECT_BOTTOM_LEFT, SELECT_TOP_RIGHT });

	protected static final int DELAY = 3;

	private static FactoryDescription[] DESCRIPTIONS = {
			new FactoryDescription("Multiplexer", Strings.getter("multiplexerComponent"), "multiplexer.gif",
					"Multiplexer"),
			new FactoryDescription("Demultiplexer", Strings.getter("demultiplexerComponent"), "demultiplexer.gif",
					"Demultiplexer"),
			new FactoryDescription("Decoder", Strings.getter("decoderComponent"), "decoder.gif", "Decoder"),
			new FactoryDescription("Priority Encoder", Strings.getter("priorityEncoderComponent"), "priencod.gif",
					"PriorityEncoder"),
			new FactoryDescription("BitSelector", Strings.getter("bitSelectorComponent"), "bitSelector.gif",
					"BitSelector"), };

	private List<Tool> tools;

	public Plexers() {
	}

	@Override
	public String getName() {
		return "Plexers";
	}

	@Override
	public String getDisplayName() {
		return Strings.get("plexerLibrary");
	}

	@Override
	public List<Tool> getTools() {
		if (tools == null) tools = FactoryDescription.getTools(Plexers.class, DESCRIPTIONS);
		return tools;
	}

	static void drawTrapezoid(Graphics g, Bounds bds, Direction facing, int facingLean) {
		int wid = bds.getWidth();
		int ht = bds.getHeight();
		int x0 = bds.getX();
		int x1 = x0 + wid;
		int y0 = bds.getY();
		int y1 = y0 + ht;
		int[] xp = { x0, x1, x1, x0 };
		int[] yp = { y0, y0, y1, y1 };
		if (facing == Direction.West) {
			yp[0] += facingLean;
			yp[3] -= facingLean;
		} else if (facing == Direction.North) {
			xp[0] += facingLean;
			xp[1] -= facingLean;
		} else if (facing == Direction.South) {
			xp[2] -= facingLean;
			xp[3] += facingLean;
		} else {
			yp[1] += facingLean;
			yp[2] -= facingLean;
		}
		GraphicsUtil.switchToWidth(g, 2);
		g.drawPolygon(xp, yp, 4);
	}

	static boolean contains(Location loc, Bounds bds, Direction facing) {
		if (bds.contains(loc, 1)) {
			int x = loc.x();
			int y = loc.y();
			int x0 = bds.getX();
			int x1 = x0 + bds.getWidth();
			int y0 = bds.getY();
			int y1 = y0 + bds.getHeight();
			if (facing == Direction.North || facing == Direction.South)
				if (x < x0 + 5 || x > x1 - 5) if (facing == Direction.South) return y < y0 + 5;
				else return y > y1 - 5;
				else return true;
			else if (y < y0 + 5 || y > y1 - 5) if (facing == Direction.East) return x < x0 + 5;
			else return x > x1 - 5;
			else return true;
		} else return false;
	}
}
