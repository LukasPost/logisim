// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{


	using Circuit = logisim.circuit.Circuit;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using CircuitTransaction = logisim.circuit.CircuitTransaction;
	using CircuitTransactionResult = logisim.circuit.CircuitTransactionResult;
	using ReplacementMap = logisim.circuit.ReplacementMap;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using AttributeSet = logisim.data.AttributeSet;
	using Location = logisim.data.Location;
	using LogisimFile = logisim.file.LogisimFile;
	using Action = logisim.proj.Action;
	using JoinedAction = logisim.proj.JoinedAction;
	using Project = logisim.proj.Project;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class SelectionActions
	{
		private SelectionActions()
		{
		}

		public static Action drop(Selection sel, ICollection<Component> comps)
		{
			HashSet<Component> floating = new HashSet<Component>(sel.FloatingComponents);
			HashSet<Component> anchored = new HashSet<Component>(sel.AnchoredComponents);
			List<Component> toDrop = new List<Component>();
			List<Component> toIgnore = new List<Component>();
			foreach (Component comp in comps)
			{
				if (floating.Contains(comp))
				{
					toDrop.Add(comp);
				}
				else if (anchored.Contains(comp))
				{
					toDrop.Add(comp);
					toIgnore.Add(comp);
				}
			}
			int numDrop = toDrop.Count - toIgnore.Count;
			if (numDrop == 0)
			{
				foreach (Component comp in toIgnore)
				{
					sel.remove(null, comp);
				}
				return null;
			}
			else
			{
				return new Drop(sel, toDrop, numDrop);
			}
		}

		public static Action dropAll(Selection sel)
		{
			return drop(sel, sel.Components);
		}

		public static Action clear(Selection sel)
		{
			return new Delete(sel);
		}

		public static Action duplicate(Selection sel)
		{
			return new Duplicate(sel);
		}

		public static Action cut(Selection sel)
		{
			return new Cut(sel);
		}

		public static Action copy(Selection sel)
		{
			return new Copy(sel);
		}

		public static Action pasteMaybe(Project proj, Selection sel)
		{
			Dictionary<Component, Component> replacements = getReplacementMap(proj);
			return new Paste(sel, replacements);
		}

		public static Action translate(Selection sel, int dx, int dy, ReplacementMap repl)
		{
			return new Translate(sel, dx, dy, repl);
		}

		private class Drop : Action
		{
			internal Selection sel;
			internal Component[] drops;
			internal int numDrops;
			internal SelectionSave before;
			internal CircuitTransaction xnReverse;

			internal Drop(Selection sel, ICollection<Component> toDrop, int numDrops)
			{
				this.sel = sel;
				this.drops = new Component[toDrop.Count];
				toDrop.toArray(this.drops);
				this.numDrops = numDrops;
				this.before = SelectionSave.create(sel);
			}

			public override string Name
			{
				get
				{
					return numDrops == 1 ? Strings.get("dropComponentAction") : Strings.get("dropComponentsAction");
				}
			}

			public override void doIt(Project proj)
			{
				Circuit circuit = proj.CurrentCircuit;
				CircuitMutation xn = new CircuitMutation(circuit);
				foreach (Component comp in drops)
				{
					sel.remove(xn, comp);
				}
				CircuitTransactionResult result = xn.execute();
				xnReverse = result.ReverseTransaction;
			}

			public override void undo(Project proj)
			{
				xnReverse.execute();
			}

			public override bool shouldAppendTo(Action other)
			{
				Action last;
				if (other is JoinedAction)
				{
					last = ((JoinedAction) other).LastAction;
				}
				else
				{
					last = other;
				}

				SelectionSave otherAfter = null;
				if (last is Paste)
				{
					otherAfter = ((Paste) last).after;
				}
				else if (last is Duplicate)
				{
					otherAfter = ((Duplicate) last).after;
				}
				return otherAfter != null && otherAfter.Equals(this.before);
			}
		}

		private class Delete : Action
		{
			internal Selection sel;
			internal CircuitTransaction xnReverse;

			internal Delete(Selection sel)
			{
				this.sel = sel;
			}

			public override string Name
			{
				get
				{
					return Strings.get("deleteSelectionAction");
				}
			}

			public override void doIt(Project proj)
			{
				Circuit circuit = proj.CurrentCircuit;
				CircuitMutation xn = new CircuitMutation(circuit);
				sel.deleteAllHelper(xn);
				CircuitTransactionResult result = xn.execute();
				xnReverse = result.ReverseTransaction;
			}

			public override void undo(Project proj)
			{
				xnReverse.execute();
			}
		}

		private class Duplicate : Action
		{
			internal Selection sel;
			internal CircuitTransaction xnReverse;
			internal SelectionSave after;

			internal Duplicate(Selection sel)
			{
				this.sel = sel;
			}

			public override string Name
			{
				get
				{
					return Strings.get("duplicateSelectionAction");
				}
			}

			public override void doIt(Project proj)
			{
				Circuit circuit = proj.CurrentCircuit;
				CircuitMutation xn = new CircuitMutation(circuit);
				sel.duplicateHelper(xn);

				CircuitTransactionResult result = xn.execute();
				xnReverse = result.ReverseTransaction;
				after = SelectionSave.create(sel);
			}

			public override void undo(Project proj)
			{
				xnReverse.execute();
			}
		}

		private class Cut : Action
		{
			internal Action first;
			internal Action second;

			internal Cut(Selection sel)
			{
				first = new Copy(sel);
				second = new Delete(sel);
			}

			public override string Name
			{
				get
				{
					return Strings.get("cutSelectionAction");
				}
			}

			public override void doIt(Project proj)
			{
				first.doIt(proj);
				second.doIt(proj);
			}

			public override void undo(Project proj)
			{
				second.undo(proj);
				first.undo(proj);
			}
		}

		private class Copy : Action
		{
			internal Selection sel;
			internal Clipboard oldClip;

			internal Copy(Selection sel)
			{
				this.sel = sel;
			}

			public override bool Modification
			{
				get
				{
					return false;
				}
			}

			public override string Name
			{
				get
				{
					return Strings.get("copySelectionAction");
				}
			}

			public override void doIt(Project proj)
			{
				oldClip = Clipboard.get();
				Clipboard.set(sel, sel.AttributeSet);
			}

			public override void undo(Project proj)
			{
				Clipboard.set(oldClip);
			}
		}

		private static Dictionary<Component, Component> getReplacementMap(Project proj)
		{
			Dictionary<Component, Component> replMap;
			replMap = new Dictionary<Component, Component>();

			LogisimFile file = proj.LogisimFile;
			List<Library> libs = new List<Library>();
			libs.Add(file);
			libs.AddRange(file.Libraries);

			List<string> dropped = null;
			Clipboard clip = Clipboard.get();
			ICollection<Component> comps = clip.Components;
			Dictionary<ComponentFactory, ComponentFactory> factoryReplacements;
			factoryReplacements = new Dictionary<ComponentFactory, ComponentFactory>();
			foreach (Component comp in comps)
			{
				if (comp is Wire)
				{
					continue;
				}

				ComponentFactory compFactory = comp.Factory;
				ComponentFactory copyFactory = findComponentFactory(compFactory, libs, false);
				if (factoryReplacements.ContainsKey(compFactory))
				{
					copyFactory = factoryReplacements[compFactory];
				}
				else if (copyFactory == null)
				{
					ComponentFactory candidate = findComponentFactory(compFactory, libs, true);
					if (candidate == null)
					{
						if (dropped == null)
						{
							dropped = new List<string>();
						}
						dropped.Add(compFactory.DisplayName);
					}
					else
					{
						string msg = Strings.get("pasteCloneQuery", compFactory.Name);
						object[] opts = new object[] {Strings.get("pasteCloneReplace"), Strings.get("pasteCloneIgnore"), Strings.get("pasteCloneCancel")};
						int select = JOptionPane.showOptionDialog(proj.Frame, msg, Strings.get("pasteCloneTitle"), 0, JOptionPane.QUESTION_MESSAGE, null, opts, opts[0]);
						if (select == 0)
						{
							copyFactory = candidate;
						}
						else if (select == 1)
						{
							copyFactory = null;
						}
						else
						{
							return null;
						}
						factoryReplacements[compFactory] = copyFactory;
					}
				}

				if (copyFactory == null)
				{
					replMap[comp] = null;
				}
				else if (copyFactory != compFactory)
				{
					Location copyLoc = comp.Location;
					AttributeSet copyAttrs = (AttributeSet) comp.AttributeSet.clone();
					Component copy = copyFactory.createComponent(copyLoc, copyAttrs);
					replMap[comp] = copy;
				}
			}

			if (dropped != null)
			{
				dropped.Sort();
				StringBuilder droppedStr = new StringBuilder();
				droppedStr.Append(Strings.get("pasteDropMessage"));
				string curName = dropped[0];
				int curCount = 1;
				int lines = 1;
				for (int i = 1; i <= dropped.Count; i++)
				{
					string nextName = i == dropped.Count ? "" : dropped[i];
					if (nextName.Equals(curName))
					{
						curCount++;
					}
					else
					{
						lines++;
						droppedStr.Append("\n  ");
						droppedStr.Append(curName);
						if (curCount > 1)
						{
							droppedStr.Append(" \u00d7 " + curCount);
						}

						curName = nextName;
						curCount = 1;
					}
				}

				lines = Math.Max(3, Math.Min(7, lines));
				JTextArea area = new JTextArea(lines, 60);
				area.setEditable(false);
				area.setText(droppedStr.ToString());
				area.setCaretPosition(0);
				JScrollPane areaPane = new JScrollPane(area);
				JOptionPane.showMessageDialog(proj.Frame, areaPane, Strings.get("pasteDropTitle"), JOptionPane.WARNING_MESSAGE);
			}

			return replMap;
		}

		private static ComponentFactory findComponentFactory(ComponentFactory factory, List<Library> libs, bool acceptNameMatch)
		{
			string name = factory.Name;
			foreach (Library lib in libs)
			{
				foreach (Tool tool in lib.Tools)
				{
					if (tool is AddTool)
					{
						AddTool addTool = (AddTool) tool;
						if (name.Equals(addTool.Name))
						{
							ComponentFactory fact = addTool.getFactory(true);
							if (acceptNameMatch)
							{
								return fact;
							}
							else if (fact == factory)
							{
								return fact;
							}
							else if (fact.GetType() == factory.GetType() && !(fact is SubcircuitFactory))
							{
								return fact;
							}
						}
					}
				}
			}
			return null;
		}

		private class Paste : Action
		{
			internal Selection sel;
			internal CircuitTransaction xnReverse;
			internal SelectionSave after;
			internal Dictionary<Component, Component> componentReplacements;

			internal Paste(Selection sel, Dictionary<Component, Component> replacements)
			{
				this.sel = sel;
				this.componentReplacements = replacements;
			}

			public override string Name
			{
				get
				{
					return Strings.get("pasteClipboardAction");
				}
			}

			public override void doIt(Project proj)
			{
				Clipboard clip = Clipboard.get();
				Circuit circuit = proj.CurrentCircuit;
				CircuitMutation xn = new CircuitMutation(circuit);
				ICollection<Component> comps = clip.Components;
				ICollection<Component> toAdd = computeAdditions(comps);
				if (toAdd.Count > 0)
				{
					sel.pasteHelper(xn, toAdd);
					CircuitTransactionResult result = xn.execute();
					xnReverse = result.ReverseTransaction;
					after = SelectionSave.create(sel);
				}
				else
				{
					xnReverse = null;
				}
			}

			internal virtual ICollection<Component> computeAdditions(ICollection<Component> comps)
			{
				Dictionary<Component, Component> replMap = componentReplacements;
				List<Component> toAdd = new List<Component>(comps.Count);
				for (IEnumerator<Component> it = comps.GetEnumerator(); it.MoveNext();)
				{
					Component comp = it.Current;
					if (replMap.ContainsKey(comp))
					{
						Component repl = replMap[comp];
						if (repl != null)
						{
							toAdd.Add(repl);
						}
					}
					else
					{
						toAdd.Add(comp);
					}
				}
				return toAdd;
			}

			public override void undo(Project proj)
			{
				if (xnReverse != null)
				{
					xnReverse.execute();
				}
			}
		}

		private class Translate : Action
		{
			internal Selection sel;
			internal int dx;
			internal int dy;
			internal ReplacementMap replacements;
			internal SelectionSave before;
			internal CircuitTransaction xnReverse;

			internal Translate(Selection sel, int dx, int dy, ReplacementMap replacements)
			{
				this.sel = sel;
				this.dx = dx;
				this.dy = dy;
				this.replacements = replacements;
				this.before = SelectionSave.create(sel);
			}

			public override string Name
			{
				get
				{
					return Strings.get("moveSelectionAction");
				}
			}

			public override void doIt(Project proj)
			{
				Circuit circuit = proj.CurrentCircuit;
				CircuitMutation xn = new CircuitMutation(circuit);

				sel.translateHelper(xn, dx, dy);
				if (replacements != null)
				{
					xn.replace(replacements);
				}

				CircuitTransactionResult result = xn.execute();
				xnReverse = result.ReverseTransaction;
			}

			public override void undo(Project proj)
			{
				xnReverse.execute();
			}

			public override bool shouldAppendTo(Action other)
			{
				Action last;
				if (other is JoinedAction)
				{
					last = ((JoinedAction) other).LastAction;
				}
				else
				{
					last = other;
				}

				SelectionSave otherAfter = null;
				if (last is Paste)
				{
					otherAfter = ((Paste) last).after;
				}
				else if (last is Duplicate)
				{
					otherAfter = ((Duplicate) last).after;
				}
				return otherAfter != null && otherAfter.Equals(this.before);
			}
		}
	}

}
