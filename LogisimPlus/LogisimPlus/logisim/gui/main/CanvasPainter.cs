// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using WidthIncompatibilityData = logisim.circuit.WidthIncompatibilityData;
	using WireSet = logisim.circuit.WireSet;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using GridPainter = logisim.gui.generic.GridPainter;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;
	using Tool = logisim.tools.Tool;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class CanvasPainter : PropertyChangeListener
	{
		private static readonly ISet<Component> NO_COMPONENTS = Collections.emptySet();

		private Canvas canvas;
		private GridPainter grid;
		private Component haloedComponent = null;
		private Circuit haloedCircuit = null;
		private WireSet highlightedWires = WireSet.EMPTY;

		internal CanvasPainter(Canvas canvas)
		{
			this.canvas = canvas;
			this.grid = new GridPainter(canvas);

			AppPreferences.PRINTER_VIEW.addPropertyChangeListener(this);
			AppPreferences.ATTRIBUTE_HALO.addPropertyChangeListener(this);
		}

		//
		// accessor methods
		//
		internal virtual GridPainter GridPainter
		{
			get
			{
				return grid;
			}
		}

		internal virtual Component HaloedComponent
		{
			get
			{
				return haloedComponent;
			}
		}

		//
		// mutator methods
		//
		internal virtual WireSet HighlightedWires
		{
			set
			{
				highlightedWires = value == null ? WireSet.EMPTY : value;
			}
		}

		internal virtual void setHaloedComponent(Circuit circ, Component comp)
		{
			if (comp == haloedComponent)
			{
				return;
			}
			Graphics g = canvas.getGraphics();
			exposeHaloedComponent(g);
			haloedCircuit = circ;
			haloedComponent = comp;
			exposeHaloedComponent(g);
		}

		private void exposeHaloedComponent(Graphics g)
		{
			Component c = haloedComponent;
			if (c == null)
			{
				return;
			}
			Bounds bds = c.getBounds(g).expand(7);
			int w = bds.Width;
			int h = bds.Height;
			double a = Canvas.SQRT_2 * w;
			double b = Canvas.SQRT_2 * h;
			canvas.repaint((int) (long)Math.Round(bds.X + w / 2.0 - a / 2.0, MidpointRounding.AwayFromZero), (int) (long)Math.Round(bds.Y + h / 2.0 - b / 2.0, MidpointRounding.AwayFromZero), (int) (long)Math.Round(a, MidpointRounding.AwayFromZero), (int) (long)Math.Round(b, MidpointRounding.AwayFromZero));
		}

		public virtual void propertyChange(PropertyChangeEvent @event)
		{
			if (AppPreferences.PRINTER_VIEW.isSource(@event) || AppPreferences.ATTRIBUTE_HALO.isSource(@event))
			{
				canvas.repaint();
			}
		}

		//
		// painting methods
		//
		internal virtual void paintContents(Graphics g, Project proj)
		{
			Rectangle clip = g.getClipBounds();
			Size size = canvas.getSize();
			double zoomFactor = canvas.ZoomFactor;
			if (canvas.ifPaintDirtyReset() || clip == null)
			{
				clip = new Rectangle(0, 0, size.width, size.height);
			}
			g.setColor(Color.white);
			g.fillRect(clip.x, clip.y, clip.width, clip.height);

			grid.paintGrid(g);
			g.setColor(Color.black);

			Graphics gScaled = g.create();
			if (zoomFactor != 1.0 && gScaled is Graphics2D)
			{
				((Graphics2D) gScaled).scale(zoomFactor, zoomFactor);
			}
			drawWithUserState(g, gScaled, proj);
			drawWidthIncompatibilityData(g, gScaled, proj);
			Circuit circ = proj.CurrentCircuit;

			CircuitState circState = proj.CircuitState;
			ComponentDrawContext ptContext = new ComponentDrawContext(canvas, circ, circState, g, gScaled);
			ptContext.HighlightedWires = highlightedWires;
			gScaled.setColor(Color.RED);
			circState.drawOscillatingPoints(ptContext);
			gScaled.setColor(Color.BLUE);
			proj.Simulator.drawStepPoints(ptContext);
			gScaled.dispose();
		}

		private void drawWithUserState(Graphics @base, Graphics g, Project proj)
		{
			Circuit circ = proj.CurrentCircuit;
			Selection sel = proj.Selection;
			ISet<Component> hidden;
			Tool dragTool = canvas.DragTool;
			if (dragTool == null)
			{
				hidden = NO_COMPONENTS;
			}
			else
			{
				hidden = dragTool.getHiddenComponents(canvas);
				if (hidden == null)
				{
					hidden = NO_COMPONENTS;
				}
			}

			// draw halo around component whose attributes we are viewing
			bool showHalo = AppPreferences.ATTRIBUTE_HALO.Boolean;
			if (showHalo && haloedComponent != null && haloedCircuit == circ && !hidden.Contains(haloedComponent))
			{
				GraphicsUtil.switchToWidth(g, 3);
				g.setColor(Canvas.HALO_COLOR);
				Bounds bds = haloedComponent.getBounds(g).expand(5);
				int w = bds.Width;
				int h = bds.Height;
				double a = Canvas.SQRT_2 * w;
				double b = Canvas.SQRT_2 * h;
				g.drawOval((int) (long)Math.Round(bds.X + w / 2.0 - a / 2.0, MidpointRounding.AwayFromZero), (int) (long)Math.Round(bds.Y + h / 2.0 - b / 2.0, MidpointRounding.AwayFromZero), (int) (long)Math.Round(a, MidpointRounding.AwayFromZero), (int) (long)Math.Round(b, MidpointRounding.AwayFromZero));
				GraphicsUtil.switchToWidth(g, 1);
				g.setColor(Color.BLACK);
			}

			// draw circuit and selection
			CircuitState circState = proj.CircuitState;
			bool printerView = AppPreferences.PRINTER_VIEW.Boolean;
			ComponentDrawContext context = new ComponentDrawContext(canvas, circ, circState, @base, g, printerView);
			context.HighlightedWires = highlightedWires;
			circ.draw(context, hidden);
			sel.draw(context, hidden);

			// draw tool
			Tool tool = dragTool != null ? dragTool : proj.Tool;
			if (tool != null && !canvas.PopupMenuUp)
			{
				Graphics gCopy = g.create();
				context.Graphics = gCopy;
				tool.draw(canvas, context);
				gCopy.dispose();
			}
		}

		private void drawWidthIncompatibilityData(Graphics @base, Graphics g, Project proj)
		{
			ISet<WidthIncompatibilityData> exceptions;
			exceptions = proj.CurrentCircuit.WidthIncompatibilityData;
			if (exceptions == null || exceptions.Count == 0)
			{
				return;
			}

			g.setColor(Value.WIDTH_ERROR_COLOR);
			GraphicsUtil.switchToWidth(g, 2);
			FontMetrics fm = @base.getFontMetrics(g.getFont());
			foreach (WidthIncompatibilityData ex in exceptions)
			{
				for (int i = 0; i < ex.size(); i++)
				{
					Location p = ex.getPoint(i);
					BitWidth w = ex.getBitWidth(i);

					// ensure it hasn't already been drawn
					bool drawn = false;
					for (int j = 0; j < i; j++)
					{
						if (ex.getPoint(j).Equals(p))
						{
							drawn = true;
							break;
						}
					}
					if (drawn)
					{
						continue;
					}

					// compute the caption combining all similar points
					string caption = "" + w.Width;
					for (int j = i + 1; j < ex.size(); j++)
					{
						if (ex.getPoint(j).Equals(p))
						{
							caption += "/" + ex.getBitWidth(j);
							break;
						}
					}
					g.drawOval(p.X - 4, p.Y - 4, 8, 8);
					g.drawString(caption, p.X + 5, p.Y + 2 + fm.getAscent());
				}
			}
			g.setColor(Color.BLACK);
			GraphicsUtil.switchToWidth(g, 1);
		}
	}

}
