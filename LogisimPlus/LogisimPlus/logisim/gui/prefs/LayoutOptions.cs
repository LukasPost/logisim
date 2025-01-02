// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.prefs
{

	using RadixOption = logisim.circuit.RadixOption;
	using AppPreferences = logisim.prefs.AppPreferences;
	using TableLayout = logisim.util.TableLayout;

	internal class LayoutOptions : OptionsPanel
	{
		private PrefBoolean[] checks;
		private PrefOptionList afterAdd;
		private PrefOptionList radix1;
		private PrefOptionList radix2;

		public LayoutOptions(PreferencesFrame window) : base(window)
		{

			checks = new PrefBoolean[]
			{
				new PrefBoolean(AppPreferences.PRINTER_VIEW, Strings.getter("layoutPrinterView")),
				new PrefBoolean(AppPreferences.ATTRIBUTE_HALO, Strings.getter("layoutAttributeHalo")),
				new PrefBoolean(AppPreferences.COMPONENT_TIPS, Strings.getter("layoutShowTips")),
				new PrefBoolean(AppPreferences.MOVE_KEEP_CONNECT, Strings.getter("layoutMoveKeepConnect")),
				new PrefBoolean(AppPreferences.ADD_SHOW_GHOSTS, Strings.getter("layoutAddShowGhosts"))
			};

			for (int i = 0; i < 2; i++)
			{
				RadixOption[] opts = RadixOption.OPTIONS;
				PrefOption[] items = new PrefOption[opts.Length];
				for (int j = 0; j < RadixOption.OPTIONS.Length; j++)
				{
					items[j] = new PrefOption(opts[j].SaveString, opts[j].toDisplayString());
				}
				if (i == 0)
				{
					radix1 = new PrefOptionList(AppPreferences.POKE_WIRE_RADIX1, Strings.getter("layoutRadix1"), items);
				}
				else
				{
					radix2 = new PrefOptionList(AppPreferences.POKE_WIRE_RADIX2, Strings.getter("layoutRadix2"), items);
				}
			}
			afterAdd = new PrefOptionList(AppPreferences.ADD_AFTER, Strings.getter("layoutAddAfter"), new PrefOption[]
			{
				new PrefOption(AppPreferences.ADD_AFTER_UNCHANGED, Strings.get("layoutAddAfterUnchanged")),
				new PrefOption(AppPreferences.ADD_AFTER_EDIT, Strings.get("layoutAddAfterEdit"))
			});

			JPanel panel = new JPanel(new TableLayout(2));
			panel.add(afterAdd.JLabel);
			panel.add(afterAdd.JComboBox);
			panel.add(radix1.JLabel);
			panel.add(radix1.JComboBox);
			panel.add(radix2.JLabel);
			panel.add(radix2.JComboBox);

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
				return Strings.get("layoutTitle");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("layoutHelp");
			}
		}

		public override void localeChanged()
		{
			for (int i = 0; i < checks.Length; i++)
			{
				checks[i].localeChanged();
			}
			radix1.localeChanged();
			radix2.localeChanged();
		}
	}

}
