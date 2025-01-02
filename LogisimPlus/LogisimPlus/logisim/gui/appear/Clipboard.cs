// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{

	using PropertyChangeWeakSupport = logisim.util.PropertyChangeWeakSupport;

	internal class Clipboard
	{
		private Clipboard()
		{
		}

		public const string contentsProperty = "appearance";

		private static ClipboardContents current = ClipboardContents.EMPTY;
		private static PropertyChangeWeakSupport propertySupport = new PropertyChangeWeakSupport(typeof(Clipboard));

		public static bool Empty
		{
			get
			{
				return current == null || current.Elements.Count == 0;
			}
		}

		public static ClipboardContents get()
		{
			return current;
		}

		public static void set(ClipboardContents value)
		{
			ClipboardContents old = current;
			current = value;
			propertySupport.firePropertyChange(contentsProperty, old, current);
		}

		//
		// PropertyChangeSource methods
		//
		public static void addPropertyChangeListener(PropertyChangeListener listener)
		{
			propertySupport.addPropertyChangeListener(listener);
		}

		public static void addPropertyChangeListener(string propertyName, PropertyChangeListener listener)
		{
			propertySupport.addPropertyChangeListener(propertyName, listener);
		}

		public static void removePropertyChangeListener(PropertyChangeListener listener)
		{
			propertySupport.removePropertyChangeListener(listener);
		}

		public static void removePropertyChangeListener(string propertyName, PropertyChangeListener listener)
		{
			propertySupport.removePropertyChangeListener(propertyName, listener);
		}
	}

}
