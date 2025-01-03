// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;
using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;
	using ProjectActions = logisim.proj.ProjectActions;

	internal class OpenRecent : JMenu, PropertyChangeListener
	{
		private const int MAX_ITEM_LENGTH = 50;

		private class RecentItem : JMenuItem, ActionListener
		{
			private readonly OpenRecent outerInstance;

			internal File file;

			internal RecentItem(OpenRecent outerInstance, File file) : base(getFileText(file))
			{
				this.outerInstance = outerInstance;
				this.file = file;
				setEnabled(file != null);
				addActionListener(this);
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				Project proj = outerInstance.menubar.Project;
				Component par = proj == null ? null : proj.Frame.getCanvas();
				ProjectActions.doOpen(par, proj, file);
			}
		}

		private LogisimMenuBar menubar;
		private List<RecentItem> recentItems;

		internal OpenRecent(LogisimMenuBar menubar)
		{
			this.menubar = menubar;
			this.recentItems = new List<RecentItem>();
			AppPreferences.addPropertyChangeListener(AppPreferences.RECENT_PROJECTS, this);
			renewItems();
		}

		private void renewItems()
		{
			for (int index = recentItems.Count - 1; index >= 0; index--)
			{
				RecentItem item = recentItems[index];
				remove(item);
			}
			recentItems.Clear();

			List<File> files = AppPreferences.RecentFiles;
			if (files.Count == 0)
			{
				recentItems.Add(new RecentItem(this, null));
			}
			else
			{
				foreach (File file in files)
				{
					recentItems.Add(new RecentItem(this, file));
				}
			}

			foreach (RecentItem item in recentItems)
			{
				add(item);
			}
		}

		private static string getFileText(File file)
		{
			if (file == null)
			{
				return Strings.get("fileOpenRecentNoChoices");
			}
			else
			{
				string ret;
				try
				{
					ret = file.getCanonicalPath();
				}
				catch (IOException)
				{
					ret = file.ToString();
				}
				if (ret.Length <= MAX_ITEM_LENGTH)
				{
					return ret;
				}
				else
				{
					ret = ret.Substring(ret.Length - MAX_ITEM_LENGTH + 3);
					int splitLoc = ret.IndexOf(Path.DirectorySeparatorChar);
					if (splitLoc >= 0)
					{
						ret = ret.Substring(splitLoc);
					}
					return "..." + ret;
				}
			}
		}

		internal virtual void localeChanged()
		{
			setText(Strings.get("fileOpenRecentItem"));
			foreach (RecentItem item in recentItems)
			{
				if (item.file == null)
				{
					item.setText(Strings.get("fileOpenRecentNoChoices"));
				}
			}
		}

		public virtual void propertyChange(PropertyChangeEvent @event)
		{
			if (@event.getPropertyName().Equals(AppPreferences.RECENT_PROJECTS))
			{
				renewItems();
			}
		}
	}

}
