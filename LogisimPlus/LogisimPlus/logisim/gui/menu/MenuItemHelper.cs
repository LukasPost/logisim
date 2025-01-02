// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{

	internal class MenuItemHelper : ActionListener
	{
		private JMenuItem source;
		private LogisimMenuItem menuItem;
		private Menu menu;
		private bool enabled;
		private bool inActionListener;
		private List<ActionListener> listeners;

		public MenuItemHelper(JMenuItem source, Menu menu, LogisimMenuItem menuItem)
		{
			this.source = source;
			this.menu = menu;
			this.menuItem = menuItem;
			this.enabled = true;
			this.inActionListener = false;
			this.listeners = new List<ActionListener>();
		}

		public virtual bool hasListeners()
		{
			return listeners.Count > 0;
		}

		public virtual void addActionListener(ActionListener l)
		{
			listeners.Add(l);
			computeEnabled();
		}

		public virtual void removeActionListener(ActionListener l)
		{
			listeners.Remove(l);
			computeEnabled();
		}

		public virtual bool Enabled
		{
			set
			{
				if (!inActionListener)
				{
					enabled = value;
				}
			}
		}

		private void computeEnabled()
		{
			inActionListener = true;
			try
			{
				source.setEnabled(enabled);
				menu.computeEnabled();
			}
			finally
			{
				inActionListener = false;
			}
		}

		public virtual void actionPerformed(ActionEvent @event)
		{
			if (listeners.Count > 0)
			{
				ActionEvent e = new ActionEvent(menuItem, @event.getID(), @event.getActionCommand(), @event.getWhen(), @event.getModifiers());
				foreach (ActionListener l in listeners)
				{
					l.actionPerformed(e);
				}
			}
		}
	}

}
