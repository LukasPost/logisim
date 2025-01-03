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

	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Direction = logisim.data.Direction;
	using StdAttr = logisim.instance.StdAttr;

	internal class SplitterAttributes : AbstractAttributeSet
	{
		public static readonly AttributeOption APPEAR_LEGACY = new AttributeOption("legacy", Strings.getter("splitterAppearanceLegacy"));
		public static readonly AttributeOption APPEAR_LEFT = new AttributeOption("left", Strings.getter("splitterAppearanceLeft"));
		public static readonly AttributeOption APPEAR_RIGHT = new AttributeOption("right", Strings.getter("splitterAppearanceRight"));
		public static readonly AttributeOption APPEAR_CENTER = new AttributeOption("center", Strings.getter("splitterAppearanceCenter"));

		public static readonly Attribute ATTR_APPEARANCE = Attributes.forOption("appear", Strings.getter("splitterAppearanceAttr"), new AttributeOption[] {APPEAR_LEFT, APPEAR_RIGHT, APPEAR_CENTER, APPEAR_LEGACY});

		public static readonly Attribute ATTR_WIDTH = Attributes.forBitWidth("incoming", Strings.getter("splitterBitWidthAttr"));
		public static readonly Attribute ATTR_FANOUT = Attributes.forIntegerRange("fanout", Strings.getter("splitterFanOutAttr"), 1, 32);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> INIT_ATTRIBUTES = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { logisim.instance.StdAttr.FACING, ATTR_FANOUT, ATTR_WIDTH, ATTR_APPEARANCE});
		private static readonly List<Attribute> INIT_ATTRIBUTES = new List<Attribute> {StdAttr.FACING, ATTR_FANOUT, ATTR_WIDTH, ATTR_APPEARANCE};

		private const string unchosen_val = "none";

		private class BitOutOption
		{
			internal int value;
			internal bool isVertical;
			internal bool isLast;

			internal BitOutOption(int value, bool isVertical, bool isLast)
			{
				this.value = value;
				this.isVertical = isVertical;
				this.isLast = isLast;
			}

			public override string ToString()
			{
				if (value < 0)
				{
					return Strings.get("splitterBitNone");
				}
				else
				{
					string ret = "" + value;
					Direction noteDir;
					if (value == 0)
					{
						noteDir = isVertical ? Direction.North : Direction.East;
					}
					else if (isLast)
					{
						noteDir = isVertical ? Direction.South : Direction.West;
					}
					else
					{
						noteDir = null;
					}
					if (noteDir != null)
					{
						ret += " (" + noteDir.toVerticalDisplayString() + ")";
					}
					return ret;
				}
			}
		}

		internal class BitOutAttribute : Attribute
		{
			internal int which;
			internal BitOutOption[] options;

			internal BitOutAttribute(int which, BitOutOption[] options) : base("bit" + which, Strings.getter("splitterBitAttr", "" + which))
			{
				this.which = which;
				this.options = options;
			}

			internal virtual BitOutAttribute createCopy()
			{
				return new BitOutAttribute(which, options);
			}

			public virtual object Default
			{
				get
				{
					return Convert.ToInt32(which + 1);
				}
			}

			public override int? parse(string value)
			{
				if (value.Equals(unchosen_val))
				{
					return Convert.ToInt32(0);
				}
				else
				{
					return Convert.ToInt32(1 + int.Parse(value));
				}
			}

			public override string toDisplayString(int? value)
			{
				int index = value.Value;
				return options[index].ToString();
			}

			public override string toStandardString(int? value)
			{
				int index = value.Value;
				if (index == 0)
				{
					return unchosen_val;
				}
				else
				{
					return "" + (index - 1);
				}
			}

			public override java.awt.Component getCellEditor(int? value)
			{
				int index = value.Value;
				javax.swing.JComboBox<BitOutOption> combo = new javax.swing.JComboBox<BitOutOption>(options);
				combo.setSelectedIndex(index);
				return combo;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.ArrayList<logisim.data.Attribute<?>> attrs = new java.util.ArrayList<logisim.data.Attribute<?>>(INIT_ATTRIBUTES);
		private List<Attribute> attrs = new List<Attribute>(INIT_ATTRIBUTES);
		private SplitterParameters parameters;
		internal AttributeOption appear = APPEAR_LEFT;
		internal Direction facing = Direction.East;
		internal sbyte fanout = 2; // number of ends this splits into
		internal sbyte[] bit_end = new sbyte[2]; // how each bit maps to an end (0 if nowhere);
										// other values will be between 1 and fanout
		internal BitOutOption[] options = null;

		internal SplitterAttributes()
		{
			configureOptions();
			configureDefaults();
			parameters = new SplitterParameters(this);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> getBitOutAttribute(int index)
		internal virtual Attribute getBitOutAttribute(int index)
		{
			return attrs[INIT_ATTRIBUTES.Count + index];
		}

		protected internal override void copyInto(AbstractAttributeSet destObj)
		{
			SplitterAttributes dest = (SplitterAttributes) destObj;
			dest.parameters = this.parameters;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: dest.attrs = new java.util.ArrayList<logisim.data.Attribute<?>>(this.attrs.size());
			dest.attrs = new List<Attribute>(this.attrs.Count);
			dest.attrs.AddRange(INIT_ATTRIBUTES);
			for (int i = INIT_ATTRIBUTES.Count, n = this.attrs.Count; i < n; i++)
			{
				BitOutAttribute attr = (BitOutAttribute) this.attrs[i];
				dest.attrs.Add(attr.createCopy());
			}

			dest.facing = this.facing;
			dest.fanout = this.fanout;
			dest.appear = this.appear;
			dest.bit_end = (sbyte[])this.bit_end.Clone();
			dest.options = this.options;
		}

		public virtual SplitterParameters Parameters
		{
			get
			{
				SplitterParameters ret = parameters;
				if (ret == null)
				{
					ret = new SplitterParameters(this);
					parameters = ret;
				}
				return ret;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return attrs;
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
			else if (attr == ATTR_FANOUT)
			{
				return Convert.ToInt32(fanout);
			}
			else if (attr == ATTR_WIDTH)
			{
				return BitWidth.create(bit_end.Length);
			}
			else if (attr == ATTR_APPEARANCE)
			{
				return appear;
			}
			else if (attr is BitOutAttribute)
			{
				BitOutAttribute bitOut = (BitOutAttribute) attr;
				return Convert.ToInt32(bit_end[bitOut.which]);
			}
			else
			{
				return null;
			}
		}

		public override void setValue(Attribute attr, object value)
		{
			if (attr == StdAttr.FACING)
			{
				facing = (Direction) value;
				configureOptions();
				parameters = null;
			}
			else if (attr == ATTR_FANOUT)
			{
				int newValue = ((int?) value).Value;
				sbyte[] bits = bit_end;
				for (int i = 0; i < bits.Length; i++)
				{
					if (bits[i] >= newValue)
					{
						bits[i] = (sbyte)(newValue - 1);
					}
				}
				fanout = (sbyte) newValue;
				configureOptions();
				configureDefaults();
				parameters = null;
			}
			else if (attr == ATTR_WIDTH)
			{
				BitWidth width = (BitWidth) value;
				bit_end = new sbyte[width.Width];
				configureOptions();
				configureDefaults();
			}
			else if (attr == ATTR_APPEARANCE)
			{
				appear = (AttributeOption) value;
				parameters = null;
			}
			else if (attr is BitOutAttribute)
			{
				BitOutAttribute bitOutAttr = (BitOutAttribute) attr;
				int val;
				if (value is int?)
				{
					val = ((int?) value).Value;
				}
				else
				{
					val = ((BitOutOption) value).value + 1;
				}
				if (val >= 0 && val <= fanout)
				{
					bit_end[bitOutAttr.which] = (sbyte) val;
				}
			}
			else
			{
				throw new System.ArgumentException("unknown attribute " + attr);
			}
			fireAttributeValueChanged(attr, value);
		}

		private void configureOptions()
		{
			// compute the set of options for BitOutAttributes
			options = new BitOutOption[fanout + 1];
			bool isVertical = facing == Direction.East || facing == Direction.West;
			for (int i = -1; i < fanout; i++)
			{
				options[i + 1] = new BitOutOption(i, isVertical, i == fanout - 1);
			}

			// go ahead and set the options for the existing attributes
			int offs = INIT_ATTRIBUTES.Count;
			int curNum = attrs.Count - offs;
			for (int i = 0; i < curNum; i++)
			{
				BitOutAttribute attr = (BitOutAttribute) attrs[offs + i];
				attr.options = options;
			}
		}

		private void configureDefaults()
		{
			int offs = INIT_ATTRIBUTES.Count;
			int curNum = attrs.Count - offs;

			// compute default values
			sbyte[] dflt = computeDistribution(fanout, bit_end.Length, 1);

			bool changed = curNum != bit_end.Length;

			// remove excess attributes
			while (curNum > bit_end.Length)
			{
				curNum--;
				attrs.RemoveAt(offs + curNum);
			}

			// set existing attributes
			for (int i = 0; i < curNum; i++)
			{
				if (bit_end[i] != dflt[i])
				{
					BitOutAttribute attr = (BitOutAttribute) attrs[offs + i];
					bit_end[i] = dflt[i];
					fireAttributeValueChanged(attr, Convert.ToInt32(bit_end[i]));
				}
			}

			// add new attributes
			for (int i = curNum; i < bit_end.Length; i++)
			{
				BitOutAttribute attr = new BitOutAttribute(i, options);
				bit_end[i] = dflt[i];
				attrs.Add(attr);
			}

			if (changed)
			{
				fireAttributeListChanged();
			}
		}

		internal static sbyte[] computeDistribution(int fanout, int bits, int order)
		{
			sbyte[] ret = new sbyte[bits];
			if (order >= 0)
			{
				if (fanout >= bits)
				{
					for (int i = 0; i < bits; i++)
					{
						ret[i] = (sbyte)(i + 1);
					}
				}
				else
				{
					int threads_per_end = bits / fanout;
					int ends_with_extra = bits % fanout;
					int cur_end = -1; // immediately increments
					int left_in_end = 0;
					for (int i = 0; i < bits; i++)
					{
						if (left_in_end == 0)
						{
							++cur_end;
							left_in_end = threads_per_end;
							if (ends_with_extra > 0)
							{
								++left_in_end;
								--ends_with_extra;
							}
						}
						ret[i] = (sbyte)(1 + cur_end);
						--left_in_end;
					}
				}
			}
			else
			{
				if (fanout >= bits)
				{
					for (int i = 0; i < bits; i++)
					{
						ret[i] = (sbyte)(fanout - i);
					}
				}
				else
				{
					int threads_per_end = bits / fanout;
					int ends_with_extra = bits % fanout;
					int cur_end = -1;
					int left_in_end = 0;
					for (int i = bits - 1; i >= 0; i--)
					{
						if (left_in_end == 0)
						{
							++cur_end;
							left_in_end = threads_per_end;
							if (ends_with_extra > 0)
							{
								++left_in_end;
								--ends_with_extra;
							}
						}
						ret[i] = (sbyte)(1 + cur_end);
						--left_in_end;
					}
				}
			}
			return ret;
		}
	}

}
