/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools.move;

import logisim.circuit.ReplacementMap;

class ConnectorThread extends Thread {
	private static ConnectorThread INSTANCE = new ConnectorThread();

	static {
		INSTANCE.start();
	}

	public static void enqueueRequest(MoveRequest req, boolean priority) {
		synchronized (INSTANCE.lock) {
			if (!req.equals(INSTANCE.processingRequest)) {
				INSTANCE.nextRequest = req;
				INSTANCE.overrideRequest = priority;
				INSTANCE.lock.notifyAll();
			}
		}
	}

	public static boolean isOverrideRequested() {
		return INSTANCE.overrideRequest;
	}

	private final Object lock;
	private transient boolean overrideRequest;
	private MoveRequest nextRequest;
	private MoveRequest processingRequest;

	private ConnectorThread() {
		lock = new Object();
		overrideRequest = false;
		nextRequest = null;
	}

	public boolean isAbortRequested() {
		return overrideRequest;
	}

	@Override
	public void run() {
		while (true) {
			MoveRequest req;
			boolean wasOverride;
			synchronized (lock) {
				processingRequest = null;
				while (nextRequest == null) try {
					lock.wait();
				} catch (InterruptedException e) {
					Thread.currentThread().interrupt();
					return;
				}
				req = nextRequest;
				wasOverride = overrideRequest;
				nextRequest = null;
				overrideRequest = false;
				processingRequest = req;
			}

			try {
				MoveResult result = Connector.computeWires(req);
				if (result != null) {
					MoveGesture gesture = req.getMoveGesture();
					gesture.notifyResult(req, result);
				}
			}
			catch (Throwable t) {
				t.printStackTrace();
				if (wasOverride) {
					MoveResult result = new MoveResult(new ReplacementMap(), req.getMoveGesture().getConnections(),
							0);
					req.getMoveGesture().notifyResult(req, result);
				}
			}
		}
	}
}
