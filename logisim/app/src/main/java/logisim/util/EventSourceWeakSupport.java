/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

import org.jetbrains.annotations.NotNull;

import java.lang.ref.WeakReference;
import java.util.Iterator;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentLinkedQueue;

public class EventSourceWeakSupport<L> implements Iterable<L> {
	private ConcurrentLinkedQueue<WeakReference<L>> listeners = new ConcurrentLinkedQueue<>();

	public EventSourceWeakSupport() {
	}

	public void add(L listener) {
		listeners.add(new WeakReference<>(listener));
	}

	public void remove(L listener) {
		for (Iterator<WeakReference<L>> it = listeners.iterator(); it.hasNext();) {
			L l = it.next().get();
			if (l == null || l == listener)
				it.remove();
		}
	}

	public boolean isEmpty() {
		for (Iterator<WeakReference<L>> it = listeners.iterator(); it.hasNext();) {
			L l = it.next().get();
			if (l == null) it.remove();
			else return false;
		}
		return true;
	}

	public @NotNull Iterator<L> iterator() {
		// copy elements into another list in case any event handlers
		// want to add a listener
		ArrayList<L> ret = new ArrayList<>(listeners.size());
		for (Iterator<WeakReference<L>> it = listeners.iterator(); it.hasNext();) {
			L l = it.next().get();
			if (l == null) it.remove();
			else ret.add(l);
		}
		return ret.iterator();
	}
}
