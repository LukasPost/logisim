// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.prefs
{

	internal abstract class OptionsPanel : JPanel
	{
		private PreferencesFrame optionsFrame;

		public OptionsPanel(PreferencesFrame frame) : base()
		{
			this.optionsFrame = frame;
		}

		public OptionsPanel(PreferencesFrame frame, LayoutManager manager) : base(manager)
		{
			this.optionsFrame = frame;
		}

		public abstract string Title {get;}

		public abstract string HelpText {get;}

		public abstract void localeChanged();

		internal virtual PreferencesFrame PreferencesFrame
		{
			get
			{
				return optionsFrame;
			}
		}
	}

}
