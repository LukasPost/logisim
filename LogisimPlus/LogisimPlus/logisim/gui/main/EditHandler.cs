// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{
	using LogisimMenuItem = logisim.gui.menu.LogisimMenuItem;

	public abstract class EditHandler
	{
		public interface Listener
		{
			void enableChanged(EditHandler handler, LogisimMenuItem action, bool value);
		}

		private Listener listener;

		public virtual Listener Listener
		{
			set
			{
				this.listener = value;
			}
		}

		protected internal virtual void setEnabled(LogisimMenuItem action, bool value)
		{
			Listener l = listener;
			if (l != null)
			{
				l.enableChanged(this, action, value);
			}
		}

		public abstract void computeEnabled();

		public abstract void cut();

		public abstract void copy();

		public abstract void paste();

		public abstract void delete();

		public abstract void duplicate();

		public abstract void selectAll();

		public abstract void raise();

		public abstract void lower();

		public abstract void raHashSetop();

		public abstract void lowerBottom();

		public abstract void addControlPoint();

		public abstract void removeControlPoint();
	}

}
