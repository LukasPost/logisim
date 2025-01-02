// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.tools
{

	using ModelAddAction = draw.actions.ModelAddAction;
	using Canvas = draw.canvas.Canvas;
	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using DrawAttr = draw.shapes.DrawAttr;
	using LineUtil = draw.shapes.LineUtil;
	using Poly = draw.shapes.Poly;
	using logisim.data;
	using Location = logisim.data.Location;
	using Icons = logisim.util.Icons;

	public class PolyTool : AbstractTool
	{
		// how close we need to be to the start point to count as "closing the loop"
		private const int CLOSE_TOLERANCE = 2;

		private bool closed; // whether we are drawing polygons or polylines
		private DrawingAttributeSet attrs;
		private bool active;
		private List<Location> locations;
		private bool mouseDown;
		private int lastMouseX;
		private int lastMouseY;

		public PolyTool(bool closed, DrawingAttributeSet attrs)
		{
			this.closed = closed;
			this.attrs = attrs;
			active = false;
			locations = new List<Location>();
		}

		public override Icon Icon
		{
			get
			{
				if (closed)
				{
					return Icons.getIcon("drawpoly.gif");
				}
				else
				{
					return Icons.getIcon("drawplin.gif");
				}
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return DrawAttr.getFillAttributes(attrs.getValue(DrawAttr.PAINT_TYPE));
			}
		}

		public override Cursor getCursor(Canvas canvas)
		{
			return Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
		}

		public override void toolDeselected(Canvas canvas)
		{
			CanvasObject add = commit(canvas);
			canvas.toolGestureComplete(this, add);
			repaintArea(canvas);
		}

		public override void mousePressed(Canvas canvas, MouseEvent e)
		{
			int mx = e.getX();
			int my = e.getY();
			lastMouseX = mx;
			lastMouseY = my;
			int mods = e.getModifiersEx();
			if ((mods & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				mx = canvas.snapX(mx);
				my = canvas.snapY(my);
			}

			if (active && e.getClickCount() > 1)
			{
				CanvasObject add = commit(canvas);
				canvas.toolGestureComplete(this, add);
				return;
			}

			Location loc = new Location(mx, my);
			List<Location> locs = locations;
			if (!active)
			{
				locs.Clear();
				locs.Add(loc);
			}
			locs.Add(loc);

			mouseDown = true;
			active = canvas.Model != null;
			repaintArea(canvas);
		}

		public override void mouseDragged(Canvas canvas, MouseEvent e)
		{
			updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
		}

		public override void mouseReleased(Canvas canvas, MouseEvent e)
		{
			if (active)
			{
				updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
				mouseDown = false;
				int size = locations.Count;
				if (size >= 3)
				{
					Location first = locations[0];
					Location last = locations[size - 1];
					if (first.manhattanDistanceTo(last) <= CLOSE_TOLERANCE)
					{
						locations.RemoveAt(size - 1);
						CanvasObject add = commit(canvas);
						canvas.toolGestureComplete(this, add);
					}
				}
			}
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			int code = e.getKeyCode();
			if (active && mouseDown && (code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_CONTROL))
			{
				updateMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
			keyPressed(canvas, e);
		}

		public override void keyTyped(Canvas canvas, KeyEvent e)
		{
			if (active)
			{
				char ch = e.getKeyChar();
				if (ch == '\u001b')
				{ // escape key
					active = false;
					locations.Clear();
					repaintArea(canvas);
					canvas.toolGestureComplete(this, null);
				}
				else if (ch == '\n')
				{ // enter key
					CanvasObject add = commit(canvas);
					canvas.toolGestureComplete(this, add);
				}
			}
		}

		private CanvasObject commit(Canvas canvas)
		{
			if (!active)
			{
				return null;
			}
			CanvasObject add = null;
			active = false;
			List<Location> locs = locations;
			for (int i = locs.Count - 2; i >= 0; i--)
			{
				if (locs[i].Equals(locs[i + 1]))
				{
					locs.RemoveAt(i);
				}
			}
			if (locs.Count > 1)
			{
				CanvasModel model = canvas.Model;
				add = new Poly(closed, locs);
				canvas.doAction(new ModelAddAction(model, add));
				repaintArea(canvas);
			}
			locs.Clear();
			return add;
		}

		private void updateMouse(Canvas canvas, int mx, int my, int mods)
		{
			lastMouseX = mx;
			lastMouseY = my;
			if (active)
			{
				int index = locations.Count - 1;
				Location last = locations[index];
				Location newLast;
				if ((mods & MouseEvent.SHIFT_DOWN_MASK) != 0 && index > 0)
				{
					Location nextLast = locations[index - 1];
					newLast = LineUtil.snapTo8Cardinals(nextLast, mx, my);
				}
				else
				{
					newLast = new Location(mx, my);
				}
				if ((mods & MouseEvent.CTRL_DOWN_MASK) != 0)
				{
					int lastX = newLast.X;
					int lastY = newLast.Y;
					lastX = canvas.snapX(lastX);
					lastY = canvas.snapY(lastY);
					newLast = new Location(lastX, lastY);
				}

				if (!newLast.Equals(last))
				{
					locations[index] = newLast;
					repaintArea(canvas);
				}
			}
		}

		private void repaintArea(Canvas canvas)
		{
			canvas.repaint();
		}

		public override void draw(Canvas canvas, Graphics g)
		{
			if (active)
			{
				g.setColor(Color.GRAY);
				int size = locations.Count;
				int[] xs = new int[size];
				int[] ys = new int[size];
				for (int i = 0; i < size; i++)
				{
					Location loc = locations[i];
					xs[i] = loc.X;
					ys[i] = loc.Y;
				}
				g.drawPolyline(xs, ys, size);
				int lastX = xs[xs.Length - 1];
				int lastY = ys[ys.Length - 1];
				g.fillOval(lastX - 2, lastY - 2, 4, 4);
			}
		}
	}

}
