/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import logisim.data.Attribute;

import java.util.Objects;

public class AttributeMapKey {
	private Attribute<?> attr;
	private CanvasObject object;

	public AttributeMapKey(Attribute<?> attr, CanvasObject object) {
		this.attr = attr;
		this.object = object;
	}

	public Attribute<?> getAttribute() {
		return attr;
	}

	public CanvasObject getObject() {
		return object;
	}

	@Override
	public int hashCode() {
		int a = attr == null ? 0 : attr.hashCode();
		int b = object == null ? 0 : object.hashCode();
		return a ^ b;
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof AttributeMapKey o && Objects.equals(attr, o.attr) && Objects.equals(object, o.object);
	}
}
