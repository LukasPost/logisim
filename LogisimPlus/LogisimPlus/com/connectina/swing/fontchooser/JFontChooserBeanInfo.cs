// ====================================================================================================
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
	/// The bean information for the <code>JFontChooser</code> JavaBean.
	/// 
	/// @author Christos Bohoris </summary>
	/// <seealso cref="JFontChooser"/>
	public class JFontChooserBeanInfo : SimpleBeanInfo
	{

		/* 16x16 color icon. */
		private readonly Image iconColor16 = loadImage("connectina/FontChooser16Color.png");
		/* 32x32 color icon. */
		private readonly Image iconColor32 = loadImage("connectina/FontChooser32Color.png");
		/* 16x16 mono icon. */
		private readonly Image iconMono16 = loadImage("connectina/FontChooser16Mono.png");
		/* 32x32 mono icon. */
		private readonly Image iconMono32 = loadImage("connectina/FontChooser32Mono.png");
		/* The bean descriptor. */
		private JFontChooserBeanDescriptor descriptor = new JFontChooserBeanDescriptor();

		/// <summary>
		/// Get the bean descriptor.
		/// </summary>
		/// <returns> the bean descriptor </returns>
		// Java5 @Override
		public virtual BeanDescriptor BeanDescriptor
		{
			get
			{
				return descriptor;
			}
		}

		/// <summary>
		/// Get the appropriate icon.
		/// </summary>
		/// <param name="iconKind"> the icon kind </param>
		/// <returns> the image </returns>
		// Java5 @Override
		public virtual Image getIcon(int iconKind)
		{
			switch (iconKind)
			{
			case ICON_COLOR_16x16:
				return iconColor16;
			case ICON_COLOR_32x32:
				return iconColor32;
			case ICON_MONO_16x16:
				return iconMono16;
			case ICON_MONO_32x32:
				return iconMono32;
			}

			return null;
		}

	}

}
