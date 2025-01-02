// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.prefs
{


	using LFrame = logisim.gui.generic.LFrame;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using WindowMenuItemManager = logisim.util.WindowMenuItemManager;

	public class PreferencesFrame : LFrame
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private static WindowMenuManager MENU_MANAGER = null;

		public static void initializeManager()
		{
			MENU_MANAGER = new WindowMenuManager();
		}

		private class WindowMenuManager : WindowMenuItemManager, LocaleListener
		{
			internal PreferencesFrame window = null;

			internal WindowMenuManager() : base(Strings.get("preferencesFrameMenuItem"), true)
			{
				LocaleManager.addLocaleListener(this);
			}

			public override JFrame getJFrame(bool create)
			{
				if (create)
				{
					if (window == null)
					{
						window = new PreferencesFrame();
						frameOpened(window);
					}
				}
				return window;
			}

			public virtual void localeChanged()
			{
				Text = Strings.get("preferencesFrameMenuItem");
			}
		}

		private class MyListener : ActionListener, LocaleListener
		{
			private readonly PreferencesFrame outerInstance;

			public MyListener(PreferencesFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.close)
				{
					WindowEvent e = new WindowEvent(outerInstance, WindowEvent.WINDOW_CLOSING);
					outerInstance.processWindowEvent(e);
				}
			}

			public virtual void localeChanged()
			{
				setTitle(Strings.get("preferencesFrameTitle"));
				for (int i = 0; i < outerInstance.panels.Length; i++)
				{
					outerInstance.tabbedPane.setTitleAt(i, outerInstance.panels[i].Title);
					outerInstance.tabbedPane.setToolTipTextAt(i, outerInstance.panels[i].getToolTipText());
					outerInstance.panels[i].localeChanged();
				}
				outerInstance.close.setText(Strings.get("closeButton"));
			}
		}

		private MyListener myListener;

		private OptionsPanel[] panels;
		private JTabbedPane tabbedPane;
		private JButton close = new JButton();

		private PreferencesFrame()
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			setDefaultCloseOperation(HIDE_ON_CLOSE);
			setJMenuBar(new LogisimMenuBar(this, null));

			panels = new OptionsPanel[]
			{
				new TemplateOptions(this),
				new IntlOptions(this),
				new WindowOptions(this),
				new LayoutOptions(this),
				new ExperimentalOptions(this)
			};
			tabbedPane = new JTabbedPane();
			int intlIndex = -1;
			for (int index = 0; index < panels.Length; index++)
			{
				OptionsPanel panel = panels[index];
				tabbedPane.addTab(panel.Title, null, panel, panel.getToolTipText());
				if (panel is IntlOptions)
				{
					intlIndex = index;
				}
			}

			JPanel buttonPanel = new JPanel();
			buttonPanel.add(close);
			close.addActionListener(myListener);

			Container contents = getContentPane();
			tabbedPane.setPreferredSize(new Size(450, 300));
			contents.add(tabbedPane, BorderLayout.CENTER);
			contents.add(buttonPanel, BorderLayout.SOUTH);

			if (intlIndex >= 0)
			{
				tabbedPane.setSelectedIndex(intlIndex);
			}

			LocaleManager.addLocaleListener(myListener);
			myListener.localeChanged();
			pack();
		}

		public static void showPreferences()
		{
			JFrame frame = MENU_MANAGER.getJFrame(true);
			frame.setVisible(true);
		}
	}

}
