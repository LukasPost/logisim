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

	using Canvas = draw.canvas.Canvas;
	using CanvasTool = draw.canvas.CanvasTool;
	using logisim.data;
    using LogisimPlus.Java;

    public abstract class AbstractTool : CanvasTool
	{
		public static AbstractTool[] getTools(DrawingAttributeSet attrs)
		{
			return new AbstractTool[]
			{
				new SelectTool(),
				new LineTool(attrs),
				new CurveTool(attrs),
				new PolyTool(false, attrs),
				new RectangleTool(attrs),
				new RoundRectangleTool(attrs),
				new OvalTool(attrs),
				new PolyTool(true, attrs)
			};
		}

		public abstract Icon Icon {get;}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public abstract java.util.List<logisim.data.Attribute<?>> getAttributes();
		public abstract List<Attribute> Attributes {get;}

		public virtual string Description
		{
			get
			{
				return null;
			}
		}

		//
		// CanvasTool methods
		//
		public override abstract Cursor getCursor(Canvas canvas);

		public override void toolSelected(Canvas canvas)
		{
		}

		public override void toolDeselected(Canvas canvas)
		{
		}

		public override void mouseMoved(Canvas canvas, MouseEvent e)
		{
		}

		public override void mousePressed(Canvas canvas, MouseEvent e)
		{
		}

		public override void mouseDragged(Canvas canvas, MouseEvent e)
		{
		}

		public override void mouseReleased(Canvas canvas, MouseEvent e)
		{
		}

		public override void mouseEntered(Canvas canvas, MouseEvent e)
		{
		}

		public override void mouseExited(Canvas canvas, MouseEvent e)
		{
		}

		/// <summary>
		/// This is because a popup menu may result from the subsequent mouse release </summary>
		public override void cancelMousePress(Canvas canvas)
		{
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
		}

		public override void keyTyped(Canvas canvas, KeyEvent e)
		{
		}

		public override void draw(Canvas canvas, JGraphics g)
		{
		}
	}

}
