/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import java.awt.Graphics;
import java.util.Collection;
import java.util.List;
import java.util.Map;

import draw.canvas.Selection;
import draw.shapes.Text;
import logisim.data.Bounds;

public interface CanvasModel {
	// listener methods
	void addCanvasModelListener(CanvasModelListener l);

	void removeCanvasModelListener(CanvasModelListener l);

	// methods that don't change any data in the model
	void paint(Graphics g, Selection selection);

	List<CanvasObject> getObjectsFromTop();

	List<CanvasObject> getObjectsFromBottom();

	Collection<CanvasObject> getObjectsIn(Bounds bds);

	Collection<CanvasObject> getObjectsOverlapping(CanvasObject shape);

	// methods that alter the model
	<V extends CanvasObject> void addObjects(int index, Collection<V> shapes);

	<V extends CanvasObject> void addObjects(Map<V, Integer> shapes);

	<V extends CanvasObject> void removeObjects(Collection<V> shapes);

	<V extends CanvasObject> void translateObjects(Collection<V> shapes, int dx, int dy);

	void reorderObjects(List<ReorderRequest> requests);

	Handle moveHandle(HandleGesture gesture);

	void insertHandle(Handle desired, Handle previous);

	Handle deleteHandle(Handle handle);

	void setAttributeValues(Map<AttributeMapKey, Object> values);

	void setText(Text text, String value);
}
