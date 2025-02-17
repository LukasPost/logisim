/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.data;

import java.awt.Component;
import java.awt.Window;

import javax.swing.JTextField;

import logisim.util.StringGetter;

public abstract class Attribute<V> {
	private String name;
	private StringGetter disp;

	public Attribute(String name, StringGetter disp) {
		this.name = name;
		this.disp = disp;
	}

	@Override
	public String toString() {
		return name;
	}

	public String getName() {
		return name;
	}

	public String getDisplayName() {
		return disp.get();
	}

	public Component getCellEditor(Window source, V value) {
		return getCellEditor(value);
	}

	protected Component getCellEditor(V value) {
		return new JTextField(toDisplayString(value));
	}

	public String toDisplayString(V value) {
		return value == null ? "" : value.toString();
	}

	public String toStandardString(V value) {
		return value.toString();
	}

	public abstract V parse(String value);
}
