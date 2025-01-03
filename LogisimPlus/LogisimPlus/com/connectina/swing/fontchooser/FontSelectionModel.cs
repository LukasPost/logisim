// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

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
	/// A model that supports selecting a <code>Font</code>.
	/// 
	/// @author Christos Bohoris </summary>
	/// <seealso cref="java.awt.Font"/>
	public interface FontSelectionModel
	{

		/// <summary>
		/// Returns the selected <code>Font</code> which should be non-<code>null</code>.
		/// </summary>
		/// <returns> the selected <code>Font</code> </returns>
		/// <seealso cref=".setSelectedFont"/>
		Font SelectedFont {get;set;}


		/// <summary>
		/// Gets the available font names. Returns a list containing the names of all font families in this
		/// <code>JGraphicsEnvironment</code> localized for the default locale, as returned by
		/// <code>Locale.getDefault()</code>.
		/// </summary>
		/// <returns> a list of String containing font family names localized for the default locale, or a suitable alternative
		///         name if no name exists for this locale </returns>
		List<string> AvailableFontNames {get;}

		/// <summary>
		/// Adds <code>listener</code> as a listener to changes in the model.
		/// </summary>
		/// <param name="listener"> the <code>ChangeListener</code> to be added </param>
		void addChangeListener(ChangeListener listener);

		/// <summary>
		/// Removes <code>listener</code> as a listener to changes in the model.
		/// </summary>
		/// <param name="listener"> the <code>ChangeListener</code> to be removed </param>
		void removeChangeListener(ChangeListener listener);

	}

}
