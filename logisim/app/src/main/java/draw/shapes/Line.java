/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import java.awt.Color;
import java.awt.Graphics;
import java.util.List;
import java.util.Random;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import draw.model.CanvasObject;
import draw.model.AbstractCanvasObject;
import draw.model.Handle;
import draw.model.HandleGesture;
import logisim.data.Attribute;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.UnmodifiableList;

public class Line extends AbstractCanvasObject {
	static final int ON_LINE_THRESH = 2;

	private Location from;
	private Location to;
	private Bounds bounds;
	private int strokeWidth;
	private Color strokeColor;

	public Line(Location from, Location to) {
		this.from = from;
		this.to = to;
		bounds = Bounds.create(from, 0, 0).add(to);
		strokeWidth = 1;
		strokeColor = Color.BLACK;
	}

	@Override
	public boolean matches(CanvasObject other) {
		return other instanceof Line that
				&& from.equals(that.from)
				&& to.equals(that.to)
				&& strokeWidth == that.strokeWidth
				&& strokeColor.equals(that.strokeColor);
	}

	@Override
	public int matchesHashCode() {
		int ret = from.x() * 31 + from.y();
		ret = ret * 31 * 31 + to.x() * 31 + to.y();
		ret = ret * 31 + strokeWidth;
		ret = ret * 31 + strokeColor.hashCode();
		return ret;
	}

	@Override
	public Element toSvgElement(Document doc) {
		return SvgCreator.createLine(doc, this);
	}

	public Location getEnd0() {
		return from;
	}

	public Location getEnd1() {
		return to;
	}

	@Override
	public String getDisplayName() {
		return Strings.get("shapeLine");
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.ATTRS_STROKE;
	}

	@Override
	@SuppressWarnings("unchecked")
	public <V> V getValue(Attribute<V> attr) {
		if (attr == DrawAttr.STROKE_COLOR)
			return (V) strokeColor;
		else if (attr == DrawAttr.STROKE_WIDTH)
			return (V) Integer.valueOf(strokeWidth);
		else
			return null;
	}

	@Override
	public void updateValue(Attribute<?> attr, Object value) {
		if (attr == DrawAttr.STROKE_COLOR)
			strokeColor = (Color) value;
		else if (attr == DrawAttr.STROKE_WIDTH)
			strokeWidth = (Integer) value;
	}

	@Override
	public Bounds getBounds() {
		return bounds;
	}

	@Override
	public Location getRandomPoint(Bounds bds, Random rand) {
		double u = rand.nextDouble();
		int x = (int) Math.round(from.x() + u * (to.x() - from.x()));
		int y = (int) Math.round(from.y() + u * (to.y() - from.y()));
		if (strokeWidth > 1) {
			x += (rand.nextInt(strokeWidth) - strokeWidth / 2);
			y += (rand.nextInt(strokeWidth) - strokeWidth / 2);
		}
		return new Location(x, y);
	}

	@Override
	public boolean contains(Location loc, boolean assumeFilled) {
		int thresh = Math.max(ON_LINE_THRESH, strokeWidth / 2);
		return LineUtil.ptDistSqSegment(from, to, loc) < thresh * thresh;
	}

	@Override
	public void translate(Location distance) {
		from = from.add(distance);
		to = to.add(distance);
	}

	public List<Handle> getHandles() {
		return getHandles(null);
	}

	@Override
	public List<Handle> getHandles(HandleGesture gesture) {
		if (gesture == null)
			return UnmodifiableList.create(new Handle[]{new Handle(this, from), new Handle(this, to)});

		Handle h = gesture.getHandle();
		Location delta = gesture.getDelta();

		Handle[] ret = new Handle[2];
		ret[0] = new Handle(this, h.isAt(from) ? from.add(delta) : from);
		ret[1] = new Handle(this, h.isAt(to) ? to.add(delta) : to);
		return UnmodifiableList.create(ret);
	}

	@Override
	public boolean canMoveHandle(Handle handle) {
		return true;
	}

	@Override
	public Handle moveHandle(HandleGesture gesture) {
		Handle h = gesture.getHandle();
		Location delta = gesture.getDelta();
		int dx = gesture.getDeltaX();
		int dy = gesture.getDeltaY();
		Handle ret = null;
		if (h.isAt(from)) {
			from = from.add(delta);
			ret = new Handle(this, from);
		}
		if (h.isAt(to)) {
			to = to.add(delta);
			ret = new Handle(this, to);
		}
		bounds = Bounds.create(from, 0, 0).add(to);
		return ret;
	}

	@Override
	public void paint(Graphics g, HandleGesture gesture) {
		if (setForStroke(g)) {
			Location delta = gesture.getDelta();
			Location from = this.from;
			Location to = this.to;
			Handle h = gesture.getHandle();
			if (h.isAt(from))
				from = from.add(delta);
			if (h.isAt(to))
				to = to.add(delta);
			g.drawLine(from.x(), from.y(), to.x(), to.y());
		}
	}

}
