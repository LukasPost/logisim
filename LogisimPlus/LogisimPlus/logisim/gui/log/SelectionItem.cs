// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{
	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using CircuitState = logisim.circuit.CircuitState;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Component = logisim.comp.Component;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using Value = logisim.data.Value;
	using StdAttr = logisim.instance.StdAttr;

	internal class SelectionItem : AttributeListener, CircuitListener
	{
		private Model model;
		private Component[] path;
		private Component comp;
		private object option;
		private int radix = 2;
		private string shortDescriptor;
		private string longDescriptor;

		public SelectionItem(Model model, Component[] path, Component comp, object option)
		{
			this.model = model;
			this.path = path;
			this.comp = comp;
			this.option = option;
			computeDescriptors();

			if (path != null)
			{
				model.CircuitState.Circuit.addCircuitListener(this);
				for (int i = 0; i < path.Length; i++)
				{
					path[i].AttributeSet.addAttributeListener(this);
					SubcircuitFactory circFact = (SubcircuitFactory) path[i].Factory;
					circFact.Subcircuit.addCircuitListener(this);
				}
			}
			comp.AttributeSet.addAttributeListener(this);
		}

		private bool computeDescriptors()
		{
			bool changed = false;

			Loggable log = (Loggable) comp.getFeature(typeof(Loggable));
			string newShort = log.getLogName(option);
			if (string.ReferenceEquals(newShort, null) || newShort.Equals(""))
			{
				newShort = comp.Factory.DisplayName + comp.Location.ToString();
				if (option != null)
				{
					newShort += "." + option.ToString();
				}
			}
			if (!newShort.Equals(shortDescriptor))
			{
				changed = true;
				shortDescriptor = newShort;
			}

			StringBuilder buf = new StringBuilder();
			for (int i = 0; i < path.Length; i++)
			{
				if (i > 0)
				{
					buf.Append(".");
				}
				string label = path[i].AttributeSet.getValue(StdAttr.LABEL);
				if (!string.ReferenceEquals(label, null) && !label.Equals(""))
				{
					buf.Append(label);
				}
				else
				{
					buf.Append(path[i].Factory.DisplayName);
					buf.Append(path[i].Location);
				}
				buf.Append(".");
			}
			buf.Append(shortDescriptor);
			string newLong = buf.ToString();
			if (!newLong.Equals(longDescriptor))
			{
				changed = true;
				longDescriptor = newLong;
			}

			return changed;
		}

		public virtual Component[] Path
		{
			get
			{
				return path;
			}
		}

		public virtual Component Component
		{
			get
			{
				return comp;
			}
		}

		public virtual object Option
		{
			get
			{
				return option;
			}
		}

		public virtual int Radix
		{
			get
			{
				return radix;
			}
			set
			{
				radix = value;
				model.fireSelectionChanged(new ModelEvent());
			}
		}


		public virtual string toShortString()
		{
			return shortDescriptor;
		}

		public override string ToString()
		{
			return longDescriptor;
		}

		public virtual Value fetchValue(CircuitState root)
		{
			CircuitState cur = root;
			for (int i = 0; i < path.Length; i++)
			{
				SubcircuitFactory circFact = (SubcircuitFactory) path[i].Factory;
				cur = circFact.getSubstate(cur, path[i]);
			}
			Loggable log = (Loggable) comp.getFeature(typeof(Loggable));
			return log == null ? Value.NIL : log.getLogValue(cur, option);
		}

		public virtual void attributeListChanged(AttributeEvent e)
		{
		}

		public virtual void attributeValueChanged(AttributeEvent e)
		{
			if (computeDescriptors())
			{
				model.fireSelectionChanged(new ModelEvent());
			}
		}

		public virtual void circuitChanged(CircuitEvent @event)
		{
			int action = @event.Action;
			if (action == CircuitEvent.ACTION_CLEAR || action == CircuitEvent.ACTION_REMOVE)
			{
				Circuit circ = @event.Circuit;
				Component circComp = null;
				if (circ == model.CircuitState.Circuit)
				{
					circComp = path != null && path.Length > 0 ? path[0] : comp;
				}
				else if (path != null)
				{
					for (int i = 0; i < path.Length; i++)
					{
						SubcircuitFactory circFact = (SubcircuitFactory) path[i].Factory;
						if (circ == circFact.Subcircuit)
						{
							circComp = i + 1 < path.Length ? path[i + 1] : comp;
						}
					}
				}
				if (circComp == null)
				{
					return;
				}

				if (action == CircuitEvent.ACTION_REMOVE && @event.Data != circComp)
				{
					return;
				}

				int index = model.Selection.indexOf(this);
				if (index < 0)
				{
					return;
				}
				model.Selection.remove(index);
			}
		}
	}

}
