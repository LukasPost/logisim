/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.prefs;

import javax.swing.JPanel;

import logisim.circuit.RadixOption;
import logisim.prefs.AppPreferences;
import logisim.util.TableLayout;

class LayoutOptions extends OptionsPanel {
	private PrefBoolean[] checks;
	private PrefOptionList radix1;
	private PrefOptionList radix2;

	public LayoutOptions(PreferencesFrame window) {
		super(window);

		checks = new PrefBoolean[] { new PrefBoolean(AppPreferences.PRINTER_VIEW, Strings.getter("layoutPrinterView")),
				new PrefBoolean(AppPreferences.ATTRIBUTE_HALO, Strings.getter("layoutAttributeHalo")),
				new PrefBoolean(AppPreferences.COMPONENT_TIPS, Strings.getter("layoutShowTips")),
				new PrefBoolean(AppPreferences.MOVE_KEEP_CONNECT, Strings.getter("layoutMoveKeepConnect")),
				new PrefBoolean(AppPreferences.ADD_SHOW_GHOSTS, Strings.getter("layoutAddShowGhosts")), };

		for (int i = 0; i < 2; i++) {
			RadixOption[] opts = RadixOption.OPTIONS;
			PrefOption[] items = new PrefOption[opts.length];
			for (int j = 0; j < RadixOption.OPTIONS.length; j++)
				items[j] = new PrefOption(opts[j].getSaveString(), opts[j].toDisplayString());
			if (i == 0)
				radix1 = new PrefOptionList(AppPreferences.POKE_WIRE_RADIX1, Strings.getter("layoutRadix1"), items);
			else radix2 = new PrefOptionList(AppPreferences.POKE_WIRE_RADIX2, Strings.getter("layoutRadix2"), items);
		}
		PrefOptionList afterAdd = new PrefOptionList(AppPreferences.ADD_AFTER, Strings.getter("layoutAddAfter"),
				new PrefOption[]{
						new PrefOption(AppPreferences.ADD_AFTER_UNCHANGED, Strings.get("layoutAddAfterUnchanged")),
						new PrefOption(AppPreferences.ADD_AFTER_EDIT, Strings.get("layoutAddAfterEdit"))});

		JPanel panel = new JPanel(new TableLayout(2));
		panel.add(afterAdd.getJLabel());
		panel.add(afterAdd.getJComboBox());
		panel.add(radix1.getJLabel());
		panel.add(radix1.getJComboBox());
		panel.add(radix2.getJLabel());
		panel.add(radix2.getJComboBox());

		setLayout(new TableLayout(1));
		for (PrefBoolean check : checks) add(check);
		add(panel);
	}

	@Override
	public String getTitle() {
		return Strings.get("layoutTitle");
	}

	@Override
	public String getHelpText() {
		return Strings.get("layoutHelp");
	}

	@Override
	public void localeChanged() {
		for (PrefBoolean check : checks) check.localeChanged();
		radix1.localeChanged();
		radix2.localeChanged();
	}
}
