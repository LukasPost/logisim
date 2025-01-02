// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{

	public class GridPainter
	{
		public const string ZOOM_PROPERTY = "zoom";
		public const string SHOW_GRID_PROPERTY = "showgrid";

		private const int GRID_DOT_COLOR = unchecked((int)0xFF777777);
		private const int GRID_DOT_ZOOMED_COLOR = unchecked((int)0xFFCCCCCC);

		private static readonly Color GRID_ZOOMED_OUT_COLOR = new Color(210, 210, 210);

		private class Listener : PropertyChangeListener
		{
			private readonly GridPainter outerInstance;

			public Listener(GridPainter outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				string prop = @event.getPropertyName();
				object val = @event.getNewValue();
				if (prop.Equals(ZoomModel.ZOOM))
				{
					outerInstance.ZoomFactor = ((double?) val).Value;
					outerInstance.destination.repaint();
				}
				else if (prop.Equals(ZoomModel.SHOW_GRID))
				{
					outerInstance.ShowGrid = ((bool?) val).Value;
					outerInstance.destination.repaint();
				}
			}
		}

		private Component destination;
		private PropertyChangeSupport support;
		private Listener listener;
		private ZoomModel zoomModel;
		private bool showGrid;
		private int gridSize;
		private double zoomFactor;
		private Image gridImage;
		private int gridImageWidth;

		public GridPainter(Component destination)
		{
			this.destination = destination;
			support = new PropertyChangeSupport(this);
			showGrid = true;
			gridSize = 10;
			zoomFactor = 1.0;
			updateGridImage(gridSize, zoomFactor);
		}

		public virtual void addPropertyChangeListener(string prop, PropertyChangeListener listener)
		{
			support.addPropertyChangeListener(prop, listener);
		}

		public virtual void removePropertyChangeListener(string prop, PropertyChangeListener listener)
		{
			support.removePropertyChangeListener(prop, listener);
		}

		public virtual bool ShowGrid
		{
			get
			{
				return showGrid;
			}
			set
			{
				if (showGrid != value)
				{
					showGrid = value;
					support.firePropertyChange(SHOW_GRID_PROPERTY, !value, value);
				}
			}
		}


		public virtual double ZoomFactor
		{
			get
			{
				return zoomFactor;
			}
			set
			{
				double oldValue = zoomFactor;
				if (oldValue != value)
				{
					zoomFactor = value;
					updateGridImage(gridSize, value);
					support.firePropertyChange(ZOOM_PROPERTY, Convert.ToDouble(oldValue), Convert.ToDouble(value));
				}
			}
		}


		public virtual ZoomModel ZoomModel
		{
			get
			{
				return zoomModel;
			}
			set
			{
				ZoomModel old = zoomModel;
				if (value != old)
				{
					if (listener == null)
					{
						listener = new Listener(this);
					}
					if (old != null)
					{
						old.removePropertyChangeListener(ZoomModel.ZOOM, listener);
						old.removePropertyChangeListener(ZoomModel.SHOW_GRID, listener);
					}
					zoomModel = value;
					if (value != null)
					{
						value.addPropertyChangeListener(ZoomModel.ZOOM, listener);
						value.addPropertyChangeListener(ZoomModel.SHOW_GRID, listener);
						ShowGrid = value.ShowGrid;
						ZoomFactor = value.ZoomFactor;
					}
					destination.repaint();
				}
			}
		}


		public virtual void paintGrid(Graphics g)
		{
			Rectangle clip = g.getClipBounds();
			Component dest = destination;
			double zoom = zoomFactor;
			int size = gridSize;

			if (!showGrid)
			{
				return;
			}

			Image img = gridImage;
			int w = gridImageWidth;
			if (img == null)
			{
				paintGridOld(g, size, zoom, clip);
				return;
			}
			int x0 = (clip.x / w) * w; // round down to multiple of w
			int y0 = (clip.y / w) * w;
			for (int x = 0; x < clip.width + w; x += w)
			{
				for (int y = 0; y < clip.height + w; y += w)
				{
					g.drawImage(img, x0 + x, y0 + y, dest);
				}
			}
		}

		private void paintGridOld(Graphics g, int size, double f, Rectangle clip)
		{
			g.setColor(Color.GRAY);
			if (f == 1.0)
			{
				int start_x = ((clip.x + 9) / size) * size;
				int start_y = ((clip.y + 9) / size) * size;
				for (int x = 0; x < clip.width; x += size)
				{
					for (int y = 0; y < clip.height; y += size)
					{
						g.fillRect(start_x + x, start_y + y, 1, 1);
					}
				}
			}
			else
			{
				/* Kevin Walsh of Cornell suggested the code below instead. */
				int x0 = size * (int) Math.Ceiling(clip.x / f / size);
				int x1 = x0 + (int)(clip.width / f);
				int y0 = size * (int) Math.Ceiling(clip.y / f / size);
				int y1 = y0 + (int)(clip.height / f);
				if (f <= 0.5)
				{
					g.setColor(GRID_ZOOMED_OUT_COLOR);
				}
				for (double x = x0; x < x1; x += size)
				{
					for (double y = y0; y < y1; y += size)
					{
						int sx = (int) (long)Math.Round(f * x, MidpointRounding.AwayFromZero);
						int sy = (int) (long)Math.Round(f * y, MidpointRounding.AwayFromZero);
						g.fillRect(sx, sy, 1, 1);
					}
				}
				if (f <= 0.5)
				{ // make every 5th pixel darker
					int size5 = 5 * size;
					g.setColor(Color.GRAY);
					x0 = size5 * (int) Math.Ceiling(clip.x / f / size5);
					y0 = size5 * (int) Math.Ceiling(clip.y / f / size5);
					for (double x = x0; x < x1; x += size5)
					{
						for (double y = y0; y < y1; y += size5)
						{
							int sx = (int) (long)Math.Round(f * x, MidpointRounding.AwayFromZero);
							int sy = (int) (long)Math.Round(f * y, MidpointRounding.AwayFromZero);
							g.fillRect(sx, sy, 1, 1);
						}
					}
				}

				/*
				 * Original code by Carl Burch int x0 = 10 * (int) Math.ceil(clip.x / f / 10); int x1 = x0 +
				 * (int)(clip.width / f); int y0 = 10 * (int) Math.ceil(clip.y / f / 10); int y1 = y0 + (int) (clip.height /
				 * f); int s = f > 0.5 ? 1 : f > 0.25 ? 2 : 3; int i0 = s - ((x0 + 10*s - 1) % (s * 10)) / 10 - 1; int j0 =
				 * s - ((y1 + 10*s - 1) % (s * 10)) / 10 - 1; for (int i = 0; i < s; i++) { for (int x = x0+i*10; x < x1; x
				 * += s*10) { for (int j = 0; j < s; j++) { g.setColor(i == i0 && j == j0 ? Color.gray :
				 * GRID_ZOOMED_OUT_COLOR); for (int y = y0+j*10; y < y1; y += s*10) { int sx = (int) Math.round(f * x); int
				 * sy = (int) Math.round(f * y); g.fillRect(sx, sy, 1, 1); } } } }
				 */
			}
		}

		//
		// creating the grid image
		//
		private void updateGridImage(int size, double f)
		{
			double ww = f * size * 5;
			while (2 * ww < 150)
			{
				ww *= 2;
			}
			int w = (int) (long)Math.Round(ww, MidpointRounding.AwayFromZero);
			int[] pix = new int[w * w];
			Arrays.Fill(pix, 0xFFFFFF);

			if (f == 1.0)
			{
				int lineStep = size * w;
				for (int j = 0; j < pix.Length; j += lineStep)
				{
					for (int i = 0; i < w; i += size)
					{
						pix[i + j] = GRID_DOT_COLOR;
					}
				}
			}
			else
			{
				int off0 = 0;
				int off1 = 1;
				if (f >= 2.0)
				{ // we'll draw several pixels for each grid point
					int num = (int)(f + 0.001);
					off0 = -(num / 2);
					off1 = off0 + num;
				}

				int dotColor = f <= 0.5 ? GRID_DOT_ZOOMED_COLOR : GRID_DOT_COLOR;
				for (int j = 0; true; j += size)
				{
					int y = (int) (long)Math.Round(f * j, MidpointRounding.AwayFromZero);
					if (y + off0 >= w)
					{
						break;
					}

					for (int yo = y + off0; yo < y + off1; yo++)
					{
						if (yo >= 0 && yo < w)
						{
							int @base = yo * w;
							for (int i = 0; true; i += size)
							{
								int x = (int) (long)Math.Round(f * i, MidpointRounding.AwayFromZero);
								if (x + off0 >= w)
								{
									break;
								}
								for (int xo = x + off0; xo < x + off1; xo++)
								{
									if (xo >= 0 && xo < w)
									{
										pix[@base + xo] = dotColor;
									}
								}
							}
						}
					}
				}
				if (f <= 0.5)
				{ // repaint over every 5th pixel so it is darker
					int size5 = size * 5;
					for (int j = 0; true; j += size5)
					{
						int y = (int) (long)Math.Round(f * j, MidpointRounding.AwayFromZero);
						if (y >= w)
						{
							break;
						}
						y *= w;

						for (int i = 0; true; i += size5)
						{
							int x = (int) (long)Math.Round(f * i, MidpointRounding.AwayFromZero);
							if (x >= w)
							{
								break;
							}
							pix[y + x] = GRID_DOT_COLOR;
						}
					}
				}
			}
			gridImage = destination.createImage(new MemoryImageSource(w, w, pix, 0, w));
			gridImageWidth = w;
		}
	}

}
