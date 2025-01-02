// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{

	using AttributeOption = logisim.data.AttributeOption;
	using StringGetter = logisim.util.StringGetter;

	internal class ComboOption
	{
		private AttributeOption value;
		private StringGetter getter;

		internal ComboOption(AttributeOption value)
		{
			this.value = value;
			this.getter = null;
		}

		public override string ToString()
		{
			if (getter != null)
			{
				return getter.get();
			}
			return value.toDisplayString();
		}

		public virtual AttributeOption Value
		{
			get
			{
				return value;
			}
		}

		internal static void setSelected(JComboBox<ComboOption> combo, AttributeOption value)
		{
			for (int i = combo.getItemCount() - 1; i >= 0; i--)
			{
				ComboOption opt = combo.getItemAt(i);
				if (opt.Value.Equals(value))
				{
					combo.setSelectedItem(opt);
					return;
				}
			}
			combo.setSelectedItem(combo.getItemAt(0));
		}

	}

}
