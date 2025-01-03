// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{
    using LogisimPlus.Java;
    using Bounds = logisim.data.Bounds;

	public abstract class InstancePoker
	{
		public virtual bool init(InstanceState state, MouseEvent e)
		{
			return true;
		}

		public virtual Bounds getBounds(InstancePainter painter)
		{
			return painter.getInstance().Bounds;
		}

		public virtual void paint(InstancePainter painter)
		{
		}

		public virtual void mousePressed(InstanceState state, MouseEvent e)
		{
		}

		public virtual void mouseReleased(InstanceState state, MouseEvent e)
		{
		}

		public virtual void mouseDragged(InstanceState state, MouseEvent e)
		{
		}

		public virtual void keyPressed(InstanceState state, KeyEvent e)
		{
		}

		public virtual void keyReleased(InstanceState state, KeyEvent e)
		{
		}

		public virtual void keyTyped(InstanceState state, KeyEvent e)
		{
		}

		public virtual void stopEditing(InstanceState state)
		{
		}
	}

}
