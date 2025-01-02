// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

/*
 * @(#)GIFEncoder.java    0.90 4/21/96 Adam Doppelt
 */
namespace logisim.util
{

	/// <summary>
	/// GIFEncoder is a class which takes an image and saves it to a stream using the GIF file format
	/// (<A HREF="http://www.dcs.ed.ac.uk/%7Emxr/gfx/">Graphics Interchange Format</A>). A GIFEncoder is constructed with
	/// either an AWT Image (which must be fully loaded) or a set of RGB arrays. The image can be written out with a call to
	/// <CODE>Write</CODE>.
	/// <P>
	/// 
	/// Three caveats:
	/// <UL>
	/// <LI>GIFEncoder will convert the image to indexed color upon construction. This will take some time, depending on the
	/// size of the image. Also, actually writing the image out (Write) will take time.
	/// <P>
	/// 
	/// <LI>The image cannot have more than 256 colors, since GIF is an 8 bit format. For a 24 bit to 8 bit quantization
	/// algorithm, see Graphics Gems II III.2 by Xialoin Wu. Or check out his
	/// <A HREF="http://www.csd.uwo.ca/faculty/wu/cq.c">C source</A>.
	/// <P>
	/// 
	/// <LI>Since the image must be completely loaded into memory, GIFEncoder may have problems with large images. Attempting
	/// to encode an image which will not fit into memory will probably result in the following exception:
	/// <P>
	/// <CODE>java.awt.AWTException: Grabber returned false: 192</CODE>
	/// <P>
	/// </UL>
	/// <P>
	/// 
	/// GIFEncoder is based upon gifsave.c, which was written and released by:
	/// <P>
	/// <CENTER> Sverre H. Huseby<BR>
	/// Bjoelsengt. 17<BR>
	/// N-0468 Oslo<BR>
	/// Norway
	/// <P>
	/// 
	/// Phone: +47 2 230539<BR>
	/// sverrehu@ifi.uio.no
	/// <P>
	/// </CENTER>
	/// 
	/// @version 0.90 21 Apr 1996
	/// @author <A HREF="http://www.cs.brown.edu/people/amd/">Adam Doppelt</A>
	/// </summary>
	public class GifEncoder
	{
		private class BitFile
		{
			internal Stream output_;
			internal sbyte[] buffer_;
			internal int index_, bitsLeft_;

			internal BitFile(Stream output)
			{
				output_ = output;
				buffer_ = new sbyte[256];
				index_ = 0;
				bitsLeft_ = 8;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void Flush() throws java.io.IOException
			internal virtual void Flush()
			{
				int numBytes = index_ + (bitsLeft_ == 8 ? 0 : 1);
				if (numBytes > 0)
				{
					output_.WriteByte(numBytes);
					output_.Write(buffer_, 0, numBytes);
					buffer_[0] = 0;
					index_ = 0;
					bitsLeft_ = 8;
				}
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void WriteBits(int bits, int numbits) throws java.io.IOException
			internal virtual void WriteBits(int bits, int numbits)
			{
				int numBytes = 255;
				do
				{
					if ((index_ == 254 && bitsLeft_ == 0) || index_ > 254)
					{
						output_.WriteByte(numBytes);
						output_.Write(buffer_, 0, numBytes);

						buffer_[0] = 0;
						index_ = 0;
						bitsLeft_ = 8;
					}

					if (numbits <= bitsLeft_)
					{
						buffer_[index_] |= (sbyte)((bits & ((1 << numbits) - 1)) << (8 - bitsLeft_));
						bitsLeft_ -= numbits;
						numbits = 0;
					}
					else
					{
						buffer_[index_] |= (sbyte)((bits & ((1 << bitsLeft_) - 1)) << (8 - bitsLeft_));
						bits >>= bitsLeft_;
						numbits -= bitsLeft_;
						buffer_[++index_] = 0;
						bitsLeft_ = 8;
					}
				} while (numbits != 0);
			}
		}

		private class LZWStringTable
		{
			internal const int RES_CODES = 2;
			internal static readonly short HASH_FREE = unchecked((short) 0xFFFF);
			internal static readonly short NEXT_FIRST = unchecked((short) 0xFFFF);
			internal const int MAXBITS = 12;
			internal static readonly int MAXSTR = (1 << MAXBITS);
			internal const short HASHSIZE = 9973;
			internal const short HASHSTEP = 2039;

			internal sbyte[] strChr_;
			internal short[] strNxt_;
			internal short[] strHsh_;
			internal short numStrings_;

			internal LZWStringTable()
			{
				strChr_ = new sbyte[MAXSTR];
				strNxt_ = new short[MAXSTR];
				strHsh_ = new short[HASHSIZE];
			}

			internal virtual int AddCharString(short index, sbyte b)
			{
				int hshidx;

				if (numStrings_ >= MAXSTR)
				{
					return 0xFFFF;
				}

				hshidx = Hash(index, b);
				while (strHsh_[hshidx] != HASH_FREE)
				{
					hshidx = (hshidx + HASHSTEP) % HASHSIZE;
				}

				strHsh_[hshidx] = numStrings_;
				strChr_[numStrings_] = b;
				strNxt_[numStrings_] = (index != HASH_FREE) ? index : NEXT_FIRST;

				return numStrings_++;
			}

			internal virtual short FindCharString(short index, sbyte b)
			{
				int hshidx, nxtidx;

				if (index == HASH_FREE)
				{
					return b;
				}

				hshidx = Hash(index, b);
				while ((nxtidx = strHsh_[hshidx]) != HASH_FREE)
				{
					if (strNxt_[nxtidx] == index && strChr_[nxtidx] == b)
					{
						return (short) nxtidx;
					}
					hshidx = (hshidx + HASHSTEP) % HASHSIZE;
				}

				return unchecked((short) 0xFFFF);
			}

			internal virtual void ClearTable(int codesize)
			{
				numStrings_ = 0;

				for (int q = 0; q < HASHSIZE; q++)
				{
					strHsh_[q] = HASH_FREE;
				}

				int w = (1 << codesize) + RES_CODES;
				for (int q = 0; q < w; q++)
				{
					AddCharString(unchecked((short) 0xFFFF), (sbyte) q);
				}
			}

			internal static int Hash(short index, sbyte lastbyte)
			{
				return ((int)((short)(lastbyte << 8) ^ index) & 0xFFFF) % HASHSIZE;
			}
		}

		private class LZWCompressor
		{
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static void LZWCompress(java.io.OutputStream output, int codesize, byte toCompress[]) throws java.io.IOException
			internal static void LZWCompress(Stream output, int codesize, sbyte[] toCompress)
			{
				sbyte c;
				short index;
				int clearcode, endofinfo, numbits, limit;
				short prefix = unchecked((short) 0xFFFF);

				BitFile bitFile = new BitFile(output);
				LZWStringTable strings = new LZWStringTable();

				clearcode = 1 << codesize;
				endofinfo = clearcode + 1;

				numbits = codesize + 1;
				limit = (1 << numbits) - 1;

				strings.ClearTable(codesize);
				bitFile.WriteBits(clearcode, numbits);

				for (int loop = 0; loop < toCompress.Length; ++loop)
				{
					c = toCompress[loop];
					if ((index = strings.FindCharString(prefix, c)) != -1)
					{
						prefix = index;
					}
					else
					{
						bitFile.WriteBits(prefix, numbits);
						if (strings.AddCharString(prefix, c) > limit)
						{
							if (++numbits > 12)
							{
								bitFile.WriteBits(clearcode, numbits - 1);
								strings.ClearTable(codesize);
								numbits = codesize + 1;
							}
							limit = (1 << numbits) - 1;
						}

						prefix = (short)((short) c & 0xFF);
					}
				}

				if (prefix != -1)
				{
					bitFile.WriteBits(prefix, numbits);
				}

				bitFile.WriteBits(endofinfo, numbits);
				bitFile.Flush();
			}
		}

		private class ScreenDescriptor
		{
			internal short localScreenWidth_, localScreenHeight_;
			internal sbyte byte_;
			internal sbyte backgroundColorIndex_, pixelAspectRatio_;

			internal ScreenDescriptor(short width, short height, int numColors)
			{
				localScreenWidth_ = width;
				localScreenHeight_ = height;
				SetGlobalColorTableSize((sbyte)(BitUtils.BitsNeeded(numColors) - 1));
				SetGlobalColorTableFlag((sbyte) 1);
				SetSortFlag((sbyte) 0);
				SetColorResolution((sbyte) 7);
				backgroundColorIndex_ = 0;
				pixelAspectRatio_ = 0;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void Write(java.io.OutputStream output) throws java.io.IOException
			internal virtual void Write(Stream output)
			{
				BitUtils.WriteWord(output, localScreenWidth_);
				BitUtils.WriteWord(output, localScreenHeight_);
				output.WriteByte(byte_);
				output.WriteByte(backgroundColorIndex_);
				output.WriteByte(pixelAspectRatio_);
			}

			internal virtual void SetGlobalColorTableSize(sbyte num)
			{
				byte_ |= (sbyte)(num & 7);
			}

			internal virtual void SetSortFlag(sbyte num)
			{
				byte_ |= (sbyte)((num & 1) << 3);
			}

			internal virtual void SetColorResolution(sbyte num)
			{
				byte_ |= (sbyte)((num & 7) << 4);
			}

			internal virtual void SetGlobalColorTableFlag(sbyte num)
			{
				byte_ |= (sbyte)((num & 1) << 7);
			}
		}

		private class ImageDescriptor
		{
			internal sbyte separator_;
			internal short leftPosition_, topPosition_, width_, height_;
			internal sbyte byte_;

			internal ImageDescriptor(short width, short height, char separator)
			{
				separator_ = (sbyte) separator;
				leftPosition_ = 0;
				topPosition_ = 0;
				width_ = width;
				height_ = height;
				SetLocalColorTableSize((sbyte) 0);
				SetReserved((sbyte) 0);
				SetSortFlag((sbyte) 0);
				SetInterlaceFlag((sbyte) 0);
				SetLocalColorTableFlag((sbyte) 0);
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void Write(java.io.OutputStream output) throws java.io.IOException
			internal virtual void Write(Stream output)
			{
				output.WriteByte(separator_);
				BitUtils.WriteWord(output, leftPosition_);
				BitUtils.WriteWord(output, topPosition_);
				BitUtils.WriteWord(output, width_);
				BitUtils.WriteWord(output, height_);
				output.WriteByte(byte_);
			}

			internal virtual void SetLocalColorTableSize(sbyte num)
			{
				byte_ |= (sbyte)(num & 7);
			}

			internal virtual void SetReserved(sbyte num)
			{
				byte_ |= (sbyte)((num & 3) << 3);
			}

			internal virtual void SetSortFlag(sbyte num)
			{
				byte_ |= (sbyte)((num & 1) << 5);
			}

			internal virtual void SetInterlaceFlag(sbyte num)
			{
				byte_ |= (sbyte)((num & 1) << 6);
			}

			internal virtual void SetLocalColorTableFlag(sbyte num)
			{
				byte_ |= (sbyte)((num & 1) << 7);
			}
		}

		private class BitUtils
		{
			internal static sbyte BitsNeeded(int n)
			{
				sbyte ret = 1;

				if (n-- == 0)
				{
					return 0;
				}

				while ((n >>= 1) != 0)
				{
					++ret;
				}

				return ret;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static void WriteWord(java.io.OutputStream output, short w) throws java.io.IOException
			internal static void WriteWord(Stream output, short w)
			{
				output.WriteByte(w & 0xFF);
				output.WriteByte((w >> 8) & 0xFF);
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static void WriteString(java.io.OutputStream output, String string) throws java.io.IOException
			internal static void WriteString(Stream output, string @string)
			{
				for (int loop = 0; loop < @string.Length; ++loop)
				{
					output.WriteByte((sbyte)(@string[loop]));
				}
			}
		}

		private class MyGrabber : PixelGrabber
		{
			internal ProgressMonitor monitor;
			internal int progress;
			internal int goal;

			internal MyGrabber(ProgressMonitor monitor, Image image, int x, int y, int width, int height, int[] values, int start, int scan) : base(image, x, y, width, height, values, start, scan)
			{
				this.monitor = monitor;
				progress = 0;
				goal = width * height;
				monitor.setMinimum(0);
				monitor.setMaximum(goal * 21 / 20);
			}

			public override void setPixels(int srcX, int srcY, int srcW, int srcH, ColorModel model, int[] pixels, int srcOff, int srcScan)
			{
				progress += srcW * srcH;
				monitor.setProgress(progress);
				if (monitor.isCanceled())
				{
					abortGrabbing();
				}
				else
				{
					base.setPixels(srcX, srcY, srcW, srcH, model, pixels, srcOff, srcScan);
				}
			}
		}

		private short width_, height_;
		private int numColors_;
		private sbyte[] pixels_; private sbyte[] colors_;

		/// <summary>
		/// Construct a GIFEncoder. The constructor will convert the image to an indexed color array. <B>This may take some
		/// time.</B>
		/// <P>
		/// </summary>
		/// <param name="image"> The image to encode. The image <B>must</B> be completely loaded. </param>
		/// <exception cref="AWTException"> Will be thrown if the pixel grab fails. This can happen if Java runs out of memory. It
		///                         may also indicate that the image contains more than 256 colors. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public GifEncoder(java.awt.Image image, javax.swing.ProgressMonitor monitor) throws java.awt.AWTException
		public GifEncoder(Image image, ProgressMonitor monitor)
		{
			width_ = (short) image.getWidth(null);
			height_ = (short) image.getHeight(null);

			int[] values = new int[width_ * height_];
			PixelGrabber grabber;
			if (monitor != null)
			{
				grabber = new MyGrabber(monitor, image, 0, 0, width_, height_, values, 0, width_);
			}
			else
			{
				grabber = new PixelGrabber(image, 0, 0, width_, height_, values, 0, width_);
			}

			try
			{
				if (grabber.grabPixels() != true)
				{
					throw new AWTException(Strings.get("grabberError") + ": " + grabber.status());
				}
			}
			catch (InterruptedException)
			{
				;
			}

// JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
// ORIGINAL LINE: sbyte[][] r = new sbyte[width_][height_];
			sbyte[][] r = RectangularArrays.RectangularSbyteArray(width_, height_);
// JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
// ORIGINAL LINE: sbyte[][] g = new sbyte[width_][height_];
			sbyte[][] g = RectangularArrays.RectangularSbyteArray(width_, height_);
// JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
// ORIGINAL LINE: sbyte[][] b = new sbyte[width_][height_];
			sbyte[][] b = RectangularArrays.RectangularSbyteArray(width_, height_);
			int index = 0;
			for (int y = 0; y < height_; ++y)
			{
				for (int x = 0; x < width_; ++x)
				{
					r[x][y] = unchecked((sbyte)((values[index] >> 16) & 0xFF));
					g[x][y] = unchecked((sbyte)((values[index] >> 8) & 0xFF));
					b[x][y] = unchecked((sbyte)((values[index]) & 0xFF));
					++index;
				}
			}
			ToIndexedColor(r, g, b);
		}

		/// <summary>
		/// Construct a GifEncoder. The constructor will convert the image to an indexed color array. <B>This may take some
		/// time.</B>
		/// <P>
		/// 
		/// Each array stores intensity values for the image. In other words, r[x][y] refers to the red intensity of the
		/// pixel at column x, row y.
		/// <P>
		/// </summary>
		/// <param name="r"> An array containing the red intensity values. </param>
		/// <param name="g"> An array containing the green intensity values. </param>
		/// <param name="b"> An array containing the blue intensity values.
		/// </param>
		/// <exception cref="AWTException"> Will be thrown if the image contains more than 256 colors. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public GifEncoder(byte r[][], byte g[][], byte b[][]) throws java.awt.AWTException
		public GifEncoder(sbyte[][] r, sbyte[][] g, sbyte[][] b)
		{
			width_ = (short)(r.Length);
			height_ = (short)(r[0].Length);

			ToIndexedColor(r, g, b);
		}

		/// <summary>
		/// Writes the image out to a stream in the GIF file format. This will be a single GIF87a image, non-interlaced, with
		/// no background color. <B>This may take some time.</B>
		/// <P>
		/// </summary>
		/// <param name="output"> The stream to output to. This should probably be a buffered stream.
		/// </param>
		/// <exception cref="IOException"> Will be thrown if a write operation fails. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void write(java.io.OutputStream output) throws java.io.IOException
		public virtual void write(Stream output)
		{
			BitUtils.WriteString(output, "GIF87a");

			ScreenDescriptor sd = new ScreenDescriptor(width_, height_, numColors_);
			sd.Write(output);

			output.Write(colors_, 0, colors_.Length);

			ImageDescriptor id = new ImageDescriptor(width_, height_, ',');
			id.Write(output);

			sbyte codesize = BitUtils.BitsNeeded(numColors_);
			if (codesize == 1)
			{
				++codesize;
			}
			output.WriteByte(codesize);

			LZWCompressor.LZWCompress(output, codesize, pixels_);
			output.WriteByte(0);

			id = new ImageDescriptor((sbyte) 0, (sbyte) 0, ';');
			id.Write(output);
			output.Flush();
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void ToIndexedColor(byte r[][], byte g[][], byte b[][]) throws java.awt.AWTException
		internal virtual void ToIndexedColor(sbyte[][] r, sbyte[][] g, sbyte[][] b)
		{
			pixels_ = new sbyte[width_ * height_];
			colors_ = new sbyte[256 * 3];
			int colornum = 0;
			for (int x = 0; x < width_; ++x)
			{
				for (int y = 0; y < height_; ++y)
				{
					int search;
					for (search = 0; search < colornum; ++search)
					{
						if (colors_[search * 3] == r[x][y] && colors_[search * 3 + 1] == g[x][y] && colors_[search * 3 + 2] == b[x][y])
						{
							break;
						}
					}

					if (search > 255)
					{
						throw new AWTException(Strings.get("manyColorError"));
					}

					pixels_[y * width_ + x] = (sbyte) search;

					if (search == colornum)
					{
						colors_[search * 3] = r[x][y];
						colors_[search * 3 + 1] = g[x][y];
						colors_[search * 3 + 2] = b[x][y];
						++colornum;
					}
				}
			}
			numColors_ = 1 << BitUtils.BitsNeeded(colornum);
			sbyte[] copy = new sbyte[numColors_ * 3];
			Array.Copy(colors_, 0, copy, 0, numColors_ * 3);
			colors_ = copy;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void toFile(java.awt.Image img, String filename, javax.swing.ProgressMonitor monitor) throws IOException, java.awt.AWTException
		public static void toFile(Image img, string filename, ProgressMonitor monitor)
		{
			FileStream @out = new FileStream(filename, FileMode.Create, FileAccess.Write);
			(new GifEncoder(img, monitor)).write(@out);
			@out.Close();
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void toFile(java.awt.Image img, java.io.File file, javax.swing.ProgressMonitor monitor) throws IOException, java.awt.AWTException
		public static void toFile(Image img, File file, ProgressMonitor monitor)
		{
			FileStream @out = new FileStream(file, FileMode.Create, FileAccess.Write);
			(new GifEncoder(img, monitor)).write(@out);
			@out.Close();
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void toFile(java.awt.Image img, String filename) throws IOException, java.awt.AWTException
		public static void toFile(Image img, string filename)
		{
			toFile(img, filename, null);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void toFile(java.awt.Image img, java.io.File file) throws IOException, java.awt.AWTException
		public static void toFile(Image img, File file)
		{
			toFile(img, file, null);
		}

	}

}
