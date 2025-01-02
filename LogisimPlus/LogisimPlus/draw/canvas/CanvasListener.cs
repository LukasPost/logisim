// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.canvas
{

	using CanvasModelEvent = draw.model.CanvasModelEvent;
	using CanvasModelListener = draw.model.CanvasModelListener;
	using CanvasObject = draw.model.CanvasObject;
	using Location = logisim.data.Location;

	internal class CanvasListener : MouseListener, MouseMotionListener, KeyListener, CanvasModelListener
	{
		private Canvas canvas;
		private CanvasTool tool;

		public CanvasListener(Canvas canvas)
		{
			this.canvas = canvas;
			tool = null;
		}

		public virtual CanvasTool Tool
		{
			get
			{
				return tool;
			}
			set
			{
				CanvasTool oldValue = tool;
				if (value != oldValue)
				{
					tool = value;
					if (oldValue != null)
					{
						oldValue.toolDeselected(canvas);
					}
					if (value != null)
					{
						value.toolSelected(canvas);
						canvas.setCursor(value.getCursor(canvas));
					}
					else
					{
						canvas.setCursor(Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR));
					}
				}
			}
		}


		public virtual void mouseMoved(MouseEvent e)
		{
			if (tool != null)
			{
				tool.mouseMoved(canvas, e);
			}
		}

		public virtual void mousePressed(MouseEvent e)
		{
			canvas.requestFocus();
			if (e.isPopupTrigger())
			{
				handlePopupTrigger(e);
			}
			else if (e.getButton() == 1)
			{
				if (tool != null)
				{
					tool.mousePressed(canvas, e);
				}
			}
		}

		public virtual void mouseDragged(MouseEvent e)
		{
			if (isButton1(e))
			{
				if (tool != null)
				{
					tool.mouseDragged(canvas, e);
				}
			}
			else
			{
				if (tool != null)
				{
					tool.mouseMoved(canvas, e);
				}
			}
		}

		public virtual void mouseReleased(MouseEvent e)
		{
			if (e.isPopupTrigger())
			{
				if (tool != null)
				{
					tool.cancelMousePress(canvas);
				}
				handlePopupTrigger(e);
			}
			else if (e.getButton() == 1)
			{
				if (tool != null)
				{
					tool.mouseReleased(canvas, e);
				}
			}
		}

		public virtual void mouseClicked(MouseEvent e)
		{
		}

		public virtual void mouseEntered(MouseEvent e)
		{
			if (tool != null)
			{
				tool.mouseEntered(canvas, e);
			}
		}

		public virtual void mouseExited(MouseEvent e)
		{
			if (tool != null)
			{
				tool.mouseExited(canvas, e);
			}
		}

		public virtual void keyPressed(KeyEvent e)
		{
			if (tool != null)
			{
				tool.keyPressed(canvas, e);
			}
		}

		public virtual void keyReleased(KeyEvent e)
		{
			if (tool != null)
			{
				tool.keyReleased(canvas, e);
			}
		}

		public virtual void keyTyped(KeyEvent e)
		{
			if (tool != null)
			{
				tool.keyTyped(canvas, e);
			}
		}

		public virtual void modelChanged(CanvasModelEvent @event)
		{
			canvas.Selection.modelChanged(@event);
			canvas.repaint();
		}

		private bool isButton1(MouseEvent e)
		{
			return (e.getModifiersEx() & MouseEvent.BUTTON1_DOWN_MASK) != 0;
		}

		private void handlePopupTrigger(MouseEvent e)
		{
			Location loc = new Location(e.getX(), e.getY());
			IList<CanvasObject> objects = canvas.Model.getObjectsFromTop();
			CanvasObject clicked = null;
			foreach (CanvasObject o in objects)
			{
				if (o.contains(loc, false))
				{
					clicked = o;
					break;
				}
			}
			if (clicked == null)
			{
				foreach (CanvasObject o in objects)
				{
					if (o.contains(loc, true))
					{
						clicked = o;
						break;
					}
				}
			}
			canvas.showPopupMenu(e, clicked);
		}
	}

}
