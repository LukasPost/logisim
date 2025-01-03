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
	using AttributeSet = logisim.data.AttributeSet;

	internal class CircuitMutatorImpl : CircuitMutator
	{
		private List<CircuitChange> log;
		private Dictionary<Circuit, ReplacementMap> replacements;
		private HashSet<Circuit> modified;

		public CircuitMutatorImpl()
		{
			log = new List<CircuitChange>();
			replacements = new Dictionary<Circuit, ReplacementMap>();
			modified = new HashSet<Circuit>();
		}

		public virtual void clear(Circuit circuit)
		{
			HashSet<Component> comps = new HashSet<Component>(circuit.NonWires);
			comps.addAll(circuit.Wires);
			if (comps.Count > 0)
			{
				modified.Add(circuit);
			}
			log.Add(CircuitChange.clear(circuit, comps));

			ReplacementMap repl = new ReplacementMap();
			foreach (Component comp in comps)
			{
				repl.remove(comp);
			}
			getMap(circuit).append(repl);

			circuit.mutatorClear();
		}

		public virtual void add(Circuit circuit, Component comp)
		{
			modified.Add(circuit);
			log.Add(CircuitChange.add(circuit, comp));

			ReplacementMap repl = new ReplacementMap();
			repl.add(comp);
			getMap(circuit).append(repl);

			circuit.mutatorAdd(comp);
		}

		public virtual void remove(Circuit circuit, Component comp)
		{
			if (circuit.contains(comp))
			{
				modified.Add(circuit);
				log.Add(CircuitChange.remove(circuit, comp));

				ReplacementMap repl = new ReplacementMap();
				repl.remove(comp);
				getMap(circuit).append(repl);

				circuit.mutatorRemove(comp);
			}
		}

		public virtual void replace(Circuit circuit, Component prev, Component next)
		{
			replace(circuit, new ReplacementMap(prev, next));
		}

		public virtual void replace(Circuit circuit, ReplacementMap repl)
		{
			if (!repl.Empty)
			{
				modified.Add(circuit);
				log.Add(CircuitChange.replace(circuit, repl));

				repl.freeze();
				getMap(circuit).append(repl);

				foreach (Component c in repl.Removals)
				{
					circuit.mutatorRemove(c);
				}
				foreach (Component c in repl.Additions)
				{
					circuit.mutatorAdd(c);
				}
			}
		}

		public virtual void set(Circuit circuit, Component comp, Attribute attr, object newValue)
		{
			if (circuit.contains(comp))
			{
				modified.Add(circuit);
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute a = (logisim.data.Attribute) attr;
				AttributeSet attrs = comp.AttributeSet;
				object oldValue = attrs.getValue(attr);
				log.Add(CircuitChange.set(circuit, comp, attr, oldValue, newValue));
				attrs.setValue(attr, newValue);
			}
		}

		public virtual void setForCircuit(Circuit circuit, Attribute attr, object newValue)
		{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute a = (logisim.data.Attribute) attr;
			AttributeSet attrs = circuit.StaticAttributes;
			object oldValue = attrs.getValue(attr);
			log.Add(CircuitChange.setForCircuit(circuit, attr, oldValue, newValue));
			attrs.setValue(attr, newValue);
		}

		private ReplacementMap getMap(Circuit circuit)
		{
			ReplacementMap ret = replacements[circuit];
			if (ret == null)
			{
				ret = new ReplacementMap();
				replacements[circuit] = ret;
			}
			return ret;
		}

		internal virtual CircuitTransaction ReverseTransaction
		{
			get
			{
				CircuitMutation ret = new CircuitMutation();
				List<CircuitChange> log = this.log;
				for (int i = log.Count - 1; i >= 0; i--)
				{
					ret.change(log[i].ReverseChange);
				}
				return ret;
			}
		}

		internal virtual ReplacementMap getReplacementMap(Circuit circuit)
		{
			return replacements[circuit];
		}

		internal virtual void markModified(Circuit circuit)
		{
			modified.Add(circuit);
		}

		internal virtual IEnumerable<Circuit> ModifiedCircuits
		{
			get
			{
				return modified;
			}
		}
	}

}
