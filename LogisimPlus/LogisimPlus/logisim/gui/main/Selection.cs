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
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using ReplacementMap = logisim.circuit.ReplacementMap;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using CustomHandles = logisim.tools.CustomHandles;

	public class Selection : SelectionBase
	{
		public class Event
		{
			internal object source;

			internal Event(object source)
			{
				this.source = source;
			}

			public virtual object Source
			{
				get
				{
					return source;
				}
			}
		}

		public interface Listener
		{
			void selectionChanged(Selection.Event @event);
		}

		private class MyListener : ProjectListener, CircuitListener
		{
			private readonly Selection outerInstance;

			internal WeakHashMap<Action, SelectionSave> savedSelections;

			internal MyListener(Selection outerInstance)
			{
				this.outerInstance = outerInstance;
				savedSelections = new WeakHashMap<Action, SelectionSave>();
			}

			public virtual void projectChanged(ProjectEvent @event)
			{
				int type = @event.Action;
				if (type == ProjectEvent.ACTION_START)
				{
					SelectionSave save = SelectionSave.create(outerInstance);
					savedSelections.put((Action) @event.Data, save);
				}
				else if (type == ProjectEvent.ACTION_COMPLETE)
				{
					SelectionSave save = savedSelections.get(@event.Data);
					if (save != null && save.isSame(outerInstance))
					{
						savedSelections.remove(@event.Data);
					}
				}
				else if (type == ProjectEvent.ACTION_MERGE)
				{
					SelectionSave save = savedSelections.get(@event.OldData);
					savedSelections.put((Action) @event.Data, save);
				}
				else if (type == ProjectEvent.UNDO_COMPLETE)
				{
					Circuit circ = @event.Project.CurrentCircuit;
					Action act = (Action) @event.Data;
					SelectionSave save = savedSelections.get(act);
					if (save != null)
					{
						outerInstance.lifted.Clear();
						outerInstance.selected.Clear();
						for (int i = 0; i < 2; i++)
						{
							Component[] cs;
							if (i == 0)
							{
								cs = save.FloatingComponents;
							}
							else
							{
								cs = save.AnchoredComponents;
							}

							if (cs != null)
							{
								foreach (Component c in cs)
								{
									if (circ.contains(c))
									{
										outerInstance.selected.Add(c);
									}
									else
									{
										outerInstance.lifted.Add(c);
									}
								}
							}
						}
						outerInstance.fireSelectionChanged();
					}
				}
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				if (@event.Action == CircuitEvent.TRANSACTION_DONE)
				{
					Circuit circuit = @event.Circuit;
					ReplacementMap repl = @event.Result.getReplacementMap(circuit);
					bool change = false;

					List<Component> oldAnchored;
					oldAnchored = new List<Component>(outerInstance.Components);
					foreach (Component comp in oldAnchored)
					{
						ICollection<Component> replacedBy = repl.get(comp);
						if (replacedBy != null)
						{
							change = true;
							outerInstance.selected.Remove(comp);
							outerInstance.lifted.Remove(comp);
							foreach (Component add in replacedBy)
							{
								if (circuit.contains(add))
								{
									outerInstance.selected.Add(add);
								}
								else
								{
									outerInstance.lifted.Add(add);
								}
							}
						}
					}

					if (change)
					{
						outerInstance.fireSelectionChanged();
					}
				}
			}
		}

		private MyListener myListener;
		private bool isVisible = true;
		private SelectionAttributes attrs;

		public Selection(Project proj, Canvas canvas) : base(proj)
		{

			myListener = new MyListener(this);
			attrs = new SelectionAttributes(canvas, this);
			proj.addProjectListener(myListener);
			proj.addCircuitListener(myListener);
		}

		//
		// query methods
		//
		public virtual bool Empty
		{
			get
			{
				return selected.Count == 0 && lifted.Count == 0;
			}
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
		}

		public override bool Equals(object other)
		{
			if (!(other is Selection))
			{
				return false;
			}
			Selection otherSelection = (Selection) other;
			return this.selected.Equals(otherSelection.selected) && this.lifted.Equals(otherSelection.lifted);
		}

		public virtual HashSet<Component> Components
		{
			get
			{
				return unionSet;
			}
		}

		public virtual ICollection<Component> AnchoredComponents
		{
			get
			{
				return selected;
			}
		}

		public virtual ICollection<Component> FloatingComponents
		{
			get
			{
				return lifted;
			}
		}

		public virtual ICollection<Component> getComponentsContaining(Location query)
		{
			HashSet<Component> ret = new HashSet<Component>();
			foreach (Component comp in unionSet)
			{
				if (comp.contains(query))
				{
					ret.Add(comp);
				}
			}
			return ret;
		}

		public virtual ICollection<Component> getComponentsContaining(Location query, JGraphics g)
		{
			HashSet<Component> ret = new HashSet<Component>();
			foreach (Component comp in unionSet)
			{
				if (comp.contains(query, g))
				{
					ret.Add(comp);
				}
			}
			return ret;
		}

		public virtual ICollection<Component> getComponentsWithin(Bounds bds)
		{
			HashSet<Component> ret = new HashSet<Component>();
			foreach (Component comp in unionSet)
			{
				if (bds.contains(comp.Bounds))
				{
					ret.Add(comp);
				}
			}
			return ret;
		}

		public virtual ICollection<Component> getComponentsWithin(Bounds bds, JGraphics g)
		{
			HashSet<Component> ret = new HashSet<Component>();
			foreach (Component comp in unionSet)
			{
				if (bds.contains(comp.getBounds(g)))
				{
					ret.Add(comp);
				}
			}
			return ret;
		}

		public virtual bool contains(Component comp)
		{
			return unionSet.Contains(comp);
		}

		//
		// JGraphics methods
		//
		public virtual void draw(ComponentDrawContext context, HashSet<Component> hidden)
		{
			JGraphics g = context.Graphics;

			foreach (Component c in lifted)
			{
				if (!hidden.Contains(c))
				{
					Location loc = c.Location;

					JGraphics g_new = g.create();
					context.Graphics = g_new;
					c.Factory.drawGhost(context, Color.Gray, loc.X, loc.Y, c.AttributeSet);
					g_new.dispose();
				}
			}

			foreach (Component comp in unionSet)
			{
				if (!suppressHandles.Contains(comp) && !hidden.Contains(comp))
				{
					JGraphics g_new = g.create();
					context.Graphics = g_new;
					CustomHandles handler = (CustomHandles) comp.getFeature(typeof(CustomHandles));
					if (handler == null)
					{
						context.drawHandles(comp);
					}
					else
					{
						handler.drawHandles(context);
					}
					g_new.dispose();
				}
			}

			context.Graphics = g;
		}

		public virtual void drawGhostsShifted(ComponentDrawContext context, int dx, int dy)
		{
			if (shouldSnap())
			{
				dx = Canvas.snapXToGrid(dx);
				dy = Canvas.snapYToGrid(dy);
			}
			JGraphics g = context.Graphics;
			foreach (Component comp in unionSet)
			{
				AttributeSet attrs = comp.AttributeSet;
				Location loc = comp.Location;
				int x = loc.X + dx;
				int y = loc.Y + dy;
				context.Graphics = g.create();
				comp.Factory.drawGhost(context, Color.Gray, x, y, attrs);
				context.Graphics.dispose();
			}
			context.Graphics = g;
		}

		public override void print()
		{
			Console.Error.WriteLine(" isVisible: " + isVisible); // OK
			base.print();
		}

	}

}
