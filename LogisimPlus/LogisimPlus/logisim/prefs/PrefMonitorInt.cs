// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.prefs
{

	internal class PrefMonitorInt : AbstractPrefMonitor<int>
	{
		private int dflt;
		private int value;

		internal PrefMonitorInt(string name, int dflt) : base(name)
		{
			this.dflt = dflt;
			this.value = dflt;
			Preferences prefs = AppPreferences.Prefs;
			set(Convert.ToInt32(prefs.getInt(name, dflt)));
			prefs.addPreferenceChangeListener(this);
		}

		public override int? get()
		{
			return Convert.ToInt32(value);
		}

		public virtual void set(int? newValue)
		{
			int newVal = newValue.Value;
			if (value != newVal)
			{
				AppPreferences.Prefs.putInt(Identifier, newVal);
			}
		}

		public override void preferenceChange(PreferenceChangeEvent @event)
		{
			Preferences prefs = @event.getNode();
			string prop = @event.getKey();
			string name = Identifier;
			if (prop.Equals(name))
			{
				int oldValue = value;
				int newValue = prefs.getInt(name, dflt);
				if (newValue != oldValue)
				{
					value = newValue;
					AppPreferences.firePropertyChange(name, Convert.ToInt32(oldValue), Convert.ToInt32(newValue));
				}
			}
		}
	}

}
