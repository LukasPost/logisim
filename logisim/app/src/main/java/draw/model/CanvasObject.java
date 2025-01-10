/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import java.awt.Graphics;
import java.util.List;

import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Location;

public interface CanvasObject {
	CanvasObject clone();

	String getDisplayName();

	AttributeSet getAttributeSet();

	<V> V getValue(Attribute<V> attr);

	Bounds getBounds();

	boolean matches(CanvasObject other);

	int matchesHashCode();

	boolean contains(Location loc, boolean assumeFilled);

	boolean overlaps(CanvasObject other);

	List<Handle> getHandles(HandleGesture gesture);

	boolean canRemove();

	boolean canMoveHandle(Handle handle);

	Handle canInsertHandle(Location desired);

	Handle canDeleteHandle(Location desired);

	void paint(Graphics g, HandleGesture gesture);

	Handle moveHandle(HandleGesture gesture);

	void insertHandle(Handle desired, Handle previous);

	Handle deleteHandle(Handle handle);

	void translate(Location distance);

	<V> void setValue(Attribute<V> attr, V value);
}
