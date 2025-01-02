// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{

	using Circuit = logisim.circuit.Circuit;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class FileStatistics
	{
		public class Count
		{
			internal Library library;
			internal ComponentFactory factory;
			internal int simpleCount;
			internal int uniqueCount;
			internal int recursiveCount;

			internal Count(ComponentFactory factory)
			{
				this.library = null;
				this.factory = factory;
				this.simpleCount = 0;
				this.uniqueCount = 0;
				this.recursiveCount = 0;
			}

			public virtual Library Library
			{
				get
				{
					return library;
				}
			}

			public virtual ComponentFactory Factory
			{
				get
				{
					return factory;
				}
			}

			public virtual int SimpleCount
			{
				get
				{
					return simpleCount;
				}
			}

			public virtual int UniqueCount
			{
				get
				{
					return uniqueCount;
				}
			}

			public virtual int RecursiveCount
			{
				get
				{
					return recursiveCount;
				}
			}
		}

		public static FileStatistics compute(LogisimFile file, Circuit circuit)
		{
			ISet<Circuit> include = new HashSet<Circuit>(file.Circuits);
			IDictionary<Circuit, IDictionary<ComponentFactory, Count>> countMap;
			countMap = new Dictionary<Circuit, IDictionary<ComponentFactory, Count>>();
			doRecursiveCount(circuit, include, countMap);
			doUniqueCounts(countMap[circuit], countMap);
			IList<Count> countList = sortCounts(countMap[circuit], file);
			return new FileStatistics(countList, getTotal(countList, include), getTotal(countList, null));
		}

		private static IDictionary<ComponentFactory, Count> doRecursiveCount(Circuit circuit, ISet<Circuit> include, IDictionary<Circuit, IDictionary<ComponentFactory, Count>> countMap)
		{
			if (countMap.ContainsKey(circuit))
			{
				return countMap[circuit];
			}

			IDictionary<ComponentFactory, Count> counts = doSimpleCount(circuit);
			countMap[circuit] = counts;
			foreach (Count count in counts.Values)
			{
				count.uniqueCount = count.simpleCount;
				count.recursiveCount = count.simpleCount;
			}
			foreach (Circuit sub in include)
			{
				SubcircuitFactory subFactory = sub.SubcircuitFactory;
				if (counts.ContainsKey(subFactory))
				{
					int multiplier = counts[subFactory].simpleCount;
					IDictionary<ComponentFactory, Count> subCount;
					subCount = doRecursiveCount(sub, include, countMap);
					foreach (Count subcount in subCount.Values)
					{
						ComponentFactory subfactory = subcount.factory;
						Count supercount = counts[subfactory];
						if (supercount == null)
						{
							supercount = new Count(subfactory);
							counts[subfactory] = supercount;
						}
						supercount.recursiveCount += multiplier * subcount.recursiveCount;
					}
				}
			}

			return counts;
		}

		private static IDictionary<ComponentFactory, Count> doSimpleCount(Circuit circuit)
		{
			IDictionary<ComponentFactory, Count> counts;
			counts = new Dictionary<ComponentFactory, Count>();
			foreach (Component comp in circuit.NonWires)
			{
				ComponentFactory factory = comp.Factory;
				Count count = counts[factory];
				if (count == null)
				{
					count = new Count(factory);
					counts[factory] = count;
				}
				count.simpleCount++;
			}
			return counts;
		}

		private static void doUniqueCounts(IDictionary<ComponentFactory, Count> counts, IDictionary<Circuit, IDictionary<ComponentFactory, Count>> circuitCounts)
		{
			foreach (Count count in counts.Values)
			{
				ComponentFactory factory = count.Factory;
				int unique = 0;
				foreach (Circuit circ in circuitCounts.Keys)
				{
					Count subcount = circuitCounts[circ][factory];
					if (subcount != null)
					{
						unique += subcount.simpleCount;
					}
				}
				count.uniqueCount = unique;
			}
		}

		private static IList<Count> sortCounts(IDictionary<ComponentFactory, Count> counts, LogisimFile file)
		{
			IList<Count> ret = new List<Count>();
			foreach (AddTool tool in file.Tools)
			{
				ComponentFactory factory = tool.Factory;
				Count count = counts[factory];
				if (count != null)
				{
					count.library = file;
					ret.Add(count);
				}
			}
			foreach (Library lib in file.Libraries)
			{
				foreach (Tool tool in lib.Tools)
				{
					if (tool is AddTool)
					{
						ComponentFactory factory = ((AddTool) tool).Factory;
						Count count = counts[factory];
						if (count != null)
						{
							count.library = lib;
							ret.Add(count);
						}
					}
				}
			}
			return ret;
		}

		private static Count getTotal(IList<Count> counts, ISet<Circuit> exclude)
		{
			Count ret = new Count(null);
			foreach (Count count in counts)
			{
				ComponentFactory factory = count.Factory;
				Circuit factoryCirc = null;
				if (factory is SubcircuitFactory)
				{
					factoryCirc = ((SubcircuitFactory) factory).Subcircuit;
				}
				if (exclude == null || !exclude.Contains(factoryCirc))
				{
					ret.simpleCount += count.simpleCount;
					ret.uniqueCount += count.uniqueCount;
					ret.recursiveCount += count.recursiveCount;
				}
			}
			return ret;
		}

		private IList<Count> counts;
		private Count totalWithout;
		private Count totalWith;

		private FileStatistics(IList<Count> counts, Count totalWithout, Count totalWith)
		{
			this.counts = counts.AsReadOnly();
			this.totalWithout = totalWithout;
			this.totalWith = totalWith;
		}

		public virtual IList<Count> Counts
		{
			get
			{
				return counts;
			}
		}

		public virtual Count TotalWithoutSubcircuits
		{
			get
			{
				return totalWithout;
			}
		}

		public virtual Count TotalWithSubcircuits
		{
			get
			{
				return totalWith;
			}
		}
	}

}
