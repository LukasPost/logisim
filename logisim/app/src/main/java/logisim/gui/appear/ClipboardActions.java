/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.appear;

import java.util.ArrayList;
import java.util.Map;

import draw.model.CanvasModel;
import draw.model.CanvasObject;
import draw.util.ZOrder;
import logisim.circuit.appear.AppearanceAnchor;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.proj.Action;
import logisim.proj.Project;

public class ClipboardActions extends Action {

	public static Action cut(AppearanceCanvas canvas) {
		return new ClipboardActions(true, canvas);
	}

	public static Action copy(AppearanceCanvas canvas) {
		return new ClipboardActions(false, canvas);
	}

	private boolean remove;
	private AppearanceCanvas canvas;
	private CanvasModel canvasModel;
	private ClipboardContents oldClipboard;
	private Map<CanvasObject, Integer> affected;
	private ClipboardContents newClipboard;

	private ClipboardActions(boolean remove, AppearanceCanvas canvas) {
		this.remove = remove;
		this.canvas = canvas;
		canvasModel = canvas.getModel();

		ArrayList<CanvasObject> contents = new ArrayList<>();
		Direction anchorFacing = null;
		Location anchorLocation = null;
		ArrayList<CanvasObject> aff = new ArrayList<>();
		for (CanvasObject o : canvas.getSelection().getSelected())
			if (o.canRemove()) {
				aff.add(o);
				contents.add(o.clone());
			}
			else if (o instanceof AppearanceAnchor anch) {
				anchorFacing = anch.getFacing();
				anchorLocation = anch.getLocation();
			}
		contents.trimToSize();
		affected = ZOrder.getZIndex(aff, canvasModel);
		newClipboard = new ClipboardContents(contents, anchorLocation, anchorFacing);
	}

	@Override
	public String getName() {
		if (remove) return Strings.get("cutSelectionAction");
		else return Strings.get("copySelectionAction");
	}

	@Override
	public void doIt(Project proj) {
		oldClipboard = Clipboard.get();
		Clipboard.set(newClipboard);
		if (remove) canvasModel.removeObjects(affected.keySet());
	}

	@Override
	public void undo(Project proj) {
		if (remove) {
			canvasModel.addObjects(affected);
			canvas.getSelection().clearSelected();
			canvas.getSelection().setSelected(affected.keySet(), true);
		}
		Clipboard.set(oldClipboard);
	}

}
