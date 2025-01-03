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
	using logisim.data;
	using Action = logisim.proj.Action;
	using StringGetter = logisim.util.StringGetter;

	public sealed class CircuitMutation : CircuitTransaction
	{
		private Circuit primary;
		private List<CircuitChange> changes;

		public CircuitMutation(Circuit circuit)
		{
			this.primary = circuit;
			this.changes = new List<CircuitChange>();
		}

		internal CircuitMutation() : this(null)
		{
		}

		public bool Empty
		{
			get
			{
				return changes.Count == 0;
			}
		}

		public void clear()
		{
			changes.Add(CircuitChange.clear(primary, null));
		}

		public void add(Component comp)
		{
			changes.Add(CircuitChange.add(primary, comp));
		}

		public void addAll<T1>(ICollection<T1> comps) where T1 : logisim.comp.Component
		{
			changes.Add(CircuitChange.addAll(primary, comps));
		}

		public void remove(Component comp)
		{
			changes.Add(CircuitChange.remove(primary, comp));
		}

		public void removeAll<T1>(ICollection<T1> comps) where T1 : logisim.comp.Component
		{
			changes.Add(CircuitChange.removeAll(primary, comps));
		}

		public void replace(Component oldComp, Component newComp)
		{
			ReplacementMap repl = new ReplacementMap(oldComp, newComp);
			changes.Add(CircuitChange.replace(primary, repl));
		}

		public void replace(ReplacementMap replacements)
		{
			if (!replacements.Empty)
			{
				replacements.freeze();
				changes.Add(CircuitChange.replace(primary, replacements));
			}
		}

		public void set(Component comp, Attribute attr, object value)
		{
			changes.Add(CircuitChange.set(primary, comp, attr, value));
		}

		public void setForCircuit(Attribute attr, object value)
		{
			changes.Add(CircuitChange.setForCircuit(primary, attr, value));
		}

		internal void change(CircuitChange change)
		{
			changes.Add(change);
		}

		public Action toAction(StringGetter name)
		{
			if (name == null)
			{
				name = Strings.getter("unknownChangeAction");
			}
			return new CircuitAction(name, this);
		}

		protected internal override Dictionary<Circuit, int> AccessedCircuits
		{
			get
			{
				Dictionary<Circuit, int> accessMap = new Dictionary<Circuit, int>();
				HashSet<Circuit> supercircsDone = new HashSet<Circuit>();
				foreach (CircuitChange change in changes)
				{
					Circuit circ = change.Circuit;
					accessMap[circ] = READ_WRITE;
    
					if (change.concernsSupercircuit())
					{
						bool isFirstForCirc = supercircsDone.Add(circ);
						if (isFirstForCirc)
						{
							foreach (Circuit supercirc in circ.CircuitsUsingThis)
							{
								accessMap[supercirc] = READ_WRITE;
							}
						}
					}
				}
				return accessMap;
			}
		}

		protected internal override void run(CircuitMutator mutator)
		{
			Circuit curCircuit = null;
			ReplacementMap curReplacements = null;
			foreach (CircuitChange change in changes)
			{
				Circuit circ = change.Circuit;
				if (circ != curCircuit)
				{
					if (curCircuit != null)
					{
						mutator.replace(curCircuit, curReplacements);
					}
					curCircuit = circ;
					curReplacements = new ReplacementMap();
				}
				change.execute(mutator, curReplacements);
			}
			if (curCircuit != null)
			{
				mutator.replace(curCircuit, curReplacements);
			}
		}
	}
}
