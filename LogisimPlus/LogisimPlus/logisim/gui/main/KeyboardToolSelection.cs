// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{


	using Toolbar = draw.toolbar.Toolbar;
	using ToolbarItem = draw.toolbar.ToolbarItem;
	using ToolbarModel = draw.toolbar.ToolbarModel;

	public class KeyboardToolSelection : AbstractAction
	{
		public static void register(Toolbar toolbar)
		{
			ActionMap amap = toolbar.getActionMap();
			InputMap imap = toolbar.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW);
			int mask = toolbar.getToolkit().getMenuShortcutKeyMaskEx();
			for (int i = 0; i < 10; i++)
			{
				KeyStroke keyStroke = KeyStroke.getKeyStroke((char)('0' + i), mask);
				int j = (i == 0 ? 10 : i - 1);
				KeyboardToolSelection action = new KeyboardToolSelection(toolbar, j);
				string key = "ToolSelect" + i;
				amap.put(key, action);
				imap.put(keyStroke, key);
			}
		}

		private Toolbar toolbar;
		private int index;

		public KeyboardToolSelection(Toolbar toolbar, int index)
		{
			this.toolbar = toolbar;
			this.index = index;
		}

		public virtual void actionPerformed(ActionEvent @event)
		{
			ToolbarModel model = toolbar.ToolbarModel;
			int i = -1;
			foreach (ToolbarItem item in model.Items)
			{
				if (item.Selectable)
				{
					i++;
					if (i == index)
					{
						model.itemSelected(item);
					}
				}
			}
		}
	}

}
