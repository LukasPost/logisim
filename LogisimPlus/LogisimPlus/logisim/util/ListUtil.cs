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

	public class ListUtil
	{
		private class JoinedList<E> : System.Collections.ObjectModel.Collection<E>
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<? extends E> a;
			internal IList<E> a;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<? extends E> b;
			internal IList<E> b;

// JAVA TO C# CONVERTER TASK: Wildcard generics in method parameters are not converted:
// ORIGINAL LINE: JoinedList(java.util.List<? extends E> a, java.util.List<? extends E> b)
			internal JoinedList(IList<E> a, IList<E> b)
			{
				this.a = a;
				this.b = b;
			}

			public override int size()
			{
				return a.Count + b.Count;
			}

			public override E get(int index)
			{
				if (index < a.Count)
				{
					return a[index];
				}
				else
				{
					return b[index - a.Count];
				}
			}

			public override IEnumerator<E> iterator()
			{
				return IteratorUtil.createJoinedIterator(a.GetEnumerator(), b.GetEnumerator());
			}

		}

		private ListUtil()
		{
		}

		public static IList<E> joinImmutableLists<E, T1, T2>(IList<T1> a, IList<T2> b) where T1 : E where T2 : E
		{
			return new JoinedList<E>(a, b);
		}
	}

}
