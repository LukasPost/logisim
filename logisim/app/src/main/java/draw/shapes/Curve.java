/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.geom.QuadCurve2D;
import java.util.List;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import draw.model.CanvasObject;
import draw.model.Handle;
import draw.model.HandleGesture;
import logisim.data.Attribute;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.UnmodifiableList;

import static logisim.data.Location.toArray;

public class Curve extends FillableCanvasObject {
	private Location p0;
	private Location p1;
	private Location p2;
	private Bounds bounds;

	public Curve(Location end0, Location end1, Location ctrl) {
		p0 = end0;
		p1 = ctrl;
		p2 = end1;
		bounds = CurveUtil.getBounds(toArray(p0), toArray(p1), toArray(p2));
	}

	@Override
	public boolean matches(CanvasObject other) {
		return other instanceof Curve that && p0.equals(that.p0) && p1.equals(that.p1) && p2.equals(that.p2) && super.matches(that);
	}

	@Override
	public int matchesHashCode() {
		int ret = p0.hashCode();
		ret = ret * 31 * 31 + p1.hashCode();
		ret = ret * 31 * 31 + p2.hashCode();
		ret = ret * 31 + super.matchesHashCode();
		return ret;
	}

	@Override
	public Element toSvgElement(Document doc) {
		return SvgCreator.createCurve(doc, this);
	}

	public Location getEnd0() {
		return p0;
	}

	public Location getEnd1() {
		return p2;
	}

	public Location getControl() {
		return p1;
	}

	public QuadCurve2D getCurve2D() {
		return new QuadCurve2D.Double(p0.x(), p0.y(), p1.x(), p1.y(), p2.x(), p2.y());
	}

	@Override
	public String getDisplayName() {
		return Strings.get("shapeCurve");
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.getFillAttributes(getPaintType());
	}

	@Override
	public Bounds getBounds() {
		return bounds;
	}

	@Override
	public boolean contains(Location loc, boolean assumeFilled) {
		Object type = getPaintType();
		if (assumeFilled && type == DrawAttr.PAINT_STROKE)
			type = DrawAttr.PAINT_STROKE_FILL;
		if (type != DrawAttr.PAINT_FILL) {
			int stroke = getStrokeWidth();
			double[] q = toArray(loc);
			double[] p0 = toArray(this.p0);
			double[] p1 = toArray(this.p1);
			double[] p2 = toArray(this.p2);
			double[] p = CurveUtil.findNearestPoint(q, p0, p1, p2);
			if (p == null)
				return false;

			int thr = type == DrawAttr.PAINT_STROKE ? Math.max(Line.ON_LINE_THRESH, stroke / 2) : stroke / 2;
			if (LineUtil.distanceSquared(p[0], p[1], q[0], q[1]) < thr * thr)
				return true;
		}
		return type != DrawAttr.PAINT_STROKE && getCurve(null).contains(loc.x(), loc.y());
	}

	@Override
	public void translate(Location distance) {
		p0 = p0.add(distance);
		p1 = p1.add(distance);
		p2 = p2.add(distance);
		bounds = bounds.translate(distance);
	}

	public List<Handle> getHandles() {
		return UnmodifiableList.create(getHandleArray(null));
	}

	@Override
	public List<Handle> getHandles(HandleGesture gesture) {
		return UnmodifiableList.create(getHandleArray(gesture));
	}

	private Handle[] getHandleArray(HandleGesture gesture) {
		Handle[] ret = {
				new Handle(this, p0),
				new Handle(this, p1),
				new Handle(this, p2)
		};
		if (gesture == null)
			return ret;

		Handle g = gesture.getHandle();
		Location gLoc = g.getLocation().add(gesture.getDeltaX(), gesture.getDeltaY());
		if (g.isAt(p0))
			ret[0] = new Handle(this, gesture.isShiftDown() ? LineUtil.snapTo8Cardinals(p2, gLoc) : gLoc);
		else if (g.isAt(p2))
			ret[2] = new Handle(this, gesture.isShiftDown() ? LineUtil.snapTo8Cardinals(p0, gLoc) : gLoc);
		else if (g.isAt(p1)) {
			if (gesture.isShiftDown()) {
				double midx = (double) (p0.x() + p2.x()) / 2;
				double midy = (double) (p0.y() + p2.y()) / 2;
				double dx = p2.x() - p0.x();
				double dy = p2.y() - p0.y();
				double[] p = LineUtil.nearestPointInfinite(gLoc.x(), gLoc.y(), midx, midy, midx - dy, midy + dx);
				gLoc = new Location((int) Math.round(p[0]), (int) Math.round(p[1]));
			}
			if (gesture.isAltDown()) {
				double[] ct = CurveUtil.interpolate(toArray(p0), toArray(p1), toArray(gLoc));
				gLoc = new Location((int) Math.round(ct[0]), (int) Math.round(ct[1]));
			}
			ret[1] = new Handle(this, gLoc);
		}
		return ret;
	}

	@Override
	public boolean canMoveHandle(Handle handle) {
		return true;
	}

	@Override
	public Handle moveHandle(HandleGesture gesture) {
		Handle[] hs = getHandleArray(gesture);
		p0 = hs[0].getLocation();
		p1 = hs[1].getLocation();
		p2 = hs[2].getLocation();
		bounds = CurveUtil.getBounds(toArray(p0), toArray(p1), toArray(p2));
		return hs[2];
	}

	@Override
	public void paint(Graphics g, HandleGesture gesture) {
		QuadCurve2D curve = getCurve(gesture);
		if (setForFill(g))
			((Graphics2D) g).fill(curve);
		if (setForStroke(g))
			((Graphics2D) g).draw(curve);
	}

	private QuadCurve2D getCurve(HandleGesture gesture) {
		Handle[] p = getHandleArray(gesture);
		return new QuadCurve2D.Double(p[0].getX(), p[0].getY(), p[1].getX(), p[1].getY(), p[2].getX(), p[2].getY());
	}
}
