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

package com.bric.swing;

import org.jetbrains.annotations.NotNull;

import java.awt.Image;
import java.awt.datatransfer.DataFlavor;
import java.awt.datatransfer.Transferable;
import java.awt.datatransfer.UnsupportedFlavorException;

class ImageTransferable implements Transferable {
	Image img;

	public ImageTransferable(Image i) {
		img = i;
	}

	public @NotNull Object getTransferData(DataFlavor f) throws UnsupportedFlavorException {
		if (!f.equals(DataFlavor.imageFlavor))
			throw new UnsupportedFlavorException(f);
		return img;
	}

	public DataFlavor[] getTransferDataFlavors() {
		return new DataFlavor[] { DataFlavor.imageFlavor };
	}

	public boolean isDataFlavorSupported(DataFlavor flavor) {
		return (flavor.equals(DataFlavor.imageFlavor));
	}

}
