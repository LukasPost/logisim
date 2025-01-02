// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.prefs
{


	using AppPreferences = logisim.prefs.AppPreferences;
	using LocaleManager = logisim.util.LocaleManager;

	internal class IntlOptions : OptionsPanel
	{
		private class RestrictedLabel : JLabel
		{
			public override Dimension MaximumSize
			{
				get
				{
					return getPreferredSize();
				}
			}
		}

		private JLabel localeLabel = new RestrictedLabel();
		private JComponent locale;
		private PrefBoolean replAccents;
		private PrefOptionList gateShape;

		public IntlOptions(PreferencesFrame window) : base(window)
		{

			locale = Strings.createLocaleSelector();
			replAccents = new PrefBoolean(AppPreferences.ACCENTS_REPLACE, Strings.getter("intlReplaceAccents"));
			gateShape = new PrefOptionList(AppPreferences.GATE_SHAPE, Strings.getter("intlGateShape"), new PrefOption[]
			{
				new PrefOption(AppPreferences.SHAPE_SHAPED, Strings.get("shapeShaped")),
				new PrefOption(AppPreferences.SHAPE_RECTANGULAR, Strings.get("shapeRectangular")),
				new PrefOption(AppPreferences.SHAPE_DIN40700, Strings.get("shapeDIN40700"))
			});

			Box localePanel = new Box(BoxLayout.X_AXIS);
			localePanel.add(Box.createGlue());
			localePanel.add(localeLabel);
			localeLabel.setMaximumSize(localeLabel.getPreferredSize());
			localeLabel.setAlignmentY(Component.TOP_ALIGNMENT);
			localePanel.add(locale);
			locale.setAlignmentY(Component.TOP_ALIGNMENT);
			localePanel.add(Box.createGlue());

			JPanel shapePanel = new JPanel();
			shapePanel.add(gateShape.JLabel);
			shapePanel.add(gateShape.JComboBox);

			setLayout(new BoxLayout(this, BoxLayout.PAGE_AXIS));
			add(Box.createGlue());
			add(shapePanel);
			add(localePanel);
			add(replAccents);
			add(Box.createGlue());
		}

		public override string Title
		{
			get
			{
				return Strings.get("intlTitle");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("intlHelp");
			}
		}

		public override void localeChanged()
		{
			gateShape.localeChanged();
			localeLabel.setText(Strings.get("intlLocale") + " ");
			replAccents.localeChanged();
			replAccents.setEnabled(LocaleManager.canReplaceAccents());
		}
	}

}
