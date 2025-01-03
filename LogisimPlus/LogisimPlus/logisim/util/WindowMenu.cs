// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{


	public class WindowMenu : JMenu
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class MyListener : LocaleListener, ActionListener
		{
			private readonly WindowMenu outerInstance;

			public MyListener(WindowMenu outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void localeChanged()
			{
				outerInstance.setText(Strings.get("windowMenu"));
				outerInstance.minimize.setText(Strings.get("windowMinimizeItem"));
				outerInstance.close.setText(Strings.get("windowCloseItem"));
				outerInstance.zoom.setText(MacCompatibility.QuitAutomaticallyPresent ? Strings.get("windowZoomItemMac") : Strings.get("windowZoomItem"));
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == outerInstance.minimize)
				{
					outerInstance.doMinimize();
				}
				else if (src == outerInstance.zoom)
				{
					outerInstance.doZoom();
				}
				else if (src == outerInstance.close)
				{
					outerInstance.doClose();
				}
				else if (src is WindowMenuItem)
				{
					WindowMenuItem choice = (WindowMenuItem) src;
					if (choice.isSelected())
					{
						WindowMenuItem item = findOwnerItem();
						if (item != null)
						{
							item.setSelected(true);
						}
						choice.actionPerformed(e);
					}
				}
			}

			internal virtual WindowMenuItem findOwnerItem()
			{
				foreach (WindowMenuItem i in outerInstance.persistentItems)
				{
					if (i.JFrame == outerInstance.owner)
					{
						return i;
					}
				}
				foreach (WindowMenuItem i in outerInstance.transientItems)
				{
					if (i.JFrame == outerInstance.owner)
					{
						return i;
					}
				}
				return null;
			}
		}

		private JFrame owner;
		private MyListener myListener;
		private JMenuItem minimize = new JMenuItem();
		private JMenuItem zoom = new JMenuItem();
		private JMenuItem close = new JMenuItem();
		private JRadioButtonMenuItem nullItem = new JRadioButtonMenuItem();
		private List<WindowMenuItem> persistentItems = new List<WindowMenuItem>();
		private List<WindowMenuItem> transientItems = new List<WindowMenuItem>();

		public WindowMenu(JFrame owner)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.owner = owner;
			WindowMenuManager.addMenu(this);

			int menuMask = getToolkit().getMenuShortcutKeyMaskEx();
			minimize.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_M, menuMask));
			close.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_W, menuMask));

			if (owner == null)
			{
				minimize.setEnabled(false);
				zoom.setEnabled(false);
				close.setEnabled(false);
			}
			else
			{
				minimize.addActionListener(myListener);
				zoom.addActionListener(myListener);
				close.addActionListener(myListener);
			}

			computeEnabled();
			computeContents();

			LocaleManager.addLocaleListener(myListener);
			myListener.localeChanged();
		}

		internal virtual void addMenuItem(object source, WindowMenuItem item, bool persistent)
		{
			if (persistent)
			{
				persistentItems.Add(item);
			}
			else
			{
				transientItems.Add(item);
			}
			item.addActionListener(myListener);
			computeContents();
		}

		internal virtual void removeMenuItem(object source, JRadioButtonMenuItem item)
		{
			if (transientItems.Remove(item))
			{
				item.removeActionListener(myListener);
			}
			computeContents();
		}

		internal virtual void computeEnabled()
		{
			WindowMenuItemManager currentManager = WindowMenuManager.CurrentManager;
			minimize.setEnabled(currentManager != null);
			zoom.setEnabled(currentManager != null);
			close.setEnabled(currentManager != null);
		}

		internal virtual bool NullItemSelected
		{
			set
			{
				nullItem.setSelected(value);
			}
		}

		private void computeContents()
		{
			ButtonGroup bgroup = new ButtonGroup();
			bgroup.add(nullItem);

			removeAll();
			add(minimize);
			add(zoom);
			add(close);

			if (persistentItems.Count > 0)
			{
				addSeparator();
				foreach (JRadioButtonMenuItem item in persistentItems)
				{
					bgroup.add(item);
					add(item);
				}
			}

			if (transientItems.Count > 0)
			{
				addSeparator();
				foreach (JRadioButtonMenuItem item in transientItems)
				{
					bgroup.add(item);
					add(item);
				}
			}

			WindowMenuItemManager currentManager = WindowMenuManager.CurrentManager;
			if (currentManager != null)
			{
				JRadioButtonMenuItem item = currentManager.getMenuItem(this);
				if (item != null)
				{
					item.setSelected(true);
				}
			}
		}

		internal virtual void doMinimize()
		{
			if (owner != null)
			{
				owner.setExtendedState(Frame.ICONIFIED);
			}
		}

		internal virtual void doClose()
		{
			if (owner is WindowClosable)
			{
				((WindowClosable) owner).requestClose();
			}
			else if (owner != null)
			{
				int action = owner.getDefaultCloseOperation();
				if (action == JFrame.EXIT_ON_CLOSE)
				{
					Environment.Exit(0);
				}
				else if (action == JFrame.HIDE_ON_CLOSE)
				{
					owner.setVisible(false);
				}
				else if (action == JFrame.DISPOSE_ON_CLOSE)
				{
					owner.dispose();
				}
			}
		}

		internal virtual void doZoom()
		{
			if (owner == null)
			{
				return;
			}

			owner.pack();
			Size screenSize = owner.getToolkit().getScreenSize();
			Size windowSize = owner.getPreferredSize();
			Point windowLoc = owner.getLocation();

			bool locChanged = false;
			bool sizeChanged = false;
			if (windowLoc.x + windowsize.Width > screensize.Width)
			{
				windowLoc.x = Math.Max(0, screensize.Width - windowsize.Width);
				locChanged = true;
				if (windowLoc.x + windowsize.Width > screensize.Width)
				{
					windowsize.Width = screensize.Width - windowLoc.x;
					sizeChanged = true;
				}
			}
			if (windowLoc.y + windowsize.Height > screensize.Height)
			{
				windowLoc.y = Math.Max(0, screensize.Height - windowsize.Height);
				locChanged = true;
				if (windowLoc.y + windowsize.Height > screensize.Height)
				{
					windowsize.Height = screensize.Height - windowLoc.y;
					sizeChanged = true;
				}
			}

			if (locChanged)
			{
				owner.setLocation(windowLoc);
			}
			if (sizeChanged)
			{
				owner.setSize(windowSize);
			}
		}
	}

}
