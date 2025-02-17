/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.Color;
import java.awt.FontMetrics;
import java.awt.Graphics;

import logisim.comp.TextField;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.instance.Instance;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.tools.key.BitWidthConfigurator;
import logisim.util.GraphicsUtil;

public class Tunnel extends InstanceFactory {
	public static final Tunnel FACTORY = new Tunnel();

	static final int MARGIN = 3;
	static final int ARROW_MARGIN = 5;
	static final int ARROW_DEPTH = 4;
	static final int ARROW_MIN_WIDTH = 16;
	static final int ARROW_MAX_WIDTH = 20;

	public Tunnel() {
		super("Tunnel", Strings.getter("tunnelComponent"));
		setIconName("tunnel.gif");
		setFacingAttribute(StdAttr.FACING);
		setKeyConfigurator(new BitWidthConfigurator(StdAttr.WIDTH));
	}

	@Override
	public AttributeSet createAttributeSet() {
		return new TunnelAttributes();
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrsBase) {
		TunnelAttributes attrs = (TunnelAttributes) attrsBase;
		Bounds bds = attrs.getOffsetBounds();
		if (bds == null) {
			int ht = attrs.getFont().getSize();
			int wd = ht * attrs.getLabel().length() / 2;
			bds = computeBounds(attrs, wd, ht, null, "");
			attrs.setOffsetBounds(bds);
		}
		return bds;
	}

	//
	// graphics methods
	//
	@Override
	public void paintGhost(InstancePainter painter) {
		TunnelAttributes attrs = (TunnelAttributes) painter.getAttributeSet();
		Direction facing = attrs.getFacing();
		String label = attrs.getLabel();

		Graphics g = painter.getGraphics();
		g.setFont(attrs.getFont());
		FontMetrics fm = g.getFontMetrics();
		Bounds bds = computeBounds(attrs, fm.stringWidth(label), fm.getAscent() + fm.getDescent(), g, label);
		if (attrs.setOffsetBounds(bds)) {
			Instance instance = painter.getInstance();
			if (instance != null)
				instance.recomputeBounds();
		}

		int x0 = bds.getX();
		int y0 = bds.getY();
		int x1 = x0 + bds.getWidth();
		int y1 = y0 + bds.getHeight();
		int mw = ARROW_MAX_WIDTH / 2;
		int[] xp;
		int[] yp;
		if (facing == Direction.North) {
			int yb = y0 + ARROW_DEPTH;
			if (x1 - x0 <= ARROW_MAX_WIDTH) {
				xp = new int[] { x0, 0, x1, x1, x0 };
				yp = new int[] { yb, y0, yb, y1, y1 };
			} else {
				xp = new int[] { x0, -mw, 0, mw, x1, x1, x0 };
				yp = new int[] { yb, yb, y0, yb, yb, y1, y1 };
			}
		} else if (facing == Direction.South) {
			int yb = y1 - ARROW_DEPTH;
			if (x1 - x0 <= ARROW_MAX_WIDTH) {
				xp = new int[] { x0, x1, x1, 0, x0 };
				yp = new int[] { y0, y0, yb, y1, yb };
			} else {
				xp = new int[] { x0, x1, x1, mw, 0, -mw, x0 };
				yp = new int[] { y0, y0, yb, yb, y1, yb, yb };
			}
		} else if (facing == Direction.East) {
			int xb = x1 - ARROW_DEPTH;
			if (y1 - y0 <= ARROW_MAX_WIDTH) {
				xp = new int[] { x0, xb, x1, xb, x0 };
				yp = new int[] { y0, y0, 0, y1, y1 };
			} else {
				xp = new int[] { x0, xb, xb, x1, xb, xb, x0 };
				yp = new int[] { y0, y0, -mw, 0, mw, y1, y1 };
			}
		} else {
			int xb = x0 + ARROW_DEPTH;
			if (y1 - y0 <= ARROW_MAX_WIDTH) {
				xp = new int[] { xb, x1, x1, xb, x0 };
				yp = new int[] { y0, y0, y1, y1, 0 };
			} else {
				xp = new int[] { xb, x1, x1, xb, xb, x0, xb };
				yp = new int[] { y0, y0, y1, y1, mw, 0, -mw };
			}
		}
		GraphicsUtil.switchToWidth(g, 2);
		g.drawPolygon(xp, yp, xp.length);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Location loc = painter.getLocation();
		int x = loc.x();
		int y = loc.y();
		Graphics g = painter.getGraphics();
		g.translate(x, y);
		g.setColor(Color.BLACK);
		paintGhost(painter);
		g.translate(-x, -y);
		painter.drawPorts();
	}

	//
	// methods for instances
	//
	@Override
	protected void configureNewInstance(Instance instance) {
		instance.addAttributeListener();
		instance.setPorts(new Port[] { new Port(0, 0, Port.INOUT, StdAttr.WIDTH) });
		configureLabel(instance);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.FACING) {
			configureLabel(instance);
			instance.recomputeBounds();
		} else if (attr == StdAttr.LABEL || attr == StdAttr.LABEL_FONT) instance.recomputeBounds();
	}

	@Override
	public void propagate(InstanceState state) {
		// nothing to do - handled by circuit
	}

	//
	// private methods
	//
	private void configureLabel(Instance instance) {
		TunnelAttributes attrs = (TunnelAttributes) instance.getAttributeSet();
		Location loc = instance.getLocation();
		instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, loc.x() + attrs.getLabelX(),
				loc.y() + attrs.getLabelY(), attrs.getLabelHAlign(), attrs.getLabelVAlign());
	}

	private Bounds computeBounds(TunnelAttributes attrs, int textWidth, int textHeight, Graphics g, String label) {
		int x = attrs.getLabelX();
		int y = attrs.getLabelY();
		int halign = attrs.getLabelHAlign();
		int valign = attrs.getLabelVAlign();

		int minDim = ARROW_MIN_WIDTH - 2 * MARGIN;
		int bw = Math.max(minDim, textWidth);
		int bh = Math.max(minDim, textHeight);
		int bx = switch (halign) {
			case TextField.H_LEFT -> x;
			case TextField.H_RIGHT -> x - bw;
			default -> x - (bw / 2);
		};
		int by = switch (valign) {
			case TextField.V_TOP -> y;
			case TextField.V_BOTTOM -> y - bh;
			default -> y - (bh / 2);
		};

		if (g != null) GraphicsUtil.drawText(g, label, bx + bw / 2, by + bh / 2, GraphicsUtil.H_CENTER,
				GraphicsUtil.V_CENTER_OVERALL);

		return Bounds.create(bx, by, bw, bh).expand(MARGIN).add(0, 0);
	}
}