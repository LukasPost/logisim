/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

import org.jetbrains.annotations.NotNull;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.AbstractSet;
import java.util.ConcurrentModificationException;
import java.util.Iterator;
import java.util.NoSuchElementException;

public class ArraySet<E> extends AbstractSet<E> {
	private static final Object[] EMPTY_ARRAY = new Object[0];

	private class ArrayIterator implements Iterator<E> {
		int itVersion = version;
		int pos; // position of next item to return
		boolean hasNext = values.length > 0;
		boolean removeOk;

		public boolean hasNext() {
			return hasNext;
		}

		public E next() {
			if (itVersion != version) throw new ConcurrentModificationException();
			else if (!hasNext) throw new NoSuchElementException();
			else {
				@SuppressWarnings("unchecked")
				E ret = (E) values[pos];
				++pos;
				hasNext = pos < values.length;
				removeOk = true;
				return ret;
			}
		}

		public void remove() {
			if (itVersion != version) throw new ConcurrentModificationException();
			else if (!removeOk) throw new IllegalStateException();
			else if (values.length == 1) {
				values = EMPTY_ARRAY;
				++version;
				itVersion = version;
				removeOk = false;
			} else {
				Object[] newValues = new Object[values.length - 1];
				if (pos > 1) System.arraycopy(values, 0, newValues, 0, pos - 1);
				if (pos < values.length) System.arraycopy(values, pos, newValues, pos - 1, values.length - pos);
				values = newValues;
				--pos;
				++version;
				itVersion = version;
				removeOk = false;
			}
		}
	}

	private int version;
	private Object[] values = EMPTY_ARRAY;

	public ArraySet() {
	}

	@Override
	public Object @NotNull [] toArray() {
		return values;
	}

	@Override
	public Object clone() {
		ArraySet<E> ret = new ArraySet<>();
		if (values == EMPTY_ARRAY) ret.values = EMPTY_ARRAY;
		else ret.values = values.clone();
		return ret;
	}

	@Override
	public void clear() {
		values = EMPTY_ARRAY;
		++version;
	}

	@Override
	public boolean isEmpty() {
		return values.length == 0;
	}

	@Override
	public int size() {
		return values.length;
	}

	@Override
	public boolean add(Object value) {
		int n = values.length;
		for (Object o : values)
			if (o.equals(value))
				return false;

		Object[] newValues = new Object[n + 1];
		System.arraycopy(values, 0, newValues, 0, n);
		newValues[n] = value;
		values = newValues;
		++version;
		return true;
	}

	@Override
	public boolean contains(Object value) {
		for (Object o : values)
			if (o.equals(value))
				return true;
		return false;
	}

	@Override
	public @NotNull Iterator<E> iterator() {
		return new ArrayIterator();
	}

	public static void main(String[] args) throws IOException {
		ArraySet<String> set = new ArraySet<>();
		BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
		while (true) {
			System.out.print(set.size() + ":"); // OK
			for (String str : set) System.out.print(" " + str); // OK
			System.out.println(); // OK
			System.out.print("> "); // OK
			String cmd = in.readLine();
			if (cmd == null)
				break;
			cmd = cmd.trim();
			if (cmd.isEmpty()) ;
			else if (cmd.startsWith("+")) set.add(cmd.substring(1));
			else if (cmd.startsWith("-")) set.remove(cmd.substring(1));
			else if (cmd.startsWith("?")) {
				boolean ret = set.contains(cmd.substring(1));
				System.out.println("  " + ret); // OK
			} else System.out.println("unrecognized command"); // OK
		}
	}
}
