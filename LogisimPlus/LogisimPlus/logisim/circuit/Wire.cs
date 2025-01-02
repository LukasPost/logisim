// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentListener = logisim.comp.ComponentListener;
	using EndData = logisim.comp.EndData;
	using logisim.data;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using CustomHandles = logisim.tools.CustomHandles;
	using Cache = logisim.util.Cache;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public sealed class Wire : Component, AttributeSet, CustomHandles, IEnumerable<Location>
	{
		/// <summary>
		/// Stroke width when drawing wires. </summary>
		public const int WIDTH = 3;

		public static readonly AttributeOption VALUE_HORZ = new AttributeOption("horz", Strings.getter("wireDirectionHorzOption"));
		public static readonly AttributeOption VALUE_VERT = new AttributeOption("vert", Strings.getter("wireDirectionVertOption"));
		public static readonly Attribute<AttributeOption> dir_attr = Attributes.forOption("direction", Strings.getter("wireDirectionAttr"), new AttributeOption[] {VALUE_HORZ, VALUE_VERT});
		public static readonly Attribute<int> len_attr = Attributes.forInteger("length", Strings.getter("wireLengthAttr"));

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRIBUTES = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { dir_attr, len_attr });
		private static readonly IList<Attribute<object>> ATTRIBUTES = new List<Attribute<object>> {dir_attr, len_attr};
		private static readonly Cache cache = new Cache();

		public static Wire create(Location e0, Location e1)
		{
			return (Wire) cache.get(new Wire(e0, e1));
		}

		private class EndList : System.Collections.ObjectModel.Collection<EndData>
		{
			private readonly Wire outerInstance;

			public EndList(Wire outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override EndData get(int i)
			{
				return outerInstance.getEnd(i);
			}

			public override int size()
			{
				return 2;
			}
		}

		internal readonly Location e0;
		internal readonly Location e1;
		internal readonly bool is_x_equal;

		private Wire(Location e0, Location e1)
		{
			this.is_x_equal = e0.X == e1.X;
			if (is_x_equal)
			{
				if (e0.Y > e1.Y)
				{
					this.e0 = e1;
					this.e1 = e0;
				}
				else
				{
					this.e0 = e0;
					this.e1 = e1;
				}
			}
			else
			{
				if (e0.X > e1.X)
				{
					this.e0 = e1;
					this.e1 = e0;
				}
				else
				{
					this.e0 = e0;
					this.e1 = e1;
				}
			}
		}

		public override bool Equals(object other)
		{
			if (!(other is Wire))
			{
				return false;
			}
			Wire w = (Wire) other;
			return w.e0.Equals(this.e0) && w.e1.Equals(this.e1);
		}

		public override int GetHashCode()
		{
			return e0.GetHashCode() * 31 + e1.GetHashCode();
		}

		public int Length
		{
			get
			{
				return (e1.Y - e0.Y) + (e1.X - e0.X);
			}
		}

		public override string ToString()
		{
			return "Wire[" + e0 + "-" + e1 + "]";
		}

		//
		// Component methods
		//
		// (Wire never issues ComponentEvents, so we don't need to track listeners)
		public void addComponentListener(ComponentListener e)
		{
		}

		public void removeComponentListener(ComponentListener e)
		{
		}

		public ComponentFactory Factory
		{
			get
			{
				return WireFactory.instance;
			}
		}

		public AttributeSet AttributeSet
		{
			get
			{
				return this;
			}
		}

		// location/extent methods
		public Location Location
		{
			get
			{
				return e0;
			}
		}

		public Bounds Bounds
		{
			get
			{
				int x0 = e0.X;
				int y0 = e0.Y;
				return Bounds.create(x0 - 2, y0 - 2, e1.X - x0 + 5, e1.Y - y0 + 5);
			}
		}

		public Bounds getBounds(Graphics g)
		{
			return Bounds;
		}

		public bool contains(Location q)
		{
			int qx = q.X;
			int qy = q.Y;
			if (is_x_equal)
			{
				int wx = e0.X;
				return qx >= wx - 2 && qx <= wx + 2 && e0.Y <= qy && qy <= e1.Y;
			}
			else
			{
				int wy = e0.Y;
				return qy >= wy - 2 && qy <= wy + 2 && e0.X <= qx && qx <= e1.X;
			}
		}

		public bool contains(Location pt, Graphics g)
		{
			return contains(pt);
		}

		//
		// propagation methods
		//
		public IList<EndData> Ends
		{
			get
			{
				return new EndList(this);
			}
		}

		public EndData getEnd(int index)
		{
			Location loc = getEndLocation(index);
			return new EndData(loc, BitWidth.UNKNOWN, EndData.INPUT_OUTPUT);
		}

		public bool endsAt(Location pt)
		{
			return e0.Equals(pt) || e1.Equals(pt);
		}

		public void propagate(CircuitState state)
		{
			// Normally this is handled by CircuitWires, and so it won't get
			// called. The exception is when a wire is added or removed
			state.markPointAsDirty(e0);
			state.markPointAsDirty(e1);
		}

		//
		// user interface methods
		//
		public void expose(ComponentDrawContext context)
		{
			java.awt.Component dest = context.Destination;
			int x0 = e0.X;
			int y0 = e0.Y;
			dest.repaint(x0 - 5, y0 - 5, e1.X - x0 + 10, e1.Y - y0 + 10);
		}

		public void draw(ComponentDrawContext context)
		{
			CircuitState state = context.CircuitState;
			Graphics g = context.Graphics;

			GraphicsUtil.switchToWidth(g, WIDTH);
			g.setColor(state.getValue(e0).Color);
			g.drawLine(e0.X, e0.Y, e1.X, e1.Y);
		}

		public object getFeature(object key)
		{
			if (key == typeof(CustomHandles))
			{
				return this;
			}
			return null;
		}

		//
		// AttributeSet methods
		//
		// It makes some sense for a wire to be its own attribute, since
		// after all it is immutable.
		//
		public object clone()
		{
			return this;
		}

		public void addAttributeListener(AttributeListener l)
		{
		}

		public void removeAttributeListener(AttributeListener l)
		{
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public IList<Attribute<object>> Attributes
		{
			get
			{
				return ATTRIBUTES;
			}
		}

		public bool containsAttribute<T1>(Attribute<T1> attr)
		{
			return ATTRIBUTES.Contains(attr);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public logisim.data.Attribute<?> getAttribute(String name)
		public Attribute<object> getAttribute(string name)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : ATTRIBUTES)
			foreach (Attribute<object> attr in ATTRIBUTES)
			{
				if (name.Equals(attr.Name))
				{
					return attr;
				}
			}
			return null;
		}

		public bool isReadOnly<T1>(Attribute<T1> attr)
		{
			return true;
		}

		public void setReadOnly<T1>(Attribute<T1> attr, bool value)
		{
			throw new System.NotSupportedException();
		}

		public bool isToSave<T1>(Attribute<T1> attr)
		{
			return false;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public V getValue<V>(Attribute<V> attr)
		{
			if (attr == dir_attr)
			{
				return (V)(is_x_equal ? VALUE_VERT : VALUE_HORZ);
			}
			else if (attr == len_attr)
			{
				return (V) Convert.ToInt32(Length);
			}
			else
			{
				return null;
			}
		}

		public void setValue<V>(Attribute<V> attr, V value)
		{
			throw new System.ArgumentException("read only attribute");
		}

		//
		// other methods
		//
		public bool Vertical
		{
			get
			{
				return is_x_equal;
			}
		}

		public Location getEndLocation(int index)
		{
			return index == 0 ? e0 : e1;
		}

		public Location End0
		{
			get
			{
				return e0;
			}
		}

		public Location End1
		{
			get
			{
				return e1;
			}
		}

		public Location getOtherEnd(Location loc)
		{
			return (loc.Equals(e0) ? e1 : e0);
		}

		public bool sharesEnd(Wire other)
		{
			return this.e0.Equals(other.e0) || this.e1.Equals(other.e0) || this.e0.Equals(other.e1) || this.e1.Equals(other.e1);
		}

		public bool overlaps(Wire other, bool includeEnds)
		{
			return overlaps(other.e0, other.e1, includeEnds);
		}

		private bool overlaps(Location q0, Location q1, bool includeEnds)
		{
			if (is_x_equal)
			{
				int x0 = q0.X;
				if (x0 != q1.X || x0 != e0.X)
				{
					return false;
				}
				if (includeEnds)
				{
					return e1.Y >= q0.Y && e0.Y <= q1.Y;
				}
				else
				{
					return e1.Y > q0.Y && e0.Y < q1.Y;
				}
			}
			else
			{
				int y0 = q0.Y;
				if (y0 != q1.Y || y0 != e0.Y)
				{
					return false;
				}
				if (includeEnds)
				{
					return e1.X >= q0.X && e0.X <= q1.X;
				}
				else
				{
					return e1.X > q0.X && e0.X < q1.X;
				}
			}
		}

		public bool isParallel(Wire other)
		{
			return this.is_x_equal == other.is_x_equal;
		}

		public IEnumerator<Location> GetEnumerator()
		{
			return new WireIterator(e0, e1);
		}

		public void drawHandles(ComponentDrawContext context)
		{
			context.drawHandle(e0);
			context.drawHandle(e1);
		}
	}

}
