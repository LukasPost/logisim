/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import java.awt.Graphics;
import java.util.List;
import java.util.Random;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import draw.model.CanvasObject;
import logisim.data.Attribute;
import logisim.data.Bounds;
import logisim.data.Location;

public class Oval extends Rectangular {
	public Oval(int x, int y, int w, int h) {
		super(x, y, w, h);
	}

	@Override
	public boolean matches(CanvasObject other) {
		return other instanceof Oval && super.matches(other);
	}

	@Override
	public int matchesHashCode() {
		return super.matchesHashCode();
	}

	@Override
	public Element toSvgElement(Document doc) {
		return SvgCreator.createOval(doc, this);
	}

	@Override
	public String getDisplayName() {
		return Strings.get("shapeOval");
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.getFillAttributes(getPaintType());
	}

	@Override
	protected boolean contains(int x, int y, int w, int h, Location q) {
		int qx = q.x();
		int qy = q.y();
		double dx = qx - (x + 0.5 * w);
		double dy = qy - (y + 0.5 * h);
		double sum = (dx * dx) / (w * w) + (dy * dy) / (h * h);
		return sum <= 0.25;
	}

	@Override
	protected Location getRandomPoint(Bounds bds, Random rand) {
		if (getPaintType() == DrawAttr.PAINT_STROKE) {
			double rx = getWidth() / 2.0;
			double ry = getHeight() / 2.0;
			double u = 2 * Math.PI * rand.nextDouble();
			int x = (int) Math.round(getX() + rx + rx * Math.cos(u));
			int y = (int) Math.round(getY() + ry + ry * Math.sin(u));
			int d = getStrokeWidth();
			if (d > 1) {
				x += rand.nextInt(d) - d / 2;
				y += rand.nextInt(d) - d / 2;
			}
			return new Location(x, y);
		} else return super.getRandomPoint(bds, rand);
	}

	@Override
	public void draw(Graphics g, int x, int y, int w, int h) {
		if (setForFill(g))
			g.fillOval(x, y, w, h);
		if (setForStroke(g))
			g.drawOval(x, y, w, h);
	}
}
