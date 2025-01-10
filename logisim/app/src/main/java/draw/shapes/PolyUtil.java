/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.shapes;

import draw.model.Handle;
import logisim.data.Location;

import java.util.Collection;

public class PolyUtil {
	private PolyUtil() {
	}

	public static class ClosestResult {
		private double dist;
		private Location loc;
		private Handle prevHandle;
		private Handle nextHandle;

		public double getDistanceSq() {
			return dist;
		}

		public Location getLocation() {
			return loc;
		}

		public Handle getPreviousHandle() {
			return prevHandle;
		}

		public Handle getNextHandle() {
			return nextHandle;
		}
	}

	public static ClosestResult getClosestPoint(Location loc, boolean closed, Collection<Handle> hs) {
		if (hs.isEmpty())
			return null;
		int xq = loc.x();
		int yq = loc.y();
		ClosestResult ret = new ClosestResult();
		ret.dist = Double.MAX_VALUE;

		var ref = new Object() {
			Handle h0 = hs.stream().findFirst().get();
			int x0 = h0.getX();
			int y0 = h0.getY();
		};
		int take = hs.size() - (closed ? 1 : 2);
		hs.stream().skip(1).limit(take).forEach(h1 -> {
			int x1 = h1.getX();
			int y1 = h1.getY();
			double d = LineUtil.ptDistSqSegment(ref.x0, ref.y0, x1, y1, xq, yq);
			if (d < ret.dist) {
				ret.dist = d;
				ret.prevHandle = ref.h0;
				ret.nextHandle = h1;
			}
			ref.h0 = h1;
			ref.x0 = x1;
			ref.y0 = y1;
		});
		if (ret.dist == Double.MAX_VALUE)
			return null;
		Handle h0 = ret.prevHandle;
		Handle h1 = ret.nextHandle;
		double[] p = LineUtil.nearestPointSegment(xq, yq, h0.getX(), h0.getY(), h1.getX(), h1.getY());
		ret.loc = new Location((int) Math.round(p[0]), (int) Math.round(p[1]));
		return ret;
	}
}
