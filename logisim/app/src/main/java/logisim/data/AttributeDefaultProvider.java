/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

import logisim.LogisimVersion;

public interface AttributeDefaultProvider {
	boolean isAllDefaultValues(AttributeSet attrs);

	Object getDefaultAttributeValue(Attribute<?> attr, LogisimVersion ver);
}
