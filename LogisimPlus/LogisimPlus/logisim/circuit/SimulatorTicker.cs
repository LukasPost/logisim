// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
	internal class SimulatorTicker : Thread
	{
		private Simulator.PropagationManager manager;
		private int ticksPerTickPhase;
		private int millisPerTickPhase;

		private bool shouldTick;
		private int ticksPending;
		private bool complete;

		public SimulatorTicker(Simulator.PropagationManager manager)
		{
			this.manager = manager;
			ticksPerTickPhase = 1;
			millisPerTickPhase = 1000;
			shouldTick = false;
			ticksPending = 0;
			complete = false;
		}

		public virtual void setTickFrequency(int millis, int ticks)
		{
			lock (this)
			{
				millisPerTickPhase = millis;
				ticksPerTickPhase = ticks;
			}
		}

		internal virtual bool Awake
		{
			set
			{
				lock (this)
				{
					shouldTick = value;
					if (shouldTick)
					{
						Monitor.PulseAll(this);
					}
				}
			}
		}

		public virtual void shutDown()
		{
			lock (this)
			{
				complete = true;
				Monitor.PulseAll(this);
			}
		}

		public virtual void tickOnce()
		{
			lock (this)
			{
				ticksPending++;
				Monitor.PulseAll(this);
			}
		}

		public override void run()
		{
			long lastTick = DateTimeHelper.CurrentUnixTimeMillis();
			while (true)
			{
				bool curShouldTick = shouldTick;
				int millis = millisPerTickPhase;
				int ticks = ticksPerTickPhase;
				try
				{
					lock (this)
					{
						curShouldTick = shouldTick;
						millis = millisPerTickPhase;
						ticks = ticksPerTickPhase;
						while (!curShouldTick && ticksPending == 0 && !complete)
						{
							Monitor.Wait(this);
							curShouldTick = shouldTick;
							millis = millisPerTickPhase;
							ticks = ticksPerTickPhase;
						}
					}
				}
				catch (InterruptedException)
				{
				}

				if (complete)
				{
					break;
				}

				int toTick;
				long now = DateTimeHelper.CurrentUnixTimeMillis();
				if (curShouldTick && now - lastTick >= millis)
				{
					toTick = ticks;
				}
				else
				{
					toTick = ticksPending;
				}

				if (toTick > 0)
				{
					lastTick = now;
					for (int i = 0; i < toTick; i++)
					{
						manager.requestTick();
					}
					lock (this)
					{
						if (ticksPending > toTick)
						{
							ticksPending -= toTick;
						}
						else
						{
							ticksPending = 0;
						}
					}
					// we fire tickCompleted in this thread so that other
					// objects (in particular the repaint process) can slow
					// the thread down.
				}

				try
				{
					long nextTick = lastTick + millis;
					int wait = (int)(nextTick - DateTimeHelper.CurrentUnixTimeMillis());
					if (wait < 1)
					{
						wait = 1;
					}
					if (wait > 100)
					{
						wait = 100;
					}
					Thread.Sleep(wait);
				}
				catch (InterruptedException)
				{
				}
			}
		}
	}

}
