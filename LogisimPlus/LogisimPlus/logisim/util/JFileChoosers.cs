// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	using AppPreferences = logisim.prefs.AppPreferences;

	public class JFileChoosers
	{
		/*
		 * A user reported that JFileChooser's constructor sometimes resulted in IOExceptions when Logisim is installed
		 * under a system administrator account and then is attempted to run as a regular user. This class is an attempt to
		 * be a bit more robust about which directory the JFileChooser opens up under. (23 Feb 2010)
		 */
		private class LogisimFileChooser : JFileChooser
		{
			internal LogisimFileChooser() : base()
			{
			}

			internal LogisimFileChooser(File initSelected) : base(initSelected)
			{
			}

			public override File SelectedFile
			{
				get
				{
					File dir = CurrentDirectory;
					if (dir != null)
					{
						JFileChoosers.currentDirectory = dir.ToString();
					}
					return base.getSelectedFile();
				}
			}
		}

		private static readonly string[] PROP_NAMES = new string[] {null, "user.home", "user.dir", "java.home", "java.io.tmpdir"};

		private static string currentDirectory = "";

		private JFileChoosers()
		{
		}

		public static string CurrentDirectory
		{
			get
			{
				return currentDirectory;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") public static javax.swing.JFileChooser create()
		public static JFileChooser create()
		{
			Exception first = null;
			for (int i = 0; i < PROP_NAMES.Length; i++)
			{
				string prop = PROP_NAMES[i];
				try
				{
					string dirname;
					if (string.ReferenceEquals(prop, null))
					{
						dirname = currentDirectory;
						if (dirname.Equals(""))
						{
							dirname = AppPreferences.DIALOG_DIRECTORY.get();
						}
					}
					else
					{
						dirname = System.getProperty(prop);
					}
					if (dirname.Equals(""))
					{
						return new LogisimFileChooser();
					}
					else
					{
						File dir = new File(dirname);
						if (dir.canRead())
						{
							return new LogisimFileChooser(dir);
						}
					}
				}
				catch (Exception t)
				{
					if (first == null)
					{
						first = t;
					}
					Exception u = t.InnerException;
					if (!(u is IOException))
					{
						throw t;
					}
				}
			}
			throw first;
		}

		public static JFileChooser createAt(File openDirectory)
		{
			if (openDirectory == null)
			{
				return create();
			}
			else
			{
				try
				{
					return new LogisimFileChooser(openDirectory);
				}
				catch (Exception t)
				{
					if (t.InnerException is IOException)
					{
						try
						{
							return create();
						}
						catch (Exception)
						{
						}
					}
					throw t;
				}
			}
		}

		public static JFileChooser createSelected(File selected)
		{
			if (selected == null)
			{
				return create();
			}
			else
			{
				JFileChooser ret = createAt(selected.getParentFile());
				ret.setSelectedFile(selected);
				return ret;
			}
		}
	}

}
