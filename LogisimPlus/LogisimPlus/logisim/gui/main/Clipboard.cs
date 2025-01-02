// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using Component = logisim.comp.Component;
	using AttributeSet = logisim.data.AttributeSet;
	using PropertyChangeWeakSupport = logisim.util.PropertyChangeWeakSupport;

	internal class Clipboard
	{
		public const string contentsProperty = "contents";

		private static Clipboard current = null;
		private static PropertyChangeWeakSupport propertySupport = new PropertyChangeWeakSupport(typeof(Clipboard));

		public static bool Empty
		{
			get
			{
				return current == null || current.components.Count == 0;
			}
		}

		public static Clipboard get()
		{
			return current;
		}

		public static void set(Selection value, AttributeSet oldAttrs)
		{
			set(new Clipboard(value, oldAttrs));
		}

		public static void set(Clipboard value)
		{
			Clipboard old = current;
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

		//
		// instance variables and methods
		//
		private HashSet<Component> components;
		private AttributeSet oldAttrs;
		private AttributeSet newAttrs;

		private Clipboard(Selection sel, AttributeSet viewAttrs)
		{
			components = new HashSet<Component>();
			oldAttrs = null;
			newAttrs = null;
			foreach (Component @base in sel.Components)
			{
				AttributeSet baseAttrs = @base.AttributeSet;
				AttributeSet copyAttrs = (AttributeSet) baseAttrs.clone();
				Component copy = @base.Factory.createComponent(@base.Location, copyAttrs);
				components.Add(copy);
				if (baseAttrs == viewAttrs)
				{
					oldAttrs = baseAttrs;
					newAttrs = copyAttrs;
				}
			}
		}

		public virtual ICollection<Component> Components
		{
			get
			{
				return components;
			}
		}

		public virtual AttributeSet OldAttributeSet
		{
			get
			{
				return oldAttrs;
			}
			set
			{
				oldAttrs = value;
			}
		}

		public virtual AttributeSet NewAttributeSet
		{
			get
			{
				return newAttrs;
			}
		}

	}

}
