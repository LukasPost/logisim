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

	public class ArraySet<E> : AbstractSet<E>
	{
		private static readonly object[] EMPTY_ARRAY = new object[0];

		private class ArrayIterator : IEnumerator<E>
		{
			internal bool instanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				itVersion = outerInstance.version;
				hasNext_Conflict = outerInstance.values.Length > 0;
			}

			private readonly ArraySet<E> outerInstance;

			public ArrayIterator(ArraySet<E> outerInstance)
			{
				this.outerInstance = outerInstance;

				if (!instanceFieldsInitialized)
				{
					InitializeInstanceFields();
					instanceFieldsInitialized = true;
				}
			}

			internal int itVersion;
			internal int pos = 0; // position of next item to return
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
			internal bool hasNext_Conflict;
			internal bool removeOk = false;

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
				else
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") E ret = (E) values[pos];
					E ret = (E) outerInstance.values[pos];
					++pos;
					hasNext_Conflict = pos < outerInstance.values.Length;
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
				else if (outerInstance.values.Length == 1)
				{
					outerInstance.values = EMPTY_ARRAY;
					++outerInstance.version;
					itVersion = outerInstance.version;
					removeOk = false;
				}
				else
				{
					object[] newValues = new object[outerInstance.values.Length - 1];
					if (pos > 1)
					{
						Array.Copy(outerInstance.values, 0, newValues, 0, pos - 1);
					}
					if (pos < outerInstance.values.Length)
					{
						Array.Copy(outerInstance.values, pos, newValues, pos - 1, outerInstance.values.Length - pos);
					}
					outerInstance.values = newValues;
					--pos;
					++outerInstance.version;
					itVersion = outerInstance.version;
					removeOk = false;
				}
			}
		}

		private int version = 0;
		private object[] values = EMPTY_ARRAY;

		public ArraySet()
		{
		}

		public override object[] toArray()
		{
			return values;
		}

		public override object clone()
		{
			ArraySet<E> ret = new ArraySet<E>();
			if (this.values == EMPTY_ARRAY)
			{
				ret.values = EMPTY_ARRAY;
			}
			else
			{
				ret.values = (object[])this.values.Clone();
			}
			return ret;
		}

		public override void clear()
		{
			values = EMPTY_ARRAY;
			++version;
		}

		public override bool Empty
		{
			get
			{
				return values.Length == 0;
			}
		}

		public override int size()
		{
			return values.Length;
		}

		public override bool add(object value)
		{
			int n = values.Length;
			for (int i = 0; i < n; i++)
			{
				if (values[i].Equals(value))
				{
					return false;
				}
			}

			object[] newValues = new object[n + 1];
			Array.Copy(values, 0, newValues, 0, n);
			newValues[n] = value;
			values = newValues;
			++version;
			return true;
		}

		public override bool contains(object value)
		{
			for (int i = 0, n = values.Length; i < n; i++)
			{
				if (values[i].Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		public override IEnumerator<E> iterator()
		{
			return new ArrayIterator(this);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void main(String[] args) throws java.io.IOException
		public static void Main(string[] args)
		{
			ArraySet<string> set = new ArraySet<string>();
			StreamReader @in = new StreamReader(System.in);
			while (true)
			{
				Console.Write(set.size() + ":"); // OK
				foreach (string str in set)
				{
					Console.Write(" " + str); // OK
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
