﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{


	using MacCompatibility = logisim.util.MacCompatibility;

	public class CanvasPane : JScrollPane
	{
		private class Listener : ComponentListener, PropertyChangeListener
		{
			private readonly CanvasPane outerInstance;

			public Listener(CanvasPane outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			//
			// ComponentListener methods
			//
			public virtual void componentResized(ComponentEvent e)
			{
				outerInstance.contents.recomputeSize();
			}

			public virtual void componentMoved(ComponentEvent e)
			{
			}

			public virtual void componentShown(ComponentEvent e)
			{
			}

			public virtual void componentHidden(ComponentEvent e)
			{
			}

			public virtual void propertyChange(PropertyChangeEvent e)
			{
				string prop = e.getPropertyName();
				if (prop.Equals(ZoomModel.ZOOM))
				{
					double oldZoom = ((double?) e.getOldValue()).Value;
					Rectangle r = getViewport().getViewRect();
					double cx = (r.X + r.Width / 2) / oldZoom;
					double cy = (r.Y + r.Height / 2) / oldZoom;

					double newZoom = ((double?) e.getNewValue()).Value;
					outerInstance.contents.recomputeSize();
					r = getViewport().getViewRect();
					int hv = (int)(cx * newZoom) - r.Width / 2;
					int vv = (int)(cy * newZoom) - r.Height / 2;
					getHorizontalScrollBar().setValue(hv);
					getVerticalScrollBar().setValue(vv);
				}
			}
		}

		private CanvasPaneContents contents;
		private Listener listener;
		private ZoomModel zoomModel;

		public CanvasPane(CanvasPaneContents contents) : base((Component) contents)
		{
			this.contents = contents;
			this.listener = new Listener(this);
			this.zoomModel = null;
			if (MacCompatibility.mrjVersion >= 0.0)
			{
				setVerticalScrollBarPolicy(JScrollPane.VERTICAL_SCROLLBAR_ALWAYS);
				setHorizontalScrollBarPolicy(JScrollPane.HORIZONTAL_SCROLLBAR_ALWAYS);
			}

			addComponentListener(listener);
			contents.CanvasPane = this;
		}

		public virtual ZoomModel ZoomModel
		{
			set
			{
				ZoomModel oldModel = zoomModel;
				if (oldModel != null)
				{
					oldModel.removePropertyChangeListener(ZoomModel.ZOOM, listener);
				}
				zoomModel = value;
				if (value != null)
				{
					value.addPropertyChangeListener(ZoomModel.ZOOM, listener);
				}
			}
		}

		public virtual double ZoomFactor
		{
			get
			{
				ZoomModel model = zoomModel;
				return model == null ? 1.0 : model.ZoomFactor;
			}
		}

		public virtual Size ViewportSize
		{
			get
			{
				Size size = new Size();
				getViewport().getSize(size);
				return size;
			}
		}

		public virtual int supportScrollableBlockIncrement(Rectangle visibleRect, int orientation, int direction)
		{
			int unit = supportScrollableUnitIncrement(visibleRect, orientation, direction);
			if (direction == SwingConstants.VERTICAL)
			{
				return visibleRect.height / unit * unit;
			}
			else
			{
				return visibleRect.width / unit * unit;
			}
		}

		public virtual int supportScrollableUnitIncrement(Rectangle visibleRect, int orientation, int direction)
		{
			double zoom = ZoomFactor;
			return (int) (long)Math.Round(10 * zoom, MidpointRounding.AwayFromZero);
		}

		public virtual Size supportPreferredSize(int width, int height)
		{
			double zoom = ZoomFactor;
			if (zoom != 1.0)
			{
				width = (int) Math.Ceiling(width * zoom);
				height = (int) Math.Ceiling(height * zoom);
			}
			Size minSize = ViewportSize;
			if (minsize.Width > width)
			{
				width = minsize.Width;
			}
			if (minsize.Height > height)
			{
				height = minsize.Height;
			}
			return new Size(width, height);
		}
	}

}
