// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{

	using Project = logisim.proj.Project;

	internal abstract class LogPanel : JPanel
	{
		private LogFrame logFrame;

		public LogPanel(LogFrame frame) : base()
		{
			this.logFrame = frame;
		}

		public LogPanel(LogFrame frame, LayoutManager manager) : base(manager)
		{
			this.logFrame = frame;
		}

		public abstract string Title {get;}

		public abstract string HelpText {get;}

		public abstract void localeChanged();

		public abstract void modelChanged(Model oldModel, Model newModel);

		internal virtual LogFrame LogFrame
		{
			get
			{
				return logFrame;
			}
		}

		internal virtual Project Project
		{
			get
			{
				return logFrame.Project;
			}
		}

		internal virtual Model Model
		{
			get
			{
				return logFrame.Model;
			}
		}

		internal virtual Selection Selection
		{
			get
			{
				return logFrame.Model.Selection;
			}
		}
	}

}
