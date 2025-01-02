// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{


	public class HorizontalSplitPane : JPanel
	{
		internal const int DRAG_TOLERANCE = 3;
		private static readonly Color DRAG_COLOR = new Color(0, 0, 0, 128);

		internal abstract class Dragbar : JComponent, MouseListener, MouseMotionListener
		{
			internal bool dragging = false;
			internal int curValue;

			internal Dragbar()
			{
				addMouseListener(this);
				addMouseMotionListener(this);
			}

			internal abstract int getDragValue(MouseEvent e);

			internal abstract int DragValue {set;}

			public override void paintComponent(Graphics g)
			{
				if (dragging)
				{
					g.setColor(DRAG_COLOR);
					g.fillRect(0, 0, getWidth(), getHeight());
				}
			}

			public virtual void mouseClicked(MouseEvent e)
			{
			}

			public virtual void mousePressed(MouseEvent e)
			{
				if (!dragging)
				{
					curValue = getDragValue(e);
					dragging = true;
					repaint();
				}
			}

			public virtual void mouseReleased(MouseEvent e)
			{
				if (dragging)
				{
					dragging = false;
					int newValue = getDragValue(e);
					if (newValue != curValue)
					{
						DragValue = newValue;
					}
					repaint();
				}
			}

			public virtual void mouseEntered(MouseEvent e)
			{
			}

			public virtual void mouseExited(MouseEvent e)
			{
			}

			public virtual void mouseDragged(MouseEvent e)
			{
				if (dragging)
				{
					int newValue = getDragValue(e);
					if (newValue != curValue)
					{
						DragValue = newValue;
					}
				}
			}

			public virtual void mouseMoved(MouseEvent e)
			{
			}
		}

		private class MyLayout : LayoutManager
		{
			private readonly HorizontalSplitPane outerInstance;

			public MyLayout(HorizontalSplitPane outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void addLayoutComponent(string name, Component comp)
			{
			}

			public virtual void removeLayoutComponent(Component comp)
			{
			}

			public virtual Size preferredLayoutSize(Container parent)
			{
				if (outerInstance.fraction <= 0.0)
				{
					return outerInstance.comp1.getPreferredSize();
				}
				if (outerInstance.fraction >= 1.0)
				{
					return outerInstance.comp0.getPreferredSize();
				}
				Insets @in = parent.getInsets();
				Size d0 = outerInstance.comp0.getPreferredSize();
				Size d1 = outerInstance.comp1.getPreferredSize();
				return new Size(@in.left + Math.Max(d0.width, d1.width) + @in.right, @in.top + d0.height + d1.height + @in.bottom);
			}

			public virtual Size minimumLayoutSize(Container parent)
			{
				if (outerInstance.fraction <= 0.0)
				{
					return outerInstance.comp1.getMinimumSize();
				}
				if (outerInstance.fraction >= 1.0)
				{
					return outerInstance.comp0.getMinimumSize();
				}
				Insets @in = parent.getInsets();
				Size d0 = outerInstance.comp0.getMinimumSize();
				Size d1 = outerInstance.comp1.getMinimumSize();
				return new Size(@in.left + Math.Max(d0.width, d1.width) + @in.right, @in.top + d0.height + d1.height + @in.bottom);
			}

			public virtual void layoutContainer(Container parent)
			{
				Insets @in = parent.getInsets();
				int maxWidth = parent.getWidth() - (@in.left + @in.right);
				int maxHeight = parent.getHeight() - (@in.top + @in.bottom);
				int split;
				if (outerInstance.fraction <= 0.0)
				{
					split = 0;
				}
				else if (outerInstance.fraction >= 1.0)
				{
					split = maxWidth;
				}
				else
				{
					split = (int) (long)Math.Round(maxHeight * outerInstance.fraction, MidpointRounding.AwayFromZero);
					split = Math.Min(split, maxHeight - outerInstance.comp1.getMinimumSize().height);
					split = Math.Max(split, outerInstance.comp0.getMinimumSize().height);
				}

				outerInstance.comp0.setBounds(@in.left, @in.top, maxWidth, split);
				outerInstance.comp1.setBounds(@in.left, @in.top + split, maxWidth, maxHeight - split);
				outerInstance.dragbar.setBounds(@in.left, @in.top + split - DRAG_TOLERANCE, maxWidth, 2 * DRAG_TOLERANCE);
			}
		}

		private class MyDragbar : Dragbar
		{
			private readonly HorizontalSplitPane outerInstance;

			internal MyDragbar(HorizontalSplitPane outerInstance)
			{
				this.outerInstance = outerInstance;
				setCursor(Cursor.getPredefinedCursor(Cursor.S_RESIZE_CURSOR));
			}

			internal override int getDragValue(MouseEvent e)
			{
				return getY() + e.getY() - outerInstance.getInsets().top;
			}

			internal override int DragValue
			{
				set
				{
					Insets @in = outerInstance.getInsets();
					outerInstance.Fraction = (double) value / (outerInstance.getHeight() - @in.bottom - @in.top);
					revalidate();
				}
			}
		}

		private JComponent comp0;
		private JComponent comp1;
		private MyDragbar dragbar;
		private double fraction;

		public HorizontalSplitPane(JComponent comp0, JComponent comp1) : this(comp0, comp1, 0.5)
		{
		}

		public HorizontalSplitPane(JComponent comp0, JComponent comp1, double fraction)
		{
			this.comp0 = comp0;
			this.comp1 = comp1;
			this.dragbar = new MyDragbar(this); // above the other components
			this.fraction = fraction;

			setLayout(new MyLayout(this));
			add(dragbar); // above the other components
			add(comp0);
			add(comp1);
		}

		public virtual double Fraction
		{
			get
			{
				return fraction;
			}
			set
			{
				if (value < 0.0)
				{
					value = 0.0;
				}
				if (value > 1.0)
				{
					value = 1.0;
				}
				if (fraction != value)
				{
					fraction = value;
					revalidate();
				}
			}
		}

	}

}
