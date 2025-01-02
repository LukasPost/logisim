// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	using AppPreferences = logisim.prefs.AppPreferences;

	internal class LocaleSelector : JList<LocaleSelector.LocaleOption>, LocaleListener, ListSelectionListener
	{
		public class LocaleOption : ThreadStart
		{
			internal Locale locale;
			internal string text;

			internal LocaleOption(Locale locale)
			{
				this.locale = locale;
				update(locale);
			}

			public override string ToString()
			{
				return text;
			}

			internal virtual void update(Locale current)
			{
				if (current != null && current.Equals(locale))
				{
					text = locale.getDisplayName(locale);
				}
				else
				{
					text = locale.getDisplayName(locale) + " / " + locale.getDisplayName(current);
				}
			}

			public virtual void run()
			{
				if (!LocaleManager.Locale.Equals(locale))
				{
					LocaleManager.Locale = locale;
					AppPreferences.LOCALE.set(locale.getLanguage());
				}
			}
		}

		private LocaleOption[] items;

		internal LocaleSelector(Locale[] locales)
		{
			setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			DefaultListModel<LocaleOption> model = new DefaultListModel<LocaleOption>();
			items = new LocaleOption[locales.Length];
			for (int i = 0; i < locales.Length; i++)
			{
				items[i] = new LocaleOption(locales[i]);
				model.addElement(items[i]);
			}
			setModel(model);
			setVisibleRowCount(Math.Min(items.Length, 8));
			LocaleManager.addLocaleListener(this);
			localeChanged();
			addListSelectionListener(this);
		}

		public virtual void localeChanged()
		{
			Locale current = LocaleManager.Locale;
			LocaleOption sel = null;
			for (int i = 0; i < items.Length; i++)
			{
				items[i].update(current);
				if (current.Equals(items[i].locale))
				{
					sel = items[i];
				}
			}
			if (sel != null)
			{
				setSelectedValue(sel, true);
			}
		}

		public virtual void valueChanged(ListSelectionEvent e)
		{
			LocaleOption opt = (LocaleOption) getSelectedValue();
			if (opt != null)
			{
				SwingUtilities.invokeLater(opt);
			}
		}
	}

}
