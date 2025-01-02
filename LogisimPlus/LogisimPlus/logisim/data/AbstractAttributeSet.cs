// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{

	public abstract class AbstractAttributeSet : Cloneable, AttributeSet
	{
		private List<AttributeListener> listeners = null;

		public AbstractAttributeSet()
		{
		}

		public virtual object clone()
		{
			AbstractAttributeSet ret;
			try
			{
				ret = (AbstractAttributeSet) base.clone();
			}
			catch (CloneNotSupportedException)
			{
				throw new System.NotSupportedException();
			}
			ret.listeners = new List<AttributeListener>();
			this.copyInto(ret);
			return ret;
		}

		public virtual void addAttributeListener(AttributeListener l)
		{
			if (listeners == null)
			{
				listeners = new List<AttributeListener>();
			}
			listeners.Add(l);
		}

		public virtual void removeAttributeListener(AttributeListener l)
		{
			listeners.Remove(l);
			if (listeners.Count == 0)
			{
				listeners = null;
			}
		}

// JAVA TO C# CONVERTER TASK: There is no C# equivalent to the Java 'super' constraint:
// ORIGINAL LINE: protected <V> void fireAttributeValueChanged(Attribute<? super V> attr, V value)
		protected internal virtual void fireAttributeValueChanged<V>(Attribute attr, V value)
		{
			if (listeners != null)
			{
				AttributeEvent @event = new AttributeEvent(this, attr, value);
				IList<AttributeListener> ls = new List<AttributeListener>(listeners);
				foreach (AttributeListener l in ls)
				{
					l.attributeValueChanged(@event);
				}
			}
		}

		protected internal virtual void fireAttributeListChanged()
		{
			if (listeners != null)
			{
				AttributeEvent @event = new AttributeEvent(this);
				IList<AttributeListener> ls = new List<AttributeListener>(listeners);
				foreach (AttributeListener l in ls)
				{
					l.attributeListChanged(@event);
				}
			}
		}

		public virtual bool containsAttribute<T1>(Attribute<T1> attr)
		{
			return Attributes.Contains(attr);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Attribute<?> getAttribute(String name)
		public virtual Attribute<object> getAttribute(string name)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (Attribute<?> attr : getAttributes())
			foreach (Attribute<object> attr in Attributes)
			{
				if (attr.Name.Equals(name))
				{
					return attr;
				}
			}
			return null;
		}

		public virtual bool isReadOnly<T1>(Attribute<T1> attr)
		{
			return false;
		}

		public virtual void setReadOnly<T1>(Attribute<T1> attr, bool value)
		{
			throw new System.NotSupportedException();
		}

		public virtual bool isToSave<T1>(Attribute<T1> attr)
		{
			return true;
		}

		protected internal abstract void copyInto(AbstractAttributeSet dest);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public abstract java.util.List<Attribute<?>> getAttributes();
		public abstract IList<Attribute<object>> Attributes {get;}

		public abstract V getValue<V>(Attribute<V> attr);

		public abstract void setValue<V>(Attribute<V> attr, V value);

	}

}
