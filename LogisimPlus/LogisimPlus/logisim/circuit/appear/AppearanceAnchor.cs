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

	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;

	using CanvasObject = draw.model.CanvasObject;
	using Handle = draw.model.Handle;
	using HandleGesture = draw.model.HandleGesture;
	using logisim.data;
	using Attributes = logisim.data.Attributes;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using logisim.util;

	public class AppearanceAnchor : AppearanceElement
	{
		public static readonly Attribute FACING = data.Attributes.forDirection("facing", Strings.getter("appearanceFacingAttr"));
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: static final java.util.List<logisim.data.Attribute<?>> ATTRIBUTES = logisim.util.UnmodifiableList.create(new logisim.data.Attribute<?>[] { FACING });
		internal static readonly List<Attribute> ATTRIBUTES = UnmodifiableList.create(new Attribute[] {FACING});

		private const int RADIUS = 3;
		private const int INDICATOR_LENGTH = 8;
		private static readonly Color SYMBOL_COLOR = Color.FromArgb(255, 0, 128, 0);

		private Direction facing;

		public AppearanceAnchor(Location location) : base(location)
		{
			facing = Direction.East;
		}

		public override bool matches(CanvasObject other)
		{
			if (other is AppearanceAnchor)
			{
				AppearanceAnchor that = (AppearanceAnchor) other;
				return base.matches(that) && this.facing.Equals(that.facing);
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			return base.matchesHashCode() * 31 + facing.GetHashCode();
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("circuitAnchor");
			}
		}

		public override Element toSvgElement(Document doc)
		{
			Location loc = Location;
			Element ret = doc.createElement("circ-anchor");
			ret.setAttribute("x", "" + (loc.X - RADIUS));
			ret.setAttribute("y", "" + (loc.Y - RADIUS));
			ret.setAttribute("width", "" + 2 * RADIUS);
			ret.setAttribute("height", "" + 2 * RADIUS);
			ret.setAttribute("facing", facing.ToString());
			return ret;
		}

		public virtual Direction Facing
		{
			get
			{
				return facing;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return ATTRIBUTES;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public virtual object getValue(Attribute attr)
		{
			if (attr == FACING)
			{
				return facing;
			}
			else
			{
				return base.getValue(attr);
			}
		}

		protected internal override void updateValue(Attribute attr, object value)
		{
			if (attr == FACING)
			{
				facing = (Direction) value;
			}
			else
			{
				base.updateValue(attr, value);
			}
		}

		public override void paint(JGraphics g, HandleGesture gesture)
		{
			Location location = Location;
			int x = location.X;
			int y = location.Y;
			g.setColor(SYMBOL_COLOR);
			g.drawOval(x - RADIUS, y - RADIUS, 2 * RADIUS, 2 * RADIUS);
			Location e0 = location.translate(facing, RADIUS);
			Location e1 = location.translate(facing, RADIUS + INDICATOR_LENGTH);
			g.drawLine(e0.X, e0.Y, e1.X, e1.Y);
		}

		public override Bounds Bounds
		{
			get
			{
				Bounds bds = base.getBounds(RADIUS);
				Location center = Location;
				Location end = center.translate(facing, RADIUS + INDICATOR_LENGTH);
				return bds.add(end);
			}
		}

		public override bool contains(Location loc, bool assumeFilled)
		{
			if (base.isInCircle(loc, RADIUS))
			{
				return true;
			}
			else
			{
				Location center = Location;
				Location end = center.translate(facing, RADIUS + INDICATOR_LENGTH);
				if (facing == Direction.East || facing == Direction.West)
				{
					return Math.Abs(loc.Y - center.Y) < 2 && (loc.X < center.X) != (loc.X < end.X);
				}
				else
				{
					return Math.Abs(loc.X - center.X) < 2 && (loc.Y < center.Y) != (loc.Y < end.Y);
				}
			}
		}

		public override List<Handle> getHandles(HandleGesture gesture)
		{
			Location c = Location;
			Location end = c.translate(facing, RADIUS + INDICATOR_LENGTH);
			return UnmodifiableList.create(new Handle[]
			{
				new Handle(this, c),
				new Handle(this, end)
			});
		}
	}

}
