/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.instance;

import java.awt.Font;
import java.awt.Graphics;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;

import logisim.circuit.CircuitState;
import logisim.comp.Component;
import logisim.comp.ComponentDrawContext;
import logisim.comp.ComponentEvent;
import logisim.comp.ComponentFactory;
import logisim.comp.ComponentListener;
import logisim.comp.ComponentUserEvent;
import logisim.comp.EndData;
import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.tools.TextEditable;
import logisim.tools.ToolTipMaker;
import logisim.util.EventSourceWeakSupport;
import logisim.util.StringGetter;
import logisim.util.UnmodifiableList;

class InstanceComponent implements Component, AttributeListener, ToolTipMaker {
	private EventSourceWeakSupport<ComponentListener> listeners;
	private InstanceFactory factory;
	private Instance instance;
	private Location loc;
	private Bounds bounds;
	private List<Port> portList;
	private EndData[] endArray;
	private List<EndData> endList;
	private boolean hasToolTips;
	private HashSet<Attribute<BitWidth>> widthAttrs;
	private AttributeSet attrs;
	private boolean attrListenRequested;
	private InstanceTextField textField;

	InstanceComponent(InstanceFactory factory, Location loc, AttributeSet attrs) {
		listeners = null;
		this.factory = factory;
		instance = new Instance(this);
		this.loc = loc;
		bounds = factory.getOffsetBounds(attrs).translate(loc.x(), loc.y());
		portList = factory.getPorts();
		endArray = null;
		hasToolTips = false;
		this.attrs = attrs;
		attrListenRequested = false;
		textField = null;

		computeEnds();
	}

	private void computeEnds() {
		List<Port> ports = portList;
		EndData[] esOld = endArray;
		int esOldLength = esOld == null ? 0 : esOld.length;
		EndData[] es = esOld;
		if (es == null || es.length != ports.size()) {
			es = new EndData[ports.size()];
			if (esOldLength > 0) {
				int toCopy = Math.min(esOldLength, es.length);
				System.arraycopy(esOld, 0, es, 0, toCopy);
			}
		}
		HashSet<Attribute<BitWidth>> wattrs = null;
		boolean toolTipFound = false;
		ArrayList<EndData> endsChangedOld = new ArrayList<>();
		ArrayList<EndData> endsChangedNew = new ArrayList<>();
		Iterator<Port> pit = ports.iterator();
		for (int i = 0; pit.hasNext() || i < esOldLength; i++) {
			Port p = pit.hasNext() ? pit.next() : null;
			@SuppressWarnings("null")
			EndData oldEnd = i < esOldLength ? esOld[i] : null;
			EndData newEnd = p == null ? null : p.toEnd(loc, attrs);
			if (oldEnd == null || !oldEnd.equals(newEnd)) {
				if (newEnd != null)
					es[i] = newEnd;
				endsChangedOld.add(oldEnd);
				endsChangedNew.add(newEnd);
			}

			if (p != null) {
				Attribute<BitWidth> attr = p.getWidthAttribute();
				if (attr != null) {
					if (wattrs == null) wattrs = new HashSet<>();
					wattrs.add(attr);
				}

				if (p.getToolTip() != null)
					toolTipFound = true;
			}
		}
		if (!attrListenRequested) {
			HashSet<Attribute<BitWidth>> oldWattrs = widthAttrs;
			if (wattrs == null && oldWattrs != null) getAttributeSet().removeAttributeListener(this);
			else if (wattrs != null && oldWattrs == null) getAttributeSet().addAttributeListener(this);
		}
		if (es != esOld) {
			endArray = es;
			endList = new UnmodifiableList<>(es);
		}
		widthAttrs = wattrs;
		hasToolTips = toolTipFound;
		if (!endsChangedOld.isEmpty()) fireEndsChanged(endsChangedOld, endsChangedNew);
	}

	//
	// listening methods
	//
	public void addComponentListener(ComponentListener l) {
		EventSourceWeakSupport<ComponentListener> ls = listeners;
		if (ls == null) {
			ls = new EventSourceWeakSupport<>();
			ls.add(l);
			listeners = ls;
		} else ls.add(l);
	}

	public void removeComponentListener(ComponentListener l) {
		if (listeners != null) {
			listeners.remove(l);
			if (listeners.isEmpty())
				listeners = null;
		}
	}

	private void fireEndsChanged(ArrayList<EndData> oldEnds, ArrayList<EndData> newEnds) {
		EventSourceWeakSupport<ComponentListener> ls = listeners;
		if (ls != null) {
			ComponentEvent e = null;
			for (ComponentListener l : ls) {
				if (e == null)
					e = new ComponentEvent(this, oldEnds, newEnds);
				l.endChanged(e);
			}
		}
	}

	void fireInvalidated() {
		EventSourceWeakSupport<ComponentListener> ls = listeners;
		if (ls != null) {
			ComponentEvent e = null;
			for (ComponentListener l : ls) {
				if (e == null)
					e = new ComponentEvent(this);
				l.componentInvalidated(e);
			}
		}
	}

	//
	// basic information methods
	//
	public ComponentFactory getFactory() {
		return factory;
	}

	public AttributeSet getAttributeSet() {
		return attrs;
	}

	public Object getFeature(Object key) {
		Object ret = factory.getInstanceFeature(instance, key);
		if (ret != null) return ret;
		else if (key == ToolTipMaker.class) {
			Object defaultTip = factory.getDefaultToolTip();
			if (hasToolTips || defaultTip != null)
				return this;
		} else if (key == TextEditable.class) return textField;
		return null;
	}

	//
	// location/extent methods
	//
	public Location getLocation() {
		return loc;
	}

	public Bounds getBounds() {
		return bounds;
	}

	public Bounds getBounds(Graphics g) {
		Bounds ret = bounds;
		InstanceTextField field = textField;
		return field != null ? ret.add(field.getBounds(g)) : ret;
	}

	public boolean contains(Location pt) {
		Location translated = pt.sub(loc);
		InstanceFactory factory = instance.getFactory();
		return factory.contains(translated, instance.getAttributeSet());
	}

	public boolean contains(Location pt, Graphics g) {
		InstanceTextField field = textField;
		return field != null && field.getBounds(g).contains(pt) || contains(pt);
	}

	//
	// propagation methods
	//
	public List<EndData> getEnds() {
		return endList;
	}

	public EndData getEnd(int index) {
		return endArray[index];
	}

	public boolean endsAt(Location pt) {
		EndData[] ends = endArray;
		for (EndData end : ends)
			if (end.getLocation().equals(pt))
				return true;
		return false;
	}

	public void propagate(CircuitState state) {
		factory.propagate(state.getInstanceState(this));
	}

	//
	// drawing methods
	//
	public void draw(ComponentDrawContext context) {
		InstancePainter painter = context.getInstancePainter();
		painter.setInstance(this);
		factory.paintInstance(painter);
	}

	public void expose(ComponentDrawContext context) {
		Bounds b = bounds;
		context.getDestination().repaint(b.getX(), b.getY(), b.getWidth(), b.getHeight());
	}

	public String getToolTip(ComponentUserEvent e) {
		int x = e.getX();
		int y = e.getY();
		int i = -1;
		for (EndData end : endArray) {
			i++;
			if (end.getLocation().manhattanDistanceTo(x, y) < 10) {
				Port p = portList.get(i);
				return p.getToolTip();
			}
		}
		StringGetter defaultTip = factory.getDefaultToolTip();
		return defaultTip == null ? null : defaultTip.get();
	}

	//
	// AttributeListener methods
	//
	public void attributeListChanged(AttributeEvent e) {
	}

	public void attributeValueChanged(AttributeEvent e) {
		Attribute<?> attr = e.getAttribute();
		if (widthAttrs != null && widthAttrs.contains(attr))
			computeEnds();
		if (attrListenRequested) factory.instanceAttributeChanged(instance, e.getAttribute());
	}

	//
	// methods for InstancePainter
	//
	void drawLabel(ComponentDrawContext context) {
		InstanceTextField field = textField;
		if (field != null)
			field.draw(context);
	}

	//
	// methods for Instance
	//
	Instance getInstance() {
		return instance;
	}

	List<Port> getPorts() {
		return portList;
	}

	void setPorts(Port[] ports) {
		Port[] portsCopy = ports.clone();
		portList = new UnmodifiableList<>(portsCopy);
		computeEnds();
	}

	void recomputeBounds() {
		Location p = loc;
		bounds = factory.getOffsetBounds(attrs).translate(p.x(), p.y());
	}

	void addAttributeListener() {
		if (!attrListenRequested) {
			attrListenRequested = true;
			if (widthAttrs == null)
				getAttributeSet().addAttributeListener(this);
		}
	}

	void setTextField(Attribute<String> labelAttr, Attribute<Font> fontAttr, int x, int y, int halign, int valign) {
		InstanceTextField field = textField;
		if (field == null) {
			field = new InstanceTextField(this);
			field.update(labelAttr, fontAttr, x, y, halign, valign);
			textField = field;
		} else field.update(labelAttr, fontAttr, x, y, halign, valign);
	}

}
