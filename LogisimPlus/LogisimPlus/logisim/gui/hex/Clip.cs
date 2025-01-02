// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.hex
{

	using Caret = global::hex.Caret;
	using HexEditor = global::hex.HexEditor;
	using HexModel = global::hex.HexModel;

	internal class Clip : ClipboardOwner
	{
		private static readonly DataFlavor binaryFlavor = new DataFlavor(typeof(int[]), "Binary data");

		private class Data : Transferable
		{
			internal int[] data;

			internal Data(int[] data)
			{
				this.data = data;
			}

			public virtual DataFlavor[] TransferDataFlavors
			{
				get
				{
					return new DataFlavor[] {binaryFlavor, DataFlavor.stringFlavor};
				}
			}

			public virtual bool isDataFlavorSupported(DataFlavor flavor)
			{
				return flavor == binaryFlavor || flavor == DataFlavor.stringFlavor;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Object getTransferData(java.awt.datatransfer.DataFlavor flavor) throws UnsupportedFlavorException, java.io.IOException
			public virtual object getTransferData(DataFlavor flavor)
			{
				if (flavor == binaryFlavor)
				{
					return data;
				}
				else if (flavor == DataFlavor.stringFlavor)
				{
					int bits = 1;
					for (int i = 0; i < data.Length; i++)
					{
						int k = data[i] >> bits;
						while (k != 0 && bits < 32)
						{
							bits++;
							k >>= 1;
						}
					}

					int chars = (bits + 3) / 4;
					StringBuilder buf = new StringBuilder();
					for (int i = 0; i < data.Length; i++)
					{
						if (i > 0)
						{
							buf.Append(i % 8 == 0 ? '\n' : ' ');
						}
						string s = Convert.ToString(data[i], 16);
						while (s.Length < chars)
						{
							s = "0" + s;
						}
						buf.Append(s);
					}
					return buf.ToString();
				}
				else
				{
					throw new UnsupportedFlavorException(flavor);
				}
			}
		}

		private HexEditor editor;

		internal Clip(HexEditor editor)
		{
			this.editor = editor;
		}

		public virtual void copy()
		{
			Caret caret = editor.Caret;
			long p0 = caret.Mark;
			long p1 = caret.Dot;
			if (p0 < 0 || p1 < 0)
			{
				return;
			}
			if (p0 > p1)
			{
				long t = p0;
				p0 = p1;
				p1 = t;
			}
			p1++;

			int[] data = new int[(int)(p1 - p0)];
			HexModel model = editor.Model;
			for (long i = p0; i < p1; i++)
			{
				data[(int)(i - p0)] = model.get(i);
			}

			Clipboard clip = editor.getToolkit().getSystemClipboard();
			clip.setContents(new Data(data), this);
		}

		public virtual bool canPaste()
		{
			Clipboard clip = editor.getToolkit().getSystemClipboard();
			Transferable xfer = clip.getContents(this);
			return xfer.isDataFlavorSupported(binaryFlavor);
		}

		public virtual void paste()
		{
			Clipboard clip = editor.getToolkit().getSystemClipboard();
			Transferable xfer = clip.getContents(this);
			int[] data;
			if (xfer.isDataFlavorSupported(binaryFlavor))
			{
				try
				{
					data = (int[]) xfer.getTransferData(binaryFlavor);
				}
				catch (UnsupportedFlavorException)
				{
					return;
				}
				catch (IOException)
				{
					return;
				}
			}
			else if (xfer.isDataFlavorSupported(DataFlavor.stringFlavor))
			{
				string buf;
				try
				{
					buf = (string) xfer.getTransferData(DataFlavor.stringFlavor);
				}
				catch (UnsupportedFlavorException)
				{
					return;
				}
				catch (IOException)
				{
					return;
				}

				try
				{
					data = HexFile.parse(new StringReader(buf));
				}
				catch (IOException e)
				{
					JOptionPane.showMessageDialog(editor.getRootPane(), e.Message, Strings.get("hexPasteErrorTitle"), JOptionPane.ERROR_MESSAGE);
					return;
				}
			}
			else
			{
				JOptionPane.showMessageDialog(editor.getRootPane(), Strings.get("hexPasteSupportedError"), Strings.get("hexPasteErrorTitle"), JOptionPane.ERROR_MESSAGE);
				return;
			}

			Caret caret = editor.Caret;
			long p0 = caret.Mark;
			long p1 = caret.Dot;
			if (p0 == p1)
			{
				HexModel model = editor.Model;
				if (p0 + data.Length - 1 <= model.LastOffset)
				{
					model.set(p0, data);
				}
				else
				{
					JOptionPane.showMessageDialog(editor.getRootPane(), Strings.get("hexPasteEndError"), Strings.get("hexPasteErrorTitle"), JOptionPane.ERROR_MESSAGE);
				}
			}
			else
			{
				if (p0 < 0 || p1 < 0)
				{
					return;
				}
				if (p0 > p1)
				{
					long t = p0;
					p0 = p1;
					p1 = t;
				}
				p1++;

				HexModel model = editor.Model;
				if (p1 - p0 == data.Length)
				{
					model.set(p0, data);
				}
				else
				{
					JOptionPane.showMessageDialog(editor.getRootPane(), Strings.get("hexPasteSizeError"), Strings.get("hexPasteErrorTitle"), JOptionPane.ERROR_MESSAGE);
				}
			}
		}

		public virtual void lostOwnership(Clipboard clip, Transferable transfer)
		{
		}

	}

}
