/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import logisim.data.Location;

import java.awt.event.InputEvent;

public class HandleGesture {
	private Handle handle;
	private Location dlocation;
	private int modifiersEx;
	private Handle resultingHandle;

	public HandleGesture(Handle handle, Location dLocation, int modifiersEx) {
		this.handle = handle;
		this.dlocation = dLocation;
		this.modifiersEx = modifiersEx;
	}

	public Handle getHandle() {
		return handle;
	}

	public int getDeltaX() {
		return dlocation.x();
	}

	public int getDeltaY() {
		return dlocation.y();
	}

	public Location getDelta(){ return dlocation; }

	public int getModifiersEx() {
		return modifiersEx;
	}

	public boolean isShiftDown() {
		return (modifiersEx & InputEvent.SHIFT_DOWN_MASK) != 0;
	}

	public boolean isControlDown() {
		return (modifiersEx & InputEvent.CTRL_DOWN_MASK) != 0;
	}

	public boolean isAltDown() {
		return (modifiersEx & InputEvent.ALT_DOWN_MASK) != 0;
	}

	public void setResultingHandle(Handle value) {
		resultingHandle = value;
	}

	public Handle getResultingHandle() {
		return resultingHandle;
	}
}
