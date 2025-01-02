// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{


	public abstract class WindowMenuItemManager
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class MyListener : WindowListener
		{
			private readonly WindowMenuItemManager outerInstance;

			public MyListener(WindowMenuItemManager outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void windowOpened(WindowEvent @event)
			{
			}

			public virtual void windowClosing(WindowEvent @event)
			{
				JFrame frame = outerInstance.getJFrame(false);
				if (frame.getDefaultCloseOperation() == JFrame.HIDE_ON_CLOSE)
				{
					outerInstance.removeFromManager();
				}
			}

			public virtual void windowClosed(WindowEvent @event)
			{
				outerInstance.removeFromManager();
			}

			public virtual void windowDeiconified(WindowEvent @event)
			{
			}

			public virtual void windowIconified(WindowEvent @event)
			{
				outerInstance.addToManager();
				WindowMenuManager.CurrentManager = outerInstance;
			}

			public virtual void windowActivated(WindowEvent @event)
			{
				outerInstance.addToManager();
				WindowMenuManager.CurrentManager = outerInstance;
			}

			public virtual void windowDeactivated(WindowEvent @event)
			{
				WindowMenuManager.unsetCurrentManager(outerInstance);
			}
		}

		private MyListener myListener;
		private string text;
		private bool persistent;
		private bool listenerAdded = false;
		private bool inManager = false;
		private Dictionary<WindowMenu, JRadioButtonMenuItem> menuItems = new Dictionary<WindowMenu, JRadioButtonMenuItem>();

		public WindowMenuItemManager(string text, bool persistent)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.text = text;
			this.persistent = persistent;
			if (persistent)
			{
				WindowMenuManager.addManager(this);
			}
		}

		public abstract JFrame getJFrame(bool create);

		public virtual void frameOpened(JFrame frame)
		{
			if (!listenerAdded)
			{
				frame.addWindowListener(myListener);
				listenerAdded = true;
			}
			addToManager();
			WindowMenuManager.CurrentManager = this;
		}

		public virtual void frameClosed(JFrame frame)
		{
			if (!persistent)
			{
				if (listenerAdded)
				{
					frame.removeWindowListener(myListener);
					listenerAdded = false;
				}
				removeFromManager();
			}
		}

		private void addToManager()
		{
			if (!persistent && !inManager)
			{
				WindowMenuManager.addManager(this);
				inManager = true;
			}
		}

		private void removeFromManager()
		{
			if (!persistent && inManager)
			{
				inManager = false;
				foreach (WindowMenu menu in WindowMenuManager.Menus)
				{
					JRadioButtonMenuItem menuItem = menuItems[menu];
					menu.removeMenuItem(this, menuItem);
				}
				WindowMenuManager.removeManager(this);
			}
		}

		public virtual string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				foreach (JRadioButtonMenuItem menuItem in menuItems.Values)
				{
					menuItem.setText(text);
				}
			}
		}


		internal virtual JRadioButtonMenuItem getMenuItem(WindowMenu key)
		{
			return menuItems[key];
		}

		internal virtual void createMenuItem(WindowMenu menu)
		{
			WindowMenuItem ret = new WindowMenuItem(this);
			menuItems[menu] = ret;
			menu.addMenuItem(this, ret, persistent);
		}

		internal virtual void removeMenuItem(WindowMenu menu)
		{
			JRadioButtonMenuItem item = menuItems.Remove(menu);
			if (item != null)
			{
				menu.removeMenuItem(this, item);
			}
		}

		internal virtual bool Selected
		{
			set
			{
				foreach (JRadioButtonMenuItem item in menuItems.Values)
				{
					item.setSelected(value);
				}
			}
		}
	}

}
