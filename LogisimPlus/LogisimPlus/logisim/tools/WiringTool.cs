// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using CircuitMutation = logisim.circuit.CircuitMutation;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using Location = logisim.data.Location;
	using Canvas = logisim.gui.main.Canvas;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Action = logisim.proj.Action;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
	using Icons = logisim.util.Icons;
	using StringGetter = logisim.util.StringGetter;


	public class WiringTool : Tool
	{
		private static Cursor cursor = Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
		private static readonly Icon toolIcon = Icons.getIcon("wiring.gif");

		private const int HORIZONTAL = 1;
		private const int VERTICAL = 2;

		private bool exists = false;
		private bool inCanvas = false;
		private Location start = new Location(0, 0);
		private Location cur = new Location(0, 0);
		private bool hasDragged = false;
		private bool startShortening = false;
		private Wire shortening = null;
		private Action lastAction = null;
		private int direction = 0;

		public WiringTool()
		{
			base.select(null);
		}

		public override void select(Canvas canvas)
		{
			base.select(canvas);
			lastAction = null;
			reset();
		}

		private void reset()
		{
			exists = false;
			inCanvas = false;
			start = new Location(0, 0);
			cur = new Location(0, 0);
			startShortening = false;
			shortening = null;
			direction = 0;
		}

		public override bool Equals(object other)
		{
			return other is WiringTool;
		}

		public override int GetHashCode()
		{
			return typeof(WiringTool).GetHashCode();
		}

		public override string Name
		{
			get
			{
				return "Wiring Tool";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("wiringTool");
			}
		}

		public override string Description
		{
			get
			{
				return Strings.get("wiringToolDesc");
			}
		}

		private bool computeMove(int newX, int newY)
		{
			if (cur.X == newX && cur.Y == newY)
			{
				return false;
			}
			Location start = this.start;
			if (direction == 0)
			{
				if (newX != start.X)
				{
					direction = HORIZONTAL;
				}
				else if (newY != start.Y)
				{
					direction = VERTICAL;
				}
			}
			else if (direction == HORIZONTAL && newX == start.X)
			{
				if (newY == start.Y)
				{
					direction = 0;
				}
				else
				{
					direction = VERTICAL;
				}
			}
			else if (direction == VERTICAL && newY == start.Y)
			{
				if (newX == start.X)
				{
					direction = 0;
				}
				else
				{
					direction = HORIZONTAL;
				}
			}
			return true;
		}

		public override HashSet<Component> getHiddenComponents(Canvas canvas)
		{
			Component shorten = willShorten(start, cur);
			if (shorten != null)
			{
				return Collections.singleton(shorten);
			}
			else
			{
				return null;
			}
		}

		public override void draw(Canvas canvas, ComponentDrawContext context)
		{
			JGraphics g = context.Graphics;
			if (exists)
			{
				Location e0 = start;
				Location e1 = cur;
				Wire shortenBefore = willShorten(start, cur);
				if (shortenBefore != null)
				{
					Wire shorten = getShortenResult(shortenBefore, start, cur);
					if (shorten == null)
					{
						return;
					}
					else
					{
						e0 = shorten.End0;
						e1 = shorten.End1;
					}
				}
				int x0 = e0.X;
				int y0 = e0.Y;
				int x1 = e1.X;
				int y1 = e1.Y;

				g.setColor(Color.Black);
				JGraphicsUtil.switchToWidth(g, 3);
				if (direction == HORIZONTAL)
				{
					if (x0 != x1)
					{
						g.drawLine(x0, y0, x1, y0);
					}
					if (y0 != y1)
					{
						g.drawLine(x1, y0, x1, y1);
					}
				}
				else if (direction == VERTICAL)
				{
					if (y0 != y1)
					{
						g.drawLine(x0, y0, x0, y1);
					}
					if (x0 != x1)
					{
						g.drawLine(x0, y1, x1, y1);
					}
				}
			}
			else if (AppPreferences.ADD_SHOW_GHOSTS.Boolean && inCanvas)
			{
				g.setColor(Color.Gray);
				g.fillOval(cur.X - 2, cur.Y - 2, 5, 5);
			}
		}

		public override void mouseEntered(Canvas canvas, JGraphics g, MouseEvent e)
		{
			inCanvas = true;
			canvas.Project.repaintCanvas();
		}

		public override void mouseExited(Canvas canvas, JGraphics g, MouseEvent e)
		{
			inCanvas = false;
			canvas.Project.repaintCanvas();
		}

		public override void mouseMoved(Canvas canvas, JGraphics g, MouseEvent e)
		{
			if (exists)
			{
				mouseDragged(canvas, g, e);
			}
			else
			{
				Canvas.snapToGrid(e);
				inCanvas = true;
				int curX = e.getX();
				int curY = e.getY();
				if (cur.X != curX || cur.Y != curY)
				{
					cur = new Location(curX, curY);
				}
				canvas.Project.repaintCanvas();
			}
		}

		public override void mousePressed(Canvas canvas, JGraphics g, MouseEvent e)
		{
			if (!canvas.Project.LogisimFile.contains(canvas.Circuit))
			{
				exists = false;
				canvas.ErrorMessage = Strings.getter("cannotModifyError");
				return;
			}

			if (exists)
			{
				mouseDragged(canvas, g, e);
			}
			else
			{
				Canvas.snapToGrid(e);
				start = new Location(e.getX(), e.getY());
				cur = start;
				exists = true;
				hasDragged = false;

				startShortening = canvas.Circuit.getWires(start).Count > 0;
				shortening = null;

				base.mousePressed(canvas, g, e);
				canvas.Project.repaintCanvas();
			}
		}

		public override void mouseDragged(Canvas canvas, JGraphics g, MouseEvent e)
		{
			if (exists)
			{
				Canvas.snapToGrid(e);
				int curX = e.getX();
				int curY = e.getY();
				if (!computeMove(curX, curY))
				{
					return;
				}
				hasDragged = true;

				Rectangle rect = new Rectangle();
				rect.add(start.X, start.Y);
				rect.add(cur.X, cur.Y);
				rect.add(curX, curY);
				rect.grow(3, 3);

				cur = new Location(curX, curY);
				base.mouseDragged(canvas, g, e);

				Wire shorten = null;
				if (startShortening)
				{
					foreach (Wire w in canvas.Circuit.getWires(start))
					{
						if (w.contains(cur))
						{
							shorten = w;
							break;
						}
					}
				}
				if (shorten == null)
				{
					foreach (Wire w in canvas.Circuit.getWires(cur))
					{
						if (w.contains(start))
						{
							shorten = w;
							break;
						}
					}
				}
				shortening = shorten;

				canvas.repaint(rect);
			}
		}

		internal virtual void resetClick()
		{
			exists = false;
		}

		public override void mouseReleased(Canvas canvas, JGraphics g, MouseEvent e)
		{
			if (!exists)
			{
				return;
			}

			Canvas.snapToGrid(e);
			int curX = e.getX();
			int curY = e.getY();
			if (computeMove(curX, curY))
			{
				cur = new Location(curX, curY);
			}
			if (hasDragged)
			{
				exists = false;
				base.mouseReleased(canvas, g, e);

				List<Wire> ws = new List<Wire>(2);
				if (cur.Y == start.Y || cur.X == start.X)
				{
					Wire w = Wire.create(cur, start);
					w = checkForRepairs(canvas, w, w.End0);
					w = checkForRepairs(canvas, w, w.End1);
					if (performShortening(canvas, start, cur))
					{
						return;
					}
					if (w.Length > 0)
					{
						ws.Add(w);
					}
				}
				else
				{
					Location m;
					if (direction == HORIZONTAL)
					{
						m = new Location(cur.X, start.Y);
					}
					else
					{
						m = new Location(start.X, cur.Y);
					}
					Wire w0 = Wire.create(start, m);
					Wire w1 = Wire.create(m, cur);
					w0 = checkForRepairs(canvas, w0, start);
					w1 = checkForRepairs(canvas, w1, cur);
					if (w0.Length > 0)
					{
						ws.Add(w0);
					}
					if (w1.Length > 0)
					{
						ws.Add(w1);
					}
				}
				if (ws.Count > 0)
				{
					CircuitMutation mutation = new CircuitMutation(canvas.Circuit);
					mutation.addAll(ws);
					StringGetter desc;
					if (ws.Count == 1)
					{
						desc = Strings.getter("addWireAction");
					}
					else
					{
						desc = Strings.getter("addWiresAction");
					}
					Action act = mutation.toAction(desc);
					canvas.Project.doAction(act);
					lastAction = act;
				}
			}
		}

		private Wire checkForRepairs(Canvas canvas, Wire w, Location end)
		{
			if (w.Length <= 10)
			{
				return w; // don't repair a short wire to nothing
			}
			if (canvas.Circuit.getNonWires(end).Count > 0)
			{
				return w;
			}

			int delta = (end.Equals(w.End0) ? 10 : -10);
			Location cand;
			if (w.Vertical)
			{
				cand = new Location(end.X, end.Y + delta);
			}
			else
			{
				cand = new Location(end.X + delta, end.Y);
			}

			foreach (Component comp in canvas.Circuit.getNonWires(cand))
			{
				if (comp.Bounds.contains(end))
				{
					WireRepair repair = (WireRepair) comp.getFeature(typeof(WireRepair));
					if (repair != null && repair.shouldRepairWire(new WireRepairData(w, cand)))
					{
						w = Wire.create(w.getOtherEnd(end), cand);
						canvas.repaint(end.X - 13, end.Y - 13, 26, 26);
						return w;
					}
				}
			}
			return w;
		}

		private Wire willShorten(Location drag0, Location drag1)
		{
			Wire shorten = shortening;
			if (shorten == null)
			{
				return null;
			}
			else if (shorten.endsAt(drag0) || shorten.endsAt(drag1))
			{
				return shorten;
			}
			else
			{
				return null;
			}
		}

		private Wire getShortenResult(Wire shorten, Location drag0, Location drag1)
		{
			if (shorten == null)
			{
				return null;
			}
			else
			{
				Location e0;
				Location e1;
				if (shorten.endsAt(drag0))
				{
					e0 = drag1;
					e1 = shorten.getOtherEnd(drag0);
				}
				else if (shorten.endsAt(drag1))
				{
					e0 = drag0;
					e1 = shorten.getOtherEnd(drag1);
				}
				else
				{
					return null;
				}
				return e0.Equals(e1) ? null : Wire.create(e0, e1);
			}
		}

		private bool performShortening(Canvas canvas, Location drag0, Location drag1)
		{
			Wire shorten = willShorten(drag0, drag1);
			if (shorten == null)
			{
				return false;
			}
			else
			{
				CircuitMutation xn = new CircuitMutation(canvas.Circuit);
				StringGetter actName;
				Wire result = getShortenResult(shorten, drag0, drag1);
				if (result == null)
				{
					xn.remove(shorten);
					actName = Strings.getter("removeComponentAction", shorten.Factory.DisplayGetter);
				}
				else
				{
					xn.replace(shorten, result);
					actName = Strings.getter("shortenWireAction");
				}
				canvas.Project.doAction(xn.toAction(actName));
				return true;
			}
		}

		public override void keyPressed(Canvas canvas, KeyEvent @event)
		{
			switch (@event.getKeyCode())
			{
			case KeyEvent.VK_BACK_SPACE:
				if (lastAction != null && canvas.Project.LastAction == lastAction)
				{
					canvas.Project.undoAction();
					lastAction = null;
				}
			break;
			}
		}

		public override void paintIcon(ComponentDrawContext c, int x, int y)
		{
			JGraphics g = c.Graphics;
			if (toolIcon != null)
			{
				toolIcon.paintIcon(c.Destination, g, x + 2, y + 2);
			}
			else
			{
				g.setColor(Color.Black);
				g.drawLine(x + 3, y + 13, x + 17, y + 7);
				g.fillOval(x + 1, y + 11, 5, 5);
				g.fillOval(x + 15, y + 5, 5, 5);
			}
		}

		public override Cursor Cursor
		{
			get
			{
				return cursor;
			}
		}
	}

}
