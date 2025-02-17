/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.util.Collection;
import java.util.Collections;
import java.util.HashSet;

import logisim.comp.Component;

class SelectionSave {
	public static SelectionSave create(Selection sel) {
		SelectionSave save = new SelectionSave();

		Collection<Component> lifted = sel.getFloatingComponents();
		if (!lifted.isEmpty()) save.floating = lifted.toArray(new Component[0]);

		Collection<Component> selected = sel.getAnchoredComponents();
		if (!selected.isEmpty()) save.anchored = selected.toArray(new Component[0]);

		return save;
	}

	private Component[] floating;
	private Component[] anchored;

	private SelectionSave() {
	}

	public Component[] getFloatingComponents() {
		return floating;
	}

	public Component[] getAnchoredComponents() {
		return anchored;
	}

	public boolean isSame(Selection sel) {
		return isSame(floating, sel.getFloatingComponents()) && isSame(anchored, sel.getAnchoredComponents());
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof SelectionSave o && isSame(floating, o.floating) && isSame(anchored, o.anchored);
	}

	@Override
	public int hashCode() {
		int ret = 0;
		if (floating != null) for (Component c : floating)
			ret += c.hashCode();
		if (anchored != null) for (Component c : anchored)
			ret += c.hashCode();
		return ret;
	}

	private static boolean isSame(Component[] save, Collection<Component> sel) {
		if (save == null) return sel.isEmpty();
		else return toSet(save).equals(sel);
	}

	private static boolean isSame(Component[] a, Component[] b) {
		if (a == null || a.length == 0) return b == null || b.length == 0;
		else if (b == null || b.length == 0) return false;
		else if (a.length != b.length) return false;
		else return toSet(a).equals(toSet(b));
	}

	private static HashSet<Component> toSet(Component[] comps) {
		HashSet<Component> ret = new HashSet<>(comps.length);
		Collections.addAll(ret, comps);
		return ret;
	}
}
