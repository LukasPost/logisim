/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit.appear;

import java.awt.Color;
import java.awt.Graphics;
import java.util.List;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import draw.model.CanvasObject;
import draw.model.Handle;
import draw.model.HandleGesture;
import logisim.data.Attribute;
import logisim.data.Attributes;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.util.UnmodifiableList;

public class AppearanceAnchor extends AppearanceElement {
	public static final Attribute<Direction> FACING = Attributes.forDirection("facing",
			Strings.getter("appearanceFacingAttr"));
	static final List<Attribute<?>> ATTRIBUTES = UnmodifiableList.create(new Attribute<?>[] { FACING });

	private static final int RADIUS = 3;
	private static final int INDICATOR_LENGTH = 8;
	private static final Color SYMBOL_COLOR = new Color(0, 128, 0);

	private Direction facing;

	public AppearanceAnchor(Location location) {
		super(location);
		facing = Direction.East;
	}

	@Override
	public boolean matches(CanvasObject other) {
		return other instanceof AppearanceAnchor that && super.matches(that) && facing.equals(that.facing);
	}

	@Override
	public int matchesHashCode() {
		return super.matchesHashCode() * 31 + facing.hashCode();
	}

	@Override
	public String getDisplayName() {
		return Strings.get("circuitAnchor");
	}

	@Override
	public Element toSvgElement(Document doc) {
		Location loc = getLocation();
		Element ret = doc.createElement("circ-anchor");
		ret.setAttribute("x", "" + (loc.x() - RADIUS));
		ret.setAttribute("y", "" + (loc.y() - RADIUS));
		ret.setAttribute("width", "" + 2 * RADIUS);
		ret.setAttribute("height", "" + 2 * RADIUS);
		ret.setAttribute("facing", facing.toString());
		return ret;
	}

	public Direction getFacing() {
		return facing;
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return ATTRIBUTES;
	}

	@Override
	@SuppressWarnings("unchecked")
	public <V> V getValue(Attribute<V> attr) {
		return attr == FACING ? (V) facing : super.getValue(attr);
	}

	@Override
	protected void updateValue(Attribute<?> attr, Object value) {
		if (attr == FACING)
			facing = (Direction) value;
		else
			super.updateValue(attr, value);
	}

	@Override
	public void paint(Graphics g, HandleGesture gesture) {
		Location location = getLocation();
		int x = location.x();
		int y = location.y();
		g.setColor(SYMBOL_COLOR);
		g.drawOval(x - RADIUS, y - RADIUS, 2 * RADIUS, 2 * RADIUS);
		Location e0 = location.translate(facing, RADIUS);
		Location e1 = location.translate(facing, RADIUS + INDICATOR_LENGTH);
		g.drawLine(e0.x(), e0.y(), e1.x(), e1.y());
	}

	@Override
	public Bounds getBounds() {
		Bounds bds = getBounds(RADIUS);
		Location center = getLocation();
		Location end = center.translate(facing, RADIUS + INDICATOR_LENGTH);
		return bds.add(end);
	}

	@Override
	public boolean contains(Location loc, boolean assumeFilled) {
		if (isInCircle(loc, RADIUS))
			return true;
		Location center = getLocation();
		Location end = center.translate(facing, RADIUS + INDICATOR_LENGTH);
		if (facing == Direction.East || facing == Direction.West)
			return Math.abs(loc.y() - center.y()) < 2 && (loc.x() < center.x()) != (loc.x() < end.x());
		else
			return Math.abs(loc.x() - center.x()) < 2 && (loc.y() < center.y()) != (loc.y() < end.y());
	}

	@Override
	public List<Handle> getHandles(HandleGesture gesture) {
		Location c = getLocation();
		Location end = c.translate(facing, RADIUS + INDICATOR_LENGTH);
		return UnmodifiableList.create(new Handle[] { new Handle(this, c), new Handle(this, end) });
	}
}
