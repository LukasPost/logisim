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

	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using InstancePainter = logisim.instance.InstancePainter;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	public class PainterShaped
	{
		private static readonly GeneralPath PATH_NARROW;
		private static readonly GeneralPath PATH_MEDIUM;
		private static readonly GeneralPath PATH_WIDE;

		private static readonly GeneralPath SHIELD_NARROW;
		private static readonly GeneralPath SHIELD_MEDIUM;
		private static readonly GeneralPath SHIELD_WIDE;

		static PainterShaped()
		{
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

		private PainterShaped()
		{
		}

		private static Dictionary<int, int[]> INPUT_LENGTHS = new Dictionary<int, int[]>();

		internal static void paintAnd(InstancePainter painter, int width, int height)
		{
			JGraphics g = painter.Graphics;
			JGraphicsUtil.switchToWidth(g, 2);
			int[] xp = new int[] {-width / 2, -width + 1, -width + 1, -width / 2};
			int[] yp = new int[] {-width / 2, -width / 2, width / 2, width / 2};
			JGraphicsUtil.drawCenteredArc(g, -width / 2, 0, width / 2, -90, 180);

			g.drawPolyline(xp, yp, 4);
			if (height > width)
			{
				g.drawLine(-width + 1, -height / 2, -width + 1, height / 2);
			}
		}

		internal static void paintOr(InstancePainter painter, int width, int height)
		{
			JGraphics g = painter.Graphics;
			JGraphicsUtil.switchToWidth(g, 2);
			/*
			 * The following, used previous to version 2.5.1, didn't use GeneralPath g.setColor(Color.LIGHT_GRAY); if (width
			 * < 40) { JGraphicsUtil.drawCenteredArc(g, -30, -21, 36, -90, 53); JGraphicsUtil.drawCenteredArc(g, -30, 21, 36,
			 * 90, -53); } else if (width < 60) { JGraphicsUtil.drawCenteredArc(g, -50, -37, 62, -90, 53);
			 * JGraphicsUtil.drawCenteredArc(g, -50, 37, 62, 90, -53); } else { JGraphicsUtil.drawCenteredArc(g, -70, -50, 85,
			 * -90, 53); JGraphicsUtil.drawCenteredArc(g, -70, 50, 85, 90, -53); } paintShield(g, -width, 0, width, height);
			 */

			GeneralPath path;
			if (width < 40)
			{
				path = PATH_NARROW;
			}
			else if (width < 60)
			{
				path = PATH_MEDIUM;
			}
			else
			{
				path = PATH_WIDE;
			}
			g.draw(path);
			if (height > width)
			{
				paintShield(g, 0, width, height);
			}
		}

		internal static void paintNot(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			JGraphicsUtil.switchToWidth(g, 2);
			if (painter.getAttributeValue(NotGate.ATTR_SIZE) == NotGate.SIZE_NARROW)
			{
				JGraphicsUtil.switchToWidth(g, 2);
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
			}
			else
			{
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

		internal static void paintXor(InstancePainter painter, int width, int height)
		{
			JGraphics g = painter.Graphics;
			paintOr(painter, width - 10, width - 10);
			paintShield(g, -10, width - 10, height);
		}

		private static void paintShield(JGraphics g, int xlate, int width, int height)
		{
			JGraphicsUtil.switchToWidth(g, 2);
			g.translate(xlate, 0);
			g.draw(computeShield(width, height));
			g.translate(-xlate, 0);

			/*
			 * The following, used previous to version 2.5.1, didn't use GeneralPath if (width < 40) {
			 * JGraphicsUtil.drawCenteredArc(g, x - 26, y, 30, -30, 60); } else if (width < 60) {
			 * JGraphicsUtil.drawCenteredArc(g, x - 43, y, 50, -30, 60); } else { JGraphicsUtil.drawCenteredArc(g, x - 60, y,
			 * 70, -30, 60); } if (height > width) { // we need to draw the shield JGraphicsUtil.drawCenteredArc(g, x - dx, y
			 * - (width + extra) / 2, extra, -30, 60); JGraphicsUtil.drawCenteredArc(g, x - dx, y + (width + extra) / 2,
			 * extra, -30, 60); }
			 */
		}

		private static GeneralPath computeShield(int width, int height)
		{
			GeneralPath @base;
			if (width < 40)
			{
				@base = SHIELD_NARROW;
			}
			else if (width < 60)
			{
				@base = SHIELD_MEDIUM;
			}
			else
			{
				@base = SHIELD_WIDE;
			}

			if (height <= width)
			{ // no wings
				return @base;
			}
			else
			{ // we need to add wings
				int wingHeight = (height - width) / 2;
				int dx = Math.Min(20, wingHeight / 4);

				GeneralPath path = new GeneralPath();
				path.moveTo(-width, -height / 2);
				path.quadTo(-width + dx, -(width + height) / 4, -width, -width / 2);
				path.append(@base, true);
				path.quadTo(-width + dx, (width + height) / 4, -width, height / 2);
				return path;
			}
		}

		internal static void paintInputLines(InstancePainter painter, AbstractGate factory)
		{
			Location loc = painter.Location;
			bool printView = painter.PrintView;
			GateAttributes attrs = (GateAttributes) painter.AttributeSet;
			Direction facing = attrs.facing;
			int inputs = attrs.inputs;
			int negated = attrs.negated;

			int[] lengths = getInputLineLengths(attrs, factory);
			if (painter.getInstance() == null)
			{ // drawing ghost - negation bubbles only
				for (int i = 0; i < inputs; i++)
				{
					bool iNegated = ((negated >> i) & 1) == 1;
					if (iNegated)
					{
						Location offs = factory.getInputOffset(attrs, i);
						Location loci = loc.translate(offs.X, offs.Y);
						Location cent = loci.translate(facing, lengths[i] + 5);
						painter.drawDongle(cent.X, cent.Y);
					}
				}
			}
			else
			{
				JGraphics g = painter.Graphics;
				Color baseColor = g.getColor();
				JGraphicsUtil.switchToWidth(g, 3);
				for (int i = 0; i < inputs; i++)
				{
					Location offs = factory.getInputOffset(attrs, i);
					Location src = loc.translate(offs.X, offs.Y);
					int len = lengths[i];
					if (len != 0 && (!printView || painter.isPortConnected(i + 1)))
					{
						if (painter.ShowState)
						{
							Value val = painter.getPort(i + 1);
							g.setColor(val.Color);
						}
						else
						{
							g.setColor(baseColor);
						}
						Location dst = src.translate(facing, len);
						g.drawLine(src.X, src.Y, dst.X, dst.Y);
					}
					if (((negated >> i) & 1) == 1)
					{
						Location cent = src.translate(facing, lengths[i] + 5);
						g.setColor(baseColor);
						painter.drawDongle(cent.X, cent.Y);
						JGraphicsUtil.switchToWidth(g, 3);
					}
				}
			}
		}

		private static int[] getInputLineLengths(GateAttributes attrs, AbstractGate factory)
		{
			int inputs = attrs.inputs;
			int mainHeight = ((int?) attrs.size.Value).Value;
			int? key = Convert.ToInt32(inputs * 31 + mainHeight);
			object ret = INPUT_LENGTHS[key];
			if (ret != null)
			{
				return (int[]) ret;
			}

			Direction facing = attrs.facing;
			if (facing != Direction.East)
			{
				attrs = (GateAttributes) attrs.clone();
				attrs.facing = Direction.East;
			}

			int[] lengths = new int[inputs];
			INPUT_LENGTHS[key] = lengths;
			int width = mainHeight;
			Location loc0 = OrGate.FACTORY.getInputOffset(attrs, 0);
			Location locn = OrGate.FACTORY.getInputOffset(attrs, inputs - 1);
			int totalHeight = 10 + loc0.manhattanDistanceTo(locn);
			if (totalHeight < width)
			{
				totalHeight = width;
			}

			GeneralPath path = computeShield(width, totalHeight);
			for (int i = 0; i < inputs; i++)
			{
				Location loci = OrGate.FACTORY.getInputOffset(attrs, i);
				Point2D p = new Point2D.Float(loci.X + 1, loci.Y);
				int iters = 0;
				while (path.contains(p) && iters < 15)
				{
					iters++;
					p.setLocation(p.getX() + 1, p.getY());
				}
				if (iters >= 15)
				{
					iters = 0;
				}
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

}
