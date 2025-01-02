// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.prefs
{

	internal class PrefOption
	{
		private object value;
		private string displayString;

		internal PrefOption(string value, string displayString)
		{
			this.value = value;
			this.displayString = displayString;
		}

		public override string ToString()
		{
			return displayString;
		}

		public virtual object Value
		{
			get
			{
				return value;
			}
		}

		internal static void setSelected(JComboBox<PrefOption> combo, object value)
		{
			for (int i = combo.getItemCount() - 1; i >= 0; i--)
			{
				PrefOption opt = combo.getItemAt(i);
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
