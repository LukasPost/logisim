/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim;

public class LogisimVersion {
	private static final int FINAL_REVISION = Integer.MAX_VALUE / 4;

	public static LogisimVersion get(int major, int minor, int release) {
		return get(major, minor, release, FINAL_REVISION);
	}

	public static LogisimVersion get(int major, int minor, int release, int revision) {
		return new LogisimVersion(major, minor, release, revision);
	}

	public static LogisimVersion parse(String versionString) {
		String[] parts = versionString.split("\\.");
		int major = 0;
		int minor = 0;
		int release = 0;
		int revision = FINAL_REVISION;
		try {
			if (parts.length >= 1)
				major = Integer.parseInt(parts[0]);
			if (parts.length >= 2)
				minor = Integer.parseInt(parts[1]);
			if (parts.length >= 3)
				release = Integer.parseInt(parts[2]);
			if (parts.length >= 4)
				revision = Integer.parseInt(parts[3]);
		}
		catch (NumberFormatException e) {
		}
		return new LogisimVersion(major, minor, release, revision);
	}

	private int major;
	private int minor;
	private int release;
	private int revision;
	private String repr;

	private LogisimVersion(int major, int minor, int release, int revision) {
		this.major = major;
		this.minor = minor;
		this.release = release;
		this.revision = revision;
		repr = null;
	}

	@Override
	public int hashCode() {
		int ret = major * 31 + minor;
		ret = ret * 31 + release;
		ret = ret * 31 + revision;
		return ret;
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof LogisimVersion o && major == o.major && minor == o.minor && release == o.release
				&& revision == o.revision;
	}

	public int compareTo(LogisimVersion other) {
		int ret = major - other.major;
		if (ret != 0) return ret;
		else {
			ret = minor - other.minor;
			if (ret != 0) return ret;
			else {
				ret = release - other.release;
				if (ret != 0) return ret;
				else return revision - other.revision;
			}
		}
	}

	@Override
	public String toString() {
		String ret = repr;
		if (ret == null) {
			ret = major + "." + minor + "." + release;
			if (revision != FINAL_REVISION)
				ret += "." + revision;
			repr = ret;
		}
		return ret;
	}
}
