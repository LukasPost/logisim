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

	internal class PrefMonitorDouble : AbstractPrefMonitor<double>
	{
		private double dflt;
		private double value;

		internal PrefMonitorDouble(string name, double dflt) : base(name)
		{
			this.dflt = dflt;
			this.value = dflt;
			Preferences prefs = AppPreferences.Prefs;
			set(Convert.ToDouble(prefs.getDouble(name, dflt)));
			prefs.addPreferenceChangeListener(this);
		}

		public override double? get()
		{
			return Convert.ToDouble(value);
		}

		public virtual void set(double? newValue)
		{
			double newVal = newValue.Value;
			if (value != newVal)
			{
				AppPreferences.Prefs.putDouble(Identifier, newVal);
			}
		}

		public override void preferenceChange(PreferenceChangeEvent @event)
		{
			Preferences prefs = @event.getNode();
			string prop = @event.getKey();
			string name = Identifier;
			if (prop.Equals(name))
			{
				double oldValue = value;
				double newValue = prefs.getDouble(name, dflt);
				if (newValue != oldValue)
				{
					value = newValue;
					AppPreferences.firePropertyChange(name, Convert.ToDouble(oldValue), Convert.ToDouble(newValue));
				}
			}
		}
	}

}
