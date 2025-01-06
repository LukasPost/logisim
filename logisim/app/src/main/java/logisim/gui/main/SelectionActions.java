/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;

import javax.swing.JOptionPane;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;

import logisim.circuit.Circuit;
import logisim.circuit.CircuitMutation;
import logisim.circuit.CircuitTransaction;
import logisim.circuit.CircuitTransactionResult;
import logisim.circuit.ReplacementMap;
import logisim.circuit.SubcircuitFactory;
import logisim.circuit.Wire;
import logisim.comp.Component;
import logisim.comp.ComponentFactory;
import logisim.data.AttributeSet;
import logisim.data.Location;
import logisim.file.LogisimFile;
import logisim.proj.Action;
import logisim.proj.JoinedAction;
import logisim.proj.Project;
import logisim.tools.AddTool;
import logisim.tools.Library;
import logisim.tools.Tool;

public class SelectionActions {
	private SelectionActions() {
	}

	public static Action drop(Selection sel, Collection<Component> comps) {
		HashSet<Component> floating = new HashSet<>(sel.getFloatingComponents());
		HashSet<Component> anchored = new HashSet<>(sel.getAnchoredComponents());
		ArrayList<Component> toDrop = new ArrayList<>();
		ArrayList<Component> toIgnore = new ArrayList<>();
		for (Component comp : comps)
			if (floating.contains(comp)) toDrop.add(comp);
			else if (anchored.contains(comp)) {
				toDrop.add(comp);
				toIgnore.add(comp);
			}
		int numDrop = toDrop.size() - toIgnore.size();
		if (numDrop == 0) {
			for (Component comp : toIgnore) sel.remove(null, comp);
			return null;
		} else return new Drop(sel, toDrop, numDrop);
	}

	public static Action dropAll(Selection sel) {
		return drop(sel, sel.getComponents());
	}

	public static Action clear(Selection sel) {
		return new Delete(sel);
	}

	public static Action duplicate(Selection sel) {
		return new Duplicate(sel);
	}

	public static Action cut(Selection sel) {
		return new Cut(sel);
	}

	public static Action copy(Selection sel) {
		return new Copy(sel);
	}

	public static Action pasteMaybe(Project proj, Selection sel) {
		HashMap<Component, Component> replacements = getReplacementMap(proj);
		return new Paste(sel, replacements);
	}

	public static Action translate(Selection sel, int dx, int dy, ReplacementMap repl) {
		return new Translate(sel, dx, dy, repl);
	}

	private static class Drop extends Action {
		private Selection sel;
		private Component[] drops;
		private int numDrops;
		private SelectionSave before;
		private CircuitTransaction xnReverse;

		Drop(Selection sel, Collection<Component> toDrop, int numDrops) {
			this.sel = sel;
			drops = new Component[toDrop.size()];
			toDrop.toArray(drops);
			this.numDrops = numDrops;
			before = SelectionSave.create(sel);
		}

		@Override
		public String getName() {
			return numDrops == 1 ? Strings.get("dropComponentAction") : Strings.get("dropComponentsAction");
		}

		@Override
		public void doIt(Project proj) {
			Circuit circuit = proj.getCurrentCircuit();
			CircuitMutation xn = new CircuitMutation(circuit);
			for (Component comp : drops) sel.remove(xn, comp);
			CircuitTransactionResult result = xn.execute();
			xnReverse = result.getReverseTransaction();
		}

		@Override
		public void undo(Project proj) {
			xnReverse.execute();
		}

		@Override
		public boolean shouldAppendTo(Action other) {
			Action last;
			if (other instanceof JoinedAction)
				last = ((JoinedAction) other).getLastAction();
			else
				last = other;

			SelectionSave otherAfter = null;
			if (last instanceof Paste) otherAfter = ((Paste) last).after;
			else if (last instanceof Duplicate) otherAfter = ((Duplicate) last).after;
			return otherAfter != null && otherAfter.equals(before);
		}
	}

	private static class Delete extends Action {
		private Selection sel;
		private CircuitTransaction xnReverse;

		Delete(Selection sel) {
			this.sel = sel;
		}

		@Override
		public String getName() {
			return Strings.get("deleteSelectionAction");
		}

		@Override
		public void doIt(Project proj) {
			Circuit circuit = proj.getCurrentCircuit();
			CircuitMutation xn = new CircuitMutation(circuit);
			sel.deleteAllHelper(xn);
			CircuitTransactionResult result = xn.execute();
			xnReverse = result.getReverseTransaction();
		}

		@Override
		public void undo(Project proj) {
			xnReverse.execute();
		}
	}

	private static class Duplicate extends Action {
		private Selection sel;
		private CircuitTransaction xnReverse;
		private SelectionSave after;

		Duplicate(Selection sel) {
			this.sel = sel;
		}

		@Override
		public String getName() {
			return Strings.get("duplicateSelectionAction");
		}

		@Override
		public void doIt(Project proj) {
			Circuit circuit = proj.getCurrentCircuit();
			CircuitMutation xn = new CircuitMutation(circuit);
			sel.duplicateHelper(xn);

			CircuitTransactionResult result = xn.execute();
			xnReverse = result.getReverseTransaction();
			after = SelectionSave.create(sel);
		}

		@Override
		public void undo(Project proj) {
			xnReverse.execute();
		}
	}

	private static class Cut extends Action {
		private Action first;
		private Action second;

		Cut(Selection sel) {
			first = new Copy(sel);
			second = new Delete(sel);
		}

		@Override
		public String getName() {
			return Strings.get("cutSelectionAction");
		}

		@Override
		public void doIt(Project proj) {
			first.doIt(proj);
			second.doIt(proj);
		}

		@Override
		public void undo(Project proj) {
			second.undo(proj);
			first.undo(proj);
		}
	}

	private static class Copy extends Action {
		private Selection sel;
		private Clipboard oldClip;

		Copy(Selection sel) {
			this.sel = sel;
		}

		@Override
		public boolean isModification() {
			return false;
		}

		@Override
		public String getName() {
			return Strings.get("copySelectionAction");
		}

		@Override
		public void doIt(Project proj) {
			oldClip = Clipboard.get();
			Clipboard.set(sel, sel.getAttributeSet());
		}

		@Override
		public void undo(Project proj) {
			Clipboard.set(oldClip);
		}
	}

	private static HashMap<Component, Component> getReplacementMap(Project proj) {
		HashMap<Component, Component> replMap = new HashMap<>();

		LogisimFile file = proj.getLogisimFile();
		ArrayList<Library> libs = new ArrayList<>();
		libs.add(file);
		libs.addAll(file.getLibraries());

		ArrayList<String> dropped = null;
		Clipboard clip = Clipboard.get();
		Collection<Component> comps = clip.getComponents();
		HashMap<ComponentFactory, ComponentFactory> factoryReplacements = new HashMap<>();
		for (Component comp : comps) {
			if (comp instanceof Wire)
				continue;

			ComponentFactory compFactory = comp.getFactory();
			ComponentFactory copyFactory = findComponentFactory(compFactory, libs, false);
			if (factoryReplacements.containsKey(compFactory)) copyFactory = factoryReplacements.get(compFactory);
			else if (copyFactory == null) {
				ComponentFactory candidate = findComponentFactory(compFactory, libs, true);
				if (candidate == null) {
					if (dropped == null) dropped = new ArrayList<>();
					dropped.add(compFactory.getDisplayName());
				} else {
					String msg = Strings.get("pasteCloneQuery", compFactory.getName());
					Object[] opts = { Strings.get("pasteCloneReplace"), Strings.get("pasteCloneIgnore"),
							Strings.get("pasteCloneCancel") };
					int select = JOptionPane.showOptionDialog(proj.getFrame(), msg, Strings.get("pasteCloneTitle"), JOptionPane.YES_NO_OPTION,
							JOptionPane.QUESTION_MESSAGE, null, opts, opts[0]);
					if (select == 0) copyFactory = candidate;
					else return null;
					factoryReplacements.put(compFactory, copyFactory);
				}
			}

			if (copyFactory == null) replMap.put(comp, null);
			else if (copyFactory != compFactory) {
				Location copyLoc = comp.getLocation();
				AttributeSet copyAttrs = (AttributeSet) comp.getAttributeSet().clone();
				Component copy = copyFactory.createComponent(copyLoc, copyAttrs);
				replMap.put(comp, copy);
			}
		}

		if (dropped != null) {
			Collections.sort(dropped);
			StringBuilder droppedStr = new StringBuilder();
			droppedStr.append(Strings.get("pasteDropMessage"));
			String curName = dropped.getFirst();
			int curCount = 1;
			int lines = 1;
			for (int i = 1; i <= dropped.size(); i++) {
				String nextName = i == dropped.size() ? "" : dropped.get(i);
				if (nextName.equals(curName)) curCount++;
				else {
					lines++;
					droppedStr.append("\n  ");
					droppedStr.append(curName);
					if (curCount > 1) droppedStr.append(" × ").append(curCount);

					curName = nextName;
					curCount = 1;
				}
			}

			lines = Math.max(3, Math.min(7, lines));
			JTextArea area = new JTextArea(lines, 60);
			area.setEditable(false);
			area.setText(droppedStr.toString());
			area.setCaretPosition(0);
			JScrollPane areaPane = new JScrollPane(area);
			JOptionPane.showMessageDialog(proj.getFrame(), areaPane, Strings.get("pasteDropTitle"),
					JOptionPane.WARNING_MESSAGE);
		}

		return replMap;
	}

	private static ComponentFactory findComponentFactory(ComponentFactory factory, ArrayList<Library> libs,
			boolean acceptNameMatch) {
		String name = factory.getName();
		for (Library lib : libs)
			for (Tool tool : lib.getTools())
				if (tool instanceof AddTool addTool) if (name.equals(addTool.getName())) {
					ComponentFactory fact = addTool.getFactory(true);
					if (acceptNameMatch) return fact;
					else if (fact == factory) return fact;
					else if (fact.getClass() == factory.getClass() && !(fact instanceof SubcircuitFactory))
						return fact;
				}
		return null;
	}

	private static class Paste extends Action {
		private Selection sel;
		private CircuitTransaction xnReverse;
		private SelectionSave after;
		private HashMap<Component, Component> componentReplacements;

		Paste(Selection sel, HashMap<Component, Component> replacements) {
			this.sel = sel;
			componentReplacements = replacements;
		}

		@Override
		public String getName() {
			return Strings.get("pasteClipboardAction");
		}

		@Override
		public void doIt(Project proj) {
			Clipboard clip = Clipboard.get();
			Circuit circuit = proj.getCurrentCircuit();
			CircuitMutation xn = new CircuitMutation(circuit);
			Collection<Component> comps = clip.getComponents();
			Collection<Component> toAdd = computeAdditions(comps);
			if (!toAdd.isEmpty()) {
				sel.pasteHelper(xn, toAdd);
				CircuitTransactionResult result = xn.execute();
				xnReverse = result.getReverseTransaction();
				after = SelectionSave.create(sel);
			} else xnReverse = null;
		}

		private Collection<Component> computeAdditions(Collection<Component> comps) {
			HashMap<Component, Component> replMap = componentReplacements;
			ArrayList<Component> toAdd = new ArrayList<>(comps.size());
			for (Component comp : comps)
				if (replMap.containsKey(comp)) {
					Component repl = replMap.get(comp);
					if (repl != null) toAdd.add(repl);
				}
				else toAdd.add(comp);
			return toAdd;
		}

		@Override
		public void undo(Project proj) {
			if (xnReverse != null) xnReverse.execute();
		}
	}

	private static class Translate extends Action {
		private Selection sel;
		private int dx;
		private int dy;
		private ReplacementMap replacements;
		private SelectionSave before;
		private CircuitTransaction xnReverse;

		Translate(Selection sel, int dx, int dy, ReplacementMap replacements) {
			this.sel = sel;
			this.dx = dx;
			this.dy = dy;
			this.replacements = replacements;
			before = SelectionSave.create(sel);
		}

		@Override
		public String getName() {
			return Strings.get("moveSelectionAction");
		}

		@Override
		public void doIt(Project proj) {
			Circuit circuit = proj.getCurrentCircuit();
			CircuitMutation xn = new CircuitMutation(circuit);

			sel.translateHelper(xn, dx, dy);
			if (replacements != null) xn.replace(replacements);

			CircuitTransactionResult result = xn.execute();
			xnReverse = result.getReverseTransaction();
		}

		@Override
		public void undo(Project proj) {
			xnReverse.execute();
		}

		@Override
		public boolean shouldAppendTo(Action other) {
			Action last;
			if (other instanceof JoinedAction)
				last = ((JoinedAction) other).getLastAction();
			else
				last = other;

			SelectionSave otherAfter = null;
			if (last instanceof Paste) otherAfter = ((Paste) last).after;
			else if (last instanceof Duplicate) otherAfter = ((Duplicate) last).after;
			return otherAfter != null && otherAfter.equals(before);
		}
	}
}
