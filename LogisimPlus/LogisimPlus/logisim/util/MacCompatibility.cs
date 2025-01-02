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
	public class MacCompatibility
	{
		private MacCompatibility()
		{
		}

		public static readonly double mrjVersion;

		static MacCompatibility()
		{
			double versionValue;
			try
			{
				versionValue = 0; // MRJAdapter.mrjVersion;
			}
			catch (Exception)
			{
				versionValue = 0.0;
			}
			mrjVersion = versionValue;
		}

		public static bool AboutAutomaticallyPresent
		{
			get
			{
				try
				{
					return false; // MRJAdapter.isAboutAutomaticallyPresent();
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public static bool PreferencesAutomaticallyPresent
		{
			get
			{
				try
				{
					return false; // MRJAdapter.isPreferencesAutomaticallyPresent();
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public static bool QuitAutomaticallyPresent
		{
			get
			{
				try
				{
					return false; // MRJAdapter.isQuitAutomaticallyPresent();
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public static bool SwingUsingScreenMenuBar
		{
			get
			{
				try
				{
					return false; // MRJAdapter.isSwingUsingScreenMenuBar();
				}
				catch (Exception)
				{
					return false;
				}
			}
		}
	}

}
