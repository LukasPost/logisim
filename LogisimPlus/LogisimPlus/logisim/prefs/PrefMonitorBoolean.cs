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

	internal class PrefMonitorBoolean : AbstractPrefMonitor<bool>
	{
		private bool dflt;
		private bool value;

		internal PrefMonitorBoolean(string name, bool dflt) : base(name)
		{
			this.dflt = dflt;
			this.value = dflt;
			Preferences prefs = AppPreferences.Prefs;
			set(Convert.ToBoolean(prefs.getBoolean(name, dflt)));
			prefs.addPreferenceChangeListener(this);
		}

		public override bool? get()
		{
			return Convert.ToBoolean(value);
		}

		public override bool Boolean
		{
			get
			{
				return value;
			}
		}

		public virtual void set(bool? newValue)
		{
			bool newVal = newValue.Value;
			if (value != newVal)
			{
				AppPreferences.Prefs.putBoolean(Identifier, newVal);
			}
		}

		public override void preferenceChange(PreferenceChangeEvent @event)
		{
			Preferences prefs = @event.getNode();
			string prop = @event.getKey();
			string name = Identifier;
			if (prop.Equals(name))
			{
				bool oldValue = value;
				bool newValue = prefs.getBoolean(name, dflt);
				if (newValue != oldValue)
				{
					value = newValue;
					AppPreferences.firePropertyChange(name, oldValue, newValue);
				}
			}
		}
	}

}
