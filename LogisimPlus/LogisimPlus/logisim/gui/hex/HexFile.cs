// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.hex
{

	using HexModel = global::hex.HexModel;

	public class HexFile
	{
		private HexFile()
		{
		}

		private const string RAW_IMAGE_HEADER = "v2.0 raw";
		private const string COMMENT_MARKER = "#";

		private class HexReader
		{
			internal StreamReader @in;
			internal int[] data;
			internal StringTokenizer curLine;
			internal long leftCount;
			internal long leftValue;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public HexReader(java.io.BufferedReader in) throws java.io.IOException
			public HexReader(StreamReader @in)
			{
				this.@in = @in;
				data = new int[4096];
				curLine = findNonemptyLine();
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private java.util.StringTokenizer findNonemptyLine() throws java.io.IOException
			internal virtual StringTokenizer findNonemptyLine()
			{
				string line = @in.ReadLine();
				while (!string.ReferenceEquals(line, null))
				{
					int index = line.IndexOf(COMMENT_MARKER, StringComparison.Ordinal);
					if (index >= 0)
					{
						line = line.Substring(0, index);
					}

					StringTokenizer ret = new StringTokenizer(line);
					if (ret.hasMoreTokens())
					{
						return ret;
					}
					line = this.@in.ReadLine();
				}
				return null;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private String nextToken() throws java.io.IOException
			internal virtual string nextToken()
			{
				if (curLine != null && curLine.hasMoreTokens())
				{
					return curLine.nextToken();
				}
				else
				{
					curLine = findNonemptyLine();
					return curLine == null ? null : curLine.nextToken();
				}
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public boolean hasNext() throws java.io.IOException
			public virtual bool hasNext()
			{
				if (leftCount > 0)
				{
					return true;
				}
				else if (curLine != null && curLine.hasMoreTokens())
				{
					return true;
				}
				else
				{
					curLine = findNonemptyLine();
					return curLine != null;
				}
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public int[] next() throws java.io.IOException
			public virtual int[] next()
			{
				int pos = 0;
				if (leftCount > 0)
				{
					int n = (int) Math.Min(data.Length - pos, leftCount);
					if (n == 1)
					{
						data[pos] = (int) leftValue;
						pos++;
						leftCount--;
					}
					else
					{
						Arrays.Fill(data, pos, pos + n, (int) leftValue);
						pos += n;
						leftCount -= n;
					}
				}
				if (pos >= data.Length)
				{
					return data;
				}

				for (string tok = nextToken(); !string.ReferenceEquals(tok, null); tok = nextToken())
				{
					try
					{
						int star = tok.IndexOf("*", StringComparison.Ordinal);
						if (star < 0)
						{
							leftCount = 1;
							leftValue = Convert.ToInt64(tok, 16);
						}
						else
						{
							leftCount = long.Parse(tok.Substring(0, star));
							leftValue = Convert.ToInt64(tok.Substring(star + 1), 16);
						}
					}
					catch (System.FormatException)
					{
						throw new IOException(Strings.get("hexNumberFormatError"));
					}

					int n = (int) Math.Min(data.Length - pos, leftCount);
					if (n == 1)
					{
						data[pos] = (int) leftValue;
						pos++;
						leftCount--;
					}
					else
					{
						Arrays.Fill(data, pos, pos + n, (int) leftValue);
						pos += n;
						leftCount -= n;
					}
					if (pos >= data.Length)
					{
						return data;
					}
				}

				if (pos >= data.Length)
				{
					return data;
				}
				else
				{
					int[] ret = new int[pos];
					Array.Copy(data, 0, ret, 0, pos);
					return ret;
				}
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void save(java.io.Writer out, hex.HexModel src) throws java.io.IOException
		public static void save(Writer @out, HexModel src)
		{
			long first = src.FirstOffset;
			long last = src.LastOffset;
			while (last > first && src.get(last) == 0)
			{
				last--;
			}
			int tokens = 0;
			long cur = 0;
			while (cur <= last)
			{
				int val = src.get(cur);
				long start = cur;
				cur++;
				while (cur <= last && src.get(cur) == val)
				{
					cur++;
				}
				long len = cur - start;
				if (len < 4)
				{
					cur = start + 1;
					len = 1;
				}
				try
				{
					if (tokens > 0)
					{
						@out.write(tokens % 8 == 0 ? '\n' : ' ');
					}
					if (cur != start + 1)
					{
						@out.write((cur - start) + "*");
					}
					@out.write(Convert.ToString(val, 16));
				}
				catch (IOException)
				{
					throw new IOException(Strings.get("hexFileWriteError"));
				}
				tokens++;
			}
			if (tokens > 0)
			{
				@out.write('\n');
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void open(hex.HexModel dst, java.io.Reader in) throws java.io.IOException
		public static void open(HexModel dst, Reader @in)
		{
			HexReader reader = new HexReader(new StreamReader(@in));
			long offs = dst.FirstOffset;
			while (reader.hasNext())
			{
				int[] values = reader.next();
				if (offs + values.Length - 1 > dst.LastOffset)
				{
					throw new IOException(Strings.get("hexFileSizeError"));
				}
				dst.set(offs, values);
				offs += values.Length;
			}
			dst.fill(offs, dst.LastOffset - offs + 1, 0);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static int[] parse(java.io.Reader in) throws java.io.IOException
		public static int[] parse(Reader @in)
		{
			HexReader reader = new HexReader(new StreamReader(@in));
			int cur = 0;
			int[] data = new int[4096];
			while (reader.hasNext())
			{
				int[] values = reader.next();
				if (cur + values.Length > data.Length)
				{
					int[] oldData = data;
					data = new int[Math.Max(cur + values.Length, 3 * data.Length / 2)];
					Array.Copy(oldData, 0, data, 0, cur);
				}
				Array.Copy(values, 0, data, cur, values.Length);
				cur += values.Length;
			}
			if (cur != data.Length)
			{
				int[] oldData = data;
				data = new int[cur];
				Array.Copy(oldData, 0, data, 0, cur);
			}
			return data;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void open(hex.HexModel dst, java.io.File src) throws java.io.IOException
		public static void open(HexModel dst, File src)
		{
			StreamReader @in;
			try
			{
				@in = new StreamReader(src);
			}
			catch (IOException)
			{
				throw new IOException(Strings.get("hexFileOpenError"));
			}
			try
			{
				string header = @in.ReadLine();
				if (!header.Equals(RAW_IMAGE_HEADER))
				{
					throw new IOException(Strings.get("hexHeaderFormatError"));
				}
				open(dst, @in);
				try
				{
					StreamReader oldIn = @in;
					@in = null;
					oldIn.Close();
				}
				catch (IOException)
				{
					throw new IOException(Strings.get("hexFileReadError"));
				}
			}
			finally
			{
				try
				{
					if (@in != null)
					{
						@in.Close();
					}
				}
				catch (IOException)
				{
				}
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void save(java.io.File dst, hex.HexModel src) throws java.io.IOException
		public static void save(File dst, HexModel src)
		{
			StreamWriter @out;
			try
			{
				@out = new StreamWriter(dst);
			}
			catch (IOException)
			{
				throw new IOException(Strings.get("hexFileOpenError"));
			}
			try
			{
				try
				{
					@out.Write(RAW_IMAGE_HEADER + "\n");
				}
				catch (IOException)
				{
					throw new IOException(Strings.get("hexFileWriteError"));
				}
				save(@out, src);
			}
			finally
			{
				try
				{
					@out.Close();
				}
				catch (IOException)
				{
					throw new IOException(Strings.get("hexFileWriteError"));
				}
			}
		}
	}

}
