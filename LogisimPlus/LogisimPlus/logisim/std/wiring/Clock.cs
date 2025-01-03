// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using CircuitState = logisim.circuit.CircuitState;
	using RadixOption = logisim.circuit.RadixOption;
	using Component = logisim.comp.Component;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceData = logisim.instance.InstanceData;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
	using Icons = logisim.util.Icons;
    using LogisimPlus.Java;

    public class Clock : InstanceFactory
	{
		public static readonly Attribute ATTR_HIGH = new DurationAttribute("highDuration", Strings.getter("clockHighAttr"), 1, int.MaxValue);

		public static readonly Attribute ATTR_LOW = new DurationAttribute("lowDuration", Strings.getter("clockLowAttr"), 1, int.MaxValue);

		public static readonly Clock FACTORY = new Clock();

		private static readonly Icon toolIcon = Icons.getIcon("clock.gif");

		private class ClockState : InstanceData, ICloneable
		{
			internal Value sending = Value.FALSE;
			internal int clicks = 0;

			public virtual object Clone()
			{
				return base.MemberwiseClone();
			}
		}

		public class ClockLogger : InstanceLogger
		{
			public override string getLogName(InstanceState state, object option)
			{
				return state.getAttributeValue(StdAttr.LABEL);
			}

			public override Value getLogValue(InstanceState state, object option)
			{
				ClockState s = getState(state);
				return s.sending;
			}
		}

		public class ClockPoker : InstancePoker
		{
			internal bool isPressed = true;

			public override void mousePressed(InstanceState state, MouseEvent e)
			{
				isPressed = isInside(state, e);
			}

			public override void mouseReleased(InstanceState state, MouseEvent e)
			{
				if (isPressed && isInside(state, e))
				{
					ClockState myState = (ClockState) state.Data;
					myState.sending = myState.sending.not();
					myState.clicks++;
					state.fireInvalidated();
				}
				isPressed = false;
			}

			internal virtual bool isInside(InstanceState state, MouseEvent e)
			{
				Bounds bds = state.Instance.Bounds;
				return bds.contains(e.x, e.y);
			}
		}

		public Clock() : base("Clock", Strings.getter("clockComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, ATTR_HIGH, ATTR_LOW, StdAttr.LABEL, Pin.ATTR_LABEL_LOC, StdAttr.LABEL_FONT}, new object[] {Direction.East, Convert.ToInt32(1), Convert.ToInt32(1), "", Direction.West, StdAttr.DEFAULT_LABEL_FONT});
			FacingAttribute = StdAttr.FACING;
			InstanceLogger = typeof(ClockLogger);
			InstancePoker = typeof(ClockPoker);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			return Probe.getOffsetBounds((Direction)attrs.getValue(StdAttr.FACING), BitWidth.ONE, RadixOption.RADIX_2);
		}

		//
		// JGraphics methods
		//
		public override void paintIcon(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			if (toolIcon != null)
			{
				toolIcon.paintIcon(painter.Destination, g, 2, 2);
			}
			else
			{
				g.drawRect(4, 4, 13, 13);
				g.setColor(Value.FALSE.Color);
				g.drawPolyline(new int[] {6, 6, 10, 10, 14, 14}, new int[] {10, 6, 6, 14, 14, 10}, 6);
			}

			Direction dir = (Direction)painter.getAttributeValue(StdAttr.FACING);
			int pinx = 15;
			int piny = 8;
			if (dir == Direction.East)
			{ // keep defaults
			}
			else if (dir == Direction.West)
			{
				pinx = 3;
			}
			else if (dir == Direction.North)
			{
				pinx = 8;
				piny = 3;
			}
			else if (dir == Direction.South)
			{
				pinx = 8;
				piny = 15;
			}
			g.setColor(Value.TRUE.Color);
			g.fillOval(pinx, piny, 3, 3);
		}

		public override void paintInstance(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			Bounds bds = painter.getInstance().Bounds; // intentionally with no JGraphics object - we don't want label
															// included
			int x = bds.X;
			int y = bds.Y;
			JGraphicsUtil.switchToWidth(g, 2);
			g.setColor(Color.Black);
			g.drawRect(x, y, bds.Width, bds.Height);

			painter.drawLabel();

			bool drawUp;
			if (painter.ShowState)
			{
				ClockState state = getState(painter);
				g.setColor(state.sending.Color);
				drawUp = state.sending == Value.TRUE;
			}
			else
			{
				g.setColor(Color.Black);
				drawUp = true;
			}
			x += 10;
			y += 10;
			int[] xs = new int[] {x - 6, x - 6, x, x, x + 6, x + 6};
			int[] ys;
			if (drawUp)
			{
				ys = new int[] {y, y - 4, y - 4, y + 4, y + 4, y};
			}
			else
			{
				ys = new int[] {y, y + 4, y + 4, y - 4, y - 4, y};
			}
			g.drawPolyline(xs, ys, xs.Length);

			painter.drawPorts();
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			instance.Ports = new Port[] { new Port(0, 0, Port.OUTPUT, BitWidth.ONE) };
			configureLabel(instance);
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == Pin.ATTR_LABEL_LOC)
			{
				configureLabel(instance);
			}
			else if (attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
				configureLabel(instance);
			}
		}

		public override void propagate(InstanceState state)
		{
			Value val = state.getPort(0);
			ClockState q = getState(state);
			if (!val.Equals(q.sending))
			{ // ignore if no change
				state.setPort(0, q.sending, 1);
			}
		}

		//
		// package methods
		//
		public static bool tick(CircuitState circState, int ticks, Component comp)
		{
			AttributeSet attrs = comp.AttributeSet;
			int durationHigh = (int)attrs.getValue(ATTR_HIGH);
			int durationLow = (int)attrs.getValue(ATTR_LOW);
			ClockState state = (ClockState) circState.getData(comp);
			if (state == null)
			{
				state = new ClockState();
				circState.setData(comp, state);
			}
			bool curValue = ticks % (durationHigh + durationLow) < durationLow;
			if (state.clicks % 2 == 1)
			{
				curValue = !curValue;
			}
			Value desired = (curValue ? Value.FALSE : Value.TRUE);
			if (!state.sending.Equals(desired))
			{
				state.sending = desired;
				Instance.getInstanceFor(comp).fireInvalidated();
				return true;
			}
			else
			{
				return false;
			}
		}

		//
		// private methods
		//
		private void configureLabel(Instance instance)
		{
			Direction facing = (Direction)instance.getAttributeValue(StdAttr.FACING);
			Direction labelLoc = instance.getAttributeValue(Pin.ATTR_LABEL_LOC);
			Probe.configureLabel(instance, labelLoc, facing);
		}

		private static ClockState getState(InstanceState state)
		{
			ClockState ret = (ClockState) state.Data;
			if (ret == null)
			{
				ret = new ClockState();
				state.Data = ret;
			}
			return ret;
		}
	}

}
