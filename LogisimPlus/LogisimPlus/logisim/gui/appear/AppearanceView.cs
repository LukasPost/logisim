// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{
	using Canvas = draw.canvas.Canvas;
	using AttrTableDrawManager = draw.gui.AttrTableDrawManager;
	using ToolbarModel = draw.toolbar.ToolbarModel;
	using DrawingAttributeSet = draw.tools.DrawingAttributeSet;
	using SelectTool = draw.tools.SelectTool;
	using CircuitState = logisim.circuit.CircuitState;
	using AttributeSet = logisim.data.AttributeSet;
	using AttrTable = logisim.gui.generic.AttrTable;
	using BasicZoomModel = logisim.gui.generic.BasicZoomModel;
	using CanvasPane = logisim.gui.generic.CanvasPane;
	using ZoomModel = logisim.gui.generic.ZoomModel;
	using EditHandler = logisim.gui.main.EditHandler;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;

	public class AppearanceView
	{
		private static readonly double[] ZOOM_OPTIONS = new double[] {100, 150, 200, 300, 400, 600, 800};

		private DrawingAttributeSet attrs;
		private AppearanceCanvas canvas;
		private CanvasPane canvasPane;
		private AppearanceToolbarModel toolbarModel;
		private AttrTableDrawManager attrTableManager;
		private ZoomModel zoomModel;
		private AppearanceEditHandler editHandler;

		public AppearanceView()
		{
			attrs = new DrawingAttributeSet();
			SelectTool selectTool = new SelectTool();
			canvas = new AppearanceCanvas(selectTool);
			toolbarModel = new AppearanceToolbarModel(selectTool, canvas, attrs);
			zoomModel = new BasicZoomModel(AppPreferences.APPEARANCE_SHOW_GRID, AppPreferences.APPEARANCE_ZOOM, ZOOM_OPTIONS);
			canvas.GridPainter.ZoomModel = zoomModel;
			attrTableManager = null;
			canvasPane = new CanvasPane(canvas);
			canvasPane.ZoomModel = zoomModel;
			editHandler = new AppearanceEditHandler(canvas);
		}

		public virtual Canvas Canvas
		{
			get
			{
				return canvas;
			}
		}

		public virtual CanvasPane CanvasPane
		{
			get
			{
				return canvasPane;
			}
		}

		public virtual ToolbarModel ToolbarModel
		{
			get
			{
				return toolbarModel;
			}
		}

		public virtual ZoomModel ZoomModel
		{
			get
			{
				return zoomModel;
			}
		}

		public virtual EditHandler EditHandler
		{
			get
			{
				return editHandler;
			}
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
		}

		public virtual AttrTableDrawManager getAttrTableDrawManager(AttrTable table)
		{
			AttrTableDrawManager ret = attrTableManager;
			if (ret == null)
			{
				ret = new AttrTableDrawManager(canvas, table, attrs);
				attrTableManager = ret;
			}
			return ret;
		}

		public virtual void setCircuit(Project proj, CircuitState circuitState)
		{
			canvas.setCircuit(proj, circuitState);
		}
	}

}
