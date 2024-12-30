/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit.appear;

import java.awt.Color;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import draw.model.CanvasObject;
import draw.shapes.Curve;
import draw.shapes.DrawAttr;
import draw.shapes.Rectangle;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.instance.Instance;
import logisim.instance.StdAttr;

class DefaultAppearance {
	private static final int OFFS = 50;

	private DefaultAppearance() {
	}

	private static class CompareLocations implements Comparator<Instance> {
		private boolean byX;

		CompareLocations(boolean byX) {
			this.byX = byX;
		}

		public int compare(Instance a, Instance b) {
			Location aloc = a.getLocation();
			Location bloc = b.getLocation();
			if (byX) {
				int ax = aloc.getX();
				int bx = bloc.getX();
				if (ax != bx) {
					return ax < bx ? -1 : 1;
				}
			} else {
				int ay = aloc.getY();
				int by = bloc.getY();
				if (ay != by) {
					return ay < by ? -1 : 1;
				}
			}
			return aloc.compareTo(bloc);
		}
	}

	static void sortPinList(List<Instance> pins, Direction facing) {
		if (facing == Direction.North || facing == Direction.South) {
			Comparator<Instance> sortHorizontal = new CompareLocations(true);
			Collections.sort(pins, sortHorizontal);
		} else {
			Comparator<Instance> sortVertical = new CompareLocations(false);
			Collections.sort(pins, sortVertical);
		}
	}

	public static List<CanvasObject> build(Collection<Instance> pins) {
		Map<Direction, List<Instance>> edge;
		edge = new HashMap<Direction, List<Instance>>();
		edge.put(Direction.North, new ArrayList<Instance>());
		edge.put(Direction.South, new ArrayList<Instance>());
		edge.put(Direction.East, new ArrayList<Instance>());
		edge.put(Direction.West, new ArrayList<Instance>());
		for (Instance pin : pins) {
			Direction pinFacing = pin.getAttributeValue(StdAttr.FACING);
			Direction pinEdge = pinFacing.reverse();
			List<Instance> e = edge.get(pinEdge);
			e.add(pin);
		}

		for (Map.Entry<Direction, List<Instance>> entry : edge.entrySet()) {
			sortPinList(entry.getValue(), entry.getKey());
		}

		int numNorth = edge.get(Direction.North).size();
		int numSouth = edge.get(Direction.South).size();
		int numEast = edge.get(Direction.East).size();
		int numWest = edge.get(Direction.West).size();
		int maxVert = Math.max(numNorth, numSouth);
		int maxHorz = Math.max(numEast, numWest);

		int offsNorth = computeOffset(numNorth, numSouth, maxHorz);
		int offsSouth = computeOffset(numSouth, numNorth, maxHorz);
		int offsEast = computeOffset(numEast, numWest, maxVert);
		int offsWest = computeOffset(numWest, numEast, maxVert);

		int width = computeDimension(maxVert, maxHorz);
		int height = computeDimension(maxHorz, maxVert);

		// compute position of anchor relative to top left corner of box
		int ax;
		int ay;
		if (numEast > 0) { // anchor is on east side
			ax = width;
			ay = offsEast;
		} else if (numNorth > 0) { // anchor is on north side
			ax = offsNorth;
			ay = 0;
		} else if (numWest > 0) { // anchor is on west side
			ax = 0;
			ay = offsWest;
		} else if (numSouth > 0) { // anchor is on south side
			ax = offsSouth;
			ay = height;
		} else { // anchor is top left corner
			ax = 0;
			ay = 0;
		}

		// place rectangle so anchor is on the grid
		int rx = OFFS + (9 - (ax + 9) % 10);
		int ry = OFFS + (9 - (ay + 9) % 10);

		Location e0 = new Location(rx + (width - 8) / 2, ry + 1);
		Location e1 = new Location(rx + (width + 8) / 2, ry + 1);
		Location ct = new Location(rx + width / 2, ry + 11);
		Curve notch = new Curve(e0, e1, ct);
		notch.setValue(DrawAttr.STROKE_WIDTH, Integer.valueOf(2));
		notch.setValue(DrawAttr.STROKE_COLOR, Color.GRAY);
		Rectangle rect = new Rectangle(rx, ry, width, height);
		rect.setValue(DrawAttr.STROKE_WIDTH, Integer.valueOf(2));

		List<CanvasObject> ret = new ArrayList<CanvasObject>();
		ret.add(notch);
		ret.add(rect);
		placePins(ret, edge.get(Direction.West), rx, ry + offsWest, 0, 10);
		placePins(ret, edge.get(Direction.East), rx + width, ry + offsEast, 0, 10);
		placePins(ret, edge.get(Direction.North), rx + offsNorth, ry, 10, 0);
		placePins(ret, edge.get(Direction.South), rx + offsSouth, ry + height, 10, 0);
		ret.add(new AppearanceAnchor(new Location(rx + ax, ry + ay)));
		return ret;
	}

	private static int computeDimension(int maxThis, int maxOthers) {
		if (maxThis < 3) {
			return 30;
		} else if (maxOthers == 0) {
			return 10 * maxThis;
		} else {
			return 10 * maxThis + 10;
		}
	}

	private static int computeOffset(int numFacing, int numOpposite, int maxOthers) {
		int maxThis = Math.max(numFacing, numOpposite);
		int maxOffs;
		switch (maxThis) {
		case 0:
		case 1:
			maxOffs = (maxOthers == 0 ? 15 : 10);
			break;
		case 2:
			maxOffs = 10;
			break;
		default:
			maxOffs = (maxOthers == 0 ? 5 : 10);
		}
		return maxOffs + 10 * ((maxThis - numFacing) / 2);
	}

	private static void placePins(List<CanvasObject> dest, List<Instance> pins, int x, int y, int dx, int dy) {
		for (Instance pin : pins) {
			dest.add(new AppearancePort(new Location(x, y), pin));
			x += dx;
			y += dy;
		}
	}
}
