// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{

	using CircuitState = logisim.circuit.CircuitState;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentEvent = logisim.comp.ComponentEvent;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using ComponentListener = logisim.comp.ComponentListener;
	using ComponentUserEvent = logisim.comp.ComponentUserEvent;
	using EndData = logisim.comp.EndData;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using TextEditable = logisim.tools.TextEditable;
	using ToolTipMaker = logisim.tools.ToolTipMaker;
	using logisim.util;
	using StringGetter = logisim.util.StringGetter;
	using logisim.util;

	internal class InstanceComponent : Component, AttributeListener, ToolTipMaker
	{
		private EventSourceWeakSupport<ComponentListener> listeners;
		private InstanceFactory factory;
		private Instance instance;
		private Location loc;
		private Bounds bounds;
		private List<Port> portList;
		private EndData[] endArray;
		private List<EndData> endList;
		private bool hasToolTips;
		private HashSet<Attribute> widthAttrs;
		private AttributeSet attrs;
		private bool attrListenRequested;
		private InstanceTextField textField;

		internal InstanceComponent(InstanceFactory factory, Location loc, AttributeSet attrs)
		{
			this.listeners = null;
			this.factory = factory;
			this.instance = new Instance(this);
			this.loc = loc;
			this.bounds = factory.getOffsetBounds(attrs).translate(loc.X, loc.Y);
			this.portList = factory.Ports;
			this.endArray = null;
			this.hasToolTips = false;
			this.attrs = attrs;
			this.attrListenRequested = false;
			this.textField = null;

			computeEnds();
		}

		private void computeEnds()
		{
			List<Port> ports = portList;
			EndData[] esOld = endArray;
			int esOldLength = esOld == null ? 0 : esOld.Length;
			EndData[] es = esOld;
			if (es == null || es.Length != ports.Count)
			{
				es = new EndData[ports.Count];
				if (esOldLength > 0)
				{
					int toCopy = Math.Min(esOldLength, es.Length);
					Array.Copy(esOld, 0, es, 0, toCopy);
				}
			}
			HashSet<Attribute> wattrs = null;
			bool toolTipFound = false;
			List<EndData> endsChangedOld = new List<EndData>();
			List<EndData> endsChangedNew = new List<EndData>();
			IEnumerator<Port> pit = ports.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			for (int i = 0; pit.hasNext() || i < esOldLength; i++)
			{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Port p = pit.hasNext() ? pit.Current : null;
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") logisim.comp.EndData oldEnd = i < esOldLength ? esOld[i] : null;
				EndData oldEnd = i < esOldLength ? esOld[i] : null;
				EndData newEnd = p == null ? null : p.toEnd(loc, attrs);
				if (oldEnd == null || !oldEnd.Equals(newEnd))
				{
					if (newEnd != null)
					{
						es[i] = newEnd;
					}
					endsChangedOld.Add(oldEnd);
					endsChangedNew.Add(newEnd);
				}

				if (p != null)
				{
					Attribute attr = p.WidthAttribute;
					if (attr != null)
					{
						if (wattrs == null)
						{
							wattrs = new HashSet<Attribute>();
						}
						wattrs.Add(attr);
					}

					if (!string.ReferenceEquals(p.getToolTip(), null))
					{
						toolTipFound = true;
					}
				}
			}
			if (!attrListenRequested)
			{
				HashSet<Attribute> oldWattrs = widthAttrs;
				if (wattrs == null && oldWattrs != null)
				{
					AttributeSet.removeAttributeListener(this);
				}
				else if (wattrs != null && oldWattrs == null)
				{
					AttributeSet.addAttributeListener(this);
				}
			}
			if (es != esOld)
			{
				endArray = es;
				endList = new UnmodifiableList<EndData>(es);
			}
			widthAttrs = wattrs;
			hasToolTips = toolTipFound;
			if (endsChangedOld.Count > 0)
			{
				fireEndsChanged(endsChangedOld, endsChangedNew);
			}
		}

		//
		// listening methods
		//
		public virtual void addComponentListener(ComponentListener l)
		{
			EventSourceWeakSupport<ComponentListener> ls = listeners;
			if (ls == null)
			{
				ls = new EventSourceWeakSupport<ComponentListener>();
				ls.add(l);
				listeners = ls;
			}
			else
			{
				ls.add(l);
			}
		}

		public virtual void removeComponentListener(ComponentListener l)
		{
			if (listeners != null)
			{
				listeners.remove(l);
				if (listeners.Empty)
				{
					listeners = null;
				}
			}
		}

		private void fireEndsChanged(List<EndData> oldEnds, List<EndData> newEnds)
		{
			EventSourceWeakSupport<ComponentListener> ls = listeners;
			if (ls != null)
			{
				ComponentEvent e = null;
				foreach (ComponentListener l in ls)
				{
					if (e == null)
					{
						e = new ComponentEvent(this, oldEnds, newEnds);
					}
					l.endChanged(e);
				}
			}
		}

		internal virtual void fireInvalidated()
		{
			EventSourceWeakSupport<ComponentListener> ls = listeners;
			if (ls != null)
			{
				ComponentEvent e = null;
				foreach (ComponentListener l in ls)
				{
					if (e == null)
					{
						e = new ComponentEvent(this);
					}
					l.componentInvalidated(e);
				}
			}
		}

		//
		// basic information methods
		//
		public virtual ComponentFactory Factory
		{
			get
			{
				return factory;
			}
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
		}

		public virtual object getFeature(object key)
		{
			object ret = factory.getInstanceFeature(instance, key);
			if (ret != null)
			{
				return ret;
			}
			else if (key == typeof(ToolTipMaker))
			{
				object defaultTip = factory.DefaultToolTip;
				if (hasToolTips || defaultTip != null)
				{
					return this;
				}
			}
			else if (key == typeof(TextEditable))
			{
				InstanceTextField field = textField;
				if (field != null)
				{
					return field;
				}
			}
			return null;
		}

		//
		// location/extent methods
		//
		public virtual Location Location
		{
			get
			{
				return loc;
			}
		}

		public virtual Bounds Bounds
		{
			get
			{
				return bounds;
			}
		}

		public virtual Bounds getBounds(JGraphics g)
		{
			Bounds ret = bounds;
			InstanceTextField field = textField;
			if (field != null)
			{
				ret = ret.add(field.getBounds(g));
			}
			return ret;
		}

		public virtual bool contains(Location pt)
		{
			Location translated = pt.translate(-loc.X, -loc.Y);
			InstanceFactory factory = instance.Factory;
			return factory.contains(translated, instance.AttributeSet);
		}

		public virtual bool contains(Location pt, JGraphics g)
		{
			InstanceTextField field = textField;
			if (field != null && field.getBounds(g).contains(pt))
			{
				return true;
			}
			else
			{
				return contains(pt);
			}
		}

		//
		// propagation methods
		//
		public virtual List<EndData> Ends
		{
			get
			{
				return endList;
			}
		}

		public virtual EndData getEnd(int index)
		{
			return endArray[index];
		}

		public virtual bool endsAt(Location pt)
		{
			EndData[] ends = endArray;
			for (int i = 0; i < ends.Length; i++)
			{
				if (ends[i].Location.Equals(pt))
				{
					return true;
				}
			}
			return false;
		}

		public virtual void propagate(CircuitState state)
		{
			factory.propagate(state.getInstanceState(this));
		}

		//
		// drawing methods
		//
		public virtual void draw(ComponentDrawContext context)
		{
			InstancePainter painter = context.InstancePainter;
			painter.setInstance(this);
			factory.paintInstance(painter);
		}

		public virtual void expose(ComponentDrawContext context)
		{
			Bounds b = bounds;
			context.Destination.repaint(b.X, b.Y, b.Width, b.Height);
		}

		public virtual string getToolTip(ComponentUserEvent e)
		{
			int x = e.X;
			int y = e.Y;
			int i = -1;
			foreach (EndData end in endArray)
			{
				i++;
				if (end.Location.manhattanDistanceTo(x, y) < 10)
				{
					Port p = portList[i];
					return p.getToolTip();
				}
			}
			StringGetter defaultTip = factory.DefaultToolTip;
			return defaultTip == null ? null : defaultTip.get();
		}

		//
		// AttributeListener methods
		//
		public virtual void attributeListChanged(AttributeEvent e)
		{
		}

		public virtual void attributeValueChanged(AttributeEvent e)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = e.getAttribute();
			Attribute attr = e.Attribute;
			if (widthAttrs != null && widthAttrs.Contains(attr))
			{
				computeEnds();
			}
			if (attrListenRequested)
			{
				factory.instanceAttributeChanged(instance, e.Attribute);
			}
		}

		//
		// methods for InstancePainter
		//
		internal virtual void drawLabel(ComponentDrawContext context)
		{
			InstanceTextField field = textField;
			if (field != null)
			{
				field.draw(this, context);
			}
		}

		//
		// methods for Instance
		//
		internal virtual InstanceComponent Instance
		{
			get
			{
				return instance;
			}
		}

		internal virtual List<Port> Ports
		{
			get
			{
				return portList;
			}
			set
			{
				Port[] portsCopy = value.ToArray();
				portList = new UnmodifiableList<Port>(portsCopy);
				computeEnds();
			}
		}


		internal virtual void recomputeBounds()
		{
			Location p = loc;
			bounds = factory.getOffsetBounds(attrs).translate(p.X, p.Y);
		}

		internal virtual void addAttributeListener(Instance instance)
		{
			if (!attrListenRequested)
			{
				attrListenRequested = true;
				if (widthAttrs == null)
				{
					AttributeSet.addAttributeListener(this);
				}
			}
		}

		internal virtual void setTextField(Attribute labelAttr, Attribute fontAttr, int x, int y, int halign, int valign)
		{
			InstanceTextField field = textField;
			if (field == null)
			{
				field = new InstanceTextField(this);
				field.update(labelAttr, fontAttr, x, y, halign, valign);
				textField = field;
			}
			else
			{
				field.update(labelAttr, fontAttr, x, y, halign, valign);
			}
		}

	}

}
