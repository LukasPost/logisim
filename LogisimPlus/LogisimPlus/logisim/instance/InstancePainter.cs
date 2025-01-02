// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using WireSet = logisim.circuit.WireSet;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Project = logisim.proj.Project;

	public class InstancePainter : InstanceState
	{
		private ComponentDrawContext context;
		private InstanceComponent comp;
		private InstanceFactory factory;
		private AttributeSet attrs;

		public InstancePainter(ComponentDrawContext context, InstanceComponent instance)
		{
			this.context = context;
			this.comp = instance;
		}

		internal virtual InstanceComponent Instance
		{
			set
			{
				this.comp = value;
			}
			get
			{
				InstanceComponent c = comp;
				return c == null ? null : c.Instance;
			}
		}

		internal virtual void setFactory(InstanceFactory factory, AttributeSet attrs)
		{
			this.comp = null;
			this.factory = factory;
			this.attrs = attrs;
		}

		public virtual InstanceFactory Factory
		{
			get
			{
				return comp == null ? factory : (InstanceFactory) comp.Factory;
			}
		}

		//
		// methods related to the context of the canvas
		//
		public virtual WireSet HighlightedWires
		{
			get
			{
				return context.HighlightedWires;
			}
		}

		public virtual bool ShowState
		{
			get
			{
				return context.ShowState;
			}
		}

		public virtual bool PrintView
		{
			get
			{
				return context.PrintView;
			}
		}

		public virtual bool shouldDrawColor()
		{
			return context.shouldDrawColor();
		}

		public virtual java.awt.Component Destination
		{
			get
			{
				return context.Destination;
			}
		}

		public virtual Graphics Graphics
		{
			get
			{
				return context.Graphics;
			}
		}

		public virtual Circuit Circuit
		{
			get
			{
				return context.Circuit;
			}
		}

		public virtual object GateShape
		{
			get
			{
				return context.GateShape;
			}
		}

		public virtual bool CircuitRoot
		{
			get
			{
				return !context.CircuitState.Substate;
			}
		}

		public virtual long TickCount
		{
			get
			{
				return context.CircuitState.Propagator.TickCount;
			}
		}

		//
		// methods related to the circuit state
		//
		public virtual Project Project
		{
			get
			{
				return context.CircuitState.Project;
			}
		}

		public virtual Value getPort(int portIndex)
		{
			InstanceComponent c = comp;
			CircuitState s = context.CircuitState;
			if (c != null && s != null)
			{
				return s.getValue(c.getEnd(portIndex).Location);
			}
			else
			{
				return Value.UNKNOWN;
			}
		}

		public virtual void setPort(int portIndex, Value value, int delay)
		{
			throw new System.NotSupportedException("setValue on InstancePainter");
		}

		public virtual InstanceData Data
		{
			get
			{
				CircuitState circState = context.CircuitState;
				if (circState == null || comp == null)
				{
					throw new System.NotSupportedException("setData on InstancePainter");
				}
				else
				{
					return (InstanceData) circState.getData(comp);
				}
			}
			set
			{
				CircuitState circState = context.CircuitState;
				if (circState == null || comp == null)
				{
					throw new System.NotSupportedException("setData on InstancePainter");
				}
				else
				{
					circState.setData(comp, value);
				}
			}
		}



		public virtual Location Location
		{
			get
			{
				InstanceComponent c = comp;
				return c == null ? new Location(0, 0) : c.Location;
			}
		}

		public virtual bool isPortConnected(int index)
		{
			Circuit circ = context.Circuit;
			Location loc = comp.getEnd(index).Location;
			return circ.isConnected(loc, comp);
		}

		public virtual Bounds OffsetBounds
		{
			get
			{
				InstanceComponent c = comp;
				if (c == null)
				{
					return factory.getOffsetBounds(attrs);
				}
				else
				{
					Location loc = c.Location;
					return c.Bounds.translate(-loc.X, -loc.Y);
				}
			}
		}

		public virtual Bounds Bounds
		{
			get
			{
				InstanceComponent c = comp;
				return c == null ? factory.getOffsetBounds(attrs) : c.Bounds;
			}
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				InstanceComponent c = comp;
				return c == null ? attrs : c.AttributeSet;
			}
		}

		public virtual E getAttributeValue<E>(Attribute<E> attr)
		{
			InstanceComponent c = comp;
			AttributeSet @as = c == null ? attrs : c.AttributeSet;
			return @as.getValue(attr);
		}

		public virtual void fireInvalidated()
		{
			comp.fireInvalidated();
		}

		//
		// helper methods for drawing common elements in components
		//
		public virtual void drawBounds()
		{
			context.drawBounds(comp);
		}

		public virtual void drawRectangle(Bounds bds, string label)
		{
			context.drawRectangle(bds.X, bds.Y, bds.Width, bds.Height, label);
		}

		public virtual void drawRectangle(int x, int y, int width, int height, string label)
		{
			context.drawRectangle(x, y, width, height, label);
		}

		public virtual void drawDongle(int x, int y)
		{
			context.drawDongle(x, y);
		}

		public virtual void drawPort(int i)
		{
			context.drawPin(comp, i);
		}

		public virtual void drawPort(int i, string label, Direction dir)
		{
			context.drawPin(comp, i, label, dir);
		}

		public virtual void drawPorts()
		{
			context.drawPins(comp);
		}

		public virtual void drawClock(int i, Direction dir)
		{
			context.drawClock(comp, i, dir);
		}

		public virtual void drawHandles()
		{
			context.drawHandles(comp);
		}

		public virtual void drawHandle(Location loc)
		{
			context.drawHandle(loc);
		}

		public virtual void drawHandle(int x, int y)
		{
			context.drawHandle(x, y);
		}

		public virtual void drawLabel()
		{
			if (comp != null)
			{
				comp.drawLabel(context);
			}
		}
	}

}
