// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.toolbar
{


	public class Toolbar : JPanel
	{
		public static readonly object VERTICAL = new object();
		public static readonly object HORIZONTAL = new object();

		private class MyListener : ToolbarModelListener
		{
			private readonly Toolbar outerInstance;

			public MyListener(Toolbar outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void toolbarAppearanceChanged(ToolbarModelEvent @event)
			{
				repaint();
			}

			public virtual void toolbarContentsChanged(ToolbarModelEvent @event)
			{
				outerInstance.computeContents();
			}
		}

		private ToolbarModel model;
		private JPanel subpanel;
		private object orientation;
		private MyListener myListener;
		private ToolbarButton curPressed;

		public Toolbar(ToolbarModel model) : base(new BorderLayout())
		{
			this.subpanel = new JPanel();
			this.model = model;
			this.orientation = HORIZONTAL;
			this.myListener = new MyListener(this);
			this.curPressed = null;

			this.add(new JPanel(), BorderLayout.CENTER);
			Orientation = HORIZONTAL;

			computeContents();
			if (model != null)
			{
				model.addToolbarModelListener(myListener);
			}
		}

		public virtual ToolbarModel ToolbarModel
		{
			get
			{
				return model;
			}
			set
			{
				ToolbarModel oldValue = model;
				if (value != oldValue)
				{
					if (oldValue != null)
					{
						oldValue.removeToolbarModelListener(myListener);
					}
					if (value != null)
					{
						value.addToolbarModelListener(myListener);
					}
					model = value;
					computeContents();
				}
			}
		}


		public virtual object Orientation
		{
			set
			{
				int axis;
				string position;
				if (value == HORIZONTAL)
				{
					axis = BoxLayout.X_AXIS;
					position = BorderLayout.LINE_START;
				}
				else if (value == VERTICAL)
				{
					axis = BoxLayout.Y_AXIS;
					position = BorderLayout.NORTH;
				}
				else
				{
					throw new System.ArgumentException();
				}
				this.remove(subpanel);
				subpanel.setLayout(new BoxLayout(subpanel, axis));
				this.add(subpanel, position);
				this.orientation = value;
			}
			get
			{
				return orientation;
			}
		}

		private void computeContents()
		{
			subpanel.removeAll();
			ToolbarModel m = model;
			if (m != null)
			{
				foreach (ToolbarItem item in m.Items)
				{
					subpanel.add(new ToolbarButton(this, item));
				}
				subpanel.add(Box.createGlue());
			}
			revalidate();
		}

		internal virtual ToolbarButton? Pressed
		{
			get
			{
				return curPressed;
			}
			set
			{
				ToolbarButton oldValue = curPressed;
				if (oldValue != value)
				{
					curPressed = value;
					if (oldValue != null)
					{
						oldValue.repaint();
					}
					if (value != null)
					{
						value.repaint();
					}
				}
			}
		}


	}

}
