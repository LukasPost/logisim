// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class UnmodifiableList<E> : System.Collections.ObjectModel.Collection<E>
	{
		public static IList<E> create<E>(E[] data)
		{
			if (data.Length == 0)
			{
				return Collections.emptyList();
			}
			else
			{
				return new UnmodifiableList<E>(data);
			}
		}

		private E[] data;

		public UnmodifiableList(E[] data)
		{
			this.data = data;
		}

		public override E get(int index)
		{
			return data[index];
		}

		public override int size()
		{
			return data.Length;
		}
	}

}
