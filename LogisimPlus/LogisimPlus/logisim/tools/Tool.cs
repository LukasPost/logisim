// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using LogisimVersion = logisim.LogisimVersion;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using logisim.data;
	using AttributeDefaultProvider = logisim.data.AttributeDefaultProvider;
	using AttributeSet = logisim.data.AttributeSet;
	using Canvas = logisim.gui.main.Canvas;

	//
	// DRAWING TOOLS
	//
	public abstract class Tool : AttributeDefaultProvider
	{
		private static Cursor dflt_cursor = Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);

		public abstract string Name {get;}

		public abstract string DisplayName {get;}

		public abstract string Description {get;}

		public virtual Tool cloneTool()
		{
			return this;
		}

		public virtual bool sharesSource(Tool other)
		{
			return this == other;
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual AttributeSet getAttributeSet(Canvas canvas)
		{
			return AttributeSet;
		}

		public virtual bool isAllDefaultValues(AttributeSet attrs, LogisimVersion ver)
		{
			return false;
		}

		public virtual object getDefaultAttributeValue<T1>(Attribute<T1> attr, LogisimVersion ver)
		{
			return null;
		}


		public virtual void paintIcon(ComponentDrawContext c, int x, int y)
		{
		}

		public override string ToString()
		{
			return Name;
		}

		// This was the draw method until 2.0.4 - As of 2.0.5, you should
		// use the other draw method.
		public virtual void draw(ComponentDrawContext context)
		{
		}

		public virtual void draw(Canvas canvas, ComponentDrawContext context)
		{
			draw(context);
		}

		public virtual ISet<Component> getHiddenComponents(Canvas canvas)
		{
			return null;
		}

		public virtual void select(Canvas canvas)
		{
		}

		public virtual void deselect(Canvas canvas)
		{
		}

		public virtual void mousePressed(Canvas canvas, Graphics g, MouseEvent e)
		{
		}

		public virtual void mouseDragged(Canvas canvas, Graphics g, MouseEvent e)
		{
		}

		public virtual void mouseReleased(Canvas canvas, Graphics g, MouseEvent e)
		{
		}

		public virtual void mouseEntered(Canvas canvas, Graphics g, MouseEvent e)
		{
		}

		public virtual void mouseExited(Canvas canvas, Graphics g, MouseEvent e)
		{
		}

		public virtual void mouseMoved(Canvas canvas, Graphics g, MouseEvent e)
		{
		}

		public virtual void keyTyped(Canvas canvas, KeyEvent e)
		{
		}

		public virtual void keyPressed(Canvas canvas, KeyEvent e)
		{
		}

		public virtual void keyReleased(Canvas canvas, KeyEvent e)
		{
		}

		public virtual Cursor Cursor
		{
			get
			{
				return dflt_cursor;
			}
		}

	}

}
