/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import java.awt.Cursor;
import java.util.List;

import javax.swing.Icon;

import draw.canvas.Canvas;
import draw.canvas.CanvasTool;
import logisim.data.Attribute;

public abstract class AbstractTool extends CanvasTool {
	public static AbstractTool[] getTools(DrawingAttributeSet attrs) {
		return new AbstractTool[] { new SelectTool(), new LineTool(attrs), new CurveTool(attrs),
				new PolyTool(false, attrs), new RectangleTool(attrs), new RoundRectangleTool(attrs),
				new OvalTool(attrs), new PolyTool(true, attrs), };
	}

	public abstract Icon getIcon();

	public abstract List<Attribute<?>> getAttributes();

	public String getDescription() {
		return null;
	}

	//
	// CanvasTool methods
	//
	@Override
	public abstract Cursor getCursor(Canvas canvas);

}
