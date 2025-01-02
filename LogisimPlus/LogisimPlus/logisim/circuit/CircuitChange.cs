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
	using StdAttr = logisim.instance.StdAttr;
	using Pin = logisim.std.wiring.Pin;

	internal class CircuitChange
	{
		internal const int CLEAR = 0;
		internal const int ADD = 1;
		internal const int ADD_ALL = 2;
		internal const int REMOVE = 3;
		internal const int REMOVE_ALL = 4;
		internal const int REPLACE = 5;
		internal const int SET = 6;
		internal const int SET_FOR_CIRCUIT = 7;

		public static CircuitChange clear(Circuit circuit, ICollection<Component> oldComponents)
		{
			return new CircuitChange(circuit, CLEAR, oldComponents);
		}

		public static CircuitChange add(Circuit circuit, Component comp)
		{
			return new CircuitChange(circuit, ADD, comp);
		}

		public static CircuitChange addAll<T1>(Circuit circuit, ICollection<T1> comps) where T1 : logisim.comp.Component
		{
			return new CircuitChange(circuit, ADD_ALL, comps);
		}

		public static CircuitChange remove(Circuit circuit, Component comp)
		{
			return new CircuitChange(circuit, REMOVE, comp);
		}

		public static CircuitChange removeAll<T1>(Circuit circuit, ICollection<T1> comps) where T1 : logisim.comp.Component
		{
			return new CircuitChange(circuit, REMOVE_ALL, comps);
		}

		public static CircuitChange replace(Circuit circuit, ReplacementMap replMap)
		{
			return new CircuitChange(circuit, REPLACE, null, null, null, replMap);
		}

		public static CircuitChange set<T1>(Circuit circuit, Component comp, Attribute<T1> attr, object value)
		{
			return new CircuitChange(circuit, SET, comp, attr, null, value);
		}

		public static CircuitChange set<T1>(Circuit circuit, Component comp, Attribute<T1> attr, object oldValue, object newValue)
		{
			return new CircuitChange(circuit, SET, comp, attr, oldValue, newValue);
		}

		public static CircuitChange setForCircuit<T1>(Circuit circuit, Attribute<T1> attr, object v)
		{
			return new CircuitChange(circuit, SET_FOR_CIRCUIT, null, attr, null, v);
		}

		public static CircuitChange setForCircuit<T1>(Circuit circuit, Attribute<T1> attr, object oldValue, object newValue)
		{
			return new CircuitChange(circuit, SET_FOR_CIRCUIT, null, attr, oldValue, newValue);
		}

		private Circuit circuit;
		private int type;
		private Component comp;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.Collection<? extends logisim.comp.Component> comps;
		private ICollection<Component> comps;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private logisim.data.Attribute<?> attr;
		private Attribute<object> attr;
		private object oldValue;
		private object newValue;

		private CircuitChange(Circuit circuit, int type, Component comp) : this(circuit, type, comp, null, null, null)
		{
		}

// JAVA TO C# CONVERTER TASK: Wildcard generics in method parameters are not converted:
// ORIGINAL LINE: private CircuitChange(Circuit circuit, int type, java.util.Collection<? extends logisim.comp.Component> comps)
		private CircuitChange(Circuit circuit, int type, ICollection<Component> comps) : this(circuit, type, null, null, null, null)
		{
			this.comps = comps;
		}

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: private CircuitChange(Circuit circuit, int type, logisim.comp.Component comp, logisim.data.Attribute<?> attr, Object oldValue, Object newValue)
		private CircuitChange(Circuit circuit, int type, Component comp, Attribute<T1> attr, object oldValue, object newValue)
		{
			this.circuit = circuit;
			this.type = type;
			this.comp = comp;
			this.attr = attr;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public virtual Circuit Circuit
		{
			get
			{
				return circuit;
			}
		}

		public virtual int Type
		{
			get
			{
				return type;
			}
		}

		public virtual Component Component
		{
			get
			{
				return comp;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public logisim.data.Attribute<?> getAttribute()
		public virtual Attribute<object> Attribute
		{
			get
			{
				return attr;
			}
		}

		public virtual object OldValue
		{
			get
			{
				return oldValue;
			}
		}

		public virtual object NewValue
		{
			get
			{
				return newValue;
			}
		}

		internal virtual CircuitChange ReverseChange
		{
			get
			{
				switch (type)
				{
				case CLEAR:
					return CircuitChange.addAll(circuit, comps);
				case ADD:
					return CircuitChange.remove(circuit, comp);
				case ADD_ALL:
					return CircuitChange.removeAll(circuit, comps);
				case REMOVE:
					return CircuitChange.add(circuit, comp);
				case REMOVE_ALL:
					return CircuitChange.addAll(circuit, comps);
				case SET:
					return CircuitChange.set(circuit, comp, attr, newValue, oldValue);
				case SET_FOR_CIRCUIT:
					return CircuitChange.setForCircuit(circuit, attr, newValue, oldValue);
				case REPLACE:
					return CircuitChange.replace(circuit, ((ReplacementMap) newValue).InverseMap);
				default:
					throw new System.ArgumentException("unknown change type " + type);
				}
			}
		}

		internal virtual void execute(CircuitMutator mutator, ReplacementMap prevReplacements)
		{
			switch (type)
			{
			case CLEAR:
				mutator.clear(circuit);
				prevReplacements.reset();
				break;
			case ADD:
				prevReplacements.add(comp);
				break;
			case ADD_ALL:
				foreach (Component comp in comps)
				{
					prevReplacements.add(comp);
				}
				break;
			case REMOVE:
				prevReplacements.remove(comp);
				break;
			case REMOVE_ALL:
				foreach (Component comp in comps)
				{
					prevReplacements.remove(comp);
				}
				break;
			case REPLACE:
				prevReplacements.append((ReplacementMap) newValue);
				break;
			case SET:
				mutator.replace(circuit, prevReplacements);
				prevReplacements.reset();
				mutator.set(circuit, comp, attr, newValue);
				break;
			case SET_FOR_CIRCUIT:
				mutator.replace(circuit, prevReplacements);
				prevReplacements.reset();
				mutator.setForCircuit(circuit, attr, newValue);
				break;
			default:
				throw new System.ArgumentException("unknown change type " + type);
			}
		}

		internal virtual bool concernsSupercircuit()
		{
			switch (type)
			{
			case CLEAR:
				return true;
			case ADD:
			case REMOVE:
				return comp.Factory is Pin;
			case ADD_ALL:
			case REMOVE_ALL:
				foreach (Component comp in comps)
				{
					if (comp.Factory is Pin)
					{
						return true;
					}
				}
				return false;
			case REPLACE:
				ReplacementMap repl = (ReplacementMap) newValue;
				foreach (Component comp in repl.Removals)
				{
					if (comp.Factory is Pin)
					{
						return true;
					}
				}
				foreach (Component comp in repl.Additions)
				{
					if (comp.Factory is Pin)
					{
						return true;
					}
				}
				return false;
			case SET:
				return comp.Factory is Pin && (attr == StdAttr.WIDTH || attr == Pin.ATTR_TYPE);
			default:
				return false;
			}
		}
	}
}
