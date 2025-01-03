// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using AttributeSets = logisim.data.AttributeSets;
	using BitWidth = logisim.data.BitWidth;
	using StdAttr = logisim.instance.StdAttr;

	internal class CounterAttributes : AbstractAttributeSet
	{
		private AttributeSet @base;

		public CounterAttributes()
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: super = logisim.data.AttributeSets.fixedSet(new logisim.data.Attribute<?>[] { logisim.instance.StdAttr.Width, Counter.ATTR_MAX, Counter.ATTR_ON_GOAL, logisim.instance.StdAttr.EDGE_TRIGGER, logisim.instance.StdAttr.LABEL, logisim.instance.StdAttr.LABEL_FONT }, new Object[] { logisim.data.BitWidth.create(8), System.Convert.ToInt32(0xFF), Counter.ON_GOAL_WRAP, logisim.instance.StdAttr.TRIG_RISING, "", logisim.instance.StdAttr.DEFAULT_LABEL_FONT });
			@base = AttributeSets.fixedSet(new Attribute[] {StdAttr.Width, Counter.ATTR_MAX, Counter.ATTR_ON_GOAL, StdAttr.EDGE_TRIGGER, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {BitWidth.create(8), Convert.ToInt32(0xFF), Counter.ON_GOAL_WRAP, StdAttr.TRIG_RISING, "", StdAttr.DEFAULT_LABEL_FONT});
		}

		protected internal override void copyInto(AbstractAttributeSet dest)
		{
			((CounterAttributes) dest).@base = (AttributeSet) this.@base.clone();
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return @base.Attributes;
			}
		}

		public override object getValue(Attribute attr)
		{
			return @base.getValue(attr);
		}

		public override void setValue(Attribute attr, object value)
		{
			object oldValue = @base.getValue(attr);
			if (oldValue == null ? value == null : oldValue.Equals(value))
			{
				return;
			}

			int? newMax = null;
			if (attr == StdAttr.Width)
			{
				BitWidth oldWidth = (BitWidth)@base.getValue(StdAttr.Width);
				BitWidth newWidth = (BitWidth) value;
				int oldW = oldWidth.Width;
				int newW = newWidth.Width;
				int? oldValObj = @base.getValue(Counter.ATTR_MAX);
				int oldVal = oldValObj.Value;
				@base.setValue(StdAttr.Width, newWidth);
				if (newW > oldW)
				{
					newMax = Convert.ToInt32(newWidth.Mask);
				}
				else
				{
					int v = oldVal & newWidth.Mask;
					if (v != oldVal)
					{
						int? newValObj = Convert.ToInt32(v);
						@base.setValue(Counter.ATTR_MAX, newValObj);
						fireAttributeValueChanged(Counter.ATTR_MAX, newValObj);
					}
				}
				fireAttributeValueChanged(StdAttr.Width, newWidth);
			}
			else if (attr == Counter.ATTR_MAX)
			{
				int oldVal = ((int?) value).Value;
				BitWidth width = (BitWidth)@base.getValue(StdAttr.Width);
				int newVal = oldVal & width.Mask;
				if (newVal != oldVal)
				{
                    // JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
                    // ORIGINAL LINE: @SuppressWarnings("unchecked") V val = (V) System.Convert.ToInt32(newVal);
                    value = Convert.ToInt32(newVal);
				}
				fireAttributeValueChanged(attr, value);
			}
			@base.setValue(attr, value);
			if (newMax != null)
			{
				@base.setValue(Counter.ATTR_MAX, newMax);
				fireAttributeValueChanged(Counter.ATTR_MAX, newMax);
			}
		}

		public override bool containsAttribute(Attribute attr)
		{
			return @base.containsAttribute(attr);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public logisim.data.Attribute<?> getAttribute(String name)
		public override Attribute getAttribute(string name)
		{
			return @base.getAttribute(name);
		}

		public override bool isReadOnly(Attribute attr)
		{
			return @base.isReadOnly(attr);
		}

		public override void setReadOnly(Attribute attr, bool value)
		{
			@base.setReadOnly(attr, value);
		}
	}

}
