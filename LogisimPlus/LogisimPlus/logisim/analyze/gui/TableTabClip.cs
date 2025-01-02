// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{

	using Entry = logisim.analyze.model.Entry;
	using TruthTable = logisim.analyze.model.TruthTable;

	internal class TableTabClip : ClipboardOwner
	{
		private static readonly DataFlavor binaryFlavor = new DataFlavor(typeof(Data), "Binary data");

		[Serializable]
		private class Data : Transferable
		{
			internal string[] headers;
			internal string[][] contents;

			internal Data(string[] headers, string[][] contents)
			{
				this.headers = headers;
				this.contents = contents;
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
					return this;
				}
				else if (flavor == DataFlavor.stringFlavor)
				{
					StringBuilder buf = new StringBuilder();
					for (int i = 0; i < headers.Length; i++)
					{
						buf.Append(headers[i]);
						buf.Append(i == headers.Length - 1 ? '\n' : '\t');
					}
					for (int i = 0; i < contents.Length; i++)
					{
						for (int j = 0; j < contents[i].Length; j++)
						{
							buf.Append(contents[i][j]);
							buf.Append(j == contents[i].Length - 1 ? '\n' : '\t');
						}
					}
					return buf.ToString();
				}
				else
				{
					throw new UnsupportedFlavorException(flavor);
				}
			}
		}

		private TableTab table;

		internal TableTabClip(TableTab table)
		{
			this.table = table;
		}

		public virtual void copy()
		{
			TableTabCaret caret = table.Caret;
			int c0 = caret.CursorCol;
			int r0 = caret.CursorRow;
			int c1 = caret.MarkCol;
			int r1 = caret.MarkRow;
			if (c1 < c0)
			{
				int t = c0;
				c0 = c1;
				c1 = t;
			}
			if (r1 < r0)
			{
				int t = r0;
				r0 = r1;
				r1 = t;
			}

			TruthTable t = table.TruthTable;
			int inputs = t.InputColumnCount;
			string[] header = new string[c1 - c0 + 1];
			for (int c = c0; c <= c1; c++)
			{
				if (c < inputs)
				{
					header[c - c0] = t.getInputHeader(c);
				}
				else
				{
					header[c - c0] = t.getOutputHeader(c - inputs);
				}
			}
// JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
// ORIGINAL LINE: string[][] contents = new string[r1 - r0 + 1][c1 - c0 + 1];
			string[][] contents = RectangularArrays.RectangularStringArray(r1 - r0 + 1, c1 - c0 + 1);
			for (int r = r0; r <= r1; r++)
			{
				for (int c = c0; c <= c1; c++)
				{
					if (c < inputs)
					{
						contents[r - r0][c - c0] = t.getInputEntry(r, c).Description;
					}
					else
					{
						contents[r - r0][c - c0] = t.getOutputEntry(r, c - inputs).Description;
					}
				}
			}

			Clipboard clip = table.getToolkit().getSystemClipboard();
			clip.setContents(new Data(header, contents), this);
		}

		public virtual bool canPaste()
		{
			Clipboard clip = table.getToolkit().getSystemClipboard();
			Transferable xfer = clip.getContents(this);
			return xfer.isDataFlavorSupported(binaryFlavor);
		}

		public virtual void paste()
		{
			Clipboard clip = table.getToolkit().getSystemClipboard();
			Transferable xfer;
			try
			{
				xfer = clip.getContents(this);
			}
			catch (Exception)
			{
				// I don't know - the above was observed to throw an odd ArrayIndexOutOfBounds
				// exception on a Linux computer using Sun's Java 5 JVM
				JOptionPane.showMessageDialog(table.getRootPane(), Strings.get("clipPasteSupportedError"), Strings.get("clipPasteErrorTitle"), JOptionPane.ERROR_MESSAGE);
				return;
			}
			Entry[][] entries;
			if (xfer.isDataFlavorSupported(binaryFlavor))
			{
				try
				{
					Data data = (Data) xfer.getTransferData(binaryFlavor);
					entries = new Entry[data.contents.Length][];
					for (int i = 0; i < entries.Length; i++)
					{
						Entry[] row = new Entry[data.contents[i].Length];
						for (int j = 0; j < row.Length; j++)
						{
							row[j] = Entry.parse(data.contents[i][j]);
						}
						entries[i] = row;
					}
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
				try
				{
					string buf = (string) xfer.getTransferData(DataFlavor.stringFlavor);
					StringTokenizer lines = new StringTokenizer(buf, "\r\n");
					string first;
					if (!lines.hasMoreTokens())
					{
						return;
					}
					first = lines.nextToken();
					StringTokenizer toks = new StringTokenizer(first, "\t,");
					string[] headers = new string[toks.countTokens()];
					Entry[] firstEntries = new Entry[headers.Length];
					bool allParsed = true;
					for (int i = 0; toks.hasMoreTokens(); i++)
					{
						headers[i] = toks.nextToken();
						firstEntries[i] = Entry.parse(headers[i]);
						allParsed = allParsed && firstEntries[i] != null;
					}
					int rows = lines.countTokens();
					if (allParsed)
					{
						rows++;
					}
					entries = new Entry[rows][];
					int cur = 0;
					if (allParsed)
					{
						entries[0] = firstEntries;
						cur++;
					}
					while (lines.hasMoreTokens())
					{
						toks = new StringTokenizer(lines.nextToken(), "\t");
						Entry[] ents = new Entry[toks.countTokens()];
						for (int i = 0; toks.hasMoreTokens(); i++)
						{
							ents[i] = Entry.parse(toks.nextToken());
						}
						entries[cur] = ents;
						cur++;
					}
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
			else
			{
				JOptionPane.showMessageDialog(table.getRootPane(), Strings.get("clipPasteSupportedError"), Strings.get("clipPasteErrorTitle"), JOptionPane.ERROR_MESSAGE);
				return;
			}

			TableTabCaret caret = table.Caret;
			int c0 = caret.CursorCol;
			int c1 = caret.MarkCol;
			int r0 = caret.CursorRow;
			int r1 = caret.MarkRow;
			if (r0 < 0 || r1 < 0 || c0 < 0 || c1 < 0)
			{
				return;
			}
			TruthTable model = table.TruthTable;
			int rows = model.RowCount;
			int inputs = model.InputColumnCount;
			int outputs = model.OutputColumnCount;
			if (c0 == c1 && r0 == r1)
			{
				if (r0 + entries.Length > rows || c0 + entries[0].Length > inputs + outputs)
				{
					JOptionPane.showMessageDialog(table.getRootPane(), Strings.get("clipPasteEndError"), Strings.get("clipPasteErrorTitle"), JOptionPane.ERROR_MESSAGE);
					return;
				}
			}
			else
			{
				if (r0 > r1)
				{
					int t = r0;
					r0 = r1;
					r1 = t;
				}
				if (c0 > c1)
				{
					int t = c0;
					c0 = c1;
					c1 = t;
				}

				if (r1 - r0 + 1 != entries.Length || c1 - c0 + 1 != entries[0].Length)
				{
					JOptionPane.showMessageDialog(table.getRootPane(), Strings.get("clipPasteSizeError"), Strings.get("clipPasteErrorTitle"), JOptionPane.ERROR_MESSAGE);
					return;
				}
			}
			for (int r = 0; r < entries.Length; r++)
			{
				for (int c = 0; c < entries[0].Length; c++)
				{
					if (c0 + c >= inputs)
					{
						model.setOutputEntry(r0 + r, c0 + c - inputs, entries[r][c]);
					}
				}
			}
		}

		public virtual void lostOwnership(Clipboard clip, Transferable transfer)
		{
		}

	}

}
