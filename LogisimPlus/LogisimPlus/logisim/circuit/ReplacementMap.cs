// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using Component = logisim.comp.Component;

	public class ReplacementMap
	{
		private bool frozen;
		private Dictionary<Component, HashSet<Component>> map;
		private Dictionary<Component, HashSet<Component>> inverse;

		public ReplacementMap(Component oldComp, Component newComp) : this(new Dictionary<Component, HashSet<Component>>(), new Dictionary<Component, HashSet<Component>>())
		{
			HashSet<Component> oldSet = new HashSet<Component>(3);
			oldSet.Add(oldComp);
			HashSet<Component> newSet = new HashSet<Component>(3);
			newSet.Add(newComp);
			map[oldComp] = newSet;
			inverse[newComp] = oldSet;
		}

		public ReplacementMap() : this(new Dictionary<Component, HashSet<Component>>(), new Dictionary<Component, HashSet<Component>>())
		{
		}

		private ReplacementMap(Dictionary<Component, HashSet<Component>> map, Dictionary<Component, HashSet<Component>> inverse)
		{
			this.map = map;
			this.inverse = inverse;
		}

		public virtual void reset()
		{
			map.Clear();
			inverse.Clear();
		}

		public virtual bool Empty
		{
			get
			{
				return map.Count == 0 && inverse.Count == 0;
			}
		}

		public virtual ICollection<Component> ReplacedComponents
		{
			get
			{
				return map.Keys;
			}
		}

		public virtual ICollection<Component> get(Component prev)
		{
			return map[prev];
		}

		internal virtual void freeze()
		{
			frozen = true;
		}

		public virtual void add(Component comp)
		{
			if (frozen)
			{
				throw new System.InvalidOperationException("cannot change map after frozen");
			}
			inverse[comp] = new HashSet<Component>(3);
		}

		public virtual void remove(Component comp)
		{
			if (frozen)
			{
				throw new System.InvalidOperationException("cannot change map after frozen");
			}
			map[comp] = new HashSet<Component>(3);
		}

		public virtual void replace(Component prev, Component next)
		{
			put(prev, Collections.singleton(next));
		}

		public virtual void put<T1>(Component prev, ICollection<T1> next) where T1 : logisim.comp.Component
		{
			if (frozen)
			{
				throw new System.InvalidOperationException("cannot change map after frozen");
			}

			HashSet<Component> repl = map[prev];
			if (repl == null)
			{
				repl = new HashSet<Component>(next.Count);
				map[prev] = repl;
			}
			repl.addAll(next);

			foreach (Component n in next)
			{
				repl = inverse[n];
				if (repl == null)
				{
					repl = new HashSet<Component>(3);
					inverse[n] = repl;
				}
				repl.Add(prev);
			}
		}

		internal virtual void append(ReplacementMap next)
		{
			foreach (KeyValuePair<Component, HashSet<Component>> e in next.map.SetOfKeyValuePairs())
			{
				Component b = e.Key;
				HashSet<Component> cs = e.Value; // what b is replaced by
				HashSet<Component> @as = this.inverse.Remove(b); // what was replaced to get b
				if (@as == null)
				{ // b pre-existed replacements so
					@as = new HashSet<Component>(3); // we say it replaces itself.
					@as.Add(b);
				}

				foreach (Component a in @as)
				{
					HashSet<Component> aDst = this.map[a];
					if (aDst == null)
					{ // should happen when b pre-existed only
						aDst = new HashSet<Component>(cs.Count);
						this.map[a] = aDst;
					}
					aDst.Remove(b);
					aDst.addAll(cs);
				}

				foreach (Component c in cs)
				{
					HashSet<Component> cSrc = this.inverse[c]; // should always be null
					if (cSrc == null)
					{
						cSrc = new HashSet<Component>(@as.Count);
						this.inverse[c] = cSrc;
					}
					cSrc.addAll(@as);
				}
			}

			foreach (KeyValuePair<Component, HashSet<Component>> e in next.inverse.SetOfKeyValuePairs())
			{
				Component c = e.Key;
				if (!inverse.ContainsKey(c))
				{
					HashSet<Component> bs = e.Value;
					if (bs.Count > 0)
					{
						Console.Error.WriteLine("internal error: component replaced but not represented"); // OK
					}
					inverse[c] = new HashSet<Component>(3);
				}
			}
		}

		internal virtual ReplacementMap InverseMap
		{
			get
			{
				return new ReplacementMap(inverse, map);
			}
		}

		public virtual ICollection<Component> getComponentsReplacing(Component comp)
		{
			return map[comp];
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.Collection<? extends logisim.comp.Component> getRemovals()
		public virtual ICollection<Component> Removals
		{
			get
			{
				return map.Keys;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.Collection<? extends logisim.comp.Component> getAdditions()
		public virtual ICollection<Component> Additions
		{
			get
			{
				return inverse.Keys;
			}
		}

		public virtual void print(PrintStream @out)
		{
			bool found = false;
			foreach (Component c in Removals)
			{
				if (!found)
				{
					@out.println("  removals:");
				}
				found = true;
				@out.println("    " + c.ToString());
			}
			if (!found)
			{
				@out.println("  removals: none");
			}

			found = false;
			foreach (Component c in Additions)
			{
				if (!found)
				{
					@out.println("  additions:");
				}
				found = true;
				@out.println("    " + c.ToString());
			}
			if (!found)
			{
				@out.println("  additions: none");
			}
		}
	}

}
