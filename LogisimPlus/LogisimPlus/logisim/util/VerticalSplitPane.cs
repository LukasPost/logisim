// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{


	public class VerticalSplitPane : JPanel
	{
		private class MyLayout : LayoutManager
		{
			private readonly VerticalSplitPane outerInstance;

			public MyLayout(VerticalSplitPane outerInstance)
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
				return new Size(@in.left + d0.width + d1.width + @in.right, @in.top + Math.Max(d0.height, d1.height) + @in.bottom);
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
				return new Size(@in.left + d0.width + d1.width + @in.right, @in.top + Math.Max(d0.height, d1.height) + @in.bottom);
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
					split = (int) (long)Math.Round(maxWidth * outerInstance.fraction, MidpointRounding.AwayFromZero);
					split = Math.Min(split, maxWidth - outerInstance.comp1.getMinimumSize().width);
					split = Math.Max(split, outerInstance.comp0.getMinimumSize().width);
				}

				outerInstance.comp0.setBounds(@in.left, @in.top, split, maxHeight);
				outerInstance.comp1.setBounds(@in.left + split, @in.top, maxWidth - split, maxHeight);
				outerInstance.dragbar.setBounds(@in.left + split - HorizontalSplitPane.DRAG_TOLERANCE, @in.top, 2 * HorizontalSplitPane.DRAG_TOLERANCE, maxHeight);
			}
		}

		private class MyDragbar : HorizontalSplitPane.Dragbar
		{
			private readonly VerticalSplitPane outerInstance;

			internal MyDragbar(VerticalSplitPane outerInstance)
			{
				this.outerInstance = outerInstance;
				setCursor(Cursor.getPredefinedCursor(Cursor.E_RESIZE_CURSOR));
			}

			internal override int getDragValue(MouseEvent e)
			{
				return getX() + e.getX() - outerInstance.getInsets().left;
			}

			internal override int DragValue
			{
				set
				{
					Insets @in = outerInstance.getInsets();
					outerInstance.Fraction = (double) value / (outerInstance.getWidth() - @in.left - @in.right);
					revalidate();
				}
			}
		}

		private JComponent comp0;
		private JComponent comp1;
		private MyDragbar dragbar;
		private double fraction;

		public VerticalSplitPane(JComponent comp0, JComponent comp1) : this(comp0, comp1, 0.5)
		{
		}

		public VerticalSplitPane(JComponent comp0, JComponent comp1, double fraction)
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
