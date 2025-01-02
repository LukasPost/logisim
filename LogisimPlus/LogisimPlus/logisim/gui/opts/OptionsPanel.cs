// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{

	using LogisimFile = logisim.file.LogisimFile;
	using Options = logisim.file.Options;
	using Project = logisim.proj.Project;

	internal abstract class OptionsPanel : JPanel
	{
		private OptionsFrame optionsFrame;

		public OptionsPanel(OptionsFrame frame) : base()
		{
			this.optionsFrame = frame;
		}

		public OptionsPanel(OptionsFrame frame, LayoutManager manager) : base(manager)
		{
			this.optionsFrame = frame;
		}

		public abstract string Title {get;}

		public abstract string HelpText {get;}

		public abstract void localeChanged();

		internal virtual OptionsFrame OptionsFrame
		{
			get
			{
				return optionsFrame;
			}
		}

		internal virtual Project Project
		{
			get
			{
				return optionsFrame.Project;
			}
		}

		internal virtual LogisimFile LogisimFile
		{
			get
			{
				return optionsFrame.LogisimFile;
			}
		}

		internal virtual Options Options
		{
			get
			{
				return optionsFrame.Options;
			}
		}
	}

}
