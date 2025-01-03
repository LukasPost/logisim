// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using TextField = logisim.comp.TextField;
	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using StdAttr = logisim.instance.StdAttr;

	internal class TunnelAttributes : AbstractAttributeSet
	{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> ATTRIBUTES = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { logisim.instance.StdAttr.FACING, logisim.instance.StdAttr.Width, logisim.instance.StdAttr.LABEL, logisim.instance.StdAttr.LABEL_FONT });
		private static readonly List<Attribute> ATTRIBUTES = new List<Attribute> {StdAttr.FACING, StdAttr.Width, StdAttr.LABEL, StdAttr.LABEL_FONT};

		private Direction facing;
		private BitWidth width;
		private string label;
		private Font labelFont;
		private Bounds offsetBounds;
		private int labelX;
		private int labelY;
		private int labelHAlign;
		private int labelVAlign;

		public TunnelAttributes()
		{
			facing = Direction.West;
			width = BitWidth.ONE;
			label = "";
			labelFont = StdAttr.DEFAULT_LABEL_FONT;
			offsetBounds = null;
			configureLabel();
		}

		internal virtual Direction Facing
		{
			get
			{
				return facing;
			}
		}

		internal virtual string Label
		{
			get
			{
				return label;
			}
		}

		internal virtual Font Font
		{
			get
			{
				return labelFont;
			}
		}

		internal virtual Bounds OffsetBounds
		{
			get
			{
				return offsetBounds;
			}
		}

		internal virtual int LabelX
		{
			get
			{
				return labelX;
			}
		}

		internal virtual int LabelY
		{
			get
			{
				return labelY;
			}
		}

		internal virtual int LabelHAlign
		{
			get
			{
				return labelHAlign;
			}
		}

		internal virtual int LabelVAlign
		{
			get
			{
				return labelVAlign;
			}
		}

		internal virtual bool setOffsetBounds(Bounds value)
		{
			Bounds old = offsetBounds;
			bool same = old == null ? value == null : old.Equals(value);
			if (!same)
			{
				offsetBounds = value;
			}
			return !same;
		}

		protected internal override void copyInto(AbstractAttributeSet destObj)
		{
			; // nothing to do
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
		public override object getValue(Attribute attr)
		{
			if (attr == StdAttr.FACING)
			{
				return facing;
			}
			if (attr == StdAttr.Width)
			{
				return width;
			}
			if (attr == StdAttr.LABEL)
			{
				return label;
			}
			if (attr == StdAttr.LABEL_FONT)
			{
				return labelFont;
			}
			return null;
		}

		public override void setValue(Attribute attr, object value)
		{
			if (attr == StdAttr.FACING)
			{
				facing = (Direction) value;
				configureLabel();
			}
			else if (attr == StdAttr.Width)
			{
				width = (BitWidth) value;
			}
			else if (attr == StdAttr.LABEL)
			{
				label = (string) value;
			}
			else if (attr == StdAttr.LABEL_FONT)
			{
				labelFont = (Font) value;
			}
			else
			{
				throw new System.ArgumentException("unknown attribute");
			}
			offsetBounds = null;
			fireAttributeValueChanged(attr, value);
		}

		private void configureLabel()
		{
			Direction facing = this.facing;
			int x;
			int y;
			int halign;
			int valign;
			int margin = Tunnel.ARROW_MARGIN;
			if (facing == Direction.North)
			{
				x = 0;
				y = margin;
				halign = TextField.H_CENTER;
				valign = TextField.V_TOP;
			}
			else if (facing == Direction.South)
			{
				x = 0;
				y = -margin;
				halign = TextField.H_CENTER;
				valign = TextField.V_BOTTOM;
			}
			else if (facing == Direction.East)
			{
				x = -margin;
				y = 0;
				halign = TextField.H_RIGHT;
				valign = TextField.V_CENTER_OVERALL;
			}
			else
			{
				x = margin;
				y = 0;
				halign = TextField.H_LEFT;
				valign = TextField.V_CENTER_OVERALL;
			}
			labelX = x;
			labelY = y;
			labelHAlign = halign;
			labelVAlign = valign;
		}
	}

}
