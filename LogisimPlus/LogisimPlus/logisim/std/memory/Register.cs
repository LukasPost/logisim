// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using logisim.data;
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
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
	using StringUtil = logisim.util.StringUtil;

	public class Register : InstanceFactory
	{
		private const int DELAY = 8;
		private const int OUT = 0;
		private const int IN = 1;
		private const int CK = 2;
		private const int CLR = 3;
		private const int EN = 4;

		public Register() : base("Register", Strings.getter("registerComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.Width, StdAttr.TRIGGER, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {BitWidth.create(8), StdAttr.TRIG_RISING, "", StdAttr.DEFAULT_LABEL_FONT});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
			OffsetBounds = Bounds.create(-30, -20, 30, 40);
			IconName = "register.gif";
			InstancePoker = typeof(RegisterPoker);
			InstanceLogger = typeof(RegisterLogger);

			Port[] ps = new Port[5];
			ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.Width);
			ps[IN] = new Port(-30, 0, Port.INPUT, StdAttr.Width);
			ps[CK] = new Port(-20, 20, Port.INPUT, 1);
			ps[CLR] = new Port(-10, 20, Port.INPUT, 1);
			ps[EN] = new Port(-30, 10, Port.INPUT, 1);
			ps[OUT].ToolTip = Strings.getter("registerQTip");
			ps[IN].ToolTip = Strings.getter("registerDTip");
			ps[CK].ToolTip = Strings.getter("registerClkTip");
			ps[CLR].ToolTip = Strings.getter("registerClrTip");
			ps[EN].ToolTip = Strings.getter("registerEnableTip");
			setPorts(ps);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			Bounds bds = instance.Bounds;
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, bds.X + bds.Width / 2, bds.Y - 3, JGraphicsUtil.H_CENTER, JGraphicsUtil.V_BASELINE);
		}

		public override void propagate(InstanceState state)
		{
			RegisterData data = (RegisterData) state.Data;
			if (data == null)
			{
				data = new RegisterData();
				state.Data = data;
			}

			BitWidth dataWidth = state.getAttributeValue(StdAttr.Width);
			object triggerType = state.getAttributeValue(StdAttr.TRIGGER);
			bool triggered = data.updateClock(state.getPort(CK), triggerType);

			if (state.getPort(CLR) == Value.TRUE)
			{
				data.value = 0;
			}
			else if (triggered && state.getPort(EN) != Value.FALSE)
			{
				Value @in = state.getPort(IN);
				if (@in.FullyDefined)
				{
					data.value = @in.toIntValue();
				}
			}

			state.setPort(OUT, Value.createKnown(dataWidth, data.value), DELAY);
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") @Override public void paintInstance(logisim.instance.InstancePainter painter)
		public override void paintInstance(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			Bounds bds = painter.Bounds;
			RegisterData state = (RegisterData) painter.Data;
			BitWidth widthVal = painter.getAttributeValue(StdAttr.Width);
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
				a = Strings.get("registerLabel");
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
			g.setColor(Color.Gray);
			painter.drawPort(CLR, "0", Direction.South);
			painter.drawPort(EN, Strings.get("memEnableLabel"), Direction.East);
			g.setColor(Color.Black);
			painter.drawClock(CK, Direction.North);

			// draw contents
			if (string.ReferenceEquals(b, null))
			{
				JGraphicsUtil.drawText(g, a, bds.X + 15, bds.Y + 4, JGraphicsUtil.H_CENTER, JGraphicsUtil.V_TOP);
			}
			else
			{
				JGraphicsUtil.drawText(g, a, bds.X + 15, bds.Y + 3, JGraphicsUtil.H_CENTER, JGraphicsUtil.V_TOP);
				JGraphicsUtil.drawText(g, b, bds.X + 15, bds.Y + 15, JGraphicsUtil.H_CENTER, JGraphicsUtil.V_TOP);
			}
		}
	}
}
