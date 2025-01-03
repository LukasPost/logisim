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

	public class CollectionUtil
	{
		private class UnionSet<E> : AbstractSet<E>
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.Set<? extends E> a;
			internal HashSet<E> a;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.Set<? extends E> b;
			internal HashSet<E> b;

// JAVA TO C# CONVERTER TASK: Wildcard generics in method parameters are not converted:
// ORIGINAL LINE: UnionSet(java.util.Set<? extends E> a, java.util.Set<? extends E> b)
			internal UnionSet(HashSet<E> a, HashSet<E> b)
			{
				this.a = a;
				this.b = b;
			}

			public override int size()
			{
				return a.Count + b.Count;
			}

			public override IEnumerator<E> iterator()
			{
				return IteratorUtil.createJoinedIterator(a.GetEnumerator(), b.GetEnumerator());
			}
		}

		private class UnionList<E> : System.Collections.ObjectModel.Collection<E>
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<? extends E> a;
			internal List<E> a;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<? extends E> b;
			internal List<E> b;

// JAVA TO C# CONVERTER TASK: Wildcard generics in method parameters are not converted:
// ORIGINAL LINE: UnionList(java.util.List<? extends E> a, java.util.List<? extends E> b)
			internal UnionList(List<E> a, List<E> b)
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
				E ret;
				if (index < a.Count)
				{
					ret = a[index];
				}
				else
				{
					ret = a[index - a.Count];
				}
				return ret;
			}
		}

		private CollectionUtil()
		{
		}

		public static HashSet<E> createUnmodifiableSetUnion<E, T1, T2>(HashSet<T1> a, HashSet<T2> b) where T1 : E where T2 : E
		{
			return new UnionSet<E>(a, b);
		}

		public static List<E> createUnmodifiableListUnion<E, T1, T2>(List<T1> a, List<T2> b) where T1 : E where T2 : E
		{
			return new UnionList<E>(a, b);
		}
	}

}
