/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import java.awt.Graphics;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

import draw.canvas.Selection;
import draw.shapes.Text;
import logisim.data.Attribute;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.util.EventSourceWeakSupport;

public class Drawing implements CanvasModel {
	private EventSourceWeakSupport<CanvasModelListener> listeners;
	private ArrayList<CanvasObject> canvasObjects;
	private DrawingOverlaps overlaps;

	public Drawing() {
		listeners = new EventSourceWeakSupport<>();
		canvasObjects = new ArrayList<>();
		overlaps = new DrawingOverlaps();
	}

	public void addCanvasModelListener(CanvasModelListener l) {
		listeners.add(l);
	}

	public void removeCanvasModelListener(CanvasModelListener l) {
		listeners.remove(l);
	}

	protected boolean isChangeAllowed(CanvasModelEvent e) {
		return true;
	}

	private void fireChanged(CanvasModelEvent e) {
		for (CanvasModelListener listener : listeners) listener.modelChanged(e);
	}

	public void paint(Graphics g, Selection selection) {
		Set<CanvasObject> suppressed = selection.getDrawsSuppressed();
		for (CanvasObject shape : getObjectsFromBottom()) {
			Graphics dup = g.create();
			if (suppressed.contains(shape))
				selection.drawSuppressed(dup, shape);
			else
				shape.paint(dup, null);
			dup.dispose();
		}
	}

	public List<CanvasObject> getObjectsFromTop() {
		return getObjectsFromBottom().reversed();
	}

	public List<CanvasObject> getObjectsFromBottom() {
		return Collections.unmodifiableList(canvasObjects);
	}

	public List<CanvasObject> getObjectsIn(Bounds bds) {
		return getObjectsFromBottom().stream().filter(shape->bds.contains(shape.getBounds())).toList();
	}

	public Collection<CanvasObject> getObjectsOverlapping(CanvasObject shape) {
		return overlaps.getObjectsOverlapping(shape);
	}

	public <V extends CanvasObject> void addObjects(int index, Collection<V> shapes) {
		LinkedHashMap<CanvasObject, Integer> indexes = new LinkedHashMap<>();
		int i = index;
		for (CanvasObject shape : shapes) {
			indexes.put(shape, i);
			i++;
		}
		addObjectsHelp(indexes);
	}

	public <V extends CanvasObject> void addObjects(Map<V, Integer> shapes) {
		addObjectsHelp(shapes);
	}

	private <V extends CanvasObject> void addObjectsHelp(Map<V, Integer> shapes) {
		// this is separate method so that subclass can call super.add to either
		// of the add methods, and it won't get redirected into the subclass
		// in calling the other add method
		CanvasModelEvent e = CanvasModelEvent.forAdd(this, shapes.keySet());
		if (shapes.isEmpty() || !isChangeAllowed(e))
			return;
		for (Entry<? extends CanvasObject, Integer> entry : shapes.entrySet()) {
			canvasObjects.add(entry.getValue(), entry.getKey());
			overlaps.addShape(entry.getKey());
		}
		fireChanged(e);
	}

	public <V extends CanvasObject> void removeObjects(Collection<V> shapes) {
		List<V> found = restrict(shapes);
		CanvasModelEvent e = CanvasModelEvent.forRemove(this, found);
		if (found.isEmpty() || !isChangeAllowed(e))
			return;
		for (CanvasObject shape : found) {
			canvasObjects.remove(shape);
			overlaps.removeShape(shape);
		}
		fireChanged(e);
	}

	public <V extends CanvasObject> void translateObjects(Collection<V> shapes, int dx, int dy) {
		List<V> found = restrict(shapes);
		CanvasModelEvent e = CanvasModelEvent.forTranslate(this, found, dx, dy);
		if (found.isEmpty() || (dx == 0 && dy == 0) || !isChangeAllowed(e))
			return;
		for (CanvasObject shape : shapes) {
			shape.translate(new Location(dx, dy));
			overlaps.invalidateShape(shape);
		}
		fireChanged(e);
	}

	public void reorderObjects(List<ReorderRequest> requests) {
		boolean hasEffect = requests.stream().anyMatch(r->r.getFromIndex() != r.getToIndex());
		if(!hasEffect)
			return;
		CanvasModelEvent e = CanvasModelEvent.forReorder(this, requests);
		if (!isChangeAllowed(e))
			return;
		for (ReorderRequest r : requests) {
			if (canvasObjects.get(r.getFromIndex()) != r.getObject())
				throw new IllegalArgumentException("object not present" + " at indicated index: " + r.getFromIndex());
			canvasObjects.remove(r.getFromIndex());
			canvasObjects.add(r.getToIndex(), r.getObject());
		}
		fireChanged(e);
	}

	public Handle moveHandle(HandleGesture gesture) {
		CanvasModelEvent e = CanvasModelEvent.forMoveHandle(this, gesture);
		CanvasObject o = gesture.getHandle().getObject();
		if (!canvasObjects.contains(o) || (gesture.getDeltaX() == 0 && gesture.getDeltaY() == 0) || !isChangeAllowed(e))
			return null;

		Handle moved = o.moveHandle(gesture);
		gesture.setResultingHandle(moved);
		overlaps.invalidateShape(o);
		fireChanged(e);
		return moved;
	}

	public void insertHandle(Handle desired, Handle previous) {
		CanvasObject obj = desired.getObject();
		CanvasModelEvent e = CanvasModelEvent.forInsertHandle(this, desired);
		if (isChangeAllowed(e)) {
			obj.insertHandle(desired, previous);
			overlaps.invalidateShape(obj);
			fireChanged(e);
		}
	}

	public Handle deleteHandle(Handle handle) {
		CanvasModelEvent e = CanvasModelEvent.forDeleteHandle(this, handle);
		if (!isChangeAllowed(e))
			return null;

		CanvasObject o = handle.getObject();
		Handle ret = o.deleteHandle(handle);
		overlaps.invalidateShape(o);
		fireChanged(e);
		return ret;
	}

	public void setAttributeValues(Map<AttributeMapKey, Object> values) {
		HashMap<AttributeMapKey, Object> oldValues = new HashMap<>();
		for (AttributeMapKey key : values.keySet()) {
			@SuppressWarnings("unchecked")
			Attribute<Object> attr = (Attribute<Object>) key.getAttribute();
			Object oldValue = key.getObject().getValue(attr);
			oldValues.put(key, oldValue);
		}
		CanvasModelEvent e = CanvasModelEvent.forChangeAttributes(this, oldValues, values);
		if (!isChangeAllowed(e))
			return;
		for (Entry<AttributeMapKey, Object> entry : values.entrySet()) {
			@SuppressWarnings("unchecked")
			Attribute<Object> attr = (Attribute<Object>) entry.getKey().getAttribute();
			CanvasObject shape = entry.getKey().getObject();
			shape.setValue(attr, entry.getValue());
			overlaps.invalidateShape(shape);
		}
		fireChanged(e);
	}

	public void setText(Text text, String value) {
		String oldValue = text.getText();
		CanvasModelEvent e = CanvasModelEvent.forChangeText(this, text, oldValue, value);
		if (canvasObjects.contains(text) && !oldValue.equals(value) && isChangeAllowed(e)) {
			text.setText(value);
			overlaps.invalidateShape(text);
			fireChanged(e);
		}
	}

	private <V extends CanvasObject> List<V> restrict(Collection<V> shapes) {
		return shapes.stream().filter(shape->!canvasObjects.contains(shape)).toList();
	}
}
