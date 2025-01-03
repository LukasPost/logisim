// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using InstancePainter = logisim.instance.InstancePainter;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	internal class PainterDin
	{
		private PainterDin()
		{
		}

		internal const int AND = 0;
		internal const int OR = 1;
		internal const int XOR = 2;
		internal const int XNOR = 3;

		private static Dictionary<int, int[]> orLenArrays = new Dictionary<int, int[]>();

		internal static void paintAnd(InstancePainter painter, int width, int height, bool drawBubble)
		{
			paint(painter, width, height, drawBubble, AND);
		}

		internal static void paintOr(InstancePainter painter, int width, int height, bool drawBubble)
		{
			paint(painter, width, height, drawBubble, OR);
		}

		internal static void paintXor(InstancePainter painter, int width, int height, bool drawBubble)
		{
			paint(painter, width, height, drawBubble, XOR);
		}

		internal static void paintXnor(InstancePainter painter, int width, int height, bool drawBubble)
		{
			paint(painter, width, height, drawBubble, XNOR);
		}

		private static void paint(InstancePainter painter, int width, int height, bool drawBubble, int dinType)
		{
			JGraphics g = painter.Graphics;
			int xMid = -width;
			int y0 = -height / 2;
			if (drawBubble)
			{
				width -= 8;
			}
			int diam = Math.Min(height, 2 * width);
			if (dinType == AND)
			{
				; // nothing to do
			}
			else if (dinType == OR)
			{
				paintOrLines(painter, width, height, drawBubble);
			}
			else if (dinType == XOR || dinType == XNOR)
			{
				int elen = Math.Min(diam / 2 - 10, 20);
				int ex0 = xMid + (diam / 2 - elen) / 2;
				int ex1 = ex0 + elen;
				g.drawLine(ex0, -5, ex1, -5);
				g.drawLine(ex0, 0, ex1, 0);
				g.drawLine(ex0, 5, ex1, 5);
				if (dinType == XOR)
				{
					int exMid = ex0 + elen / 2;
					g.drawLine(exMid, -8, exMid, 8);
				}
			}
			else
			{
				throw new System.ArgumentException("unrecognized shape");
			}

			JGraphicsUtil.switchToWidth(g, 2);
			int x0 = xMid - diam / 2;
			Color oldColor = g.getColor();
			if (painter.ShowState)
			{
				Value val = painter.getPort(0);
				g.setColor(val.Color);
			}
			g.drawLine(x0 + diam, 0, 0, 0);
			g.setColor(oldColor);
			if (height <= diam)
			{
				g.drawArc(x0, y0, diam, diam, -90, 180);
			}
			else
			{
				int x1 = x0 + diam;
				int yy0 = -(height - diam) / 2;
				int yy1 = (height - diam) / 2;
				g.drawArc(x0, y0, diam, diam, 0, 90);
				g.drawLine(x1, yy0, x1, yy1);
				g.drawArc(x0, y0 + height - diam, diam, diam, -90, 90);
			}
			g.drawLine(xMid, y0, xMid, y0 + height);
			if (drawBubble)
			{
				g.fillOval(x0 + diam - 4, -4, 8, 8);
				xMid += 4;
			}
		}

		private static void paintOrLines(InstancePainter painter, int width, int height, bool hasBubble)
		{
			GateAttributes baseAttrs = (GateAttributes) painter.AttributeSet;
			int inputs = baseAttrs.inputs;
			GateAttributes attrs = (GateAttributes) OrGate.FACTORY.createAttributeSet();
			attrs.inputs = inputs;
			attrs.size = baseAttrs.size;

			JGraphics g = painter.Graphics;
			// draw state if appropriate
			// ignore lines if in print view
			int r = Math.Min(height / 2, width);
			int? hash = Convert.ToInt32(r << 4 | inputs);
			int[] lens = orLenArrays[hash];
			if (lens == null)
			{
				lens = new int[inputs];
				orLenArrays[hash] = lens;
				int yCurveStart = height / 2 - r;
				for (int i = 0; i < inputs; i++)
				{
					int y = OrGate.FACTORY.getInputOffset(attrs, i).Y;
					if (y < 0)
					{
						y = -y;
					}
					if (y <= yCurveStart)
					{
						lens[i] = r;
					}
					else
					{
						int dy = y - yCurveStart;
						lens[i] = (int)(Math.Sqrt(r * r - dy * dy) + 0.5);
					}
				}
			}

			AbstractGate factory = hasBubble ? (logisim.std.gates.AbstractGate)NorGate.FACTORY : OrGate.FACTORY;
			bool printView = painter.PrintView && painter.getInstance() != null;
			JGraphicsUtil.switchToWidth(g, 2);
			for (int i = 0; i < inputs; i++)
			{
				if (!printView || painter.isPortConnected(i))
				{
					Location loc = factory.getInputOffset(attrs, i);
					int x = loc.X;
					int y = loc.Y;
					g.drawLine(x, y, x + lens[i], y);
				}
			}
		}
	}

}
