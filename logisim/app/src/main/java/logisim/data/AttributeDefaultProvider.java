/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

import logisim.LogisimVersion;

public interface AttributeDefaultProvider {
	public boolean isAllDefaultValues(AttributeSet attrs, LogisimVersion ver);
	public Object getDefaultAttributeValue(Attribute<?> attr, LogisimVersion ver);
}
