// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.prefs
{

	using Direction = logisim.data.Direction;
	using AppPreferences = logisim.prefs.AppPreferences;
	using TableLayout = logisim.util.TableLayout;

	internal class WindowOptions : OptionsPanel
	{
		private PrefBoolean[] checks;
		private PrefOptionList toolbarPlacement;

		public WindowOptions(PreferencesFrame window) : base(window)
		{

			checks = new PrefBoolean[] {new PrefBoolean(AppPreferences.SHOW_TICK_RATE, Strings.getter("windowTickRate"))};

			toolbarPlacement = new PrefOptionList(AppPreferences.TOOLBAR_PLACEMENT, Strings.getter("windowToolbarLocation"), new PrefOption[]
			{
				new PrefOption(Direction.North.ToString(), Direction.North.toDisplayString()),
				new PrefOption(Direction.South.ToString(), Direction.South.toDisplayString()),
				new PrefOption(Direction.East.ToString(), Direction.East.toDisplayString()),
				new PrefOption(Direction.West.ToString(), Direction.West.toDisplayString()),
				new PrefOption(AppPreferences.TOOLBAR_DOWN_MIDDLE, Strings.get("windowToolbarDownMiddle")),
				new PrefOption(AppPreferences.TOOLBAR_HIDDEN, Strings.get("windowToolbarHidden"))
			});

			JPanel panel = new JPanel(new TableLayout(2));
			panel.add(toolbarPlacement.JLabel);
			panel.add(toolbarPlacement.JComboBox);

			setLayout(new TableLayout(1));
			for (int i = 0; i < checks.Length; i++)
			{
				add(checks[i]);
			}
			add(panel);
		}

		public override string Title
		{
			get
			{
				return Strings.get("windowTitle");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("windowHelp");
			}
		}

		public override void localeChanged()
		{
			for (int i = 0; i < checks.Length; i++)
			{
				checks[i].localeChanged();
			}
			toolbarPlacement.localeChanged();
		}
	}

}
