// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using TextField = logisim.comp.TextField;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
    using LogisimPlus.Java;

    public class Tunnel : InstanceFactory
	{
		public static readonly Tunnel FACTORY = new Tunnel();

		internal const int MARGIN = 3;
		internal const int ARROW_MARGIN = 5;
		internal const int ARROW_DEPTH = 4;
		internal const int ARROW_MIN_WIDTH = 16;
		internal const int ARROW_MAX_WIDTH = 20;

		public Tunnel() : base("Tunnel", Strings.getter("tunnelComponent"))
		{
			IconName = "tunnel.gif";
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
		}

		public override AttributeSet createAttributeSet()
		{
			return new TunnelAttributes();
		}

		public override Bounds getOffsetBounds(AttributeSet attrsBase)
		{
			TunnelAttributes attrs = (TunnelAttributes) attrsBase;
			Bounds bds = attrs.OffsetBounds;
			if (bds != null)
			{
				return bds;
			}
			else
			{
				int ht = attrs.Font.getSize();
				int wd = ht * attrs.Label.Length / 2;
				bds = computeBounds(attrs, wd, ht, null, "");
				attrs.OffsetBounds = bds;
				return bds;
			}
		}

		//
		// JGraphics methods
		//
		public override void paintGhost(InstancePainter painter)
		{
			TunnelAttributes attrs = (TunnelAttributes) painter.AttributeSet;
			Direction facing = attrs.Facing;
			string label = attrs.Label;

			JGraphics g = painter.Graphics;
			g.setFont(attrs.Font);
			FontMetrics fm = g.getFontMetrics();
			Bounds bds = computeBounds(attrs, fm.stringWidth(label), fm.getAscent() + fm.getDescent(), g, label);
			if (attrs.setOffsetBounds(bds))
			{
				Instance instance = painter.getInstance();
				if (instance != null)
				{
					instance.recomputeBounds();
				}
			}

			int x0 = bds.X;
			int y0 = bds.Y;
			int x1 = x0 + bds.Width;
			int y1 = y0 + bds.Height;
			int mw = ARROW_MAX_WIDTH / 2;
			int[] xp;
			int[] yp;
			if (facing == Direction.North)
			{
				int yb = y0 + ARROW_DEPTH;
				if (x1 - x0 <= ARROW_MAX_WIDTH)
				{
					xp = new int[] {x0, 0, x1, x1, x0};
					yp = new int[] {yb, y0, yb, y1, y1};
				}
				else
				{
					xp = new int[] {x0, -mw, 0, mw, x1, x1, x0};
					yp = new int[] {yb, yb, y0, yb, yb, y1, y1};
				}
			}
			else if (facing == Direction.South)
			{
				int yb = y1 - ARROW_DEPTH;
				if (x1 - x0 <= ARROW_MAX_WIDTH)
				{
					xp = new int[] {x0, x1, x1, 0, x0};
					yp = new int[] {y0, y0, yb, y1, yb};
				}
				else
				{
					xp = new int[] {x0, x1, x1, mw, 0, -mw, x0};
					yp = new int[] {y0, y0, yb, yb, y1, yb, yb};
				}
			}
			else if (facing == Direction.East)
			{
				int xb = x1 - ARROW_DEPTH;
				if (y1 - y0 <= ARROW_MAX_WIDTH)
				{
					xp = new int[] {x0, xb, x1, xb, x0};
					yp = new int[] {y0, y0, 0, y1, y1};
				}
				else
				{
					xp = new int[] {x0, xb, xb, x1, xb, xb, x0};
					yp = new int[] {y0, y0, -mw, 0, mw, y1, y1};
				}
			}
			else
			{
				int xb = x0 + ARROW_DEPTH;
				if (y1 - y0 <= ARROW_MAX_WIDTH)
				{
					xp = new int[] {xb, x1, x1, xb, x0};
					yp = new int[] {y0, y0, y1, y1, 0};
				}
				else
				{
					xp = new int[] {xb, x1, x1, xb, xb, x0, xb};
					yp = new int[] {y0, y0, y1, y1, mw, 0, -mw};
				}
			}
			JGraphicsUtil.switchToWidth(g, 2);
			g.drawPolygon(xp, yp, xp.Length);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			JGraphics g = painter.Graphics;
			g.translate(x, y);
			g.setColor(Color.Black);
			paintGhost(painter);
			g.translate(-x, -y);
			painter.drawPorts();
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			instance.Ports = new Port[] { new Port(0, 0, Port.INOUT, StdAttr.Width) };
			configureLabel(instance);
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.FACING)
			{
				configureLabel(instance);
				instance.recomputeBounds();
			}
			else if (attr == StdAttr.LABEL || attr == StdAttr.LABEL_FONT)
			{
				instance.recomputeBounds();
			}
		}

		public override void propagate(InstanceState state)
		{
			; // nothing to do - handled by circuit
		}

		//
		// private methods
		//
		private void configureLabel(Instance instance)
		{
			TunnelAttributes attrs = (TunnelAttributes) instance.AttributeSet;
			Location loc = instance.Location;
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, loc.X + attrs.LabelX, loc.Y + attrs.LabelY, attrs.LabelHAlign, attrs.LabelVAlign);
		}

		private Bounds computeBounds(TunnelAttributes attrs, int textWidth, int textHeight, JGraphics g, string label)
		{
			int x = attrs.LabelX;
			int y = attrs.LabelY;
			int halign = attrs.LabelHAlign;
			int valign = attrs.LabelVAlign;

			int minDim = ARROW_MIN_WIDTH - 2 * MARGIN;
			int bw = Math.Max(minDim, textWidth);
			int bh = Math.Max(minDim, textHeight);
			int bx;
			int by;
			switch (halign)
			{
			case TextField.H_LEFT:
				bx = x;
				break;
			case TextField.H_RIGHT:
				bx = x - bw;
				break;
			default:
				bx = x - (bw / 2);
			break;
			}
			switch (valign)
			{
			case TextField.V_TOP:
				by = y;
				break;
			case TextField.V_BOTTOM:
				by = y - bh;
				break;
			default:
				by = y - (bh / 2);
			break;
			}

			if (g != null)
			{
				JGraphicsUtil.drawText(g, label, bx + bw / 2, by + bh / 2, JGraphicsUtil.H_CENTER, JGraphicsUtil.V_CENTER_OVERALL);
			}

			return Bounds.create(bx, by, bw, bh).expand(MARGIN).add(0, 0);
		}
	}
}
