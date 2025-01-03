// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{

	using Selection = draw.canvas.Selection;
	using Text = draw.shapes.Text;
	using logisim.data;
	using Bounds = logisim.data.Bounds;
	using logisim.util;
    using LogisimPlus.Java;

    public class Drawing : CanvasModel
	{
		private EventSourceWeakSupport<CanvasModelListener> listeners;
		private List<CanvasObject> canvasObjects;
		private DrawingOverlaps overlaps;

		public Drawing()
		{
			listeners = new EventSourceWeakSupport<CanvasModelListener>();
			canvasObjects = new List<CanvasObject>();
			overlaps = new DrawingOverlaps();
		}

		public virtual void addCanvasModelListener(CanvasModelListener l)
		{
			listeners.add(l);
		}

		public virtual void removeCanvasModelListener(CanvasModelListener l)
		{
			listeners.remove(l);
		}

		protected internal virtual bool isChangeAllowed(CanvasModelEvent e)
		{
			return true;
		}

		private void fireChanged(CanvasModelEvent e)
		{
			foreach (CanvasModelListener listener in listeners)
			{
				listener.modelChanged(e);
			}
		}

		public virtual void paint(JGraphics g, Selection selection)
		{
			HashSet<CanvasObject> suppressed = selection.DrawsSuppressed;
			foreach (CanvasObject shape in ObjectsFromBottom)
			{
				JGraphics dup = g.create();
				if (suppressed.Contains(shape))
				{
					selection.drawSuppressed(dup, shape);
				}
				else
				{
					shape.paint(dup, null);
				}
				dup.dispose();
			}
		}

		public virtual List<CanvasObject> ObjectsFromTop
		{
			get
			{
				List<CanvasObject> ret = new List<CanvasObject>(ObjectsFromBottom);
				ret.Reverse();
				return ret;
			}
		}

		public virtual List<CanvasObject> ObjectsFromBottom
		{
			get
			{
				return canvasObjects.AsReadOnly();
			}
		}

		public virtual ICollection<CanvasObject> getObjectsIn(Bounds bds)
		{
			List<CanvasObject> ret = null;
			foreach (CanvasObject shape in ObjectsFromBottom)
			{
				if (bds.contains(shape.Bounds))
				{
					if (ret == null)
					{
						ret = new List<CanvasObject>();
					}
					ret.Add(shape);
				}
			}
			if (ret == null)
			{
				return [];
			}
			else
			{
				return ret;
			}
		}

		public virtual ICollection<CanvasObject> getObjectsOverlapping(CanvasObject shape)
		{
			return overlaps.getObjectsOverlapping(shape);
		}

		public virtual void addObjects<T1>(int index, ICollection<T1> shapes) where T1 : CanvasObject
		{
			Dictionary<CanvasObject, int> indexes;
			indexes = new Dictionary<CanvasObject, int>();
			int i = index;
			foreach (CanvasObject shape in shapes)
			{
				indexes.put(shape, Convert.ToInt32(i));
				i++;
			}
			addObjectsHelp(indexes);
		}

		public virtual void addObjects<T1>(Dictionary<T1> shapes) where T1 : CanvasObject
		{
			addObjectsHelp(shapes);
		}

		private void addObjectsHelp<T1>(Dictionary<T1> shapes) where T1 : CanvasObject
		{
			// this is separate method so that subclass can call super.add to either
			// of the add methods, and it won't get redirected into the subclass
			// in calling the other add method
			CanvasModelEvent e = CanvasModelEvent.forAdd(this, shapes.Keys);
			if (shapes.Count > 0 && isChangeAllowed(e))
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (java.util.Map.Entry<? extends CanvasObject, int> entry : shapes.entrySet())
				foreach (KeyValuePair<CanvasObject, int> entry in shapes.SetOfKeyValuePairs())
				{
					CanvasObject shape = entry.Key;
					int index = entry.Value.intValue();
					canvasObjects.Insert(index, shape);
					overlaps.addShape(shape);
				}
				fireChanged(e);
			}
		}

		public virtual void removeObjects<T1>(ICollection<T1> shapes) where T1 : CanvasObject
		{
			List<CanvasObject> found = restrict(shapes);
			CanvasModelEvent e = CanvasModelEvent.forRemove(this, found);
			if (found.Count > 0 && isChangeAllowed(e))
			{
				foreach (CanvasObject shape in found)
				{
					canvasObjects.Remove(shape);
					overlaps.removeShape(shape);
				}
				fireChanged(e);
			}
		}

		public virtual void translateObjects<T1>(ICollection<T1> shapes, int dx, int dy) where T1 : CanvasObject
		{
			List<CanvasObject> found = restrict(shapes);
			CanvasModelEvent e = CanvasModelEvent.forTranslate(this, found, dx, dy);
			if (found.Count > 0 && (dx != 0 || dy != 0) && isChangeAllowed(e))
			{
				foreach (CanvasObject shape in shapes)
				{
					shape.translate(dx, dy);
					overlaps.invalidateShape(shape);
				}
				fireChanged(e);
			}
		}

		public virtual void reorderObjects(List<ReorderRequest> requests)
		{
			bool hasEffect = false;
			foreach (ReorderRequest r in requests)
			{
				if (r.FromIndex != r.ToIndex)
				{
					hasEffect = true;
				}
			}
			CanvasModelEvent e = CanvasModelEvent.forReorder(this, requests);
			if (hasEffect && isChangeAllowed(e))
			{
				foreach (ReorderRequest r in requests)
				{
					if (canvasObjects[r.FromIndex] != r.Object)
					{
						throw new System.ArgumentException("object not present" + " at indicated index: " + r.FromIndex);
					}
					canvasObjects.RemoveAt(r.FromIndex);
					canvasObjects.Insert(r.ToIndex, r.Object);
				}
				fireChanged(e);
			}
		}

		public virtual Handle moveHandle(HandleGesture gesture)
		{
			CanvasModelEvent e = CanvasModelEvent.forMoveHandle(this, gesture);
			CanvasObject o = gesture.Handle.Object;
			if (canvasObjects.Contains(o) && (gesture.DeltaX != 0 || gesture.DeltaY != 0) && isChangeAllowed(e))
			{
				Handle moved = o.moveHandle(gesture);
				gesture.ResultingHandle = moved;
				overlaps.invalidateShape(o);
				fireChanged(e);
				return moved;
			}
			else
			{
				return null;
			}
		}

		public virtual void insertHandle(Handle desired, Handle previous)
		{
			CanvasObject obj = desired.Object;
			CanvasModelEvent e = CanvasModelEvent.forInsertHandle(this, desired);
			if (isChangeAllowed(e))
			{
				obj.insertHandle(desired, previous);
				overlaps.invalidateShape(obj);
				fireChanged(e);
			}
		}

		public virtual Handle deleteHandle(Handle handle)
		{
			CanvasModelEvent e = CanvasModelEvent.forDeleteHandle(this, handle);
			if (isChangeAllowed(e))
			{
				CanvasObject o = handle.Object;
				Handle ret = o.deleteHandle(handle);
				overlaps.invalidateShape(o);
				fireChanged(e);
				return ret;
			}
			else
			{
				return null;
			}
		}

		public virtual Dictionary<AttributeMapKey, object> AttributeValues
		{
			set
			{
				Dictionary<AttributeMapKey, object> oldValues;
				oldValues = new Dictionary<AttributeMapKey, object>();
				foreach (AttributeMapKey key in value.Keys)
				{
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute attr = (logisim.data.Attribute) key.getAttribute();
					Attribute attr = (Attribute) key.Attribute;
					object oldValue = key.Object.getValue(attr);
					oldValues[key] = oldValue;
				}
				CanvasModelEvent e = CanvasModelEvent.forChangeAttributes(this, oldValues, value);
				if (isChangeAllowed(e))
				{
					foreach (KeyValuePair<AttributeMapKey, object> entry in value.SetOfKeyValuePairs())
					{
						AttributeMapKey key = entry.Key;
						CanvasObject shape = key.Object;
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute attr = (logisim.data.Attribute) key.getAttribute();
						Attribute attr = (Attribute) key.Attribute;
						shape.setValue(attr, entry.Value);
						overlaps.invalidateShape(shape);
					}
					fireChanged(e);
				}
			}
		}

		public virtual void setText(Text text, string value)
		{
			string oldValue = text.Text;
			CanvasModelEvent e = CanvasModelEvent.forChangeText(this, text, oldValue, value);
			if (canvasObjects.Contains(text) && !oldValue.Equals(value) && isChangeAllowed(e))
			{
				text.Text = value;
				overlaps.invalidateShape(text);
				fireChanged(e);
			}
		}

		private List<CanvasObject> restrict<T1>(ICollection<T1> shapes) where T1 : CanvasObject
		{
			List<CanvasObject> ret;
			ret = new List<CanvasObject>(shapes.Count);
			foreach (CanvasObject shape in shapes)
			{
				if (canvasObjects.Contains(shape))
				{
					ret.Add(shape);
				}
			}
			return ret;
		}
	}

}
