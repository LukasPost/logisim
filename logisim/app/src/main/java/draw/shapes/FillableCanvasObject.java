/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import java.awt.Color;

import draw.model.CanvasObject;
import draw.model.AbstractCanvasObject;
import logisim.data.Attribute;
import logisim.data.AttributeOption;

abstract class FillableCanvasObject extends AbstractCanvasObject {
	private AttributeOption paintType;
	private int strokeWidth;
	private Color strokeColor;
	private Color fillColor;

	public FillableCanvasObject() {
		paintType = DrawAttr.PAINT_STROKE;
		strokeWidth = 1;
		strokeColor = Color.BLACK;
		fillColor = Color.WHITE;
	}

	@Override
	public boolean matches(CanvasObject other) {
		if (!(other instanceof FillableCanvasObject that))
			return false;
		boolean ret = paintType == that.paintType;
		if (ret && paintType != DrawAttr.PAINT_FILL)
			ret = strokeWidth == that.strokeWidth && strokeColor.equals(that.strokeColor);
		if (ret && paintType != DrawAttr.PAINT_STROKE)
			return fillColor.equals(that.fillColor);
		return ret;
	}

	@Override
	public int matchesHashCode() {
		int ret = paintType.hashCode();
		if (paintType != DrawAttr.PAINT_FILL) {
			ret = ret * 31 + strokeWidth;
			ret = ret * 31 + strokeColor.hashCode();
		} else
			ret = ret * 31 * 31;
		return ret * 31 + (paintType != DrawAttr.PAINT_STROKE ? fillColor.hashCode() : 0);
	}

	public AttributeOption getPaintType() {
		return paintType;
	}

	public int getStrokeWidth() {
		return strokeWidth;
	}

	@Override
	@SuppressWarnings("unchecked")
	public <V> V getValue(Attribute<V> attr) {
		if (attr == DrawAttr.PAINT_TYPE)
			return (V) paintType;
		else if (attr == DrawAttr.STROKE_COLOR)
			return (V) strokeColor;
		else if (attr == DrawAttr.FILL_COLOR)
			return (V) fillColor;
		else if (attr == DrawAttr.STROKE_WIDTH)
			return (V) Integer.valueOf(strokeWidth);
		else
			return null;
	}

	@Override
	public void updateValue(Attribute<?> attr, Object value) {
		if (attr == DrawAttr.PAINT_TYPE) {
			paintType = (AttributeOption) value;
			fireAttributeListChanged();
		} else if (attr == DrawAttr.STROKE_COLOR)
			strokeColor = (Color) value;
		else if (attr == DrawAttr.FILL_COLOR)
			fillColor = (Color) value;
		else if (attr == DrawAttr.STROKE_WIDTH)
			strokeWidth = (Integer) value;
	}
}
