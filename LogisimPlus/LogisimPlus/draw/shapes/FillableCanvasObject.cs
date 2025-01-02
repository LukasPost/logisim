﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.shapes
{

	using CanvasObject = draw.model.CanvasObject;
	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;

	internal abstract class FillableCanvasObject : AbstractCanvasObject
	{
		private AttributeOption paintType;
		private int strokeWidth;
		private Color strokeColor;
		private Color fillColor;

		public FillableCanvasObject()
		{
			paintType = DrawAttr.PAINT_STROKE;
			strokeWidth = 1;
			strokeColor = Color.BLACK;
			fillColor = Color.WHITE;
		}

		public override bool matches(CanvasObject other)
		{
			if (other is FillableCanvasObject)
			{
				FillableCanvasObject that = (FillableCanvasObject) other;
				bool ret = this.paintType == that.paintType;
				if (ret && this.paintType != DrawAttr.PAINT_FILL)
				{
					ret = ret && this.strokeWidth == that.strokeWidth && this.strokeColor.Equals(that.strokeColor);
				}
				if (ret && this.paintType != DrawAttr.PAINT_STROKE)
				{
					ret = ret && this.fillColor.Equals(that.fillColor);
				}
				return ret;
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			int ret = paintType.GetHashCode();
			if (paintType != DrawAttr.PAINT_FILL)
			{
				ret = ret * 31 + strokeWidth;
				ret = ret * 31 + strokeColor.GetHashCode();
			}
			else
			{
				ret = ret * 31 * 31;
			}
			if (paintType != DrawAttr.PAINT_STROKE)
			{
				ret = ret * 31 + fillColor.GetHashCode();
			}
			else
			{
				ret = ret * 31;
			}
			return ret;
		}

		public virtual AttributeOption PaintType
		{
			get
			{
				return paintType;
			}
		}

		public virtual int StrokeWidth
		{
			get
			{
				return strokeWidth;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public virtual V getValue<V>(Attribute<V> attr)
		{
			if (attr == DrawAttr.PAINT_TYPE)
			{
				return (V) paintType;
			}
			else if (attr == DrawAttr.STROKE_COLOR)
			{
				return (V) strokeColor;
			}
			else if (attr == DrawAttr.FILL_COLOR)
			{
				return (V) fillColor;
			}
			else if (attr == DrawAttr.STROKE_WIDTH)
			{
				return (V) Convert.ToInt32(strokeWidth);
			}
			else
			{
				return null;
			}
		}

		public override void updateValue<T1>(Attribute<T1> attr, object value)
		{
			if (attr == DrawAttr.PAINT_TYPE)
			{
				paintType = (AttributeOption) value;
				fireAttributeListChanged();
			}
			else if (attr == DrawAttr.STROKE_COLOR)
			{
				strokeColor = (Color) value;
			}
			else if (attr == DrawAttr.FILL_COLOR)
			{
				fillColor = (Color) value;
			}
			else if (attr == DrawAttr.STROKE_WIDTH)
			{
				strokeWidth = ((int?) value).Value;
			}
		}
	}

}