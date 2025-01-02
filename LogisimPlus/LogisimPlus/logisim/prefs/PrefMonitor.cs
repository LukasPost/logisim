// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.prefs
{

	public interface PrefMonitor<E> : PreferenceChangeListener
	{
		string Identifier {get;}

		bool isSource(PropertyChangeEvent @event);

		void addPropertyChangeListener(PropertyChangeListener listener);

		void removePropertyChangeListener(PropertyChangeListener listener);

		E get();

		void set(E value);

		bool Boolean {get;set;}


		void preferenceChange(PreferenceChangeEvent e);
	}

}
