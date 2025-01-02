// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
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
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using StringGetter = logisim.util.StringGetter;

	internal abstract class AbstractFlipFlop : InstanceFactory
	{
		private const int STD_PORTS = 6;

		private Attribute<AttributeOption> triggerAttribute;

		protected internal AbstractFlipFlop(string name, string iconName, StringGetter desc, int numInputs, bool allowLevelTriggers) : base(name, desc)
		{
			IconName = iconName;
			triggerAttribute = allowLevelTriggers ? StdAttr.TRIGGER : StdAttr.EDGE_TRIGGER;
			setAttributes(new Attribute[] {triggerAttribute, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {StdAttr.TRIG_RISING, "", StdAttr.DEFAULT_LABEL_FONT});
			OffsetBounds = Bounds.create(-40, -10, 40, 40);
			InstancePoker = typeof(Poker);
			InstanceLogger = typeof(Logger);

			Port[] ps = new Port[numInputs + STD_PORTS];
			if (numInputs == 1)
			{
				ps[0] = new Port(-40, 20, Port.INPUT, 1);
				ps[1] = new Port(-40, 0, Port.INPUT, 1);
			}
			else if (numInputs == 2)
			{
				ps[0] = new Port(-40, 0, Port.INPUT, 1);
				ps[1] = new Port(-40, 20, Port.INPUT, 1);
				ps[2] = new Port(-40, 10, Port.INPUT, 1);
			}
			else
			{
				throw new Exception("flip-flop input > 2");
			}
			ps[numInputs + 1] = new Port(0, 0, Port.OUTPUT, 1);
			ps[numInputs + 2] = new Port(0, 20, Port.OUTPUT, 1);
			ps[numInputs + 3] = new Port(-10, 30, Port.INPUT, 1);
			ps[numInputs + 4] = new Port(-30, 30, Port.INPUT, 1);
			ps[numInputs + 5] = new Port(-20, 30, Port.INPUT, 1);
			ps[numInputs].setToolTip(Strings.getter("flipFlopClockTip"));
			ps[numInputs + 1].setToolTip(Strings.getter("flipFlopQTip"));
			ps[numInputs + 2].setToolTip(Strings.getter("flipFlopNotQTip"));
			ps[numInputs + 3].setToolTip(Strings.getter("flipFlopResetTip"));
			ps[numInputs + 4].setToolTip(Strings.getter("flipFlopPresetTip"));
			ps[numInputs + 5].setToolTip(Strings.getter("flipFlopEnableTip"));
			setPorts(ps);
		}

		//
		// abstract methods intended to be implemented in subclasses
		//
		protected internal abstract string getInputName(int index);

		protected internal abstract Value computeValue(Value[] inputs, Value curValue);

		//
		// concrete methods not intended to be overridden
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			Bounds bds = instance.Bounds;
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, bds.X + bds.Width / 2, bds.Y - 3, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
		}

		public override void propagate(InstanceState state)
		{
			StateData data = (StateData) state.Data;
			if (data == null)
			{
				data = new StateData();
				state.Data = data;
			}

			int n = Ports.Count - STD_PORTS;
			object triggerType = state.getAttributeValue(triggerAttribute);
			bool triggered = data.updateClock(state.getPort(n), triggerType);

			if (state.getPort(n + 3) == Value.TRUE) // clear requested
			{
				data.curValue = Value.FALSE;
			}
			else if (state.getPort(n + 4) == Value.TRUE) // preset requested
			{
				data.curValue = Value.TRUE;
			}
			else if (triggered && state.getPort(n + 5) != Value.FALSE)
			{
				// Clock has triggered and flip-flop is enabled: Update the state
				Value[] inputs = new Value[n];
				for (int i = 0; i < n; i++)
				{
					inputs[i] = state.getPort(i);
				}

				Value newVal = computeValue(inputs, data.curValue);
				if (newVal == Value.TRUE || newVal == Value.FALSE)
				{
					data.curValue = newVal;
				}
			}

			state.setPort(n + 1, data.curValue, Memory.DELAY);
			state.setPort(n + 2, data.curValue.not(), Memory.DELAY);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			painter.drawBounds();
			painter.drawLabel();
			if (painter.ShowState)
			{
				Location loc = painter.Location;
				StateData myState = (StateData) painter.Data;
				if (myState != null)
				{
					int x = loc.X;
					int y = loc.Y;
					g.setColor(myState.curValue.Color);
					g.fillOval(x - 26, y + 4, 13, 13);
					g.setColor(Color.WHITE);
					GraphicsUtil.drawCenteredText(g, myState.curValue.toDisplayString(), x - 19, y + 9);
					g.setColor(Color.BLACK);
				}
			}

			int n = Ports.Count - STD_PORTS;
			g.setColor(Color.GRAY);
			painter.drawPort(n + 3, "0", Direction.South);
			painter.drawPort(n + 4, "1", Direction.South);
			painter.drawPort(n + 5, Strings.get("memEnableLabel"), Direction.South);
			g.setColor(Color.BLACK);
			for (int i = 0; i < n; i++)
			{
				painter.drawPort(i, getInputName(i), Direction.East);
			}
			painter.drawClock(n, Direction.East);
			painter.drawPort(n + 1, "Q", Direction.West);
			painter.drawPort(n + 2);
		}

		private class StateData : ClockState, InstanceData
		{
			internal Value curValue = Value.FALSE;
		}

		public class Logger : InstanceLogger
		{
			public override string getLogName(InstanceState state, object option)
			{
				string ret = state.getAttributeValue(StdAttr.LABEL);
				return !string.ReferenceEquals(ret, null) && !ret.Equals("") ? ret : null;
			}

			public override Value getLogValue(InstanceState state, object option)
			{
				StateData s = (StateData) state.Data;
				return s == null ? Value.FALSE : s.curValue;
			}
		}

		public class Poker : InstancePoker
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
					StateData myState = (StateData) state.Data;
					if (myState == null)
					{
						return;
					}

					myState.curValue = myState.curValue.not();
					state.fireInvalidated();
				}
				isPressed = false;
			}

			internal virtual bool isInside(InstanceState state, MouseEvent e)
			{
				Location loc = state.Instance.Location;
				int dx = e.getX() - (loc.X - 20);
				int dy = e.getY() - (loc.Y + 10);
				int d2 = dx * dx + dy * dy;
				return d2 < 8 * 8;
			}
		}
	}

}
