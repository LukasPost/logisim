// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{

	public class ReorderRequest
	{
		public static readonly IComparer<ReorderRequest> ASCENDING_FROM = new Compare(true, true);
		public static readonly IComparer<ReorderRequest> DESCENDING_FROM = new Compare(true, true);
		public static readonly IComparer<ReorderRequest> ASCENDING_TO = new Compare(true, true);
		public static readonly IComparer<ReorderRequest> DESCENDING_TO = new Compare(true, true);

		private class Compare : IComparer<ReorderRequest>
		{
			internal bool onFrom;
			internal bool asc;

			internal Compare(bool onFrom, bool asc)
			{
				this.onFrom = onFrom;
				this.asc = asc;
			}

			public virtual int Compare(ReorderRequest a, ReorderRequest b)
			{
				int i = onFrom ? a.fromIndex : a.toIndex;
				int j = onFrom ? b.fromIndex : b.toIndex;
				if (i < j)
				{
					return asc ? -1 : 1;
				}
				else if (i > j)
				{
					return asc ? 1 : -1;
				}
				else
				{
					return 0;
				}
			}
		}

		private CanvasObject @object;
		private int fromIndex;
		private int toIndex;

		public ReorderRequest(CanvasObject @object, int from, int to)
		{
			this.@object = @object;
			this.fromIndex = from;
			this.toIndex = to;
		}

		public virtual CanvasObject Object
		{
			get
			{
				return @object;
			}
		}

		public virtual int FromIndex
		{
			get
			{
				return fromIndex;
			}
		}

		public virtual int ToIndex
		{
			get
			{
				return toIndex;
			}
		}
	}

}
