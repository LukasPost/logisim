// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{
	using DrawAttr = draw.shapes.DrawAttr;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using logisim.util;
	using GraphicsUtil = logisim.util.GraphicsUtil;


	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;

	public abstract class AbstractCanvasObject : AttributeSet, CanvasObject, ICloneable
	{
		private const int OVERLAP_TRIES = 50;
		private const int GENERATE_RANDOM_TRIES = 20;

		private EventSourceWeakSupport<AttributeListener> listeners;

		public AbstractCanvasObject()
		{
			listeners = new EventSourceWeakSupport<AttributeListener>();
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return this;
			}
		}

		public abstract string DisplayName {get;}

		public abstract Element toSvgElement(Document doc);

		public abstract bool matches(CanvasObject other);

		public abstract int matchesHashCode();

		public abstract Bounds Bounds {get;}

		public abstract bool contains(Location loc, bool assumeFilled);

		public abstract void translate(int dx, int dy);

		public abstract IList<Handle> getHandles(HandleGesture gesture);

		protected internal abstract void updateValue<T1>(Attribute<T1> attr, object value);

		public abstract void paint(Graphics g, HandleGesture gesture);

		public virtual bool canRemove()
		{
			return true;
		}

		public virtual bool canMoveHandle(Handle handle)
		{
			return false;
		}

		public virtual Handle canInsertHandle(Location desired)
		{
			return null;
		}

		public virtual Handle canDeleteHandle(Location loc)
		{
			return null;
		}

		public virtual Handle moveHandle(HandleGesture gesture)
		{
			throw new System.NotSupportedException("moveHandle");
		}

		public virtual void insertHandle(Handle desired, Handle previous)
		{
			throw new System.NotSupportedException("insertHandle");
		}

		public virtual Handle deleteHandle(Handle handle)
		{
			throw new System.NotSupportedException("deleteHandle");
		}

		public virtual bool overlaps(CanvasObject other)
		{
			Bounds a = this.Bounds;
			Bounds b = other.Bounds;
			Bounds c = a.intersect(b);
			Random rand = new Random();
			if (c.Width == 0 || c.Height == 0)
			{
				return false;
			}
			else if (other is AbstractCanvasObject)
			{
				AbstractCanvasObject that = (AbstractCanvasObject) other;
				for (int i = 0; i < OVERLAP_TRIES; i++)
				{
					if (i % 2 == 0)
					{
						Location loc = this.getRandomPoint(c, rand);
						if (loc != null && that.contains(loc, false))
						{
							return true;
						}
					}
					else
					{
						Location loc = that.getRandomPoint(c, rand);
						if (loc != null && this.contains(loc, false))
						{
							return true;
						}
					}
				}
				return false;
			}
			else
			{
				for (int i = 0; i < OVERLAP_TRIES; i++)
				{
					Location loc = this.getRandomPoint(c, rand);
					if (loc != null && other.contains(loc, false))
					{
						return true;
					}
				}
				return false;
			}
		}

		protected internal virtual Location getRandomPoint(Bounds bds, Random rand)
		{
			int x = bds.X;
			int y = bds.Y;
			int w = bds.Width;
			int h = bds.Height;
			for (int i = 0; i < GENERATE_RANDOM_TRIES; i++)
			{
				Location loc = new Location(x + rand.Next(w), y + rand.Next(h));
				if (contains(loc, false))
				{
					return loc;
				}
			}
			return null;
		}

		// methods required by AttributeSet interface
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public abstract java.util.List<logisim.data.Attribute<?>> getAttributes();
		public abstract IList<Attribute<object>> Attributes {get;}

		public abstract V getValue<V>(Attribute<V> attr);

		public virtual void addAttributeListener(AttributeListener l)
		{
			listeners.add(l);
		}

		public virtual void removeAttributeListener(AttributeListener l)
		{
			listeners.remove(l);
		}

		public virtual CanvasObject clone()
		{
			try
			{
				AbstractCanvasObject ret = (AbstractCanvasObject) base.clone();
				ret.listeners = new EventSourceWeakSupport<AttributeListener>();
				return ret;
			}
			catch (CloneNotSupportedException)
			{
				return null;
			}
		}

		public virtual bool containsAttribute<T1>(Attribute<T1> attr)
		{
			return Attributes.Contains(attr);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public logisim.data.Attribute<?> getAttribute(String name)
		public virtual Attribute<object> getAttribute(string name)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : getAttributes())
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
			throw new System.NotSupportedException("setReadOnly");
		}

		public virtual bool isToSave<T1>(Attribute<T1> attr)
		{
			return true;
		}

		public void setValue<V>(Attribute<V> attr, V value)
		{
			object old = getValue(attr);
			bool same = old == null ? value == null : old.Equals(value);
			if (!same)
			{
				updateValue(attr, value);
				AttributeEvent e = new AttributeEvent(this, attr, value);
				foreach (AttributeListener listener in listeners)
				{
					listener.attributeValueChanged(e);
				}
			}
		}

		protected internal virtual void fireAttributeListChanged()
		{
			AttributeEvent e = new AttributeEvent(this);
			foreach (AttributeListener listener in listeners)
			{
				listener.attributeListChanged(e);
			}
		}

		protected internal virtual bool setForStroke(Graphics g)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<logisim.data.Attribute<?>> attrs = getAttributes();
			IList<Attribute<object>> attrs = Attributes;
			if (attrs.Contains(DrawAttr.PAINT_TYPE))
			{
				object value = getValue(DrawAttr.PAINT_TYPE);
				if (value == DrawAttr.PAINT_FILL)
				{
					return false;
				}
			}

			int? width = getValue(DrawAttr.STROKE_WIDTH);
			if (width != null && width.Value > 0)
			{
				Color color = getValue(DrawAttr.STROKE_COLOR);
				if (color != null && color.getAlpha() == 0)
				{
					return false;
				}
				else
				{
					GraphicsUtil.switchToWidth(g, width.Value);
					if (color != null)
					{
						g.setColor(color);
					}
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		protected internal virtual bool setForFill(Graphics g)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<logisim.data.Attribute<?>> attrs = getAttributes();
			IList<Attribute<object>> attrs = Attributes;
			if (attrs.Contains(DrawAttr.PAINT_TYPE))
			{
				object value = getValue(DrawAttr.PAINT_TYPE);
				if (value == DrawAttr.PAINT_STROKE)
				{
					return false;
				}
			}

			Color color = getValue(DrawAttr.FILL_COLOR);
			if (color != null && color.getAlpha() == 0)
			{
				return false;
			}
			else
			{
				if (color != null)
				{
					g.setColor(color);
				}
				return true;
			}
		}

	}

}
