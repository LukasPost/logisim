/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.actions;

import java.util.Collection;
import java.util.Collections;
import java.util.HashSet;

import draw.model.CanvasModel;
import draw.model.CanvasObject;
import draw.undo.Action;

public class ModelTranslateAction extends ModelAction {
	private HashSet<CanvasObject> moved;
	private int dx;
	private int dy;

	public ModelTranslateAction(CanvasModel model, Collection<CanvasObject> moved, int dx, int dy) {
		super(model);
		this.moved = new HashSet<>(moved);
		this.dx = dx;
		this.dy = dy;
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
		model.translateObjects(moved, dx, dy);
	}

	@Override
	void undoSub(CanvasModel model) {
		model.translateObjects(moved, -dx, -dy);
	}

	@Override
	public boolean shouldAppendTo(Action other) {
		return other instanceof ModelTranslateAction o && moved.equals(o.moved);
	}

	@Override
	public Action append(Action other) {
		if (other instanceof ModelTranslateAction o)
			if (moved.equals(o.moved)) return new ModelTranslateAction(getModel(), moved, dx + o.dx, dy + o.dy);
		return super.append(other);
	}
}
