// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{


	internal class WindowMenuItem : JRadioButtonMenuItem
	{
		private WindowMenuItemManager manager;

		internal WindowMenuItem(WindowMenuItemManager manager)
		{
			this.manager = manager;
			setText(manager.Text);
			setSelected(WindowMenuManager.CurrentManager == manager);
		}

		public virtual JFrame JFrame
		{
			get
			{
				return manager.getJFrame(true);
			}
		}

		public virtual void actionPerformed(ActionEvent @event)
		{
			JFrame frame = JFrame;
			frame.setExtendedState(Frame.NORMAL);
			frame.setVisible(true);
			frame.toFront();
		}
	}

}
