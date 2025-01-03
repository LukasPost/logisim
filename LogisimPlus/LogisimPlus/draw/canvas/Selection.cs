// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.canvas
{

	using CanvasModelEvent = draw.model.CanvasModelEvent;
	using CanvasObject = draw.model.CanvasObject;
	using Handle = draw.model.Handle;
	using HandleGesture = draw.model.HandleGesture;
	using Location = logisim.data.Location;

	public class Selection
	{
		private const string MOVING_HANDLE = "movingHandle";
		private const string TRANSLATING = "translating";
		private const string HIDDEN = "hidden";

		private List<SelectionListener> listeners;
		private HashSet<CanvasObject> selected;
		private HashSet<CanvasObject> selectedView;
		private Dictionary<CanvasObject, string> suppressed;
		private HashSet<CanvasObject> suppressedView;
		private Handle selectedHandle;
		private HandleGesture curHandleGesture;
		private int moveDx;
		private int moveDy;

		protected internal Selection()
		{
			listeners = new List<SelectionListener>();
			selected = new HashSet<CanvasObject>();
			suppressed = new Dictionary<CanvasObject, string>();
			selectedView = Collections.unmodifiableSet(selected);
			suppressedView = Collections.unmodifiableSet(suppressed.Keys);
		}

		public virtual void addSelectionListener(SelectionListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeSelectionListener(SelectionListener l)
		{
			listeners.Remove(l);
		}

		private void fireChanged(int action, ICollection<CanvasObject> affected)
		{
			SelectionEvent e = null;
			foreach (SelectionListener listener in listeners)
			{
				if (e == null)
				{
					e = new SelectionEvent(this, action, affected);
				}
				listener.selectionChanged(e);
			}
		}

		public virtual bool Empty
		{
			get
			{
				return selected.Count == 0;
			}
		}

		public virtual bool isSelected(CanvasObject shape)
		{
			return selected.Contains(shape);
		}

		public virtual HashSet<CanvasObject> Selected
		{
			get
			{
				return selectedView;
			}
		}

		public virtual void clearSelected()
		{
			if (selected.Count > 0)
			{
				List<CanvasObject> oldSelected;
				oldSelected = new List<CanvasObject>(selected);
				selected.Clear();
				suppressed.Clear();
				HandleSelected = null;
				fireChanged(SelectionEvent.ACTION_REMOVED, oldSelected);
			}
		}

		public virtual void setSelected(CanvasObject shape, bool value)
		{
			setSelected(Collections.singleton(shape), value);
		}

		public virtual void setSelected(ICollection<CanvasObject> shapes, bool value)
		{
			if (value)
			{
				List<CanvasObject> added;
				added = new List<CanvasObject>(shapes.Count);
				foreach (CanvasObject shape in shapes)
				{
					if (selected.Add(shape))
					{
						added.Add(shape);
					}
				}
				if (added.Count > 0)
				{
					fireChanged(SelectionEvent.ACTION_ADDED, added);
				}
			}
			else
			{
				List<CanvasObject> removed;
				removed = new List<CanvasObject>(shapes.Count);
				foreach (CanvasObject shape in shapes)
				{
					if (selected.Remove(shape))
					{
						suppressed.Remove(shape);
						Handle h = selectedHandle;
						if (h != null && h.Object == shape)
						{
							HandleSelected = null;
						}
						removed.Add(shape);
					}
				}
				if (removed.Count > 0)
				{
					fireChanged(SelectionEvent.ACTION_REMOVED, removed);
				}
			}
		}

		public virtual void toggleSelected(ICollection<CanvasObject> shapes)
		{
			List<CanvasObject> added;
			added = new List<CanvasObject>(shapes.Count);
			List<CanvasObject> removed;
			removed = new List<CanvasObject>(shapes.Count);
			foreach (CanvasObject shape in shapes)
			{
				if (selected.Contains(shape))
				{
					selected.Remove(shape);
					suppressed.Remove(shape);
					Handle h = selectedHandle;
					if (h != null && h.Object == shape)
					{
						HandleSelected = null;
					}
					removed.Add(shape);
				}
				else
				{
					selected.Add(shape);
					added.Add(shape);
				}
			}
			if (removed.Count > 0)
			{
				fireChanged(SelectionEvent.ACTION_REMOVED, removed);
			}
			if (added.Count > 0)
			{
				fireChanged(SelectionEvent.ACTION_ADDED, added);
			}
		}

		public virtual HashSet<CanvasObject> DrawsSuppressed
		{
			get
			{
				return suppressedView;
			}
		}

		public virtual void clearDrawsSuppressed()
		{
			suppressed.Clear();
			curHandleGesture = null;
		}

		public virtual Handle SelectedHandle
		{
			get
			{
				return selectedHandle;
			}
		}

		public virtual Handle HandleSelected
		{
			set
			{
				Handle cur = selectedHandle;
				bool same = cur == null ? value == null : cur.Equals(value);
				if (!same)
				{
					selectedHandle = value;
					curHandleGesture = null;
					ICollection<CanvasObject> objs;
					if (value == null)
					{
						objs = Collections.emptySet();
					}
					else
					{
						objs = Collections.singleton(value.Object);
					}
					fireChanged(SelectionEvent.ACTION_HANDLE, objs);
				}
			}
		}

		public virtual HandleGesture HandleGesture
		{
			set
			{
				HandleGesture g = curHandleGesture;
				if (g != null)
				{
					suppressed.Remove(g.Handle.Object);
				}
    
				Handle h = value.Handle;
				suppressed[h.Object] = MOVING_HANDLE;
				curHandleGesture = value;
			}
		}

		public virtual void setMovingShapes<T1>(ICollection<T1> shapes, int dx, int dy) where T1 : draw.model.CanvasObject
		{
			foreach (CanvasObject o in shapes)
			{
				suppressed[o] = TRANSLATING;
			}
			moveDx = dx;
			moveDy = dy;
		}

		public virtual void setHidden<T1>(ICollection<T1> shapes, bool value) where T1 : draw.model.CanvasObject
		{
			if (value)
			{
				foreach (CanvasObject o in shapes)
				{
					suppressed[o] = HIDDEN;
				}
			}
			else
			{
				suppressed.Keys.RemoveAll(shapes);
			}
		}

		public virtual Location MovingDelta
		{
			get
			{
				return new Location(moveDx, moveDy);
			}
		}

		public virtual void setMovingDelta(int dx, int dy)
		{
			moveDx = dx;
			moveDy = dy;
		}

		public virtual void drawSuppressed(JGraphics g, CanvasObject shape)
		{
			string state = suppressed[shape];
			if (string.ReferenceEquals(state, MOVING_HANDLE))
			{
				shape.paint(g, curHandleGesture);
			}
			else if (string.ReferenceEquals(state, TRANSLATING))
			{
				g.translate(moveDx, moveDy);
				shape.paint(g, null);
			}
		}

		internal virtual void modelChanged(CanvasModelEvent @event)
		{
			int action = @event.Action;
			switch (action)
			{
			case CanvasModelEvent.ACTION_REMOVED:
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Collection<? extends draw.model.CanvasObject> affected = event.getAffected();
				ICollection<CanvasObject> affected = @event.Affected;
				if (affected != null)
				{
					selected.RemoveAll(affected);
					suppressed.Keys.RemoveAll(affected);
					Handle h = selectedHandle;
					if (h != null && affected.Contains(h.Object))
					{
						HandleSelected = null;
					}
				}
				break;
			case CanvasModelEvent.ACTION_HANDLE_DELETED:
				if (@event.Handle.Equals(selectedHandle))
				{
					HandleSelected = null;
				}
				break;
			case CanvasModelEvent.ACTION_HANDLE_MOVED:
				HandleGesture gesture = @event.HandleGesture;
				if (gesture.Handle.Equals(selectedHandle))
				{
					HandleSelected = gesture.ResultingHandle;
				}
			break;
			}
		}
	}

}
