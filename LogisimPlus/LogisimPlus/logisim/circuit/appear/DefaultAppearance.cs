// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit.appear
{

	using CanvasObject = draw.model.CanvasObject;
	using Curve = draw.shapes.Curve;
	using DrawAttr = draw.shapes.DrawAttr;
	using Rectangle = draw.shapes.Rectangle;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Instance = logisim.instance.Instance;
	using StdAttr = logisim.instance.StdAttr;

	internal class DefaultAppearance
	{
		private const int OFFS = 50;

		private DefaultAppearance()
		{
		}

		private class CompareLocations : IComparer<Instance>
		{
			internal bool byX;

			internal CompareLocations(bool byX)
			{
				this.byX = byX;
			}

			public virtual int Compare(Instance a, Instance b)
			{
				Location aloc = a.Location;
				Location bloc = b.Location;
				if (byX)
				{
					int ax = aloc.X;
					int bx = bloc.X;
					if (ax != bx)
					{
						return ax < bx ? -1 : 1;
					}
				}
				else
				{
					int ay = aloc.Y;
					int by = bloc.Y;
					if (ay != by)
					{
						return ay < by ? -1 : 1;
					}
				}
				return aloc.CompareTo(bloc);
			}
		}

		internal static void sortPinList(IList<Instance> pins, Direction facing)
		{
			if (facing == Direction.North || facing == Direction.South)
			{
				IComparer<Instance> sortHorizontal = new CompareLocations(true);
				pins.Sort(sortHorizontal);
			}
			else
			{
				IComparer<Instance> sortVertical = new CompareLocations(false);
				pins.Sort(sortVertical);
			}
		}

		public static IList<CanvasObject> build(ICollection<Instance> pins)
		{
			IDictionary<Direction, IList<Instance>> edge;
			edge = new Dictionary<Direction, IList<Instance>>();
			edge[Direction.North] = new List<Instance>();
			edge[Direction.South] = new List<Instance>();
			edge[Direction.East] = new List<Instance>();
			edge[Direction.West] = new List<Instance>();
			foreach (Instance pin in pins)
			{
				Direction pinFacing = pin.getAttributeValue(StdAttr.FACING);
				Direction pinEdge = pinFacing.reverse();
				IList<Instance> e = edge[pinEdge];
				e.Add(pin);
			}

			foreach (KeyValuePair<Direction, IList<Instance>> entry in edge.SetOfKeyValuePairs())
			{
				sortPinList(entry.Value, entry.Key);
			}

			int numNorth = edge[Direction.North].Count;
			int numSouth = edge[Direction.South].Count;
			int numEast = edge[Direction.East].Count;
			int numWest = edge[Direction.West].Count;
			int maxVert = Math.Max(numNorth, numSouth);
			int maxHorz = Math.Max(numEast, numWest);

			int offsNorth = computeOffset(numNorth, numSouth, maxHorz);
			int offsSouth = computeOffset(numSouth, numNorth, maxHorz);
			int offsEast = computeOffset(numEast, numWest, maxVert);
			int offsWest = computeOffset(numWest, numEast, maxVert);

			int width = computeDimension(maxVert, maxHorz);
			int height = computeDimension(maxHorz, maxVert);

			// compute position of anchor relative to top left corner of box
			int ax;
			int ay;
			if (numEast > 0)
			{ // anchor is on east side
				ax = width;
				ay = offsEast;
			}
			else if (numNorth > 0)
			{ // anchor is on north side
				ax = offsNorth;
				ay = 0;
			}
			else if (numWest > 0)
			{ // anchor is on west side
				ax = 0;
				ay = offsWest;
			}
			else if (numSouth > 0)
			{ // anchor is on south side
				ax = offsSouth;
				ay = height;
			}
			else
			{ // anchor is top left corner
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
			notch.setValue(DrawAttr.STROKE_WIDTH, Convert.ToInt32(2));
			notch.setValue(DrawAttr.STROKE_COLOR, Color.GRAY);
			Rectangle rect = new Rectangle(rx, ry, width, height);
			rect.setValue(DrawAttr.STROKE_WIDTH, Convert.ToInt32(2));

			IList<CanvasObject> ret = new List<CanvasObject>();
			ret.Add(notch);
			ret.Add(rect);
			placePins(ret, edge[Direction.West], rx, ry + offsWest, 0, 10);
			placePins(ret, edge[Direction.East], rx + width, ry + offsEast, 0, 10);
			placePins(ret, edge[Direction.North], rx + offsNorth, ry, 10, 0);
			placePins(ret, edge[Direction.South], rx + offsSouth, ry + height, 10, 0);
			ret.Add(new AppearanceAnchor(new Location(rx + ax, ry + ay)));
			return ret;
		}

		private static int computeDimension(int maxThis, int maxOthers)
		{
			if (maxThis < 3)
			{
				return 30;
			}
			else if (maxOthers == 0)
			{
				return 10 * maxThis;
			}
			else
			{
				return 10 * maxThis + 10;
			}
		}

		private static int computeOffset(int numFacing, int numOpposite, int maxOthers)
		{
			int maxThis = Math.Max(numFacing, numOpposite);
			int maxOffs;
			switch (maxThis)
			{
			case 0:
			case 1:
				maxOffs = (maxOthers == 0 ? 15 : 10);
				break;
			case 2:
				maxOffs = 10;
				break;
			default:
				maxOffs = (maxOthers == 0 ? 5 : 10);
			break;
			}
			return maxOffs + 10 * ((maxThis - numFacing) / 2);
		}

		private static void placePins(IList<CanvasObject> dest, IList<Instance> pins, int x, int y, int dx, int dy)
		{
			foreach (Instance pin in pins)
			{
				dest.Add(new AppearancePort(new Location(x, y), pin));
				x += dx;
				y += dy;
			}
		}
	}

}
