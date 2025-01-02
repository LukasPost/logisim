// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using CircuitTransaction = logisim.circuit.CircuitTransaction;
	using CircuitTransactionResult = logisim.circuit.CircuitTransactionResult;
	using Component = logisim.comp.Component;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using StringGetter = logisim.util.StringGetter;

	public class SetAttributeAction : Action
	{
		private StringGetter nameGetter;
		private Circuit circuit;
		private IList<Component> comps;
		private IList<Attribute<object>> attrs;
		private IList<object> values;
		private IList<object> oldValues;
		private CircuitTransaction xnReverse;

		public SetAttributeAction(Circuit circuit, StringGetter nameGetter)
		{
			this.nameGetter = nameGetter;
			this.circuit = circuit;
			this.comps = new List<Component>();
			this.attrs = new List<Attribute<object>>();
			this.values = new List<object>();
			this.oldValues = new List<object>();
		}

		public virtual void set<T1>(Component comp, Attribute<T1> attr, object value)
		{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> a = (logisim.data.Attribute<Object>) attr;
			Attribute<object> a = (Attribute<object>) attr;
			comps.Add(comp);
			attrs.Add(a);
			values.Add(value);
		}

		public virtual bool Empty
		{
			get
			{
				return comps.Count == 0;
			}
		}

		public override string Name
		{
			get
			{
				return nameGetter.get();
			}
		}

		public override void doIt(Project proj)
		{
			CircuitMutation xn = new CircuitMutation(circuit);
			int len = values.Count;
			oldValues.Clear();
			for (int i = 0; i < len; i++)
			{
				Component comp = comps[i];
				Attribute<object> attr = attrs[i];
				object value = values[i];
				if (circuit.contains(comp))
				{
					oldValues.Add(null);
					xn.set(comp, attr, value);
				}
				else
				{
					AttributeSet compAttrs = comp.AttributeSet;
					oldValues.Add(compAttrs.getValue(attr));
					compAttrs.setValue(attr, value);
				}
			}

			if (!xn.Empty)
			{
				CircuitTransactionResult result = xn.execute();
				xnReverse = result.ReverseTransaction;
			}
		}

		public override void undo(Project proj)
		{
			if (xnReverse != null)
			{
				xnReverse.execute();
			}
			for (int i = oldValues.Count - 1; i >= 0; i--)
			{
				Component comp = comps[i];
				Attribute<object> attr = attrs[i];
				object value = oldValues[i];
				if (value != null)
				{
					comp.AttributeSet.setValue(attr, value);
				}
			}
		}
	}

}
