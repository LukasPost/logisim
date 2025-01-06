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
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.instance.Instance;
import logisim.std.wiring.Pin;
import logisim.util.UnmodifiableList;

public class AppearancePort extends AppearanceElement {
	private static final int INPUT_RADIUS = 4;
	private static final int OUTPUT_RADIUS = 5;
	private static final int MINOR_RADIUS = 2;
	public static final Color COLOR = Color.BLUE;

	private Instance pin;

	public AppearancePort(Location location, Instance pin) {
		super(location);
		this.pin = pin;
	}

	@Override
	public boolean matches(CanvasObject other) {
		return other instanceof AppearancePort that && matches(that) && pin == that.pin;
	}

	@Override
	public int matchesHashCode() {
		return super.matchesHashCode() + pin.hashCode();
	}

	@Override
	public String getDisplayName() {
		return Strings.get("circuitPort");
	}

	@Override
	public Element toSvgElement(Document doc) {
		Location loc = getLocation();
		Location pinLoc = pin.getLocation();
		Element ret = doc.createElement("circ-port");
		int r = isInput() ? INPUT_RADIUS : OUTPUT_RADIUS;
		ret.setAttribute("x", "" + (loc.x() - r));
		ret.setAttribute("y", "" + (loc.y() - r));
		ret.setAttribute("width", "" + 2 * r);
		ret.setAttribute("height", "" + 2 * r);
		ret.setAttribute("pin", pinLoc.x() + "," + pinLoc.y());
		return ret;
	}

	public Instance getPin() {
		return pin;
	}

	void setPin(Instance value) {
		pin = value;
	}

	private boolean isInput() {
		Instance p = pin;
		return p == null || Pin.FACTORY.isInputPin(p);
	}

	@Override
	public Bounds getBounds() {
		int r = isInput() ? INPUT_RADIUS : OUTPUT_RADIUS;
		return getBounds(r);
	}

	@Override
	public boolean contains(Location loc, boolean assumeFilled) {
		if (isInput()) return getBounds().contains(loc);
		else return isInCircle(loc, OUTPUT_RADIUS);
	}

	@Override
	public List<Handle> getHandles(HandleGesture gesture) {
		Location loc = getLocation();

		int r = isInput() ? INPUT_RADIUS : OUTPUT_RADIUS;
		return UnmodifiableList
				.create(new Handle[] { new Handle(this, loc.translate(-r, -r)), new Handle(this, loc.translate(r, -r)),
						new Handle(this, loc.translate(r, r)), new Handle(this, loc.translate(-r, r)) });
	}

	@Override
	public void paint(Graphics g, HandleGesture gesture) {
		Location location = getLocation();
		int x = location.x();
		int y = location.y();
		g.setColor(COLOR);
		if (isInput()) {
			int r = INPUT_RADIUS;
			g.drawRect(x - r, y - r, 2 * r, 2 * r);
		} else {
			int r = OUTPUT_RADIUS;
			g.drawOval(x - r, y - r, 2 * r, 2 * r);
		}
		g.fillOval(x - MINOR_RADIUS, y - MINOR_RADIUS, 2 * MINOR_RADIUS, 2 * MINOR_RADIUS);
	}
}
