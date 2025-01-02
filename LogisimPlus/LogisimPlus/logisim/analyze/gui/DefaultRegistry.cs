// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{


	internal class DefaultRegistry
	{
		private class MyListener : FocusListener
		{
			private readonly DefaultRegistry outerInstance;

			internal JButton defaultButton;

			internal MyListener(DefaultRegistry outerInstance, JButton defaultButton)
			{
				this.outerInstance = outerInstance;
				this.defaultButton = defaultButton;
			}

			public virtual void focusGained(FocusEvent @event)
			{
				outerInstance.rootPane.setDefaultButton(defaultButton);
			}

			public virtual void focusLost(FocusEvent @event)
			{
				JButton currentDefault = outerInstance.rootPane.getDefaultButton();
				if (currentDefault == defaultButton)
				{
					outerInstance.rootPane.setDefaultButton(null);
				}
			}
		}

		private JRootPane rootPane;

		public DefaultRegistry(JRootPane rootPane)
		{
			this.rootPane = rootPane;
			rootPane.setDefaultButton(null);
		}

		public virtual void registerDefaultButton(JComponent comp, JButton button)
		{
			comp.addFocusListener(new MyListener(this, button));
		}
	}

}
