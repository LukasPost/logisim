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

	public abstract class AbstractAttributeSet : ICloneable, AttributeSet
	{
		private List<AttributeListener> listeners = null;

		public AbstractAttributeSet()
		{
		}

		public virtual object Clone()
		{
            AbstractAttributeSet ret = (AbstractAttributeSet) base.MemberwiseClone();
			ret.listeners = new List<AttributeListener>();
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
		protected internal virtual void fireAttributeValueChanged(Attribute attr, object value)
		{
			if (listeners != null)
			{
				AttributeEvent @event = new AttributeEvent(this, attr, value);
				List<AttributeListener> ls = new List<AttributeListener>(listeners);
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
				List<AttributeListener> ls = new List<AttributeListener>(listeners);
				foreach (AttributeListener l in ls)
				{
					l.attributeListChanged(@event);
				}
			}
		}

		public virtual bool containsAttribute(Attribute attr)
		{
			return Attributes.Contains(attr);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Attribute<?> getAttribute(String name)
		public virtual Attribute getAttribute(string name)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (Attribute<?> attr : getAttributes())
			foreach (Attribute attr in Attributes)
			{
				if (attr.Name.Equals(name))
				{
					return attr;
				}
			}
			return null;
		}

		public virtual bool isReadOnly(Attribute attr)
		{
			return false;
		}

		public virtual void setReadOnly(Attribute attr, bool value)
		{
			throw new System.NotSupportedException();
		}

		public virtual bool isToSave(Attribute attr)
		{
			return true;
		}

		protected internal abstract void copyInto(AbstractAttributeSet dest);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public abstract java.util.List<Attribute<?>> getAttributes();
		public abstract List<Attribute> Attributes {get;}

		public abstract object getValue(Attribute attr);

		public abstract void setValue(Attribute attr, object value);

	}

}
