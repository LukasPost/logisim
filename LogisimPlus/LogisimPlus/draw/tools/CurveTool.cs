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
	using Curve = draw.shapes.Curve;
	using CurveUtil = draw.shapes.CurveUtil;
	using DrawAttr = draw.shapes.DrawAttr;
	using LineUtil = draw.shapes.LineUtil;
	using logisim.data;
	using Location = logisim.data.Location;
	using Icons = logisim.util.Icons;
    using LogisimPlus.Java;

    public class CurveTool : AbstractTool
	{
		private const int BEFORE_CREATION = 0;
		private const int ENDPOINT_DRAG = 1;
		private const int CONTROL_DRAG = 2;

		private DrawingAttributeSet attrs;
		private int state;
		private Location end0;
		private Location end1;
		private Curve curCurve;
		private bool mouseDown;
		private int lastMouseX;
		private int lastMouseY;

		public CurveTool(DrawingAttributeSet attrs)
		{
			this.attrs = attrs;
			state = BEFORE_CREATION;
			mouseDown = false;
		}

		public override Icon Icon
		{
			get
			{
				return Icons.getIcon("drawcurv.gif");
			}
		}

		public override Cursor getCursor(Canvas canvas)
		{
			return Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);
		}

		public override void toolDeselected(Canvas canvas)
		{
			state = BEFORE_CREATION;
			repaintArea(canvas);
		}

		public override void mousePressed(Canvas canvas, MouseEvent e)
		{
			int mx = e.getX();
			int my = e.getY();
			lastMouseX = mx;
			lastMouseY = my;
			mouseDown = true;
			int mods = e.getModifiersEx();
			if ((mods & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				mx = canvas.snapX(mx);
				my = canvas.snapY(my);
			}

			switch (state)
			{
			case BEFORE_CREATION:
			case CONTROL_DRAG:
				end0 = new Location(mx, my);
				end1 = end0;
				state = ENDPOINT_DRAG;
				break;
			case ENDPOINT_DRAG:
				curCurve = new Curve(end0, end1, new Location(mx, my));
				state = CONTROL_DRAG;
				break;
			}
			repaintArea(canvas);
		}

		public override void mouseDragged(Canvas canvas, MouseEvent e)
		{
			updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
			repaintArea(canvas);
		}

		public override void mouseReleased(Canvas canvas, MouseEvent e)
		{
			Curve c = updateMouse(canvas, e.getX(), e.getY(), e.getModifiersEx());
			mouseDown = false;
			if (state == CONTROL_DRAG)
			{
				if (c != null)
				{
					attrs.applyTo(c);
					CanvasModel model = canvas.Model;
					canvas.doAction(new ModelAddAction(model, c));
					canvas.toolGestureComplete(this, c);
				}
				state = BEFORE_CREATION;
			}
			repaintArea(canvas);
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			int code = e.getKeyCode();
			if (mouseDown && (code == KeyEvent.VK_SHIFT || code == KeyEvent.VK_CONTROL || code == KeyEvent.VK_ALT))
			{
				updateMouse(canvas, lastMouseX, lastMouseY, e.getModifiersEx());
				repaintArea(canvas);
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
			keyPressed(canvas, e);
		}

		public override void keyTyped(Canvas canvas, KeyEvent e)
		{
			char ch = e.getKeyChar();
			if (ch == '\u001b')
			{ // escape key
				state = BEFORE_CREATION;
				repaintArea(canvas);
				canvas.toolGestureComplete(this, null);
			}
		}

		private Curve updateMouse(Canvas canvas, int mx, int my, int mods)
		{
			lastMouseX = mx;
			lastMouseY = my;

			bool shiftDown = (mods & MouseEvent.SHIFT_DOWN_MASK) != 0;
			bool ctrlDown = (mods & MouseEvent.CTRL_DOWN_MASK) != 0;
			bool altDown = (mods & MouseEvent.ALT_DOWN_MASK) != 0;
			Curve ret = null;
			switch (state)
			{
			case ENDPOINT_DRAG:
				if (mouseDown)
				{
					if (shiftDown)
					{
						Location p = LineUtil.snapTo8Cardinals(end0, mx, my);
						mx = p.X;
						my = p.Y;
					}
					if (ctrlDown)
					{
						mx = canvas.snapX(mx);
						my = canvas.snapY(my);
					}
					end1 = new Location(mx, my);
				}
				break;
			case CONTROL_DRAG:
				if (mouseDown)
				{
					int cx = mx;
					int cy = my;
					if (ctrlDown)
					{
						cx = canvas.snapX(cx);
						cy = canvas.snapY(cy);
					}
					if (shiftDown)
					{
						double x0 = end0.X;
						double y0 = end0.Y;
						double x1 = end1.X;
						double y1 = end1.Y;
						double midx = (x0 + x1) / 2;
						double midy = (y0 + y1) / 2;
						double dx = x1 - x0;
						double dy = y1 - y0;
						double[] p = LineUtil.nearestPointInfinite(cx, cy, midx, midy, midx - dy, midy + dx);
						cx = (int) (long)Math.Round(p[0], MidpointRounding.AwayFromZero);
						cy = (int) (long)Math.Round(p[1], MidpointRounding.AwayFromZero);
					}
					if (altDown)
					{
						double[] e0 = new double[] {end0.X, end0.Y};
						double[] e1 = new double[] {end1.X, end1.Y};
						double[] mid = new double[] {cx, cy};
						double[] ct = CurveUtil.interpolate(e0, e1, mid);
						cx = (int) (long)Math.Round(ct[0], MidpointRounding.AwayFromZero);
						cy = (int) (long)Math.Round(ct[1], MidpointRounding.AwayFromZero);
					}
					ret = new Curve(end0, end1, new Location(cx, cy));
					curCurve = ret;
				}
				break;
			}
			return ret;
		}

		private void repaintArea(Canvas canvas)
		{
			canvas.repaint();
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return DrawAttr.getFillAttributes((AttributeOption)attrs.getValue(DrawAttr.PAINT_TYPE));
			}
		}

		public override void draw(Canvas canvas, JGraphics g)
		{
			g.setColor(Color.Gray);
			switch (state)
			{
			case ENDPOINT_DRAG:
				g.drawLine(end0.X, end0.Y, end1.X, end1.Y);
				break;
			case CONTROL_DRAG:
				g.draw(curCurve.Curve2D);
				break;
			}
		}
	}

}
