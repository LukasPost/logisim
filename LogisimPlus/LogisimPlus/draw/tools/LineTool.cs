// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
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
	using logisim.util;
    using LogisimPlus.Java;

    public class LineTool : AbstractTool
	{
		private DrawingAttributeSet attrs;
		private bool active;
		private Location mouseStart;
		private Location mouseEnd;
		private int lastMouseX;
		private int lastMouseY;

		public LineTool(DrawingAttributeSet attrs)
		{
			this.attrs = attrs;
			active = false;
		}

		public override Icon Icon
		{
			get
			{
				return Icons.getIcon("drawline.gif");
			}
		}

		public override Cursor getCursor(Canvas canvas)
		{
			return Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return DrawAttr.ATTRS_STROKE;
			}
		}

		public override void toolDeselected(Canvas canvas)
		{
			active = false;
			repaintArea(canvas);
		}

		public override void mousePressed(Canvas canvas, MouseEvent e)
		{
			int x = e.getX();
			int y = e.getY();
			int mods = e.getModifiersEx();
			if ((mods & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				x = canvas.snapX(x);
				y = canvas.snapY(y);
			}
			Location loc = new Location(x, y);
			mouseStart = loc;
			mouseEnd = loc;
			lastMouseX = loc.X;
			lastMouseY = loc.Y;
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
				Location start = mouseStart;
				Location end = mouseEnd;
				CanvasObject add = null;
				if (!start.Equals(end))
				{
					active = false;
					CanvasModel model = canvas.Model;
					Location[] ends = new Location[] {start, end};
					List<Location> locs = UnmodifiableList.create(ends);
					add = attrs.applyTo(new Poly(false, locs));
					add.setValue(DrawAttr.PAINT_TYPE, DrawAttr.PAINT_STROKE);
					canvas.doAction(new ModelAddAction(model, add));
					repaintArea(canvas);
				}
				canvas.toolGestureComplete(this, add);
			}
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			int code = e.getKeyCode();
			if (active && (code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_CONTROL))
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
			if (active)
			{
				bool shift = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0;
				Location newEnd;
				if (shift)
				{
					newEnd = LineUtil.snapTo8Cardinals(mouseStart, mx, my);
				}
				else
				{
					newEnd = new Location(mx, my);
				}

				if ((mods & InputEvent.CTRL_DOWN_MASK) != 0)
				{
					int x = newEnd.X;
					int y = newEnd.Y;
					x = canvas.snapX(x);
					y = canvas.snapY(y);
					newEnd = new Location(x, y);
				}

				if (!newEnd.Equals(mouseEnd))
				{
					mouseEnd = newEnd;
					repaintArea(canvas);
				}
			}
			lastMouseX = mx;
			lastMouseY = my;
		}

		private void repaintArea(Canvas canvas)
		{
			canvas.repaint();
		}

		public override void draw(Canvas canvas, JGraphics g)
		{
			if (active)
			{
				Location start = mouseStart;
				Location end = mouseEnd;
				g.setColor(Color.Gray);
				g.drawLine(start.X, start.Y, end.X, end.Y);
			}
		}

		internal static Location snapTo4Cardinals(Location from, int mx, int my)
		{
			int px = from.X;
			int py = from.Y;
			if (mx != px && my != py)
			{
				if (Math.Abs(my - py) < Math.Abs(mx - px))
				{
					return new Location(mx, py);
				}
				else
				{
					return new Location(px, my);
				}
			}
			return new Location(mx, my); // should never happen
		}
	}

}
