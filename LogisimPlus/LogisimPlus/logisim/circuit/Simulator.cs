// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using AppPreferences = logisim.prefs.AppPreferences;

	public class Simulator
	{
		/*
		 * begin DEBUGGING private static PrintWriter debug_log;
		 * 
		 * static { try { debug_log = new PrintWriter(new BufferedWriter(new FileWriter("DEBUG"))); } catch (IOException e)
		 * { System.err.println("Could not open debug log"); //OK } }
		 * 
		 * public static void log(String msg) { debug_log.println(msg); }
		 * 
		 * public static void flushLog() { debug_log.flush(); } //end DEBUGGING
		 */

		internal class PropagationManager : Thread
		{
			private readonly Simulator outerInstance;

			public PropagationManager(Simulator outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal Propagator propagator = null;
			internal PropagationPoints stepPoints = new PropagationPoints();
			internal volatile int ticksRequested = 0;
			internal volatile int stepsRequested = 0;
			internal volatile bool resetRequested = false;
			internal volatile bool propagateRequested = false;
			internal volatile bool complete = false;

			// These variables apply only if PRINT_TICK_RATE is set
			internal int tickRateTicks = 0;
			internal long tickRateStart = DateTimeHelper.CurrentUnixTimeMillis();

			public virtual Propagator Propagator
			{
				get
				{
					return propagator;
				}
				set
				{
					propagator = value;
				}
			}


			public virtual void requestPropagate()
			{
				lock (this)
				{
					if (!propagateRequested)
					{
						propagateRequested = true;
						Monitor.PulseAll(this);
					}
				}
			}

			public virtual void requestReset()
			{
				lock (this)
				{
					if (!resetRequested)
					{
						resetRequested = true;
						Monitor.PulseAll(this);
					}
				}
			}

			public virtual void requestTick()
			{
				lock (this)
				{
					if (ticksRequested < 16)
					{
						ticksRequested++;
					}
					Monitor.PulseAll(this);
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

			public override void run()
			{
				while (!complete)
				{
					lock (this)
					{
						while (!complete && !propagateRequested && !resetRequested && ticksRequested == 0 && stepsRequested == 0)
						{
							try
							{
								Monitor.Wait(this);
							}
							catch (InterruptedException)
							{
							}
						}
					}

					if (resetRequested)
					{
						resetRequested = false;
						if (propagator != null)
						{
							propagator.reset();
						}
						outerInstance.firePropagationCompleted();
						propagateRequested |= outerInstance.isRunning;
					}

					if (propagateRequested || ticksRequested > 0 || stepsRequested > 0)
					{
						bool ticked = false;
						propagateRequested = false;
						if (outerInstance.isRunning)
						{
							stepPoints.clear();
							stepsRequested = 0;
							if (propagator == null)
							{
								ticksRequested = 0;
							}
							else
							{
								ticked = ticksRequested > 0;
								if (ticked)
								{
									doTick();
								}
								do
								{
									propagateRequested = false;
									try
									{
										outerInstance.exceptionEncountered = false;
										propagator.propagate();
									}
									catch (Exception thr)
									{
										Console.WriteLine(thr.ToString());
										Console.Write(thr.StackTrace);
										outerInstance.exceptionEncountered = true;
										outerInstance.IsRunning = false;
									}
								} while (propagateRequested);
								if (outerInstance.Oscillating)
								{
									outerInstance.IsRunning = false;
									ticksRequested = 0;
									propagateRequested = false;
								}
							}
						}
						else
						{
							if (stepsRequested > 0)
							{
								if (ticksRequested > 0)
								{
									ticksRequested = 1;
									doTick();
								}

								lock (this)
								{
									stepsRequested--;
								}
								outerInstance.exceptionEncountered = false;
								try
								{
									stepPoints.clear();
									propagator.step(stepPoints);
								}
								catch (Exception thr)
								{
									Console.WriteLine(thr.ToString());
									Console.Write(thr.StackTrace);
									outerInstance.exceptionEncountered = true;
								}
							}
						}
						if (ticked)
						{
							outerInstance.fireTickCompleted();
						}
						outerInstance.firePropagationCompleted();
					}
				}
			}

			internal virtual void doTick()
			{
				lock (this)
				{
					ticksRequested--;
				}
				propagator.tick();
			}
		}

		private bool isRunning = true;
		private bool isTicking = false;
		private bool exceptionEncountered = false;
		private double tickFrequency = 1.0;

		private PropagationManager manager;
		private SimulatorTicker ticker;
		private List<SimulatorListener> listeners = new List<SimulatorListener>();

		public Simulator()
		{
			manager = new PropagationManager(this);
			ticker = new SimulatorTicker(manager);
			try
			{
				manager.setPriority(manager.getPriority() - 1);
				ticker.setPriority(ticker.getPriority() - 1);
			}
			catch (SecurityException)
			{
			}
			catch (System.ArgumentException)
			{
			}
			manager.Start();
			ticker.Start();

			tickFrequency = 0.0;
			TickFrequency = AppPreferences.TICK_FREQUENCY.get().doubleValue();
		}

		public virtual void shutDown()
		{
			ticker.shutDown();
			manager.shutDown();
		}

		public virtual CircuitState CircuitState
		{
			set
			{
				manager.Propagator = value.Propagator;
				renewTickerAwake();
			}
			get
			{
				Propagator prop = manager.Propagator;
				return prop == null ? null : prop.RootState;
			}
		}


		public virtual void requestReset()
		{
			manager.requestReset();
		}

		public virtual void tick()
		{
			ticker.tickOnce();
		}

		public virtual void step()
		{
			lock (manager)
			{
				manager.stepsRequested++;
				Monitor.PulseAll(manager);
			}
		}

		public virtual void drawStepPoints(ComponentDrawContext context)
		{
			manager.stepPoints.draw(context);
		}

		public virtual bool ExceptionEncountered
		{
			get
			{
				return exceptionEncountered;
			}
		}

		public virtual bool Running
		{
			get
			{
				return isRunning;
			}
		}

		public virtual bool IsRunning
		{
			set
			{
				if (isRunning != value)
				{
					isRunning = value;
					renewTickerAwake();
					/*
					 * DEBUGGING - comment out: if (!value) flushLog(); //
					 */
					fireSimulatorStateChanged();
				}
			}
		}

		public virtual bool Ticking
		{
			get
			{
				return isTicking;
			}
		}

		public virtual bool IsTicking
		{
			set
			{
				if (isTicking != value)
				{
					isTicking = value;
					renewTickerAwake();
					fireSimulatorStateChanged();
				}
			}
		}

		private void renewTickerAwake()
		{
			ticker.Awake = isRunning && isTicking && tickFrequency > 0;
		}

		public virtual double TickFrequency
		{
			get
			{
				return tickFrequency;
			}
			set
			{
				if (tickFrequency != value)
				{
					int millis = (int) (long)Math.Round(1000 / value, MidpointRounding.AwayFromZero);
					int ticks;
					if (millis > 0)
					{
						ticks = 1;
					}
					else
					{
						millis = 1;
						ticks = (int) (long)Math.Round(value / 1000, MidpointRounding.AwayFromZero);
					}
    
					tickFrequency = value;
					ticker.setTickFrequency(millis, ticks);
					renewTickerAwake();
					fireSimulatorStateChanged();
				}
			}
		}


		public virtual void requestPropagate()
		{
			manager.requestPropagate();
		}

		public virtual bool Oscillating
		{
			get
			{
				Propagator prop = manager.Propagator;
				return prop != null && prop.Oscillating;
			}
		}

		public virtual void addSimulatorListener(SimulatorListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeSimulatorListener(SimulatorListener l)
		{
			listeners.Remove(l);
		}

		internal virtual void firePropagationCompleted()
		{
			SimulatorEvent e = new SimulatorEvent(this);
			foreach (SimulatorListener l in new List<SimulatorListener>(listeners))
			{
				l.propagationCompleted(e);
			}
		}

		internal virtual void fireTickCompleted()
		{
			SimulatorEvent e = new SimulatorEvent(this);
			foreach (SimulatorListener l in new List<SimulatorListener>(listeners))
			{
				l.tickCompleted(e);
			}
		}

		internal virtual void fireSimulatorStateChanged()
		{
			SimulatorEvent e = new SimulatorEvent(this);
			foreach (SimulatorListener l in new List<SimulatorListener>(listeners))
			{
				l.simulatorStateChanged(e);
			}
		}
	}

}
