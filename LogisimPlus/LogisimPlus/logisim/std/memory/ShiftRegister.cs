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
	using IntegerConfigurator = logisim.tools.key.IntegerConfigurator;
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class ShiftRegister : InstanceFactory
	{
		internal static readonly Attribute<int> ATTR_LENGTH = Attributes.forIntegerRange("length", Strings.getter("shiftRegLengthAttr"), 1, 32);
		internal static readonly Attribute<bool> ATTR_LOAD = Attributes.forBoolean("parallel", Strings.getter("shiftRegParallelAttr"));

		private const int IN = 0;
		private const int SH = 1;
		private const int CK = 2;
		private const int CLR = 3;
		private const int OUT = 4;
		private const int LD = 5;

		public ShiftRegister() : base("Shift Register", Strings.getter("shiftRegisterComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.WIDTH, ATTR_LENGTH, ATTR_LOAD, StdAttr.EDGE_TRIGGER, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {BitWidth.ONE, Convert.ToInt32(8), true, StdAttr.TRIG_RISING, "", StdAttr.DEFAULT_LABEL_FONT});
			KeyConfigurator = JoinedConfigurator.create(new IntegerConfigurator(ATTR_LENGTH, 1, 32, 0), new BitWidthConfigurator(StdAttr.WIDTH));

			IconName = "shiftreg.gif";
			InstanceLogger = typeof(ShiftRegisterLogger);
			InstancePoker = typeof(ShiftRegisterPoker);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			object parallel = attrs.getValue(ATTR_LOAD);
			if (parallel == null || ((bool?) parallel).Value)
			{
				int len = (int)attrs.getValue(ATTR_LENGTH);
				return Bounds.create(0, -20, 20 + 10 * len, 40);
			}
			else
			{
				return Bounds.create(0, -20, 30, 40);
			}
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			configurePorts(instance);
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == ATTR_LOAD || attr == ATTR_LENGTH || attr == StdAttr.WIDTH)
			{
				instance.recomputeBounds();
				configurePorts(instance);
			}
		}

		private void configurePorts(Instance instance)
		{
			BitWidth widthObj = instance.getAttributeValue(StdAttr.WIDTH);
			int width = widthObj.Width;
			bool? parallelObj = instance.getAttributeValue(ATTR_LOAD);
			Bounds bds = instance.Bounds;
			Port[] ps;
			if (parallelObj == null || parallelObj.Value)
			{
				int? lenObj = instance.getAttributeValue(ATTR_LENGTH);
				int len = lenObj == null ? 8 : lenObj.Value;
				ps = new Port[6 + 2 * len];
				ps[LD] = new Port(10, -20, Port.INPUT, 1);
				ps[LD].setToolTip(Strings.getter("shiftRegLoadTip"));
				for (int i = 0; i < len; i++)
				{
					ps[6 + 2 * i] = new Port(20 + 10 * i, -20, Port.INPUT, width);
					ps[6 + 2 * i + 1] = new Port(20 + 10 * i, 20, Port.OUTPUT, width);
				}
			}
			else
			{
				ps = new Port[5];
			}
			ps[OUT] = new Port(bds.Width, 0, Port.OUTPUT, width);
			ps[SH] = new Port(0, -10, Port.INPUT, 1);
			ps[IN] = new Port(0, 0, Port.INPUT, width);
			ps[CK] = new Port(0, 10, Port.INPUT, 1);
			ps[CLR] = new Port(10, 20, Port.INPUT, 1);
			ps[OUT].setToolTip(Strings.getter("shiftRegOutTip"));
			ps[SH].setToolTip(Strings.getter("shiftRegShiftTip"));
			ps[IN].setToolTip(Strings.getter("shiftRegInTip"));
			ps[CK].setToolTip(Strings.getter("shiftRegClockTip"));
			ps[CLR].setToolTip(Strings.getter("shiftRegClearTip"));
			instance.setPorts(ps);

			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, bds.X + bds.Width / 2, bds.Y + bds.Height / 4, GraphicsUtil.H_CENTER, GraphicsUtil.V_CENTER);
		}

		private ShiftRegisterData getData(InstanceState state)
		{
			BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
			int? lenObj = state.getAttributeValue(ATTR_LENGTH);
			int length = lenObj == null ? 8 : lenObj.Value;
			ShiftRegisterData data = (ShiftRegisterData) state.Data;
			if (data == null)
			{
				data = new ShiftRegisterData(width, length);
				state.Data = data;
			}
			else
			{
				data.setSizes(width, length);
			}
			return data;
		}

		public override void propagate(InstanceState state)
		{
			object triggerType = state.getAttributeValue(StdAttr.EDGE_TRIGGER);
			bool parallel = (bool)state.getAttributeValue(ATTR_LOAD);
			ShiftRegisterData data = getData(state);
			int len = data.Length;

			bool triggered = data.updateClock(state.getPort(CK), triggerType);
			if (state.getPort(CLR) == Value.TRUE)
			{
				data.clear();
			}
			else if (triggered)
			{
				if (parallel && state.getPort(LD) == Value.TRUE)
				{
					data.clear();
					for (int i = len - 1; i >= 0; i--)
					{
						data.push(state.getPort(6 + 2 * i));
					}
				}
				else if (state.getPort(SH) != Value.FALSE)
				{
					data.push(state.getPort(IN));
				}
			}

			state.setPort(OUT, data.get(0), 4);
			if (parallel)
			{
				for (int i = 0; i < len; i++)
				{
					state.setPort(6 + 2 * i + 1, data.get(len - 1 - i), 4);
				}
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			// draw boundary, label
			painter.drawBounds();
			painter.drawLabel();

			// draw state
			bool parallel = (bool)painter.getAttributeValue(ATTR_LOAD);
			if (parallel)
			{
				BitWidth widObj = painter.getAttributeValue(StdAttr.WIDTH);
				int wid = widObj.Width;
				int? lenObj = painter.getAttributeValue(ATTR_LENGTH);
				int len = lenObj == null ? 8 : lenObj.Value;
				if (painter.ShowState)
				{
					if (wid <= 4)
					{
						ShiftRegisterData data = getData(painter);
						Bounds bds = painter.Bounds;
						int x = bds.X + 20;
						int y = bds.Y;
						object label = painter.getAttributeValue(StdAttr.LABEL);
						if (label == null || label.Equals(""))
						{
							y += bds.Height / 2;
						}
						else
						{
							y += 3 * bds.Height / 4;
						}
						Graphics g = painter.Graphics;
						for (int i = 0; i < len; i++)
						{
							string s = data.get(len - 1 - i).toHexString();
							GraphicsUtil.drawCenteredText(g, s, x, y);
							x += 10;
						}
					}
				}
				else
				{
					Bounds bds = painter.Bounds;
					int x = bds.X + bds.Width / 2;
					int y = bds.Y;
					int h = bds.Height;
					Graphics g = painter.Graphics;
					object label = painter.getAttributeValue(StdAttr.LABEL);
					if (label == null || label.Equals(""))
					{
						string a = Strings.get("shiftRegisterLabel1");
						GraphicsUtil.drawCenteredText(g, a, x, y + h / 4);
					}
					string b = Strings.get("shiftRegisterLabel2", "" + len, "" + wid);
					GraphicsUtil.drawCenteredText(g, b, x, y + 3 * h / 4);
				}
			}

			// draw input and output ports
			int ports = painter.getInstance().getPorts().Count;
			for (int i = 0; i < ports; i++)
			{
				if (i != CK)
				{
					painter.drawPort(i);
				}
			}
			painter.drawClock(CK, Direction.East);
		}
	}
}
