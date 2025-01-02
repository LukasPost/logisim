// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.tools
{

	using ModelMoveHandleAction = draw.actions.ModelMoveHandleAction;
	using ModelRemoveAction = draw.actions.ModelRemoveAction;
	using ModelTranslateAction = draw.actions.ModelTranslateAction;
	using Canvas = draw.canvas.Canvas;
	using Selection = draw.canvas.Selection;
	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using Handle = draw.model.Handle;
	using HandleGesture = draw.model.HandleGesture;
	using logisim.data;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using Icons = logisim.util.Icons;

	public class SelectTool : AbstractTool
	{
		private const int IDLE = 0;
		private const int MOVE_ALL = 1;
		private const int RECT_SELECT = 2;
		private const int RECT_TOGGLE = 3;
		private const int MOVE_HANDLE = 4;

		private const int DRAG_TOLERANCE = 2;
		private const int HANDLE_SIZE = 8;

		private static readonly Color RECT_SELECT_BACKGROUND = new Color(0, 0, 0, 32);

		private int curAction;
		private IList<CanvasObject> beforePressSelection;
		private Handle beforePressHandle;
		private Location dragStart;
		private Location dragEnd;
		private bool dragEffective;
		private int lastMouseX;
		private int lastMouseY;
		private HandleGesture curGesture;

		public SelectTool()
		{
			curAction = IDLE;
			dragStart = new Location(0, 0);
			dragEnd = dragStart;
			dragEffective = false;
		}

		public override Icon Icon
		{
			get
			{
				return Icons.getIcon("select.gif");
			}
		}

		public override Cursor getCursor(Canvas canvas)
		{
			return Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return Collections.emptyList();
			}
		}

		public override void toolSelected(Canvas canvas)
		{
			curAction = IDLE;
			canvas.Selection.clearSelected();
			repaintArea(canvas);
		}

		public override void toolDeselected(Canvas canvas)
		{
			curAction = IDLE;
			canvas.Selection.clearSelected();
			repaintArea(canvas);
		}

		private int getHandleSize(Canvas canvas)
		{
			double zoom = canvas.ZoomFactor;
			return (int) Math.Ceiling(HANDLE_SIZE / Math.Sqrt(zoom));
		}

		public override void mousePressed(Canvas canvas, MouseEvent e)
		{
			beforePressSelection = new List<CanvasObject>(canvas.Selection.Selected);
			beforePressHandle = canvas.Selection.SelectedHandle;
			int mx = e.getX();
			int my = e.getY();
			bool shift = (e.getModifiersEx() & MouseEvent.SHIFT_DOWN_MASK) != 0;
			dragStart = new Location(mx, my);
			dragEffective = false;
			dragEnd = dragStart;
			lastMouseX = mx;
			lastMouseY = my;
			Selection selection = canvas.Selection;
			selection.HandleSelected = null;

			// see whether user is pressing within an existing handle
			int halfSize = getHandleSize(canvas) / 2;
			CanvasObject clicked = null;
			foreach (CanvasObject shape in selection.Selected)
			{
				IList<Handle> handles = shape.getHandles(null);
				foreach (Handle han in handles)
				{
					int dx = han.X - mx;
					int dy = han.Y - my;
					if (dx >= -halfSize && dx <= halfSize && dy >= -halfSize && dy <= halfSize)
					{
						if (shape.canMoveHandle(han))
						{
							curAction = MOVE_HANDLE;
							curGesture = new HandleGesture(han, 0, 0, e.getModifiersEx());
							repaintArea(canvas);
							return;
						}
						else if (clicked == null)
						{
							clicked = shape;
						}
					}
				}
			}

			// see whether the user is clicking within a shape
			if (clicked == null)
			{
				clicked = getObjectAt(canvas.Model, e.getX(), e.getY(), false);
			}
			if (clicked != null)
			{
				if (shift && selection.isSelected(clicked))
				{
					selection.setSelected(clicked, false);
					curAction = IDLE;
				}
				else
				{
					if (!shift && !selection.isSelected(clicked))
					{
						selection.clearSelected();
					}
					selection.setSelected(clicked, true);
					selection.setMovingShapes(selection.Selected, 0, 0);
					curAction = MOVE_ALL;
				}
				repaintArea(canvas);
				return;
			}

			clicked = getObjectAt(canvas.Model, e.getX(), e.getY(), true);
			if (clicked != null && selection.isSelected(clicked))
			{
				if (shift)
				{
					selection.setSelected(clicked, false);
					curAction = IDLE;
				}
				else
				{
					selection.setMovingShapes(selection.Selected, 0, 0);
					curAction = MOVE_ALL;
				}
				repaintArea(canvas);
				return;
			}

			if (shift)
			{
				curAction = RECT_TOGGLE;
			}
			else
			{
				selection.clearSelected();
				curAction = RECT_SELECT;
			}
			repaintArea(canvas);
		}

		public override void cancelMousePress(Canvas canvas)
		{
			IList<CanvasObject> before = beforePressSelection;
			Handle handle = beforePressHandle;
			beforePressSelection = null;
			beforePressHandle = null;
			if (before != null)
			{
				curAction = IDLE;
				Selection sel = canvas.Selection;
				sel.clearDrawsSuppressed();
				sel.setMovingShapes(Enumerable.Empty<CanvasObject>(), 0, 0);
				sel.clearSelected();
				sel.setSelected(before, true);
				sel.HandleSelected = handle;
				repaintArea(canvas);
			}
		}

		public override void mouseDragged(Canvas canvas, MouseEvent e)
		{
			setMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
		}

		public override void mouseReleased(Canvas canvas, MouseEvent e)
		{
			beforePressSelection = null;
			beforePressHandle = null;
			setMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());

			CanvasModel model = canvas.Model;
			Selection selection = canvas.Selection;
			ISet<CanvasObject> selected = selection.Selected;
			int action = curAction;
			curAction = IDLE;

			if (!dragEffective)
			{
				Location loc = dragEnd;
				CanvasObject o = getObjectAt(model, loc.X, loc.Y, false);
				if (o != null)
				{
					Handle han = o.canDeleteHandle(loc);
					if (han != null)
					{
						selection.HandleSelected = han;
					}
					else
					{
						han = o.canInsertHandle(loc);
						if (han != null)
						{
							selection.HandleSelected = han;
						}
					}
				}
			}

			Location start = dragStart;
			int x1 = e.getX();
			int y1 = e.getY();
			switch (action)
			{
			case MOVE_ALL:
				Location moveDelta = selection.MovingDelta;
				if (dragEffective && !moveDelta.Equals(new Location(0, 0)))
				{
					canvas.doAction(new ModelTranslateAction(model, selected, moveDelta.X, moveDelta.Y));
				}
				break;
			case MOVE_HANDLE:
				HandleGesture gesture = curGesture;
				curGesture = null;
				if (dragEffective && gesture != null)
				{
					ModelMoveHandleAction act;
					act = new ModelMoveHandleAction(model, gesture);
					canvas.doAction(act);
					Handle result = act.NewHandle;
					if (result != null)
					{
						Handle h = result.Object.canDeleteHandle(result.Location);
						selection.HandleSelected = h;
					}
				}
				break;
			case RECT_SELECT:
				if (dragEffective)
				{
					Bounds bds = Bounds.create(start).add(x1, y1);
					selection.setSelected(canvas.Model.getObjectsIn(bds), true);
				}
				else
				{
					CanvasObject clicked;
					clicked = getObjectAt(model, start.X, start.Y, true);
					if (clicked != null)
					{
						selection.clearSelected();
						selection.setSelected(clicked, true);
					}
				}
				break;
			case RECT_TOGGLE:
				if (dragEffective)
				{
					Bounds bds = Bounds.create(start).add(x1, y1);
					selection.toggleSelected(canvas.Model.getObjectsIn(bds));
				}
				else
				{
					CanvasObject clicked;
					clicked = getObjectAt(model, start.X, start.Y, true);
					selection.setSelected(clicked, !selected.Contains(clicked));
				}
				break;
			}
			selection.clearDrawsSuppressed();
			repaintArea(canvas);
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			int code = e.getKeyCode();
			if ((code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_CONTROL || code == KeyEvent.VK_ALT) && curAction != IDLE)
			{
				setMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
			keyPressed(canvas, e);
		}

		public override void keyTyped(Canvas canvas, KeyEvent e)
		{
			char ch = e.getKeyChar();
			Selection selected = canvas.Selection;
			if ((ch == '\u0008' || ch == '\u007F') && !selected.Empty)
			{
				List<CanvasObject> toRemove = new List<CanvasObject>();
				foreach (CanvasObject shape in selected.Selected)
				{
					if (shape.canRemove())
					{
						toRemove.Add(shape);
					}
				}
				if (toRemove.Count > 0)
				{
					e.consume();
					CanvasModel model = canvas.Model;
					canvas.doAction(new ModelRemoveAction(model, toRemove));
					selected.clearSelected();
					repaintArea(canvas);
				}
			}
			else if (ch == '\u001b' && !selected.Empty)
			{
				selected.clearSelected();
				repaintArea(canvas);
			}
		}

		private void setMouse(Canvas canvas, int mx, int my, int mods)
		{
			lastMouseX = mx;
			lastMouseY = my;
			bool shift = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0;
			bool ctrl = (mods & InputEvent.CTRL_DOWN_MASK) != 0;
			Location newEnd = new Location(mx, my);
			dragEnd = newEnd;

			Location start = dragStart;
			int dx = newEnd.X - start.X;
			int dy = newEnd.Y - start.Y;
			if (!dragEffective)
			{
				if (Math.Abs(dx) + Math.Abs(dy) > DRAG_TOLERANCE)
				{
					dragEffective = true;
				}
				else
				{
					return;
				}
			}

			switch (curAction)
			{
			case MOVE_HANDLE:
				HandleGesture gesture = curGesture;
				if (ctrl)
				{
					Handle h = gesture.Handle;
					dx = canvas.snapX(h.X + dx) - h.X;
					dy = canvas.snapY(h.Y + dy) - h.Y;
				}
				curGesture = new HandleGesture(gesture.Handle, dx, dy, mods);
				canvas.Selection.HandleGesture = curGesture;
				break;
			case MOVE_ALL:
				if (ctrl)
				{
					int minX = int.MaxValue;
					int minY = int.MaxValue;
					foreach (CanvasObject o in canvas.Selection.Selected)
					{
						foreach (Handle handle in o.getHandles(null))
						{
							int x = handle.X;
							int y = handle.Y;
							if (x < minX)
							{
								minX = x;
							}
							if (y < minY)
							{
								minY = y;
							}
						}
					}
					dx = canvas.snapX(minX + dx) - minX;
					dy = canvas.snapY(minY + dy) - minY;
				}
				if (shift)
				{
					if (Math.Abs(dx) > Math.Abs(dy))
					{
						dy = 0;
					}
					else
					{
						dx = 0;
					}
				}
				canvas.Selection.setMovingDelta(dx, dy);
				break;
			}
			repaintArea(canvas);
		}

		private void repaintArea(Canvas canvas)
		{
			canvas.repaint();
		}

		public override void draw(Canvas canvas, Graphics g)
		{
			Selection selection = canvas.Selection;
			int action = curAction;

			Location start = dragStart;
			Location end = dragEnd;
			HandleGesture gesture = null;
			bool drawHandles;
			switch (action)
			{
			case MOVE_ALL:
				drawHandles = !dragEffective;
				break;
			case MOVE_HANDLE:
				drawHandles = !dragEffective;
				if (dragEffective)
				{
					gesture = curGesture;
				}
				break;
			default:
				drawHandles = true;
			break;
			}

			CanvasObject moveHandleObj = null;
			if (gesture != null)
			{
				moveHandleObj = gesture.Handle.Object;
			}
			if (drawHandles)
			{
				// unscale the coordinate system so that the stroke width isn't scaled
				double zoom = 1.0;
				Graphics gCopy = g.create();
				if (gCopy is Graphics2D)
				{
					zoom = canvas.ZoomFactor;
					if (zoom != 1.0)
					{
						((Graphics2D) gCopy).scale(1.0 / zoom, 1.0 / zoom);
					}
				}
				GraphicsUtil.switchToWidth(gCopy, 1);

				int size = (int) Math.Ceiling(HANDLE_SIZE * Math.Sqrt(zoom));
				int offs = size / 2;
				foreach (CanvasObject obj in selection.Selected)
				{
					IList<Handle> handles;
					if (action == MOVE_HANDLE && obj == moveHandleObj)
					{
						handles = obj.getHandles(gesture);
					}
					else
					{
						handles = obj.getHandles(null);
					}
					foreach (Handle han in handles)
					{
						int x = han.X;
						int y = han.Y;
						if (action == MOVE_ALL && dragEffective)
						{
							Location delta = selection.MovingDelta;
							x += delta.X;
							y += delta.Y;
						}
						x = (int) (long)Math.Round(zoom * x, MidpointRounding.AwayFromZero);
						y = (int) (long)Math.Round(zoom * y, MidpointRounding.AwayFromZero);
						gCopy.clearRect(x - offs, y - offs, size, size);
						gCopy.drawRect(x - offs, y - offs, size, size);
					}
				}
				Handle selHandle = selection.SelectedHandle;
				if (selHandle != null)
				{
					int x = selHandle.X;
					int y = selHandle.Y;
					if (action == MOVE_ALL && dragEffective)
					{
						Location delta = selection.MovingDelta;
						x += delta.X;
						y += delta.Y;
					}
					x = (int) (long)Math.Round(zoom * x, MidpointRounding.AwayFromZero);
					y = (int) (long)Math.Round(zoom * y, MidpointRounding.AwayFromZero);
					int[] xs = new int[] {x - offs, x, x + offs, x};
					int[] ys = new int[] {y, y - offs, y, y + offs};
					gCopy.setColor(Color.WHITE);
					gCopy.fillPolygon(xs, ys, 4);
					gCopy.setColor(Color.BLACK);
					gCopy.drawPolygon(xs, ys, 4);
				}
			}

			switch (action)
			{
			case RECT_SELECT:
			case RECT_TOGGLE:
				if (dragEffective)
				{
					// find rectangle currently to show
					int x0 = start.X;
					int y0 = start.Y;
					int x1 = end.X;
					int y1 = end.Y;
					if (x1 < x0)
					{
						int t = x0;
						x0 = x1;
						x1 = t;
					}
					if (y1 < y0)
					{
						int t = y0;
						y0 = y1;
						y1 = t;
					}

					// make the region that's not being selected darker
					int w = canvas.getWidth();
					int h = canvas.getHeight();
					g.setColor(RECT_SELECT_BACKGROUND);
					g.fillRect(0, 0, w, y0);
					g.fillRect(0, y0, x0, y1 - y0);
					g.fillRect(x1, y0, w - x1, y1 - y0);
					g.fillRect(0, y1, w, h - y1);

					// now draw the rectangle
					g.setColor(Color.GRAY);
					g.drawRect(x0, y0, x1 - x0, y1 - y0);
				}
				break;
			}
		}

		private static CanvasObject getObjectAt(CanvasModel model, int x, int y, bool assumeFilled)
		{
			Location loc = new Location(x, y);
			foreach (CanvasObject o in model.ObjectsFromTop)
			{
				if (o.contains(loc, assumeFilled))
				{
					return o;
				}
			}
			return null;
		}
	}

}
