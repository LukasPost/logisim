/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.awt.Color;
import java.awt.Graphics;
import java.util.*;

import logisim.comp.Component;
import logisim.comp.ComponentDrawContext;
import logisim.comp.EndData;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.StdAttr;
import logisim.std.wiring.PullResistor;
import logisim.std.wiring.Tunnel;
import logisim.util.GraphicsUtil;
import logisim.util.IteratorUtil;
import logisim.util.SmallSet;

class CircuitWires {
	static class SplitterData {
		WireBundle[] end_bundle; // PointData associated with each end

		SplitterData(int fan_out) {
			end_bundle = new WireBundle[fan_out + 1];
		}
	}

	static class ThreadBundle {
		int loc;
		WireBundle b;

		ThreadBundle(int loc, WireBundle b) {
			this.loc = loc;
			this.b = b;
		}
	}

	static class State {
		BundleMap bundleMap;
		HashMap<WireThread, WireValue> thr_values = new HashMap<>();

		State(BundleMap bundleMap) {
			this.bundleMap = bundleMap;
		}

		@Override
		public Object clone() {
			State ret = new State(bundleMap);
			ret.thr_values.putAll(thr_values);
			return ret;
		}
	}

	private class TunnelListener implements AttributeListener {
		public void attributeListChanged(AttributeEvent e) {
		}

		public void attributeValueChanged(AttributeEvent e) {
			Attribute<?> attr = e.getAttribute();
			if (attr == StdAttr.LABEL || attr == PullResistor.ATTR_PULL_TYPE) voidBundleMap();
		}
	}

	static class BundleMap {
		boolean computed;
		HashMap<Location, WireBundle> pointBundles = new HashMap<>();
		HashSet<WireBundle> bundles = new HashSet<>();
		boolean isValid = true;
		// NOTE: It would make things more efficient if we also had
		// a set of just the first bundle in each tree.
		HashSet<WidthIncompatibilityData> incompatibilityData;

		HashSet<WidthIncompatibilityData> getWidthIncompatibilityData() {
			return incompatibilityData;
		}

		void addWidthIncompatibilityData(WidthIncompatibilityData e) {
			if (incompatibilityData == null) incompatibilityData = new HashSet<>();
			incompatibilityData.add(e);
		}

		WireBundle getBundleAt(Location p) {
			return pointBundles.get(p);
		}

		WireBundle createBundleAt(Location p) {
			WireBundle ret = pointBundles.get(p);
			if (ret == null) {
				ret = new WireBundle();
				pointBundles.put(p, ret);
				ret.points.add(p);
				bundles.add(ret);
			}
			return ret;
		}

		boolean isValid() {
			return isValid;
		}

		void invalidate() {
			isValid = false;
		}

		void setBundleAt(Location p, WireBundle b) {
			pointBundles.put(p, b);
		}

		Set<Location> getBundlePoints() {
			return pointBundles.keySet();
		}

		Set<WireBundle> getBundles() {
			return bundles;
		}

		synchronized void markComputed() {
			computed = true;
			notifyAll();
		}

		synchronized void waitUntilComputed() {
			while (!computed) try {
				wait();
			} catch (InterruptedException e) {
			}
		}
	}

	// user-given data
	private HashSet<Wire> wires = new HashSet<>();
	private HashSet<Splitter> splitters = new HashSet<>();
	private HashSet<Component> tunnels = new HashSet<>(); // of Components with Tunnel factory
	private TunnelListener tunnelListener = new TunnelListener();
	private HashSet<Component> pulls = new HashSet<>(); // of Components with PullResistor factory
	final CircuitPoints points = new CircuitPoints();

	// derived data
	private Bounds bounds = Bounds.EMPTY_BOUNDS;
	private BundleMap bundleMap;

	CircuitWires() {
	}

	//
	// query methods
	//
	boolean isMapVoided() {
		return bundleMap == null;
	}

	Set<WidthIncompatibilityData> getWidthIncompatibilityData() {
		return getBundleMap().getWidthIncompatibilityData();
	}

	void ensureComputed() {
		getBundleMap();
	}

	BitWidth getWidth(Location q) {
		BitWidth det = points.getWidth(q);
		if (det != BitWidth.UNKNOWN)
			return det;

		BundleMap bmap = getBundleMap();
		if (!bmap.isValid())
			return BitWidth.UNKNOWN;
		WireBundle qb = bmap.getBundleAt(q);
		if (qb != null && qb.isValid())
			return qb.getWidth();

		return BitWidth.UNKNOWN;
	}

	Location getWidthDeterminant(Location q) {
		BitWidth det = points.getWidth(q);
		if (det != BitWidth.UNKNOWN)
			return q;

		WireBundle qb = getBundleMap().getBundleAt(q);
		if (qb != null && qb.isValid())
			return qb.getWidthDeterminant();

		return q;
	}

	Iterator<? extends Component> getComponents() {
		return IteratorUtil.createJoinedIterator(splitters.iterator(), wires.iterator());
	}

	Set<Wire> getWires() {
		return wires;
	}

	Bounds getWireBounds() {
		Bounds bds = bounds;
		if (bds == Bounds.EMPTY_BOUNDS) return recomputeBounds();
		return bds;
	}

	WireBundle getWireBundle(Location query) {
		BundleMap bmap = getBundleMap();
		return bmap.getBundleAt(query);
	}

	WireSet getWireSet(Wire start) {
		WireBundle bundle = getWireBundle(start.e0);
		if (bundle == null)
			return WireSet.EMPTY;
		HashSet<Wire> wires = new HashSet<>();
		for (Location loc : bundle.points) wires.addAll(points.getWires(loc));
		return new WireSet(wires);
	}

	//
	// action methods
	//
	// NOTE: this could be made much more efficient in most cases to
	// avoid voiding the bundle map.
	boolean add(Component comp) {
		boolean added = true;
		if (comp instanceof Wire) added = addWire((Wire) comp);
		else if (comp instanceof Splitter) splitters.add((Splitter) comp);
		else {
			Object factory = comp.getFactory();
			if (factory instanceof Tunnel) {
				tunnels.add(comp);
				comp.getAttributeSet().addAttributeListener(tunnelListener);
			} else if (factory instanceof PullResistor) {
				pulls.add(comp);
				comp.getAttributeSet().addAttributeListener(tunnelListener);
			}
		}
		if (added) {
			points.add(comp);
			voidBundleMap();
		}
		return added;
	}

	void remove(Component comp) {
		if (comp instanceof Wire) removeWire((Wire) comp);
		else if (comp instanceof Splitter) splitters.remove(comp);
		else {
			Object factory = comp.getFactory();
			if (factory instanceof Tunnel) {
				tunnels.remove(comp);
				comp.getAttributeSet().removeAttributeListener(tunnelListener);
			} else if (factory instanceof PullResistor) {
				pulls.remove(comp);
				comp.getAttributeSet().removeAttributeListener(tunnelListener);
			}
		}
		points.remove(comp);
		voidBundleMap();
	}

	void add(Component comp, EndData end) {
		points.add(comp, end);
		voidBundleMap();
	}

	void remove(Component comp, EndData end) {
		points.remove(comp, end);
		voidBundleMap();
	}

	void replace(Component comp, EndData oldEnd, EndData newEnd) {
		points.remove(comp, oldEnd);
		points.add(comp, newEnd);
		voidBundleMap();
	}

	private boolean addWire(Wire w) {
		boolean added = wires.add(w);
		if (!added)
			return false;

		// update bounds
		if (bounds != Bounds.EMPTY_BOUNDS) bounds = bounds.add(w.e0).add(w.e1);
		return true;
	}

	private void removeWire(Wire w) {
		boolean removed = wires.remove(w);
		if (!removed)
			return;

		if (bounds != Bounds.EMPTY_BOUNDS) {
			// bounds is valid - invalidate if endpoint on border
			Bounds smaller = bounds.expand(-2);
			if (!smaller.contains(w.e0) || !smaller.contains(w.e1)) bounds = Bounds.EMPTY_BOUNDS;
		}
	}

	//
	// utility methods
	//
	void propagate(CircuitState circState, List<Location> points) {
		BundleMap map = getBundleMap();
		SmallSet<WireThread> dirtyThreads = new SmallSet<>(); // affected threads

		// get state, or create a new one if current state is outdated
		State s = circState.getWireData();
		if (s == null || s.bundleMap != map) {
			// if it is outdated, we need to compute for all threads
			s = new State(map);
			for (WireBundle b : map.getBundles()) {
				WireThread[] th = b.threads;
				if (b.isValid() && th != null) Collections.addAll(dirtyThreads, th);
			}
			circState.setWireData(s);
		}

		// determine affected threads, and set values for unwired points
		for (Location p : points) {
			WireBundle pb = map.getBundleAt(p);
			// point is not wired
			if (pb == null) circState.setValueByWire(p, circState.getComponentOutputAt(p));
			else {
				WireThread[] th = pb.threads;
				if (!pb.isValid() || th == null) {
					// immediately propagate NILs across invalid bundles
					SmallSet<Location> pbPoints = pb.points;
					if (pbPoints == null) circState.setValueByWire(p, WireValues.NIL);
					else for (Location loc2 : pbPoints) circState.setValueByWire(loc2, WireValues.NIL);
				} else dirtyThreads.addAll(Arrays.asList(th));
			}
		}

		if (dirtyThreads.isEmpty())
			return;

		// determine values of affected threads
		HashSet<ThreadBundle> bundles = new HashSet<>();
		for (WireThread t : dirtyThreads) {
			WireValue v = getThreadValue(circState, t);
			s.thr_values.put(t, v);
			bundles.addAll(t.getBundles());
		}

		// now propagate values through circuit
		for (ThreadBundle tb : bundles) {
			WireBundle b = tb.b;

			WireValue bv = null;
			if (!b.isValid() || b.threads == null) ; // do nothing
			else if (b.threads.length == 1) bv = s.thr_values.get(b.threads[0]);
			else {
				WireValue[] tvs = new WireValue[b.threads.length];
				boolean tvs_valid = true;
				for (int i = 0; i < tvs.length; i++) {
					WireValue tv = s.thr_values.get(b.threads[i]);
					if (tv == null) {
						tvs_valid = false;
						break;
					}
					tvs[i] = tv;
				}
				if (tvs_valid)
					bv = WireValue.Companion.create(tvs);
			}

			if (bv != null) for (Location p : b.points) circState.setValueByWire(p, bv);
		}
	}

	void draw(ComponentDrawContext context, Collection<Component> hidden) {
		boolean showState = context.getShowState();
		CircuitState state = context.getCircuitState();
		Graphics g = context.getGraphics();
		g.setColor(Color.BLACK);
		GraphicsUtil.switchToWidth(g, Wire.WIDTH);
		WireSet highlighted = context.getHighlightedWires();

		BundleMap bmap = getBundleMap();
		boolean isValid = bmap.isValid();
		if (hidden == null || hidden.isEmpty()) {
			for (Wire w : wires) drawWire(showState, state, g, highlighted, bmap, isValid, w);

			for (Location loc : points.getSplitLocations())
				if (points.getComponentCount(loc) > 2)
					drawWireBundle(showState, state, g, highlighted, bmap, isValid, loc);
		} else {
			for (Wire w : wires) if (!hidden.contains(w)) drawWire(showState, state, g, highlighted, bmap, isValid, w);

			// this is just an approximation, but it's good enough since
			// the problem is minor, and hidden only exists for a short
			// while at a time anway.
			for (Location loc : points.getSplitLocations())
				if (points.getComponentCount(loc) > 2) {
					int icount = 0;
					for (Component comp : points.getComponents(loc))
						if (!hidden.contains(comp))
							++icount;
					if (icount > 2) drawWireBundle(showState, state, g, highlighted, bmap, isValid, loc);
				}
		}
	}

	private void drawWireBundle(boolean showState, CircuitState state, Graphics g, WireSet highlighted, BundleMap bmap, boolean isValid, Location loc) {
		WireBundle wb = bmap.getBundleAt(loc);
		if (wb != null) {
			if (!wb.isValid()) g.setColor(WireValue.Companion.getWIDTH_ERROR_COLOR());
			else if (showState) if (!isValid)
				g.setColor(WireValues.Companion.getNIL_COLOR());
			else
				g.setColor(state.getValue(loc).getColor());
			else g.setColor(Color.BLACK);
			if (highlighted.containsLocation(loc)) g.fillOval(loc.x() - 5, loc.y() - 5, 10, 10);
			else g.fillOval(loc.x() - 4, loc.y() - 4, 8, 8);
		}
	}

	private void drawWire(boolean showState, CircuitState state, Graphics g, WireSet highlighted, BundleMap bmap, boolean isValid, Wire w) {
		Location s = w.e0;
		Location t = w.e1;
		WireBundle wb = bmap.getBundleAt(s);
		if (!wb.isValid()) g.setColor(WireValue.Companion.getWIDTH_ERROR_COLOR());
		else if (showState) if (!isValid)
			g.setColor(WireValues.Companion.getNIL_COLOR());
		else
			g.setColor(state.getValue(s).getColor());
		else g.setColor(Color.BLACK);
		if (highlighted.containsWire(w)) {
			GraphicsUtil.switchToWidth(g, Wire.WIDTH + 2);
			g.drawLine(s.x(), s.y(), t.x(), t.y());
			GraphicsUtil.switchToWidth(g, Wire.WIDTH);
		} else g.drawLine(s.x(), s.y(), t.x(), t.y());
	}

	//
	// helper methods
	//
	private void voidBundleMap() {
		bundleMap = null;
	}

	private BundleMap getBundleMap() {
		// Maybe we already have a valid bundle map (or maybe
		// one is in progress).
		BundleMap ret = bundleMap;
		if (ret != null) {
			ret.waitUntilComputed();
			return ret;
		}
		try {
			// Ok, we have to create our own.
			for (int tries = 4; tries >= 0; tries--)
				try {
					ret = new BundleMap();
					computeBundleMap(ret);
					bundleMap = ret;
					break;
				} catch (Throwable t) {
					if (tries == 0) {
						t.printStackTrace();
						bundleMap = ret;
					}
				}
		} catch (RuntimeException ex) {
			if (ret != null) {
				ret.invalidate();
				ret.markComputed();
			}
			throw ex;
		} finally {
			// Mark the BundleMap as computed in case anybody is waiting for the result.
			if(ret != null)
				ret.markComputed();
		}
		return ret;
	}

	// To be called by getBundleMap only
	private void computeBundleMap(BundleMap ret) {
		// create bundles corresponding to wires and tunnels
		connectWires(ret);
		connectTunnels(ret);
		connectPullResistors(ret);

		// merge any WireBundle objects united by previous steps
		for (Iterator<WireBundle> it = ret.getBundles().iterator(); it.hasNext();) {
			WireBundle b = it.next();
			WireBundle bpar = b.find();
			if (bpar != b) { // b isn't group's representative
				for (Location pt : b.points) {
					ret.setBundleAt(pt, bpar);
					bpar.points.add(pt);
				}
				bpar.addPullValue(b.getPullValue());
				it.remove();
			}
		}

		// make a WireBundle object for each end of a splitter
		for (Splitter spl : splitters) {
			List<EndData> ends = new ArrayList<>(spl.getEnds());
			for (EndData end : ends) {
				Location p = end.getLocation();
				WireBundle pb = ret.createBundleAt(p);
				pb.setWidth(end.getWidth(), p);
			}
		}

		// set the width for each bundle whose size is known
		// based on components
		for (Location p : ret.getBundlePoints()) {
			WireBundle pb = ret.getBundleAt(p);
			BitWidth width = points.getWidth(p);
			if (width != BitWidth.UNKNOWN) pb.setWidth(width, p);
		}

		// determine the bundles at the end of each splitter
		for (Splitter spl : splitters) {
			List<EndData> ends = new ArrayList<>(spl.getEnds());
			int index = -1;
			for (EndData end : ends) {
				index++;
				Location p = end.getLocation();
				WireBundle pb = ret.getBundleAt(p);
				if (pb != null) {
					pb.setWidth(end.getWidth(), p);
					spl.wire_data.end_bundle[index] = pb;
				}
			}
		}

		// unite threads going through splitters
		for (Splitter spl : splitters)
			synchronized (spl) {
				SplitterAttributes spl_attrs = (SplitterAttributes) spl.getAttributeSet();
				byte[] bit_end = spl_attrs.bit_end;
				SplitterData spl_data = spl.wire_data;
				WireBundle from_bundle = spl_data.end_bundle[0];
				if (from_bundle == null || !from_bundle.isValid())
					continue;

				for (int i = 0; i < bit_end.length; i++) {
					int j = bit_end[i];
					if (j > 0) {
						int thr = spl.bit_thread[i];
						WireBundle to_bundle = spl_data.end_bundle[j];
						WireThread[] to_threads = to_bundle.threads;
						if (to_threads != null && to_bundle.isValid()) {
							WireThread[] from_threads = from_bundle.threads;
							if (i >= from_threads.length)
								throw new ArrayIndexOutOfBoundsException("from " + i + " of " + from_threads.length);
							if (thr >= to_threads.length)
								throw new ArrayIndexOutOfBoundsException("to " + thr + " of " + to_threads.length);
							from_threads[i].unite(to_threads[thr]);
						}
					}
				}
			}

		// merge any threads united by previous step
		for (WireBundle b : ret.getBundles())
			if (b.isValid() && b.threads != null) for (int i = 0; i < b.threads.length; i++) {
				WireThread thr = b.threads[i].find();
				b.threads[i] = thr;
				thr.getBundles().add(new ThreadBundle(i, b));
			}

		// All threads are sewn together! Compute the exception set before leaving
		Collection<WidthIncompatibilityData> exceptions = points.getWidthIncompatibilityData();
		if (exceptions != null && !exceptions.isEmpty())
			for (WidthIncompatibilityData wid : exceptions) ret.addWidthIncompatibilityData(wid);
		for (WireBundle b : ret.getBundles()) {
			WidthIncompatibilityData e = b.getWidthIncompatibilityData();
			if (e != null)
				ret.addWidthIncompatibilityData(e);
		}
	}

	private void connectWires(BundleMap ret) {
		// make a WireBundle object for each tree of connected wires
		for (Wire w : wires) {
			WireBundle b0 = ret.getBundleAt(w.e0);
			if (b0 == null) {
				WireBundle b1 = ret.createBundleAt(w.e1);
				b1.points.add(w.e0);
				ret.setBundleAt(w.e0, b1);
			} else {
				WireBundle b1 = ret.getBundleAt(w.e1);
				if (b1 == null) { // t1 doesn't exist
					b0.points.add(w.e1);
					ret.setBundleAt(w.e1, b0);
				} else b1.unite(b0); // unite b0 and b1
			}
		}
	}

	private void connectTunnels(BundleMap ret) {
		// determine the sets of tunnels
		HashMap<String, ArrayList<Location>> tunnelSets = new HashMap<>();
		for (Component comp : tunnels) {
			String label = comp.getAttributeSet().getValue(StdAttr.LABEL);
			label = label.trim();
			if (!label.isEmpty()) {
				ArrayList<Location> tunnelSet = tunnelSets.computeIfAbsent(label, k -> new ArrayList<>(3));
				tunnelSet.add(comp.getLocation());
			}
		}

		// now connect the bundles that are tunnelled together
		for (ArrayList<Location> tunnelSet : tunnelSets.values()) {
			WireBundle foundBundle = null;
			Location foundLocation = null;
			for (Location loc : tunnelSet) {
				WireBundle b = ret.getBundleAt(loc);
				if (b != null) {
					foundBundle = b;
					foundLocation = loc;
					break;
				}
			}
			if (foundBundle == null) {
				foundLocation = tunnelSet.getFirst();
				foundBundle = ret.createBundleAt(foundLocation);
			}
			for (Location loc : tunnelSet)
				if (loc != foundLocation) {
					WireBundle b = ret.getBundleAt(loc);
					if (b == null) {
						foundBundle.points.add(loc);
						ret.setBundleAt(loc, foundBundle);
					}
					else b.unite(foundBundle);
				}
		}
	}

	private void connectPullResistors(BundleMap ret) {
		for (Component comp : pulls) {
			Location loc = comp.getEnd(0).getLocation();
			WireBundle b = ret.getBundleAt(loc);
			if (b == null) {
				b = ret.createBundleAt(loc);
				b.points.add(loc);
				ret.setBundleAt(loc, b);
			}
			Instance instance = Instance.getInstanceFor(comp);
			b.addPullValue(PullResistor.getPullValue(instance));
		}
	}

	private WireValue getThreadValue(CircuitState state, WireThread t) {
		WireValue ret = WireValues.UNKNOWN;
		WireValue pull = WireValues.UNKNOWN;
		for (ThreadBundle tb : t.getBundles()) {
			for (Location p : tb.b.points) {
				WireValue val = state.getComponentOutputAt(p);
				if (val != null && val != WireValues.NIL) ret = ret.combine(val.get(tb.loc));
			}
			WireValue pullHere = tb.b.getPullValue();
			if (pullHere != WireValues.UNKNOWN)
				pull = pull.combine(pullHere);
		}
		if (pull != WireValues.UNKNOWN) return pullValue(ret, pull);
		return ret;
	}

	private static WireValue pullValue(WireValue base, WireValue pullTo) {
		if (base.isFullyDefined()) return base;
		else if (base.getWidth() == 1) if (base == WireValues.UNKNOWN)
			return pullTo;
		else
			return base;
		else {
			WireValue[] ret = base.getAll();
			for (int i = 0; i < ret.length; i++)
				if (ret[i] == WireValues.UNKNOWN)
					ret[i] = pullTo;
			return WireValue.Companion.create(ret);
		}
	}

	private Bounds recomputeBounds() {
		Iterator<Wire> it = wires.iterator();
		if (!it.hasNext()) {
			bounds = Bounds.EMPTY_BOUNDS;
			return Bounds.EMPTY_BOUNDS;
		}

		Wire w = it.next();
		int xmin = w.e0.x();
		int ymin = w.e0.y();
		int xmax = w.e1.x();
		int ymax = w.e1.y();
		while (it.hasNext()) {
			w = it.next();
			int x0 = w.e0.x();
			if (x0 < xmin)
				xmin = x0;
			int x1 = w.e1.x();
			if (x1 > xmax)
				xmax = x1;
			int y0 = w.e0.y();
			if (y0 < ymin)
				ymin = y0;
			int y1 = w.e1.y();
			if (y1 > ymax)
				ymax = y1;
		}
		Bounds bds = Bounds.create(xmin, ymin, xmax - xmin + 1, ymax - ymin + 1);
		bounds = bds;
		return bds;
	}
}
