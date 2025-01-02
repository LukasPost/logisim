// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.util
{

	using CanvasObject = draw.model.CanvasObject;

	public class MatchingSet<E> : AbstractSet<E> where E : draw.model.CanvasObject
	{
		private class Member<E> where E : draw.model.CanvasObject
		{
			internal E value;

			public Member(E value)
			{
				this.value = value;
			}

			public override bool Equals(object other)
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") Member<E> that = (Member<E>) other;
				Member<E> that = (Member<E>) other;
				return this.value.matches(that.value);
			}

			public override int GetHashCode()
			{
				return value.matchesHashCode();
			}
		}

		private class MatchIterator<E> : IEnumerator<E> where E : draw.model.CanvasObject
		{
			internal IEnumerator<Member<E>> it;

			internal MatchIterator(IEnumerator<Member<E>> it)
			{
				this.it = it;
			}

			public virtual bool hasNext()
			{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return it.hasNext();
			}

			public virtual E next()
			{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return it.next().value;
			}

			public virtual void remove()
			{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
				it.remove();
			}

		}

		private HashSet<Member<E>> set;

		public MatchingSet()
		{
			set = new HashSet<Member<E>>();
		}

		public MatchingSet(ICollection<E> initialContents)
		{
			set = new HashSet<Member<E>>(initialContents.Count);
			foreach (E value in initialContents)
			{
				set.Add(new Member<E>(value));
			}
		}

		public override bool add(E value)
		{
			return set.Add(new Member<E>(value));
		}

		public override bool remove(object value)
		{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") E eValue = (E) value;
			E eValue = (E) value;
			return set.Remove(new Member<E>(eValue));
		}

		public override bool contains(object value)
		{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") E eValue = (E) value;
			E eValue = (E) value;
			return set.Contains(new Member<E>(eValue));
		}

		public override IEnumerator<E> iterator()
		{
			return new MatchIterator<E>(set.GetEnumerator());
		}

		public override int size()
		{
			return set.Count;
		}

	}

}
