// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/*
* @(#)ImageTransferable.java  1.0  2008-03-01
*
* Copyright (c) 2008 Jeremy Wood
* E-mail: mickleness@gmail.com
* All rights reserved.
*
* The copyright of this software is owned by Jeremy Wood.
* You may not use, copy or modify this software, except in
* accordance with the license agreement you entered into with
* Jeremy Wood. For details see accompanying license terms.
*/

namespace com.bric.swing
{

	internal class ImageTransferable : Transferable
	{
		internal Image img;

		public ImageTransferable(Image i)
		{
			img = i;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Object getTransferData(java.awt.datatransfer.DataFlavor f) throws UnsupportedFlavorException, java.io.IOException
		public virtual object getTransferData(DataFlavor f)
		{
			if (f.Equals(DataFlavor.imageFlavor) == false)
			{
				throw new UnsupportedFlavorException(f);
			}
			return img;
		}

		public virtual DataFlavor[] TransferDataFlavors
		{
			get
			{
				return new DataFlavor[] {DataFlavor.imageFlavor};
			}
		}

		public virtual bool isDataFlavorSupported(DataFlavor flavor)
		{
			return (flavor.Equals(DataFlavor.imageFlavor));
		}

	}

}
