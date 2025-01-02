// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{

	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;

	public abstract class AttributeSetTableModel : AttrTableModel, AttributeListener
	{
		private class AttrRow : AttrTableModelRow
		{
			private readonly AttributeSetTableModel outerInstance;

			internal Attribute<object> attr;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: AttrRow(logisim.data.Attribute<?> attr)
			internal AttrRow(AttributeSetTableModel outerInstance, Attribute<T1> attr)
			{
				this.outerInstance = outerInstance;
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> objAttr = (logisim.data.Attribute<Object>) attr;
				Attribute<object> objAttr = (Attribute<object>) attr;
				this.attr = objAttr;
			}

			public virtual string Label
			{
				get
				{
					return attr.DisplayName;
				}
			}

			public virtual string Value
			{
				get
				{
					object value = outerInstance.attrs.getValue(attr);
					if (value == null)
					{
						return "";
					}
					else
					{
						try
						{
							return attr.toDisplayString(value);
						}
						catch (Exception)
						{
							return "???";
						}
					}
				}
				set
				{
					Attribute<object> attr = this.attr;
					if (attr == null || value == null)
					{
						return;
					}
    
					try
					{
						if (value is string)
						{
							value = attr.parse((string) value);
						}
						outerInstance.setValueRequested(attr, value);
					}
					catch (System.InvalidCastException e)
					{
						string msg = Strings.get("attributeChangeInvalidError") + ": " + e;
						throw new AttrTableSetException(msg);
					}
					catch (System.FormatException e)
					{
						string msg = Strings.get("attributeChangeInvalidError");
						string emsg = e.Message;
						if (!string.ReferenceEquals(emsg, null) && emsg.Length > 0)
						{
							msg += ": " + emsg;
						}
						msg += ".";
						throw new AttrTableSetException(msg);
					}
				}
			}

			public virtual bool ValueEditable
			{
				get
				{
					return !outerInstance.attrs.isReadOnly(attr);
				}
			}

			public virtual Component getEditor(Window parent)
			{
				object value = outerInstance.attrs.getValue(attr);
				return attr.getCellEditor(parent, value);
			}

		}

		private List<AttrTableModelListener> listeners;
		private AttributeSet attrs;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.HashMap<logisim.data.Attribute<?>, AttrRow> rowMap;
		private Dictionary<Attribute<object>, AttrRow> rowMap;
		private List<AttrRow> rows;

		public AttributeSetTableModel(AttributeSet attrs)
		{
			this.attrs = attrs;
			this.listeners = new List<AttrTableModelListener>();
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: this.rowMap = new java.util.HashMap<logisim.data.Attribute<?>, AttrRow>();
			this.rowMap = new Dictionary<Attribute<object>, AttrRow>();
			this.rows = new List<AttrRow>();
			if (attrs != null)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : attrs.getAttributes())
				foreach (Attribute<object> attr in attrs.Attributes)
				{
					AttrRow row = new AttrRow(this, attr);
					rowMap[attr] = row;
					rows.Add(row);
				}
			}
		}

		public abstract string Title {get;}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
			set
			{
				if (attrs != value)
				{
					if (listeners.Count > 0)
					{
						attrs.removeAttributeListener(this);
					}
					attrs = value;
					if (listeners.Count > 0)
					{
						attrs.addAttributeListener(this);
					}
					attributeListChanged(null);
				}
			}
		}


		public virtual void addAttrTableModelListener(AttrTableModelListener listener)
		{
			if (listeners.Count == 0 && attrs != null)
			{
				attrs.addAttributeListener(this);
			}
			listeners.Add(listener);
		}

		public virtual void removeAttrTableModelListener(AttrTableModelListener listener)
		{
			listeners.Remove(listener);
			if (listeners.Count == 0 && attrs != null)
			{
				attrs.removeAttributeListener(this);
			}
		}

		protected internal virtual void fireTitleChanged()
		{
			AttrTableModelEvent @event = new AttrTableModelEvent(this);
			foreach (AttrTableModelListener l in listeners)
			{
				l.attrTitleChanged(@event);
			}
		}

		protected internal virtual void fireStructureChanged()
		{
			AttrTableModelEvent @event = new AttrTableModelEvent(this);
			foreach (AttrTableModelListener l in listeners)
			{
				l.attrStructureChanged(@event);
			}
		}

		protected internal virtual void fireValueChanged(int index)
		{
			AttrTableModelEvent @event = new AttrTableModelEvent(this, index);
			foreach (AttrTableModelListener l in listeners)
			{
				l.attrValueChanged(@event);
			}
		}

		public virtual int RowCount
		{
			get
			{
				return rows.Count;
			}
		}

		public virtual AttrTableModelRow getRow(int rowIndex)
		{
			return rows[rowIndex];
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected abstract void setValueRequested(logisim.data.Attribute<Object> attr, Object value) throws AttrTableSetException;
		protected internal abstract void setValueRequested(Attribute<object> attr, object value);

		//
		// AttributeListener methods
		//
		public virtual void attributeListChanged(AttributeEvent e)
		{
			// if anything has changed, don't do anything
			int index = 0;
			bool match = true;
			int rowsSize = rows.Count;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : attrs.getAttributes())
			foreach (Attribute<object> attr in attrs.Attributes)
			{
				if (index >= rowsSize || rows[index].attr != attr)
				{
					match = false;
					break;
				}
				index++;
			}
			if (match && index == rows.Count)
			{
				return;
			}

			// compute the new list of rows, possible adding into hash map
			List<AttrRow> newRows = new List<AttrRow>();
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.HashSet<logisim.data.Attribute<?>> missing = new java.util.HashSet<logisim.data.Attribute<?>>(rowMap.keySet());
			Dictionary<Attribute<object>, AttrRow>.KeyCollection missing = new HashSet<Attribute<object>>(rowMap.Keys);
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : attrs.getAttributes())
			foreach (Attribute<object> attr in attrs.Attributes)
			{
				AttrRow row = rowMap[attr];
				if (row == null)
				{
					row = new AttrRow(this, attr);
					rowMap[attr] = row;
				}
				else
				{
					missing.remove(attr);
				}
				newRows.Add(row);
			}
			rows = newRows;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : missing)
			foreach (Attribute<object> attr in missing)
			{
				rowMap.Remove(attr);
			}

			fireStructureChanged();
		}

		public virtual void attributeValueChanged(AttributeEvent e)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = e.getAttribute();
			Attribute<object> attr = e.Attribute;
			AttrTableModelRow row = rowMap[attr];
			if (row != null)
			{
				int index = rows.IndexOf(row);
				if (index >= 0)
				{
					fireValueChanged(index);
				}
			}
		}

	}

}
