/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.prefs;

import java.util.prefs.PreferenceChangeEvent;
import java.util.prefs.Preferences;

class PrefMonitorBoolean extends AbstractPrefMonitor<Boolean> {
	private boolean dflt;
	private boolean value;

	PrefMonitorBoolean(String name, boolean dflt) {
		super(name);
		this.dflt = dflt;
		value = dflt;
		Preferences prefs = AppPreferences.getPrefs();
		set(prefs.getBoolean(name, dflt));
		prefs.addPreferenceChangeListener(this);
	}

	public Boolean get() {
		return value;
	}

	@Override
	public boolean getBoolean() {
		return value;
	}

	public void set(Boolean newValue) {
		boolean newVal = newValue;
		if (value != newVal) AppPreferences.getPrefs().putBoolean(getIdentifier(), newVal);
	}

	public void preferenceChange(PreferenceChangeEvent event) {
		Preferences prefs = event.getNode();
		String prop = event.getKey();
		String name = getIdentifier();
		if (prop.equals(name)) {
			boolean oldValue = value;
			boolean newValue = prefs.getBoolean(name, dflt);
			if (newValue != oldValue) {
				value = newValue;
				AppPreferences.firePropertyChange(name, oldValue, newValue);
			}
		}
	}
}
