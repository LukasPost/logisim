// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{

	public class Implicant : IComparable<Implicant>
	{
		internal static Implicant MINIMAL_IMPLICANT = new Implicant(0, -1);
		internal static IList<Implicant> MINIMAL_LIST = new List<Implicant> {MINIMAL_IMPLICANT};

		private class TermIterator : IEnumerable<Implicant>, IEnumerator<Implicant>
		{
			internal Implicant source;
			internal int currentMask = 0;

			internal TermIterator(Implicant source)
			{
				this.source = source;
			}

			public virtual IEnumerator<Implicant> GetEnumerator()
			{
				return this;
			}

			public virtual bool hasNext()
			{
				return currentMask >= 0;
			}

			public virtual Implicant next()
			{
				int ret = currentMask | source.values;
				int diffs = currentMask ^ source.unknowns;
				int diff = diffs ^ ((diffs - 1) & diffs);
				if (diff == 0)
				{
					currentMask = -1;
				}
				else
				{
					currentMask = (currentMask & ~(diff - 1)) | diff;
				}
				return new Implicant(0, ret);
			}

			public virtual void remove()
			{
			}
		}

		private int unknowns;
		private int values;

		private Implicant(int unknowns, int values)
		{
			this.unknowns = unknowns;
			this.values = values;
		}

		public override bool Equals(object other)
		{
			if (!(other is Implicant))
			{
				return false;
			}
			Implicant o = (Implicant) other;
			return this.unknowns == o.unknowns && this.values == o.values;
		}

		public virtual int CompareTo(Implicant o)
		{
			if (this.values < o.values)
			{
				return -1;
			}
			if (this.values > o.values)
			{
				return 1;
			}
			if (this.unknowns < o.unknowns)
			{
				return -1;
			}
			if (this.unknowns > o.unknowns)
			{
				return 1;
			}
			return 0;
		}

		public override int GetHashCode()
		{
			return (unknowns << 16) | values;
		}

		public virtual int UnknownCount
		{
			get
			{
				int ret = 0;
				int n = unknowns;
				while (n != 0)
				{
					n &= (n - 1);
					ret++;
				}
				return ret;
			}
		}

		public virtual IEnumerable<Implicant> Terms
		{
			get
			{
				return new TermIterator(this);
			}
		}

		public virtual int Row
		{
			get
			{
				if (unknowns != 0)
				{
					return -1;
				}
				return values;
			}
		}

		private Expression toProduct(TruthTable source)
		{
			Expression term = null;
			int cols = source.InputColumnCount;
			for (int i = cols - 1; i >= 0; i--)
			{
				if ((unknowns & (1 << i)) == 0)
				{
					Expression literal = Expressions.variable(source.getInputHeader(cols - 1 - i));
					if ((values & (1 << i)) == 0)
					{
						literal = Expressions.not(literal);
					}
					term = Expressions.and(term, literal);
				}
			}
			return term == null ? Expressions.constant(1) : term;
		}

		private Expression toSum(TruthTable source)
		{
			Expression term = null;
			int cols = source.InputColumnCount;
			for (int i = cols - 1; i >= 0; i--)
			{
				if ((unknowns & (1 << i)) == 0)
				{
					Expression literal = Expressions.variable(source.getInputHeader(cols - 1 - i));
					if ((values & (1 << i)) != 0)
					{
						literal = Expressions.not(literal);
					}
					term = Expressions.or(term, literal);
				}
			}
			return term == null ? Expressions.constant(1) : term;
		}

		internal static Expression toExpression(int format, AnalyzerModel model, IList<Implicant> implicants)
		{
			if (implicants == null)
			{
				return null;
			}
			TruthTable table = model.TruthTable;
			if (format == AnalyzerModel.FORMAT_PRODUCT_OF_SUMS)
			{
				Expression product = null;
				foreach (Implicant imp in implicants)
				{
					product = Expressions.and(product, imp.toSum(table));
				}
				return product == null ? Expressions.constant(1) : product;
			}
			else
			{
				Expression sum = null;
				foreach (Implicant imp in implicants)
				{
					sum = Expressions.or(sum, imp.toProduct(table));
				}
				return sum == null ? Expressions.constant(0) : sum;
			}
		}

		internal static IList<Implicant> computeMinimal(int format, AnalyzerModel model, string variable)
		{
			TruthTable table = model.TruthTable;
			int column = model.Outputs.IndexOf(variable);
			if (column < 0)
			{
				return Collections.emptyList();
			}

			Entry desired = format == AnalyzerModel.FORMAT_SUM_OF_PRODUCTS ? Entry.ONE : Entry.ZERO;
			Entry undesired = desired == Entry.ONE ? Entry.ZERO : Entry.ONE;

			// determine the first-cut implicants, as well as the rows
			// that we need to cover.
			Dictionary<Implicant, Entry> @base = new Dictionary<Implicant, Entry>();
			HashSet<Implicant> toCover = new HashSet<Implicant>();
			bool knownFound = false;
			for (int i = 0; i < table.RowCount; i++)
			{
				Entry entry = table.getOutputEntry(i, column);
				if (entry == undesired)
				{
					knownFound = true;
				}
				else if (entry == desired)
				{
					knownFound = true;
					Implicant imp = new Implicant(0, i);
					@base[imp] = entry;
					toCover.Add(imp);
				}
				else
				{
					Implicant imp = new Implicant(0, i);
					@base[imp] = entry;
				}
			}
			if (!knownFound)
			{
				return null;
			}

			// work up to more general implicants, discovering
			// any prime implicants.
			HashSet<Implicant> primes = new HashSet<Implicant>();
			Dictionary<Implicant, Entry> current = @base;
			while (current.Count > 1)
			{
				HashSet<Implicant> toRemove = new HashSet<Implicant>();
				Dictionary<Implicant, Entry> next = new Dictionary<Implicant, Entry>();
				foreach (KeyValuePair<Implicant, Entry> curEntry in current.SetOfKeyValuePairs())
				{
					Implicant imp = curEntry.Key;
					Entry detEntry = curEntry.Value;
					for (int j = 1; j <= imp.values; j *= 2)
					{
						if ((imp.values & j) != 0)
						{
							Implicant opp = new Implicant(imp.unknowns, imp.values ^ j);
							Entry oppEntry = current[opp];
							if (oppEntry != null)
							{
								toRemove.Add(imp);
								toRemove.Add(opp);
								Implicant i = new Implicant(opp.unknowns | j, opp.values);
								Entry e;
								if (oppEntry == Entry.DONT_CARE && detEntry == Entry.DONT_CARE)
								{
									e = Entry.DONT_CARE;
								}
								else
								{
									e = desired;
								}
								next[i] = e;
							}
						}
					}
				}

				foreach (KeyValuePair<Implicant, Entry> curEntry in current.SetOfKeyValuePairs())
				{
					Implicant det = curEntry.Key;
					if (!toRemove.Contains(det) && curEntry.Value == desired)
					{
						primes.Add(det);
					}
				}

				current = next;
			}

			// we won't have more than one implicant left, but it
			// is probably prime.
			foreach (KeyValuePair<Implicant, Entry> curEntry in current.SetOfKeyValuePairs())
			{
				Implicant imp = curEntry.Key;
				if (current[imp] == desired)
				{
					primes.Add(imp);
				}
			}

			// determine the essential prime implicants
			HashSet<Implicant> retSet = new HashSet<Implicant>();
			HashSet<Implicant> covered = new HashSet<Implicant>();
			foreach (Implicant required in toCover)
			{
				if (covered.Contains(required))
				{
					continue;
				}
				int row = required.Row;
				Implicant essential = null;
				foreach (Implicant imp in primes)
				{
					if ((row & ~imp.unknowns) == imp.values)
					{
						if (essential == null)
						{
							essential = imp;
						}
						else
						{
							essential = null;
							break;
						}
					}
				}
				if (essential != null)
				{
					retSet.Add(essential);
					primes.Remove(essential);
					foreach (Implicant imp in essential.Terms)
					{
						covered.Add(imp);
					}
				}
			}
			toCover.RemoveAll(covered);

			// This is an unusual case, but it's possible that the
			// essential prime implicants don't cover everything.
			// In that case, greedily pick out prime implicants
			// that cover the most uncovered rows.
			while (toCover.Count > 0)
			{
				// find the implicant covering the most rows
				Implicant max = null;
				int maxCount = 0;
				int maxUnknowns = int.MaxValue;
				for (IEnumerator<Implicant> it = primes.GetEnumerator(); it.MoveNext();)
				{
					Implicant imp = it.Current;
					int count = 0;
					foreach (Implicant term in imp.Terms)
					{
						if (toCover.Contains(term))
						{
							++count;
						}
					}
					if (count == 0)
					{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
						it.remove();
					}
					else if (count > maxCount)
					{
						max = imp;
						maxCount = count;
						maxUnknowns = imp.UnknownCount;
					}
					else if (count == maxCount)
					{
						int unk = imp.UnknownCount;
						if (unk > maxUnknowns)
						{
							max = imp;
							maxUnknowns = unk;
						}
					}
				}

				// add it to our choice, and remove the covered rows
				if (max != null)
				{
					retSet.Add(max);
					primes.Remove(max);
					foreach (Implicant term in max.Terms)
					{
						toCover.Remove(term);
					}
				}
			}

			// Now build up our sum-of-products expression
			// from the remaining terms
			List<Implicant> ret = new List<Implicant>(retSet);
			ret.Sort();
			return ret;
		}
	}

}
