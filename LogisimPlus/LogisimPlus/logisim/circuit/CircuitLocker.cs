// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	internal class CircuitLocker
	{
		private static int NEXT_SERIAL_NUMBER = 0;

		private int serialNumber;
		private ReadWriteLock circuitLock;
		[NonSerialized]
		private Thread mutatingThread;
		private CircuitMutatorImpl mutatingMutator;

		internal CircuitLocker()
		{
            serialNumber = Interlocked.Increment(ref NEXT_SERIAL_NUMBER);
			circuitLock = new ReentrantReadWriteLock();
			mutatingThread = null;
			mutatingMutator = null;
		}

		public virtual bool hasWriteLock()
		{
			return mutatingThread == Thread.CurrentThread;
		}

		internal virtual CircuitMutatorImpl Mutator
		{
			get
			{
				return mutatingMutator;
			}
		}

		internal virtual void checkForWritePermission(string operationName)
		{
			if (mutatingThread != Thread.CurrentThread)
			{
				throw new InvalidOperationException(operationName + " outside transaction");
			}
		}

		internal virtual void execute(CircuitTransaction xn)
		{
			if (mutatingThread == Thread.CurrentThread)
			{
				xn.run(mutatingMutator);
			}
			else
			{
				xn.execute();
			}
		}

		private class CircuitComparator : IComparer<Circuit>
		{
			public virtual int Compare(Circuit a, Circuit b)
			{
				int an = a.Locker.serialNumber;
				int bn = b.Locker.serialNumber;
				return an - bn;
			}
		}

		internal static Dictionary<Circuit, Lock> acquireLocks(CircuitTransaction xn, CircuitMutatorImpl mutator)
		{
			Dictionary<Circuit, int> requests = xn.AccessedCircuits;
			Dictionary<Circuit, Lock> circuitLocks = new Dictionary<Circuit, Lock>();
			// Acquire locks in serial-number order to avoid deadlock
			Circuit[] lockOrder = requests.Keys.ToArray();
			Array.Sort(lockOrder, new CircuitComparator());
			try
			{
				foreach (Circuit circ in lockOrder)
				{
					int? access = requests[circ];
					CircuitLocker locker = circ.Locker;
					if (access.Value == CircuitTransaction.READ_ONLY)
					{
						Lock @lock = locker.circuitLock.readLock();
						@lock.@lock();
						circuitLocks[circ] = @lock;
					}
					else if (access.Value == CircuitTransaction.READ_WRITE)
					{
						Thread curThread = Thread.CurrentThread;
						if (locker.mutatingThread == curThread)
						{
							; // nothing to do - thread already has lock
						}
						else
						{
							Lock @lock = locker.circuitLock.writeLock();
							@lock.@lock();
							circuitLocks[circ] = @lock;
							locker.mutatingThread = Thread.CurrentThread;
							if (mutator == null)
							{
								mutator = new CircuitMutatorImpl();
							}
							locker.mutatingMutator = mutator;
						}
					}
				}
			}
			catch (Exception t)
			{
				releaseLocks(circuitLocks);
				throw t;
			}
			return circuitLocks;
		}

		internal static void releaseLocks(Dictionary<Circuit, Lock> locks)
		{
			Thread curThread = Thread.CurrentThread;
			foreach (KeyValuePair<Circuit, Lock> entry in locks.SetOfKeyValuePairs())
			{
				Circuit circ = entry.Key;
				Lock @lock = entry.Value;
				CircuitLocker locker = circ.Locker;
				if (locker.mutatingThread == curThread)
				{
					locker.mutatingThread = null;
					locker.mutatingMutator = null;
				}
				@lock.unlock();
			}
		}
	}

}
