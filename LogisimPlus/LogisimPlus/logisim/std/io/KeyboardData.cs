// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using Value = logisim.data.Value;
	using InstanceData = logisim.instance.InstanceData;

	internal class KeyboardData : InstanceData, ICloneable
	{
		private Value lastClock;
		private char[] buffer;
		private string str;
		private int bufferLength;
		private int cursorPos;
		private bool dispValid;
		private int dispStart;
		private int dispEnd;

		public KeyboardData(int capacity)
		{
			lastClock = Value.UNKNOWN;
			buffer = new char[capacity];
			clear();
		}
        public virtual object Clone()
        {
            KeyboardData ret = (KeyboardData)base.MemberwiseClone();
            ret.buffer = (char[])this.buffer.Clone();
            return ret;
        }

		public virtual Value setLastClock(Value newClock)
		{
			Value ret = lastClock;
			lastClock = newClock;
			return ret;
		}

		public virtual bool DisplayValid
		{
			get
			{
				return dispValid;
			}
		}

		public virtual int DisplayStart
		{
			get
			{
				return dispStart;
			}
		}

		public virtual int DisplayEnd
		{
			get
			{
				return dispEnd;
			}
		}

		public virtual int CursorPosition
		{
			get
			{
				return cursorPos;
			}
		}

		public virtual void updateBufferLength(int len)
		{
			lock (this)
			{
				char[] buf = buffer;
				int oldLen = buf.Length;
				if (oldLen != len)
				{
					char[] newBuf = new char[len];
					Array.Copy(buf, 0, newBuf, 0, Math.Min(len, oldLen));
					if (len < oldLen)
					{
						if (bufferLength > len)
						{
							bufferLength = len;
						}
						if (cursorPos > len)
						{
							cursorPos = len;
						}
					}
					buffer = newBuf;
					str = null;
					dispValid = false;
				}
			}
		}

		public override string ToString()
		{
			string s = str;
			if (!string.ReferenceEquals(s, null))
			{
				return s;
			}
			StringBuilder build = new StringBuilder();
			char[] buf = buffer;
			int len = bufferLength;
			for (int i = 0; i < len; i++)
			{
				char c = buf[i];
				build.Append(char.IsControl(c) ? ' ' : c);
			}
			str = build.ToString();
			return str;
		}

		public virtual char getChar(int pos)
		{
			return pos >= 0 && pos < bufferLength ? buffer[pos] : '\0';
		}

		public virtual int getNextSpecial(int pos)
		{
			char[] buf = buffer;
			int len = bufferLength;
			for (int i = pos; i < len; i++)
			{
				char c = buf[i];
				if (char.IsControl(c))
				{
					return i;
				}
			}
			return -1;
		}

		public virtual void clear()
		{
			bufferLength = 0;
			cursorPos = 0;
			str = "";
			dispValid = false;
			dispStart = 0;
			dispEnd = 0;
		}

		public virtual char dequeue()
		{
			char[] buf = buffer;
			int len = bufferLength;
			if (len == 0)
			{
				return '\0';
			}
			char ret = buf[0];
			for (int i = 1; i < len; i++)
			{
				buf[i - 1] = buf[i];
			}
			bufferLength = len - 1;
			int pos = cursorPos;
			if (pos > 0)
			{
				cursorPos = pos - 1;
			}
			str = null;
			dispValid = false;
			return ret;
		}

		public virtual bool insert(char value)
		{
			char[] buf = buffer;
			int len = bufferLength;
			if (len >= buf.Length)
			{
				return false;
			}
			int pos = cursorPos;
			for (int i = len; i > pos; i--)
			{
				buf[i] = buf[i - 1];
			}
			buf[pos] = value;
			bufferLength = len + 1;
			cursorPos = pos + 1;
			str = null;
			dispValid = false;
			return true;
		}

		public virtual bool delete()
		{
			char[] buf = buffer;
			int len = bufferLength;
			int pos = cursorPos;
			if (pos >= len)
			{
				return false;
			}
			for (int i = pos + 1; i < len; i++)
			{
				buf[i - 1] = buf[i];
			}
			bufferLength = len - 1;
			str = null;
			dispValid = false;
			return true;
		}

		public virtual bool moveCursorBy(int delta)
		{
			int len = bufferLength;
			int pos = cursorPos;
			int newPos = pos + delta;
			if (newPos < 0 || newPos > len)
			{
				return false;
			}
			cursorPos = newPos;
			dispValid = false;
			return true;
		}

		public virtual bool setCursor(int value)
		{
			int len = bufferLength;
			if (value > len)
			{
				value = len;
			}
			int pos = cursorPos;
			if (pos == value)
			{
				return false;
			}
			cursorPos = value;
			dispValid = false;
			return true;
		}

		public virtual void updateDisplay(FontMetrics fm)
		{
			if (dispValid)
			{
				return;
			}
			int pos = cursorPos;
			int i0 = dispStart;
			int i1 = dispEnd;
			string str = ToString();
			int len = str.Length;
			int max = Keyboard.WIDTH - 8 - 4;
			if (str.Equals("") || fm.stringWidth(str) <= max)
			{
				i0 = 0;
				i1 = len;
			}
			else
			{
				// grow to include end of string if possible
				int w0 = fm.stringWidth(str[0] + "m");
				int w1 = fm.stringWidth("m");
				int w = i0 == 0 ? fm.stringWidth(str) : w0 + fm.stringWidth(str.Substring(i0));
				if (w <= max)
				{
					i1 = len;
				}

				// rearrange start/end so as to include cursor
				if (pos <= i0)
				{
					if (pos < i0)
					{
						i1 += pos - i0;
						i0 = pos;
					}
					if (pos == i0 && i0 > 0)
					{
						i0--;
						i1--;
					}
				}
				if (pos >= i1)
				{
					if (pos > i1)
					{
						i0 += pos - i1;
						i1 = pos;
					}
					if (pos == i1 && i1 < len)
					{
						i0++;
						i1++;
					}
				}
				if (i0 <= 2)
				{
					i0 = 0;
				}

				// resize segment to fit
				if (fits(fm, str, w0, w1, i0, i1, max))
				{ // maybe should grow
					while (fits(fm, str, w0, w1, i0, i1 + 1, max))
					{
						i1++;
					}
					while (fits(fm, str, w0, w1, i0 - 1, i1, max))
					{
						i0--;
					}
				}
				else
				{ // should shrink
					if (pos < (i0 + i1) / 2)
					{
						i1--;
						while (!fits(fm, str, w0, w1, i0, i1, max))
						{
							i1--;
						}
					}
					else
					{
						i0++;
						while (!fits(fm, str, w0, w1, i0, i1, max))
						{
							i0++;
						}
					}

				}
				if (i0 == 1)
				{
					i0 = 0;
				}
			}
			dispStart = i0;
			dispEnd = i1;
			dispValid = true;
		}

		private bool fits(FontMetrics fm, string str, int w0, int w1, int i0, int i1, int max)
		{
			if (i0 >= i1)
			{
				return true;
			}
			int len = str.Length;
			if (i0 < 0 || i1 > len)
			{
				return false;
			}
			int w = fm.stringWidth(str.Substring(i0, i1 - i0));
			if (i0 > 0)
			{
				w += w0;
			}
			if (i1 < str.Length)
			{
				w += w1;
			}
			return w <= max;
		}
	}
}
