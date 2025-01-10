/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.actions;

import java.util.Collection;
import java.util.Collections;
import java.util.HashSet;

import draw.model.CanvasModel;
import draw.model.CanvasObject;
import draw.undo.Action;
import logisim.data.Location;

public class ModelTranslateAction extends ModelAction {
	private HashSet<CanvasObject> moved;
	private Location dLocation;

	public ModelTranslateAction(CanvasModel model, Collection<CanvasObject> moved, Location dLocation) {
		super(model);
		this.moved = new HashSet<>(moved);
		this.dLocation = dLocation;
	}

	@Override
	public Collection<CanvasObject> getObjects() {
		return Collections.unmodifiableSet(moved);
	}

	@Override
	public String getName() {
		return Strings.get("actionTranslate", getShapesName(moved));
	}

	@Override
	void doSub(CanvasModel model) {
		model.translateObjects(moved, dLocation.x(), dLocation.y());
	}

	@Override
	void undoSub(CanvasModel model) {
		model.translateObjects(moved, -dLocation.x(), -dLocation.y());
	}

	@Override
	public boolean shouldAppendTo(Action other) {
		return other instanceof ModelTranslateAction o && moved.equals(o.moved);
	}

	@Override
	public Action append(Action other) {
		return other instanceof ModelTranslateAction o && moved.equals(o.moved)
				? new ModelTranslateAction(getModel(), moved, dLocation.add(o.dLocation))
				: super.append(other);
	}
}
