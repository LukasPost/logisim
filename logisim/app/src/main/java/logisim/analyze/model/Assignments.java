/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.model;

import java.util.HashMap;
import java.util.Map;

public class Assignments {
	private Map<String, Boolean> map = new HashMap<>();

	public Assignments() {
	}

	public boolean get(String variable) {
		Boolean value = map.get(variable);
		return value != null && value;
	}

	public void put(String variable, boolean value) {
		map.put(variable, value);
	}
}
