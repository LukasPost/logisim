// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{

	internal class ScrollPanel : LogPanel
	{
		private TablePanel table;

		public ScrollPanel(LogFrame frame) : base(frame)
		{
			this.table = new TablePanel(frame);
			JScrollPane pane = new JScrollPane(table, JScrollPane.VERTICAL_SCROLLBAR_ALWAYS, JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			pane.setVerticalScrollBar(table.VerticalScrollBar);
			setLayout(new BorderLayout());
			add(pane);
		}

		public override string Title
		{
			get
			{
				return table.Title;
			}
		}

		public override string HelpText
		{
			get
			{
				return table.HelpText;
			}
		}

		public override void localeChanged()
		{
			table.localeChanged();
		}

		public override void modelChanged(Model oldModel, Model newModel)
		{
			table.modelChanged(oldModel, newModel);
		}
	}

}
