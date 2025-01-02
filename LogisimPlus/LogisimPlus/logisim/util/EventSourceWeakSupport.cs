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

	public class EventSourceWeakSupport<L> : IEnumerable<L>
	{
		private ConcurrentLinkedQueue<WeakReference<L>> listeners = new ConcurrentLinkedQueue<WeakReference<L>>();

		public EventSourceWeakSupport()
		{
		}

		public virtual void add(L listener)
		{
			listeners.add(new WeakReference<L>(listener));
		}

		public virtual void remove(L listener)
		{
			for (IEnumerator<WeakReference<L>> it = listeners.GetEnumerator(); it.MoveNext();)
			{
				L l = it.Current.get();
				if (l == null || l == listener)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
			}
		}

		public virtual bool Empty
		{
			get
			{
				for (IEnumerator<WeakReference<L>> it = listeners.GetEnumerator(); it.MoveNext();)
				{
					L l = it.Current.get();
					if (l == null)
					{
	// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
						it.remove();
					}
					else
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual IEnumerator<L> GetEnumerator()
		{
			// copy elements into another list in case any event handlers
			// want to add a listener
			List<L> ret = new List<L>(listeners.size());
			for (IEnumerator<WeakReference<L>> it = listeners.GetEnumerator(); it.MoveNext();)
			{
				L l = it.Current.get();
				if (l == null)
				{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
				}
				else
				{
					ret.Add(l);
				}
			}
			return ret.GetEnumerator();
		}
	}

}
