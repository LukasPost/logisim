/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.awt.Graphics;
import java.awt.Color;

import logisim.comp.Component;
import logisim.comp.AbstractComponentFactory;
import logisim.comp.ComponentDrawContext;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.GraphicsUtil;
import logisim.util.StringGetter;

class WireFactory extends AbstractComponentFactory {
	public static final WireFactory instance = new WireFactory();

	private WireFactory() {
	}

	@Override
	public String getName() {
		return "Wire";
	}

	@Override
	public StringGetter getDisplayGetter() {
		return Strings.getter("wireComponent");
	}

	@Override
	public AttributeSet createAttributeSet() {
		return Wire.create(new Location(0, 0), new Location(100, 0));
	}

	@Override
	public Component createComponent(Location loc, AttributeSet attrs) {
		Object dir = attrs.getValue(Wire.dir_attr);
		int len = attrs.getValue(Wire.len_attr);

		if (dir == Wire.VALUE_HORZ)
			return Wire.create(loc, loc.add(len, 0));
		else
			return Wire.create(loc, loc.add(0, len));
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Object dir = attrs.getValue(Wire.dir_attr);
		int len = attrs.getValue(Wire.len_attr);

		if (dir == Wire.VALUE_HORZ)
			return Bounds.create(0, -2, len, 5);
		else
			return Bounds.create(-2, 0, 5, len);
	}

	//
	// user interface methods
	//
	@Override
	public void drawGhost(ComponentDrawContext context, Color color, int x, int y, AttributeSet attrs) {
		Graphics g = context.getGraphics();
		Object dir = attrs.getValue(Wire.dir_attr);
		int len = attrs.getValue(Wire.len_attr);

		g.setColor(color);
		GraphicsUtil.switchToWidth(g, 3);
		if (dir == Wire.VALUE_HORZ) g.drawLine(x, y, x + len, y);
		else g.drawLine(x, y, x, y + len);
	}
}
