// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{


	public class CardPanel : JPanel
	{
		private List<ChangeListener> listeners;
		private string current;

		public CardPanel() : base(new CardLayout())
		{
			listeners = new List<ChangeListener>();
			current = "";
		}

		public virtual void addChangeListener(ChangeListener listener)
		{
			listeners.Add(listener);
		}

		public virtual void addView(string name, Component comp)
		{
			add(comp, name);
		}

		public virtual string View
		{
			get
			{
				return current;
			}
			set
			{
				if (string.ReferenceEquals(value, null))
				{
					value = "";
				}
				string oldChoice = current;
				if (!oldChoice.Equals(value))
				{
					current = value;
					((CardLayout) getLayout()).show(this, value);
					ChangeEvent e = new ChangeEvent(this);
					foreach (ChangeListener listener in listeners)
					{
						listener.stateChanged(e);
					}
				}
			}
		}


	}

}
