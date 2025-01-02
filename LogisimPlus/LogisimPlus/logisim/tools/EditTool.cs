// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using LogisimVersion = logisim.LogisimVersion;
	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Canvas = logisim.gui.main.Canvas;
	using Selection = logisim.gui.main.Selection;
	using SelectionActions = logisim.gui.main.SelectionActions;
	using Event = logisim.gui.main.Selection.Event;
	using Action = logisim.proj.Action;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class EditTool : Tool
	{
		private const int CACHE_MAX_SIZE = 32;
		private static readonly Location NULL_LOCATION = new Location(int.MinValue, int.MinValue);

		private class Listener : CircuitListener, Selection.Listener
		{
			private readonly EditTool outerInstance;

			public Listener(EditTool outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				if (@event.Action != CircuitEvent.ACTION_INVALIDATE)
				{
					outerInstance.lastX = -1;
					outerInstance.cache.clear();
					outerInstance.updateLocation(outerInstance.lastCanvas, outerInstance.lastRawX, outerInstance.lastRawY, outerInstance.lastMods);
				}
			}

			public virtual void selectionChanged(Selection.Event @event)
			{
				outerInstance.lastX = -1;
				outerInstance.cache.clear();
				outerInstance.updateLocation(outerInstance.lastCanvas, outerInstance.lastRawX, outerInstance.lastRawY, outerInstance.lastMods);
			}
		}

		private Listener listener;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private SelectTool select_Conflict;
		private WiringTool wiring;
		private Tool current;
		private LinkedHashMap<Location, bool> cache;
		private Canvas lastCanvas;
		private int lastRawX;
		private int lastRawY;
		private int lastX; // last coordinates where wiring was computed
		private int lastY;
		private int lastMods; // last modifiers for mouse event
		private Location wireLoc; // coordinates where to draw wiring indicator, if
		private int pressX; // last coordinate where mouse was pressed
		private int pressY; // (used to determine when a short wire has been clicked)

		public EditTool(SelectTool select, WiringTool wiring)
		{
			this.listener = new Listener(this);
			this.select_Conflict = select;
			this.wiring = wiring;
			this.current = select;
			this.cache = new LinkedHashMap<Location, bool>();
			this.lastX = -1;
			this.wireLoc = NULL_LOCATION;
			this.pressX = -1;
		}

		public override bool Equals(object other)
		{
			return other is EditTool;
		}

		public override int GetHashCode()
		{
			return typeof(EditTool).GetHashCode();
		}

		public override string Name
		{
			get
			{
				return "Edit Tool";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("editTool");
			}
		}

		public override string Description
		{
			get
			{
				return Strings.get("editToolDesc");
			}
		}

		public override AttributeSet AttributeSet
		{
			get
			{
				return select_Conflict.AttributeSet;
			}
			set
			{
				select_Conflict.AttributeSet = value;
			}
		}


		public override AttributeSet getAttributeSet(Canvas canvas)
		{
			return canvas.Selection.AttributeSet;
		}

		public override bool isAllDefaultValues(AttributeSet attrs, LogisimVersion ver)
		{
			return true;
		}

		public override void paintIcon(ComponentDrawContext c, int x, int y)
		{
			select_Conflict.paintIcon(c, x, y);
		}

		public override ISet<Component> getHiddenComponents(Canvas canvas)
		{
			return current.getHiddenComponents(canvas);
		}

		public override void draw(Canvas canvas, ComponentDrawContext context)
		{
			Location loc = wireLoc;
			if (loc != NULL_LOCATION && current != wiring)
			{
				int x = loc.X;
				int y = loc.Y;
				Graphics g = context.Graphics;
				g.setColor(Value.TRUE_COLOR);
				GraphicsUtil.switchToWidth(g, 2);
				g.drawOval(x - 5, y - 5, 10, 10);
				g.setColor(Color.BLACK);
				GraphicsUtil.switchToWidth(g, 1);
			}
			current.draw(canvas, context);
		}

		public override void select(Canvas canvas)
		{
			current = select_Conflict;
			lastCanvas = canvas;
			cache.clear();
			canvas.Circuit.addCircuitListener(listener);
			canvas.Selection.addListener(listener);
			select_Conflict.select(canvas);
		}

		public override void deselect(Canvas canvas)
		{
			current = select_Conflict;
			canvas.Selection.SuppressHandles = null;
			cache.clear();
			canvas.Circuit.removeCircuitListener(listener);
			canvas.Selection.removeListener(listener);
		}

		public override void mousePressed(Canvas canvas, Graphics g, MouseEvent e)
		{
			bool wire = updateLocation(canvas, e);
			Location oldWireLoc = wireLoc;
			wireLoc = NULL_LOCATION;
			lastX = int.MinValue;
			if (wire)
			{
				current = wiring;
				Selection sel = canvas.Selection;
				Circuit circ = canvas.Circuit;
				ICollection<Component> selected = sel.AnchoredComponents;
				List<Component> suppress = null;
				foreach (Wire w in circ.Wires)
				{
					if (selected.Contains(w))
					{
						if (w.contains(oldWireLoc))
						{
							if (suppress == null)
							{
								suppress = new List<Component>();
							}
							suppress.Add(w);
						}
					}
				}
				sel.SuppressHandles = suppress;
			}
			else
			{
				current = select_Conflict;
			}
			pressX = e.getX();
			pressY = e.getY();
			current.mousePressed(canvas, g, e);
		}

		public override void mouseDragged(Canvas canvas, Graphics g, MouseEvent e)
		{
			isClick(e);
			current.mouseDragged(canvas, g, e);
		}

		public override void mouseReleased(Canvas canvas, Graphics g, MouseEvent e)
		{
			bool click = isClick(e) && current == wiring;
			canvas.Selection.SuppressHandles = null;
			current.mouseReleased(canvas, g, e);
			if (click)
			{
				wiring.resetClick();
				select_Conflict.mousePressed(canvas, g, e);
				select_Conflict.mouseReleased(canvas, g, e);
			}
			current = select_Conflict;
			cache.clear();
			updateLocation(canvas, e);
		}

		public override void mouseEntered(Canvas canvas, Graphics g, MouseEvent e)
		{
			pressX = -1;
			current.mouseEntered(canvas, g, e);
			canvas.requestFocusInWindow();
		}

		public override void mouseExited(Canvas canvas, Graphics g, MouseEvent e)
		{
			pressX = -1;
			current.mouseExited(canvas, g, e);
		}

		public override void mouseMoved(Canvas canvas, Graphics g, MouseEvent e)
		{
			updateLocation(canvas, e);
			select_Conflict.mouseMoved(canvas, g, e);
		}

		private bool isClick(MouseEvent e)
		{
			int px = pressX;
			if (px < 0)
			{
				return false;
			}
			else
			{
				int dx = e.getX() - px;
				int dy = e.getY() - pressY;
				if (dx * dx + dy * dy <= 4)
				{
					return true;
				}
				else
				{
					pressX = -1;
					return false;
				}
			}
		}

		private bool updateLocation(Canvas canvas, MouseEvent e)
		{
			return updateLocation(canvas, e.getX(), e.getY(), e.getModifiersEx());
		}

		private bool updateLocation(Canvas canvas, KeyEvent e)
		{
			int x = lastRawX;
			if (x >= 0)
			{
				return updateLocation(canvas, x, lastRawY, e.getModifiersEx());
			}
			else
			{
				return false;
			}
		}

		private bool updateLocation(Canvas canvas, int mx, int my, int mods)
		{
			int snapx = Canvas.snapXToGrid(mx);
			int snapy = Canvas.snapYToGrid(my);
			int dx = mx - snapx;
			int dy = my - snapy;
			bool isEligible = dx * dx + dy * dy < 36;
			if ((mods & MouseEvent.ALT_DOWN_MASK) != 0)
			{
				isEligible = true;
			}
			if (!isEligible)
			{
				snapx = -1;
				snapy = -1;
			}
			bool modsSame = lastMods == mods;
			lastCanvas = canvas;
			lastRawX = mx;
			lastRawY = my;
			lastMods = mods;
			if (lastX == snapx && lastY == snapy && modsSame)
			{ // already computed
				return wireLoc != NULL_LOCATION;
			}
			else
			{
				Location snap = new Location(snapx, snapy);
				if (modsSame)
				{
					object o = cache.get(snap);
					if (o != null)
					{
						lastX = snapx;
						lastY = snapy;
						canvas.repaint();
						bool ret = ((bool?) o).Value;
						wireLoc = ret ? snap : NULL_LOCATION;
						return ret;
					}
				}
				else
				{
					cache.clear();
				}

				bool ret = isEligible && isWiringPoint(canvas, snap, mods);
				wireLoc = ret ? snap : NULL_LOCATION;
				cache.put(snap, Convert.ToBoolean(ret));
				int toRemove = cache.size() - CACHE_MAX_SIZE;
				IEnumerator<Location> it = cache.keySet().GetEnumerator();
				while (it.MoveNext() && toRemove > 0)
				{
					it.Current;
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
					it.remove();
					toRemove--;
				}

				lastX = snapx;
				lastY = snapy;
				canvas.repaint();
				return ret;
			}
		}

		private bool isWiringPoint(Canvas canvas, Location loc, int modsEx)
		{
			if (canvas == null)
			{
				return false;
			}

			bool wiring = (modsEx & MouseEvent.ALT_DOWN_MASK) == 0;
			bool select = !wiring;

			if (canvas != null && canvas.Selection != null)
			{
				ICollection<Component> sel = canvas.Selection.Components;
				if (sel != null)
				{
					foreach (Component c in sel)
					{
						if (c is Wire w && w.contains(loc) && !w.endsAt(loc))
						{
							return select;
						}

					}
				}
			}

			Circuit circ = canvas.Circuit;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Collection<? extends logisim.comp.Component> at = circ.getComponents(loc);
			ICollection<Component> at = circ.getComponents(loc);
			if (at != null && at.Count > 0)
			{
				return wiring;
			}

			foreach (Wire w in circ.Wires)
			{
				if (w.contains(loc))
				{
					return wiring;
				}
			}
			return select;
		}

		public override void keyTyped(Canvas canvas, KeyEvent e)
		{
			select_Conflict.keyTyped(canvas, e);
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			switch (e.getKeyCode())
			{
			case KeyEvent.VK_BACK_SPACE:
			case KeyEvent.VK_DELETE:
				if (!canvas.Selection.Empty)
				{
					Action act = SelectionActions.clear(canvas.Selection);
					canvas.Project.doAction(act);
					e.consume();
				}
				else
				{
					wiring.keyPressed(canvas, e);
				}
				break;
			case KeyEvent.VK_INSERT:
				Action act = SelectionActions.duplicate(canvas.Selection);
				canvas.Project.doAction(act);
				e.consume();
				break;
			case KeyEvent.VK_UP:
				if (e.getModifiersEx() == 0)
				{
					attemptReface(canvas, Direction.North, e);
				}
				else
				{
					select_Conflict.keyPressed(canvas, e);
				}
				break;
			case KeyEvent.VK_DOWN:
				if (e.getModifiersEx() == 0)
				{
					attemptReface(canvas, Direction.South, e);
				}
				else
				{
					select_Conflict.keyPressed(canvas, e);
				}
				break;
			case KeyEvent.VK_LEFT:
				if (e.getModifiersEx() == 0)
				{
					attemptReface(canvas, Direction.West, e);
				}
				else
				{
					select_Conflict.keyPressed(canvas, e);
				}
				break;
			case KeyEvent.VK_RIGHT:
				if (e.getModifiersEx() == 0)
				{
					attemptReface(canvas, Direction.East, e);
				}
				else
				{
					select_Conflict.keyPressed(canvas, e);
				}
				break;
			case KeyEvent.VK_ALT:
				updateLocation(canvas, e);
				e.consume();
				break;
			default:
				select_Conflict.keyPressed(canvas, e);
			break;
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
			switch (e.getKeyCode())
			{
			case KeyEvent.VK_ALT:
				updateLocation(canvas, e);
				e.consume();
				break;
			default:
				select_Conflict.keyReleased(canvas, e);
			break;
			}
		}

		private void attemptReface(Canvas canvas, in Direction facing, KeyEvent e)
		{
			if (e.getModifiersEx() == 0)
			{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final logisim.circuit.Circuit circuit = canvas.getCircuit();
				Circuit circuit = canvas.Circuit;
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final logisim.gui.main.Selection sel = canvas.getSelection();
				Selection sel = canvas.Selection;
				SetAttributeAction act = new SetAttributeAction(circuit, Strings.getter("selectionRefaceAction"));
				foreach (Component comp in sel.Components)
				{
					if (!(comp is Wire))
					{
						Attribute<Direction> attr = getFacingAttribute(comp);
						if (attr != null)
						{
							act.set(comp, attr, facing);
						}
					}
				}
				if (!act.Empty)
				{
					canvas.Project.doAction(act);
					e.consume();
				}
			}
		}

		private Attribute<Direction> getFacingAttribute(Component comp)
		{
			AttributeSet attrs = comp.AttributeSet;
			object key = ComponentFactory.FACING_ATTRIBUTE_KEY;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> a = (logisim.data.Attribute<?>) comp.getFactory().getFeature(key, attrs);
			Attribute<object> a = (Attribute<object>) comp.Factory.getFeature(key, attrs);
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<logisim.data.Direction> ret = (logisim.data.Attribute<logisim.data.Direction>) a;
			Attribute<Direction> ret = (Attribute<Direction>) a;
			return ret;
		}

		public override Cursor Cursor
		{
			get
			{
				return select_Conflict.Cursor;
			}
		}
	}

}
