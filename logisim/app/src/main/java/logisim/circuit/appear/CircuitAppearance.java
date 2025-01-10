/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit.appear;

import java.awt.Graphics;
import java.awt.Graphics2D;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Map;
import java.util.SortedMap;
import java.util.TreeMap;

import draw.model.CanvasModelEvent;
import draw.model.CanvasModelListener;
import draw.model.CanvasObject;
import draw.model.Drawing;
import logisim.circuit.Circuit;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.instance.Instance;
import logisim.util.EventSourceWeakSupport;

public class CircuitAppearance extends Drawing {
	private class MyListener implements CanvasModelListener {
		public void modelChanged(CanvasModelEvent event) {
			if (!suppressRecompute) {
				setDefaultAppearance(false);
				fireCircuitAppearanceChanged(CircuitAppearanceEvent.ALL_TYPES);
			}
		}
	}

	private Circuit circuit;
	private EventSourceWeakSupport<CircuitAppearanceListener> listeners;
	private CircuitPins circuitPins;
	private boolean isDefault;
	private boolean suppressRecompute;

	public CircuitAppearance(Circuit circuit) {
		this.circuit = circuit;
		listeners = new EventSourceWeakSupport<>();
		PortManager portManager = new PortManager(this);
		circuitPins = new CircuitPins(portManager);
		MyListener myListener = new MyListener();
		suppressRecompute = false;
		addCanvasModelListener(myListener);
		setDefaultAppearance(true);
	}

	public CircuitPins getCircuitPins() {
		return circuitPins;
	}

	public void addCircuitAppearanceListener(CircuitAppearanceListener l) {
		listeners.add(l);
	}

	public void removeCircuitAppearanceListener(CircuitAppearanceListener l) {
		listeners.remove(l);
	}

	void fireCircuitAppearanceChanged(int affected) {
		CircuitAppearanceEvent event = new CircuitAppearanceEvent(circuit, affected);
		listeners.forEach(l -> l.circuitAppearanceChanged(event));
	}

	void replaceAutomatically(List<AppearancePort> removes, List<AppearancePort> adds) {
		// this should be called only when substituting ports via PortManager
		boolean oldSuppress = suppressRecompute;
		try {
			suppressRecompute = true;
			removeObjects(removes);
			addObjects(getObjectsFromBottom().size() - 1, adds);
			recomputeDefaultAppearance();
		} finally {
			suppressRecompute = oldSuppress;
		}
		fireCircuitAppearanceChanged(CircuitAppearanceEvent.ALL_TYPES);
	}

	public boolean isDefaultAppearance() {
		return isDefault;
	}

	public void setDefaultAppearance(boolean value) {
		if (isDefault != value) {
			isDefault = value;
			if (value)
				recomputeDefaultAppearance();
		}
	}

	void recomputePorts() {
		if (isDefault)
			recomputeDefaultAppearance();
		else
			fireCircuitAppearanceChanged(CircuitAppearanceEvent.ALL_TYPES);
	}

	private void recomputeDefaultAppearance() {
		if (isDefault) {
			List<CanvasObject> shapes = DefaultAppearance.build(circuitPins.getPins());
			setObjectsForce(shapes);
		}
	}

	public Direction getFacing() {
		AppearanceAnchor anchor = findAnchor();
		return anchor == null ? Direction.East : anchor.getFacing();
	}

	public void setObjectsForce(List<? extends CanvasObject> shapesBase) {
		// This shouldn't ever be an issue, but just to make doubly sure, we'll
		// check that the anchor and all ports are in their proper places.
		List<CanvasObject> shapes = new ArrayList<>(shapesBase);
		int n = shapes.size();
		int ports = 0;
		for (int i = n - 1; i >= 0; i--) { // count ports, move anchor to end
			CanvasObject o = shapes.get(i);
			if (o instanceof AppearanceAnchor) {
				if (i != n - 1) {
					shapes.remove(i);
					shapes.add(o);
				}
			} else if (o instanceof AppearancePort)
				ports++;
		}
		for (int i = (n - ports - 1) - 1; i >= 0; i--) { // move ports to top
			CanvasObject o = shapes.get(i);
			if (o instanceof AppearancePort) {
				shapes.remove(i);
				shapes.add(n - ports - 1, o);
				i--;
			}
		}

		try {
			suppressRecompute = true;
			super.removeObjects(new ArrayList<>(getObjectsFromBottom()));
			super.addObjects(0, shapes);
		} finally {
			suppressRecompute = false;
		}
		fireCircuitAppearanceChanged(CircuitAppearanceEvent.ALL_TYPES);
	}

	public void paintSubcircuit(Graphics g, Direction facing) {
		Direction defaultFacing = getFacing();
		double rotate = 0.0;
		if (facing != defaultFacing && g instanceof Graphics2D) {
			rotate = defaultFacing.toRadians() - facing.toRadians();
			((Graphics2D) g).rotate(rotate);
		}
		Location offset = findAnchorLocation();
		g.translate(-offset.x(), -offset.y());
		for (CanvasObject shape : getObjectsFromBottom())
			if (!(shape instanceof AppearanceElement)) {
				Graphics dup = g.create();
				shape.paint(dup, null);
				dup.dispose();
			}
		g.translate(offset.x(), offset.y());
		if (rotate != 0.0)
			((Graphics2D) g).rotate(-rotate);
	}

	private Location findAnchorLocation() {
		AppearanceAnchor anchor = findAnchor();
		return anchor == null ? new Location(100, 100) : anchor.getLocation();
	}

	private AppearanceAnchor findAnchor() {
		return (AppearanceAnchor) getObjectsFromBottom().stream()
				.filter(shape -> shape instanceof AppearanceAnchor)
				.findFirst()
				.orElse(null);
	}

	public Bounds getOffsetBounds() {
		return getBounds(true);
	}

	public Bounds getAbsoluteBounds() {
		return getBounds(false);
	}

	private Bounds getBounds(boolean relativeToAnchor) {
		Bounds ret = null;
		Location offset = null;
		for (CanvasObject o : getObjectsFromBottom())
			if (o instanceof AppearanceElement) {
				Location loc = ((AppearanceElement) o).getLocation();
				if (o instanceof AppearanceAnchor)
					offset = loc;
				ret = ret == null ? Bounds.create(loc) : ret.add(loc);
			}
			else if (ret == null)
				ret = o.getBounds();
			else
				ret = ret.add(o.getBounds());
		if (ret == null)
			return Bounds.EMPTY_BOUNDS;
		else if (relativeToAnchor && offset != null)
			return ret.translate(-offset.x(), -offset.y());
		else
			return ret;
	}

	public SortedMap<Location, Instance> getPortOffsets(Direction facing) {
		Location anchor = null;
		Direction defaultFacing = Direction.East;
		List<AppearancePort> ports = new ArrayList<>();
		for (CanvasObject shape : getObjectsFromBottom())
			if (shape instanceof AppearancePort)
				ports.add((AppearancePort) shape);
			else if (shape instanceof AppearanceAnchor o) {
				anchor = o.getLocation();
				defaultFacing = o.getFacing();
			}

		SortedMap<Location, Instance> ret = new TreeMap<>();
		for (AppearancePort port : ports) {
			Location loc = port.getLocation();
			if (anchor != null)
				loc = loc.sub(anchor);
			if (facing != defaultFacing)
				loc = loc.rotate(defaultFacing, facing, 0, 0);
			ret.put(loc, port.getPin());
		}
		return ret;
	}

	@Override
	public <V extends CanvasObject> void addObjects(int index, Collection<V> shapes) {
		super.addObjects(index, shapes);
		checkToFirePortsChanged(shapes);
	}

	@Override
	public <V extends CanvasObject> void addObjects(Map<V, Integer> shapes) {
		super.addObjects(shapes);
		checkToFirePortsChanged(shapes.keySet());
	}

	@Override
	public <V extends CanvasObject> void removeObjects(Collection<V> shapes) {
		super.removeObjects(shapes);
		checkToFirePortsChanged(shapes);
	}

	@Override
	public <V extends CanvasObject> void translateObjects(Collection<V> shapes, int dx, int dy) {
		super.translateObjects(shapes, dx, dy);
		checkToFirePortsChanged(shapes);
	}

	private <V extends CanvasObject> void checkToFirePortsChanged(Collection<V> shapes) {
		if (affectsPorts(shapes)) recomputePorts();
	}

	private <V extends CanvasObject> boolean affectsPorts(Collection<V> shapes) {
		return shapes.stream().anyMatch(o -> o instanceof AppearanceElement);
	}
}
