// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.move
{

	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;

	internal class AvoidanceMap
	{
		internal static AvoidanceMap create(ICollection<Component> elements, int dx, int dy)
		{
			AvoidanceMap ret = new AvoidanceMap(new Dictionary<Location, string>());
			ret.markAll(elements, dx, dy);
			return ret;
		}

		private readonly Dictionary<Location, string> avoid;

		private AvoidanceMap(Dictionary<Location, string> map)
		{
			avoid = map;
		}

		public virtual AvoidanceMap cloneMap()
		{
			return new AvoidanceMap(new Dictionary<Location, string>(avoid));
		}

		public virtual object get(Location loc)
		{
			return avoid[loc];
		}

		public virtual void markAll(ICollection<Component> elements, int dx, int dy)
		{
			// first we go through the components, saying that we should not
			// intersect with any point that lies within a component
			foreach (Component el in elements)
			{
				if (el is Wire)
				{
					markWire((Wire) el, dx, dy);
				}
				else
				{
					markComponent(el, dx, dy);
				}
			}
		}

		public virtual void markComponent(Component comp, int dx, int dy)
		{
			Dictionary<Location, string> avoid = this.avoid;
			bool translated = dx != 0 || dy != 0;
			Bounds bds = comp.Bounds;
			int x0 = bds.X + dx;
			int y0 = bds.Y + dy;
			int x1 = x0 + bds.Width;
			int y1 = y0 + bds.Height;
			x0 += 9 - (x0 + 9) % 10;
			y0 += 9 - (y0 + 9) % 10;
			for (int x = x0; x <= x1; x += 10)
			{
				for (int y = y0; y <= y1; y += 10)
				{
					Location loc = new Location(x, y);
					// loc is most likely in the component, so go ahead and
					// put it into the map as if it is - and in the rare event
					// that loc isn't in the component, we can remove it.
					string prev = avoid[loc] = Connector.ALLOW_NEITHER;
					if (!string.ReferenceEquals(prev, Connector.ALLOW_NEITHER))
					{
						Location baseLoc = translated ? loc.translate(-dx, -dy) : loc;
						if (!comp.contains(baseLoc))
						{
							if (string.ReferenceEquals(prev, null))
							{
								avoid.Remove(loc);
							}
							else
							{
								avoid[loc] = prev;
							}
						}
					}
				}
			}
		}

		public virtual void markWire(Wire w, int dx, int dy)
		{
			Dictionary<Location, string> avoid = this.avoid;
			bool translated = dx != 0 || dy != 0;
			Location loc0 = w.End0;
			Location loc1 = w.End1;
			if (translated)
			{
				loc0 = loc0.translate(dx, dy);
				loc1 = loc1.translate(dx, dy);
			}
			avoid[loc0] = Connector.ALLOW_NEITHER;
			avoid[loc1] = Connector.ALLOW_NEITHER;
			int x0 = loc0.X;
			int y0 = loc0.Y;
			int x1 = loc1.X;
			int y1 = loc1.Y;
			if (x0 == x1)
			{ // vertical wire
				foreach (Location loc in Wire.create(loc0, loc1))
				{
					object prev = avoid[loc] = Connector.ALLOW_HORIZONTAL;
					if (prev == Connector.ALLOW_NEITHER || prev == Connector.ALLOW_VERTICAL)
					{
						avoid[loc] = Connector.ALLOW_NEITHER;
					}
				}
			}
			else if (y0 == y1)
			{ // horizontal wire
				foreach (Location loc in Wire.create(loc0, loc1))
				{
					object prev = avoid[loc] = Connector.ALLOW_VERTICAL;
					if (prev == Connector.ALLOW_NEITHER || prev == Connector.ALLOW_HORIZONTAL)
					{
						avoid[loc] = Connector.ALLOW_NEITHER;
					}
				}
			}
			else
			{ // diagonal - shouldn't happen
				throw new Exception("diagonal wires not supported");
			}
		}

		public virtual void unmarkLocation(Location loc)
		{
			avoid.Remove(loc);
		}

		public virtual void unmarkWire(Wire w, Location deletedEnd, ISet<Location> unmarkable)
		{
			Location loc0 = w.End0;
			Location loc1 = w.End1;
			if (unmarkable == null || unmarkable.Contains(deletedEnd))
			{
				avoid.Remove(deletedEnd);
			}
			int x0 = loc0.X;
			int y0 = loc0.Y;
			int x1 = loc1.X;
			int y1 = loc1.Y;
			if (x0 == x1)
			{ // vertical wire
				foreach (Location loc in w)
				{
					if (unmarkable == null || unmarkable.Contains(deletedEnd))
					{
						object prev = avoid.Remove(loc);
						if (prev != Connector.ALLOW_HORIZONTAL && prev != null)
						{
							avoid[loc] = Connector.ALLOW_VERTICAL;
						}
					}
				}
			}
			else if (y0 == y1)
			{ // horizontal wire
				foreach (Location loc in w)
				{
					if (unmarkable == null || unmarkable.Contains(deletedEnd))
					{
						object prev = avoid.Remove(loc);
						if (prev != Connector.ALLOW_VERTICAL && prev != null)
						{
							avoid[loc] = Connector.ALLOW_HORIZONTAL;
						}
					}
				}
			}
			else
			{ // diagonal - shouldn't happen
				throw new Exception("diagonal wires not supported");
			}
		}

		public virtual void print(PrintStream stream)
		{
			List<Location> list = new List<Location>(avoid.Keys);
			list.Sort();
			for (int i = 0, n = list.Count; i < n; i++)
			{
				stream.println(list[i] + ": " + avoid[list[i]]);
			}
		}
	}

}
