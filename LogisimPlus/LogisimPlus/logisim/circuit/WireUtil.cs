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
	using Location = logisim.data.Location;

	public class WireUtil
	{
		private WireUtil()
		{
		}

		internal static CircuitPoints computeCircuitPoints<T1>(ICollection<T1> components) where T1 : logisim.comp.Component
		{
			CircuitPoints points = new CircuitPoints();
			foreach (Component comp in components)
			{
				points.add(comp);
			}
			return points;
		}

		// Merge all parallel endpoint-to-endpoint wires within the given set.
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public static java.util.Collection<? extends logisim.comp.Component> mergeExclusive(java.util.Collection<? extends logisim.comp.Component> toMerge)
		public static ICollection<Component> mergeExclusive<T1>(ICollection<T1> toMerge) where T1 : logisim.comp.Component
		{
			if (toMerge.Count <= 1)
			{
				return toMerge;
			}

			HashSet<Component> ret = new HashSet<Component>(toMerge);
			CircuitPoints points = computeCircuitPoints(toMerge);

			HashSet<Wire> wires = new HashSet<Wire>();
			foreach (Location loc in points.SplitLocations)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Collection<? extends logisim.comp.Component> at = points.getComponents(loc);
				ICollection<Component> at = points.getComponents(loc);
				if (at.Count == 2)
				{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Iterator<? extends logisim.comp.Component> atIt = at.iterator();
					IEnumerator<Component> atIt = at.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Component o0 = atIt.next();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Component o1 = atIt.next();
					if (o0 is Wire && o1 is Wire)
					{
						Wire w0 = (Wire) o0;
						Wire w1 = (Wire) o1;
						if (w0.is_x_equal == w1.is_x_equal)
						{
							wires.Add(w0);
							wires.Add(w1);
						}
					}
				}
			}
			points = null;

			ret.RemoveAll(wires);
			while (wires.Count > 0)
			{
				IEnumerator<Wire> it = wires.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Wire w = it.next();
				Location e0 = w.e0;
				Location e1 = w.e1;
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
				it.remove();
				bool found;
				do
				{
					found = false;
					for (it = wires.GetEnumerator(); it.MoveNext();)
					{
						Wire cand = it.Current;
						if (cand.e0.Equals(e1))
						{
							e1 = cand.e1;
							found = true;
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
							it.remove();
						}
						else if (cand.e1.Equals(e0))
						{
							e0 = cand.e0;
							found = true;
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
							it.remove();
						}
					}
				} while (found);
				ret.Add(Wire.create(e0, e1));
			}

			return ret;
		}
	}

}
