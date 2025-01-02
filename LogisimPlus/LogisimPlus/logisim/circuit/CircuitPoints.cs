// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using Component = logisim.comp.Component;
	using EndData = logisim.comp.EndData;
	using BitWidth = logisim.data.BitWidth;
	using Location = logisim.data.Location;

	internal class CircuitPoints
	{
		private class LocationData
		{
			internal BitWidth width = BitWidth.UNKNOWN;
			internal List<Component> components = new List<Component>(4);
			internal List<EndData> ends = new List<EndData>(4);
			// these lists are parallel - ends corresponding to wires are null
		}

		private Dictionary<Location, LocationData> map = new Dictionary<Location, LocationData>();
		private Dictionary<Location, WidthIncompatibilityData> incompatibilityData = new Dictionary<Location, WidthIncompatibilityData>();

		public CircuitPoints()
		{
		}

		//
		// access methods
		//
		internal virtual ISet<Location> SplitLocations
		{
			get
			{
				return map.Keys;
			}
		}

		internal virtual BitWidth getWidth(Location loc)
		{
			LocationData locData = map[loc];
			return locData == null ? BitWidth.UNKNOWN : locData.width;
		}

		internal virtual int getComponentCount(Location loc)
		{
			LocationData locData = map[loc];
			return locData == null ? 0 : locData.components.Count;
		}

		internal virtual Component getExclusive(Location loc)
		{
			LocationData locData = map[loc];
			if (locData == null)
			{
				return null;
			}
			int i = -1;
			foreach (EndData endData in locData.ends)
			{
				i++;
				if (endData != null && endData.Exclusive)
				{
					return locData.components[i];
				}
			}
			return null;
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Collection<? extends logisim.comp.Component> getComponents(logisim.data.Location loc)
		internal virtual ICollection<Component> getComponents(Location loc)
		{
			LocationData locData = map[loc];
			if (locData == null)
			{
				return Collections.emptySet();
			}
			else
			{
				return locData.components;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Collection<? extends logisim.comp.Component> getSplitCauses(logisim.data.Location loc)
		internal virtual ICollection<Component> getSplitCauses(Location loc)
		{
			return getComponents(loc);
		}

		internal virtual ICollection<Wire> getWires(Location loc)
		{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Collection<Wire> ret = (java.util.Collection<Wire>) find(loc, true);
			ICollection<Wire> ret = (ICollection<Wire>) find(loc, true);
			return ret;
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Collection<? extends logisim.comp.Component> getNonWires(logisim.data.Location loc)
		internal virtual ICollection<Component> getNonWires(Location loc)
		{
			return find(loc, false);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.Collection<? extends logisim.comp.Component> find(logisim.data.Location loc, boolean isWire)
		private ICollection<Component> find(Location loc, bool isWire)
		{
			LocationData locData = map[loc];
			if (locData == null)
			{
				return Collections.emptySet();
			}

			// first see how many elements we have; we can handle some simple
			// cases without creating any new lists
			List<Component> list = locData.components;
			int retSize = 0;
			Component retValue = null;
			foreach (Component o in list)
			{
				if ((o is Wire) == isWire)
				{
					retValue = o;
					retSize++;
				}
			}
			if (retSize == list.Count)
			{
				return list;
			}
			if (retSize == 0)
			{
				return Collections.emptySet();
			}
			if (retSize == 1)
			{
				return Collections.singleton(retValue);
			}

			// otherwise we have to create our own list
			Component[] ret = new Component[retSize];
			int retPos = 0;
			foreach (Component o in list)
			{
				if ((o is Wire) == isWire)
				{
					ret[retPos] = o;
					retPos++;
				}
			}
			return ret;
		}

		internal virtual ICollection<WidthIncompatibilityData> WidthIncompatibilityData
		{
			get
			{
				return incompatibilityData.Values;
			}
		}

		internal virtual bool hasConflict(Component comp)
		{
			if (comp is Wire)
			{
				return false;
			}
			else
			{
				foreach (EndData endData in comp.Ends)
				{
					if (endData != null && endData.Exclusive && getExclusive(endData.Location) != null)
					{
						return true;
					}
				}
				return false;
			}
		}

		//
		// update methods
		//
		internal virtual void add(Component comp)
		{
			if (comp is Wire)
			{
				Wire w = (Wire) comp;
				addSub(w.End0, w, null);
				addSub(w.End1, w, null);
			}
			else
			{
				foreach (EndData endData in comp.Ends)
				{
					if (endData != null)
					{
						addSub(endData.Location, comp, endData);
					}
				}
			}
		}

		internal virtual void add(Component comp, EndData endData)
		{
			if (endData != null)
			{
				addSub(endData.Location, comp, endData);
			}
		}

		internal virtual void remove(Component comp)
		{
			if (comp is Wire)
			{
				Wire w = (Wire) comp;
				removeSub(w.End0, w);
				removeSub(w.End1, w);
			}
			else
			{
				foreach (EndData endData in comp.Ends)
				{
					if (endData != null)
					{
						removeSub(endData.Location, comp);
					}
				}
			}
		}

		internal virtual void remove(Component comp, EndData endData)
		{
			if (endData != null)
			{
				removeSub(endData.Location, comp);
			}
		}

		private void addSub(Location loc, Component comp, EndData endData)
		{
			LocationData locData = map[loc];
			if (locData == null)
			{
				locData = new LocationData();
				map[loc] = locData;
			}
			locData.components.Add(comp);
			locData.ends.Add(endData);
			computeIncompatibilityData(loc, locData);
		}

		private void removeSub(Location loc, Component comp)
		{
			LocationData locData = map[loc];
			if (locData == null)
			{
				return;
			}

			int index = locData.components.IndexOf(comp);
			if (index < 0)
			{
				return;
			}

			if (locData.components.Count == 1)
			{
				map.Remove(loc);
				incompatibilityData.Remove(loc);
			}
			else
			{
				locData.components.RemoveAt(index);
				locData.ends.RemoveAt(index);
				computeIncompatibilityData(loc, locData);
			}
		}

		private void computeIncompatibilityData(Location loc, LocationData locData)
		{
			WidthIncompatibilityData error = null;
			if (locData != null)
			{
				BitWidth width = BitWidth.UNKNOWN;
				foreach (EndData endData in locData.ends)
				{
					if (endData != null)
					{
						BitWidth endWidth = endData.Width;
						if (width == BitWidth.UNKNOWN)
						{
							width = endWidth;
						}
						else if (width != endWidth && endWidth != BitWidth.UNKNOWN)
						{
							if (error == null)
							{
								error = new WidthIncompatibilityData();
								error.add(loc, width);
							}
							error.add(loc, endWidth);
						}
					}
				}
				locData.width = width;
			}

			if (error == null)
			{
				incompatibilityData.Remove(loc);
			}
			else
			{
				incompatibilityData[loc] = error;
			}
		}

	}

}
