/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import java.awt.Graphics;
import java.awt.geom.GeneralPath;
import java.util.List;
import java.util.Objects;
import java.util.Random;

import draw.shapes.PolyUtil.ClosestResult;
import org.w3c.dom.Document;
import org.w3c.dom.Element;

import draw.model.CanvasObject;
import draw.model.Handle;
import draw.model.HandleGesture;
import logisim.data.Attribute;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.UnmodifiableList;

public class Poly extends FillableCanvasObject {
	private boolean closed;
	// "handles" should be immutable - create a new array and change using
	// setHandles rather than changing contents
	private List<Handle> handles;
	private GeneralPath path;
	private double[] lens;
	private Bounds bounds;

	public Poly(boolean closed, List<Location> locations) {
		handles = locations.stream().map(l->new Handle(this, l)).toList();
		this.closed = closed;
		recomputeBounds();
	}

	@Override
	public boolean matches(CanvasObject other) {
		if (!(other instanceof Poly that))
			return false;
		List<Handle> a = handles;
		List<Handle> b = that.handles;
		if (closed != that.closed || a.size() != b.size())
			return false;
		for (int i = 0, n = a.size(); i < n; i++)
			if (!Objects.equals(a.get(i), b.get(i)))
				return false;
		return super.matches(that);
	}

	@Override
	public int matchesHashCode() {
		int ret = super.matchesHashCode();
		ret = ret * 3 + (closed ? 1 : 0);
		for (Handle h : handles)
			ret = ret * 31 + h.hashCode();
		return ret;
	}

	@Override
	public String getDisplayName() {
		if (closed) return Strings.get("shapePolygon");
		else return Strings.get("shapePolyline");
	}

	@Override
	public Element toSvgElement(Document doc) {
		return SvgCreator.createPoly(doc, this);
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return DrawAttr.getFillAttributes(getPaintType());
	}

	@Override
	public final boolean contains(Location loc, boolean assumeFilled) {
		Object type = getPaintType();
		if (assumeFilled && type == DrawAttr.PAINT_STROKE) type = DrawAttr.PAINT_STROKE_FILL;
		if (type == DrawAttr.PAINT_STROKE) {
			int thresh = Math.max(Line.ON_LINE_THRESH, getStrokeWidth() / 2);
			ClosestResult result = PolyUtil.getClosestPoint(loc, closed, handles);
			return result.getDistanceSq() < thresh * thresh;
		} else if (type == DrawAttr.PAINT_FILL) {
			GeneralPath path = getPath();
			return path.contains(loc.x(), loc.y());
		} else { // fill and stroke
			GeneralPath path = getPath();
			if (path.contains(loc.x(), loc.y()))
				return true;
			int width = getStrokeWidth();
			ClosestResult result = PolyUtil.getClosestPoint(loc, closed, handles);
			return result.getDistanceSq() < (double) (width * width) / 4;
		}
	}

	@Override
	public final Location getRandomPoint(Bounds bds, Random rand) {
		if (getPaintType() == DrawAttr.PAINT_STROKE) {
			Location ret = getRandomBoundaryPoint(bds, rand);
			int w = getStrokeWidth();
			if (w > 1) {
				int dx = rand.nextInt(w) - w / 2;
				int dy = rand.nextInt(w) - w / 2;
				return ret.add(dx, dy);
			}
			return ret;
		} else return super.getRandomPoint(bds, rand);
	}

	private Location getRandomBoundaryPoint(Bounds bds, Random rand) {
		List<Handle> hs = handles;
		double[] ls = lens;
		if (ls == null) {
			ls = new double[hs.size() + (closed ? 1 : 0)];
			double total = 0.0;
			for (int i = 0; i < ls.length; i++) {
				total += LineUtil.distance(hs.get(i).location, hs.get(i).location);
				ls[i] = total;
			}
			lens = ls;
		}
		double pos = ls[ls.length - 1] * rand.nextDouble();
		for (int i = 0; true; i++)
			if (pos < ls[i]) {
				Handle p = hs.get(i);
				Handle q = hs.get((i + 1) % hs.size());
				double u = Math.random();
				int x = (int) Math.round(p.getX() + u * (q.getX() - p.getX()));
				int y = (int) Math.round(p.getY() + u * (q.getY() - p.getY()));
				return new Location(x, y);
			}
	}

	@Override
	public Bounds getBounds() {
		return bounds;
	}

	@Override
	public void translate(Location distance) {
		setHandles(handles.stream().map(h -> new Handle(this, h.location.add(distance))).toList());
	}

	public boolean isClosed() {
		return closed;
	}

	@Override
	public List<Handle> getHandles(HandleGesture gesture) {
		List<Handle> hs = handles;
		if (gesture == null)
			return handles;

		Handle g = gesture.getHandle();
		Handle[] ret = new Handle[hs.size()];
		for (int i = 0, n = hs.size(); i < n; i++) {
			Handle h = hs.get(i);
			if (!h.equals(g)) {
				ret[i] = h;
				continue;
			}

			Location to = h.getLocation().add(gesture.getDelta());
			if (!gesture.isShiftDown()) {
				ret[i] = new Handle(this, to);
				continue;
			}

			Location prev = hs.get((i + n - 1) % n).getLocation();
			Location next = hs.get((i + 1) % n).getLocation();
			if (!closed) {
				if (i == 0)
					prev = null;
				if (i == n - 1)
					next = null;
			}
			if (prev == null)
				to = LineUtil.snapTo8Cardinals(next, to);
			else if (next == null)
				to = LineUtil.snapTo8Cardinals(prev, to);
			else {
				Location a = LineUtil.snapTo8Cardinals(prev, to);
				Location b = LineUtil.snapTo8Cardinals(next, to);
				to = a.manhattanDistanceTo(to) < b.manhattanDistanceTo(to) ? a : b;
			}
			ret[i] = new Handle(this, to);
		}
		return UnmodifiableList.create(ret);
	}

	@Override
	public boolean canMoveHandle(Handle handle) {
		return true;
	}

	@Override
	public Handle moveHandle(HandleGesture gesture) {
		setHandles(getHandles(gesture).stream().toList());
		return null;
	}

	@Override
	public Handle canInsertHandle(Location loc) {
		ClosestResult result = PolyUtil.getClosestPoint(loc, closed, handles);
		int thresh = Math.max(Line.ON_LINE_THRESH, getStrokeWidth() / 2);
		if (!(result.getDistanceSq() < thresh * thresh))
			return null;
		Location resLoc = result.getLocation();
		return result.getPreviousHandle().isAt(resLoc) || result.getNextHandle().isAt(resLoc)
				? null : new Handle(this, result.getLocation());
	}

	@Override
	public Handle canDeleteHandle(Location loc) {
		int minHandles = closed ? 3 : 2;
		if (handles.size() <= minHandles)
			return null;

		int w = Math.max(Line.ON_LINE_THRESH, getStrokeWidth() / 2);
		return handles.stream().filter(h->LineUtil.distance(loc, h.location) < w * w).findAny().orElse(null);
	}

	@Override
	public void insertHandle(Handle desired, Handle previous) {
		if (previous == null)
			previous = PolyUtil.getClosestPoint(desired.getLocation(), closed, handles).getPreviousHandle();
		int index = handles.indexOf(previous);
		if(index == -1)
			throw new IllegalArgumentException("no such handle");
		handles.add(index, desired);
		setHandles(handles);
	}

	@Override
	public Handle deleteHandle(Handle handle) {
		int previous = handles.indexOf(handle) - 1;
		handles.remove(handle);
		setHandles(handles);
		return handles.get(previous);
	}

	@Override
	public void paint(Graphics g, HandleGesture gesture) {
		List<Handle> hs = getHandles(gesture);
		int[] xs = new int[hs.size()];
		int[] ys = new int[hs.size()];
		int i = -1;
		for (Handle h : hs) {
			i++;
			xs[i] = h.getX();
			ys[i] = h.getY();
		}

		if (setForFill(g))
			g.fillPolygon(xs, ys, xs.length);
		if (setForStroke(g)) {
			if (closed)
				g.drawPolygon(xs, ys, xs.length);
			else
				g.drawPolyline(xs, ys, xs.length);
		}
	}

	private void setHandles(List<Handle> hs) {
		handles = hs;
		lens = null;
		path = null;
		recomputeBounds();
	}

	private void recomputeBounds() {
		List<Handle> hs = handles;
		int x0 = hs.getFirst().getX();
		int y0 = hs.getFirst().getY();
		int x1 = x0;
		int y1 = y0;
		for (int i = 1; i < hs.size(); i++) {
			int x = hs.get(i).getX();
			int y = hs.get(i).getY();
			if (x < x0)
				x0 = x;
			if (x > x1)
				x1 = x;
			if (y < y0)
				y0 = y;
			if (y > y1)
				y1 = y;
		}
		Bounds bds = Bounds.create(x0, y0, x1 - x0 + 1, y1 - y0 + 1);
		int stroke = getStrokeWidth();
		bounds = stroke < 2 ? bds : bds.expand(stroke / 2);
	}

	private GeneralPath getPath() {
		if (path != null)
			return path;
		path = new GeneralPath();
		if (handles.isEmpty())
			return path;

		Handle first = handles.getFirst();
		path.moveTo(first.getX(), first.getY());
		handles.stream().skip(1).forEach(h -> path.lineTo(h.getX(), h.getY()));
		return path;
	}
}
