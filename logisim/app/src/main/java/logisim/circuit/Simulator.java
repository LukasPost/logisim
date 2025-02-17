/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.util.ArrayList;

import logisim.comp.ComponentDrawContext;
import logisim.prefs.AppPreferences;

public class Simulator {
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

	class PropagationManager extends Thread {
		private Propagator propagator;
		private PropagationPoints stepPoints = new PropagationPoints();
		private volatile int ticksRequested;
		private volatile int stepsRequested;
		private volatile boolean resetRequested;
		private volatile boolean propagateRequested;
		private volatile boolean complete;

		public Propagator getPropagator() {
			return propagator;
		}

		public void setPropagator(Propagator value) {
			propagator = value;
		}

		public synchronized void requestPropagate() {
			if (!propagateRequested) {
				propagateRequested = true;
				notifyAll();
			}
		}

		public synchronized void requestReset() {
			if (!resetRequested) {
				resetRequested = true;
				notifyAll();
			}
		}

		public synchronized void requestTick() {
			if (ticksRequested < 16) ticksRequested++;
			notifyAll();
		}

		public synchronized void shutDown() {
			complete = true;
			notifyAll();
		}

		@Override
		public void run() {
			while (!complete) {
				synchronized (this) {
					while (!complete && !propagateRequested && !resetRequested && ticksRequested == 0
							&& stepsRequested == 0) try {
						wait();
					} catch (InterruptedException e) {
					}
				}

				if (resetRequested) {
					resetRequested = false;
					if (propagator != null)
						propagator.reset();
					firePropagationCompleted();
					synchronized (this) {
						propagateRequested |= isRunning;
					}
				}

				if (propagateRequested || ticksRequested > 0 || stepsRequested > 0) {
					boolean ticked = false;
					propagateRequested = false;
					if (isRunning) {
						stepPoints.clear();
						stepsRequested = 0;
						if (propagator == null) ticksRequested = 0;
						else {
							ticked = ticksRequested > 0;
							if (ticked)
								doTick();
							do {
								propagateRequested = false;
								try {
									exceptionEncountered = false;
									propagator.propagate();
								}
								catch (Throwable thr) {
									thr.printStackTrace();
									exceptionEncountered = true;
									setIsRunning(false);
								}
							} while (propagateRequested);
							if (isOscillating()) {
								setIsRunning(false);
								ticksRequested = 0;
								propagateRequested = false;
							}
						}
					} else if (stepsRequested > 0) {
						if (ticksRequested > 0) {
							ticksRequested = 1;
							doTick();
						}

						synchronized (this) {
							stepsRequested--;
						}
						exceptionEncountered = false;
						try {
							stepPoints.clear();
							propagator.step(stepPoints);
						} catch (Throwable thr) {
							thr.printStackTrace();
							exceptionEncountered = true;
						}
					}
					if (ticked)
						fireTickCompleted();
					firePropagationCompleted();
				}
			}
		}

		private void doTick() {
			synchronized (this) {
				ticksRequested--;
			}
			propagator.tick();
		}
	}

	private boolean isRunning = true;
	private boolean isTicking;
	private boolean exceptionEncountered;
	private double tickFrequency;

	private final PropagationManager manager;
	private SimulatorTicker ticker;
	private ArrayList<SimulatorListener> listeners = new ArrayList<>();

	public Simulator() {
		manager = new PropagationManager();
		ticker = new SimulatorTicker(manager);
		try {
			manager.setPriority(manager.getPriority() - 1);
			ticker.setPriority(ticker.getPriority() - 1);
		}
		catch (SecurityException | IllegalArgumentException e) {
		}
		manager.start();
		ticker.start();

		tickFrequency = 0.0;
		setTickFrequency(AppPreferences.TICK_FREQUENCY.get());
	}

	public void shutDown() {
		ticker.shutDown();
		manager.shutDown();
	}

	public void setCircuitState(CircuitState state) {
		manager.setPropagator(state.getPropagator());
		renewTickerAwake();
	}

	public CircuitState getCircuitState() {
		Propagator prop = manager.getPropagator();
		return prop == null ? null : prop.getRootState();
	}

	public void requestReset() {
		manager.requestReset();
	}

	public void tick() {
		ticker.tickOnce();
	}

	public void step() {
		synchronized (manager) {
			manager.stepsRequested++;
			manager.notifyAll();
		}
	}

	public void drawStepPoints(ComponentDrawContext context) {
		manager.stepPoints.draw(context);
	}

	public boolean isExceptionEncountered() {
		return exceptionEncountered;
	}

	public boolean isRunning() {
		return isRunning;
	}

	public void setIsRunning(boolean value) {
		if (isRunning != value) {
			isRunning = value;
			renewTickerAwake();
			/*
			 * DEBUGGING - comment out: if (!value) flushLog(); //
			 */
			fireSimulatorStateChanged();
		}
	}

	public boolean isTicking() {
		return isTicking;
	}

	public void setIsTicking(boolean value) {
		if (isTicking != value) {
			isTicking = value;
			renewTickerAwake();
			fireSimulatorStateChanged();
		}
	}

	private void renewTickerAwake() {
		ticker.setAwake(isRunning && isTicking && tickFrequency > 0);
	}

	public double getTickFrequency() {
		return tickFrequency;
	}

	public void setTickFrequency(double freq) {
		if (tickFrequency != freq) {
			int millis = (int) Math.round(1000 / freq);
			int ticks;
			if (millis > 0) ticks = 1;
			else {
				millis = 1;
				ticks = (int) Math.round(freq / 1000);
			}

			tickFrequency = freq;
			ticker.setTickFrequency(millis, ticks);
			renewTickerAwake();
			fireSimulatorStateChanged();
		}
	}

	public void requestPropagate() {
		manager.requestPropagate();
	}

	public boolean isOscillating() {
		Propagator prop = manager.getPropagator();
		return prop != null && prop.isOscillating();
	}

	public void addSimulatorListener(SimulatorListener l) {
		listeners.add(l);
	}

	public void removeSimulatorListener(SimulatorListener l) {
		listeners.remove(l);
	}

	void firePropagationCompleted() {
		SimulatorEvent e = new SimulatorEvent(this);
		for (SimulatorListener l : new ArrayList<>(listeners)) l.propagationCompleted(e);
	}

	void fireTickCompleted() {
		SimulatorEvent e = new SimulatorEvent(this);
		for (SimulatorListener l : new ArrayList<>(listeners)) l.tickCompleted(e);
	}

	void fireSimulatorStateChanged() {
		SimulatorEvent e = new SimulatorEvent(this);
		for (SimulatorListener l : new ArrayList<>(listeners)) l.simulatorStateChanged(e);
	}
}
