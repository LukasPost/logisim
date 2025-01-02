// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{


	using ColorPicker = com.bric.swing.ColorPicker;
	using FontUtil = logisim.util.FontUtil;
	using JInputComponent = logisim.util.JInputComponent;
	using StringGetter = logisim.util.StringGetter;
	using JFontChooser = com.connectina.swing.fontchooser.JFontChooser;

	public class Attributes
	{
		private Attributes()
		{
		}

		private class ConstantGetter : StringGetter
		{
			internal string str;

			public ConstantGetter(string str)
			{
				this.str = str;
			}

			public virtual string get()
			{
				return str;
			}

			public override string ToString()
			{
				return get();
			}
		}

		private static StringGetter getter(string s)
		{
			return new ConstantGetter(s);
		}

		//
		// methods with display name == standard name
		//
		public static Attribute<string> forString(string name)
		{
			return forString(name, getter(name));
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public static Attribute<?> forOption(String name, Object[] vals)
		public static Attribute<object> forOption(string name, object[] vals)
		{
			return forOption(name, getter(name), vals);
		}

		public static Attribute<int> forInteger(string name)
		{
			return forInteger(name, getter(name));
		}

		public static Attribute<int> forHexInteger(string name)
		{
			return forHexInteger(name, getter(name));
		}

		public static Attribute<int> forIntegerRange(string name, int start, int end)
		{
			return forIntegerRange(name, getter(name), start, end);
		}

		public static Attribute<double> forDouble(string name)
		{
			return forDouble(name, getter(name));
		}

		public static Attribute<bool> forBoolean(string name)
		{
			return forBoolean(name, getter(name));
		}

		public static Attribute<Direction> forDirection(string name)
		{
			return forDirection(name, getter(name));
		}

		public static Attribute<BitWidth> forBitWidth(string name)
		{
			return forBitWidth(name, getter(name));
		}

		public static Attribute<BitWidth> forBitWidth(string name, int min, int max)
		{
			return forBitWidth(name, getter(name), min, max);
		}

		public static Attribute<Font> forFont(string name)
		{
			return forFont(name, getter(name));
		}

		public static Attribute<Location> forLocation(string name)
		{
			return forLocation(name, getter(name));
		}

		public static Attribute<Color> forColor(string name)
		{
			return forColor(name, getter(name));
		}

		//
		// methods with internationalization support
		//
		public static Attribute<string> forString(string name, StringGetter disp)
		{
			return new StringAttribute(name, disp);
		}

		public static Attribute<V> forOption<V>(string name, StringGetter disp, V[] vals)
		{
			return new OptionAttribute<V>(name, disp, vals);
		}

		public static Attribute<int> forInteger(string name, StringGetter disp)
		{
			return new IntegerAttribute(name, disp);
		}

		public static Attribute<int> forHexInteger(string name, StringGetter disp)
		{
			return new HexIntegerAttribute(name, disp);
		}

		public static Attribute<int> forIntegerRange(string name, StringGetter disp, int start, int end)
		{
			return new IntegerRangeAttribute(name, disp, start, end);
		}

		public static Attribute<double> forDouble(string name, StringGetter disp)
		{
			return new DoubleAttribute(name, disp);
		}

		public static Attribute<bool> forBoolean(string name, StringGetter disp)
		{
			return new BooleanAttribute(name, disp);
		}

		public static Attribute<Direction> forDirection(string name, StringGetter disp)
		{
			return new DirectionAttribute(name, disp);
		}

		public static Attribute<BitWidth> forBitWidth(string name, StringGetter disp)
		{
			return new BitWidth.Attribute(name, disp);
		}

		public static Attribute<BitWidth> forBitWidth(string name, StringGetter disp, int min, int max)
		{
			return new BitWidth.Attribute(name, disp, min, max);
		}

		public static Attribute<Font> forFont(string name, StringGetter disp)
		{
			return new FontAttribute(name, disp);
		}

		public static Attribute<Location> forLocation(string name, StringGetter disp)
		{
			return new LocationAttribute(name, disp);
		}

		public static Attribute<Color> forColor(string name, StringGetter disp)
		{
			return new ColorAttribute(name, disp);
		}

		private class StringAttribute : Attribute<string>
		{
			internal StringAttribute(string name, StringGetter disp) : base(name, disp)
			{
			}

			public override string parse(string value)
			{
				return value;
			}
		}

		private class OptionComboRenderer<V> : BasicComboBoxRenderer
		{
			internal Attribute<V> attr;

			internal OptionComboRenderer(Attribute<V> attr)
			{
				this.attr = attr;
			}

			public override Component getListCellRendererComponent<T1>(JList<T1> list, object value, int index, bool isSelected, bool cellHasFocus)
			{
				Component ret = base.getListCellRendererComponent(list, value, index, isSelected, cellHasFocus);
				if (ret is JLabel)
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") V val = (V) value;
					V val = (V) value;
					((JLabel) ret).setText(value == null ? "" : attr.toDisplayString(val));
				}
				return ret;
			}
		}

		private class OptionAttribute<V> : Attribute<V>
		{
			internal V[] vals;

			internal OptionAttribute(string name, StringGetter disp, V[] vals) : base(name, disp)
			{
				this.vals = vals;
			}

			public override string toDisplayString(V value)
			{
				if (value is AttributeOptionInterface)
				{
					return ((AttributeOptionInterface) value).toDisplayString();
				}
				else
				{
					return value.ToString();
				}
			}

			public override V parse(string value)
			{
				for (int i = 0; i < vals.Length; i++)
				{
					if (value.Equals(vals[i].ToString()))
					{
						return vals[i];
					}
				}
				throw new System.FormatException("value not among choices");
			}

			public override Component getCellEditor(object value)
			{
				JComboBox<V> combo = new JComboBox<V>(vals);
				combo.setRenderer(new OptionComboRenderer<V>(this));
				if (value == null)
				{
					combo.setSelectedIndex(-1);
				}
				else
				{
					combo.setSelectedItem(value);
				}
				return combo;
			}
		}

		private class IntegerAttribute : Attribute<int>
		{
			internal IntegerAttribute(string name, StringGetter disp) : base(name, disp)
			{
			}

			public override int? parse(string value)
			{
				return Convert.ToInt32(value);
			}
		}

		private class HexIntegerAttribute : Attribute<int>
		{
			internal HexIntegerAttribute(string name, StringGetter disp) : base(name, disp)
			{
			}

			public override string toDisplayString(int? value)
			{
				int val = value.Value;
				return "0x" + Convert.ToString(val, 16);
			}

			public override string toStandardString(int? value)
			{
				return toDisplayString(value.Value);
			}

			public override int? parse(string value)
			{
				value = value.ToLower();
				if (value.StartsWith("0x", StringComparison.Ordinal))
				{
					value = value.Substring(2);
					return Convert.ToInt32((int) Convert.ToInt64(value, 16));
				}
				else if (value.StartsWith("0b", StringComparison.Ordinal))
				{
					value = value.Substring(2);
					return Convert.ToInt32((int) Convert.ToInt64(value, 2));
				}
				else if (value.StartsWith("0", StringComparison.Ordinal))
				{
					value = value.Substring(1);
					return Convert.ToInt32((int) Convert.ToInt64(value, 8));
				}
				else
				{
					return Convert.ToInt32((int) Convert.ToInt64(value, 10));
				}

			}
		}

		private class DoubleAttribute : Attribute<double>
		{
			internal DoubleAttribute(string name, StringGetter disp) : base(name, disp)
			{
			}

			public override double? parse(string value)
			{
				return Convert.ToDouble(value);
			}
		}

		private class BooleanAttribute : OptionAttribute<bool>
		{
			internal static bool?[] vals = new bool?[] {true, false};

			internal BooleanAttribute(string name, StringGetter disp) : base(name, disp, vals)
			{
			}

			public override string toDisplayString(bool? value)
			{
				if (value.Value)
				{
					return Strings.get("booleanTrueOption");
				}
				else
				{
					return Strings.get("booleanFalseOption");
				}
			}

			public override bool? parse(string value)
			{
				bool? b = Convert.ToBoolean(value);
				return vals[b.Value ? 0 : 1];
			}
		}

		private class IntegerRangeAttribute : Attribute<int>
		{
			internal int?[] options = null;
			internal int start;
			internal int end;

			internal IntegerRangeAttribute(string name, StringGetter disp, int start, int end) : base(name, disp)
			{
				this.start = start;
				this.end = end;
			}

			public override int? parse(string value)
			{
				int v = (int) long.Parse(value);
				if (v < start)
				{
					throw new System.FormatException("integer too small");
				}
				if (v > end)
				{
					throw new System.FormatException("integer too large");
				}
				return Convert.ToInt32(v);
			}

			public override Component getCellEditor(int? value)
			{
				if (end - start + 1 > 32)
				{
					return base.getCellEditor(value.Value);
				}
				else
				{
					if (options == null)
					{
						options = new int?[end - start + 1];
						for (int i = start; i <= end; i++)
						{
							options[i - start] = Convert.ToInt32(i);
						}
					}
					JComboBox<int> combo = new JComboBox<int>(options);
					if (value == null)
					{
						combo.setSelectedIndex(-1);
					}
					else
					{
						combo.setSelectedItem(value);
					}
					return combo;
				}
			}
		}

		private class DirectionAttribute : OptionAttribute<Direction>
		{
			public DirectionAttribute(string name, StringGetter disp) : base(name, disp, Direction.getCardinals())
			{
			}

			public override string toDisplayString(Direction value)
			{
				return value == null ? "???" : value.toDisplayString();
			}

			public override Direction parse(string value)
			{
				return Direction.parse(value);
			}
		}

		private class FontAttribute : Attribute<Font>
		{
			internal FontAttribute(string name, StringGetter disp) : base(name, disp)
			{
			}

			public override string toDisplayString(Font f)
			{
				if (f == null)
				{
					return "???";
				}
				return f.getFamily() + " " + FontUtil.toStyleDisplayString(f.getStyle()) + " " + f.getSize();
			}

			public override string toStandardString(Font f)
			{
				return f.getFamily() + " " + FontUtil.toStyleStandardString(f.getStyle()) + " " + f.getSize();
			}

			public override Font parse(string value)
			{
				return Font.decode(value);
			}

			public override Component getCellEditor(Font value)
			{
				return new FontChooser(value);
			}
		}

		private class FontChooser : JFontChooser, JInputComponent
		{
			internal FontChooser(Font initial) : base(initial)
			{
			}

			public virtual object Value
			{
				get
				{
					return SelectedFont;
				}
				set
				{
					SelectedFont = (Font) value;
				}
			}

		}

		private class LocationAttribute : Attribute<Location>
		{
			public LocationAttribute(string name, StringGetter desc) : base(name, desc)
			{
			}

			public override Location parse(string value)
			{
				return Location.parse(value);
			}
		}

		private class ColorAttribute : Attribute<Color>
		{
			public ColorAttribute(string name, StringGetter desc) : base(name, desc)
			{
			}

			public override string toDisplayString(Color value)
			{
				return toStandardString(value);
			}

			public override string toStandardString(Color c)
			{
				string ret = "#" + hex(c.getRed()) + hex(c.getGreen()) + hex(c.getBlue());
				return c.getAlpha() == 255 ? ret : ret + hex(c.getAlpha());
			}

			internal virtual string hex(int value)
			{
				if (value >= 16)
				{
					return Convert.ToString(value, 16);
				}
				else
				{
					return "0" + Convert.ToString(value, 16);
				}
			}

			public override Color parse(string value)
			{
				if (value.Length == 9)
				{
					int r = Convert.ToInt32(value.Substring(1, 2), 16);
					int g = Convert.ToInt32(value.Substring(3, 2), 16);
					int b = Convert.ToInt32(value.Substring(5, 2), 16);
					int a = Convert.ToInt32(value.Substring(7, 2), 16);
					return new Color(r, g, b, a);
				}
				else
				{
					return Color.decode(value);
				}
			}

			public override Component getCellEditor(Color value)
			{
				Color init = value == null ? Color.BLACK : value;
				return new ColorChooser(init);
			}
		}

		private class ColorChooser : ColorPicker, JInputComponent
		{
			internal ColorChooser(Color initial)
			{
				if (initial != null)
				{
					Color = initial;
				}
				OpacityVisible = true;
			}

			public virtual object Value
			{
				get
				{
					return Color;
				}
				set
				{
					Color = (Color) value;
				}
			}

		}
	}

}
