// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using WireSet = logisim.circuit.WireSet;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using InstancePainter = logisim.instance.InstancePainter;
	using AppPreferences = logisim.prefs.AppPreferences;
	using GraphicsUtil = logisim.util.GraphicsUtil;
    using LogisimPlus.Java;

    public class ComponentDrawContext
	{
		private const int PIN_OFFS = 2;
		private const int PIN_RAD = 4;

		private java.awt.Component dest;
		private Circuit circuit;
		private CircuitState circuitState;
		private JGraphics @base;
		private JGraphics g;
		private bool showState;
		private bool showColor;
		private bool printView;
		private WireSet highlightedWires;
		private InstancePainter instancePainter;

		public ComponentDrawContext(java.awt.Component dest, Circuit circuit, CircuitState circuitState, JGraphics @base, JGraphics g, bool printView)
		{
			this.dest = dest;
			this.circuit = circuit;
			this.circuitState = circuitState;
			this.@base = @base;
			this.g = g;
			this.showState = true;
			this.showColor = true;
			this.printView = printView;
			this.highlightedWires = WireSet.EMPTY;
			this.instancePainter = new InstancePainter(this, null);
		}

		public ComponentDrawContext(java.awt.Component dest, Circuit circuit, CircuitState circuitState, JGraphics @base, JGraphics g) : this(dest, circuit, circuitState, @base, g, false)
		{
		}

		public virtual bool ShowState
		{
			set
			{
				showState = value;
			}
			get
			{
				return !printView && showState;
			}
		}

		public virtual bool ShowColor
		{
			set
			{
				showColor = value;
			}
		}

		public virtual InstancePainter InstancePainter
		{
			get
			{
				return instancePainter;
			}
		}

		public virtual WireSet HighlightedWires
		{
			set
			{
				this.highlightedWires = value == null ? WireSet.EMPTY : value;
			}
			get
			{
				return highlightedWires;
			}
		}



		public virtual bool PrintView
		{
			get
			{
				return printView;
			}
		}

		public virtual bool shouldDrawColor()
		{
			return !printView && showColor;
		}

		public virtual java.awt.Component Destination
		{
			get
			{
				return dest;
			}
		}

		public virtual JGraphics Graphics
		{
			get
			{
				return g;
			}
			set
			{
				this.g = value;
			}
		}

		public virtual Circuit Circuit
		{
			get
			{
				return circuit;
			}
		}

		public virtual CircuitState CircuitState
		{
			get
			{
				return circuitState;
			}
		}


		public virtual object GateShape
		{
			get
			{
				return AppPreferences.GATE_SHAPE.get();
			}
		}

		//
		// helper methods
		//
		public virtual void drawBounds(Component comp)
		{
			GraphicsUtil.switchToWidth(g, 2);
			g.setColor(Color.Black);
			Bounds bds = comp.Bounds;
			g.drawRect(bds.X, bds.Y, bds.Width, bds.Height);
			GraphicsUtil.switchToWidth(g, 1);
		}

		public virtual void drawRectangle(Component comp)
		{
			drawRectangle(comp, "");
		}

		public virtual void drawRectangle(Component comp, string label)
		{
			Bounds bds = comp.getBounds(g);
			drawRectangle(bds.X, bds.Y, bds.Width, bds.Height, label);
		}

		public virtual void drawRectangle(int x, int y, int width, int height, string label)
		{
			GraphicsUtil.switchToWidth(g, 2);
			g.drawRect(x, y, width, height);
			if (!string.ReferenceEquals(label, null) && !label.Equals(""))
			{
				SizeF labelSize= g.measureString(label);
				int lwid = (int)labelSize.Width;
				if (height > 20)
				{ // centered at top edge
					g.drawString(label, x + (width - lwid) / 2, y + 2 + (int)labelSize.Height);
				}
				else
				{ // centered overall
					g.drawString(label, x + (width - lwid) / 2, y + (height + (int)labelSize.Height) / 2 - 1);
				}
			}
		}

		public virtual void drawRectangle(ComponentFactory source, int x, int y, AttributeSet attrs, string label)
		{
			Bounds bds = source.getOffsetBounds(attrs);
			drawRectangle(source, x + bds.X, y + bds.Y, bds.Width, bds.Height, label);
		}

		public virtual void drawRectangle(ComponentFactory source, int x, int y, int width, int height, string label)
		{
			GraphicsUtil.switchToWidth(g, 2);
			g.drawRect(x + 1, y + 1, width - 1, height - 1);
			if (!string.ReferenceEquals(label, null) && !label.Equals(""))
			{
                SizeF labelSize = g.measureString(label);
                int lwid = (int)labelSize.Width;
                if (height > 20)
				{ // centered at top edge
					g.drawString(label, x + (width - lwid) / 2, y + 2 + (int)labelSize.Height);
				}
				else
				{ // centered overall
					g.drawString(label, x + (width - lwid) / 2, y + (height + (int)labelSize.Height) / 2 - 1);
				}
			}
		}

		public virtual void drawDongle(int x, int y)
		{
			GraphicsUtil.switchToWidth(g, 2);
			g.drawOval(x - 4, y - 4, 9, 9);
		}

		public virtual void drawPin(Component comp, int i, string label, Direction dir)
		{
			Color curColor = g.getColor();
			if (i < 0 || i >= comp.Ends.Count)
			{
				return;
			}
			EndData e = comp.getEnd(i);
			Location pt = e.Location;
			int x = pt.X;
			int y = pt.Y;
			if (ShowState)
			{
				CircuitState state = CircuitState;
				g.setColor(state.getValue(pt).Color);
			}
			else
			{
				g.setColor(Color.Black);
			}
			g.fillOval(x - PIN_OFFS, y - PIN_OFFS, PIN_RAD, PIN_RAD);
			g.setColor(curColor);
			if (dir == Direction.East)
			{
				GraphicsUtil.drawText(g, label, x + 3, y, GraphicsUtil.H_LEFT, GraphicsUtil.V_CENTER);
			}
			else if (dir == Direction.West)
			{
				GraphicsUtil.drawText(g, label, x - 3, y, GraphicsUtil.H_RIGHT, GraphicsUtil.V_CENTER);
			}
			else if (dir == Direction.South)
			{
				GraphicsUtil.drawText(g, label, x, y - 3, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
			}
			else if (dir == Direction.North)
			{
				GraphicsUtil.drawText(g, label, x, y + 3, GraphicsUtil.H_CENTER, GraphicsUtil.V_TOP);
			}
		}

		public virtual void drawPin(Component comp, int i)
		{
			EndData e = comp.getEnd(i);
			Location pt = e.Location;
			Color curColor = g.getColor();
			if (ShowState)
			{
				CircuitState state = CircuitState;
				g.setColor(state.getValue(pt).Color);
			}
			else
			{
				g.setColor(Color.Black);
			}
			g.fillOval(pt.X - PIN_OFFS, pt.Y - PIN_OFFS, PIN_RAD, PIN_RAD);
			g.setColor(curColor);
		}

		public virtual void drawPins(Component comp)
		{
			Color curColor = g.getColor();
			foreach (EndData e in comp.Ends)
			{
				Location pt = e.Location;
				if (ShowState)
				{
					CircuitState state = CircuitState;
					g.setColor(state.getValue(pt).Color);
				}
				else
				{
					g.setColor(Color.Black);
				}
				g.fillOval(pt.X - PIN_OFFS, pt.Y - PIN_OFFS, PIN_RAD, PIN_RAD);
			}
			g.setColor(curColor);
		}

		public virtual void drawClock(Component comp, int i, Direction dir)
		{
			Color curColor = g.getColor();
			g.setColor(Color.Black);
			GraphicsUtil.switchToWidth(g, 2);

			EndData e = comp.getEnd(i);
			Location pt = e.Location;
			int x = pt.X;
			int y = pt.Y;
			const int CLK_SZ = 4;
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final int CLK_SZD = CLK_SZ - 1;
			int CLK_SZD = CLK_SZ - 1;
			if (dir == Direction.North)
			{
				g.drawLine(x - CLK_SZD, y - 1, x, y - CLK_SZ);
				g.drawLine(x + CLK_SZD, y - 1, x, y - CLK_SZ);
			}
			else if (dir == Direction.South)
			{
				g.drawLine(x - CLK_SZD, y + 1, x, y + CLK_SZ);
				g.drawLine(x + CLK_SZD, y + 1, x, y + CLK_SZ);
			}
			else if (dir == Direction.East)
			{
				g.drawLine(x + 1, y - CLK_SZD, x + CLK_SZ, y);
				g.drawLine(x + 1, y + CLK_SZD, x + CLK_SZ, y);
			}
			else if (dir == Direction.West)
			{
				g.drawLine(x - 1, y - CLK_SZD, x - CLK_SZ, y);
				g.drawLine(x - 1, y + CLK_SZD, x - CLK_SZ, y);
			}

			g.setColor(curColor);
			GraphicsUtil.switchToWidth(g, 1);
		}

		public virtual void drawHandles(Component comp)
		{
			Bounds b = comp.getBounds(g);
			int left = b.X;
			int right = left + b.Width;
			int top = b.Y;
			int bot = top + b.Height;
			drawHandle(right, top);
			drawHandle(left, bot);
			drawHandle(right, bot);
			drawHandle(left, top);
		}

		public virtual void drawHandle(Location loc)
		{
			drawHandle(loc.X, loc.Y);
		}

		public virtual void drawHandle(int x, int y)
		{
			g.setColor(Color.White);
			g.fillRect(x - 3, y - 3, 7, 7);
			g.setColor(Color.Black);
			g.drawRect(x - 3, y - 3, 7, 7);
		}

	}

}
