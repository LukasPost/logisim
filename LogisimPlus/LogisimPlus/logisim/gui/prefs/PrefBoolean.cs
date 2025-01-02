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

	internal class PrefBoolean : JCheckBox, ActionListener, PropertyChangeListener
	{
		private PrefMonitor<bool> pref;
		private StringGetter title;

		internal PrefBoolean(PrefMonitor<bool> pref, StringGetter title) : base(title.get())
		{
			this.pref = pref;
			this.title = title;

			addActionListener(this);
			pref.addPropertyChangeListener(this);
			setSelected(pref.Boolean);
		}

		internal virtual void localeChanged()
		{
			setText(title.get());
		}

		public virtual void actionPerformed(ActionEvent e)
		{
			pref.Boolean = this.isSelected();
		}

		public virtual void propertyChange(PropertyChangeEvent @event)
		{
			if (pref.isSource(@event))
			{
				setSelected(pref.Boolean);
			}
		}
	}

}
