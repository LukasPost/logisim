/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools.key;

import java.util.HashMap;
import java.util.Map;

import logisim.data.Attribute;

public class KeyConfigurationResult {
	private KeyConfigurationEvent event;
	private Map<Attribute<?>, Object> attrValueMap;

	public KeyConfigurationResult(KeyConfigurationEvent event, Attribute<?> attr, Object value) {
		this.event = event;
		Map<Attribute<?>, Object> singleMap = new HashMap<>(1);
		singleMap.put(attr, value);
		attrValueMap = singleMap;
	}

	public KeyConfigurationResult(KeyConfigurationEvent event, Map<Attribute<?>, Object> values) {
		this.event = event;
		attrValueMap = values;
	}

	public KeyConfigurationEvent getEvent() {
		return event;
	}

	public Map<Attribute<?>, Object> getAttributeValues() {
		return attrValueMap;
	}
}
