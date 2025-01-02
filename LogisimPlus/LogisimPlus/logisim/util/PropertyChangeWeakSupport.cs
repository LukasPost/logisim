// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class PropertyChangeWeakSupport
	{
		private const string ALL_PROPERTIES = "ALL PROPERTIES";

		private class ListenerData
		{
			internal string property;
			internal WeakReference<PropertyChangeListener> listener;

			internal ListenerData(string property, PropertyChangeListener listener)
			{
				this.property = property;
				this.listener = new WeakReference<PropertyChangeListener>(listener);
			}
		}

		private object source;
		private ConcurrentLinkedQueue<ListenerData> listeners;

		public PropertyChangeWeakSupport(object source)
		{
			this.source = source;
			this.listeners = new ConcurrentLinkedQueue<ListenerData>();
		}

		public virtual void addPropertyChangeListener(PropertyChangeListener listener)
		{
			addPropertyChangeListener(ALL_PROPERTIES, listener);
		}

		public virtual void addPropertyChangeListener(string property, PropertyChangeListener listener)
		{
			listeners.add(new ListenerData(property, listener));
		}

		public virtual void removePropertyChangeListener(PropertyChangeListener listener)
		{
			removePropertyChangeListener(ALL_PROPERTIES, listener);
		}

		public virtual void removePropertyChangeListener(string property, PropertyChangeListener listener)
		{
			for (IEnumerator<ListenerData> it = listeners.GetEnumerator(); it.MoveNext();)
			{
				ListenerData data = it.Current;
				PropertyChangeListener l = data.listener.get();
				if (l == null)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
				else if (data.property.Equals(property) && l == listener)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
			}
		}

		public virtual void firePropertyChange(string property, object oldValue, object newValue)
		{
			PropertyChangeEvent e = null;
			for (IEnumerator<ListenerData> it = listeners.GetEnumerator(); it.MoveNext();)
			{
				ListenerData data = it.Current;
				PropertyChangeListener l = data.listener.get();
				if (l == null)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
				else if (string.ReferenceEquals(data.property, ALL_PROPERTIES) || data.property.Equals(property))
				{
					if (e == null)
					{
						e = new PropertyChangeEvent(source, property, oldValue, newValue);
					}
					l.propertyChange(e);
				}
			}
		}

		public virtual void firePropertyChange(string property, int oldValue, int newValue)
		{
			PropertyChangeEvent e = null;
			for (IEnumerator<ListenerData> it = listeners.GetEnumerator(); it.MoveNext();)
			{
				ListenerData data = it.Current;
				PropertyChangeListener l = data.listener.get();
				if (l == null)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
				else if (string.ReferenceEquals(data.property, ALL_PROPERTIES) || data.property.Equals(property))
				{
					if (e == null)
					{
						e = new PropertyChangeEvent(source, property, Convert.ToInt32(oldValue), Convert.ToInt32(newValue));
					}
					l.propertyChange(e);
				}
			}
		}

		public virtual void firePropertyChange(string property, bool oldValue, bool newValue)
		{
			PropertyChangeEvent e = null;
			for (IEnumerator<ListenerData> it = listeners.GetEnumerator(); it.MoveNext();)
			{
				ListenerData data = it.Current;
				PropertyChangeListener l = data.listener.get();
				if (l == null)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
				else if (string.ReferenceEquals(data.property, ALL_PROPERTIES) || data.property.Equals(property))
				{
					if (e == null)
					{
						e = new PropertyChangeEvent(source, property, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue));
					}
					l.propertyChange(e);
				}
			}
		}

	}

}
