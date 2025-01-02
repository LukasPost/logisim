// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.IO;

/*
 * Copyright 2004-2005 The Apache Software Foundation.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */
namespace logisim.file
{

	/// <summary>
	/// Adapts a <code>Reader</code> as an <code>InputStream</code>. Adapted from <CODE>StringInputStream</CODE>.
	/// 
	/// </summary>
	public class ReaderInputStream : Stream
	{

		/// <summary>
		/// Source Reader </summary>
		private Reader @in;

		private string encoding = System.getProperty("file.encoding");

		private sbyte[] slack;

		private int begin;

		/// <summary>
		/// Construct a <CODE>ReaderInputStream</CODE> for the specified <CODE>Reader</CODE>.
		/// </summary>
		/// <param name="reader"> <CODE>Reader</CODE>. Must not be <code>null</code>. </param>
		public ReaderInputStream(Reader reader)
		{
			@in = reader;
		}

		/// <summary>
		/// Construct a <CODE>ReaderInputStream</CODE> for the specified <CODE>Reader</CODE>, with the specified encoding.
		/// </summary>
		/// <param name="reader">   non-null <CODE>Reader</CODE>. </param>
		/// <param name="encoding"> non-null <CODE>String</CODE> encoding. </param>
		public ReaderInputStream(Reader reader, string encoding) : this(reader)
		{
			if (string.ReferenceEquals(encoding, null))
			{
				throw new System.ArgumentException("encoding must not be null");
			}
			else
			{
				this.encoding = encoding;
			}
		}

		/// <summary>
		/// Reads from the <CODE>Reader</CODE>, returning the same value.
		/// </summary>
		/// <returns> the value of the next character in the <CODE>Reader</CODE>.
		/// </returns>
		/// <exception cref="IOException"> if the original <code>Reader</code> fails to be read </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public synchronized int read() throws java.io.IOException
		public override int read()
		{
			lock (this)
			{
				if (@in == null)
				{
					throw new IOException("Stream Closed");
				}
        
				sbyte result;
				if (slack != null && begin < slack.Length)
				{
					result = slack[begin];
					if (++begin == slack.Length)
					{
						slack = null;
					}
				}
				else
				{
					sbyte[] buf = new sbyte[1];
					if (Read(buf, 0, 1) <= 0)
					{
						result = -1;
					}
					result = buf[0];
				}
        
				if (result < -1)
				{
					result += unchecked((sbyte)256);
				}
        
				return result;
			}
		}

		/// <summary>
		/// Reads from the <code>Reader</code> into a byte array
		/// </summary>
		/// <param name="b">   the byte array to read into </param>
		/// <param name="off"> the offset in the byte array </param>
		/// <param name="len"> the length in the byte array to fill </param>
		/// <returns> the actual number read into the byte array, -1 at the end of the stream </returns>
		/// <exception cref="IOException"> if an error occurs </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public synchronized int read(byte[] b, int off, int len) throws java.io.IOException
		public override int read(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				if (@in == null)
				{
					throw new IOException("Stream Closed");
				}
        
				while (slack == null)
				{
					char[] buf = new char[len]; // might read too much
					int n = @in.read(buf);
					if (n == -1)
					{
						return -1;
					}
					if (n > 0)
					{
						slack = (new string(buf, 0, n)).GetBytes(encoding);
						begin = 0;
					}
				}
        
				if (len > slack.Length - begin)
				{
					len = slack.Length - begin;
				}
        
				Array.Copy(slack, begin, b, off, len);
        
				if ((begin += len) >= slack.Length)
				{
					slack = null;
				}
        
				return len;
			}
		}

		/// <summary>
		/// Marks the read limit of the StringReader.
		/// </summary>
		/// <param name="limit"> the maximum limit of bytes that can be read before the mark position becomes invalid </param>
		public override void mark(in int limit)
		{
			lock (this)
			{
				try
				{
					@in.mark(limit);
				}
				catch (IOException ioe)
				{
					throw new Exception(ioe.Message);
				}
			}
		}

		/// <returns> the current number of bytes ready for reading </returns>
		/// <exception cref="IOException"> if an error occurs </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public synchronized int available() throws java.io.IOException
		public override int available()
		{
			lock (this)
			{
				if (@in == null)
				{
					throw new IOException("Stream Closed");
				}
				if (slack != null)
				{
					return slack.Length - begin;
				}
				if (@in.ready())
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <returns> false - mark is not supported </returns>
		public override bool markSupported()
		{
			return false; // would be imprecise
		}

		/// <summary>
		/// Resets the StringReader.
		/// </summary>
		/// <exception cref="IOException"> if the StringReader fails to be reset </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public synchronized void reset() throws java.io.IOException
		public override void reset()
		{
			lock (this)
			{
				if (@in == null)
				{
					throw new IOException("Stream Closed");
				}
				slack = null;
				@in.reset();
			}
		}

		/// <summary>
		/// Closes the Stringreader.
		/// </summary>
		/// <exception cref="IOException"> if the original StringReader fails to be closed </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public synchronized void close() throws java.io.IOException
		public override void close()
		{
			lock (this)
			{
				if (@in != null)
				{
					@in.close();
					slack = null;
					@in = null;
				}
			}
		}
	}
}
