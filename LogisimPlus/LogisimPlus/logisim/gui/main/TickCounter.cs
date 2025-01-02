// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{
	using Simulator = logisim.circuit.Simulator;
	using SimulatorEvent = logisim.circuit.SimulatorEvent;
	using SimulatorListener = logisim.circuit.SimulatorListener;

	internal class TickCounter : SimulatorListener
	{
		private const int QUEUE_LENGTH = 1000;

		private long[] queueTimes;
		private double[] queueRates;
		private int queueStart;
		private int queueSize;
		private double tickFrequency;

		public TickCounter()
		{
			queueTimes = new long[QUEUE_LENGTH];
			queueRates = new double[QUEUE_LENGTH];
			queueSize = 0;
		}

		public virtual void clear()
		{
			queueSize = 0;
		}

		public virtual void propagationCompleted(SimulatorEvent e)
		{
			Simulator sim = e.Source;
			if (!sim.Ticking)
			{
				queueSize = 0;
			}
		}

		public virtual void simulatorStateChanged(SimulatorEvent e)
		{
			propagationCompleted(e);
		}

		public virtual void tickCompleted(SimulatorEvent e)
		{
			Simulator sim = e.Source;
			if (!sim.Ticking)
			{
				queueSize = 0;
			}
			else
			{
				double freq = sim.TickFrequency;
				if (freq != tickFrequency)
				{
					queueSize = 0;
					tickFrequency = freq;
				}

				int curSize = queueSize;
				int maxSize = queueTimes.Length;
				int start = queueStart;
				int end;
				if (curSize < maxSize)
				{ // new sample is added into queue
					end = start + curSize;
					if (end >= maxSize)
					{
						end -= maxSize;
					}
					curSize++;
					queueSize = curSize;
				}
				else
				{ // new sample replaces oldest value in queue
					end = queueStart;
					if (end + 1 >= maxSize)
					{
						queueStart = 0;
					}
					else
					{
						queueStart = end + 1;
					}
				}
				long startTime = queueTimes[start];
				long endTime = DateTimeHelper.CurrentUnixTimeMillis();
				double rate;
				if (startTime == endTime || curSize <= 1)
				{
					rate = double.MaxValue;
				}
				else
				{
					rate = 1000.0 * (curSize - 1) / (endTime - startTime);
				}
				queueTimes[end] = endTime;
				queueRates[end] = rate;
			}
		}

		public virtual string TickRate
		{
			get
			{
				int size = queueSize;
				if (size <= 1)
				{
					return "";
				}
				else
				{
					int maxSize = queueTimes.Length;
					int start = queueStart;
					int end = start + size - 1;
					if (end >= maxSize)
					{
						end -= maxSize;
					}
					double rate = queueRates[end];
					if (rate <= 0 || rate == double.MaxValue)
					{
						return "";
					}
					else
					{
						// Figure out the minimum over the previous 100 readings, and
						// base our rounding off of that. This is meant to provide some
						// stability in the rounding - we don't want the result to
						// oscillate rapidly between 990 Hz and 1 KHz - it's better for
						// it to oscillate between 990 Hz and 1005 Hz.
						int baseLen = size;
						if (baseLen > 100)
						{
							baseLen = 100;
						}
						int baseStart = end - baseLen + 1;
						double min = rate;
						if (baseStart < 0)
						{
							baseStart += maxSize;
							for (int i = baseStart + maxSize; i < maxSize; i++)
							{
								double x = queueRates[i];
								if (x < min)
								{
									min = x;
								}
							}
							for (int i = 0; i < end; i++)
							{
								double x = queueRates[i];
								if (x < min)
								{
									min = x;
								}
							}
						}
						else
						{
							for (int i = baseStart; i < end; i++)
							{
								double x = queueRates[i];
								if (x < min)
								{
									min = x;
								}
							}
						}
						if (min < 0.9 * rate)
						{
							min = rate;
						}
    
						if (min >= 1000.0)
						{
							return Strings.get("tickRateKHz", roundString(rate / 1000.0, min / 1000.0));
						}
						else
						{
							return Strings.get("tickRateHz", roundString(rate, min));
						}
					}
				}
			}
		}

		private string roundString(double val, double min)
		{
			// round so we have only three significant digits
			int i = 0; // invariant: a = 10^i
			double a = 1.0; // invariant: a * bm == min, a is power of 10
			double bm = min;
			double bv = val;
			if (bm >= 1000)
			{
				while (bm >= 1000)
				{
					i++;
					a *= 10;
					bm /= 10;
					bv /= 10;
				}
			}
			else
			{
				while (bm < 100)
				{
					i--;
					a /= 10;
					bm *= 10;
					bv *= 10;
				}
			}

			// Examples:
			// 2.34: i = -2, a = .2, b = 234
			// 20.1: i = -1, a = .1, b = 201

			if (i >= 0)
			{ // nothing after decimal point
				return "" + (int) (long)Math.Round(a * (long)Math.Round(bv, MidpointRounding.AwayFromZero), MidpointRounding.AwayFromZero);
			}
			else
			{ // keep some after decimal point
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				return string.Format("%." + (-i) + "f", Convert.ToDouble(a * bv));
			}
		}
	}

}
