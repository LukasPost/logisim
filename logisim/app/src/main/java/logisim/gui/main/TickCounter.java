/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import logisim.circuit.Simulator;
import logisim.circuit.SimulatorEvent;
import logisim.circuit.SimulatorListener;

class TickCounter implements SimulatorListener {
	private static final int QUEUE_LENGTH = 1000;

	private long[] queueTimes;
	private double[] queueRates;
	private int queueStart;
	private int queueSize;
	private double tickFrequency;

	public TickCounter() {
		queueTimes = new long[QUEUE_LENGTH];
		queueRates = new double[QUEUE_LENGTH];
		queueSize = 0;
	}

	public void clear() {
		queueSize = 0;
	}

	public void propagationCompleted(SimulatorEvent e) {
		Simulator sim = e.getSource();
		if (!sim.isTicking()) queueSize = 0;
	}

	public void simulatorStateChanged(SimulatorEvent e) {
		propagationCompleted(e);
	}

	public void tickCompleted(SimulatorEvent e) {
		Simulator sim = e.getSource();
		if (!sim.isTicking()) queueSize = 0;
		else {
			double freq = sim.getTickFrequency();
			if (freq != tickFrequency) {
				queueSize = 0;
				tickFrequency = freq;
			}

			int curSize = queueSize;
			int maxSize = queueTimes.length;
			int start = queueStart;
			int end;
			if (curSize < maxSize) { // new sample is added into queue
				end = start + curSize;
				if (end >= maxSize) end -= maxSize;
				curSize++;
				queueSize = curSize;
			} else { // new sample replaces oldest value in queue
				end = queueStart;
				if (end + 1 >= maxSize) queueStart = 0;
				else queueStart = end + 1;
			}
			long startTime = queueTimes[start];
			long endTime = System.currentTimeMillis();
			double rate;
			if (startTime == endTime || curSize <= 1) rate = Double.MAX_VALUE;
			else rate = 1000.0 * (curSize - 1) / (endTime - startTime);
			queueTimes[end] = endTime;
			queueRates[end] = rate;
		}
	}

	public String getTickRate() {
		int size = queueSize;
		if (size <= 1) return "";
		else {
			int maxSize = queueTimes.length;
			int start = queueStart;
			int end = start + size - 1;
			if (end >= maxSize) end -= maxSize;
			double rate = queueRates[end];
			if (rate <= 0 || rate == Double.MAX_VALUE) return "";
			else {
				// Figure out the minimum over the previous 100 readings, and
				// base our rounding off of that. This is meant to provide some
				// stability in the rounding - we don't want the result to
				// oscillate rapidly between 990 Hz and 1 KHz - it's better for
				// it to oscillate between 990 Hz and 1005 Hz.
				int baseLen = size;
				if (baseLen > 100)
					baseLen = 100;
				int baseStart = end - baseLen + 1;
				double min = rate;
				if (baseStart < 0) {
					baseStart += maxSize;
					for (int i = baseStart + maxSize; i < maxSize; i++) {
						double x = queueRates[i];
						if (x < min)
							min = x;
					}
					for (int i = 0; i < end; i++) {
						double x = queueRates[i];
						if (x < min)
							min = x;
					}
				} else for (int i = baseStart; i < end; i++) {
					double x = queueRates[i];
					if (x < min)
						min = x;
				}
				if (min < 0.9 * rate)
					min = rate;

				if (min >= 1000.0) return Strings.get("tickRateKHz", roundString(rate / 1000.0, min / 1000.0));
				else return Strings.get("tickRateHz", roundString(rate, min));
			}
		}
	}

	private String roundString(double val, double min) {
		// round so we have only three significant digits
		int i = 0; // invariant: a = 10^i
		double a = 1.0; // invariant: a * bm == min, a is power of 10
		double bm = min;
		double bv = val;
		if (bm >= 1000) while (bm >= 1000) {
			i++;
			a *= 10;
			bm /= 10;
			bv /= 10;
		}
		else while (bm < 100) {
			i--;
			a /= 10;
			bm *= 10;
			bv *= 10;
		}

		// Examples:
		// 2.34: i = -2, a = .2, b = 234
		// 20.1: i = -1, a = .1, b = 201

		// keep some after decimal point
		// nothing after decimal point
		if (i >= 0) return "" + (int) Math.round(a * Math.round(bv));
		else return String.format("%." + (-i) + "f", a * bv);
	}
}
