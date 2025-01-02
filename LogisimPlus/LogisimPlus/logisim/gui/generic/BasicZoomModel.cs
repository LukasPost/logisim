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

	using logisim.prefs;

	public class BasicZoomModel : ZoomModel
	{
		private double[] zoomOptions;

		private PropertyChangeSupport support;
		private double zoomFactor;
		private bool showGrid;

		public BasicZoomModel(PrefMonitor<bool> gridPref, PrefMonitor<double> zoomPref, double[] zoomOpts)
		{
			zoomOptions = zoomOpts;
			support = new PropertyChangeSupport(this);
			zoomFactor = 1.0;
			showGrid = true;

			ZoomFactor = zoomPref.get().doubleValue();
			ShowGrid = gridPref.Boolean;
		}

		public virtual void addPropertyChangeListener(string prop, PropertyChangeListener l)
		{
			support.addPropertyChangeListener(prop, l);
		}

		public virtual void removePropertyChangeListener(string prop, PropertyChangeListener l)
		{
			support.removePropertyChangeListener(prop, l);
		}

		public virtual bool ShowGrid
		{
			get
			{
				return showGrid;
			}
			set
			{
				if (value != showGrid)
				{
					showGrid = value;
					support.firePropertyChange(ZoomModel.SHOW_GRID, !value, value);
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
				if (value != oldValue)
				{
					zoomFactor = value;
					support.firePropertyChange(ZoomModel.ZOOM, Convert.ToDouble(oldValue), Convert.ToDouble(value));
				}
			}
		}

		public virtual double[] ZoomOptions
		{
			get
			{
				return zoomOptions;
			}
		}


	}

}
