/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.prefs;

import java.util.prefs.PreferenceChangeEvent;
import java.util.prefs.Preferences;

class PrefMonitorInt extends AbstractPrefMonitor<Integer> {
	private int dflt;
	private int value;

	PrefMonitorInt(String name, int dflt) {
		super(name);
		this.dflt = dflt;
		value = dflt;
		Preferences prefs = AppPreferences.getPrefs();
		set(prefs.getInt(name, dflt));
		prefs.addPreferenceChangeListener(this);
	}

	public Integer get() {
		return value;
	}

	public void set(Integer newValue) {
		int newVal = newValue;
		if (value != newVal) AppPreferences.getPrefs().putInt(getIdentifier(), newVal);
	}

	public void preferenceChange(PreferenceChangeEvent event) {
		Preferences prefs = event.getNode();
		String prop = event.getKey();
		String name = getIdentifier();
		if (prop.equals(name)) {
			int oldValue = value;
			int newValue = prefs.getInt(name, dflt);
			if (newValue != oldValue) {
				value = newValue;
				AppPreferences.firePropertyChange(name, oldValue, newValue);
			}
		}
	}
}
