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

	internal class ExperimentalOptions : OptionsPanel
	{
		private JLabel accelRestart = new JLabel();
		private PrefOptionList accel;

		public ExperimentalOptions(PreferencesFrame window) : base(window)
		{

			accel = new PrefOptionList(AppPreferences.GRAPHICS_ACCELERATION, Strings.getter("accelLabel"), new PrefOption[]
			{
				new PrefOption(AppPreferences.ACCEL_DEFAULT, Strings.get("accelDefault")),
				new PrefOption(AppPreferences.ACCEL_NONE, Strings.get("accelNone")),
				new PrefOption(AppPreferences.ACCEL_OPENGL, Strings.get("accelOpenGL")),
				new PrefOption(AppPreferences.ACCEL_D3D, Strings.get("accelD3D"))
			});

			JPanel accelPanel = new JPanel(new BorderLayout());
			accelPanel.add(accel.JLabel, BorderLayout.LINE_START);
			accelPanel.add(accel.JComboBox, BorderLayout.CENTER);
			accelPanel.add(accelRestart, BorderLayout.PAGE_END);
			accelRestart.setFont(accelRestart.getFont().deriveFont(Font.ITALIC));
			JPanel accelPanel2 = new JPanel();
			accelPanel2.add(accelPanel);

			setLayout(new BoxLayout(this, BoxLayout.PAGE_AXIS));
			add(Box.createGlue());
			add(accelPanel2);
			add(Box.createGlue());
		}

		public override string Title
		{
			get
			{
				return Strings.get("experimentTitle");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("experimentHelp");
			}
		}

		public override void localeChanged()
		{
			accel.localeChanged();
			accelRestart.setText(Strings.get("accelRestartLabel"));
		}
	}

}
