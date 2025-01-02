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
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using StringUtil = logisim.util.StringUtil;

	public class Counter : InstanceFactory
	{
		internal static readonly AttributeOption ON_GOAL_WRAP = new AttributeOption("wrap", "wrap", Strings.getter("counterGoalWrap"));
		internal static readonly AttributeOption ON_GOAL_STAY = new AttributeOption("stay", "stay", Strings.getter("counterGoalStay"));
		internal static readonly AttributeOption ON_GOAL_CONT = new AttributeOption("continue", "continue", Strings.getter("counterGoalContinue"));
		internal static readonly AttributeOption ON_GOAL_LOAD = new AttributeOption("load", "load", Strings.getter("counterGoalLoad"));

		internal static readonly Attribute<int> ATTR_MAX = Attributes.forHexInteger("max", Strings.getter("counterMaxAttr"));
		internal static readonly Attribute<AttributeOption> ATTR_ON_GOAL = Attributes.forOption("ongoal", Strings.getter("counterGoalAttr"), new AttributeOption[] {ON_GOAL_WRAP, ON_GOAL_STAY, ON_GOAL_CONT, ON_GOAL_LOAD});

		private const int DELAY = 8;
		private const int OUT = 0;
		private const int IN = 1;
		private const int CK = 2;
		private const int CLR = 3;
		private const int LD = 4;
		private const int CT = 5;
		private const int CARRY = 6;

		public Counter() : base("Counter", Strings.getter("counterComponent"))
		{
			OffsetBounds = Bounds.create(-30, -20, 30, 40);
			IconName = "counter.gif";
			InstancePoker = typeof(RegisterPoker);
			InstanceLogger = typeof(RegisterLogger);
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);

			Port[] ps = new Port[7];
			ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			ps[IN] = new Port(-30, 0, Port.INPUT, StdAttr.WIDTH);
			ps[CK] = new Port(-20, 20, Port.INPUT, 1);
			ps[CLR] = new Port(-10, 20, Port.INPUT, 1);
			ps[LD] = new Port(-30, -10, Port.INPUT, 1);
			ps[CT] = new Port(-30, 10, Port.INPUT, 1);
			ps[CARRY] = new Port(0, 10, Port.OUTPUT, 1);
			ps[OUT].setToolTip(Strings.getter("counterQTip"));
			ps[IN].setToolTip(Strings.getter("counterDataTip"));
			ps[CK].setToolTip(Strings.getter("counterClockTip"));
			ps[CLR].setToolTip(Strings.getter("counterResetTip"));
			ps[LD].setToolTip(Strings.getter("counterLoadTip"));
			ps[CT].setToolTip(Strings.getter("counterEnableTip"));
			ps[CARRY].setToolTip(Strings.getter("counterCarryTip"));
			setPorts(ps);
		}

		public override AttributeSet createAttributeSet()
		{
			return new CounterAttributes();
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			Bounds bds = instance.Bounds;
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, bds.X + bds.Width / 2, bds.Y - 3, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
		}

		public override void propagate(InstanceState state)
		{
			RegisterData data = (RegisterData) state.Data;
			if (data == null)
			{
				data = new RegisterData();
				state.Data = data;
			}

			BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
			object triggerType = state.getAttributeValue(StdAttr.EDGE_TRIGGER);
			int max = (int)state.getAttributeValue(ATTR_MAX);
			Value clock = state.getPort(CK);
			bool triggered = data.updateClock(clock, triggerType);

			Value newValue;
			bool carry;
			if (state.getPort(CLR) == Value.TRUE)
			{
				newValue = Value.createKnown(dataWidth, 0);
				carry = false;
			}
			else
			{
				bool ld = state.getPort(LD) == Value.TRUE;
				bool ct = state.getPort(CT) != Value.FALSE;
				int oldVal = data.value;
				int newVal;
				if (!triggered)
				{
					newVal = oldVal;
				}
				else if (ct)
				{ // trigger, enable = 1: should increment or decrement
					int goal = ld ? 0 : max;
					if (oldVal == goal)
					{
						object onGoal = state.getAttributeValue(ATTR_ON_GOAL);
						if (onGoal == ON_GOAL_WRAP)
						{
							newVal = ld ? max : 0;
						}
						else if (onGoal == ON_GOAL_STAY)
						{
							newVal = oldVal;
						}
						else if (onGoal == ON_GOAL_LOAD)
						{
							Value @in = state.getPort(IN);
							newVal = @in.FullyDefined ? @in.toIntValue() : 0;
							if (newVal > max)
							{
								newVal &= max;
							}
						}
						else if (onGoal == ON_GOAL_CONT)
						{
							newVal = ld ? oldVal - 1 : oldVal + 1;
						}
						else
						{
							Console.Error.WriteLine("Invalid goal attribute " + onGoal); // OK
							newVal = ld ? max : 0;
						}
					}
					else
					{
						newVal = ld ? oldVal - 1 : oldVal + 1;
					}
				}
				else if (ld)
				{ // trigger, enable = 0, load = 1: should load
					Value @in = state.getPort(IN);
					newVal = @in.FullyDefined ? @in.toIntValue() : 0;
					if (newVal > max)
					{
						newVal &= max;
					}
				}
				else
				{ // trigger, enable = 0, load = 0: no change
					newVal = oldVal;
				}
				newValue = Value.createKnown(dataWidth, newVal);
				newVal = newValue.toIntValue();
				carry = newVal == (ld && ct ? 0 : max);
				/*
				 * I would want this if I were worried about the carry signal outrunning the clock. But the component's
				 * delay should be enough to take care of it. if (carry) { if (triggerType == StdAttr.TRIG_FALLING) { carry
				 * = clock == Value.TRUE; } else { carry = clock == Value.FALSE; } }
				 */
			}

			data.value = newValue.toIntValue();
			state.setPort(OUT, newValue, DELAY);
			state.setPort(CARRY, carry ? Value.TRUE : Value.FALSE, DELAY);
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") @Override public void paintInstance(logisim.instance.InstancePainter painter)
		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			Bounds bds = painter.Bounds;
			RegisterData state = (RegisterData) painter.Data;
			BitWidth widthVal = painter.getAttributeValue(StdAttr.WIDTH);
			int width = widthVal == null ? 8 : widthVal.Width;

			// determine text to draw in label
			string a;
			string b = null;
			if (painter.ShowState)
			{
				int val = state == null ? 0 : state.value;
				string str = StringUtil.toHexString(width, val);
				if (str.Length <= 4)
				{
					a = str;
				}
				else
				{
					int split = str.Length - 4;
					a = str.Substring(0, split);
					b = str.Substring(split);
				}
			}
			else
			{
				a = Strings.get("counterLabel");
				b = Strings.get("registerWidthLabel", "" + widthVal.Width);
			}

			// draw boundary, label
			painter.drawBounds();
			painter.drawLabel();

			// draw input and output ports
			if (string.ReferenceEquals(b, null))
			{
				painter.drawPort(IN, "D", Direction.East);
				painter.drawPort(OUT, "Q", Direction.West);
			}
			else
			{
				painter.drawPort(IN);
				painter.drawPort(OUT);
			}
			g.setColor(Color.GRAY);
			painter.drawPort(LD);
			painter.drawPort(CARRY);
			painter.drawPort(CLR, "0", Direction.South);
			painter.drawPort(CT, Strings.get("counterEnableLabel"), Direction.East);
			g.setColor(Color.BLACK);
			painter.drawClock(CK, Direction.North);

			// draw contents
			if (string.ReferenceEquals(b, null))
			{
				GraphicsUtil.drawText(g, a, bds.X + 15, bds.Y + 4, GraphicsUtil.H_CENTER, GraphicsUtil.V_TOP);
			}
			else
			{
				GraphicsUtil.drawText(g, a, bds.X + 15, bds.Y + 3, GraphicsUtil.H_CENTER, GraphicsUtil.V_TOP);
				GraphicsUtil.drawText(g, b, bds.X + 15, bds.Y + 15, GraphicsUtil.H_CENTER, GraphicsUtil.V_TOP);
			}
		}
	}
}
