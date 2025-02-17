/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.awt.Color;
import java.awt.Graphics;
import java.util.HashMap;

import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.instance.InstancePainter;
import logisim.util.GraphicsUtil;

class PainterDin {
	private PainterDin() {
	}

	static final int AND = 0;
	static final int OR = 1;
	static final int XOR = 2;
	static final int XNOR = 3;

	private static HashMap<Integer, int[]> orLenArrays = new HashMap<>();

	static void paintAnd(InstancePainter painter, int width, int height, boolean drawBubble) {
		paint(painter, width, height, drawBubble, AND);
	}

	static void paintOr(InstancePainter painter, int width, int height, boolean drawBubble) {
		paint(painter, width, height, drawBubble, OR);
	}

	static void paintXor(InstancePainter painter, int width, int height, boolean drawBubble) {
		paint(painter, width, height, drawBubble, XOR);
	}

	static void paintXnor(InstancePainter painter, int width, int height, boolean drawBubble) {
		paint(painter, width, height, drawBubble, XNOR);
	}

	private static void paint(InstancePainter painter, int width, int height, boolean drawBubble, int dinType) {
		Graphics g = painter.getGraphics();
		int xMid = -width;
		int y0 = -height / 2;
		if (drawBubble) width -= 8;
		int diam = Math.min(height, 2 * width);
		if (dinType == AND) ; // nothing to do
		else if (dinType == OR) paintOrLines(painter, width, height, drawBubble);
		else if (dinType == XOR || dinType == XNOR) {
			int elen = Math.min(diam / 2 - 10, 20);
			int ex0 = xMid + (diam / 2 - elen) / 2;
			int ex1 = ex0 + elen;
			g.drawLine(ex0, -5, ex1, -5);
			g.drawLine(ex0, 0, ex1, 0);
			g.drawLine(ex0, 5, ex1, 5);
			if (dinType == XOR) {
				int exMid = ex0 + elen / 2;
				g.drawLine(exMid, -8, exMid, 8);
			}
		} else throw new IllegalArgumentException("unrecognized shape");

		GraphicsUtil.switchToWidth(g, 2);
		int x0 = xMid - diam / 2;
		Color oldColor = g.getColor();
		if (painter.getShowState()) {
			WireValue val = painter.getPort(0);
			g.setColor(val.getColor());
		}
		g.drawLine(x0 + diam, 0, 0, 0);
		g.setColor(oldColor);
		if (height <= diam) g.drawArc(x0, y0, diam, diam, -90, 180);
		else {
			int x1 = x0 + diam;
			int yy0 = -(height - diam) / 2;
			int yy1 = (height - diam) / 2;
			g.drawArc(x0, y0, diam, diam, 0, 90);
			g.drawLine(x1, yy0, x1, yy1);
			g.drawArc(x0, y0 + height - diam, diam, diam, -90, 90);
		}
		g.drawLine(xMid, y0, xMid, y0 + height);
		if (drawBubble) g.fillOval(x0 + diam - 4, -4, 8, 8);
	}

	private static void paintOrLines(InstancePainter painter, int width, int height, boolean hasBubble) {
		GateAttributes baseAttrs = (GateAttributes) painter.getAttributeSet();
		int inputs = baseAttrs.inputs;
		GateAttributes attrs = (GateAttributes) OrGate.FACTORY.createAttributeSet();
		attrs.inputs = inputs;
		attrs.size = baseAttrs.size;

		Graphics g = painter.getGraphics();
		// draw state if appropriate
		// ignore lines if in print view
		int r = Math.min(height / 2, width);
		Integer hash = r << 4 | inputs;
		int[] lens = orLenArrays.get(hash);
		if (lens == null) {
			lens = new int[inputs];
			orLenArrays.put(hash, lens);
			int yCurveStart = height / 2 - r;
			for (int i = 0; i < inputs; i++) {
				int y = OrGate.FACTORY.getInputOffset(attrs, i).y();
				if (y < 0)
					y = -y;
				if (y <= yCurveStart) lens[i] = r;
				else {
					int dy = y - yCurveStart;
					lens[i] = (int) (Math.sqrt(r * r - dy * dy) + 0.5);
				}
			}
		}

		AbstractGate factory = hasBubble ? NorGate.FACTORY : OrGate.FACTORY;
		boolean printView = painter.isPrintView() && painter.getInstance() != null;
		GraphicsUtil.switchToWidth(g, 2);
		for (int i = 0; i < inputs; i++)
			if (!printView || painter.isPortConnected(i)) {
				Location loc = factory.getInputOffset(attrs, i);
				int x = loc.x();
				int y = loc.y();
				g.drawLine(x, y, x + lens[i], y);
			}
	}
}
