// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.tools
{
	using CanvasObject = draw.model.CanvasObject;
	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using DrawAttr = draw.shapes.DrawAttr;
	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using logisim.util;
	using logisim.util;


	public class DrawingAttributeSet : AttributeSet, ICloneable
    {
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: static final java.util.List<logisim.data.Attribute<?>> ATTRS_ALL = logisim.util.UnmodifiableList.create(new logisim.data.Attribute<?>[] { draw.shapes.DrawAttr.FONT, draw.shapes.DrawAttr.ALIGNMENT, draw.shapes.DrawAttr.PAINT_TYPE, draw.shapes.DrawAttr.STROKE_WIDTH, draw.shapes.DrawAttr.STROKE_COLOR, draw.shapes.DrawAttr.FILL_COLOR, draw.shapes.DrawAttr.TEXT_DEFAULT_FILL, draw.shapes.DrawAttr.CORNER_RADIUS });
		internal static readonly List<Attribute> ATTRS_ALL = UnmodifiableList.create(new Attribute[] {DrawAttr.FONT, DrawAttr.ALIGNMENT, DrawAttr.PAINT_TYPE, DrawAttr.STROKE_WIDTH, DrawAttr.STROKE_COLOR, DrawAttr.FILL_COLOR, DrawAttr.TEXT_DEFAULT_FILL, DrawAttr.CORNER_RADIUS});
		internal static readonly List<object> DEFAULTS_ALL = new List<object>
		{
			new object[] {DrawAttr.DEFAULT_FONT, DrawAttr.ALIGN_CENTER, DrawAttr.PAINT_STROKE, Convert.ToInt32(1), Color.Black, Color.White, Color.Black, Convert.ToInt32(10)}
		};

		private class Restriction : AbstractAttributeSet, AttributeListener
		{
			private readonly DrawingAttributeSet outerInstance;

			internal AbstractTool tool;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<logisim.data.Attribute<?>> selectedAttrs;
			internal List<Attribute> selectedAttrs;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<logisim.data.Attribute<?>> selectedView;
			internal List<Attribute> selectedView;

			internal Restriction(DrawingAttributeSet outerInstance, AbstractTool tool)
			{
				this.outerInstance = outerInstance;
				this.tool = tool;
				updateAttributes();
			}

			internal virtual void updateAttributes()
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<logisim.data.Attribute<?>> toolAttrs;
				List<Attribute> toolAttrs;
				if (tool == null)
				{
					toolAttrs = [];
				}
				else
				{
					toolAttrs = tool.Attributes;
				}
// JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
// ORIGINAL LINE: if (!toolAttrs.equals(selectedAttrs))
				if (!toolAttrs.SequenceEqual(selectedAttrs))
				{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: selectedAttrs = new java.util.ArrayList<logisim.data.Attribute<?>>(toolAttrs);
					selectedAttrs = new List<Attribute>(toolAttrs);
					selectedView = selectedAttrs.AsReadOnly();
					outerInstance.addAttributeListener(this);
					fireAttributeListChanged();
				}
			}

			protected internal override void copyInto(AbstractAttributeSet dest)
			{
				outerInstance.addAttributeListener(this);
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
			public override List<Attribute> Attributes
			{
				get
				{
					return selectedView;
				}
			}

			public override object getValue(Attribute attr)
			{
				return outerInstance.getValue(attr);
			}

			public override void setValue(Attribute attr, object value)
			{
				outerInstance.setValue(attr, value);
				updateAttributes();
			}

			//
			// AttributeListener methods
			//
			public virtual void attributeListChanged(AttributeEvent e)
			{
				fireAttributeListChanged();
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				if (selectedAttrs.Contains(e.Attribute))
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute attr = (logisim.data.Attribute) e.getAttribute();
					Attribute attr = (Attribute) e.Attribute;
					fireAttributeValueChanged(attr, e.Value);
				}
				updateAttributes();
			}
		}

		private EventSourceWeakSupport<AttributeListener> listeners;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<logisim.data.Attribute<?>> attrs;
		private List<Attribute> attrs;
		private List<object> values;

		public DrawingAttributeSet()
		{
			listeners = new EventSourceWeakSupport<AttributeListener>();
			attrs = ATTRS_ALL;
			values = DEFAULTS_ALL;
		}

		public virtual AttributeSet createSubset(AbstractTool tool)
		{
			return new Restriction(this, tool);
		}

		public virtual void addAttributeListener(AttributeListener l)
		{
			listeners.add(l);
		}

		public virtual void removeAttributeListener(AttributeListener l)
		{
			listeners.remove(l);
		}

		public virtual object Clone()
		{
			DrawingAttributeSet ret = (DrawingAttributeSet)base.MemberwiseClone();
			ret.listeners = new EventSourceWeakSupport<AttributeListener>();
			ret.values = new List<object>(this.values);
			return ret;
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public virtual List<Attribute> Attributes
		{
			get
			{
				return attrs;
			}
		}

		public virtual bool containsAttribute(Attribute attr)
		{
			return attrs.Contains(attr);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public logisim.data.Attribute<?> getAttribute(String name)
		public virtual Attribute getAttribute(string name)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : attrs)
			foreach (Attribute attr in attrs)
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
			throw new System.NotSupportedException("setReadOnly");
		}

		public virtual bool isToSave(Attribute attr)
		{
			return true;
		}

		public virtual object getValue(Attribute attr)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Iterator<logisim.data.Attribute<?>> ait = attrs.iterator();
			IEnumerator<Attribute> ait = attrs.GetEnumerator();
			IEnumerator<object> vit = values.GetEnumerator();
			while (ait.MoveNext())
			{
				object a = ait.Current;
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				object v = vit.next();
				if (a.Equals(attr))
				{
					return v;
				}
			}
			return null;
		}

		public virtual void setValue(Attribute attr, object value)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Iterator<logisim.data.Attribute<?>> ait = attrs.iterator();
			IEnumerator<Attribute> ait = attrs.GetEnumerator();
// JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
			IEnumerator<object> vit = values.GetEnumerator();
			while (ait.MoveNext())
			{
				object a = ait.Current;
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				vit.next();
				if (a.Equals(attr))
				{
					vit.set(value);
					AttributeEvent e = new AttributeEvent(this, attr, value);
					foreach (AttributeListener listener in listeners)
					{
						listener.attributeValueChanged(e);
					}
					if (attr == DrawAttr.PAINT_TYPE)
					{
						e = new AttributeEvent(this);
						foreach (AttributeListener listener in listeners)
						{
							listener.attributeListChanged(e);
						}
					}
					return;
				}
			}
			throw new System.ArgumentException(attr.ToString());
		}

		public virtual E applyTo<E>(E drawable) where E : draw.model.CanvasObject
		{
			AbstractCanvasObject d = (AbstractCanvasObject) drawable;
			// use a for(i...) loop since the attribute list may change as we go on
			for (int i = 0; i < d.Attributes.Count; i++)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = d.getAttributes().get(i);
				Attribute attr = d.Attributes[i];
				if (attr == DrawAttr.FILL_COLOR && this.containsAttribute(DrawAttr.TEXT_DEFAULT_FILL))
				{
					d.setValue(attr, this.getValue(DrawAttr.TEXT_DEFAULT_FILL));
				}
				else if (this.containsAttribute(attr))
				{
					object value = this.getValue(attr);
					d.setValue(attr, value);
				}
			}
			return drawable;
		}
	}

}
