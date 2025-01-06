/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.proj.Action;
import logisim.proj.Project;
import logisim.tools.Tool;
import logisim.tools.key.KeyConfigurationEvent;
import logisim.tools.key.KeyConfigurationResult;

public class ToolAttributeAction extends Action {
	public static Action create(Tool tool, Attribute<?> attr, Object value) {
		AttributeSet attrs = tool.getAttributeSet();
		KeyConfigurationEvent e = new KeyConfigurationEvent(0, attrs, null, null);
		KeyConfigurationResult r = new KeyConfigurationResult(e, attr, value);
		return new ToolAttributeAction(r);
	}

	public static Action create(KeyConfigurationResult results) {
		return new ToolAttributeAction(results);
	}

	private KeyConfigurationResult config;
	private Map<Attribute<?>, Object> oldValues;

	private ToolAttributeAction(KeyConfigurationResult config) {
		this.config = config;
		oldValues = new HashMap<>(2);
	}

	@Override
	public String getName() {
		return Strings.get("changeToolAttrAction");
	}

	@Override
	public void doIt(Project proj) {
		AttributeSet attrs = config.getEvent().getAttributeSet();
		Map<Attribute<?>, Object> newValues = config.getAttributeValues();
		Map<Attribute<?>, Object> oldValues = new HashMap<>(newValues.size());
		for (Entry<Attribute<?>, Object> entry : newValues.entrySet()) {
			@SuppressWarnings("unchecked")
			Attribute<Object> attr = (Attribute<Object>) entry.getKey();
			oldValues.put(attr, attrs.getValue(attr));
			attrs.setValue(attr, entry.getValue());
		}
		this.oldValues = oldValues;
	}

	@Override
	public void undo(Project proj) {
		AttributeSet attrs = config.getEvent().getAttributeSet();
		Map<Attribute<?>, Object> oldValues = this.oldValues;
		for (Entry<Attribute<?>, Object> entry : oldValues.entrySet()) {
			@SuppressWarnings("unchecked")
			Attribute<Object> attr = (Attribute<Object>) entry.getKey();
			attrs.setValue(attr, entry.getValue());
		}
	}

}
