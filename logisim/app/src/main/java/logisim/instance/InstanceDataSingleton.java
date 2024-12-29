/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.instance;

public class InstanceDataSingleton implements InstanceData, Cloneable {
	private Object value;

	public InstanceDataSingleton(Object value) {
		this.value = value;
	}

	@Override
	public InstanceDataSingleton clone() {
		try {
			return (InstanceDataSingleton) super.clone();
		}
		catch (CloneNotSupportedException e) {
			return null;
		}
	}

	public Object getValue() {
		return value;
	}

	public void setValue(Object value) {
		this.value = value;
	}
}
