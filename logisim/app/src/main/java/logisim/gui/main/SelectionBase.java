/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.awt.Graphics;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.HashSet;
import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

import logisim.circuit.Circuit;
import logisim.circuit.CircuitMutation;
import logisim.circuit.Wire;
import logisim.comp.Component;
import logisim.comp.ComponentFactory;
import logisim.comp.EndData;
import logisim.data.AttributeSet;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.gui.main.Selection.Event;
import logisim.gui.main.Selection.Listener;
import logisim.proj.Project;
import logisim.util.CollectionUtil;

class SelectionBase {

	Project proj;
	private ArrayList<Listener> listeners = new ArrayList<>();

	final HashSet<Component> selected = new HashSet<>(); // of selected Components in circuit
	final HashSet<Component> lifted = new HashSet<>(); // of selected Components removed
	final HashSet<Component> suppressHandles = new HashSet<>(); // of Components
	final Set<Component> unionSet = CollectionUtil.createUnmodifiableSetUnion(selected, lifted);

	private Bounds bounds = Bounds.EMPTY_BOUNDS;
	private boolean shouldSnap;

	public SelectionBase(Project proj) {
		this.proj = proj;
	}

	//
	// listener methods
	//
	public void addListener(Listener l) {
		listeners.add(l);
	}

	public void removeListener(Listener l) {
		listeners.remove(l);
	}

	public void fireSelectionChanged() {
		bounds = null;
		computeShouldSnap();
		Event e = new Event(this);
		for (Listener l : listeners) l.selectionChanged(e);
	}

	//
	// query methods
	//
	public Bounds getBounds() {
		if (bounds == null) bounds = computeBounds(unionSet);
		return bounds;
	}

	public Bounds getBounds(Graphics g) {
		Iterator<Component> it = unionSet.iterator();
		if (it.hasNext()) {
			bounds = it.next().getBounds(g);
			while (it.hasNext()) {
				Component comp = it.next();
				Bounds bds = comp.getBounds(g);
				bounds = bounds.add(bds);
			}
		} else bounds = Bounds.EMPTY_BOUNDS;
		return bounds;
	}

	public boolean shouldSnap() {
		return shouldSnap;
	}

	public boolean hasConflictWhenMoved(int dx, int dy) {
		return hasConflictTranslated(unionSet, dx, dy, false);
	}

	//
	// action methods
	//
	public void add(Component comp) {
		if (selected.add(comp)) fireSelectionChanged();
	}

	public void addAll(Collection<? extends Component> comps) {
		if (selected.addAll(comps)) fireSelectionChanged();
	}

	// removes from selection - NOT from circuit
	void remove(CircuitMutation xn, Component comp) {
		boolean removed = selected.remove(comp);
		if (lifted.contains(comp)) if (xn == null) throw new IllegalStateException("cannot remove");
		else {
			lifted.remove(comp);
			removed = true;
			xn.add(comp);
		}

		if (removed) {
			if (shouldSnapComponent(comp))
				computeShouldSnap();
			fireSelectionChanged();
		}
	}

	void dropAll(CircuitMutation xn) {
		if (!lifted.isEmpty()) {
			xn.addAll(lifted);
			selected.addAll(lifted);
			lifted.clear();
		}
	}

	// removes all from selection - NOT from circuit
	void clear(CircuitMutation xn) {
		if (selected.isEmpty() && lifted.isEmpty())
			return;

		if (!lifted.isEmpty()) xn.addAll(lifted);

		selected.clear();
		lifted.clear();
		shouldSnap = false;
		bounds = Bounds.EMPTY_BOUNDS;

		fireSelectionChanged();
	}

	public void setSuppressHandles(Collection<Component> toSuppress) {
		suppressHandles.clear();
		if (toSuppress != null)
			suppressHandles.addAll(toSuppress);
	}

	void duplicateHelper(CircuitMutation xn) {
		HashSet<Component> oldSelected = new HashSet<>(selected);
		oldSelected.addAll(lifted);
		pasteHelper(xn, oldSelected);
	}

	void pasteHelper(CircuitMutation xn, Collection<Component> comps) {
		clear(xn);

		Map<Component, Component> newLifted = copyComponents(comps);
		lifted.addAll(newLifted.values());
		fireSelectionChanged();
	}

	void deleteAllHelper(CircuitMutation xn) {
		for (Component comp : selected) xn.remove(comp);
		selected.clear();
		lifted.clear();
		fireSelectionChanged();
	}

	void translateHelper(CircuitMutation xn, int dx, int dy) {
		Map<Component, Component> selectedAfter = copyComponents(selected, dx, dy);
		for (Entry<Component, Component> entry : selectedAfter.entrySet()) xn.replace(entry.getKey(), entry.getValue());

		Map<Component, Component> liftedAfter = copyComponents(lifted, dx, dy);
		lifted.clear();
		for (Entry<Component, Component> entry : liftedAfter.entrySet()) {
			xn.add(entry.getValue());
			selected.add(entry.getValue());
		}
		fireSelectionChanged();
	}

	//
	// private methods
	//
	private void computeShouldSnap() {
		shouldSnap = false;
		for (Component comp : unionSet)
			if (shouldSnapComponent(comp)) {
				shouldSnap = true;
				return;
			}
	}

	private static boolean shouldSnapComponent(Component comp) {
		Boolean shouldSnapValue = (Boolean) comp.getFactory().getFeature(ComponentFactory.SHOULD_SNAP,
				comp.getAttributeSet());
		return shouldSnapValue == null || shouldSnapValue;
	}

	private boolean hasConflictTranslated(Collection<Component> components, int dx, int dy, boolean selfConflicts) {
		Circuit circuit = proj.getCurrentCircuit();
		if (circuit == null)
			return false;
		for (Component comp : components)
			if (!(comp instanceof Wire)) {
				for (EndData endData : comp.getEnds())
					if (endData != null && endData.isExclusive()) {
						Location endLoc = endData.getLocation().add(dx, dy);
						Component conflict = circuit.getExclusive(endLoc);
						if (conflict != null) if (selfConflicts || !components.contains(conflict))
							return true;
					}
				Location newLoc = comp.getLocation().add(dx, dy);
				Bounds newBounds = comp.getBounds().translate(dx, dy);
				for (Component comp2 : circuit.getAllContaining(newLoc)) {
					Bounds bds = comp2.getBounds();
					if (bds.equals(newBounds)) if (selfConflicts || !components.contains(comp2))
						return true;
				}
			}
		return false;
	}

	private static Bounds computeBounds(Collection<Component> components) {
		if (components.isEmpty()) return Bounds.EMPTY_BOUNDS;
		else {
			Iterator<Component> it = components.iterator();
			Bounds ret = it.next().getBounds();
			while (it.hasNext()) {
				Component comp = it.next();
				Bounds bds = comp.getBounds();
				ret = ret.add(bds);
			}
			return ret;
		}
	}

	private HashMap<Component, Component> copyComponents(Collection<Component> components) {
		// determine translation offset where we can legally place the clipboard
		int dx;
		int dy;
		Bounds bds = computeBounds(components);
		for (int index = 0;; index++) {
			// compute offset to try: We try points along successively larger
			// squares radiating outward from 0,0
			if (index == 0) {
				dx = 0;
				dy = 0;
			} else {
				int side = 1;
				while (side * side <= index)
					side += 2;
				int offs = index - (side - 2) * (side - 2);
				dx = side / 2;
				dy = side / 2;
				// top edge of square
				if (offs < side - 1) dx -= offs;
				else if (offs < 2 * (side - 1)) { // left edge
					offs -= side - 1;
					dx = -dx;
					dy -= offs;
				} else if (offs < 3 * (side - 1)) { // right edge
					offs -= 2 * (side - 1);
					dx = -dx + offs;
					dy = -dy;
				} else {
					offs -= 3 * (side - 1);
					dy = -dy + offs;
				}
				dx *= 10;
				dy *= 10;
			}

			if (bds.getX() + dx >= 0 && bds.getY() + dy >= 0 && !hasConflictTranslated(components, dx, dy, true))
				return copyComponents(components, dx, dy);
		}
	}

	private HashMap<Component, Component> copyComponents(Collection<Component> components, int dx, int dy) {
		HashMap<Component, Component> ret = new HashMap<>();
		for (Component comp : components) {
			Location oldLoc = comp.getLocation();
			AttributeSet attrs = (AttributeSet) comp.getAttributeSet().clone();
			int newX = oldLoc.x() + dx;
			int newY = oldLoc.y() + dy;
			Object snap = comp.getFactory().getFeature(ComponentFactory.SHOULD_SNAP, attrs);
			if (snap == null || (Boolean) snap) {
				newX = Canvas.snapXToGrid(newX);
				newY = Canvas.snapYToGrid(newY);
			}
			Location newLoc = new Location(newX, newY);

			Component copy = comp.getFactory().createComponent(newLoc, attrs);
			ret.put(comp, copy);
		}
		return ret;
	}

	// debugging methods
	public void print() {
		System.err.println(" shouldSnap: " + shouldSnap()); // OK

		boolean hasPrinted = false;
		for (Component comp : selected) {
			System.err.println((hasPrinted ? "         " : " select: ") // OK
					+ comp + "  [" + comp.hashCode() + "]");
			hasPrinted = true;
		}

		hasPrinted = false;
		for (Component comp : lifted) {
			System.err.println((hasPrinted ? "         " : " lifted: ") // OK
					+ comp + "  [" + comp.hashCode() + "]");
			hasPrinted = true;
		}
	}

}
