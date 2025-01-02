// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.canvas
{


	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using Action = draw.undo.Action;

	public class Canvas : JComponent
	{
		public const string TOOL_PROPERTY = "tool";
		public const string MODEL_PROPERTY = "model";

		private CanvasModel model;
		private ActionDispatcher dispatcher;
		private CanvasListener listener;
		private Selection selection;

		public Canvas()
		{
			model = null;
			listener = new CanvasListener(this);
			selection = new Selection();

			addMouseListener(listener);
			addMouseMotionListener(listener);
			addKeyListener(listener);
			setPreferredSize(new Size(200, 200));
		}

		public virtual CanvasModel Model
		{
			get
			{
				return model;
			}
		}

		public virtual CanvasTool Tool
		{
			get
			{
				return listener.Tool;
			}
			set
			{
				CanvasTool oldValue = listener.Tool;
				if (value != oldValue)
				{
					listener.Tool = value;
					firePropertyChange(TOOL_PROPERTY, oldValue, value);
				}
			}
		}

		public virtual void toolGestureComplete(CanvasTool tool, CanvasObject created)
		{
			; // nothing to do - subclass may override
		}

		protected internal virtual JPopupMenu showPopupMenu(MouseEvent e, CanvasObject clicked)
		{
			return null; // subclass will override if it supports popup menus
		}

		public virtual Selection Selection
		{
			get
			{
				return selection;
			}
			set
			{
				selection = value;
				repaint();
			}
		}


		public virtual void doAction(Action action)
		{
			dispatcher.doAction(action);
		}

		public virtual void setModel(CanvasModel value, ActionDispatcher dispatcher)
		{
			CanvasModel oldValue = model;
			if (oldValue != value)
			{
				if (oldValue != null)
				{
					oldValue.removeCanvasModelListener(listener);
				}
				model = value;
				this.dispatcher = dispatcher;
				if (value != null)
				{
					value.addCanvasModelListener(listener);
				}
				selection.clearSelected();
				repaint();
				firePropertyChange(MODEL_PROPERTY, oldValue, value);
			}
		}


		public virtual void repaintCanvasCoords(int x, int y, int width, int height)
		{
			repaint(x, y, width, height);
		}

		public virtual double ZoomFactor
		{
			get
			{
				return 1.0; // subclass will have to override this
			}
		}

		public virtual int snapX(int x)
		{
			return x; // subclass will have to override this
		}

		public virtual int snapY(int y)
		{
			return y; // subclass will have to override this
		}

		public override void paintComponent(Graphics g)
		{
			paintBackground(g);
			paintForeground(g);
		}

		protected internal virtual void paintBackground(Graphics g)
		{
			g.clearRect(0, 0, getWidth(), getHeight());
		}

		protected internal virtual void paintForeground(Graphics g)
		{
			CanvasModel model = this.model;
			CanvasTool tool = listener.Tool;
			if (model != null)
			{
				Graphics dup = g.create();
				model.paint(g, selection);
				dup.dispose();
			}
			if (tool != null)
			{
				Graphics dup = g.create();
				tool.draw(this, dup);
				dup.dispose();
			}
		}
	}

}
