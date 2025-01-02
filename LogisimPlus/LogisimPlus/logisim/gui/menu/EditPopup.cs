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


	public abstract class EditPopup : JPopupMenu
	{
		private class Listener : ActionListener
		{
			private readonly EditPopup outerInstance;

			public Listener(EditPopup outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object source = e.getSource();
				foreach (KeyValuePair<LogisimMenuItem, JMenuItem> entry in outerInstance.items.SetOfKeyValuePairs())
				{
					if (entry.Value == source)
					{
						outerInstance.fire(entry.Key);
						return;
					}
				}
			}
		}

		private Listener listener;
		private IDictionary<LogisimMenuItem, JMenuItem> items;

		public EditPopup() : this(false)
		{
		}

		public EditPopup(bool waitForInitialize)
		{
			listener = new Listener(this);
			items = new Dictionary<LogisimMenuItem, JMenuItem>();
			if (!waitForInitialize)
			{
				initialize();
			}
		}

		protected internal virtual void initialize()
		{
			bool x = false;
			x |= add(LogisimMenuBar.CUT, Strings.get("editCutItem"));
			x |= add(LogisimMenuBar.COPY, Strings.get("editCopyItem"));
			if (x)
			{
				addSeparator();
				x = false;
			}
			x |= add(LogisimMenuBar.DELETE, Strings.get("editClearItem"));
			x |= add(LogisimMenuBar.DUPLICATE, Strings.get("editDuplicateItem"));
			if (x)
			{
				addSeparator();
				x = false;
			}
			x |= add(LogisimMenuBar.RAISE, Strings.get("editRaiseItem"));
			x |= add(LogisimMenuBar.LOWER, Strings.get("editLowerItem"));
			x |= add(LogisimMenuBar.RAISE_TOP, Strings.get("editRaiseTopItem"));
			x |= add(LogisimMenuBar.LOWER_BOTTOM, Strings.get("editLowerBottomItem"));
			if (x)
			{
				addSeparator();
				x = false;
			}
			x |= add(LogisimMenuBar.ADD_CONTROL, Strings.get("editAddControlItem"));
			x |= add(LogisimMenuBar.REMOVE_CONTROL, Strings.get("editRemoveControlItem"));
			if (!x && getComponentCount() > 0)
			{
				remove(getComponentCount() - 1);
			}
		}

		private bool add(LogisimMenuItem item, string display)
		{
			if (shouldShow(item))
			{
				JMenuItem menu = new JMenuItem(display);
				items[item] = menu;
				menu.setEnabled(isEnabled(item));
				menu.addActionListener(listener);
				add(menu);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected internal abstract bool shouldShow(LogisimMenuItem item);

		protected internal abstract bool isEnabled(LogisimMenuItem item);

		protected internal abstract void fire(LogisimMenuItem item);
	}

}
