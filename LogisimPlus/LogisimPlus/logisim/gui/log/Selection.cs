// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{

	using CircuitState = logisim.circuit.CircuitState;

	internal class Selection
	{
		private CircuitState root;
		private Model model;
		private List<SelectionItem> components;

		public Selection(CircuitState root, Model model)
		{
			this.root = root;
			this.model = model;
			components = new List<SelectionItem>();
		}

		public virtual void addModelListener(ModelListener l)
		{
			model.addModelListener(l);
		}

		public virtual void removeModelListener(ModelListener l)
		{
			model.removeModelListener(l);
		}

		public virtual CircuitState CircuitState
		{
			get
			{
				return root;
			}
		}

		public virtual int size()
		{
			return components.Count;
		}

		public virtual SelectionItem get(int index)
		{
			return components[index];
		}

		public virtual int indexOf(SelectionItem value)
		{
			return components.IndexOf(value);
		}

		public virtual void add(SelectionItem item)
		{
			components.Add(item);
			model.fireSelectionChanged(new ModelEvent());
		}

		public virtual void remove(int index)
		{
			components.RemoveAt(index);
			model.fireSelectionChanged(new ModelEvent());
		}

		public virtual void move(int fromIndex, int toIndex)
		{
			if (fromIndex == toIndex)
			{
				return;
			}
			SelectionItem o = components.RemoveAndReturn(fromIndex);
			components.Insert(toIndex, o);
			model.fireSelectionChanged(new ModelEvent());
		}
	}

}
