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

	internal class WindowMenuManager
	{
		private WindowMenuManager()
		{
		}

		private static List<WindowMenu> menus = new List<WindowMenu>();
		private static List<WindowMenuItemManager> managers = new List<WindowMenuItemManager>();
		private static WindowMenuItemManager currentManager = null;

		public static void addMenu(WindowMenu menu)
		{
			foreach (WindowMenuItemManager manager in managers)
			{
				manager.createMenuItem(menu);
			}
			menus.Add(menu);
		}

		// TODO frames should call removeMenu when they're destroyed

		public static void addManager(WindowMenuItemManager manager)
		{
			foreach (WindowMenu menu in menus)
			{
				manager.createMenuItem(menu);
			}
			managers.Add(manager);
		}

		public static void removeManager(WindowMenuItemManager manager)
		{
			foreach (WindowMenu menu in menus)
			{
				manager.removeMenuItem(menu);
			}
			managers.Remove(manager);
		}

		internal static IList<WindowMenu> Menus
		{
			get
			{
				return menus;
			}
		}

		internal static WindowMenuItemManager CurrentManager
		{
			get
			{
				return currentManager;
			}
			set
			{
				if (value == currentManager)
				{
					return;
				}
    
				bool doEnable = (currentManager == null) != (value == null);
				if (currentManager == null)
				{
					NullItems = false;
				}
				else
				{
					currentManager.Selected = false;
				}
				currentManager = value;
				if (currentManager == null)
				{
					NullItems = true;
				}
				else
				{
					currentManager.Selected = true;
				}
				if (doEnable)
				{
					enableAll();
				}
			}
		}


		internal static void unsetCurrentManager(WindowMenuItemManager value)
		{
			if (value != currentManager)
			{
				return;
			}
			CurrentManager = null;
		}

		private static bool NullItems
		{
			set
			{
				foreach (WindowMenu menu in menus)
				{
					menu.NullItemSelected = value;
				}
			}
		}

		private static void enableAll()
		{
			foreach (WindowMenu menu in menus)
			{
				menu.computeEnabled();
			}
		}
	}

}
