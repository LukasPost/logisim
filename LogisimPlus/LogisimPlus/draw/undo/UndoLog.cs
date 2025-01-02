// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.undo
{

	using logisim.util;

	public class UndoLog
	{
		private const int MAX_UNDO_SIZE = 64;

		private EventSourceWeakSupport<UndoLogListener> listeners;
		private LinkedList<Action> undoLog;
		private LinkedList<Action> redoLog;
		private int modCount;

		public UndoLog()
		{
			this.listeners = new EventSourceWeakSupport<UndoLogListener>();
			this.undoLog = new LinkedList<Action>();
			this.redoLog = new LinkedList<Action>();
			this.modCount = 0;
		}

		//
		// listening methods
		//
		public virtual void addProjectListener(UndoLogListener what)
		{
			listeners.add(what);
		}

		public virtual void removeProjectListener(UndoLogListener what)
		{
			listeners.remove(what);
		}

		private void fireEvent(int action, Action actionObject)
		{
			UndoLogEvent e = null;
			foreach (UndoLogListener listener in listeners)
			{
				if (e == null)
				{
					e = new UndoLogEvent(this, action, actionObject);
				}
				listener.undoLogChanged(e);
			}
		}

		//
		// accessor methods
		//
		public virtual Action UndoAction
		{
			get
			{
				if (undoLog.Count == 0)
				{
					return null;
				}
				else
				{
					return undoLog.Last.Value;
				}
			}
		}

		public virtual Action RedoAction
		{
			get
			{
				if (redoLog.Count == 0)
				{
					return null;
				}
				else
				{
					return redoLog.Last.Value;
				}
			}
		}

		public virtual bool Modified
		{
			get
			{
				return modCount != 0;
			}
		}

		//
		// mutator methods
		//
		public virtual void doAction(Action act)
		{
			if (act == null)
			{
				return;
			}
			act.doIt();
			logAction(act);
		}

		public virtual void logAction(Action act)
		{
			redoLog.Clear();
			if (undoLog.Count > 0)
			{
				Action prev = undoLog.Last.Value;
				if (act.shouldAppendTo(prev))
				{
					if (prev.Modification)
					{
						--modCount;
					}
					Action joined = prev.append(act);
					if (joined == null)
					{
						fireEvent(UndoLogEvent.ACTION_DONE, act);
						return;
					}
					act = joined;
				}
				while (undoLog.Count > MAX_UNDO_SIZE)
				{
					undoLog.RemoveFirst();
				}
			}
			undoLog.AddLast(act);
			if (act.Modification)
			{
				++modCount;
			}
			fireEvent(UndoLogEvent.ACTION_DONE, act);
		}

		public virtual void undoAction()
		{
			if (undoLog.Count > 0)
			{
				Action action = undoLog.RemoveLast();
				if (action.Modification)
				{
					--modCount;
				}
				action.undo();
				redoLog.AddLast(action);
				fireEvent(UndoLogEvent.ACTION_UNDONE, action);
			}
		}

		public virtual void redoAction()
		{
			if (redoLog.Count > 0)
			{
				Action action = redoLog.RemoveLast();
				if (action.Modification)
				{
					++modCount;
				}
				action.doIt();
				undoLog.AddLast(action);
				fireEvent(UndoLogEvent.ACTION_DONE, action);
			}
		}

		public virtual void clearModified()
		{
			modCount = 0;
		}
	}

}
