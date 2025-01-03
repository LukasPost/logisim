// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.toolbar
{

	public abstract class AbstractToolbarModel : ToolbarModel
	{
		private List<ToolbarModelListener> listeners;

		public AbstractToolbarModel()
		{
			listeners = new List<ToolbarModelListener>();
		}

		public virtual void addToolbarModelListener(ToolbarModelListener listener)
		{
			listeners.Add(listener);
		}

		public virtual void removeToolbarModelListener(ToolbarModelListener listener)
		{
			listeners.Remove(listener);
		}

		protected internal virtual void fireToolbarContentsChanged()
		{
			ToolbarModelEvent @event = new ToolbarModelEvent(this);
			foreach (ToolbarModelListener listener in listeners)
			{
				listener.toolbarContentsChanged(@event);
			}
		}

		protected internal virtual void fireToolbarAppearanceChanged()
		{
			ToolbarModelEvent @event = new ToolbarModelEvent(this);
			foreach (ToolbarModelListener listener in listeners)
			{
				listener.toolbarAppearanceChanged(@event);
			}
		}

		public abstract List<ToolbarItem> Items {get;}

		public abstract bool isSelected(ToolbarItem item);

		public abstract void itemSelected(ToolbarItem item);
	}

}
