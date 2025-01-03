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

	using Bounds = logisim.data.Bounds;

	public class AbstractCaret : Caret
	{
		private List<CaretListener> listeners = new List<CaretListener>();
		private List<CaretListener> listenersView;
		private Bounds bds = Bounds.EMPTY_BOUNDS;

		public AbstractCaret()
		{
			listenersView = listeners.AsReadOnly();
		}

		// listener methods
		public virtual void addCaretListener(CaretListener e)
		{
			listeners.Add(e);
		}

		public virtual void removeCaretListener(CaretListener e)
		{
			listeners.Remove(e);
		}

		protected internal virtual List<CaretListener> CaretListeners
		{
			get
			{
				return listenersView;
			}
		}

		// configuration methods
		public virtual Bounds Bounds
		{
			set
			{
				bds = value;
			}
		}

		// query/JGraphics methods
		public virtual string Text
		{
			get
			{
				return "";
			}
		}

		public virtual Bounds getBounds(JGraphics g)
		{
			return bds;
		}

		public virtual void draw(JGraphics g)
		{
		}

		// finishing
		public virtual void commitText(string text)
		{
		}

		public virtual void cancelEditing()
		{
		}

		public virtual void stopEditing()
		{
		}

		// events to handle
		public virtual void mousePressed(MouseEvent e)
		{
		}

		public virtual void mouseDragged(MouseEvent e)
		{
		}

		public virtual void mouseReleased(MouseEvent e)
		{
		}

		public virtual void keyPressed(KeyEvent e)
		{
		}

		public virtual void keyReleased(KeyEvent e)
		{
		}

		public virtual void keyTyped(KeyEvent e)
		{
		}
	}

}
