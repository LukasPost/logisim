// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit.appear
{

	using CanvasModelEvent = draw.model.CanvasModelEvent;
	using CanvasModelListener = draw.model.CanvasModelListener;
	using CanvasObject = draw.model.CanvasObject;
	using Drawing = draw.model.Drawing;
	using Circuit = logisim.circuit.Circuit;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Instance = logisim.instance.Instance;
	using logisim.util;

	public class CircuitAppearance : Drawing
	{
		private class MyListener : CanvasModelListener
		{
			private readonly CircuitAppearance outerInstance;

			public MyListener(CircuitAppearance outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void modelChanged(CanvasModelEvent @event)
			{
				if (!outerInstance.suppressRecompute)
				{
					outerInstance.DefaultAppearance = false;
					outerInstance.fireCircuitAppearanceChanged(CircuitAppearanceEvent.ALL_TYPES);
				}
			}
		}

		private Circuit circuit;
		private EventSourceWeakSupport<CircuitAppearanceListener> listeners;
		private PortManager portManager;
		private CircuitPins circuitPins;
		private MyListener myListener;
		private bool isDefault;
		private bool suppressRecompute;

		public CircuitAppearance(Circuit circuit)
		{
			this.circuit = circuit;
			listeners = new EventSourceWeakSupport<CircuitAppearanceListener>();
			portManager = new PortManager(this);
			circuitPins = new CircuitPins(portManager);
			myListener = new MyListener(this);
			suppressRecompute = false;
			addCanvasModelListener(myListener);
			DefaultAppearance = true;
		}

		public virtual CircuitPins CircuitPins
		{
			get
			{
				return circuitPins;
			}
		}

		public virtual void addCircuitAppearanceListener(CircuitAppearanceListener l)
		{
			listeners.add(l);
		}

		public virtual void removeCircuitAppearanceListener(CircuitAppearanceListener l)
		{
			listeners.remove(l);
		}

		internal virtual void fireCircuitAppearanceChanged(int affected)
		{
			CircuitAppearanceEvent @event;
			@event = new CircuitAppearanceEvent(circuit, affected);
			foreach (CircuitAppearanceListener listener in listeners)
			{
				listener.circuitAppearanceChanged(@event);
			}
		}

		internal virtual void replaceAutomatically(IList<AppearancePort> removes, IList<AppearancePort> adds)
		{
			// this should be called only when substituting ports via PortManager
			bool oldSuppress = suppressRecompute;
			try
			{
				suppressRecompute = true;
				removeObjects(removes);
				addObjects(ObjectsFromBottom.Count - 1, adds);
				recomputeDefaultAppearance();
			}
			finally
			{
				suppressRecompute = oldSuppress;
			}
			fireCircuitAppearanceChanged(CircuitAppearanceEvent.ALL_TYPES);
		}

		public virtual bool DefaultAppearance
		{
			get
			{
				return isDefault;
			}
			set
			{
				if (isDefault != value)
				{
					isDefault = value;
					if (value)
					{
						recomputeDefaultAppearance();
					}
				}
			}
		}


		internal virtual void recomputePorts()
		{
			if (isDefault)
			{
				recomputeDefaultAppearance();
			}
			else
			{
				fireCircuitAppearanceChanged(CircuitAppearanceEvent.ALL_TYPES);
			}
		}

		private void recomputeDefaultAppearance()
		{
			if (isDefault)
			{
				IList<CanvasObject> shapes;
				shapes = logisim.circuit.appear.DefaultAppearance.build(circuitPins.Pins);
				ObjectsForce = shapes;
			}
		}

		public virtual Direction Facing
		{
			get
			{
				AppearanceAnchor anchor = findAnchor();
				if (anchor == null)
				{
					return Direction.East;
				}
				else
				{
					return anchor.Facing;
				}
			}
		}

		public virtual void setObjectsForce<T1>(IList<T1> value) where T1 : draw.model.CanvasObject
		{
			// This shouldn't ever be an issue, but just to make doubly sure, we'll
			// check that the anchor and all ports are in their proper places.
			IList<CanvasObject> shapes = new List<CanvasObject>(value);
			int n = shapes.Count;
			int ports = 0;
			for (int i = n - 1; i >= 0; i--)
			{ // count ports, move anchor to end
				CanvasObject o = shapes[i];
				if (o is AppearanceAnchor)
				{
					if (i != n - 1)
					{
						shapes.RemoveAt(i);
						shapes.Add(o);
					}
				}
				else if (o is AppearancePort)
				{
					ports++;
				}
			}
			for (int i = (n - ports - 1) - 1; i >= 0; i--)
			{ // move ports to top
				CanvasObject o = shapes[i];
				if (o is AppearancePort)
				{
					shapes.RemoveAt(i);
					shapes.Insert(n - ports - 1, o);
					i--;
				}
			}

			try
			{
				suppressRecompute = true;
				base.removeObjects(new List<CanvasObject>(ObjectsFromBottom));
				base.addObjects(0, shapes);
			}
			finally
			{
				suppressRecompute = false;
			}
			fireCircuitAppearanceChanged(CircuitAppearanceEvent.ALL_TYPES);
		}

		public virtual void paintSubcircuit(Graphics g, Direction facing)
		{
			Direction defaultFacing = Facing;
			double rotate = 0.0;
			if (facing != defaultFacing)
			{
				rotate = defaultFacing.toRadians() - facing.toRadians();
				((Graphics2D) g).rotate(rotate);
			}
			Location offset = findAnchorLocation();
			g.translate(-offset.X, -offset.Y);
			foreach (CanvasObject shape in ObjectsFromBottom)
			{
				if (!(shape is AppearanceElement))
				{
					Graphics dup = g.create();
					shape.paint(dup, null);
					dup.dispose();
				}
			}
			g.translate(offset.X, offset.Y);
			if (rotate != 0.0)
			{
				((Graphics2D) g).rotate(-rotate);
			}
		}

		private Location findAnchorLocation()
		{
			AppearanceAnchor anchor = findAnchor();
			if (anchor == null)
			{
				return new Location(100, 100);
			}
			else
			{
				return anchor.Location;
			}
		}

		private AppearanceAnchor findAnchor()
		{
			foreach (CanvasObject shape in ObjectsFromBottom)
			{
				if (shape is AppearanceAnchor)
				{
					return (AppearanceAnchor) shape;
				}
			}
			return null;
		}

		public virtual Bounds OffsetBounds
		{
			get
			{
				return getBounds(true);
			}
		}

		public virtual Bounds AbsoluteBounds
		{
			get
			{
				return getBounds(false);
			}
		}

		private Bounds getBounds(bool relativeToAnchor)
		{
			Bounds ret = null;
			Location offset = null;
			foreach (CanvasObject o in ObjectsFromBottom)
			{
				if (o is AppearanceElement)
				{
					Location loc = ((AppearanceElement) o).Location;
					if (o is AppearanceAnchor)
					{
						offset = loc;
					}
					if (ret == null)
					{
						ret = Bounds.create(loc);
					}
					else
					{
						ret = ret.add(loc);
					}
				}
				else
				{
					if (ret == null)
					{
						ret = o.Bounds;
					}
					else
					{
						ret = ret.add(o.Bounds);
					}
				}
			}
			if (ret == null)
			{
				return Bounds.EMPTY_BOUNDS;
			}
			else if (relativeToAnchor && offset != null)
			{
				return ret.translate(-offset.X, -offset.Y);
			}
			else
			{
				return ret;
			}
		}

		public virtual SortedDictionary<Location, Instance> getPortOffsets(Direction facing)
		{
			Location anchor = null;
			Direction defaultFacing = Direction.East;
			IList<AppearancePort> ports = new List<AppearancePort>();
			foreach (CanvasObject shape in ObjectsFromBottom)
			{
				if (shape is AppearancePort)
				{
					ports.Add((AppearancePort) shape);
				}
				else if (shape is AppearanceAnchor)
				{
					AppearanceAnchor o = (AppearanceAnchor) shape;
					anchor = o.Location;
					defaultFacing = o.Facing;
				}
			}

			SortedDictionary<Location, Instance> ret = new SortedDictionary<Location, Instance>();
			foreach (AppearancePort port in ports)
			{
				Location loc = port.Location;
				if (anchor != null)
				{
					loc = loc.translate(-anchor.X, -anchor.Y);
				}
				if (facing != defaultFacing)
				{
					loc = loc.rotate(defaultFacing, facing, 0, 0);
				}
				ret[loc] = port.Pin;
			}
			return ret;
		}

		public override void addObjects<T1>(int index, ICollection<T1> shapes) where T1 : draw.model.CanvasObject
		{
			base.addObjects(index, shapes);
			checkToFirePortsChanged(shapes);
		}

		public override void addObjects<T1>(IDictionary<T1> shapes) where T1 : draw.model.CanvasObject
		{
			base.addObjects(shapes);
			checkToFirePortsChanged(shapes.Keys);
		}

		public override void removeObjects<T1>(ICollection<T1> shapes) where T1 : draw.model.CanvasObject
		{
			base.removeObjects(shapes);
			checkToFirePortsChanged(shapes);
		}

		public override void translateObjects<T1>(ICollection<T1> shapes, int dx, int dy) where T1 : draw.model.CanvasObject
		{
			base.translateObjects(shapes, dx, dy);
			checkToFirePortsChanged(shapes);
		}

		private void checkToFirePortsChanged<T1>(ICollection<T1> shapes) where T1 : draw.model.CanvasObject
		{
			if (affectsPorts(shapes))
			{
				recomputePorts();
			}
		}

		private bool affectsPorts<T1>(ICollection<T1> shapes) where T1 : draw.model.CanvasObject
		{
			foreach (CanvasObject o in shapes)
			{
				if (o is AppearanceElement)
				{
					return true;
				}
			}
			return false;
		}
	}

}
