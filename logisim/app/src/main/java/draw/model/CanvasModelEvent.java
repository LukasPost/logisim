/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.model;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.EventObject;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;

public class CanvasModelEvent extends EventObject {
	public static final int ACTION_ADDED = 0;
	public static final int ACTION_REMOVED = 1;
	public static final int ACTION_TRANSLATED = 2;
	public static final int ACTION_REORDERED = 3;
	public static final int ACTION_HANDLE_MOVED = 4;
	public static final int ACTION_HANDLE_INSERTED = 5;
	public static final int ACTION_HANDLE_DELETED = 6;
	public static final int ACTION_ATTRIBUTES_CHANGED = 7;
	public static final int ACTION_TEXT_CHANGED = 8;

	public static CanvasModelEvent forAdd(CanvasModel source, Collection<? extends CanvasObject> affected) {
		return new CanvasModelEvent(source, ACTION_ADDED, affected);
	}

	public static CanvasModelEvent forRemove(CanvasModel source, Collection<? extends CanvasObject> affected) {
		return new CanvasModelEvent(source, ACTION_REMOVED, affected);
	}

	public static CanvasModelEvent forTranslate(CanvasModel source, Collection<? extends CanvasObject> affected, int dx,
			int dy) {
		return new CanvasModelEvent(source, ACTION_TRANSLATED, affected, 0, 0);
	}

	public static CanvasModelEvent forReorder(CanvasModel source, Collection<ReorderRequest> requests) {
		return new CanvasModelEvent(true, source, ACTION_REORDERED, requests);
	}

	public static CanvasModelEvent forInsertHandle(CanvasModel source, Handle desired) {
		return new CanvasModelEvent(source, ACTION_HANDLE_INSERTED, desired);
	}

	public static CanvasModelEvent forDeleteHandle(CanvasModel source, Handle handle) {
		return new CanvasModelEvent(source, ACTION_HANDLE_DELETED, handle);
	}

	public static CanvasModelEvent forMoveHandle(CanvasModel source, HandleGesture gesture) {
		return new CanvasModelEvent(source, ACTION_HANDLE_MOVED, gesture);
	}

	public static CanvasModelEvent forChangeAttributes(CanvasModel source, Map<AttributeMapKey, Object> oldValues,
			Map<AttributeMapKey, Object> newValues) {
		return new CanvasModelEvent(source, ACTION_ATTRIBUTES_CHANGED, oldValues, newValues);
	}

	public static CanvasModelEvent forChangeText(CanvasModel source, CanvasObject obj, String oldText, String newText) {
		return new CanvasModelEvent(source, ACTION_TEXT_CHANGED, Collections.singleton(obj), oldText, newText);
	}

	private int action;
	private Collection<? extends CanvasObject> affected;
	private int deltaX;
	private int deltaY;
	private Map<AttributeMapKey, Object> oldValues;
	private Map<AttributeMapKey, Object> newValues;
	private Collection<ReorderRequest> reorderRequests;
	private Handle handle;
	private HandleGesture gesture;
	private String oldText;
	private String newText;

	private CanvasModelEvent(CanvasModel source, int action, Collection<? extends CanvasObject> affected) {
		super(source);

		this.action = action;
		this.affected = affected;
		deltaX = 0;
		deltaY = 0;
		oldValues = null;
		newValues = null;
		reorderRequests = null;
		handle = null;
		gesture = null;
		oldText = null;
		newText = null;
	}

	private CanvasModelEvent(CanvasModel source, int action, Collection<? extends CanvasObject> affected, int dx,
			int dy) {
		this(source, action, affected);

		deltaX = dx;
		deltaY = dy;
	}

	private CanvasModelEvent(CanvasModel source, int action, Handle handle) {
		this(source, action, Collections.singleton(handle.getObject()));

		this.handle = handle;
	}

	private CanvasModelEvent(CanvasModel source, int action, HandleGesture gesture) {
		this(source, action, gesture.getHandle());

		this.gesture = gesture;
	}

	private CanvasModelEvent(CanvasModel source, int action, Map<AttributeMapKey, Object> oldValues, Map<AttributeMapKey, Object> newValues) {
		this(source, action, Collections.emptySet());
		HashSet<CanvasObject> affected = new HashSet<>(newValues.size());
		for (AttributeMapKey key : newValues.keySet())
			affected.add(key.getObject());
		this.affected = affected;

		Map<AttributeMapKey, Object> oldValuesCopy = new HashMap<>(oldValues);
		Map<AttributeMapKey, Object> newValuesCopy = new HashMap<>(newValues);

		this.oldValues = Collections.unmodifiableMap(oldValuesCopy);
		this.newValues = Collections.unmodifiableMap(newValuesCopy);
	}

	private CanvasModelEvent(CanvasModel source, int action, Collection<? extends CanvasObject> affected,
			String oldText, String newText) {
		this(source, action, affected);
		this.oldText = oldText;
		this.newText = newText;
	}

	// the boolean parameter is just because the compiler insists upon it to
	// avoid an erasure conflict with the first constructor
	private CanvasModelEvent(boolean dummy, CanvasModel source, int action, Collection<ReorderRequest> requests) {
		this(source, action, Collections.emptySet());

		ArrayList<CanvasObject> affected = new ArrayList<>(requests.size());
		for (ReorderRequest r : requests) affected.add(r.getObject());
		this.affected = affected;

		reorderRequests = Collections.unmodifiableCollection(requests);
	}

	public int getAction() {
		return action;
	}

	public Collection<? extends CanvasObject> getAffected() {
		if (affected != null)
			return affected;
		if (newValues == null)
			return null;
		HashSet<CanvasObject> keys = new HashSet<>();
		for (AttributeMapKey key : newValues.keySet())
			keys.add(key.getObject());
		affected = Collections.unmodifiableCollection(keys);
		return affected;
	}

	public int getDeltaX() {
		return deltaX;
	}

	public int getDeltaY() {
		return deltaY;
	}

	public Handle getHandle() {
		return handle;
	}

	public HandleGesture getHandleGesture() {
		return gesture;
	}

	public Map<AttributeMapKey, Object> getOldValues() {
		return oldValues;
	}

	public Map<AttributeMapKey, Object> getNewValues() {
		return newValues;
	}

	public Collection<ReorderRequest> getReorderRequests() {
		return reorderRequests;
	}

	public String getOldText() {
		return oldText;
	}

	public String getNewText() {
		return newText;
	}
}
