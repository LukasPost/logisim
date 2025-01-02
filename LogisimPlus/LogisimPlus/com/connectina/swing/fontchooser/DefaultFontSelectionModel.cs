// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * A font chooser JavaBean component.
 * Copyright (C) 2009 Dr Christos Bohoris
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License version 3 as published by the Free Software Foundation;
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 *
 * swing@connectina.com
 */
namespace com.connectina.swing.fontchooser
{

	/// <summary>
	/// A generic implementation of <code>FontSelectionModel</code>.
	/// 
	/// @author Christos Bohoris </summary>
	/// <seealso cref="java.awt.Font"/>
	public class DefaultFontSelectionModel : FontSelectionModel
	{

		/// <summary>
		/// Only one <code>ChangeEvent</code> is needed per model instance since the event's only (read-only) state is the
		/// source property. The source of events generated here is always "this".
		/// </summary>
		[NonSerialized]
		protected internal ChangeEvent changeEvent = null;

		/// <summary>
		/// A list of registered event listeners.
		/// </summary>
		protected internal EventListenerList listenerList = new EventListenerList();

		private Font selectedFont;

		private IList<string> availableFontNames = new List<string>();

		/// <summary>
		/// Creates a <code>DefaultFontSelectionModel</code> with the current font set to
		/// <code>new Font(Font.SANS_SERIF, Font.PLAIN, 12)
		/// </code>. This is the default constructor.
		/// </summary>
		public DefaultFontSelectionModel() : this(new Font(JFontChooser.SANS_SERIF, Font.PLAIN, 12))
		{
		}

		/// <summary>
		/// Creates a <code>DefaultFontSelectionModel</code> with the current font set to <code>font</code>, which should be
		/// non-<code>null</code>. Note that setting the font to <code>null</code> is undefined and may have unpredictable
		/// results.
		/// </summary>
		/// <param name="font"> the new <code>Font</code> </param>
		public DefaultFontSelectionModel(Font font)
		{
			selectedFont = font;
			GraphicsEnvironment ge = GraphicsEnvironment.getLocalGraphicsEnvironment();
			string[] families = ge.getAvailableFontFamilyNames();
			for (int i = 0; i < families.Length; i++)
			{
				availableFontNames.Add(families[i]);
			}
		}

		/// <summary>
		/// Returns the selected <code>Font</code> which should be non-<code>null</code>.
		/// </summary>
		/// <returns> the selected <code>Font</code> </returns>
		public virtual Font SelectedFont
		{
			get
			{
				return selectedFont;
			}
			set
			{
				if (value != null && !selectedFont.Equals(value))
				{
					selectedFont = value;
					fireStateChanged();
				}
			}
		}


		/// <summary>
		/// Gets the available font names. Returns a list containing the names of all font families in this
		/// <code>GraphicsEnvironment</code> localized for the default locale, as returned by
		/// <code>Locale.getDefault()</code>.
		/// </summary>
		/// <returns> a list of String containing font family names localized for the default locale, or a suitable alternative
		///         name if no name exists for this locale </returns>
		public virtual IList<string> AvailableFontNames
		{
			get
			{
				return availableFontNames;
			}
		}

		/// <summary>
		/// Adds a <code>ChangeListener</code> to the model.
		/// </summary>
		/// <param name="l"> the <code>ChangeListener</code> to be added </param>
		public virtual void addChangeListener(ChangeListener l)
		{
			listenerList.add(typeof(ChangeListener), l);
		}

		/// <summary>
		/// Removes a <code>ChangeListener</code> from the model.
		/// </summary>
		/// <param name="l"> the <code>ChangeListener</code> to be removed </param>
		public virtual void removeChangeListener(ChangeListener l)
		{
			listenerList.remove(typeof(ChangeListener), l);
		}

		/// <summary>
		/// Returns an array of all the <code>ChangeListener</code>s added to this <code>DefaultFontSelectionModel</code>
		/// with <code>addChangeListener</code>.
		/// </summary>
		/// <returns> all of the <code>ChangeListener</code>s added, or an empty array if no listeners have been added </returns>
		public virtual ChangeListener[] ChangeListeners
		{
			get
			{
				return (ChangeListener[]) listenerList.getListeners(typeof(ChangeListener));
			}
		}

		/// <summary>
		/// Runs each <code>ChangeListener</code>'s <code>stateChanged</code> method.
		/// </summary>
		protected internal virtual void fireStateChanged()
		{
			object[] listeners = listenerList.getListenerList();
			for (int i = listeners.Length - 2; i >= 0; i -= 2)
			{
				if (listeners[i] == typeof(ChangeListener))
				{
					if (changeEvent == null)
					{
						changeEvent = new ChangeEvent(this);
					}
					((ChangeListener) listeners[i + 1]).stateChanged(changeEvent);
				}
			}
		}

	}

}
