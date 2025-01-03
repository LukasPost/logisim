// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit.appear
{

	using CanvasObject = draw.model.CanvasObject;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Instance = logisim.instance.Instance;
	using StdAttr = logisim.instance.StdAttr;

	internal class PortManager
	{
		private CircuitAppearance appearance;
		private bool doingUpdate;

		internal PortManager(CircuitAppearance appearance)
		{
			this.appearance = appearance;
			this.doingUpdate = false;
		}

		internal virtual void updatePorts()
		{
			appearance.recomputePorts();
		}

		internal virtual void updatePorts(HashSet<Instance> adds, HashSet<Instance> removes, Dictionary<Instance, Instance> replaces, ICollection<Instance> allPins)
		{
			if (appearance.DefaultAppearance)
			{
				appearance.recomputePorts();
			}
			else if (!doingUpdate)
			{
				// "doingUpdate" ensures infinite recursion doesn't happen
				try
				{
					doingUpdate = true;
					performUpdate(adds, removes, replaces, allPins);
					appearance.recomputePorts();
				}
				finally
				{
					doingUpdate = false;
				}
			}
		}

		private void performUpdate(HashSet<Instance> adds, HashSet<Instance> removes, Dictionary<Instance, Instance> replaces, ICollection<Instance> allPins)
		{
			// Find the current objects corresponding to pins
			Dictionary<Instance, AppearancePort> oldObjects;
			oldObjects = new Dictionary<Instance, AppearancePort>();
			AppearanceAnchor anchor = null;
			foreach (CanvasObject o in appearance.ObjectsFromBottom)
			{
				if (o is AppearancePort)
				{
					AppearancePort port = (AppearancePort) o;
					oldObjects[port.Pin] = port;
				}
				else if (o is AppearanceAnchor)
				{
					anchor = (AppearanceAnchor) o;
				}
			}

			// ensure we have the anchor in the circuit
			if (anchor == null)
			{
				foreach (CanvasObject o in DefaultAppearance.build(allPins))
				{
					if (o is AppearanceAnchor)
					{
						anchor = (AppearanceAnchor) o;
					}
				}
				if (anchor == null)
				{
					anchor = new AppearanceAnchor(new Location(100, 100));
				}
				int dest = appearance.ObjectsFromBottom.Count;
				appearance.addObjects(dest, Collections.singleton(anchor));
			}

			// Compute how the ports should change
			List<AppearancePort> portRemoves;
			portRemoves = new List<AppearancePort>(removes.Count);
			List<AppearancePort> portAdds;
			portAdds = new List<AppearancePort>(adds.Count);

			// handle removals
			foreach (Instance pin in removes)
			{
				AppearancePort port = oldObjects.Remove(pin);
				if (port != null)
				{
					portRemoves.Add(port);
				}
			}
			// handle replacements
			List<Instance> addsCopy = new List<Instance>(adds);
			foreach (KeyValuePair<Instance, Instance> entry in replaces.SetOfKeyValuePairs())
			{
				AppearancePort port = oldObjects.Remove(entry.Key);
				if (port != null)
				{
					port.Pin = entry.Value;
					oldObjects[entry.Value] = port;
				}
				else
				{ // this really shouldn't happen, but just to make sure...
					addsCopy.Add(entry.Value);
				}
			}
			// handle additions
			DefaultAppearance.sortPinList(addsCopy, Direction.East);
			// They're probably not really all facing east.
			// I'm just sorting them so it works predictably.
			foreach (Instance pin in addsCopy)
			{
				if (!oldObjects.ContainsKey(pin))
				{
					Location loc = computeDefaultLocation(appearance, pin, oldObjects);
					AppearancePort o = new AppearancePort(loc, pin);
					portAdds.Add(o);
					oldObjects[pin] = o;
				}
			}

			// Now update the appearance
			appearance.replaceAutomatically(portRemoves, portAdds);
		}

		private static Location computeDefaultLocation(CircuitAppearance appear, Instance pin, Dictionary<Instance, AppearancePort> others)
		{
			// Determine which locations are being used in canvas, and look for
			// which instances facing the same way in layout
			HashSet<Location> usedLocs = new HashSet<Location>();
			List<Instance> sameWay = new List<Instance>();
			Direction facing = (Direction)pin.getAttributeValue(StdAttr.FACING);
			foreach (KeyValuePair<Instance, AppearancePort> entry in others.SetOfKeyValuePairs())
			{
				Instance pin2 = entry.Key;
				Location loc = entry.Value.getLocation();
				usedLocs.Add(loc);
				if (pin2.getAttributeValue(StdAttr.FACING) == facing)
				{
					sameWay.Add(pin2);
				}
			}

			// if at least one faces the same way, place pin relative to that
			if (sameWay.Count > 0)
			{
				sameWay.Add(pin);
				DefaultAppearance.sortPinList(sameWay, facing);
				bool isFirst = false;
				Instance neighbor = null; // (preferably previous in map)
				foreach (Instance p in sameWay)
				{
					if (p == pin)
					{
						break;
					}
					else
					{
						neighbor = p;
					}
				}
				if (neighbor == null)
				{ // pin must have been first in list
					neighbor = sameWay[1];
				}
				int dx;
				int dy;
				if (facing == Direction.East || facing == Direction.West)
				{
					dx = 0;
					dy = isFirst ? -10 : 10;
				}
				else
				{
					dx = isFirst ? -10 : 10;
					dy = 0;
				}
				Location loc = others[neighbor].Location;
				do
				{
					loc = loc.translate(dx, dy);
				} while (usedLocs.Contains(loc));
				if (loc.X >= 0 && loc.Y >= 0)
				{
					return loc;
				}
				do
				{
					loc = loc.translate(-dx, -dy);
				} while (usedLocs.Contains(loc));
				return loc;
			}

			// otherwise place it on the boundary of the bounding rectangle
			Bounds bds = appear.AbsoluteBounds;
			int x;
			int y;
			int dx = 0;
			int dy = 0;
			if (facing == Direction.East)
			{ // on west side by default
				x = bds.X - 7;
				y = bds.Y + 5;
				dy = 10;
			}
			else if (facing == Direction.West)
			{ // on east side by default
				x = bds.X + bds.Width - 3;
				y = bds.Y + 5;
				dy = 10;
			}
			else if (facing == Direction.South)
			{ // on north side by default
				x = bds.X + 5;
				y = bds.Y - 7;
				dx = 10;
			}
			else
			{ // on south side by default
				x = bds.X + 5;
				y = bds.Y + bds.Height - 3;
				dx = 10;
			}
			x = (x + 9) / 10 * 10; // round coordinates up to ensure they're on grid
			y = (y + 9) / 10 * 10;
			Location loc = new Location(x, y);
			while (usedLocs.Contains(loc))
			{
				loc = loc.translate(dx, dy);
			}
			return loc;
		}
	}

}
