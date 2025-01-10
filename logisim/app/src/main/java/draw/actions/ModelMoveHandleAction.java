/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.actions;

import java.util.Collection;
import java.util.Collections;

import draw.model.CanvasModel;
import draw.model.CanvasObject;
import draw.model.Handle;
import draw.model.HandleGesture;

public class ModelMoveHandleAction extends ModelAction {
	private HandleGesture gesture;
	private Handle newHandle;

	public ModelMoveHandleAction(CanvasModel model, HandleGesture gesture) {
		super(model);
		this.gesture = gesture;
	}

	public Handle getNewHandle() {
		return newHandle;
	}

	@Override
	public Collection<CanvasObject> getObjects() {
		return Collections.singleton(gesture.getHandle().getObject());
	}

	@Override
	public String getName() {
		return Strings.get("actionMoveHandle");
	}

	@Override
	void doSub(CanvasModel model) {
		newHandle = model.moveHandle(gesture);
	}

	@Override
	void undoSub(CanvasModel model) {
		Handle oldHandle = gesture.getHandle();
		HandleGesture reverse = new HandleGesture(newHandle, oldHandle.getLocation().sub(newHandle.getLocation()), 0);
		model.moveHandle(reverse);
	}
}
