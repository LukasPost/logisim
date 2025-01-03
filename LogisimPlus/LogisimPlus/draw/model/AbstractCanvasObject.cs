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
	using JGraphicsUtil = logisim.util.JGraphicsUtil;


	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
    using LogisimPlus.Java;

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

		public abstract List<Handle> getHandles(HandleGesture gesture);

		protected internal abstract void updateValue(Attribute attr, object value);

		public abstract void paint(JGraphics g, HandleGesture gesture);

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
		public abstract List<Attribute> Attributes {get;}

		public abstract object getValue(Attribute attr);

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
			AbstractCanvasObject ret = (AbstractCanvasObject)base.MemberwiseClone();
			ret.listeners = new EventSourceWeakSupport<AttributeListener>();
			return ret;
		}

		public virtual bool containsAttribute(Attribute attr)
		{
			return Attributes.Contains(attr);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public logisim.data.Attribute<?> getAttribute(String name)
		public virtual Attribute getAttribute(string name)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : getAttributes())
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
			throw new System.NotSupportedException("setReadOnly");
		}

		public virtual bool isToSave(Attribute attr)
		{
			return true;
		}

		public void setValue(Attribute attr, object value)
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

		protected internal virtual bool setForStroke(JGraphics g)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<logisim.data.Attribute<?>> attrs = getAttributes();
			List<Attribute> attrs = Attributes;
			if (attrs.Contains(DrawAttr.PAINT_TYPE))
			{
				object value = getValue(DrawAttr.PAINT_TYPE);
				if (value == DrawAttr.PAINT_FILL)
				{
					return false;
				}
			}

			int? width = (int)getValue(DrawAttr.STROKE_WIDTH);
			if (width != null && width.Value > 0)
			{
				Color color = (Color)getValue(DrawAttr.STROKE_COLOR);
				if (color != null && color.A == 0)
				{
					return false;
				}
				else
				{
					JGraphicsUtil.switchToWidth(g, width.Value);
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

		protected internal virtual bool setForFill(JGraphics g)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<logisim.data.Attribute<?>> attrs = getAttributes();
			List<Attribute> attrs = Attributes;
			if (attrs.Contains(DrawAttr.PAINT_TYPE))
			{
				object value = getValue(DrawAttr.PAINT_TYPE);
				if (value == DrawAttr.PAINT_STROKE)
				{
					return false;
				}
			}

			Color color = (Color)getValue(DrawAttr.FILL_COLOR);
			if (color != null && color.A == 0)
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
