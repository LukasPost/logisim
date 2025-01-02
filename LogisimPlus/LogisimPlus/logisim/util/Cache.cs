// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{
	/// <summary>
	/// Allows immutable objects to be cached in memory in order to reduce the creation of duplicate objects.
	/// </summary>
	public class Cache
	{
		private int mask;
		private object[] data;

		public Cache() : this(8)
		{
		}

		public Cache(int logSize)
		{
			if (logSize > 12)
			{
				logSize = 12;
			}

			data = new object[1 << logSize];
			mask = data.Length - 1;
		}

		public virtual object get(int hashCode)
		{
			return data[hashCode & mask];
		}

		public virtual void put(int hashCode, object value)
		{
			if (value != null)
			{
				data[hashCode & mask] = value;
			}
		}

		public virtual object get(object value)
		{
			if (value == null)
			{
				return null;
			}
			int code = value.GetHashCode() & mask;
			object ret = data[code];
			if (ret != null && ret.Equals(value))
			{
				return ret;
			}
			else
			{
				data[code] = value;
				return value;
			}
		}
	}

}
