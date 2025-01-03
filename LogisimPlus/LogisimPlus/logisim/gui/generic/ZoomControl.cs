// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{


	public class ZoomControl : JPanel
	{
		private class SpinnerModel : AbstractSpinnerModel, PropertyChangeListener
		{
			private readonly ZoomControl outerInstance;

			public SpinnerModel(ZoomControl outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual object NextValue
			{
				get
				{
					double zoom = outerInstance.model.ZoomFactor;
					double[] choices = outerInstance.model.ZoomOptions;
					double factor = zoom * 100.0 * 1.001;
					for (int i = 0; i < choices.Length; i++)
					{
						if (choices[i] > factor)
						{
							return toString(choices[i]);
						}
					}
					return null;
				}
			}

			public virtual object PreviousValue
			{
				get
				{
					double zoom = outerInstance.model.ZoomFactor;
					double[] choices = outerInstance.model.ZoomOptions;
					double factor = zoom * 100.0 * 0.999;
					for (int i = choices.Length - 1; i >= 0; i--)
					{
						if (choices[i] < factor)
						{
							return toString(choices[i]);
						}
					}
					return null;
				}
			}

			public virtual object Value
			{
				get
				{
					double zoom = outerInstance.model.ZoomFactor;
					return toString(zoom * 100.0);
				}
				set
				{
					if (value is string)
					{
						string s = (string) value;
						if (s.EndsWith("%", StringComparison.Ordinal))
						{
							s = s.Substring(0, s.Length - 1);
						}
						s = s.Trim();
						try
						{
							double zoom = double.Parse(s) / 100.0;
							outerInstance.model.ZoomFactor = zoom;
						}
						catch (System.FormatException)
						{
						}
					}
				}
			}

			internal virtual string toString(double factor)
			{
				if (factor > 10)
				{
					return (int)(factor + 0.5) + "%";
				}
				else if (factor > 0.1)
				{
					return (int)(factor * 100 + 0.5) / 100.0 + "%";
				}
				else
				{
					return factor + "%";
				}
			}


			public virtual void propertyChange(PropertyChangeEvent evt)
			{
				fireStateChanged();
			}
		}

		private class GridIcon : JComponent, MouseListener, PropertyChangeListener
		{
			private readonly ZoomControl outerInstance;

			internal bool state = true;

			public GridIcon(ZoomControl outerInstance)
			{
				this.outerInstance = outerInstance;
				addMouseListener(this);
				setPreferredSize(new Size(15, 15));
				setToolTipText("");
				setFocusable(true);
			}

			public override string getToolTipText(MouseEvent e)
			{
				return Strings.get("zoomShowGrid");
			}

			internal virtual void update()
			{
				bool grid = outerInstance.model.ShowGrid;
				if (grid != state)
				{
					state = grid;
					repaint();
				}
			}

			protected internal override void paintComponent(JGraphics g)
			{
				int width = getWidth();
				int height = getHeight();
				g.setColor(state ? Color.Black : getBackground().darker());
				int dim = (Math.Min(width, height) - 4) / 3 * 3 + 1;
				int xoff = (width - dim) / 2;
				int yoff = (height - dim) / 2;
				for (int x = 0; x < dim; x += 3)
				{
					for (int y = 0; y < dim; y += 3)
					{
						g.drawLine(x + xoff, y + yoff, x + xoff, y + yoff);
					}
				}
			}

			public virtual void mouseClicked(MouseEvent e)
			{
			}

			public virtual void mouseEntered(MouseEvent e)
			{
			}

			public virtual void mouseExited(MouseEvent e)
			{
			}

			public virtual void mouseReleased(MouseEvent e)
			{
			}

			public virtual void mousePressed(MouseEvent e)
			{
				outerInstance.model.ShowGrid = !state;
			}

			public virtual void propertyChange(PropertyChangeEvent evt)
			{
				update();
			}
		}

		private ZoomModel model;
		private JSpinner spinner;
		private SpinnerModel spinnerModel;
		private GridIcon grid;

		public ZoomControl(ZoomModel model) : base(new BorderLayout())
		{
			this.model = model;

			spinnerModel = new SpinnerModel(this);
			spinner = new JSpinner();
			spinner.setModel(spinnerModel);
			this.add(spinner, BorderLayout.CENTER);

			grid = new GridIcon(this);
			this.add(grid, BorderLayout.EAST);
			grid.update();

			model.addPropertyChangeListener(ZoomModel.SHOW_GRID, grid);
			model.addPropertyChangeListener(ZoomModel.ZOOM, spinnerModel);
		}

		public virtual ZoomModel ZoomModel
		{
			set
			{
				ZoomModel oldModel = model;
				if (oldModel != value)
				{
					if (oldModel != null)
					{
						oldModel.removePropertyChangeListener(ZoomModel.SHOW_GRID, grid);
						oldModel.removePropertyChangeListener(ZoomModel.ZOOM, spinnerModel);
					}
					model = value;
					spinnerModel = new SpinnerModel(this);
					spinner.setModel(spinnerModel);
					grid.update();
					if (value != null)
					{
						value.addPropertyChangeListener(ZoomModel.SHOW_GRID, grid);
						value.addPropertyChangeListener(ZoomModel.ZOOM, spinnerModel);
					}
				}
			}
		}
	}

}
