// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.prefs
{

	internal abstract class AbstractPrefMonitor<E> : PrefMonitor<E>
	{
		public abstract void preferenceChange(java.util.prefs.PreferenceChangeEvent e);
		public abstract void set(E value);
		public abstract E get();
		private string name;

		internal AbstractPrefMonitor(string name)
		{
			this.name = name;
		}

		public virtual string Identifier
		{
			get
			{
				return name;
			}
		}

		public virtual bool isSource(PropertyChangeEvent @event)
		{
			return name.Equals(@event.getPropertyName());
		}

		public virtual void addPropertyChangeListener(PropertyChangeListener listener)
		{
			AppPreferences.addPropertyChangeListener(name, listener);
		}

		public virtual void removePropertyChangeListener(PropertyChangeListener listener)
		{
			AppPreferences.removePropertyChangeListener(name, listener);
		}

		public virtual bool Boolean
		{
			get
			{
				return ((bool?) get()).Value;
			}
			set
			{
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @SuppressWarnings("unchecked") E valObj = (E) System.Convert.ToBoolean(value);
				E valObj = (E) Convert.ToBoolean(value);
				set(valObj);
			}
		}

	}

}
