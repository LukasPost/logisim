/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit.appear;

import java.util.Collections;
import java.util.List;
import java.util.Random;

import draw.model.CanvasObject;
import draw.model.AbstractCanvasObject;
import logisim.data.Attribute;
import logisim.data.Bounds;
import logisim.data.Location;

public abstract class AppearanceElement extends AbstractCanvasObject {
	private Location location;

	public AppearanceElement(Location location) {
		this.location = location;
	}

	public Location getLocation() {
		return location;
	}

	@Override
	public boolean matches(CanvasObject other) {
		if (other instanceof AppearanceElement) {
			AppearanceElement that = (AppearanceElement) other;
			return this.location.equals(that.location);
		} else {
			return false;
		}
	}

	@Override
	public int matchesHashCode() {
		return location.hashCode();
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return Collections.emptyList();
	}

	@Override
	public <V> V getValue(Attribute<V> attr) {
		return null;
	}

	@Override
	public boolean canRemove() {
		return false;
	}

	@Override
	protected void updateValue(Attribute<?> attr, Object value) {
		// nothing to do
	}

	@Override
	public void translate(int dx, int dy) {
		location = location.translate(dx, dy);
	}

	protected boolean isInCircle(Location loc, int radius) {
		int dx = loc.getX() - location.getX();
		int dy = loc.getY() - location.getY();
		return dx * dx + dy * dy < radius * radius;
	}

	@Override
	public Location getRandomPoint(Bounds bds, Random rand) {
		return null; // this is only used to determine what lies on top of what - but the elements will always be on top
						// anyway
	}

	protected Bounds getBounds(int radius) {
		return Bounds.create(location.getX() - radius, location.getY() - radius, 2 * radius, 2 * radius);
	}
}
