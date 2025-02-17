/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import logisim.circuit.Circuit;
import logisim.circuit.SubcircuitFactory;
import logisim.comp.Component;
import logisim.comp.ComponentFactory;
import logisim.tools.AddTool;
import logisim.tools.Library;
import logisim.tools.Tool;

public class FileStatistics {
	public static class Count {
		private Library library;
		private ComponentFactory factory;
		private int simpleCount;
		private int uniqueCount;
		private int recursiveCount;

		private Count(ComponentFactory factory) {
			library = null;
			this.factory = factory;
			simpleCount = 0;
			uniqueCount = 0;
			recursiveCount = 0;
		}

		public Library getLibrary() {
			return library;
		}

		public ComponentFactory getFactory() {
			return factory;
		}

		public int getSimpleCount() {
			return simpleCount;
		}

		public int getUniqueCount() {
			return uniqueCount;
		}

		public int getRecursiveCount() {
			return recursiveCount;
		}
	}

	public static FileStatistics compute(LogisimFile file, Circuit circuit) {
		Set<Circuit> include = new HashSet<>(file.getCircuits());
		Map<Circuit, Map<ComponentFactory, Count>> countMap = new HashMap<>();
		doRecursiveCount(circuit, include, countMap);
		doUniqueCounts(countMap.get(circuit), countMap);
		List<Count> countList = sortCounts(countMap.get(circuit), file);
		return new FileStatistics(countList, getTotal(countList, include), getTotal(countList, null));
	}

	private static Map<ComponentFactory, Count> doRecursiveCount(Circuit circuit, Set<Circuit> include,
			Map<Circuit, Map<ComponentFactory, Count>> countMap) {
		if (countMap.containsKey(circuit)) return countMap.get(circuit);

		Map<ComponentFactory, Count> counts = doSimpleCount(circuit);
		countMap.put(circuit, counts);
		for (Count count : counts.values()) {
			count.uniqueCount = count.simpleCount;
			count.recursiveCount = count.simpleCount;
		}
		for (Circuit sub : include) {
			SubcircuitFactory subFactory = sub.getSubcircuitFactory();
			if (counts.containsKey(subFactory)) {
				int multiplier = counts.get(subFactory).simpleCount;
				Map<ComponentFactory, Count> subCount = doRecursiveCount(sub, include, countMap);
				for (Count subcount : subCount.values()) {
					ComponentFactory subfactory = subcount.factory;
					Count supercount = counts.computeIfAbsent(subfactory, Count::new);
					supercount.recursiveCount += multiplier * subcount.recursiveCount;
				}
			}
		}

		return counts;
	}

	private static Map<ComponentFactory, Count> doSimpleCount(Circuit circuit) {
		Map<ComponentFactory, Count> counts = new HashMap<>();
		for (Component comp : circuit.getNonWires()) {
			ComponentFactory factory = comp.getFactory();
			Count count = counts.computeIfAbsent(factory, Count::new);
			count.simpleCount++;
		}
		return counts;
	}

	private static void doUniqueCounts(Map<ComponentFactory, Count> counts,
			Map<Circuit, Map<ComponentFactory, Count>> circuitCounts) {
		for (Count count : counts.values()) {
			ComponentFactory factory = count.getFactory();
			int unique = 0;
			for (Circuit circ : circuitCounts.keySet()) {
				Count subcount = circuitCounts.get(circ).get(factory);
				if (subcount != null) unique += subcount.simpleCount;
			}
			count.uniqueCount = unique;
		}
	}

	private static List<Count> sortCounts(Map<ComponentFactory, Count> counts, LogisimFile file) {
		List<Count> ret = new ArrayList<>();
		for (AddTool tool : file.getTools()) {
			ComponentFactory factory = tool.getFactory();
			Count count = counts.get(factory);
			if (count != null) {
				count.library = file;
				ret.add(count);
			}
		}
		for (Library lib : file.getLibraries())
			for (Tool tool : lib.getTools())
				if (tool instanceof AddTool) {
					ComponentFactory factory = ((AddTool) tool).getFactory();
					Count count = counts.get(factory);
					if (count != null) {
						count.library = lib;
						ret.add(count);
					}
				}
		return ret;
	}

	private static Count getTotal(List<Count> counts, Set<Circuit> exclude) {
		Count ret = new Count(null);
		for (Count count : counts) {
			ComponentFactory factory = count.getFactory();
			Circuit factoryCirc = null;
			if (factory instanceof SubcircuitFactory) factoryCirc = ((SubcircuitFactory) factory).getSubcircuit();
			if (exclude == null || !exclude.contains(factoryCirc)) {
				ret.simpleCount += count.simpleCount;
				ret.uniqueCount += count.uniqueCount;
				ret.recursiveCount += count.recursiveCount;
			}
		}
		return ret;
	}

	private List<Count> counts;
	private Count totalWithout;
	private Count totalWith;

	private FileStatistics(List<Count> counts, Count totalWithout, Count totalWith) {
		this.counts = Collections.unmodifiableList(counts);
		this.totalWithout = totalWithout;
		this.totalWith = totalWith;
	}

	public List<Count> getCounts() {
		return counts;
	}

	public Count getTotalWithoutSubcircuits() {
		return totalWithout;
	}

	public Count getTotalWithSubcircuits() {
		return totalWith;
	}
}
