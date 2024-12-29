/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.prefs;

import javax.swing.JPanel;

import logisim.data.Direction;
import logisim.prefs.AppPreferences;
import logisim.util.TableLayout;

class WindowOptions extends OptionsPanel {
	private PrefBoolean[] checks;
	private PrefOptionList toolbarPlacement;

	public WindowOptions(PreferencesFrame window) {
		super(window);

		checks = new PrefBoolean[] {
				new PrefBoolean(AppPreferences.SHOW_TICK_RATE, Strings.getter("windowTickRate")), };

		toolbarPlacement = new PrefOptionList(AppPreferences.TOOLBAR_PLACEMENT, Strings.getter("windowToolbarLocation"),
				new PrefOption[] { new PrefOption(Direction.North.toString(), Direction.North.toDisplayString()),
						new PrefOption(Direction.South.toString(), Direction.South.toDisplayString()),
						new PrefOption(Direction.East.toString(), Direction.East.toDisplayString()),
						new PrefOption(Direction.West.toString(), Direction.West.toDisplayString()),
						new PrefOption(AppPreferences.TOOLBAR_DOWN_MIDDLE, Strings.get("windowToolbarDownMiddle")),
						new PrefOption(AppPreferences.TOOLBAR_HIDDEN, Strings.get("windowToolbarHidden")) });

		JPanel panel = new JPanel(new TableLayout(2));
		panel.add(toolbarPlacement.getJLabel());
		panel.add(toolbarPlacement.getJComboBox());

		setLayout(new TableLayout(1));
		for (int i = 0; i < checks.length; i++) {
			add(checks[i]);
		}
		add(panel);
	}

	@Override
	public String getTitle() {
		return Strings.get("windowTitle");
	}

	@Override
	public String getHelpText() {
		return Strings.get("windowHelp");
	}

	@Override
	public void localeChanged() {
		for (int i = 0; i < checks.length; i++) {
			checks[i].localeChanged();
		}
		toolbarPlacement.localeChanged();
	}
}
