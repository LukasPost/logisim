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

	public class IteratorUtil
	{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public static java.util.Iterator<?> EMPTY_ITERATOR = new EmptyIterator<Object>();
		public static IEnumerator<object> EMPTY_ITERATOR = new EmptyIterator<object>();

		public static IEnumerator<E> emptyIterator<E>()
		{
			return new EmptyIterator<E>();
		}

		private class EmptyIterator<E> : IEnumerator<E>
		{
			internal EmptyIterator()
			{
			}

			public virtual E next()
			{
				throw new NoSuchElementException();
			}

			public virtual bool hasNext()
			{
				return false;
			}

			public virtual void remove()
			{
				throw new System.NotSupportedException("EmptyIterator.remove");
			}
		}

		private class UnitIterator<E> : IEnumerator<E>
		{
			internal E data;
			internal bool taken = false;

			internal UnitIterator(E data)
			{
				this.data = data;
			}

			public virtual E next()
			{
				if (taken)
				{
					throw new NoSuchElementException();
				}
				taken = true;
				return data;
			}

			public virtual bool hasNext()
			{
				return !taken;
			}

			public virtual void remove()
			{
				throw new System.NotSupportedException("UnitIterator.remove");
			}
		}

		private class ArrayIterator<E> : IEnumerator<E>
		{
			internal E[] data;
			internal int i = -1;

			internal ArrayIterator(E[] data)
			{
				this.data = data;
			}

			public virtual E next()
			{
				if (!hasNext())
				{
					throw new NoSuchElementException();
				}
				i++;
				return data[i];
			}

			public virtual bool hasNext()
			{
				return i + 1 < data.Length;
			}

			public virtual void remove()
			{
				throw new System.NotSupportedException("ArrayIterator.remove");
			}
		}

		private class IteratorUnion<E> : IEnumerator<E>
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Iterator<? extends E> cur;
			internal IEnumerator<E> cur;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Iterator<? extends E> next;
			internal IEnumerator<E> next_Conflict;

// JAVA TO C# CONVERTER TASK: Wildcard generics in method parameters are not converted:
// ORIGINAL LINE: private IteratorUnion(java.util.Iterator<? extends E> cur, java.util.Iterator<? extends E> next)
			internal IteratorUnion(IEnumerator<E> cur, IEnumerator<E> next)
			{
				this.cur = cur;
				this.next_Conflict = next;
			}

			public virtual E next()
			{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (!cur.hasNext())
				{
					if (next_Conflict == null)
					{
						throw new NoSuchElementException();
					}
					cur = next_Conflict;
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (!cur.hasNext())
					{
						throw new NoSuchElementException();
					}
				}
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return cur.next();
			}

			public virtual bool hasNext()
			{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return cur.hasNext() || (next_Conflict != null && next_Conflict.hasNext());
			}

			public virtual void remove()
			{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
				cur.remove();
			}
		}

		public static IEnumerator<E> createUnitIterator<E>(E data)
		{
			return new UnitIterator<E>(data);
		}

		public static IEnumerator<E> createArrayIterator<E>(E[] data)
		{
			return new ArrayIterator<E>(data);
		}

		public static IEnumerator<E> createJoinedIterator<E, T1, T2>(IEnumerator<T1> i0, IEnumerator<T2> i1) where T1 : E where T2 : E
		{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (!i0.hasNext())
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Iterator<E> ret = (java.util.Iterator<E>) i1;
				IEnumerator<E> ret = (IEnumerator<E>) i1;
				return ret;
			}
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			else if (!i1.hasNext())
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Iterator<E> ret = (java.util.Iterator<E>) i0;
				IEnumerator<E> ret = (IEnumerator<E>) i0;
				return ret;
			}
			else
			{
				return new IteratorUnion<E>(i0, i1);
			}
		}

	}

}
