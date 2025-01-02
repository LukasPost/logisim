// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.prefs
{

	internal class PrefMonitorString : AbstractPrefMonitor<string>
	{
		private string dflt;
		private string value;

		internal PrefMonitorString(string name, string dflt) : base(name)
		{
			this.dflt = dflt;
			Preferences prefs = AppPreferences.Prefs;
			this.value = prefs.get(name, dflt);
			prefs.addPreferenceChangeListener(this);
		}

		public override string get()
		{
			return value;
		}

		public virtual void set(string newValue)
		{
			string oldValue = value;
			if (!isSame(oldValue, newValue))
			{
				value = newValue;
				AppPreferences.Prefs.put(Identifier, newValue);
			}
		}

		public override void preferenceChange(PreferenceChangeEvent @event)
		{
			Preferences prefs = @event.getNode();
			string prop = @event.getKey();
			string name = Identifier;
			if (prop.Equals(name))
			{
				string oldValue = value;
				string newValue = prefs.get(name, dflt);
				if (!isSame(oldValue, newValue))
				{
					value = newValue;
					AppPreferences.firePropertyChange(name, oldValue, newValue);
				}
			}
		}

		private static bool isSame(string a, string b)
		{
			return string.ReferenceEquals(a, null) ? string.ReferenceEquals(b, null) : a.Equals(b);
		}
	}

}
