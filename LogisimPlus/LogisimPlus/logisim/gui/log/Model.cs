// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{

	using CircuitState = logisim.circuit.CircuitState;
	using Value = logisim.data.Value;
	using logisim.util;

	internal class Model
	{
		private EventSourceWeakSupport<ModelListener> listeners;
		private Selection selection;
		private Dictionary<SelectionItem, ValueLog> log;
		private bool fileEnabled = false;
		private File file = null;
		private bool fileHeader = true;
		private bool selected = false;
		private LogThread logger = null;

		public Model(CircuitState circuitState)
		{
			listeners = new EventSourceWeakSupport<ModelListener>();
			selection = new Selection(circuitState, this);
			log = new Dictionary<SelectionItem, ValueLog>();
		}

		public virtual bool Selected
		{
			get
			{
				return selected;
			}
		}

		public virtual void addModelListener(ModelListener l)
		{
			listeners.add(l);
		}

		public virtual void removeModelListener(ModelListener l)
		{
			listeners.remove(l);
		}

		public virtual CircuitState CircuitState
		{
			get
			{
				return selection.CircuitState;
			}
		}

		public virtual Selection Selection
		{
			get
			{
				return selection;
			}
		}

		public virtual ValueLog getValueLog(SelectionItem item)
		{
			ValueLog ret = log[item];
			if (ret == null && selection.indexOf(item) >= 0)
			{
				ret = new ValueLog();
				log[item] = ret;
			}
			return ret;
		}

		public virtual bool FileEnabled
		{
			get
			{
				return fileEnabled;
			}
			set
			{
				if (fileEnabled == value)
				{
					return;
				}
				fileEnabled = value;
				fireFilePropertyChanged(new ModelEvent());
			}
		}

		public virtual File File
		{
			get
			{
				return file;
			}
			set
			{
				if (file == null ? value == null : file.Equals(value))
				{
					return;
				}
				file = value;
				fileEnabled = file != null;
				fireFilePropertyChanged(new ModelEvent());
			}
		}

		public virtual bool FileHeader
		{
			get
			{
				return fileHeader;
			}
			set
			{
				if (fileHeader == value)
				{
					return;
				}
				fileHeader = value;
				fireFilePropertyChanged(new ModelEvent());
			}
		}




		public virtual void propagationCompleted()
		{
			CircuitState circuitState = CircuitState;
			Value[] vals = new Value[selection.size()];
			bool changed = false;
			for (int i = selection.size() - 1; i >= 0; i--)
			{
				SelectionItem item = selection.get(i);
				vals[i] = item.fetchValue(circuitState);
				if (!changed)
				{
					Value v = getValueLog(item).Last;
					changed = v == null ? vals[i] != null :!v.Equals(vals[i]);
				}
			}
			if (changed)
			{
				for (int i = selection.size() - 1; i >= 0; i--)
				{
					SelectionItem item = selection.get(i);
					getValueLog(item).append(vals[i]);
				}
				fireEntryAdded(new ModelEvent(), vals);
			}
		}

		public virtual void setSelected(JFrame frame, bool value)
		{
			if (selected == value)
			{
				return;
			}
			selected = value;
			if (selected)
			{
				logger = new LogThread(this);
				logger.Start();
			}
			else
			{
				if (logger != null)
				{
					logger.cancel();
				}
				logger = null;
				fileEnabled = false;
			}
			fireFilePropertyChanged(new ModelEvent());
		}

		internal virtual void fireSelectionChanged(ModelEvent e)
		{
			for (IEnumerator<SelectionItem> it = log.Keys.GetEnumerator(); it.MoveNext();)
			{
				SelectionItem i = it.Current;
				if (selection.indexOf(i) < 0)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
			}

			foreach (ModelListener l in listeners)
			{
				l.selectionChanged(e);
			}
		}

		private void fireEntryAdded(ModelEvent e, Value[] values)
		{
			foreach (ModelListener l in listeners)
			{
				l.entryAdded(e, values);
			}
		}

		private void fireFilePropertyChanged(ModelEvent e)
		{
			foreach (ModelListener l in listeners)
			{
				l.filePropertyChanged(e);
			}
		}
	}

}
