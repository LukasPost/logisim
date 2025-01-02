// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.tools
{

	using ModelAddAction = draw.actions.ModelAddAction;
	using Canvas = draw.canvas.Canvas;
	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;

	internal abstract class RectangularTool : AbstractTool
	{
		private bool active;
		private Location dragStart;
		private int lastMouseX;
		private int lastMouseY;
		private Bounds currentBounds;

		public RectangularTool()
		{
			active = false;
			currentBounds = Bounds.EMPTY_BOUNDS;
		}

		public abstract CanvasObject createShape(int x, int y, int w, int h);

		public abstract void drawShape(Graphics g, int x, int y, int w, int h);

		public abstract void fillShape(Graphics g, int x, int y, int w, int h);

		public override Cursor getCursor(Canvas canvas)
		{
			return Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
		}

		public override void toolDeselected(Canvas canvas)
		{
			Bounds bds = currentBounds;
			active = false;
			repaintArea(canvas, bds);
		}

		public override void mousePressed(Canvas canvas, MouseEvent e)
		{
			Location loc = new Location(e.getX(), e.getY());
			Bounds bds = Bounds.create(loc);
			dragStart = loc;
			lastMouseX = loc.X;
			lastMouseY = loc.Y;
			active = canvas.Model != null;
			repaintArea(canvas, bds);
		}

		public override void mouseDragged(Canvas canvas, MouseEvent e)
		{
			updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
		}

		public override void mouseReleased(Canvas canvas, MouseEvent e)
		{
			if (active)
			{
				Bounds oldBounds = currentBounds;
				Bounds bds = computeBounds(canvas, e.getX(), e.getY(), e.getModifiersEx());
				currentBounds = Bounds.EMPTY_BOUNDS;
				active = false;
				CanvasObject add = null;
				if (bds.Width != 0 && bds.Height != 0)
				{
					CanvasModel model = canvas.Model;
					add = createShape(bds.X, bds.Y, bds.Width, bds.Height);
					canvas.doAction(new ModelAddAction(model, add));
					repaintArea(canvas, oldBounds.add(bds));
				}
				canvas.toolGestureComplete(this, add);
			}
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			int code = e.getKeyCode();
			if (active && (code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_ALT || code == KeyEvent.VK_CONTROL))
			{
				updateMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
			keyPressed(canvas, e);
		}

		private void updateMouse(Canvas canvas, int mx, int my, int mods)
		{
			Bounds oldBounds = currentBounds;
			Bounds bds = computeBounds(canvas, mx, my, mods);
			if (!bds.Equals(oldBounds))
			{
				currentBounds = bds;
				repaintArea(canvas, oldBounds.add(bds));
			}
		}

		private Bounds computeBounds(Canvas canvas, int mx, int my, int mods)
		{
			lastMouseX = mx;
			lastMouseY = my;
			if (!active)
			{
				return Bounds.EMPTY_BOUNDS;
			}
			else
			{
				Location start = dragStart;
				int x0 = start.X;
				int y0 = start.Y;
				int x1 = mx;
				int y1 = my;
				if (x0 == x1 && y0 == y1)
				{
					return Bounds.EMPTY_BOUNDS;
				}

				bool ctrlDown = (mods & MouseEvent.CTRL_DOWN_MASK) != 0;
				if (ctrlDown)
				{
					x0 = canvas.snapX(x0);
					y0 = canvas.snapY(y0);
					x1 = canvas.snapX(x1);
					y1 = canvas.snapY(y1);
				}

				bool altDown = (mods & MouseEvent.ALT_DOWN_MASK) != 0;
				bool shiftDown = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0;
				if (altDown)
				{
					if (shiftDown)
					{
						int r = Math.Min(Math.Abs(x0 - x1), Math.Abs(y0 - y1));
						x1 = x0 + r;
						y1 = y0 + r;
						x0 -= r;
						y0 -= r;
					}
					else
					{
						x0 = x0 - (x1 - x0);
						y0 = y0 - (y1 - y0);
					}
				}
				else
				{
					if (shiftDown)
					{
						int r = Math.Min(Math.Abs(x0 - x1), Math.Abs(y0 - y1));
						y1 = y1 < y0 ? y0 - r : y0 + r;
						x1 = x1 < x0 ? x0 - r : x0 + r;
					}
				}

				int x = x0;
				int y = y0;
				int w = x1 - x0;
				int h = y1 - y0;
				if (w < 0)
				{
					x = x1;
					w = -w;
				}
				if (h < 0)
				{
					y = y1;
					h = -h;
				}
				return Bounds.create(x, y, w, h);
			}
		}

		private void repaintArea(Canvas canvas, Bounds bds)
		{
			canvas.repaint();
			/*
			 * The below doesn't work because Java doesn't deal correctly with stroke widths that go outside the clip area
			 * canvas.repaintCanvasCoords(bds.getX() - 10, bds.getY() - 10, bds.getWidth() + 20, bds.getHeight() + 20);
			 */
		}

		public override void draw(Canvas canvas, Graphics g)
		{
			Bounds bds = currentBounds;
			if (active && bds != null && bds != Bounds.EMPTY_BOUNDS)
			{
				g.setColor(Color.GRAY);
				drawShape(g, bds.X, bds.Y, bds.Width, bds.Height);
			}
		}

	}

}
