/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import java.awt.Graphics;
import java.util.List;

import javax.swing.Icon;

import draw.model.CanvasObject;
import draw.shapes.DrawAttr;
import draw.shapes.RoundRectangle;
import logisim.data.Attribute;
import logisim.util.Icons;

public class RoundRectangleTool extends RectangularTool {
	private DrawingAttributeSet attrs;

	public RoundRectangleTool(DrawingAttributeSet attrs) {
		this.attrs = attrs;
	}

	@Override
	public Icon getIcon() {
		return Icons.getIcon("drawrrct.gif");
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.getRoundRectAttributes(attrs.getValue(DrawAttr.PAINT_TYPE));
	}

	@Override
	public CanvasObject createShape(int x, int y, int w, int h) {
		return attrs.applyTo(new RoundRectangle(x, y, w, h));
	}

	@Override
	public void drawShape(Graphics g, int x, int y, int w, int h) {
		int r = 2 * attrs.getValue(DrawAttr.CORNER_RADIUS).intValue();
		g.drawRoundRect(x, y, w, h, r, r);
	}

	@Override
	public void fillShape(Graphics g, int x, int y, int w, int h) {
		int r = 2 * attrs.getValue(DrawAttr.CORNER_RADIUS).intValue();
		g.fillRoundRect(x, y, w, h, r, r);
	}
}
