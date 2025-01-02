// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.move
{

	using ReplacementMap = logisim.circuit.ReplacementMap;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using Location = logisim.data.Location;

	public class MoveResult
	{
		private ReplacementMap replacements;
		private ICollection<ConnectionData> unsatisfiedConnections;
		private ICollection<Location> unconnectedLocations;
		private int totalDistance;

		public MoveResult(MoveRequest request, ReplacementMap replacements, ICollection<ConnectionData> unsatisfiedConnections, int totalDistance)
		{
			this.replacements = replacements;
			this.unsatisfiedConnections = unsatisfiedConnections;
			this.totalDistance = totalDistance;

			List<Location> unconnected = new List<Location>();
			foreach (ConnectionData conn in unsatisfiedConnections)
			{
				unconnected.Add(conn.Location);
			}
			unconnectedLocations = unconnected;
		}

		internal virtual void addUnsatisfiedConnections(ICollection<ConnectionData> toAdd)
		{
			unsatisfiedConnections.addAll(toAdd);
			foreach (ConnectionData conn in toAdd)
			{
				unconnectedLocations.Add(conn.Location);
			}
		}

		public virtual ICollection<Wire> WiresToAdd
		{
			get
			{
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Collection<logisim.circuit.Wire> ret = (java.util.Collection<logisim.circuit.Wire>) replacements.getAdditions();
				ICollection<Wire> ret = (ICollection<Wire>) replacements.Additions;
				return ret;
			}
		}

		public virtual ICollection<Wire> WiresToRemove
		{
			get
			{
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Collection<logisim.circuit.Wire> ret = (java.util.Collection<logisim.circuit.Wire>) replacements.getAdditions();
				ICollection<Wire> ret = (ICollection<Wire>) replacements.Additions;
				return ret;
			}
		}

		public virtual ReplacementMap ReplacementMap
		{
			get
			{
				return replacements;
			}
		}

		public virtual ICollection<Location> UnconnectedLocations
		{
			get
			{
				return unconnectedLocations;
			}
		}

		internal virtual ICollection<ConnectionData> UnsatisifiedConnections
		{
			get
			{
				return unsatisfiedConnections;
			}
		}

		internal virtual int TotalDistance
		{
			get
			{
				return totalDistance;
			}
		}

		public virtual void print(PrintStream @out)
		{
			bool printed = false;
			foreach (Component w in replacements.Additions)
			{
				printed = true;
				@out.println("add " + w);
			}
			foreach (Component w in replacements.Removals)
			{
				printed = true;
				@out.println("del " + w);
			}
			foreach (Component w in replacements.ReplacedComponents)
			{
				printed = true;
				@out.print("repl " + w + " by");
				foreach (Component w2 in replacements.getComponentsReplacing(w))
				{
					@out.print(" " + w2);
				}
				@out.println();
			}
			if (!printed)
			{
				@out.println("no replacements");
			}
		}
	}

}
