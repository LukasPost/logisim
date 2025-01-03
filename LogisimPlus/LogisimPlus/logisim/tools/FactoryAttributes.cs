// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using ComponentFactory = logisim.comp.ComponentFactory;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using AttributeSets = logisim.data.AttributeSets;

	internal class FactoryAttributes : AttributeSet, AttributeListener, ICloneable
	{
		private Type descBase;
		private FactoryDescription desc;
		private ComponentFactory factory;
		private AttributeSet baseAttrs;
		private List<AttributeListener> listeners;

		public FactoryAttributes(Type descBase, FactoryDescription desc)
		{
			this.descBase = descBase;
			this.desc = desc;
			this.factory = null;
			this.baseAttrs = null;
			this.listeners = new List<AttributeListener>();
		}

		public FactoryAttributes(ComponentFactory factory)
		{
			this.descBase = null;
			this.desc = null;
			this.factory = factory;
			this.baseAttrs = null;
			this.listeners = new List<AttributeListener>();
		}

		internal virtual bool FactoryInstantiated
		{
			get
			{
				return baseAttrs != null;
			}
		}

		internal virtual AttributeSet Base
		{
			get
			{
				AttributeSet ret = baseAttrs;
				if (ret == null)
				{
					ComponentFactory fact = factory;
					if (fact == null)
					{
						fact = desc.getFactory(descBase);
						factory = fact;
					}
					if (fact == null)
					{
						ret = AttributeSets.EMPTY;
					}
					else
					{
						ret = fact.createAttributeSet();
						ret.addAttributeListener(this);
					}
					baseAttrs = ret;
				}
				return ret;
			}
		}

		public virtual void addAttributeListener(AttributeListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeAttributeListener(AttributeListener l)
		{
			listeners.Remove(l);
		}

		public virtual object Clone()
		{
			return (AttributeSet) Base.clone();
		}

		public virtual bool containsAttribute(Attribute attr)
		{
			return Base.containsAttribute(attr);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public logisim.data.Attribute<?> getAttribute(String name)
		public virtual Attribute getAttribute(string name)
		{
			return Base.getAttribute(name);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public virtual List<Attribute> Attributes
		{
			get
			{
				return Base.Attributes;
			}
		}

		public virtual object getValue(Attribute attr)
		{
			return Base.getValue(attr);
		}

		public virtual bool isReadOnly(Attribute attr)
		{
			return Base.isReadOnly(attr);
		}

		public virtual bool isToSave(Attribute attr)
		{
			return Base.isToSave(attr);
		}

		public virtual void setReadOnly(Attribute attr, bool value)
		{
			Base.setReadOnly(attr, value);
		}

		public virtual void setValue(Attribute attr, object value)
		{
			Base.setValue(attr, value);
		}

		public virtual void attributeListChanged(AttributeEvent baseEvent)
		{
			AttributeEvent e = null;
			foreach (AttributeListener l in listeners)
			{
				if (e == null)
				{
					e = new AttributeEvent(this, baseEvent.Attribute, baseEvent.Value);
				}
				l.attributeListChanged(e);
			}
		}

		public virtual void attributeValueChanged(AttributeEvent baseEvent)
		{
			AttributeEvent e = null;
			foreach (AttributeListener l in listeners)
			{
				if (e == null)
				{
					e = new AttributeEvent(this, baseEvent.Attribute, baseEvent.Value);
				}
				l.attributeValueChanged(e);
			}
		}
	}

}
