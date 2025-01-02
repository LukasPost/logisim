// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.prefs
{

	internal class RecentProjects : PreferenceChangeListener
	{
		private const string BASE_PROPERTY = "recent";
		private const int NUM_RECENT = 10;

		private class FileTime
		{
			internal long time;
			internal File file;

			public FileTime(File file, long time)
			{
				this.time = time;
				this.file = file;
			}

			public override bool Equals(object other)
			{
				if (other is FileTime)
				{
					FileTime o = (FileTime) other;
					return this.time == o.time && isSame(this.file, o.file);
				}
				else
				{
					return false;
				}
			}
		}

		private File[] recentFiles;
		private long[] recentTimes;

		internal RecentProjects()
		{
			recentFiles = new File[NUM_RECENT];
			recentTimes = new long[NUM_RECENT];
			Arrays.Fill(recentTimes, DateTimeHelper.CurrentUnixTimeMillis());

			Preferences prefs = AppPreferences.Prefs;
			prefs.addPreferenceChangeListener(this);

			for (int index = 0; index < NUM_RECENT; index++)
			{
				getAndDecode(prefs, index);
			}
		}

		public virtual IList<File> RecentFiles
		{
			get
			{
				long now = DateTimeHelper.CurrentUnixTimeMillis();
				long[] ages = new long[NUM_RECENT];
				long[] toSort = new long[NUM_RECENT];
				for (int i = 0; i < NUM_RECENT; i++)
				{
					if (recentFiles[i] == null)
					{
						ages[i] = -1;
					}
					else
					{
						ages[i] = now - recentTimes[i];
					}
					toSort[i] = ages[i];
				}
				Array.Sort(toSort);
    
				IList<File> ret = new List<File>();
				foreach (long age in toSort)
				{
					if (age >= 0)
					{
						int index = -1;
						for (int i = 0; i < NUM_RECENT; i++)
						{
							if (ages[i] == age)
							{
								index = i;
								ages[i] = -1;
								break;
							}
						}
						if (index >= 0)
						{
							ret.Add(recentFiles[index]);
						}
					}
				}
				return ret;
			}
		}

		public virtual void updateRecent(File file)
		{
			File fileToSave = file;
			try
			{
				fileToSave = file.getCanonicalFile();
			}
			catch (IOException)
			{
			}
			long now = DateTimeHelper.CurrentUnixTimeMillis();
			int index = getReplacementIndex(now, fileToSave);
			updateInto(index, now, fileToSave);
		}

		private int getReplacementIndex(long now, File f)
		{
			long oldestAge = -1;
			int oldestIndex = 0;
			int nullIndex = -1;
			for (int i = 0; i < NUM_RECENT; i++)
			{
				if (f.Equals(recentFiles[i]))
				{
					return i;
				}
				if (recentFiles[i] == null)
				{
					nullIndex = i;
				}
				long age = now - recentTimes[i];
				if (age > oldestAge)
				{
					oldestIndex = i;
					oldestAge = age;
				}
			}
			if (nullIndex != -1)
			{
				return nullIndex;
			}
			else
			{
				return oldestIndex;
			}
		}

		public virtual void preferenceChange(PreferenceChangeEvent @event)
		{
			Preferences prefs = @event.getNode();
			string prop = @event.getKey();
			if (prop.StartsWith(BASE_PROPERTY, StringComparison.Ordinal))
			{
				string rest = prop.Substring(BASE_PROPERTY.Length);
				int index = -1;
				try
				{
					index = int.Parse(rest);
					if (index < 0 || index >= NUM_RECENT)
					{
						index = -1;
					}
				}
				catch (System.FormatException)
				{
				}
				if (index >= 0)
				{
					File oldValue = recentFiles[index];
					long oldTime = recentTimes[index];
					getAndDecode(prefs, index);
					File newValue = recentFiles[index];
					long newTime = recentTimes[index];
					if (!isSame(oldValue, newValue) || oldTime != newTime)
					{
						AppPreferences.firePropertyChange(AppPreferences.RECENT_PROJECTS, new FileTime(oldValue, oldTime), new FileTime(newValue, newTime));
					}
				}
			}
		}

		private void updateInto(int index, long time, File file)
		{
			File oldFile = recentFiles[index];
			long oldTime = recentTimes[index];
			if (!isSame(oldFile, file) || oldTime != time)
			{
				recentFiles[index] = file;
				recentTimes[index] = time;
				try
				{
					AppPreferences.Prefs.put(BASE_PROPERTY + index, "" + time + ";" + file.getCanonicalPath());
					AppPreferences.firePropertyChange(AppPreferences.RECENT_PROJECTS, new FileTime(oldFile, oldTime), new FileTime(file, time));
				}
				catch (IOException)
				{
					recentFiles[index] = oldFile;
					recentTimes[index] = oldTime;
				}
			}
		}

		private void getAndDecode(Preferences prefs, int index)
		{
			string encoding = prefs.get(BASE_PROPERTY + index, null);
			if (string.ReferenceEquals(encoding, null))
			{
				return;
			}
			int semi = encoding.IndexOf(';');
			if (semi < 0)
			{
				return;
			}
			try
			{
				long time = long.Parse(encoding.Substring(0, semi));
				File file = new File(encoding.Substring(semi + 1));
				updateInto(index, time, file);
			}
			catch (System.FormatException)
			{
			}
		}

		private static bool isSame(object a, object b)
		{
			return a == null ? b == null : a.Equals(b);
		}
	}

}
