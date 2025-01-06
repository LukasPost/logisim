/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.prefs;

import javax.swing.JPanel;

abstract class OptionsPanel extends JPanel {
	private PreferencesFrame optionsFrame;

	public OptionsPanel(PreferencesFrame frame) {
		super();
		optionsFrame = frame;
	}

	public abstract String getTitle();

	public abstract String getHelpText();

	public abstract void localeChanged();

	PreferencesFrame getPreferencesFrame() {
		return optionsFrame;
	}
}
