/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import java.awt.Color;
import java.awt.Font;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.w3c.dom.Element;

import draw.model.AbstractCanvasObject;
import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.Location;
import logisim.util.UnmodifiableList;

public class SvgReader {
	private SvgReader() {
	}

	private static final Pattern PATH_REGEX = Pattern.compile("[a-zA-Z]|[-0-9.]+");

	public static AbstractCanvasObject createShape(Element elt) {
		String name = elt.getTagName();
		AbstractCanvasObject ret;
		switch (name) {
			case "ellipse" -> ret = createOval(elt);
			case "line" -> ret = createLine(elt);
			case "path" -> ret = createPath(elt);
			case "polyline" -> ret = createPolyline(elt);
			case "polygon" -> ret = createPolygon(elt);
			case "rect" -> ret = createRectangle(elt);
			case "text" -> ret = createText(elt);
			case null, default -> {
				return null;
			}
		}
		List<Attribute<?>> attrs = ret.getAttributes();
		if (attrs.contains(DrawAttr.PAINT_TYPE)) {
			String stroke = elt.getAttribute("stroke");
			String fill = elt.getAttribute("fill");
			if (stroke.isEmpty() || "none".equals(stroke))
				ret.setValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_FILL);
			else if ("none".equals(fill))
				ret.setValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_STROKE);
			else
				ret.setValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_STROKE_FILL);
		}
		attrs = ret.getAttributes(); // since changing paintType could change it
		if (attrs.contains(DrawAttr.STROKE_WIDTH) && elt.hasAttribute("stroke-width")) {
			Integer width = Integer.valueOf(elt.getAttribute("stroke-width"));
			ret.setValue(DrawAttr.STROKE_WIDTH, width);
		}
		if (attrs.contains(DrawAttr.STROKE_COLOR)) {
			String color = elt.getAttribute("stroke");
			String opacity = elt.getAttribute("stroke-opacity");
			if (!"none".equals(color))
				ret.setValue(DrawAttr.STROKE_COLOR, getColor(color, opacity));
		}
		if (attrs.contains(DrawAttr.FILL_COLOR)) {
			String color = elt.getAttribute("fill");
			if (color.isEmpty())
				color = "#000000";
			String opacity = elt.getAttribute("fill-opacity");
			if (!"none".equals(color))
				ret.setValue(DrawAttr.FILL_COLOR, getColor(color, opacity));
		}
		return ret;
	}

	private static AbstractCanvasObject createRectangle(Element elt) {
		int x = Integer.parseInt(elt.getAttribute("x"));
		int y = Integer.parseInt(elt.getAttribute("y"));
		int w = Integer.parseInt(elt.getAttribute("width"));
		int h = Integer.parseInt(elt.getAttribute("height"));
		if (!elt.hasAttribute("rx"))
			return new Rectangle(x, y, w, h);
		AbstractCanvasObject ret = new RoundRectangle(x, y, w, h);
		int rx = Integer.parseInt(elt.getAttribute("rx"));
		ret.setValue(DrawAttr.CORNER_RADIUS, rx);
		return ret;
	}

	private static AbstractCanvasObject createOval(Element elt) {
		double cx = Double.parseDouble(elt.getAttribute("cx"));
		double cy = Double.parseDouble(elt.getAttribute("cy"));
		double rx = Double.parseDouble(elt.getAttribute("rx"));
		double ry = Double.parseDouble(elt.getAttribute("ry"));
		int x = (int) Math.round(cx - rx);
		int y = (int) Math.round(cy - ry);
		int w = (int) Math.round(rx * 2);
		int h = (int) Math.round(ry * 2);
		return new Oval(x, y, w, h);
	}

	private static AbstractCanvasObject createLine(Element elt) {
		int x0 = Integer.parseInt(elt.getAttribute("x1"));
		int y0 = Integer.parseInt(elt.getAttribute("y1"));
		int x1 = Integer.parseInt(elt.getAttribute("x2"));
		int y1 = Integer.parseInt(elt.getAttribute("y2"));
		return new Line(new Location(x0, y0), new Location(x1, y1));
	}

	private static AbstractCanvasObject createPolygon(Element elt) {
		return new Poly(true, parsePoints(elt.getAttribute("points")));
	}

	private static AbstractCanvasObject createPolyline(Element elt) {
		return new Poly(false, parsePoints(elt.getAttribute("points")));
	}

	private static AbstractCanvasObject createText(Element elt) {
		int x = Integer.parseInt(elt.getAttribute("x"));
		int y = Integer.parseInt(elt.getAttribute("y"));
		String text = elt.getTextContent();
		Text ret = new Text(new Location(x, y), text);

		String fontFamily = elt.getAttribute("font-family");
		String fontStyle = elt.getAttribute("font-style");
		String fontWeight = elt.getAttribute("font-weight");
		String fontSize = elt.getAttribute("font-size");
		int styleFlags = 0;
		if ("italic".equals(fontStyle))
			styleFlags |= Font.ITALIC;
		if ("bold".equals(fontWeight))
			styleFlags |= Font.BOLD;
		int size = Integer.parseInt(fontSize);
		ret.setValue(DrawAttr.FONT, new Font(fontFamily, styleFlags, size));

		String alignStr = elt.getAttribute("text-anchor");
		AttributeOption halign;
		if ("start".equals(alignStr))
			halign = DrawAttr.ALIGN_LEFT;
		else if ("end".equals(alignStr))
			halign = DrawAttr.ALIGN_RIGHT;
		else
			halign = DrawAttr.ALIGN_CENTER;
		ret.setValue(DrawAttr.ALIGNMENT, halign);
		// fill color is handled after we return
		return ret;
	}

	private static List<Location> parsePoints(String points) {
		Pattern patten = Pattern.compile("[ ,\n\r\t]+");
		String[] tokens = patten.split(points);
		Location[] ret = new Location[tokens.length / 2];
		for (int i = 0; i < ret.length; i++) {
			int x = Integer.parseInt(tokens[2 * i]);
			int y = Integer.parseInt(tokens[2 * i + 1]);
			ret[i] = new Location(x, y);
		}
		return UnmodifiableList.create(ret);
	}

	private static AbstractCanvasObject createPath(Element elt) {
		Matcher pattern = PATH_REGEX.matcher(elt.getAttribute("d"));
		List<String> tokens = new ArrayList<>();
		int type = -1; // -1 error, 0 start, 1 curve, 2 polyline
		while (pattern.find()) {
			String token = pattern.group();
			tokens.add(token);
			if (Character.isLetter(token.charAt(0))) {
				type = switch (token.charAt(0)) {
					case 'M' -> type == -1 ? 0 : -1;
					case 'Q', 'q' -> type == 0 ? 1 : -1;
					/*
					 * not supported case 'L': case 'l': case 'H': case 'h': case 'V': case 'v': if (type == 0 || type == 2)
					 * type = 2; else type = -1; break;
					 */
					default -> -1;
				};
				if (type == -1)
					throw new NumberFormatException("Unrecognized path command '" + token.charAt(0) + "'");
			}
		}

		if (type != 1)
			throw new NumberFormatException("Unrecognized path");
		if (tokens.size() != 8 || !"M".equals(tokens.get(0)) || !"Q".equalsIgnoreCase(tokens.get(3)))
			throw new NumberFormatException("Unexpected format for curve");

		int x0 = Integer.parseInt(tokens.get(1));
		int y0 = Integer.parseInt(tokens.get(2));
		int x1 = Integer.parseInt(tokens.get(4));
		int y1 = Integer.parseInt(tokens.get(5));
		int x2 = Integer.parseInt(tokens.get(6));
		int y2 = Integer.parseInt(tokens.get(7));
		if ("q".equals(tokens.get(3))) {
			x1 += x0;
			y1 += y0;
			x2 += x0;
			y2 += y0;
		}
		return new Curve(new Location(x0, y0), new Location(x2, y2), new Location(x1, y1));
	}

	private static Color getColor(String hue, String opacity) {
		int r;
		int g;
		int b;
		if (hue == null || hue.isEmpty()) {
			r = 0;
			g = 0;
			b = 0;
		} else {
			r = Integer.parseInt(hue.substring(1, 3), 16);
			g = Integer.parseInt(hue.substring(3, 5), 16);
			b = Integer.parseInt(hue.substring(5, 7), 16);
		}
		int a = opacity == null || opacity.isEmpty() ? 255 : (int) Math.round(Double.parseDouble(opacity) * 255);
		return new Color(r, g, b, a);
	}
}
