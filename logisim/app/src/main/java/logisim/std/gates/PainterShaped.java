/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.geom.GeneralPath;
import java.awt.geom.Point2D;
import java.util.HashMap;

import logisim.data.Direction;
import logisim.data.Location;
import logisim.data.Value;
import logisim.instance.InstancePainter;
import logisim.util.GraphicsUtil;

public class PainterShaped {
	private static final GeneralPath PATH_NARROW;
	private static final GeneralPath PATH_MEDIUM;
	private static final GeneralPath PATH_WIDE;

	private static final GeneralPath SHIELD_NARROW;
	private static final GeneralPath SHIELD_MEDIUM;
	private static final GeneralPath SHIELD_WIDE;

	static {
		PATH_NARROW = new GeneralPath();
		PATH_NARROW.moveTo(0, 0);
		PATH_NARROW.quadTo(-10, -15, -30, -15);
		PATH_NARROW.quadTo(-22, 0, -30, 15);
		PATH_NARROW.quadTo(-10, 15, 0, 0);
		PATH_NARROW.closePath();

		PATH_MEDIUM = new GeneralPath();
		PATH_MEDIUM.moveTo(0, 0);
		PATH_MEDIUM.quadTo(-20, -25, -50, -25);
		PATH_MEDIUM.quadTo(-37, 0, -50, 25);
		PATH_MEDIUM.quadTo(-20, 25, 0, 0);
		PATH_MEDIUM.closePath();

		PATH_WIDE = new GeneralPath();
		PATH_WIDE.moveTo(0, 0);
		PATH_WIDE.quadTo(-25, -35, -70, -35);
		PATH_WIDE.quadTo(-50, 0, -70, 35);
		PATH_WIDE.quadTo(-25, 35, 0, 0);
		PATH_WIDE.closePath();

		SHIELD_NARROW = new GeneralPath();
		SHIELD_NARROW.moveTo(-30, -15);
		SHIELD_NARROW.quadTo(-22, 0, -30, 15);

		SHIELD_MEDIUM = new GeneralPath();
		SHIELD_MEDIUM.moveTo(-50, -25);
		SHIELD_MEDIUM.quadTo(-37, 0, -50, 25);

		SHIELD_WIDE = new GeneralPath();
		SHIELD_WIDE.moveTo(-70, -35);
		SHIELD_WIDE.quadTo(-50, 0, -70, 35);
	}

	private PainterShaped() {
	}

	private static HashMap<Integer, int[]> INPUT_LENGTHS = new HashMap<>();

	static void paintAnd(InstancePainter painter, int width, int height) {
		Graphics g = painter.getGraphics();
		GraphicsUtil.switchToWidth(g, 2);
		int[] xp = { -width / 2, -width + 1, -width + 1, -width / 2 };
		int[] yp = { -width / 2, -width / 2, width / 2, width / 2 };
		GraphicsUtil.drawCenteredArc(g, -width / 2, 0, width / 2, -90, 180);

		g.drawPolyline(xp, yp, 4);
		if (height > width) g.drawLine(-width + 1, -height / 2, -width + 1, height / 2);
	}

	static void paintOr(InstancePainter painter, int width, int height) {
		Graphics g = painter.getGraphics();
		GraphicsUtil.switchToWidth(g, 2);
		/*
		 * The following, used previous to version 2.5.1, didn't use GeneralPath g.setColor(Color.LIGHT_GRAY); if (width
		 * < 40) { GraphicsUtil.drawCenteredArc(g, -30, -21, 36, -90, 53); GraphicsUtil.drawCenteredArc(g, -30, 21, 36,
		 * 90, -53); } else if (width < 60) { GraphicsUtil.drawCenteredArc(g, -50, -37, 62, -90, 53);
		 * GraphicsUtil.drawCenteredArc(g, -50, 37, 62, 90, -53); } else { GraphicsUtil.drawCenteredArc(g, -70, -50, 85,
		 * -90, 53); GraphicsUtil.drawCenteredArc(g, -70, 50, 85, 90, -53); } paintShield(g, -width, 0, width, height);
		 */

		GeneralPath path;
		if (width < 40) path = PATH_NARROW;
		else if (width < 60) path = PATH_MEDIUM;
		else path = PATH_WIDE;
		((Graphics2D) g).draw(path);
		if (height > width) paintShield(g, 0, width, height);
	}

	static void paintNot(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		GraphicsUtil.switchToWidth(g, 2);
		if (painter.getAttributeValue(NotGate.ATTR_SIZE) == NotGate.SIZE_NARROW) {
			GraphicsUtil.switchToWidth(g, 2);
			int[] xp = new int[4];
			int[] yp = new int[4];
			xp[0] = -6;
			yp[0] = 0;
			xp[1] = -19;
			yp[1] = -6;
			xp[2] = -19;
			yp[2] = 6;
			xp[3] = -6;
			yp[3] = 0;
			g.drawPolyline(xp, yp, 4);
			g.drawOval(-6, -3, 6, 6);
		} else {
			int[] xp = new int[4];
			int[] yp = new int[4];
			xp[0] = -10;
			yp[0] = 0;
			xp[1] = -29;
			yp[1] = -7;
			xp[2] = -29;
			yp[2] = 7;
			xp[3] = -10;
			yp[3] = 0;
			g.drawPolyline(xp, yp, 4);
			g.drawOval(-9, -4, 9, 9);
		}
	}

	static void paintXor(InstancePainter painter, int width, int height) {
		Graphics g = painter.getGraphics();
		paintOr(painter, width - 10, width - 10);
		paintShield(g, -10, width - 10, height);
	}

	private static void paintShield(Graphics g, int xlate, int width, int height) {
		GraphicsUtil.switchToWidth(g, 2);
		g.translate(xlate, 0);
		((Graphics2D) g).draw(computeShield(width, height));
		g.translate(-xlate, 0);

		/*
		 * The following, used previous to version 2.5.1, didn't use GeneralPath if (width < 40) {
		 * GraphicsUtil.drawCenteredArc(g, x - 26, y, 30, -30, 60); } else if (width < 60) {
		 * GraphicsUtil.drawCenteredArc(g, x - 43, y, 50, -30, 60); } else { GraphicsUtil.drawCenteredArc(g, x - 60, y,
		 * 70, -30, 60); } if (height > width) { // we need to draw the shield GraphicsUtil.drawCenteredArc(g, x - dx, y
		 * - (width + extra) / 2, extra, -30, 60); GraphicsUtil.drawCenteredArc(g, x - dx, y + (width + extra) / 2,
		 * extra, -30, 60); }
		 */
	}

	private static GeneralPath computeShield(int width, int height) {
		GeneralPath base;
		if (width < 40) base = SHIELD_NARROW;
		else if (width < 60) base = SHIELD_MEDIUM;
		else base = SHIELD_WIDE;

		// no wings
		if (height <= width) return base;
		else { // we need to add wings
			int wingHeight = (height - width) / 2;
			int dx = Math.min(20, wingHeight / 4);

			GeneralPath path = new GeneralPath();
			path.moveTo(-width, (float) -height / 2);
			path.quadTo(-width + dx, (float) -(width + height) / 4, -width, (float) -width / 2);
			path.append(base, true);
			path.quadTo(-width + dx, (float) (width + height) / 4, -width, (float) height / 2);
			return path;
		}
	}

	static void paintInputLines(InstancePainter painter, AbstractGate factory) {
		Location loc = painter.getLocation();
		boolean printView = painter.isPrintView();
		GateAttributes attrs = (GateAttributes) painter.getAttributeSet();
		Direction facing = attrs.facing;
		int inputs = attrs.inputs;
		int negated = attrs.negated;

		int[] lengths = getInputLineLengths(attrs);
		// drawing ghost - negation bubbles only
		if (painter.getInstance() == null) for (int i = 0; i < inputs; i++) {
			boolean iNegated = ((negated >> i) & 1) == 1;
			if (iNegated) {
				Location offs = factory.getInputOffset(attrs, i);
				Location loci = loc.translate(offs.x(), offs.y());
				Location cent = loci.translate(facing, lengths[i] + 5);
				painter.drawDongle(cent.x(), cent.y());
			}
		}
		else {
			Graphics g = painter.getGraphics();
			Color baseColor = g.getColor();
			GraphicsUtil.switchToWidth(g, 3);
			for (int i = 0; i < inputs; i++) {
				Location offs = factory.getInputOffset(attrs, i);
				Location src = loc.translate(offs.x(), offs.y());
				int len = lengths[i];
				if (len != 0 && (!printView || painter.isPortConnected(i + 1))) {
					if (painter.getShowState()) {
						Value val = painter.getPort(i + 1);
						g.setColor(val.getColor());
					} else g.setColor(baseColor);
					Location dst = src.translate(facing, len);
					g.drawLine(src.x(), src.y(), dst.x(), dst.y());
				}
				if (((negated >> i) & 1) == 1) {
					Location cent = src.translate(facing, lengths[i] + 5);
					g.setColor(baseColor);
					painter.drawDongle(cent.x(), cent.y());
					GraphicsUtil.switchToWidth(g, 3);
				}
			}
		}
	}

	private static int[] getInputLineLengths(GateAttributes attrs) {
		int inputs = attrs.inputs;
		int mainHeight = (Integer) attrs.size.getValue();
		Integer key = inputs * 31 + mainHeight;
		Object ret = INPUT_LENGTHS.get(key);
		if (ret != null) return (int[]) ret;

		Direction facing = attrs.facing;
		if (facing != Direction.East) {
			attrs = (GateAttributes) attrs.clone();
			attrs.facing = Direction.East;
		}

		int[] lengths = new int[inputs];
		INPUT_LENGTHS.put(key, lengths);
		Location loc0 = OrGate.FACTORY.getInputOffset(attrs, 0);
		Location locn = OrGate.FACTORY.getInputOffset(attrs, inputs - 1);
		int totalHeight = 10 + loc0.manhattanDistanceTo(locn);
		if (totalHeight < mainHeight)
			totalHeight = mainHeight;

		GeneralPath path = computeShield(mainHeight, totalHeight);
		for (int i = 0; i < inputs; i++) {
			Location loci = OrGate.FACTORY.getInputOffset(attrs, i);
			Point2D p = new Point2D.Float(loci.x() + 1, loci.y());
			int iters = 0;
			while (path.contains(p) && iters < 15) {
				iters++;
				p.setLocation(p.getX() + 1, p.getY());
			}
			if (iters >= 15)
				iters = 0;
			lengths[i] = iters;
		}

		/*
		 * used prior to 2.5.1, when moved to GeneralPath int wingHeight = (totalHeight - mainHeight) / 2; double
		 * wingCenterX = wingHeight * Math.sqrt(3) / 2; double mainCenterX = mainHeight * Math.sqrt(3) / 2;
		 * 
		 * for (int i = 0; i < inputs; i++) { Location loci = factory.getInputOffset(attrs, i); int disti = 5 +
		 * loc0.manhattanDistanceTo(loci); if (disti > totalHeight - disti) { // ensure on top half disti = totalHeight
		 * - disti; } double dx; if (disti < wingHeight) { // point is on wing int dy = wingHeight / 2 - disti; dx =
		 * Math.sqrt(wingHeight * wingHeight - dy * dy) - wingCenterX; } else { // point is on main shield int dy =
		 * totalHeight / 2 - disti; dx = Math.sqrt(mainHeight * mainHeight - dy * dy) - mainCenterX; } lengths[i] =
		 * (int) (dx - 0.5); }
		 */
		return lengths;
	}
}
