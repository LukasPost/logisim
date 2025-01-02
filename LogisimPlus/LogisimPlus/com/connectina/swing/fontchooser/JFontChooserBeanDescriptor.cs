﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

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
	/// The bean descriptor for the <code>JFontChooser</code> JavaBean.
	/// 
	/// @author Christos Bohoris </summary>
	/// <seealso cref="JFontChooser"/>
	public class JFontChooserBeanDescriptor : BeanDescriptor
	{

		public JFontChooserBeanDescriptor() : base(typeof(JFontChooser))
		{
			setShortDescription("<html>com.connectina.fontchooser.JFontChooser<br>A font selection pane.</html>");
			setDisplayName("Font Chooser");
			setPreferred(true);
		}

	}

}