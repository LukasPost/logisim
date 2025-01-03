// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

using LogisimPlus.Java;

namespace draw.canvas
{

	public abstract class CanvasTool
	{
		public abstract Cursor getCursor(Canvas canvas);

		public virtual void draw(Canvas canvas, JGraphics g)
		{
		}

		public virtual void toolSelected(Canvas canvas)
		{
		}

		public virtual void toolDeselected(Canvas canvas)
		{
		}

		public virtual void mouseMoved(Canvas canvas, MouseEvent e)
		{
		}

		public virtual void mousePressed(Canvas canvas, MouseEvent e)
		{
		}

		public virtual void mouseDragged(Canvas canvas, MouseEvent e)
		{
		}

		public virtual void mouseReleased(Canvas canvas, MouseEvent e)
		{
		}

		public virtual void mouseEntered(Canvas canvas, MouseEvent e)
		{
		}

		public virtual void mouseExited(Canvas canvas, MouseEvent e)
		{
		}

		/// <summary>
		/// This is because a popup menu may result from the subsequent mouse release </summary>
		public virtual void cancelMousePress(Canvas canvas)
		{
		}

		public virtual void keyPressed(Canvas canvas, KeyEvent e)
		{
		}

		public virtual void keyReleased(Canvas canvas, KeyEvent e)
		{
		}

		public virtual void keyTyped(Canvas canvas, KeyEvent e)
		{
		}

		public virtual void zoomFactorChanged(Canvas canvas)
		{
		}
	}

}
