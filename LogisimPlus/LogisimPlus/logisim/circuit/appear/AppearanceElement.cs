// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit.appear
{

	using CanvasObject = draw.model.CanvasObject;
	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using logisim.data;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;

	public abstract class AppearanceElement : AbstractCanvasObject
	{
		private Location location;

		public AppearanceElement(Location location)
		{
			this.location = location;
		}

		public virtual Location Location
		{
			get
			{
				return location;
			}
		}

		public override bool matches(CanvasObject other)
		{
			if (other is AppearanceElement)
			{
				AppearanceElement that = (AppearanceElement) other;
				return this.location.Equals(that.location);
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			return location.GetHashCode();
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return Collections.emptyList();
			}
		}

		public override V getValue<V>(Attribute<V> attr)
		{
			return null;
		}

		public override bool canRemove()
		{
			return false;
		}

		protected internal override void updateValue<T1>(Attribute<T1> attr, object value)
		{
			// nothing to do
		}

		public override void translate(int dx, int dy)
		{
			location = location.translate(dx, dy);
		}

		protected internal virtual bool isInCircle(Location loc, int radius)
		{
			int dx = loc.X - location.X;
			int dy = loc.Y - location.Y;
			return dx * dx + dy * dy < radius * radius;
		}

		public override Location getRandomPoint(Bounds bds, Random rand)
		{
			return null; // this is only used to determine what lies on top of what - but the elements will always be on top
							// anyway
		}

		protected internal virtual Bounds getBounds(int radius)
		{
			return Bounds.create(location.X - radius, location.Y - radius, 2 * radius, 2 * radius);
		}
	}

}
