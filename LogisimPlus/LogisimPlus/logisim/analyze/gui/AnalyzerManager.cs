// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{

	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using WindowMenuItemManager = logisim.util.WindowMenuItemManager;

	public class AnalyzerManager : WindowMenuItemManager, LocaleListener
	{
		public static void initialize()
		{
			analysisManager = new AnalyzerManager();
		}

		public static Analyzer Analyzer
		{
			get
			{
				if (analysisWindow == null)
				{
					analysisWindow = new Analyzer();
					analysisWindow.pack();
					if (analysisManager != null)
					{
						analysisManager.frameOpened(analysisWindow);
					}
				}
				return analysisWindow;
			}
		}

		private static Analyzer analysisWindow = null;
		private static AnalyzerManager analysisManager = null;

		private AnalyzerManager() : base(Strings.get("analyzerWindowTitle"), true)
		{
			LocaleManager.addLocaleListener(this);
		}

		public override JFrame getJFrame(bool create)
		{
			if (create)
			{
				return Analyzer;
			}
			else
			{
				return analysisWindow;
			}
		}

		public virtual void localeChanged()
		{
			Text = Strings.get("analyzerWindowTitle");
		}
	}

}
