// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{

	public class VariableList
	{
		private List<VariableListListener> listeners = new List<VariableListListener>();
		private int maxSize;
		private List<string> data;
		private List<string> dataView;

		public VariableList(int maxSize)
		{
			this.maxSize = maxSize;
			data = maxSize > 16 ? new List<string>() : new List<string>(maxSize);
			dataView = data.AsReadOnly();
		}

		//
		// listener methods
		//
		public virtual void addVariableListListener(VariableListListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeVariableListListener(VariableListListener l)
		{
			listeners.Remove(l);
		}

		private void fireEvent(int type)
		{
			fireEvent(type, null, null);
		}

		private void fireEvent(int type, string variable)
		{
			fireEvent(type, variable, null);
		}

		private void fireEvent(int type, string variable, object data)
		{
			if (listeners.Count == 0)
			{
				return;
			}
			VariableListEvent @event = new VariableListEvent(this, type, variable, data);
			foreach (VariableListListener l in listeners)
			{
				l.listChanged(@event);
			}
		}

		//
		// data methods
		//
		public virtual int MaximumSize
		{
			get
			{
				return maxSize;
			}
		}

		public virtual List<string> All
		{
			get
			{
				return dataView;
			}
			set
			{
				if (value.Count > maxSize)
				{
					throw new System.ArgumentException("maximum size is " + maxSize);
				}
				data.Clear();
				data.AddRange(value);
				fireEvent(VariableListEvent.ALL_REPLACED);
			}
		}

		public virtual int indexOf(string name)
		{
			return data.IndexOf(name);
		}

		public virtual int size()
		{
			return data.Count;
		}

		public virtual bool Empty
		{
			get
			{
				return data.Count == 0;
			}
		}

		public virtual bool Full
		{
			get
			{
				return data.Count >= maxSize;
			}
		}

		public virtual string get(int index)
		{
			return data[index];
		}

		public virtual bool contains(string value)
		{
			return data.Contains(value);
		}

		public virtual string[] toArray(string[] dest)
		{
			return data.toArray(dest);
		}


		public virtual void add(string name)
		{
			if (data.Count >= maxSize)
			{
				throw new System.ArgumentException("maximum size is " + maxSize);
			}
			data.Add(name);
			fireEvent(VariableListEvent.ADD, name);
		}

		public virtual void remove(string name)
		{
			int index = data.IndexOf(name);
			if (index < 0)
			{
				throw new NoSuchElementException("input " + name);
			}
			data.RemoveAt(index);
			fireEvent(VariableListEvent.REMOVE, name, Convert.ToInt32(index));
		}

		public virtual void move(string name, int delta)
		{
			int index = data.IndexOf(name);
			if (index < 0)
			{
				throw new NoSuchElementException(name);
			}
			int newIndex = index + delta;
			if (newIndex < 0)
			{
				throw new System.ArgumentException("cannot move index " + index + " by " + delta);
			}
			if (newIndex > data.Count - 1)
			{
				throw new System.ArgumentException("cannot move index " + index + " by " + delta + ": size " + data.Count);
			}
			if (index == newIndex)
			{
				return;
			}
			data.RemoveAt(index);
			data.Insert(newIndex, name);
			fireEvent(VariableListEvent.MOVE, name, Convert.ToInt32(newIndex - index));
		}

		public virtual void replace(string oldName, string newName)
		{
			int index = data.IndexOf(oldName);
			if (index < 0)
			{
				throw new NoSuchElementException(oldName);
			}
			if (oldName.Equals(newName))
			{
				return;
			}
			data[index] = newName;
			fireEvent(VariableListEvent.REPLACE, oldName, Convert.ToInt32(index));
		}

	}

}
