// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class SmallSet<E> : AbstractSet<E>
	{
		private const int HASH_POINT = 4;

		private class ArrayIterator : IEnumerator<E>
		{
			internal bool instanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				itVersion = outerInstance.version;
			}

			private readonly SmallSet<E> outerInstance;

			internal int itVersion;
			internal object myValues;
			internal int pos = 0; // position of next item to return
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
			internal bool hasNext_Conflict = true;
			internal bool removeOk = false;

			internal ArrayIterator(SmallSet<E> outerInstance)
			{
				this.outerInstance = outerInstance;

				if (!instanceFieldsInitialized)
				{
					InitializeInstanceFields();
					instanceFieldsInitialized = true;
				}
				myValues = outerInstance.values;
			}

			public virtual bool hasNext()
			{
				return hasNext_Conflict;
			}

			public virtual E next()
			{
				if (itVersion != outerInstance.version)
				{
					throw new ConcurrentModificationException();
				}
				else if (!hasNext_Conflict)
				{
					throw new NoSuchElementException();
				}
				else if (outerInstance.size_Conflict == 1)
				{
					pos = 1;
					hasNext_Conflict = false;
					removeOk = true;
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") E ret = (E) myValues;
					E ret = (E) myValues;
					return ret;
				}
				else
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") E ret = ((E[]) myValues)[pos];
					E ret = ((E[]) myValues)[pos];
					++pos;
					hasNext_Conflict = pos < outerInstance.size_Conflict;
					removeOk = true;
					return ret;
				}
			}

			public virtual void remove()
			{
				if (itVersion != outerInstance.version)
				{
					throw new ConcurrentModificationException();
				}
				else if (!removeOk)
				{
					throw new System.InvalidOperationException();
				}
				else if (outerInstance.size_Conflict == 1)
				{
					outerInstance.values = null;
					outerInstance.size_Conflict = 0;
					++outerInstance.version;
					itVersion = outerInstance.version;
					removeOk = false;
				}
				else
				{
					object[] vals = (object[]) outerInstance.values;
					if (outerInstance.size_Conflict == 2)
					{
						myValues = (pos == 2 ? vals[0] : vals[1]);
						outerInstance.values = myValues;
						outerInstance.size_Conflict = 1;
					}
					else
					{
						for (int i = pos; i < outerInstance.size_Conflict; i++)
						{
							vals[i - 1] = vals[i];
						}
						--pos;
						--outerInstance.size_Conflict;
						vals[outerInstance.size_Conflict] = null;
					}
					++outerInstance.version;
					itVersion = outerInstance.version;
					removeOk = false;
				}
			}
		}

// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private int size_Conflict = 0;
		private int version = 0;
		private object values = null;

		public SmallSet()
		{
		}

		public override SmallSet<E> clone()
		{
			SmallSet<E> ret = new SmallSet<E>();
			ret.size_Conflict = this.size_Conflict;
			if (size_Conflict == 1)
			{
				ret.values = this.values;
			}
			else if (size_Conflict <= HASH_POINT)
			{
				object[] oldVals = (object[]) this.values;
				object[] retVals = new object[size_Conflict];
				for (int i = size_Conflict - 1; i >= 0; i--)
				{
					retVals[i] = oldVals[i];
				}
			}
			else
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.HashSet<E> oldVals = (java.util.HashSet<E>) this.values;
				HashSet<E> oldVals = (HashSet<E>) this.values;
				values = oldVals.clone();
			}
			return ret;
		}

		public override object[] toArray()
		{
			object vals = values;
			int sz = size_Conflict;
			if (sz == 1)
			{
				return new object[] {vals};
			}
			else if (sz <= HASH_POINT)
			{
				object[] ret = new object[sz];
				Array.Copy(vals, 0, ret, 0, sz);
				return ret;
			}
			else
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.HashSet<?> hash = (java.util.HashSet<?>) vals;
				HashSet<object> hash = (HashSet<object>) vals;
				return hash.ToArray();
			}
		}

		public override void clear()
		{
			size_Conflict = 0;
			values = null;
			++version;
		}

		public override bool Empty
		{
			get
			{
				if (size_Conflict <= HASH_POINT)
				{
					return size_Conflict == 0;
				}
				else
				{
	// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	// ORIGINAL LINE: return ((java.util.HashSet<?>) values).isEmpty();
					return ((HashSet<object>) values).Count == 0;
				}
			}
		}

		public override int size()
		{
			if (size_Conflict <= HASH_POINT)
			{
				return size_Conflict;
			}
			else
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: return ((java.util.HashSet<?>) values).size();
				return ((HashSet<object>) values).Count;
			}
		}

		public override bool add(E value)
		{
			int oldSize = size_Conflict;
			object oldValues = values;
			int newVersion = version + 1;

			if (oldSize < 2)
			{
				if (oldSize == 0)
				{
					values = value;
					size_Conflict = 1;
					version = newVersion;
					return true;
				}
				else
				{
					object curValue = oldValues;
					if (curValue.Equals(value))
					{
						return false;
					}
					else
					{
						object[] newValues = new object[HASH_POINT];
						newValues[0] = values;
						newValues[1] = value;
						values = newValues;
						size_Conflict = 2;
						version = newVersion;
						return true;
					}
				}
			}
			else if (oldSize <= HASH_POINT)
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") E[] vals = (E[]) oldValues;
				E[] vals = (E[]) oldValues;
				for (int i = 0; i < oldSize; i++)
				{
					object val = vals[i];
					bool same = val == null ? value == null : val.Equals(value);
					if (same)
					{
						return false;
					}
				}
				if (oldSize < HASH_POINT)
				{
					vals[oldSize] = value;
					size_Conflict = oldSize + 1;
					version = newVersion;
					return true;
				}
				else
				{
					HashSet<E> newValues = new HashSet<E>();
					for (int i = 0; i < oldSize; i++)
					{
						newValues.Add(vals[i]);
					}
					newValues.Add(value);
					values = newValues;
					size_Conflict = oldSize + 1;
					version = newVersion;
					return true;
				}
			}
			else
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.HashSet<E> vals = (java.util.HashSet<E>) oldValues;
				HashSet<E> vals = (HashSet<E>) oldValues;
				if (vals.Add(value))
				{
					version = newVersion;
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public override bool contains(object value)
		{
			if (size_Conflict <= 2)
			{
				if (size_Conflict == 0)
				{
					return false;
				}
				else
				{
					return values.Equals(value);
				}
			}
			else if (size_Conflict <= HASH_POINT)
			{
				object[] vals = (object[]) values;
				for (int i = 0; i < size_Conflict; i++)
				{
					if (vals[i].Equals(value))
					{
						return true;
					}
				}
				return false;
			}
			else
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.HashSet<E> vals = (java.util.HashSet<E>) values;
				HashSet<E> vals = (HashSet<E>) values;
				return vals.Contains(value);
			}
		}

		public override IEnumerator<E> iterator()
		{
			if (size_Conflict <= HASH_POINT)
			{
				if (size_Conflict == 0)
				{
					return IteratorUtil.emptyIterator();
				}
				else
				{
					return new ArrayIterator(this);
				}
			}
			else
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.HashSet<E> set = (java.util.HashSet<E>) values;
				HashSet<E> set = (HashSet<E>) values;
				return set.GetEnumerator();
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void main(String[] args) throws java.io.IOException
		public static void Main(string[] args)
		{
			SmallSet<string> set = new SmallSet<string>();
			StreamReader @in = new StreamReader(System.in);
			while (true)
			{
				Console.Write(set.size() + ":"); // OK
				for (IEnumerator<string> it = set.GetEnumerator(); it.MoveNext();)
				{
					Console.Write(" " + it.Current); // OK
				}
				Console.WriteLine(); // OK
				Console.Write("> "); // OK
				string cmd = @in.ReadLine();
				if (string.ReferenceEquals(cmd, null))
				{
					break;
				}
				cmd = cmd.Trim();
				if (cmd.Equals(""))
				{
					;
				}
				else if (cmd.StartsWith("+", StringComparison.Ordinal))
				{
					set.add(cmd.Substring(1));
				}
				else if (cmd.StartsWith("-", StringComparison.Ordinal))
				{
					set.remove(cmd.Substring(1));
				}
				else if (cmd.StartsWith("?", StringComparison.Ordinal))
				{
					bool ret = set.contains(cmd.Substring(1));
					Console.WriteLine("  " + ret); // OK
				}
				else
				{
					Console.WriteLine("unrecognized command"); // OK
				}
			}
		}
	}

}
