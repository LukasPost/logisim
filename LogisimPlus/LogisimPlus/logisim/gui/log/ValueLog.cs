// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{
	using Value = logisim.data.Value;

	internal class ValueLog
	{
		private const int LOG_SIZE = 400;

		private Value[] log;
		private short curSize;
		private short firstIndex;

		public ValueLog()
		{
			log = new Value[LOG_SIZE];
			curSize = 0;
			firstIndex = 0;
		}

		public virtual int size()
		{
			return curSize;
		}

		public virtual Value get(int index)
		{
			int i = firstIndex + index;
			if (i >= LOG_SIZE)
			{
				i -= LOG_SIZE;
			}
			return log[i];
		}

		public virtual Value Last
		{
			get
			{
				return curSize < LOG_SIZE ? (curSize == 0 ? null : log[curSize - 1]) : (firstIndex == 0 ? log[curSize - 1] : log[firstIndex - 1]);
			}
		}

		public virtual void append(Value val)
		{
			if (curSize < LOG_SIZE)
			{
				log[curSize] = val;
				curSize++;
			}
			else
			{
				log[firstIndex] = val;
				firstIndex++;
				if (firstIndex >= LOG_SIZE)
				{
					firstIndex = 0;
				}
			}
		}
	}

}
