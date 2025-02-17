/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import java.util.List;
import java.util.Objects;

import logisim.data.AbstractAttributeSet;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.AttributeSets;
import logisim.data.BitWidth;
import logisim.instance.StdAttr;

class CounterAttributes extends AbstractAttributeSet {
	private AttributeSet base;

	public CounterAttributes() {
		base = AttributeSets.fixedSet(
				new Attribute<?>[] { StdAttr.WIDTH, Counter.ATTR_MAX, Counter.ATTR_ON_GOAL, StdAttr.EDGE_TRIGGER,
						StdAttr.LABEL, StdAttr.LABEL_FONT },
				new Object[] { BitWidth.create(8), 0xFF, Counter.ON_GOAL_WRAP, StdAttr.TRIG_RISING, "",
						StdAttr.DEFAULT_LABEL_FONT });
	}

	@Override
	public void copyInto(AbstractAttributeSet dest) {
		((CounterAttributes) dest).base = (AttributeSet) base.clone();
	}

	@Override
	public List<Attribute<?>> getAttributes() {
		return base.getAttributes();
	}

	@Override
	public <V> V getValue(Attribute<V> attr) {
		return base.getValue(attr);
	}

	@Override
	public <V> void setValue(Attribute<V> attr, V value) {
		Object oldValue = base.getValue(attr);
		if (Objects.equals(oldValue, value))
			return;

		Integer newMax = null;
		if (attr == StdAttr.WIDTH) {
			BitWidth oldWidth = base.getValue(StdAttr.WIDTH);
			BitWidth newWidth = (BitWidth) value;
			int oldW = oldWidth.getWidth();
			int newW = newWidth.getWidth();
			int oldVal = base.getValue(Counter.ATTR_MAX);
			base.setValue(StdAttr.WIDTH, newWidth);
			if (newW > oldW) newMax = newWidth.getMask();
			else {
				int v = oldVal & newWidth.getMask();
				if (v != oldVal) {
					Integer newValObj = v;
					base.setValue(Counter.ATTR_MAX, newValObj);
					fireAttributeValueChanged(Counter.ATTR_MAX, newValObj);
				}
			}
			fireAttributeValueChanged(StdAttr.WIDTH, newWidth);
		} else if (attr == Counter.ATTR_MAX) {
			int oldVal = (Integer) value;
			BitWidth width = base.getValue(StdAttr.WIDTH);
			int newVal = oldVal & width.getMask();
			if (newVal != oldVal) {
				@SuppressWarnings("unchecked")
				V val = (V) Integer.valueOf(newVal);
				value = val;
			}
			fireAttributeValueChanged(attr, value);
		}
		base.setValue(attr, value);
		if (newMax != null) {
			base.setValue(Counter.ATTR_MAX, newMax);
			fireAttributeValueChanged(Counter.ATTR_MAX, newMax);
		}
	}

	@Override
	public boolean containsAttribute(Attribute<?> attr) {
		return base.containsAttribute(attr);
	}

	@Override
	public Attribute<?> getAttribute(String name) {
		return base.getAttribute(name);
	}

	@Override
	public boolean isReadOnly(Attribute<?> attr) {
		return base.isReadOnly(attr);
	}

	@Override
	public void setReadOnly(Attribute<?> attr, boolean value) {
		base.setReadOnly(attr, value);
	}
}
