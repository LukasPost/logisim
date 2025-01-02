// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{

	using StringGetter = logisim.util.StringGetter;

	public abstract class Attribute<V>
	{
		private string name;
		private StringGetter disp;

		public Attribute(string name, StringGetter disp)
		{
			this.name = name;
			this.disp = disp;
		}

		public override string ToString()
		{
			return name;
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				return disp.get();
			}
		}

		public virtual java.awt.Component getCellEditor(Window source, V value)
		{
			return getCellEditor(value);
		}

		protected internal virtual java.awt.Component getCellEditor(V value)
		{
			return new JTextField(toDisplayString(value));
		}

		public virtual string toDisplayString(V value)
		{
			return value == null ? "" : value.ToString();
		}

		public virtual string toStandardString(V value)
		{
			return value.ToString();
		}

		public abstract V parse(string value);
	}

}
