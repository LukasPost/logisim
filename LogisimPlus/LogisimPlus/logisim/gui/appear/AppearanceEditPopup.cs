// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{

	using EditHandler = logisim.gui.main.EditHandler;
	using EditPopup = logisim.gui.menu.EditPopup;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using LogisimMenuItem = logisim.gui.menu.LogisimMenuItem;

	public class AppearanceEditPopup : EditPopup, EditHandler.Listener
	{
		private AppearanceCanvas canvas;
		private EditHandler handler;
		private Dictionary<LogisimMenuItem, bool> enabled;

		public AppearanceEditPopup(AppearanceCanvas canvas) : base(true)
		{
			this.canvas = canvas;
			handler = new AppearanceEditHandler(canvas);
			handler.Listener = this;
			enabled = new Dictionary<LogisimMenuItem, bool>();
			handler.computeEnabled();
			initialize();
		}

		public virtual void enableChanged(EditHandler handler, LogisimMenuItem action, bool value)
		{
			enabled[action] = Convert.ToBoolean(value);
		}

		protected internal override bool shouldShow(LogisimMenuItem item)
		{
			if (item == LogisimMenuBar.ADD_CONTROL || item == LogisimMenuBar.REMOVE_CONTROL)
			{
				return canvas.Selection.SelectedHandle != null;
			}
			else
			{
				return true;
			}
		}

		protected internal override bool isEnabled(LogisimMenuItem item)
		{
			bool? value = enabled[item];
			return value != null && value.Value;
		}

		protected internal override void fire(LogisimMenuItem item)
		{
			if (item == LogisimMenuBar.CUT)
			{
				handler.cut();
			}
			else if (item == LogisimMenuBar.COPY)
			{
				handler.copy();
			}
			else if (item == LogisimMenuBar.DELETE)
			{
				handler.delete();
			}
			else if (item == LogisimMenuBar.DUPLICATE)
			{
				handler.duplicate();
			}
			else if (item == LogisimMenuBar.RAISE)
			{
				handler.raise();
			}
			else if (item == LogisimMenuBar.LOWER)
			{
				handler.lower();
			}
			else if (item == LogisimMenuBar.RAISE_TOP)
			{
				handler.raHashSetop();
			}
			else if (item == LogisimMenuBar.LOWER_BOTTOM)
			{
				handler.lowerBottom();
			}
			else if (item == LogisimMenuBar.ADD_CONTROL)
			{
				handler.addControlPoint();
			}
			else if (item == LogisimMenuBar.REMOVE_CONTROL)
			{
				handler.removeControlPoint();
			}
		}
	}

}
