/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.actions;

import java.util.Collection;
import java.util.HashSet;
import java.util.Map;
import java.util.Objects;

import draw.model.AttributeMapKey;
import draw.model.CanvasModel;
import draw.model.CanvasObject;
import logisim.data.Attribute;

public class ModelChangeAttributeAction extends ModelAction {
	private Map<AttributeMapKey, Object> oldValues;
	private Map<AttributeMapKey, Object> newValues;
	private Attribute<?> attr;

	public ModelChangeAttributeAction(CanvasModel model, Map<AttributeMapKey, Object> oldValues,
			Map<AttributeMapKey, Object> newValues) {
		super(model);
		this.oldValues = oldValues;
		this.newValues = newValues;
	}

	@Override
	public Collection<CanvasObject> getObjects() {
		HashSet<CanvasObject> ret = new HashSet<>();
		for (AttributeMapKey key : newValues.keySet()) ret.add(key.getObject());
		return ret;
	}

	@Override
	public String getName() {
		Attribute<?> a = attr;
		if (a == null) {
			boolean found = false;
			for (AttributeMapKey key : newValues.keySet()) {
				Attribute<?> at = key.getAttribute();
				if (found) {
					if (!Objects.equals(a, at)) {
						a = null;
						break;
					}
				} else {
					found = true;
					a = at;
				}
			}
			attr = a;
		}
		return a == null ? Strings.get("actionChangeAttributes") : Strings.get("actionChangeAttribute", a.getDisplayName());
	}

	@Override
	void doSub(CanvasModel model) {
		model.setAttributeValues(newValues);
	}

	@Override
	void undoSub(CanvasModel model) {
		model.setAttributeValues(oldValues);
	}
}
