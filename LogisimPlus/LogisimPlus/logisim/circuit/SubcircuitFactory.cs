// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{


	using Component = logisim.comp.Component;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using Project = logisim.proj.Project;
	using Pin = logisim.std.wiring.Pin;
	using MenuExtender = logisim.tools.MenuExtender;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using StringGetter = logisim.util.StringGetter;
	using StringUtil = logisim.util.StringUtil;
    using LogisimPlus.Java;

    public class SubcircuitFactory : InstanceFactory
	{
		private class CircuitFeature : StringGetter, MenuExtender, ActionListener
		{
			private readonly SubcircuitFactory outerInstance;

			internal Instance instance;
			internal Project proj;

			public CircuitFeature(SubcircuitFactory outerInstance, Instance instance)
			{
				this.outerInstance = outerInstance;
				this.instance = instance;
			}

			public virtual string get()
			{
				return outerInstance.source.Name;
			}

			public virtual void configureMenu(JPopupMenu menu, Project proj)
			{
				this.proj = proj;
				string name = instance.Factory.DisplayName;
				string text = Strings.get("subcircuitViewItem", name);
				JMenuItem item = new JMenuItem(text);
				item.addActionListener(this);
				menu.add(item);
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				CircuitState superState = proj.CircuitState;
				if (superState == null)
				{
					return;
				}

				CircuitState subState = outerInstance.getSubstate(superState, instance);
				if (subState == null)
				{
					return;
				}
				proj.CircuitState = subState;
			}
		}

		private Circuit source;

		public SubcircuitFactory(Circuit source) : base("", null)
		{
			this.source = source;
			FacingAttribute = StdAttr.FACING;
			DefaultToolTip = new CircuitFeature(this, null);
			InstancePoker = typeof(SubcircuitPoker);
		}

		public virtual Circuit Subcircuit
		{
			get
			{
				return source;
			}
		}

		public override string Name
		{
			get
			{
				return source.Name;
			}
		}

		public override StringGetter DisplayGetter
		{
			get
			{
				return StringUtil.constantGetter(source.Name);
			}
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			Direction defaultFacing = source.Appearance.Facing;
			Bounds bds = source.Appearance.OffsetBounds;
			return bds.rotate(defaultFacing, facing, 0, 0);
		}

		public override AttributeSet createAttributeSet()
		{
			return new CircuitAttributes(source);
		}

		//
		// methods for configuring instances
		//
		public override void configureNewInstance(Instance instance)
		{
			CircuitAttributes attrs = (CircuitAttributes) instance.AttributeSet;
			attrs.Subcircuit = instance;

			instance.addAttributeListener();
			computePorts(instance);
			// configureLabel(instance); already done in computePorts
		}

		public override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.FACING)
			{
				computePorts(instance);
			}
			else if (attr == CircuitAttributes.LABEL_LOCATION_ATTR)
			{
				configureLabel(instance);
			}
		}

		public override object getInstanceFeature(Instance instance, object key)
		{
			if (key == typeof(MenuExtender))
			{
				return new CircuitFeature(this, instance);
			}
			return base.getInstanceFeature(instance, key);
		}

		internal virtual void computePorts(Instance instance)
		{
			Direction facing = instance.getAttributeValue(StdAttr.FACING);
			IDictionary<Location, Instance> portLocs = source.Appearance.getPortOffsets(facing);
			Port[] ports = new Port[portLocs.Count];
			Instance[] pins = new Instance[portLocs.Count];
			int i = -1;
			foreach (KeyValuePair<Location, Instance> portLoc in portLocs.SetOfKeyValuePairs())
			{
				i++;
				Location loc = portLoc.Key;
				Instance pin = portLoc.Value;
				string type = Pin.FACTORY.isInputPin(pin) ? Port.INPUT : Port.OUTPUT;
				BitWidth width = pin.getAttributeValue(StdAttr.WIDTH);
				ports[i] = new Port(loc.X, loc.Y, type, width);
				pins[i] = pin;

				string label = pin.getAttributeValue(StdAttr.LABEL);
				if (!string.ReferenceEquals(label, null) && label.Length > 0)
				{
					ports[i].setToolTip(StringUtil.constantGetter(label));
				}
			}

			CircuitAttributes attrs = (CircuitAttributes) instance.AttributeSet;
			attrs.PinInstances = pins;
			instance.setPorts(ports);
			instance.recomputeBounds();
			configureLabel(instance); // since this affects the circuit's bounds
		}

		private void configureLabel(Instance instance)
		{
			Bounds bds = instance.Bounds;
			Direction loc = instance.getAttributeValue(CircuitAttributes.LABEL_LOCATION_ATTR);

			int x = bds.X + bds.Width / 2;
			int y = bds.Y + bds.Height / 2;
			int ha = GraphicsUtil.H_CENTER;
			int va = GraphicsUtil.V_CENTER;
			if (loc == Direction.East)
			{
				x = bds.X + bds.Width + 2;
				ha = GraphicsUtil.H_LEFT;
			}
			else if (loc == Direction.West)
			{
				x = bds.X - 2;
				ha = GraphicsUtil.H_RIGHT;
			}
			else if (loc == Direction.South)
			{
				y = bds.Y + bds.Height + 2;
				va = GraphicsUtil.V_TOP;
			}
			else
			{
				y = bds.Y - 2;
				va = GraphicsUtil.V_BASELINE;
			}
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, x, y, ha, va);
		}

		//
		// propagation-oriented methods
		//
		public virtual CircuitState getSubstate(CircuitState superState, Instance instance)
		{
			return getSubstate(createInstanceState(superState, instance));
		}

		public virtual CircuitState getSubstate(CircuitState superState, Component comp)
		{
			return getSubstate(createInstanceState(superState, comp));
		}

		private CircuitState getSubstate(InstanceState instanceState)
		{
			CircuitState subState = (CircuitState) instanceState.Data;
			if (subState == null)
			{
				subState = new CircuitState(instanceState.Project, source);
				instanceState.Data = subState;
				instanceState.fireInvalidated();
			}
			return subState;
		}

		public override void propagate(InstanceState superState)
		{
			CircuitState subState = getSubstate(superState);

			CircuitAttributes attrs = (CircuitAttributes) superState.AttributeSet;
			Instance[] pins = attrs.PinInstances;
			for (int i = 0; i < pins.Length; i++)
			{
				Instance pin = pins[i];
				InstanceState pinState = subState.getInstanceState(pin);
				if (Pin.FACTORY.isInputPin(pin))
				{
					Value newVal = superState.getPort(i);
					Value oldVal = Pin.FACTORY.getValue(pinState);
					if (!newVal.Equals(oldVal))
					{
						Pin.FACTORY.setValue(pinState, newVal);
						Pin.FACTORY.propagate(pinState);
					}
				}
				else
				{ // it is output-only
					Value val = pinState.getPort(0);
					superState.setPort(i, val, 1);
				}
			}
		}

		//
		// user interface features
		//
		public override void paintGhost(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			Color fg = g.getColor();
			int v = fg.getRed() + fg.getGreen() + fg.getBlue();
			Composite oldComposite = null;
			if (g is Graphics2D && v > 50)
			{
				oldComposite = ((Graphics2D) g).getComposite();
				Composite c = AlphaComposite.getInstance(AlphaComposite.SRC_OVER, 0.5f);
				((Graphics2D) g).setComposite(c);
			}
			paintBase(painter, g);
			if (oldComposite != null)
			{
				((Graphics2D) g).setComposite(oldComposite);
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			paintBase(painter, painter.Graphics);
			painter.drawPorts();
		}

		private void paintBase(InstancePainter painter, Graphics g)
		{
			CircuitAttributes attrs = (CircuitAttributes) painter.AttributeSet;
			Direction facing = attrs.Facing;
			Direction defaultFacing = source.Appearance.Facing;
			Location loc = painter.Location;
			g.translate(loc.X, loc.Y);
			source.Appearance.paintSubcircuit(g, facing);
			drawCircuitLabel(painter, getOffsetBounds(attrs), facing, defaultFacing);
			g.translate(-loc.X, -loc.Y);
			painter.drawLabel();
		}

		private void drawCircuitLabel(InstancePainter painter, Bounds bds, Direction facing, Direction defaultFacing)
		{
			AttributeSet staticAttrs = source.StaticAttributes;
			string label = staticAttrs.getValue(CircuitAttributes.CIRCUIT_LABEL_ATTR);
			if (!string.ReferenceEquals(label, null) && !label.Equals(""))
			{
				Direction up = staticAttrs.getValue(CircuitAttributes.CIRCUIT_LABEL_FACING_ATTR);
				Font font = staticAttrs.getValue(CircuitAttributes.CIRCUIT_LABEL_FONT_ATTR);

				int back = label.IndexOf('\\');
				int lines = 1;
				bool backs = false;
				while (back >= 0 && back <= label.Length - 2)
				{
					char c = label[back + 1];
					if (c == 'n')
					{
						lines++;
					}
					else if (c == '\\')
					{
						backs = true;
					}
					back = label.IndexOf('\\', back + 2);
				}

				int x = bds.X + bds.Width / 2;
				int y = bds.Y + bds.Height / 2;
				JGraphics g = painter.Graphics;
				double angle = Math.PI / 2 - (up.toRadians() - defaultFacing.toRadians()) - facing.toRadians();
				if (Math.Abs(angle) > 0.01)
				{
					g.rotate(angle, x, y);
				}
				g.setFont(font);
				if (lines == 1 && !backs)
				{
					GraphicsUtil.drawCenteredText(g, label, x, y);
				}
				else
				{
                    SizeF labelSize = g.measureString(label);
                    int height = labelSize.Height;
					y = y - (height * lines - labelSize.Height) / 2 + labelSize.Height;
					back = label.IndexOf('\\');
					while (back >= 0 && back <= label.Length - 2)
					{
						char c = label[back + 1];
						if (c == 'n')
						{
							string line = label.Substring(0, back);
							GraphicsUtil.drawText(g, line, x, y, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
							y += height;
							label = label.Substring(back + 2);
							back = label.IndexOf('\\');
						}
						else if (c == '\\')
						{
							label = label.Substring(0, back) + label.Substring(back + 1);
							back = label.IndexOf('\\', back + 1);
						}
						else
						{
							back = label.IndexOf('\\', back + 2);
						}
					}
					GraphicsUtil.drawText(g, label, x, y, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
				}
				g.dispose();
			}
		}

		/*
		 * TODO public String getToolTip(ComponentUserEvent e) { return
		 * StringUtil.format(Strings.get("subcircuitCircuitTip"), source.getDisplayName()); }
		 */
	}

}
