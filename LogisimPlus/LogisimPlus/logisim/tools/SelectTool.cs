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
	using ReplacementMap = logisim.circuit.ReplacementMap;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Canvas = logisim.gui.main.Canvas;
	using Selection = logisim.gui.main.Selection;
	using SelectionActions = logisim.gui.main.SelectionActions;
	using Event = logisim.gui.main.Selection.Event;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using KeyConfigurationEvent = logisim.tools.key.KeyConfigurationEvent;
	using KeyConfigurator = logisim.tools.key.KeyConfigurator;
	using KeyConfigurationResult = logisim.tools.key.KeyConfigurationResult;
	using MoveResult = logisim.tools.move.MoveResult;
	using MoveGesture = logisim.tools.move.MoveGesture;
	using MoveRequestListener = logisim.tools.move.MoveRequestListener;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using Icons = logisim.util.Icons;
	using StringGetter = logisim.util.StringGetter;


	public class SelectTool : Tool
	{
		private static readonly Cursor selectCursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR);
		private static readonly Cursor rectSelectCursor = Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
		private static readonly Cursor moveCursor = Cursor.getPredefinedCursor(Cursor.MOVE_CURSOR);

		private const int IDLE = 0;
		private const int MOVING = 1;
		private const int RECT_SELECT = 2;
		private static readonly Icon toolIcon = Icons.getIcon("select.gif");

		private static readonly Color COLOR_UNMATCHED = new Color(192, 0, 0);
		private static readonly Color COLOR_COMPUTING = new Color(96, 192, 96);
		private static readonly Color COLOR_RECT_SELECT = new Color(0, 64, 128, 255);
		private static readonly Color BACKGROUND_RECT_SELECT = new Color(192, 192, 255, 192);

		private class MoveRequestHandler : MoveRequestListener
		{
			internal Canvas canvas;

			internal MoveRequestHandler(Canvas canvas)
			{
				this.canvas = canvas;
			}

			public virtual void requestSatisfied(MoveGesture gesture, int dx, int dy)
			{
				clearCanvasMessage(canvas, dx, dy);
			}
		}

		private class Listener : Selection.Listener
		{
			private readonly SelectTool outerInstance;

			public Listener(SelectTool outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void selectionChanged(Selection.Event @event)
			{
				outerInstance.keyHandlers = null;
			}
		}

		private Location start;
		private int state;
		private int curDx;
		private int curDy;
		private bool drawConnections;
		private MoveGesture moveGesture;
		private Dictionary<Component, KeyConfigurator> keyHandlers;
		private HashSet<Selection> selectionsAdded;
		private Listener selListener;

		public SelectTool()
		{
			start = null;
			state = IDLE;
			selectionsAdded = new HashSet<Selection>();
			selListener = new Listener(this);
			keyHandlers = null;
		}

		public override bool Equals(object other)
		{
			return other is SelectTool;
		}

		public override int GetHashCode()
		{
			return typeof(SelectTool).GetHashCode();
		}

		public override string Name
		{
			get
			{
				return "Select Tool";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("selectTool");
			}
		}

		public override string Description
		{
			get
			{
				return Strings.get("selectToolDesc");
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

		public override void draw(Canvas canvas, ComponentDrawContext context)
		{
			Project proj = canvas.Project;
			int dx = curDx;
			int dy = curDy;
			if (state == MOVING)
			{
				proj.Selection.drawGhostsShifted(context, dx, dy);

				MoveGesture gesture = moveGesture;
				if (gesture != null && drawConnections && (dx != 0 || dy != 0))
				{
					MoveResult result = gesture.findResult(dx, dy);
					if (result != null)
					{
						ICollection<Wire> wiresToAdd = result.WiresToAdd;
						Graphics g = context.Graphics;
						GraphicsUtil.switchToWidth(g, 3);
						g.setColor(Color.GRAY);
						foreach (Wire w in wiresToAdd)
						{
							Location loc0 = w.End0;
							Location loc1 = w.End1;
							g.drawLine(loc0.X, loc0.Y, loc1.X, loc1.Y);
						}
						GraphicsUtil.switchToWidth(g, 1);
						g.setColor(COLOR_UNMATCHED);
						foreach (Location conn in result.UnconnectedLocations)
						{
							int connX = conn.X;
							int connY = conn.Y;
							g.fillOval(connX - 3, connY - 3, 6, 6);
							g.fillOval(connX + dx - 3, connY + dy - 3, 6, 6);
						}
					}
				}
			}
			else if (state == RECT_SELECT)
			{
				int left = start.X;
				int right = left + dx;
				if (left > right)
				{
					int i = left;
					left = right;
					right = i;
				}
				int top = start.Y;
				int bot = top + dy;
				if (top > bot)
				{
					int i = top;
					top = bot;
					bot = i;
				}

				Graphics gBase = context.Graphics;
				int w = right - left - 1;
				int h = bot - top - 1;
				if (w > 2 && h > 2)
				{
					gBase.setColor(BACKGROUND_RECT_SELECT);
					gBase.fillRect(left + 1, top + 1, w - 1, h - 1);
				}

				Circuit circ = canvas.Circuit;
				Bounds bds = Bounds.create(left, top, right - left, bot - top);
				foreach (Component c in circ.getAllWithin(bds))
				{
					Location cloc = c.Location;
					Graphics gDup = gBase.create();
					context.Graphics = gDup;
					c.Factory.drawGhost(context, COLOR_RECT_SELECT, cloc.X, cloc.Y, c.AttributeSet);
					gDup.dispose();
				}

				gBase.setColor(COLOR_RECT_SELECT);
				GraphicsUtil.switchToWidth(gBase, 2);
				if (w < 0)
				{
					w = 0;
				}
				if (h < 0)
				{
					h = 0;
				}
				gBase.drawRect(left, top, w, h);
			}
		}

		public override void select(Canvas canvas)
		{
			Selection sel = canvas.Selection;
			if (!selectionsAdded.Contains(sel))
			{
				sel.addListener(selListener);
			}
		}

		public override void deselect(Canvas canvas)
		{
			moveGesture = null;
		}

		public override void mouseEntered(Canvas canvas, Graphics g, MouseEvent e)
		{
			canvas.requestFocusInWindow();
		}

		public override void mousePressed(Canvas canvas, Graphics g, MouseEvent e)
		{
			Project proj = canvas.Project;
			Selection sel = proj.Selection;
			Circuit circuit = canvas.Circuit;
			start = new Location(e.getX(), e.getY());
			curDx = 0;
			curDy = 0;
			moveGesture = null;

			// if the user clicks into the selection,
			// selection is being modified
			ICollection<Component> in_sel = sel.getComponentsContaining(start, g);
			if (in_sel.Count > 0)
			{
				if ((e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) == 0)
				{
					setState(proj, MOVING);
					proj.repaintCanvas();
					return;
				}
				else
				{
					Action act = SelectionActions.drop(sel, in_sel);
					if (act != null)
					{
						proj.doAction(act);
					}
				}
			}

			// if the user clicks into a component outside selection, user
			// wants to add/reset selection
			ICollection<Component> clicked = circuit.getAllContaining(start, g);
			if (clicked.Count > 0)
			{
				if ((e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) == 0)
				{
					if (sel.getComponentsContaining(start).Count == 0)
					{
						Action act = SelectionActions.dropAll(sel);
						if (act != null)
						{
							proj.doAction(act);
						}
					}
				}
				foreach (Component comp in clicked)
				{
					if (!in_sel.Contains(comp))
					{
						sel.add(comp);
					}
				}
				setState(proj, MOVING);
				proj.repaintCanvas();
				return;
			}

			// The user clicked on the background. This is a rectangular
			// selection (maybe with the shift key down).
			if ((e.getModifiersEx() & InputEvent.SHIFT_DOWN_MASK) == 0)
			{
				Action act = SelectionActions.dropAll(sel);
				if (act != null)
				{
					proj.doAction(act);
				}
			}
			setState(proj, RECT_SELECT);
			proj.repaintCanvas();
		}

		public override void mouseDragged(Canvas canvas, Graphics g, MouseEvent e)
		{
			if (state == MOVING)
			{
				Project proj = canvas.Project;
				computeDxDy(proj, e, g);
				handleMoveDrag(canvas, curDx, curDy, e.getModifiersEx());
			}
			else if (state == RECT_SELECT)
			{
				Project proj = canvas.Project;
				curDx = e.getX() - start.X;
				curDy = e.getY() - start.Y;
				proj.repaintCanvas();
			}
		}

		private void handleMoveDrag(Canvas canvas, int dx, int dy, int modsEx)
		{
			bool connect = shouldConnect(canvas, modsEx);
			drawConnections = connect;
			if (connect)
			{
				MoveGesture gesture = moveGesture;
				if (gesture == null)
				{
					gesture = new MoveGesture(new MoveRequestHandler(canvas), canvas.Circuit, canvas.Selection.AnchoredComponents);
					moveGesture = gesture;
				}
				if (dx != 0 || dy != 0)
				{
					bool queued = gesture.enqueueRequest(dx, dy);
					if (queued)
					{
						canvas.setErrorMessage(new ComputingMessage(dx, dy), COLOR_COMPUTING);
						// maybe CPU scheduled led the request to be satisfied
						// just before the "if(queued)" statement. In any case, it
						// doesn't hurt to check to ensure the message belongs.
						if (gesture.findResult(dx, dy) != null)
						{
							clearCanvasMessage(canvas, dx, dy);
						}
					}
				}
			}
			canvas.repaint();
		}

		private bool shouldConnect(Canvas canvas, int modsEx)
		{
			bool shiftReleased = (modsEx & MouseEvent.SHIFT_DOWN_MASK) == 0;
			bool dflt = AppPreferences.MOVE_KEEP_CONNECT.Boolean;
			if (shiftReleased)
			{
				return dflt;
			}
			else
			{
				return !dflt;
			}
		}

		public override void mouseReleased(Canvas canvas, Graphics g, MouseEvent e)
		{
			Project proj = canvas.Project;
			if (state == MOVING)
			{
				setState(proj, IDLE);
				computeDxDy(proj, e, g);
				int dx = curDx;
				int dy = curDy;
				if (dx != 0 || dy != 0)
				{
					if (!proj.LogisimFile.contains(canvas.Circuit))
					{
						canvas.ErrorMessage = Strings.getter("cannotModifyError");
					}
					else if (proj.Selection.hasConflictWhenMoved(dx, dy))
					{
						canvas.ErrorMessage = Strings.getter("exclusiveError");
					}
					else
					{
						bool connect = shouldConnect(canvas, e.getModifiersEx());
						drawConnections = false;
						ReplacementMap repl;
						if (connect)
						{
							MoveGesture gesture = moveGesture;
							if (gesture == null)
							{
								gesture = new MoveGesture(new MoveRequestHandler(canvas), canvas.Circuit, canvas.Selection.AnchoredComponents);
							}
							canvas.setErrorMessage(new ComputingMessage(dx, dy), COLOR_COMPUTING);
							MoveResult result = gesture.forceRequest(dx, dy);
							clearCanvasMessage(canvas, dx, dy);
							repl = result.ReplacementMap;
						}
						else
						{
							repl = null;
						}
						Selection sel = proj.Selection;
						proj.doAction(SelectionActions.translate(sel, dx, dy, repl));
					}
				}
				moveGesture = null;
				proj.repaintCanvas();
			}
			else if (state == RECT_SELECT)
			{
				Bounds bds = Bounds.create(start).add(start.X + curDx, start.Y + curDy);
				Circuit circuit = canvas.Circuit;
				Selection sel = proj.Selection;
				ICollection<Component> in_sel = sel.getComponentsWithin(bds, g);
				foreach (Component comp in circuit.getAllWithin(bds, g))
				{
					if (!in_sel.Contains(comp))
					{
						sel.add(comp);
					}
				}
				Action act = SelectionActions.drop(sel, in_sel);
				if (act != null)
				{
					proj.doAction(act);
				}
				setState(proj, IDLE);
				proj.repaintCanvas();
			}
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			if (state == MOVING && e.getKeyCode() == KeyEvent.VK_SHIFT)
			{
				handleMoveDrag(canvas, curDx, curDy, e.getModifiersEx());
			}
			else
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
					break;
				default:
					processKeyEvent(canvas, e, KeyConfigurationEvent.KEY_PRESSED);
				break;
				}
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
			if (state == MOVING && e.getKeyCode() == KeyEvent.VK_SHIFT)
			{
				handleMoveDrag(canvas, curDx, curDy, e.getModifiersEx());
			}
			else
			{
				processKeyEvent(canvas, e, KeyConfigurationEvent.KEY_RELEASED);
			}
		}

		public override void keyTyped(Canvas canvas, KeyEvent e)
		{
			processKeyEvent(canvas, e, KeyConfigurationEvent.KEY_TYPED);
		}

		private void processKeyEvent(Canvas canvas, KeyEvent e, int type)
		{
			Dictionary<Component, KeyConfigurator> handlers = keyHandlers;
			if (handlers == null)
			{
				handlers = new Dictionary<Component, KeyConfigurator>();
				Selection sel = canvas.Selection;
				foreach (Component comp in sel.Components)
				{
					ComponentFactory factory = comp.Factory;
					AttributeSet attrs = comp.AttributeSet;
					object handler = factory.getFeature(typeof(KeyConfigurator), attrs);
					if (handler != null)
					{
						KeyConfigurator @base = (KeyConfigurator) handler;
						handlers[comp] = @base.clone();
					}
				}
				keyHandlers = handlers;
			}

			if (handlers.Count > 0)
			{
				bool consume = false;
				List<KeyConfigurationResult> results;
				results = new List<KeyConfigurationResult>();
				foreach (KeyValuePair<Component, KeyConfigurator> entry in handlers.SetOfKeyValuePairs())
				{
					Component comp = entry.Key;
					KeyConfigurator handler = entry.Value;
					KeyConfigurationEvent @event = new KeyConfigurationEvent(type, comp.AttributeSet, e, comp);
					KeyConfigurationResult result = handler.keyEventReceived(@event);
					consume |= @event.Consumed;
					if (result != null)
					{
						results.Add(result);
					}
				}
				if (consume)
				{
					e.consume();
				}
				if (results.Count > 0)
				{
					SetAttributeAction act = new SetAttributeAction(canvas.Circuit, Strings.getter("changeComponentAttributesAction"));
					foreach (KeyConfigurationResult result in results)
					{
						Component comp = (Component) result.Event.Data;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Map<logisim.data.Attribute<?>, Object> newValues = result.getAttributeValues();
						IDictionary<Attribute<object>, object> newValues = result.AttributeValues;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (java.util.Map.Entry<logisim.data.Attribute<?>, Object> entry : newValues.entrySet())
						foreach (KeyValuePair<Attribute<object>, object> entry in newValues.SetOfKeyValuePairs())
						{
							act.set(comp, entry.Key, entry.Value);
						}
					}
					if (!act.Empty)
					{
						canvas.Project.doAction(act);
					}
				}
			}
		}

		private void computeDxDy(Project proj, MouseEvent e, Graphics g)
		{
			Bounds bds = proj.Selection.getBounds(g);
			int dx;
			int dy;
			if (bds == Bounds.EMPTY_BOUNDS)
			{
				dx = e.getX() - start.X;
				dy = e.getY() - start.Y;
			}
			else
			{
				dx = Math.Max(e.getX() - start.X, -bds.X);
				dy = Math.Max(e.getY() - start.Y, -bds.Y);
			}

			Selection sel = proj.Selection;
			if (sel.shouldSnap())
			{
				dx = Canvas.snapXToGrid(dx);
				dy = Canvas.snapYToGrid(dy);
			}
			curDx = dx;
			curDy = dy;
		}

		public override void paintIcon(ComponentDrawContext c, int x, int y)
		{
			Graphics g = c.Graphics;
			if (toolIcon != null)
			{
				toolIcon.paintIcon(c.Destination, g, x + 2, y + 2);
			}
			else
			{
				int[] xp = new int[] {x + 5, x + 5, x + 9, x + 12, x + 14, x + 11, x + 16};
				int[] yp = new int[] {y, y + 17, y + 12, y + 18, y + 18, y + 12, y + 12};
				g.setColor(Color.black);
				g.fillPolygon(xp, yp, xp.Length);
			}
		}

		public override Cursor Cursor
		{
			get
			{
				return state == IDLE ? selectCursor : (state == RECT_SELECT ? rectSelectCursor : moveCursor);
			}
		}

		public override ISet<Component> getHiddenComponents(Canvas canvas)
		{
			if (state == MOVING)
			{
				int dx = curDx;
				int dy = curDy;
				if (dx == 0 && dy == 0)
				{
					return null;
				}

				ISet<Component> sel = canvas.Selection.Components;
				MoveGesture gesture = moveGesture;
				if (gesture != null && drawConnections)
				{
					MoveResult result = gesture.findResult(dx, dy);
					if (result != null)
					{
						HashSet<Component> ret = new HashSet<Component>(sel);
						ret.addAll(result.ReplacementMap.Removals);
						return ret;
					}
				}
				return sel;
			}
			else
			{
				return null;
			}
		}

		private void setState(Project proj, int new_state)
		{
			if (state == new_state)
			{
				return; // do nothing if state not new
			}

			state = new_state;
			proj.Frame.getCanvas().setCursor(Cursor);
		}

		private static void clearCanvasMessage(Canvas canvas, int dx, int dy)
		{
			object getter = canvas.ErrorMessage;
			if (getter is ComputingMessage)
			{
				ComputingMessage msg = (ComputingMessage) getter;
				if (msg.dx == dx && msg.dy == dy)
				{
					canvas.ErrorMessage = null;
					canvas.repaint();
				}
			}
		}

		private class ComputingMessage : StringGetter
		{
			internal int dx;
			internal int dy;

			public ComputingMessage(int dx, int dy)
			{
				this.dx = dx;
				this.dy = dy;
			}

			public virtual string get()
			{
				return Strings.get("moveWorkingMsg");
			}
		}
	}

}
