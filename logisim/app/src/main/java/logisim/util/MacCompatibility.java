/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

public class MacCompatibility {
	private MacCompatibility() {
	}

	public static final double mrjVersion;

	static {
		double versionValue;
		try {
			versionValue = 0;// MRJAdapter.mrjVersion;
		}
		catch (Throwable t) {
			versionValue = 0.0;
		}
		mrjVersion = versionValue;
	}

	public static boolean isAboutAutomaticallyPresent() {
		try {
			return false;// MRJAdapter.isAboutAutomaticallyPresent();
		}
		catch (Throwable t) {
			return false;
		}
	}

	public static boolean isPreferencesAutomaticallyPresent() {
		try {
			return false;// MRJAdapter.isPreferencesAutomaticallyPresent();
		}
		catch (Throwable t) {
			return false;
		}
	}

	public static boolean isQuitAutomaticallyPresent() {
		try {
			return false;// MRJAdapter.isQuitAutomaticallyPresent();
		}
		catch (Throwable t) {
			return false;
		}
	}

	public static boolean isSwingUsingScreenMenuBar() {
		try {
			return false;// MRJAdapter.isSwingUsingScreenMenuBar();
		}
		catch (Throwable t) {
			return false;
		}
	}
}
