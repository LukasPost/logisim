// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using EndData = logisim.comp.EndData;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Project = logisim.proj.Project;
	using CollectionUtil = logisim.util.CollectionUtil;

	internal class SelectionBase
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			unionSet = CollectionUtil.createUnmodifiableSetUnion(selected, lifted);
		}

		internal static readonly HashSet<Component> NO_COMPONENTS = Collections.emptySet();

		internal Project proj;
		private List<Selection.Listener> listeners = new List<Selection.Listener>();

		internal readonly HashSet<Component> selected = new HashSet<Component>(); // of selected Components in circuit
		internal readonly HashSet<Component> lifted = new HashSet<Component>(); // of selected Components removed
		internal readonly HashSet<Component> suppressHandles = new HashSet<Component>(); // of Components
		internal HashSet<Component> unionSet;

		private Bounds bounds = Bounds.EMPTY_BOUNDS;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool shouldSnap_Conflict = false;

		public SelectionBase(Project proj)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.proj = proj;
		}

		//
		// listener methods
		//
		public virtual void addListener(Selection.Listener l)
		{
			listeners.Add(l);
		}

		public virtual void removeListener(Selection.Listener l)
		{
			listeners.Remove(l);
		}

		public virtual void fireSelectionChanged()
		{
			bounds = null;
			computeShouldSnap();
			Selection.Event e = new Selection.Event(this);
			foreach (Selection.Listener l in listeners)
			{
				l.selectionChanged(e);
			}
		}

		//
		// query methods
		//
		public virtual Bounds Bounds
		{
			get
			{
				if (bounds == null)
				{
					bounds = computeBounds(unionSet);
				}
				return bounds;
			}
		}

		public virtual Bounds getBounds(JGraphics g)
		{
			IEnumerator<Component> it = unionSet.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (it.hasNext())
			{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				bounds = it.next().getBounds(g);
				while (it.MoveNext())
				{
					Component comp = it.Current;
					Bounds bds = comp.getBounds(g);
					bounds = bounds.add(bds);
				}
			}
			else
			{
				bounds = Bounds.EMPTY_BOUNDS;
			}
			return bounds;
		}

		public virtual bool shouldSnap()
		{
			return shouldSnap_Conflict;
		}

		public virtual bool hasConflictWhenMoved(int dx, int dy)
		{
			return hasConflictTranslated(unionSet, dx, dy, false);
		}

		//
		// action methods
		//
		public virtual void add(Component comp)
		{
			if (selected.Add(comp))
			{
				fireSelectionChanged();
			}
		}

		public virtual void addAll<T1>(ICollection<T1> comps) where T1 : logisim.comp.Component
		{
			if (selected.addAll(comps))
			{
				fireSelectionChanged();
			}
		}

		// removes from selection - NOT from circuit
		internal virtual void remove(CircuitMutation xn, Component comp)
		{
			bool removed = selected.Remove(comp);
			if (lifted.Contains(comp))
			{
				if (xn == null)
				{
					throw new System.InvalidOperationException("cannot remove");
				}
				else
				{
					lifted.Remove(comp);
					removed = true;
					xn.add(comp);
				}
			}

			if (removed)
			{
				if (shouldSnapComponent(comp))
				{
					computeShouldSnap();
				}
				fireSelectionChanged();
			}
		}

		internal virtual void dropAll(CircuitMutation xn)
		{
			if (lifted.Count > 0)
			{
				xn.addAll(lifted);
				selected.addAll(lifted);
				lifted.Clear();
			}
		}

		internal virtual void clear(CircuitMutation xn)
		{
			clear(xn, true);
		}

		// removes all from selection - NOT from circuit
		internal virtual void clear(CircuitMutation xn, bool dropLifted)
		{
			if (selected.Count == 0 && lifted.Count == 0)
			{
				return;
			}

			if (dropLifted && lifted.Count > 0)
			{
				xn.addAll(lifted);
			}

			selected.Clear();
			lifted.Clear();
			shouldSnap_Conflict = false;
			bounds = Bounds.EMPTY_BOUNDS;

			fireSelectionChanged();
		}

		public virtual ICollection<Component> SuppressHandles
		{
			set
			{
				suppressHandles.Clear();
				if (value != null)
				{
					suppressHandles.addAll(value);
				}
			}
		}

		internal virtual void duplicateHelper(CircuitMutation xn)
		{
			HashSet<Component> oldSelected = new HashSet<Component>(selected);
			oldSelected.addAll(lifted);
			pasteHelper(xn, oldSelected);
		}

		internal virtual void pasteHelper(CircuitMutation xn, ICollection<Component> comps)
		{
			clear(xn);

			Dictionary<Component, Component> newLifted = copyComponents(comps);
			lifted.addAll(newLifted.Values);
			fireSelectionChanged();
		}

		internal virtual void deleteAllHelper(CircuitMutation xn)
		{
			foreach (Component comp in selected)
			{
				xn.remove(comp);
			}
			selected.Clear();
			lifted.Clear();
			fireSelectionChanged();
		}

		internal virtual void translateHelper(CircuitMutation xn, int dx, int dy)
		{
			Dictionary<Component, Component> selectedAfter = copyComponents(selected, dx, dy);
			foreach (KeyValuePair<Component, Component> entry in selectedAfter.SetOfKeyValuePairs())
			{
				xn.replace(entry.Key, entry.Value);
			}

			Dictionary<Component, Component> liftedAfter = copyComponents(lifted, dx, dy);
			lifted.Clear();
			foreach (KeyValuePair<Component, Component> entry in liftedAfter.SetOfKeyValuePairs())
			{
				xn.add(entry.Value);
				selected.Add(entry.Value);
			}
			fireSelectionChanged();
		}

		//
		// private methods
		//
		private void computeShouldSnap()
		{
			shouldSnap_Conflict = false;
			foreach (Component comp in unionSet)
			{
				if (shouldSnapComponent(comp))
				{
					shouldSnap_Conflict = true;
					return;
				}
			}
		}

		private static bool shouldSnapComponent(Component comp)
		{
			bool? shouldSnapValue = (bool?) comp.Factory.getFeature(ComponentFactory.SHOULD_SNAP, comp.AttributeSet);
			return shouldSnapValue == null ? true : shouldSnapValue.Value;
		}

		private bool hasConflictTranslated(ICollection<Component> components, int dx, int dy, bool selfConflicts)
		{
			Circuit circuit = proj.CurrentCircuit;
			if (circuit == null)
			{
				return false;
			}
			foreach (Component comp in components)
			{
				if (!(comp is Wire))
				{
					foreach (EndData endData in comp.Ends)
					{
						if (endData != null && endData.Exclusive)
						{
							Location endLoc = endData.Location.translate(dx, dy);
							Component conflict = circuit.getExclusive(endLoc);
							if (conflict != null)
							{
								if (selfConflicts || !components.Contains(conflict))
								{
									return true;
								}
							}
						}
					}
					Location newLoc = comp.Location.translate(dx, dy);
					Bounds newBounds = comp.Bounds.translate(dx, dy);
					foreach (Component comp2 in circuit.getAllContaining(newLoc))
					{
						Bounds bds = comp2.Bounds;
						if (bds.Equals(newBounds))
						{
							if (selfConflicts || !components.Contains(comp2))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private static Bounds computeBounds(ICollection<Component> components)
		{
			if (components.Count == 0)
			{
				return Bounds.EMPTY_BOUNDS;
			}
			else
			{
				IEnumerator<Component> it = components.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Bounds ret = it.next().getBounds();
				while (it.MoveNext())
				{
					Component comp = it.Current;
					Bounds bds = comp.Bounds;
					ret = ret.add(bds);
				}
				return ret;
			}
		}

		private Dictionary<Component, Component> copyComponents(ICollection<Component> components)
		{
			// determine translation offset where we can legally place the clipboard
			int dx;
			int dy;
			Bounds bds = computeBounds(components);
			for (int index = 0;; index++)
			{
				// compute offset to try: We try points along successively larger
				// squares radiating outward from 0,0
				if (index == 0)
				{
					dx = 0;
					dy = 0;
				}
				else
				{
					int side = 1;
					while (side * side <= index)
					{
						side += 2;
					}
					int offs = index - (side - 2) * (side - 2);
					dx = side / 2;
					dy = side / 2;
					if (offs < side - 1)
					{ // top edge of square
						dx -= offs;
					}
					else if (offs < 2 * (side - 1))
					{ // left edge
						offs -= side - 1;
						dx = -dx;
						dy -= offs;
					}
					else if (offs < 3 * (side - 1))
					{ // right edge
						offs -= 2 * (side - 1);
						dx = -dx + offs;
						dy = -dy;
					}
					else
					{
						offs -= 3 * (side - 1);
						dy = -dy + offs;
					}
					dx *= 10;
					dy *= 10;
				}

				if (bds.X + dx >= 0 && bds.Y + dy >= 0 && !hasConflictTranslated(components, dx, dy, true))
				{
					return copyComponents(components, dx, dy);
				}
			}
		}

		private Dictionary<Component, Component> copyComponents(ICollection<Component> components, int dx, int dy)
		{
			Dictionary<Component, Component> ret = new Dictionary<Component, Component>();
			foreach (Component comp in components)
			{
				Location oldLoc = comp.Location;
				AttributeSet attrs = (AttributeSet) comp.AttributeSet.clone();
				int newX = oldLoc.X + dx;
				int newY = oldLoc.Y + dy;
				object snap = comp.Factory.getFeature(ComponentFactory.SHOULD_SNAP, attrs);
				if (snap == null || ((bool?) snap).Value)
				{
					newX = Canvas.snapXToGrid(newX);
					newY = Canvas.snapYToGrid(newY);
				}
				Location newLoc = new Location(newX, newY);

				Component copy = comp.Factory.createComponent(newLoc, attrs);
				ret[comp] = copy;
			}
			return ret;
		}

		// debugging methods
		public virtual void print()
		{
			Console.Error.WriteLine(" shouldSnap: " + shouldSnap()); // OK

			bool hasPrinted = false;
			foreach (Component comp in selected)
			{
				Console.Error.WriteLine((hasPrinted ? "         " : " select: ") + comp + "  [" + comp.GetHashCode() + "]");
				hasPrinted = true;
			}

			hasPrinted = false;
			foreach (Component comp in lifted)
			{
				Console.Error.WriteLine((hasPrinted ? "         " : " lifted: ") + comp + "  [" + comp.GetHashCode() + "]");
				hasPrinted = true;
			}
		}

	}

}
