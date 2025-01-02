// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;
using System.Linq;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{

	public class CanvasModelEvent : EventObject
	{
		public const int ACTION_ADDED = 0;
		public const int ACTION_REMOVED = 1;
		public const int ACTION_TRANSLATED = 2;
		public const int ACTION_REORDERED = 3;
		public const int ACTION_HANDLE_MOVED = 4;
		public const int ACTION_HANDLE_INSERTED = 5;
		public const int ACTION_HANDLE_DELETED = 6;
		public const int ACTION_ATTRIBUTES_CHANGED = 7;
		public const int ACTION_TEXT_CHANGED = 8;

		public static CanvasModelEvent forAdd<T1>(CanvasModel source, ICollection<T1> affected) where T1 : CanvasObject
		{
			return new CanvasModelEvent(source, ACTION_ADDED, affected);
		}

		public static CanvasModelEvent forRemove<T1>(CanvasModel source, ICollection<T1> affected) where T1 : CanvasObject
		{
			return new CanvasModelEvent(source, ACTION_REMOVED, affected);
		}

		public static CanvasModelEvent forTranslate<T1>(CanvasModel source, ICollection<T1> affected, int dx, int dy) where T1 : CanvasObject
		{
			return new CanvasModelEvent(source, ACTION_TRANSLATED, affected, 0, 0);
		}

		public static CanvasModelEvent forReorder(CanvasModel source, ICollection<ReorderRequest> requests)
		{
			return new CanvasModelEvent(true, source, ACTION_REORDERED, requests);
		}

		public static CanvasModelEvent forInsertHandle(CanvasModel source, Handle desired)
		{
			return new CanvasModelEvent(source, ACTION_HANDLE_INSERTED, desired);
		}

		public static CanvasModelEvent forDeleteHandle(CanvasModel source, Handle handle)
		{
			return new CanvasModelEvent(source, ACTION_HANDLE_DELETED, handle);
		}

		public static CanvasModelEvent forMoveHandle(CanvasModel source, HandleGesture gesture)
		{
			return new CanvasModelEvent(source, ACTION_HANDLE_MOVED, gesture);
		}

		public static CanvasModelEvent forChangeAttributes(CanvasModel source, IDictionary<AttributeMapKey, object> oldValues, IDictionary<AttributeMapKey, object> newValues)
		{
			return new CanvasModelEvent(source, ACTION_ATTRIBUTES_CHANGED, oldValues, newValues);
		}

		public static CanvasModelEvent forChangeText(CanvasModel source, CanvasObject obj, string oldText, string newText)
		{
			return new CanvasModelEvent(source, ACTION_TEXT_CHANGED, Collections.singleton(obj), oldText, newText);
		}

		private int action;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.Collection<? extends CanvasObject> affected;
		private ICollection<CanvasObject> affected;
		private int deltaX;
		private int deltaY;
		private IDictionary<AttributeMapKey, object> oldValues;
		private IDictionary<AttributeMapKey, object> newValues;
		private ICollection<ReorderRequest> reorderRequests;
		private Handle handle;
		private HandleGesture gesture;
		private string oldText;
		private string newText;

// JAVA TO C# CONVERTER TASK: Wildcard generics in method parameters are not converted:
// ORIGINAL LINE: private CanvasModelEvent(CanvasModel source, int action, java.util.Collection<? extends CanvasObject> affected)
		private CanvasModelEvent(CanvasModel source, int action, ICollection<CanvasObject> affected) : base(source)
		{

			this.action = action;
			this.affected = affected;
			this.deltaX = 0;
			this.deltaY = 0;
			this.oldValues = null;
			this.newValues = null;
			this.reorderRequests = null;
			this.handle = null;
			this.gesture = null;
			this.oldText = null;
			this.newText = null;
		}

// JAVA TO C# CONVERTER TASK: Wildcard generics in method parameters are not converted:
// ORIGINAL LINE: private CanvasModelEvent(CanvasModel source, int action, java.util.Collection<? extends CanvasObject> affected, int dx, int dy)
		private CanvasModelEvent(CanvasModel source, int action, ICollection<CanvasObject> affected, int dx, int dy) : this(source, action, affected)
		{

			this.deltaX = dx;
			this.deltaY = dy;
		}

		private CanvasModelEvent(CanvasModel source, int action, Handle handle) : this(source, action, Collections.singleton(handle.Object))
		{

			this.handle = handle;
		}

		private CanvasModelEvent(CanvasModel source, int action, HandleGesture gesture) : this(source, action, gesture.Handle)
		{

			this.gesture = gesture;
		}

		private CanvasModelEvent(CanvasModel source, int action, IDictionary<AttributeMapKey, object> oldValues, IDictionary<AttributeMapKey, object> newValues) : this(source, action, Enumerable.Empty<CanvasObject>())
		{

			HashSet<CanvasObject> affected;
			affected = new HashSet<CanvasObject>(newValues.Count);
			foreach (AttributeMapKey key in newValues.Keys)
			{
				affected.Add(key.Object);
			}
			this.affected = affected;

			IDictionary<AttributeMapKey, object> oldValuesCopy;
			oldValuesCopy = new Dictionary<AttributeMapKey, object>(oldValues);
			IDictionary<AttributeMapKey, object> newValuesCopy;
			newValuesCopy = new Dictionary<AttributeMapKey, object>(newValues);

			this.oldValues = Collections.unmodifiableMap(oldValuesCopy);
			this.newValues = Collections.unmodifiableMap(newValuesCopy);
		}

// JAVA TO C# CONVERTER TASK: Wildcard generics in method parameters are not converted:
// ORIGINAL LINE: private CanvasModelEvent(CanvasModel source, int action, java.util.Collection<? extends CanvasObject> affected, String oldText, String newText)
		private CanvasModelEvent(CanvasModel source, int action, ICollection<CanvasObject> affected, string oldText, string newText) : this(source, action, affected)
		{
			this.oldText = oldText;
			this.newText = newText;
		}

		// the boolean parameter is just because the compiler insists upon it to
		// avoid an erasure conflict with the first constructor
		private CanvasModelEvent(bool dummy, CanvasModel source, int action, ICollection<ReorderRequest> requests) : this(source, action, Enumerable.Empty<CanvasObject>())
		{

			List<CanvasObject> affected;
			affected = new List<CanvasObject>(requests.Count);
			foreach (ReorderRequest r in requests)
			{
				affected.Add(r.Object);
			}
			this.affected = affected;

			this.reorderRequests = Collections.unmodifiableCollection(requests);
		}

		public virtual int Action
		{
			get
			{
				return action;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.Collection<? extends CanvasObject> getAffected()
		public virtual ICollection<CanvasObject> Affected
		{
			get
			{
	// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	// ORIGINAL LINE: java.util.Collection<? extends CanvasObject> ret = affected;
				ICollection<CanvasObject> ret = affected;
				if (ret == null)
				{
					IDictionary<AttributeMapKey, object> newVals = newValues;
					if (newVals != null)
					{
						HashSet<CanvasObject> keys = new HashSet<CanvasObject>();
						foreach (AttributeMapKey key in newVals.Keys)
						{
							keys.Add(key.Object);
						}
						ret = Collections.unmodifiableCollection(keys);
						affected = ret;
					}
				}
				return affected;
			}
		}

		public virtual int DeltaX
		{
			get
			{
				return deltaX;
			}
		}

		public virtual int DeltaY
		{
			get
			{
				return deltaY;
			}
		}

		public virtual Handle Handle
		{
			get
			{
				return handle;
			}
		}

		public virtual HandleGesture HandleGesture
		{
			get
			{
				return gesture;
			}
		}

		public virtual IDictionary<AttributeMapKey, object> OldValues
		{
			get
			{
				return oldValues;
			}
		}

		public virtual IDictionary<AttributeMapKey, object> NewValues
		{
			get
			{
				return newValues;
			}
		}

		public virtual ICollection<ReorderRequest> ReorderRequests
		{
			get
			{
				return reorderRequests;
			}
		}

		public virtual string OldText
		{
			get
			{
				return oldText;
			}
		}

		public virtual string NewText
		{
			get
			{
				return newText;
			}
		}
	}

}
