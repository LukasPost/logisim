// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.prefs
{


	using logisim.prefs;
	using StringGetter = logisim.util.StringGetter;

	internal class PrefOptionList : ActionListener, PropertyChangeListener
	{
		private PrefMonitor<string> pref;
		private StringGetter labelStr;

		private JLabel label;
		private JComboBox<PrefOption> combo;

		public PrefOptionList(PrefMonitor<string> pref, StringGetter labelStr, PrefOption[] options)
		{
			this.pref = pref;
			this.labelStr = labelStr;

			label = new JLabel(labelStr.get() + " ");
			combo = new JComboBox<PrefOption>();
			foreach (PrefOption opt in options)
			{
				combo.addItem(opt);
			}

			combo.addActionListener(this);
			pref.addPropertyChangeListener(this);
			selectOption(pref.get());
		}

		internal virtual JPanel createJPanel()
		{
			JPanel ret = new JPanel();
			ret.add(label);
			ret.add(combo);
			return ret;
		}

		internal virtual JLabel JLabel
		{
			get
			{
				return label;
			}
		}

		internal virtual JComboBox<PrefOption> JComboBox
		{
			get
			{
				return combo;
			}
		}

		internal virtual void localeChanged()
		{
			label.setText(labelStr.get() + " ");
		}

		public virtual void actionPerformed(ActionEvent e)
		{
			PrefOption x = (PrefOption) combo.getSelectedItem();
			pref.set((string) x.Value);
		}

		public virtual void propertyChange(PropertyChangeEvent @event)
		{
			if (pref.isSource(@event))
			{
				selectOption(pref.get());
			}
		}

		private void selectOption(object value)
		{
			for (int i = combo.getItemCount() - 1; i >= 0; i--)
			{
				PrefOption opt = (PrefOption) combo.getItemAt(i);
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
