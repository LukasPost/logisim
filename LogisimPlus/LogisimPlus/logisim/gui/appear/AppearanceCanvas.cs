// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{

	using ModelAddAction = draw.actions.ModelAddAction;
	using ModelReorderAction = draw.actions.ModelReorderAction;
	using ActionDispatcher = draw.canvas.ActionDispatcher;
	using Canvas = draw.canvas.Canvas;
	using CanvasTool = draw.canvas.CanvasTool;
	using CanvasModel = draw.model.CanvasModel;
	using CanvasModelEvent = draw.model.CanvasModelEvent;
	using CanvasModelListener = draw.model.CanvasModelListener;
	using CanvasObject = draw.model.CanvasObject;
	using ReorderRequest = draw.model.ReorderRequest;
	using Action = draw.undo.Action;
	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using AppearanceElement = logisim.circuit.appear.AppearanceElement;
	using Bounds = logisim.data.Bounds;
	using CanvasPane = logisim.gui.generic.CanvasPane;
	using CanvasPaneContents = logisim.gui.generic.CanvasPaneContents;
	using GridPainter = logisim.gui.generic.GridPainter;
	using Project = logisim.proj.Project;

	public class AppearanceCanvas : Canvas, CanvasPaneContents, ActionDispatcher
	{
		private const int BOUNDS_BUFFER = 70;
		// pixels shown in canvas beyond outermost boundaries
		private const int THRESH_SIZE_UPDATE = 10;
		// don't bother to update the size if it hasn't changed more than this

		private class Listener : CanvasModelListener, PropertyChangeListener
		{
			private readonly AppearanceCanvas outerInstance;

			public Listener(AppearanceCanvas outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void modelChanged(CanvasModelEvent @event)
			{
				outerInstance.computeSize(false);
			}

			public virtual void propertyChange(PropertyChangeEvent evt)
			{
				string prop = evt.getPropertyName();
				if (prop.Equals(GridPainter.ZOOM_PROPERTY))
				{
					CanvasTool t = outerInstance.Tool;
					if (t != null)
					{
						t.zoomFactorChanged(outerInstance);
					}
				}
			}
		}

		private CanvasTool selectTool;
		private Project proj;
		private CircuitState circuitState;
		private Listener listener;
		private GridPainter grid;
		private CanvasPane canvasPane;
		private Bounds oldPreferredSize;
		private LayoutPopupManager popupManager;

		public AppearanceCanvas(CanvasTool selectTool)
		{
			this.selectTool = selectTool;
			this.grid = new GridPainter(this);
			this.listener = new Listener(this);
			this.oldPreferredSize = null;
			Selection = new AppearanceSelection();
			Tool = selectTool;

			CanvasModel model = base.Model;
			if (model != null)
			{
				model.addCanvasModelListener(listener);
			}
			grid.addPropertyChangeListener(GridPainter.ZOOM_PROPERTY, listener);
		}

		public override CanvasTool Tool
		{
			set
			{
				hidePopup();
				base.Tool = value;
			}
		}

		public override void toolGestureComplete(CanvasTool tool, CanvasObject created)
		{
			if (tool == Tool && tool != selectTool)
			{
				Tool = selectTool;
				if (created != null)
				{
					Selection.clearSelected();
					Selection.setSelected(created, true);
				}
			}
		}

		public override void setModel(CanvasModel value, ActionDispatcher dispatcher)
		{
			CanvasModel oldModel = base.Model;
			if (oldModel != null)
			{
				oldModel.removeCanvasModelListener(listener);
			}
			base.setModel(value, dispatcher);
			if (value != null)
			{
				value.addCanvasModelListener(listener);
			}
		}

		public virtual void setCircuit(Project proj, CircuitState circuitState)
		{
			this.proj = proj;
			this.circuitState = circuitState;
			Circuit circuit = circuitState.Circuit;
			setModel(circuit.Appearance, this);
		}

		internal virtual Project Project
		{
			get
			{
				return proj;
			}
		}

		internal virtual Circuit Circuit
		{
			get
			{
				return circuitState.Circuit;
			}
		}

		internal virtual CircuitState CircuitState
		{
			get
			{
				return circuitState;
			}
		}

		internal virtual GridPainter GridPainter
		{
			get
			{
				return grid;
			}
		}

		public override void doAction(Action canvasAction)
		{
			Circuit circuit = circuitState.Circuit;
			if (!proj.LogisimFile.contains(circuit))
			{
				return;
			}

			if (canvasAction is ModelReorderAction)
			{
				int max = getMaxIndex(Model);
				ModelReorderAction reorder = (ModelReorderAction) canvasAction;
				List<ReorderRequest> rs = reorder.ReorderRequests;
				List<ReorderRequest> mod = new List<ReorderRequest>(rs.Count);
				bool changed = false;
				bool movedToMax = false;
				foreach (ReorderRequest r in rs)
				{
					CanvasObject o = r.Object;
					if (o is AppearanceElement)
					{
						changed = true;
					}
					else
					{
						if (r.ToIndex > max)
						{
							int from = r.FromIndex;
							changed = true;
							movedToMax = true;
							if (from == max && !movedToMax)
							{
								; // this change is ineffective - don't add it
							}
							else
							{
								mod.Add(new ReorderRequest(o, from, max));
							}
						}
						else
						{
							if (r.ToIndex == max)
							{
								movedToMax = true;
							}
							mod.Add(r);
						}
					}
				}
				if (changed)
				{
					if (mod.Count == 0)
					{
						return;
					}
					canvasAction = new ModelReorderAction(Model, mod);
				}
			}

			if (canvasAction is ModelAddAction)
			{
				ModelAddAction addAction = (ModelAddAction) canvasAction;
				int cur = addAction.DestinationIndex;
				int max = getMaxIndex(Model);
				if (cur > max)
				{
					canvasAction = new ModelAddAction(Model, addAction.Objects, max + 1);
				}
			}

			proj.doAction(new CanvasActionAdapter(circuit, canvasAction));
		}

		public override double ZoomFactor
		{
			get
			{
				return grid.ZoomFactor;
			}
		}

		public override int snapX(int x)
		{
			if (x < 0)
			{
				return -((-x + 5) / 10 * 10);
			}
			else
			{
				return (x + 5) / 10 * 10;
			}
		}

		public override int snapY(int y)
		{
			if (y < 0)
			{
				return -((-y + 5) / 10 * 10);
			}
			else
			{
				return (y + 5) / 10 * 10;
			}
		}

		protected internal override void paintBackground(JGraphics g)
		{
			base.paintBackground(g);
			grid.paintGrid(g);
		}

		protected internal override void paintForeground(JGraphics g)
		{
			double zoom = grid.ZoomFactor;
			JGraphics gScaled = g.create();
			if (zoom != 1.0 && zoom != 0.0)
			{
				gScaled.scale(zoom, zoom);
			}
			base.paintForeground(gScaled);
			gScaled.dispose();
		}

		public override void repaintCanvasCoords(int x, int y, int width, int height)
		{
			double zoom = grid.ZoomFactor;
			if (zoom != 1.0)
			{
				x = (int)(x * zoom - 1);
				y = (int)(y * zoom - 1);
				width = (int)(width * zoom + 4);
				height = (int)(height * zoom + 4);
			}
			base.repaintCanvasCoords(x, y, width, height);
		}

		protected internal override void processMouseEvent(MouseEvent e)
		{
			repairEvent(e, grid.ZoomFactor);
			base.processMouseEvent(e);
		}

		public override JPopupMenu showPopupMenu(MouseEvent e, CanvasObject clicked)
		{
			double zoom = grid.ZoomFactor;
			int x = (int) (long)Math.Round(e.getX() * zoom, MidpointRounding.AwayFromZero);
			int y = (int) (long)Math.Round(e.getY() * zoom, MidpointRounding.AwayFromZero);
			if (clicked != null && Selection.isSelected(clicked))
			{
				AppearanceEditPopup popup = new AppearanceEditPopup(this);
				popup.show(this, x, y);
				return popup;
			}
			return null;
		}

		protected internal override void processMouseMotionEvent(MouseEvent e)
		{
			repairEvent(e, grid.ZoomFactor);
			base.processMouseMotionEvent(e);
		}

		private void hidePopup()
		{
			LayoutPopupManager man = popupManager;
			if (man != null)
			{
				man.hideCurrentPopup();
			}
		}

		private void repairEvent(MouseEvent e, double zoom)
		{
			if (zoom != 1.0)
			{
				int oldx = e.getX();
				int oldy = e.getY();
				int newx = (int) (long)Math.Round(e.getX() / zoom, MidpointRounding.AwayFromZero);
				int newy = (int) (long)Math.Round(e.getY() / zoom, MidpointRounding.AwayFromZero);
				e.translatePoint(newx - oldx, newy - oldy);
			}
		}

		private void computeSize(bool immediate)
		{
			hidePopup();
			Bounds bounds;
			CircuitState circState = circuitState;
			if (circState == null)
			{
				bounds = Bounds.create(0, 0, 50, 50);
			}
			else
			{
				bounds = circState.Circuit.Appearance.AbsoluteBounds;
			}
			int width = bounds.X + bounds.Width + BOUNDS_BUFFER;
			int height = bounds.Y + bounds.Height + BOUNDS_BUFFER;
			Size dim;
			if (canvasPane == null)
			{
				dim = new Size(width, height);
			}
			else
			{
				dim = canvasPane.supportPreferredSize(width, height);
			}
			if (!immediate)
			{
				Bounds old = oldPreferredSize;
				if (old != null && Math.Abs(old.Width - dim.Width) < THRESH_SIZE_UPDATE && Math.Abs(old.Height - dim.Height) < THRESH_SIZE_UPDATE)
				{
					return;
				}
			}
			oldPreferredSize = Bounds.create(0, 0, dim.Width, dim.Height);
			setPreferredSize(dim);
			revalidate();
		}

		//
		// CanvasPaneContents methods
		//
		public virtual CanvasPane CanvasPane
		{
			set
			{
				canvasPane = value;
				computeSize(true);
				popupManager = new LayoutPopupManager(value, this);
			}
		}

		public virtual void recomputeSize()
		{
			computeSize(true);
			repaint();
		}

		public virtual Size PreferredScrollableViewportSize
		{
			get
			{
				return getPreferredSize();
			}
		}

		public virtual int getScrollableBlockIncrement(Rectangle visibleRect, int orientation, int direction)
		{
			return canvasPane.supportScrollableBlockIncrement(visibleRect, orientation, direction);
		}

		public virtual bool ScrollableTracksViewportHeight
		{
			get
			{
				return false;
			}
		}

		public virtual bool ScrollableTracksViewportWidth
		{
			get
			{
				return false;
			}
		}

		public virtual int getScrollableUnitIncrement(Rectangle visibleRect, int orientation, int direction)
		{
			return canvasPane.supportScrollableUnitIncrement(visibleRect, orientation, direction);
		}

		internal static int getMaxIndex(CanvasModel model)
		{
			List<CanvasObject> objects = model.ObjectsFromBottom;
			for (int i = objects.Count - 1; i >= 0; i--)
			{
				if (!(objects[i] is AppearanceElement))
				{
					return i;
				}
			}
			return -1;
		}
	}

}
