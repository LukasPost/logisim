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

	internal class WireRepair : CircuitTransaction
	{

		private class MergeSets
		{
			internal readonly Dictionary<Wire, List<Wire>> map;

			internal MergeSets()
			{
				map = new Dictionary<Wire, List<Wire>>();
			}

			internal virtual void merge(Wire a, Wire b)
			{
				List<Wire> set0 = map[a];
				List<Wire> set1 = map[b];
				if (set0 == set1)
				{
					return;
				}

				if (set0 != null && set1 != null)
				{
					if (set0.Count > set1.Count)
					{ // ensure set1 is the larger
						List<Wire> temp = set0;
						set0 = set1;
						set1 = temp;
					}
					set1.AddRange(set0);
					foreach (Wire w in set0)
					{
						map[w] = set1;
					}

				}
				else if (set0 == null && set1 != null)
				{
					set1.Add(a);
					map[a] = set1;
				}
				else if (set0 != null && set1 == null)
				{
					set0.Add(b);
					map[b] = set0;
				}
				else
				{
					set0 = new List<Wire>(2);
					set0.Add(a);
					set0.Add(b);
					map[a] = set0;
					map[b] = set0;
				}
			}

			internal virtual ICollection<List<Wire>> MergeSets
			{
				get
				{
					IdentityHashMap<List<Wire>, bool> lists;
					lists = new IdentityHashMap<List<Wire>, bool>();
					foreach (List<Wire> list in map.Values)
					{
						lists.put(list, true);
					}
    
					return lists.keySet();
				}
			}
		}

		private Circuit circuit;

		public WireRepair(Circuit circuit)
		{
			this.circuit = circuit;
		}

		protected internal override IDictionary<Circuit, int> AccessedCircuits
		{
			get
			{
				return Collections.singletonMap(circuit, READ_WRITE);
			}
		}

		protected internal override void run(CircuitMutator mutator)
		{
			doMerges(mutator);
			doOverlaps(mutator);
			doSplits(mutator);
		}

		/*
		 * for debugging: private void printWires(String prefix, PrintStream out) {
		 * boolean first = true; for (Wire w :
		 * circuit.getWires()) { if (first) { out.println(prefix + ": " + w); first =
		 * false; } else { out.println("      " +
		 * w); } } out.println(prefix + ": none"); }
		 */

		private void doMerges(CircuitMutator mutator)
		{
			MergeSets sets = new MergeSets();
			foreach (Location loc in circuit.wires.points.getSplitLocations())
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Collection<?> at = circuit.getComponents(loc);
				ICollection<object> at = circuit.getComponents(loc);
				if (at.Count == 2)
				{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Iterator<?> atit = at.iterator();
					IEnumerator<object> atit = at.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					object at0 = atit.next();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					object at1 = atit.next();
					if (at0 is Wire && at1 is Wire)
					{
						Wire w0 = (Wire) at0;
						Wire w1 = (Wire) at1;
						if (w0.isParallel(w1))
						{
							sets.merge(w0, w1);
						}
					}
				}
			}

			ReplacementMap repl = new ReplacementMap();
			foreach (List<Wire> mergeSet in sets.MergeSets)
			{
				if (mergeSet.Count > 1)
				{
					List<Location> locs = new List<Location>(2 * mergeSet.Count);
					foreach (Wire w in mergeSet)
					{
						locs.Add(w.End0);
						locs.Add(w.End1);
					}
					locs.Sort();
					Location e0 = locs[0];
					Location e1 = locs[locs.Count - 1];
					Wire wnew = Wire.create(e0, e1);
					ICollection<Wire> wset = Collections.singleton(wnew);

					foreach (Wire w in mergeSet)
					{
						if (!w.Equals(wset))
						{
							repl.put(w, wset);
						}
					}
				}
			}
			mutator.replace(circuit, repl);
		}

		private void doOverlaps(CircuitMutator mutator)
		{
			Dictionary<Location, List<Wire>> wirePoints;
			wirePoints = new Dictionary<Location, List<Wire>>();
			foreach (Wire w in circuit.Wires)
			{
				foreach (Location loc in w)
				{
					List<Wire> locWires = wirePoints[loc];
					if (locWires == null)
					{
						locWires = new List<Wire>(3);
						wirePoints[loc] = locWires;
					}
					locWires.Add(w);
				}
			}

			MergeSets mergeSets = new MergeSets();
			foreach (List<Wire> locWires in wirePoints.Values)
			{
				if (locWires.Count > 1)
				{
					for (int i = 0, n = locWires.Count; i < n; i++)
					{
						Wire w0 = locWires[i];
						for (int j = i + 1; j < n; j++)
						{
							Wire w1 = locWires[j];
							if (w0.overlaps(w1, false))
							{
								mergeSets.merge(w0, w1);
							}
						}
					}
				}
			}

			ReplacementMap replacements = new ReplacementMap();
			ISet<Location> splitLocs = circuit.wires.points.getSplitLocations();
			foreach (List<Wire> mergeSet in mergeSets.MergeSets)
			{
				if (mergeSet.Count > 1)
				{
					doMergeSet(mergeSet, replacements, splitLocs);
				}
			}
			mutator.replace(circuit, replacements);
		}

		private void doMergeSet(List<Wire> mergeSet, ReplacementMap replacements, ISet<Location> splitLocs)
		{
			SortedSet<Location> ends = new SortedSet<Location>();
			foreach (Wire w in mergeSet)
			{
				ends.Add(w.End0);
				ends.Add(w.End1);
			}
			Wire whole = Wire.create(ends.First, ends.Last);

			SortedSet<Location> mids = new SortedSet<Location>();
			mids.Add(whole.End0);
			mids.Add(whole.End1);
			foreach (Location loc in whole)
			{
				if (splitLocs.Contains(loc))
				{
					foreach (Component comp in circuit.getComponents(loc))
					{
						if (!mergeSet.Contains(comp))
						{
							mids.Add(loc);
							break;
						}
					}
				}
			}

			List<Wire> mergeResult = new List<Wire>();
			if (mids.Count == 2)
			{
				mergeResult.Add(whole);
			}
			else
			{
				Location e0 = mids.First;
				foreach (Location e1 in mids)
				{
					mergeResult.Add(Wire.create(e0, e1));
					e0 = e1;
				}
			}

			foreach (Wire w in mergeSet)
			{
				List<Component> wRepl = new List<Component>(2);
				foreach (Wire w2 in mergeResult)
				{
					if (w2.overlaps(w, false))
					{
						wRepl.Add(w2);
					}
				}
				replacements.put(w, wRepl);
			}
		}

		private void doSplits(CircuitMutator mutator)
		{
			ISet<Location> splitLocs = circuit.wires.points.getSplitLocations();
			ReplacementMap repl = new ReplacementMap();
			foreach (Wire w in circuit.Wires)
			{
				Location w0 = w.End0;
				Location w1 = w.End1;
				List<Location> splits = null;
				foreach (Location loc in splitLocs)
				{
					if (w.contains(loc) && !loc.Equals(w0) && !loc.Equals(w1))
					{
						if (splits == null)
						{
							splits = new List<Location>();
						}
						splits.Add(loc);
					}
				}
				if (splits != null)
				{
					splits.Add(w1);
					splits.Sort();
					Location e0 = w0;
					List<Wire> subs = new List<Wire>(splits.Count);
					foreach (Location e1 in splits)
					{
						subs.Add(Wire.create(e0, e1));
						e0 = e1;
					}
					repl.put(w, subs);
				}
			}
			mutator.replace(circuit, repl);
		}
	}

}
