/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit.appear;

import java.util.Map;

import org.w3c.dom.Element;

import draw.model.AbstractCanvasObject;
import draw.shapes.SvgReader;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.instance.Instance;

public class AppearanceSvgReader {
	public static AbstractCanvasObject createShape(Element elt, Map<Location, Instance> pins) {
		String name = elt.getTagName();
		if ("circ-anchor".equals(name) || "circ-origin".equals(name)) {
			Location loc = getLocation(elt);
			AbstractCanvasObject ret = new AppearanceAnchor(loc);
			if (elt.hasAttribute("facing")) {
				Direction facing = Direction.parse(elt.getAttribute("facing"));
				ret.setValue(AppearanceAnchor.FACING, facing);
			}
			return ret;
		} else if ("circ-port".equals(name)) {
			Location loc = getLocation(elt);
			String[] pinStr = elt.getAttribute("pin").split(",");
			Location pinLoc = new Location(Integer.parseInt(pinStr[0].trim()), Integer.parseInt(pinStr[1].trim()));
			Instance pin = pins.get(pinLoc);
			return pin == null ? null : new AppearancePort(loc, pin);
		} else
			return SvgReader.createShape(elt);
	}

	private static Location getLocation(Element elt) {
		double x = Double.parseDouble(elt.getAttribute("x"));
		double y = Double.parseDouble(elt.getAttribute("y"));
		double w = Double.parseDouble(elt.getAttribute("width"));
		double h = Double.parseDouble(elt.getAttribute("height"));
		int px = (int) Math.round(x + w / 2);
		int py = (int) Math.round(y + h / 2);
		return new Location(px, py);
	}
}
