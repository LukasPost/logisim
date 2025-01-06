/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.appear;

import java.util.Collection;
import java.util.Collections;
import java.util.List;

import draw.model.CanvasObject;
import logisim.data.Direction;
import logisim.data.Location;

class ClipboardContents {
	static final ClipboardContents EMPTY = new ClipboardContents(Collections.emptySet(), null, null);

	private Collection<CanvasObject> onClipboard;
	private Location anchorLocation;
	private Direction anchorFacing;

	public ClipboardContents(Collection<CanvasObject> onClipboard, Location anchorLocation, Direction anchorFacing) {
		this.onClipboard = List.copyOf(onClipboard);
		this.anchorLocation = anchorLocation;
		this.anchorFacing = anchorFacing;
	}

	public Collection<CanvasObject> getElements() {
		return onClipboard;
	}

	public Location getAnchorLocation() {
		return anchorLocation;
	}

	public Direction getAnchorFacing() {
		return anchorFacing;
	}
}
